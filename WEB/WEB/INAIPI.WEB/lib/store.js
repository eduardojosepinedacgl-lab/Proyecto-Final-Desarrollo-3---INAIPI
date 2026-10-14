import { DatabaseSync } from 'node:sqlite';
import { mkdirSync } from 'node:fs';
import { join } from 'node:path';
import { createHash } from 'node:crypto';
import { fail } from './validation.js';
export const hash = value => createHash('sha256').update(value).digest('hex');
const decode = row => row ? {...row,payload:JSON.parse(row.payload),result:row.result ? JSON.parse(row.result) : null} : null;
export class Store {
  constructor(directory) {
    mkdirSync(directory,{recursive:true,mode:0o700});
    this.db = new DatabaseSync(join(directory,'inaipi-web.sqlite'));
    this.db.exec(`PRAGMA journal_mode=WAL; PRAGMA synchronous=FULL; PRAGMA busy_timeout=5000; PRAGMA foreign_keys=ON;
      CREATE TABLE IF NOT EXISTS sessions(token TEXT PRIMARY KEY, realm TEXT NOT NULL, user TEXT NOT NULL, csrf TEXT NOT NULL, expires INTEGER NOT NULL);
      CREATE TABLE IF NOT EXISTS cache(user_id INTEGER PRIMARY KEY, data TEXT NOT NULL, updated TEXT NOT NULL);
      CREATE TABLE IF NOT EXISTS public_centers(id INTEGER PRIMARY KEY, data TEXT NOT NULL);
      CREATE TABLE IF NOT EXISTS operations(seq INTEGER PRIMARY KEY AUTOINCREMENT, id TEXT UNIQUE NOT NULL, kind TEXT NOT NULL, actor INTEGER NOT NULL, username TEXT NOT NULL, center INTEGER NOT NULL, payload TEXT NOT NULL, digest TEXT NOT NULL, input_digest TEXT NOT NULL, status TEXT NOT NULL DEFAULT 'pending', created TEXT NOT NULL, updated TEXT NOT NULL, attempts INTEGER NOT NULL DEFAULT 0, next_at INTEGER NOT NULL DEFAULT 0, lease_until INTEGER NOT NULL DEFAULT 0, error TEXT, result TEXT, fiscal_key TEXT UNIQUE);
      CREATE TABLE IF NOT EXISTS events(id INTEGER PRIMARY KEY AUTOINCREMENT, operation_id TEXT NOT NULL, actor INTEGER NOT NULL, center INTEGER NOT NULL, kind TEXT NOT NULL, detail TEXT NOT NULL, created TEXT NOT NULL);
      CREATE TABLE IF NOT EXISTS clients(id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT NOT NULL, email TEXT NOT NULL UNIQUE, phone TEXT NOT NULL, password TEXT NOT NULL, created TEXT NOT NULL);
      CREATE TABLE IF NOT EXISTS requests(seq INTEGER PRIMARY KEY AUTOINCREMENT, id TEXT UNIQUE NOT NULL, client_id INTEGER NOT NULL REFERENCES clients(id), kind TEXT NOT NULL, center INTEGER NOT NULL, payload TEXT NOT NULL, input_digest TEXT NOT NULL, status TEXT NOT NULL DEFAULT 'submitted', operation_id TEXT REFERENCES operations(id), note TEXT, reviewer TEXT, created TEXT NOT NULL, updated TEXT NOT NULL);
      CREATE TABLE IF NOT EXISTS request_events(id INTEGER PRIMARY KEY AUTOINCREMENT, request_id TEXT NOT NULL REFERENCES requests(id), kind TEXT NOT NULL, detail TEXT NOT NULL, created TEXT NOT NULL);
      CREATE INDEX IF NOT EXISTS ix_outbox_due ON operations(status,next_at);
      CREATE INDEX IF NOT EXISTS ix_requests_client ON requests(client_id);
    `);
  }
  tx(fn) { this.db.exec('BEGIN IMMEDIATE'); try { const result=fn(); this.db.exec('COMMIT'); return result; } catch(error) { this.db.exec('ROLLBACK'); throw error; } }
  session(token,realm) { if(!token) return null; const row=this.db.prepare('SELECT * FROM sessions WHERE token=? AND realm=? AND expires>?').get(hash(token),realm,Date.now()); return row ? {...row,user:JSON.parse(row.user)} : null; }
  saveSession(token,realm,user,csrf,expires) { this.db.prepare('DELETE FROM sessions WHERE expires<=?').run(Date.now()); this.db.prepare('INSERT INTO sessions VALUES(?,?,?,?,?)').run(hash(token),realm,JSON.stringify(user),csrf,expires); }
  logout(token) { if(token) this.db.prepare('DELETE FROM sessions WHERE token=?').run(hash(token)); }
  cache(user) { const r=this.db.prepare('SELECT * FROM cache WHERE user_id=?').get(user); return r ? {...JSON.parse(r.data),updated:r.updated} : {centros:[],beneficiarios:[],updated:null}; }
  saveCache(user,data,allCenters=false) { this.tx(()=> { this.db.prepare('INSERT OR REPLACE INTO cache VALUES(?,?,?)').run(user,JSON.stringify(data),new Date().toISOString()); if(allCenters) this.db.exec('DELETE FROM public_centers'); for(const c of data.centros) this.db.prepare('INSERT OR REPLACE INTO public_centers VALUES(?,?)').run(c.CentroId,JSON.stringify({CentroId:c.CentroId,NombreCentro:c.NombreCentro,TipoCentro:c.TipoCentro,Direccion:c.Direccion||'',Telefono:c.Telefono||''})); }); }
  centers() { return this.db.prepare('SELECT data FROM public_centers ORDER BY id').all().map(r=>JSON.parse(r.data)); }
  event(op,kind,detail) { this.db.prepare('INSERT INTO events(operation_id,actor,center,kind,detail,created) VALUES(?,?,?,?,?,?)').run(op.id,op.actor,op.center,kind,detail,new Date().toISOString()); }
  get(id) { return decode(this.db.prepare('SELECT * FROM operations WHERE id=?').get(id)); }
  list(center=null) { return this.db.prepare('SELECT * FROM operations WHERE (? IS NULL OR center=?) ORDER BY seq DESC').all(center,center).map(decode); }
  enqueue(id,kind,payload,user,inputDigest,inside=false) {
    const write=()=> {
      const digest=hash(JSON.stringify({kind,payload,actor:user.UsuarioId})); const old=this.get(id);
      if(old) { if(old.actor!==user.UsuarioId || old.input_digest!==inputDigest) fail(409,'Este identificador ya se utilizó con otros datos.'); return old; }
      const fiscalKey=payload.Fiscal ? `${payload.Fiscal.RncEmisor}:${payload.Fiscal.Ncf}` : null;
      if(fiscalKey && this.db.prepare('SELECT 1 FROM operations WHERE fiscal_key=?').get(fiscalKey)) fail(409,'El NCF ya está registrado en la WEB.');
      const now=new Date().toISOString();
      this.db.prepare('INSERT INTO operations(id,kind,actor,username,center,payload,digest,input_digest,created,updated,fiscal_key) VALUES(?,?,?,?,?,?,?,?,?,?,?)').run(id,kind,user.UsuarioId,user.Username,payload.CentroId,JSON.stringify(payload),digest,inputDigest,now,now,fiscalKey);
      const op=this.get(id); this.event(op,'guardada','Guardada en WEB; pendiente de confirmación del CORE.'); return op;
    };
    return inside ? write() : this.tx(write);
  }
  claim(timeout) { return this.tx(()=> { const now=Date.now(); const op=decode(this.db.prepare("SELECT * FROM operations WHERE (status='pending' AND next_at<=?) OR (status='processing' AND lease_until<=?) ORDER BY seq LIMIT 1").get(now,now)); if(!op) return null; this.db.prepare("UPDATE operations SET status='processing',attempts=attempts+1,lease_until=?,updated=? WHERE id=?").run(now+timeout+10000,new Date().toISOString(),op.id); return this.get(op.id); }); }
  finish(op,status,error,result=null,nextAt=0) { this.tx(()=> { const r=this.db.prepare("UPDATE operations SET status=?,error=?,result=?,next_at=?,lease_until=0,updated=? WHERE id=? AND status='processing' AND attempts=?").run(status,error,result ? JSON.stringify(result) : null,nextAt,new Date().toISOString(),op.id,op.attempts); if(r.changes) this.event(op,status,error||'Confirmada por CORE.'); }); }
  retry(op) { if(!['pending','review'].includes(op.status)) fail(409,'Solo se pueden reintentar operaciones pendientes o en revisión.'); this.tx(()=> {this.db.prepare("UPDATE operations SET status='pending',next_at=0,updated=? WHERE id=?").run(new Date().toISOString(),op.id);this.event(op,'reintento','Reintento solicitado por el personal.');}); }
  events(center=null) { return this.db.prepare('SELECT * FROM events WHERE (? IS NULL OR center=?) ORDER BY id DESC LIMIT 2000').all(center,center); }
  operationEvents(id) { return this.db.prepare('SELECT * FROM events WHERE operation_id=? ORDER BY id').all(id); }
  client(email) { return this.db.prepare('SELECT * FROM clients WHERE email=?').get(email); }
  createClient(name,email,phone,password) { try { const r=this.db.prepare('INSERT INTO clients(name,email,phone,password,created) VALUES(?,?,?,?,?)').run(name,email,phone,password,new Date().toISOString()); return {id:Number(r.lastInsertRowid),name,email,phone}; } catch(e) { if(e.code?.startsWith('ERR_SQLITE_CONSTRAINT') || /UNIQUE/.test(e.message)) fail(409,'Ya existe una cuenta con ese correo. Inicie sesión.'); throw e; } }
  request(id) { const r=this.db.prepare('SELECT r.*, c.name AS client_name,c.email AS client_email,c.phone AS client_phone FROM requests r JOIN clients c ON c.id=r.client_id WHERE r.id=?').get(id); return r ? {...r,payload:JSON.parse(r.payload),operation:r.operation_id ? this.get(r.operation_id) : null} : null; }
  requests(client=null,center=null) { return this.db.prepare('SELECT id FROM requests WHERE (? IS NULL OR client_id=?) AND (? IS NULL OR center=?) ORDER BY seq DESC').all(client,client,center,center).map(r=>this.request(r.id)); }
  requestEvent(id,kind,detail) { this.db.prepare('INSERT INTO request_events(request_id,kind,detail,created) VALUES(?,?,?,?)').run(id,kind,detail,new Date().toISOString()); }
  requestEvents(id) { return this.db.prepare('SELECT kind,detail,created FROM request_events WHERE request_id=? ORDER BY id').all(id); }
  submitRequest(id,kind,payload,client,digest) { return this.tx(()=> { const old=this.request(id); if(old) { if(old.client_id!==client.id || old.input_digest!==digest) fail(409,'Identificador ya utilizado.'); return old; } const now=new Date().toISOString(); this.db.prepare('INSERT INTO requests(id,client_id,kind,center,payload,input_digest,created,updated) VALUES(?,?,?,?,?,?,?,?)').run(id,client.id,kind,payload.CentroId,JSON.stringify(payload),digest,now,now); this.requestEvent(id,'recibida','Solicitud recibida; pendiente de revisión del centro.'); return this.request(id); }); }
  reviewRequest(id,decision,note,user,kind,payload,digest) {
    return this.tx(()=> {
      const r=this.request(id);
      if(r.status!=='submitted') {
        if(decision==='approve' && r.operation && r.operation.input_digest===digest && r.operation.actor===user.UsuarioId) return r;
        if(decision==='reject' && r.status==='rejected' && r.note===note) return r;
        fail(409,'La solicitud ya fue revisada.');
      }
      if(decision==='approve') this.enqueue(id,kind,payload,user,digest,true);
      this.db.prepare('UPDATE requests SET status=?,operation_id=?,note=?,reviewer=?,updated=? WHERE id=?').run(decision==='approve'?'approved':'rejected',decision==='approve'?id:null,note,user.Username,new Date().toISOString(),id);
      this.requestEvent(id,decision==='approve'?'aprobada':'rechazada',note || 'Revisada por el personal del centro.'); return this.request(id);
    });
  }
  close() { this.db.close(); }
}
