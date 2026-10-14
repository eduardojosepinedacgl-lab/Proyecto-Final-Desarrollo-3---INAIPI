import http from 'node:http';
import { readFile } from 'node:fs/promises';
import { randomBytes, scrypt as scryptCallback, timingSafeEqual } from 'node:crypto';
import { promisify } from 'node:util';
import { fileURLToPath } from 'node:url';
import { resolve, dirname } from 'node:path';
import { Store, hash } from './lib/store.js';
import { CoreClient, worker } from './lib/core.js';
import { fail, required, integer, uuid, scope, restrict, isAdmin, validate, enrollment, day, today } from './lib/validation.js';
import { csv, labels, statuses, operationReport, requestReport, summaryReport } from './lib/reports.js';
const scrypt=promisify(scryptCallback), root=dirname(fileURLToPath(import.meta.url));
async function passwordHash(password,salt=randomBytes(16).toString('hex')) { return salt+':'+(await scrypt(password,salt,64)).toString('hex'); }
async function verifyPassword(password,encoded) { const computed=await passwordHash(password,encoded.split(':')[0]);return timingSafeEqual(Buffer.from(computed),Buffer.from(encoded)); }
async function body(req) {let raw='';for await(const chunk of req){raw+=chunk;if(Buffer.byteLength(raw)>32768)fail(413,'Solicitud demasiado grande.');}try{const d=JSON.parse(raw);if(!d||typeof d!=='object'||Array.isArray(d))throw Error();return d;}catch{fail(400,'JSON inválido.');}}
const publicClient=c=>({id:c.id,name:c.name,email:c.email,phone:c.phone});
function publicRequest(r) {return {id:r.id,seq:r.seq,kind:r.kind,center:r.center,payload:r.payload,status:r.operation?.status||r.status,note:r.note,created:r.created,updated:r.updated,beneficiaryId:r.kind==='inscription'&&r.operation?.status==='confirmed'?r.operation.result.ResultadoId:null,receipt:r.operation?.status==='confirmed'&&r.kind==='attention'?{...r.operation.payload}:null};}
export function createApp({dataDir=resolve(root,process.env.DATA_DIR||'data'),coreUrl=process.env.CORE_API_URL||'https://localhost:44333/',key=process.env.WEB_API_KEY||'',timeout=Number(process.env.CORE_TIMEOUT_MS||8000),interval=Number(process.env.SYNC_INTERVAL_MS||5000),baseDelay=5000,sessionMs=Number(process.env.SESSION_HOURS||12)*3600000,secure=process.env.COOKIE_SECURE==='true'}={}) {
  const store=new Store(dataDir),core=new CoreClient(coreUrl,key,timeout),sync=worker(store,core,{interval,baseDelay}),attempts=new Map();
  const assets=new Map([['/','index.html'],['/familias','familias.html'],['/familias/','familias.html'],...['app.js','familias.js','shared.js','styles.css','familias.css','print.css','print.js'].map(x=>['/'+x,x])]);
  const respond=(res,status,data)=>{res.writeHead(status,{'Content-Type':'application/json; charset=utf-8'});res.end(JSON.stringify(data));};
  function sendFile(res,content,type='text/html',filename) {res.writeHead(200,{'Content-Type':type+'; charset=utf-8',...(filename?{'Content-Disposition':`attachment; filename="${filename}"`}:{})});res.end(content);}
  function rate(req,realm) {const now=Date.now();for(const[k,v]of attempts)if(v.until<now)attempts.delete(k);const key=realm+req.socket.remoteAddress;const a=attempts.get(key)||{count:0,until:now+60000};if(a.count>=10)fail(429,'Demasiados intentos. Espere un minuto.');a.count++;attempts.set(key,a);}
  function issue(res,realm,user,oldToken) {store.logout(oldToken);const token=randomBytes(32).toString('hex'),csrf=randomBytes(32).toString('hex');store.saveSession(token,realm,user,csrf,Date.now()+sessionMs);res.setHeader('Set-Cookie',`inaipi_${realm}=${token}; HttpOnly; SameSite=Strict; Path=/; Max-Age=${Math.floor(sessionMs/1000)}${secure?'; Secure':''}`);return {user,csrf};}
  async function refresh(user) {const catalog=await core.call(`api/web/catalogo?usuarioId=${user.UsuarioId}`);if(!Array.isArray(catalog.centros)||!Array.isArray(catalog.beneficiarios))fail(502,'Catálogo CORE inválido.');const center=scope(user);if(catalog.centros.some(c=>center!==null&&c.CentroId!==center)||catalog.beneficiarios.some(b=>center!==null&&b.CentroId!==center))fail(502,'El catálogo no respeta el centro asignado.');store.saveCache(user.UsuarioId,catalog,isAdmin(user));return store.cache(user.UsuarioId);}
  const server=http.createServer(async(req,res)=>{
    res.setHeader('Cache-Control','no-store');res.setHeader('X-Content-Type-Options','nosniff');res.setHeader('Referrer-Policy','no-referrer');res.setHeader('Content-Security-Policy',"default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self' https: data:; connect-src 'self'; frame-ancestors 'none'; base-uri 'none'; form-action 'self'");
    try {
      const url=new URL(req.url,'http://localhost'),path=url.pathname,post=req.method==='POST';
      if(req.method==='GET'&&assets.has(path)){const file=assets.get(path);sendFile(res,await readFile(resolve(root,'public',file)),file.endsWith('.js')?'text/javascript':file.endsWith('.css')?'text/css':'text/html');return;}
      if(!path.startsWith('/api/')&&!path.startsWith('/reports/')&&!path.startsWith('/familias/reports/'))fail(404,'Recurso no encontrado.');
      if(req.method!=='GET') {if(req.headers.origin&&new URL(req.headers.origin).host!==req.headers.host||req.headers['sec-fetch-site']==='cross-site')fail(403,'Origen no permitido.');if(!req.headers['content-type']?.startsWith('application/json'))fail(415,'Se requiere application/json.');}
      const realm=path.startsWith('/api/client/')||path.startsWith('/familias/reports/')?'client':'staff';
      const token=new RegExp(`(?:^|;\\s*)inaipi_${realm}=([a-f0-9]{64})(?:;|$)`).exec(req.headers.cookie||'')?.[1];
      const session=store.session(token,realm);
      if(path==='/api/client/centers'&&req.method==='GET'){respond(res,200,store.centers());return;}
      if((path==='/api/login'||path==='/api/client/login'||path==='/api/client/register')&&post) {
        rate(req,realm);const d=await body(req);
        if(realm==='staff') {
          const username=required(d.Username,50,'Usuario'),password=required(d.Password,256,'Contraseña');
          const user=await core.call('api/seguridad/login','POST',{Username:username,PasswordHash:hash(password)});integer(user.UsuarioId,'Usuario');scope(user);
          const result=issue(res,realm,user,token);try{await refresh(user);}catch{/* Login remains usable with a cached catalogue. */}respond(res,200,result);return;
        }
        const email=required(d.email,150,'Correo').toLowerCase();if(!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email))fail(400,'Correo inválido.');const password=required(d.password,128,'Contraseña');
        let client;
        if(path.endsWith('/register')) {if(password.length<10)fail(400,'Utilice al menos 10 caracteres en la contraseña.');client=store.createClient(required(d.name,150,'Nombre completo'),email,required(d.phone,20,'Teléfono'),await passwordHash(password));}
        else {client=store.client(email);const fallback='0'.repeat(32)+':'+ '0'.repeat(128);const valid=await verifyPassword(password,client?.password||fallback);if(!client||!valid)fail(401,'Correo o contraseña incorrectos.');}
        respond(res,200,issue(res,realm,publicClient(client),token));return;
      }
      if(!session)fail(401,'Inicie sesión para continuar.');
      if(req.method!=='GET'&&req.headers['x-csrf-token']!==session.csrf)fail(403,'Token de sesión inválido.');
      if(path==='/api/session'||path==='/api/client/session'){if(req.method!=='GET')fail(405,'Método no permitido.');respond(res,200,{user:session.user,csrf:session.csrf});return;}
      if((path==='/api/logout'||path==='/api/client/logout')&&post){store.logout(token);res.setHeader('Set-Cookie',`inaipi_${realm}=; HttpOnly; SameSite=Strict; Path=/; Max-Age=0${secure?'; Secure':''}`);respond(res,200,{Mensaje:'Sesión cerrada.'});return;}
      if(realm==='client') {
        const client=session.user;
        if(path==='/api/client/state'&&req.method==='GET'){respond(res,200,{requests:store.requests(client.id).map(publicRequest),centers:store.centers()});return;}
        if(path==='/api/client/requests'&&post) {
          const d=await body(req);if(!uuid(d.id))fail(400,'Identificador inválido.');const id=d.id.toLowerCase(),digest=hash(JSON.stringify({kind:d.kind,payload:d.payload}));const old=store.request(id);
          if(old){if(old.client_id!==client.id||old.input_digest!==digest)fail(409,'Identificador ya utilizado.');respond(res,200,publicRequest(old));return;}
          let p;
          if(d.kind==='inscription')p=enrollment(d.payload||{},store.centers());
          else if(d.kind==='attention') {
            const parent=store.request(d.payload?.InscripcionId);
            if(!parent||parent.client_id!==client.id||parent.kind!=='inscription'||parent.operation?.status!=='confirmed')fail(400,'Seleccione una inscripción confirmada de su cuenta.');
            const preferred=day(d.payload.FechaPreferida,'Fecha preferida');if(preferred<today()||preferred>`${Number(today().slice(0,4))+1}-12-31`)fail(400,'Seleccione una fecha futura cercana.');
            p={InscripcionId:parent.id,BeneficiarioId:parent.operation.result.ResultadoId,CentroId:parent.center,NombreBeneficiario:`${parent.payload.Nombres} ${parent.payload.Apellidos}`,FechaPreferida:preferred,Motivo:required(d.payload.Motivo,500,'Motivo')};
          }else fail(400,'Solicitud no disponible.');
          respond(res,202,publicRequest(store.submitRequest(id,d.kind,p,client,digest)));return;
        }
        if(path.startsWith('/familias/reports/')&&req.method==='GET') {const r=store.request(path.split('/').pop());if(!r||r.client_id!==client.id)fail(404,'Solicitud no disponible.');sendFile(res,requestReport(r,store.requestEvents(r.id)));return;}
        fail(404,'Operación no disponible.');
      }
      const user=session.user,center=scope(user);
      if(path==='/api/state'&&req.method==='GET'){respond(res,200,{catalog:store.cache(user.UsuarioId),operations:store.list(center),requests:store.requests(null,center),core:core.state});return;}
      if(path==='/api/refresh'&&post){try{respond(res,200,{catalog:await refresh(user),offline:false});}catch(e){respond(res,200,{catalog:store.cache(user.UsuarioId),offline:true,Mensaje:e.message});}return;}
      if(path==='/api/operations'&&post) {
        const d=await body(req);if(!uuid(d.id))fail(400,'Identificador inválido.');const id=d.id.toLowerCase(),digest=hash(JSON.stringify({kind:d.kind,payload:d.payload})),old=store.get(id);
        if(old){restrict(user,old.center);if(old.actor!==user.UsuarioId||old.input_digest!==digest)fail(409,'Este identificador ya se utilizó con otros datos.');respond(res,200,old);return;}
        const p=validate(d.kind,d.payload,user,store.cache(user.UsuarioId));respond(res,202,store.enqueue(id,d.kind,p,user,digest));void sync.tick();return;
      }
      const retry=/^\/api\/operations\/([^/]+)\/retry$/.exec(path);
      if(retry&&post){const op=store.get(retry[1]);if(!op)fail(404,'Operación no encontrada.');restrict(user,op.center);store.retry(op);respond(res,200,store.get(op.id));void sync.tick();return;}
      const review=/^\/api\/requests\/([^/]+)\/review$/.exec(path);
      if(review&&post) {
        const r=store.request(review[1]);if(!r)fail(404,'Solicitud no encontrada.');restrict(user,r.center);const d=await body(req);if(!['approve','reject'].includes(d.decision))fail(400,'Decisión inválida.');
        const note=d.note?required(d.note,500,'Observación'):'';if(d.decision==='reject'&&!note)fail(400,'Indique el motivo del rechazo.');
        const digest=hash(JSON.stringify(d));let p,kind=r.kind==='inscription'?'beneficiario.crear':'atencion.registrar';
        if(d.decision==='approve'&&r.status==='submitted') {
          const source=r.kind==='inscription'?r.payload:{...d.attention,BeneficiarioId:r.payload.BeneficiarioId,CentroId:r.center};
          p=validate(kind,source,user,store.cache(user.UsuarioId));
        }
        const result=store.reviewRequest(r.id,d.decision,note,user,kind,p,digest);respond(res,200,result);void sync.tick();return;
      }
      if(path.startsWith('/reports/')&&req.method==='GET') {
        const name=path.split('/').pop();const requestMatch=/^request-(.+)$/.exec(name);
        if(requestMatch){const r=store.request(requestMatch[1]);if(!r)fail(404,'Solicitud no encontrada.');restrict(user,r.center);sendFile(res,requestReport(r,store.requestEvents(r.id)));return;}
        if(uuid(name)){const op=store.get(name);if(!op)fail(404,'Operación no encontrada.');restrict(user,op.center);sendFile(res,operationReport(op,store.operationEvents(op.id)));return;}
        let ops=store.list(center);const since=url.searchParams.get('desde'),until=url.searchParams.get('hasta');if(since)day(since);if(until)day(until);if(since&&until&&since>until)fail(400,'Rango de fechas invertido.');
        ops=ops.filter(o=>{const local=new Intl.DateTimeFormat('sv-SE',{timeZone:'America/Santo_Domingo'}).format(new Date(o.created));return(!since||local>=since)&&(!until||local<=until)&&(!url.searchParams.get('estado')||o.status===url.searchParams.get('estado'))&&(!url.searchParams.get('tipo')||o.kind===url.searchParams.get('tipo'));});
        if(name==='summary'){sendFile(res,summaryReport(ops));return;}
        let rows;
        if(name==='operations.csv')rows=[['Referencia','Proceso','Estado','Fecha UTC','Usuario','Centro','CORE ID','Base','ITBIS','Total'],...ops.map(o=>[o.id,labels[o.kind],statuses[o.status],o.created,o.username,o.center,o.result?.ResultadoId,(o.payload.BaseCentavos||0)/100,(o.payload.ItbisCentavos||0)/100,((o.payload.BaseCentavos||0)+(o.payload.ItbisCentavos||0))/100])];
        else if(name==='fiscal.csv')rows=[['Referencia','Estado','Fecha UTC','NCF','RNC emisor','Emisor','Receptor','Documento receptor','Base','ITBIS','Total'],...ops.filter(o=>o.payload.Fiscal).map(o=>[o.id,statuses[o.status],o.created,o.payload.Fiscal.Ncf,o.payload.Fiscal.RncEmisor,o.payload.Fiscal.Emisor,o.payload.Fiscal.Receptor,o.payload.Fiscal.DocumentoReceptor,o.payload.BaseCentavos/100,o.payload.ItbisCentavos/100,(o.payload.BaseCentavos+o.payload.ItbisCentavos)/100])];
        else if(name==='audit.csv')rows=[['Referencia','Actor','Centro','Evento','Detalle','Fecha UTC'],...store.events(center).map(e=>[e.operation_id,e.actor,e.center,e.kind,e.detail,e.created])];
        else if(name==='requests.csv')rows=[['Referencia','Cliente','Proceso','Estado','Fecha UTC','Respuesta'],...store.requests(null,center).map(r=>[r.id,r.client_name,labels[r.kind],statuses[r.operation?.status||r.status],r.created,r.note])];
        else fail(404,'Reporte no disponible.');sendFile(res,csv(rows),'text/csv',name);return;
      }
      fail(404,'Operación no disponible.');
    }catch(e){respond(res,e.status||500,{Mensaje:e.status?e.message:'No se pudo completar la operación. Revise el registro del servidor.'});if(!e.status)console.error(e);}
  });
  return {server,store,core,sync,async close(){await sync.stop();await new Promise(resolve=>{if(server.listening){server.close(resolve);server.closeIdleConnections();}else resolve();});store.close();}};
}
if(process.argv[1]===fileURLToPath(import.meta.url)) {
  const app=createApp(),host=process.env.HOST||'127.0.0.1',port=Number(process.env.PORT||3000);
  app.server.on('error',e=>{console.error(e.code==='EADDRINUSE'?`El puerto ${port} ya está ocupado. Cierre la otra WEB o cambie PORT.`:e.message);process.exitCode=1;});
  app.server.listen(port,host,()=>{console.log(`Personal: http://${host}:${port}\nClientes: http://${host}:${port}/familias`);void app.sync.tick();});
  for(const signal of ['SIGINT','SIGTERM'])process.once(signal,async()=>{await app.close();process.exit(0);});
}
