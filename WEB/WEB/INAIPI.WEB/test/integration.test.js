import test from 'node:test';
import assert from 'node:assert/strict';
import { mkdtemp, rm, mkdir, copyFile, readFile, writeFile } from 'node:fs/promises';
import { tmpdir } from 'node:os';
import { join, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';
import { randomUUID } from 'node:crypto';
import { execFile } from 'node:child_process';
import { promisify } from 'node:util';
import { createApp } from '../server.js';
import { Store } from '../lib/store.js';
import { today } from '../lib/validation.js';
import { mockCore, demoKey, admin } from './mock-core.js';
const exec=promisify(execFile);
const enrollment=(center=1)=>({Nombres:'Valentina',Apellidos:'Prueba',FechaNacimiento:'2023-03-12',CentroId:center,DocumentoTutor:'00000000002',NombresTutor:'María',ApellidosTutor:'Prueba',TelefonoTutor:'809-000-0000',DireccionTutor:'Dirección ficticia'});
const attention=()=>({BeneficiarioId:1,CentroId:1,FechaRegistro:today(),DescripcionServicio:'Servicio de prueba',BaseCentavos:12550,ItbisCentavos:2259,Fiscal:null});
async function fixture(t){const dir=await mkdtemp(join(tmpdir(),'inaipi-test-')),core=await mockCore().start();let app,url;
 const start=async()=>{app=createApp({dataDir:dir,coreUrl:core.url,key:demoKey,interval:3600000,baseDelay:1,timeout:500});await new Promise(r=>app.server.listen(0,'127.0.0.1',r));url=`http://127.0.0.1:${app.server.address().port}`;};await start();
 const request=async(path,{method='GET',data,session,headers={}}={})=>{const r=await fetch(url+path,{method,headers:{...(data!==undefined?{'Content-Type':'application/json'}:{}),...(session?{Cookie:session.cookie,'X-CSRF-Token':session.csrf}:{}),...headers},body:data===undefined?undefined:JSON.stringify(data)});const text=await r.text();return {status:r.status,headers:r.headers,data:r.headers.get('content-type')?.includes('json')?JSON.parse(text):text};};
 const login=async(name='admin')=>{const r=await request('/api/login',{method:'POST',data:{Username:name,Password:'Demo2026*'}});assert.equal(r.status,200);return {cookie:r.headers.get('set-cookie').split(';')[0],csrf:r.data.csrf};};
 const register=async(email)=>{const r=await request('/api/client/register',{method:'POST',data:{name:'Familia Prueba',email,phone:'809-000-0000',password:'Familia2026*'}});assert.equal(r.status,200);return {cookie:r.headers.get('set-cookie').split(';')[0],csrf:r.data.csrf};};
 const settle=async()=>{await app.sync.tick();};
 t.after(async()=>{await app.close();await core.close();await rm(dir,{recursive:true,force:true});});
 return {core,dir,request,login,register,settle,get app(){return app;},restart:async()=>{await app.close();await start();}};
}
test('la cola y la sesión sobreviven reinicio; envío repetido conserva una sola inscripción',async t=>{
 const f=await fixture(t),s=await f.login(),id=randomUUID(),d={id,kind:'beneficiario.crear',payload:enrollment()};f.core.state.offline=true;
 assert.equal((await f.request('/api/operations',{method:'POST',session:s,data:d})).status,202);await f.settle();assert.equal(f.app.store.get(id).status,'pending');
 await f.restart();assert.equal((await f.request('/api/session',{session:s})).status,200);assert.equal(f.app.store.get(id).status,'pending');f.core.state.offline=false;
 f.app.store.retry(f.app.store.get(id));await f.settle();assert.equal(f.app.store.get(id).status,'confirmed');assert.equal(f.core.state.beneficiarios.length,2);
 assert.equal((await f.request('/api/operations',{method:'POST',session:s,data:d})).status,200);assert.equal((await f.request('/api/operations',{method:'POST',session:s,data:{...d,payload:{...d.payload,Nombres:'Otra'}}})).status,409);
 assert.match((await f.request('/reports/'+id,{session:s})).data,/Confirmada en CORE/);
});
test('respuesta perdida después de persistir se recupera sin duplicar el registro',async t=>{
 const f=await fixture(t),s=await f.login(),id=randomUUID();f.core.state.dropResponse=true;
 await f.request('/api/operations',{method:'POST',session:s,data:{id,kind:'beneficiario.crear',payload:enrollment()}});await f.settle();assert.equal(f.app.store.get(id).status,'pending');assert.equal(f.core.state.receipts.size,1);
 f.app.store.retry(f.app.store.get(id));await f.settle();assert.equal(f.app.store.get(id).status,'confirmed');assert.equal(f.core.state.beneficiarios.length,2);assert.equal(f.core.state.receipts.size,1);
});
test('conflictos de versión no sobrescriben y una repetición exacta funciona tras refrescar catálogo',async t=>{
 const f=await fixture(t),s=await f.login();const d={id:randomUUID(),kind:'beneficiario.actualizar',payload:{...enrollment(),BeneficiarioId:1,Version:1}};
 f.core.state.beneficiarios[0].VersionRegistro=2;await f.request('/api/operations',{method:'POST',session:s,data:d});await f.settle();assert.equal(f.app.store.get(d.id).status,'review');assert.equal(f.core.state.beneficiarios[0].NombreBeneficiario,'Lucía');
 await f.request('/api/refresh',{method:'POST',session:s,data:{}});const d2={...d,id:randomUUID(),payload:{...d.payload,Version:2}};assert.equal((await f.request('/api/operations',{method:'POST',session:s,data:d2})).status,202);await f.settle();assert.equal(f.app.store.get(d2.id).status,'confirmed');
 await f.request('/api/refresh',{method:'POST',session:s,data:{}});assert.equal((await f.request('/api/operations',{method:'POST',session:s,data:d2})).status,200);assert.equal(f.core.state.beneficiarios[0].VersionRegistro,3);
});
test('sesiones, CSRF, origen, centros, rutas de datos y permisos de reportes',async t=>{
 const f=await fixture(t),s=await f.login(),o=await f.login('operador'),d={id:randomUUID(),kind:'atencion.registrar',payload:attention()};
 assert.equal((await f.request('/api/state')).status,401);assert.equal((await f.request('/api/operations',{method:'POST',session:s,headers:{'X-CSRF-Token':'wrong'},data:d})).status,403);
 assert.equal((await f.request('/api/operations',{method:'POST',session:s,headers:{Origin:'http://evil.invalid'},data:d})).status,403);
 assert.equal((await f.request('/api/operations',{method:'POST',session:o,data:d})).status,403);
 await f.request('/api/operations',{method:'POST',session:s,data:d});await f.settle();assert.equal((await f.request('/reports/'+d.id,{session:o})).status,403);assert.deepEqual((await f.request('/api/state',{session:o})).data.operations,[]);
 for(const path of ['/data/inaipi-web.sqlite','/.env','/server.js'])assert.equal((await f.request(path,{session:s})).status,404);
 await f.request('/api/logout',{method:'POST',session:s,data:{}});assert.equal((await f.request('/api/state',{session:s})).status,401);
});
test('importes en centavos, NCF único, reportes filtrados y neutralización de HTML/CSV',async t=>{
 const f=await fixture(t),s=await f.login(),p={...attention(),DescripcionServicio:'<script>malicioso</script>',Fiscal:{Ncf:'B0200000001',RncEmisor:'000000001',Emisor:'=SUM(1,1)',Receptor:'Familia Prueba',DocumentoReceptor:'00000000002'}};
 const id=randomUUID();assert.equal((await f.request('/api/operations',{method:'POST',session:s,data:{id,kind:'atencion.registrar',payload:p}})).status,202);await f.settle();assert.equal(f.app.store.get(id).payload.BaseCentavos,12550);
 assert.equal((await f.request('/api/operations',{method:'POST',session:s,data:{id:randomUUID(),kind:'atencion.registrar',payload:p}})).status,409);
 const report=(await f.request('/reports/'+id,{session:s})).data;assert.ok(!report.includes('<script>malicioso'));assert.match(report,/&lt;script&gt;/);
 assert.match((await f.request('/reports/fiscal.csv',{session:s})).data,/'=SUM/);assert.ok(!(await f.request('/reports/operations.csv?estado=pending',{session:s})).data.includes(id));
 assert.equal((await f.request('/api/operations',{method:'POST',session:s,data:{id:randomUUID(),kind:'atencion.registrar',payload:{...attention(),BaseCentavos:12.5}}})).status,400);
});
test('familias: aislamiento, inscripción, revisión, caída del CORE, reinicio, atención y constancia',async t=>{
 const f=await fixture(t),staff=await f.login(),otherCenter=await f.login('operador'),a=await f.register('familia-a@example.test'),b=await f.register('familia-b@example.test');
 const payload={id:randomUUID(),kind:'inscription',payload:enrollment()};
 assert.equal((await f.request('/api/client/requests',{method:'POST',session:a,data:payload})).status,202);
 assert.equal((await f.request('/api/client/requests',{method:'POST',session:a,data:payload})).status,200);
 assert.equal((await f.request('/api/client/requests',{method:'POST',session:b,data:payload})).status,409);
 assert.equal((await f.request('/familias/reports/'+payload.id,{session:b})).status,404);
 assert.equal((await f.request('/api/state',{session:a})).status,401);
 assert.equal((await f.request('/api/client/state',{session:b})).data.requests.length,0);
 assert.equal((await f.request(`/api/requests/${payload.id}/review`,{method:'POST',session:otherCenter,data:{decision:'approve'}})).status,403);
 f.core.state.offline=true;assert.equal((await f.request(`/api/requests/${payload.id}/review`,{method:'POST',session:staff,data:{decision:'approve',note:'Bienvenida a nuestro centro.'}})).status,200);await f.settle();
 assert.equal((await f.request('/api/client/state',{session:a})).data.requests[0].status,'pending');await f.restart();assert.equal((await f.request('/api/client/session',{session:a})).status,200);
 f.core.state.offline=false;f.app.store.retry(f.app.store.get(payload.id));await f.settle();const own=(await f.request('/api/client/state',{session:a})).data.requests[0];assert.equal(own.status,'confirmed');assert.equal(own.beneficiaryId,2);
 assert.equal((await f.request('/api/client/requests',{method:'POST',session:b,data:{id:randomUUID(),kind:'attention',payload:{InscripcionId:payload.id,FechaPreferida:today(),Motivo:'Prueba'}}})).status,400);
 const atn={id:randomUUID(),kind:'attention',payload:{InscripcionId:payload.id,FechaPreferida:today(),Motivo:'Seguimiento familiar'}};
 assert.equal((await f.request('/api/client/requests',{method:'POST',session:a,data:atn})).status,202);await f.request('/api/refresh',{method:'POST',session:staff,data:{}});
 const approval={decision:'approve',note:'Atención completada.',attention:{...attention(),DescripcionServicio:'Seguimiento familiar'}};
 assert.equal((await f.request(`/api/requests/${atn.id}/review`,{method:'POST',session:staff,data:approval})).status,200);await f.settle();assert.equal(f.app.store.get(atn.id).payload.BeneficiarioId,2);
 assert.equal((await f.request(`/api/requests/${atn.id}/review`,{method:'POST',session:staff,data:approval})).status,200);assert.equal(f.core.state.receipts.size,2);
 assert.match((await f.request('/familias/reports/'+atn.id,{session:a})).data,/Confirmada en CORE/);assert.equal((await f.request('/reports/'+atn.id,{session:a})).status,401);
 assert.equal((await f.request(`/api/requests/${atn.id}/review`,{method:'POST',session:staff,data:{decision:'reject',note:'Cambio'}})).status,409);
 const rejected={id:randomUUID(),kind:'inscription',payload:enrollment()};await f.request('/api/client/requests',{method:'POST',session:a,data:rejected});await f.request(`/api/requests/${rejected.id}/review`,{method:'POST',session:staff,data:{decision:'reject',note:'Faltan datos del tutor.'}});assert.equal((await f.request('/api/client/state',{session:a})).data.requests[0].status,'rejected');assert.equal(f.app.store.get(rejected.id),null);
});
test('la concesión del trabajador evita doble envío y recupera trabajos interrumpidos',async t=>{
 const dir=await mkdtemp(join(tmpdir(),'inaipi-lease-')),a=new Store(dir),b=new Store(dir);t.after(async()=>{a.close();b.close();await rm(dir,{recursive:true,force:true});});const op=a.enqueue(randomUUID(),'atencion.registrar',attention(),admin,'digest');const claimed=a.claim(500);assert.equal(b.claim(500),null);a.db.prepare('UPDATE operations SET lease_until=0 WHERE id=?').run(op.id);const reclaimed=b.claim(500);assert.equal(reclaimed.attempts,2);a.finish(claimed,'confirmed',null,{ResultadoId:1});assert.equal(a.get(op.id).status,'processing');b.finish(reclaimed,'confirmed',null,{ResultadoId:1});assert.equal(a.get(op.id).status,'confirmed');
});
test('setup conserva conexión y clave, e incluye el controlador una sola vez',async t=>{
 const dir=await mkdtemp(join(tmpdir(),'inaipi-setup-'));t.after(()=>rm(dir,{recursive:true,force:true}));const web=join(dir,'WEB/INAIPI.WEB'),core=join(dir,'CORE/INAIPI.CORE/INAIPI.Core.Services');await mkdir(join(web,'scripts'),{recursive:true});await mkdir(join(core,'Controllers'),{recursive:true});await copyFile(fileURLToPath(new URL('../scripts/setup.mjs',import.meta.url)),join(web,'scripts/setup.mjs'));await copyFile(fileURLToPath(new URL('../.env.example',import.meta.url)),join(web,'.env.example'));await writeFile(join(core,'Controllers/WebSyncController.cs'),'// test');const original='<configuration><connectionStrings>CONSERVAR</connectionStrings><appSettings></appSettings></configuration>';await writeFile(join(core,'Web.config'),original);await writeFile(join(core,'INAIPI.Core.Services.csproj'),'<Project><ItemGroup><Compile Include="Controllers\\HomeController.cs" /></ItemGroup></Project>');
 await exec(process.execPath,[join(web,'scripts/setup.mjs')]);const env=await readFile(join(web,'.env'),'utf8');await exec(process.execPath,[join(web,'scripts/setup.mjs')]);assert.equal(await readFile(join(web,'.env'),'utf8'),env);assert.match(await readFile(join(core,'Web.config'),'utf8'),/CONSERVAR/);assert.equal((await readFile(join(core,'INAIPI.Core.Services.csproj'),'utf8')).match(/WebSyncController/g).length,1);assert.equal(await readFile(join(core,'Web.config.antes-web-v2'),'utf8'),original);
});
