import http from 'node:http';
import { hash } from '../lib/store.js';
export const demoKey='f'.repeat(64);
export const admin={UsuarioId:1,Username:'admin',Nombres:'Administrador',Apellidos:'Demo',NombreRol:'Administrador',CentroId:1,RolId:1};
export const operator={UsuarioId:2,Username:'operador',Nombres:'Personal',Apellidos:'Demo',NombreRol:'Operador',CentroId:2,RolId:2};
export function mockCore(){
 const centers=[{CentroId:1,NombreCentro:'CAIPI Los Jardines',CodigoCentro:'CAIPI-01',TipoCentro:'CAIPI',Direccion:'Dirección de demostración',Telefono:'809-000-0001'},{CentroId:2,NombreCentro:'CAFI Villa Esperanza',CodigoCentro:'CAFI-02',TipoCentro:'CAFI',Direccion:'Dirección de demostración',Telefono:'809-000-0002'}];
 const state={offline:false,dropResponse:false,calls:0,receipts:new Map(),beneficiarios:[{BeneficiarioId:1,NombreBeneficiario:'Lucía',ApellidoBeneficiario:'Muestra',FechaNacimientoStr:'10/02/2023',EdadAnios:3,VersionRegistro:1,CentroId:1,CodigoCentro:'CAIPI-01',NombreCentro:centers[0].NombreCentro,TipoCentro:'CAIPI',TutorId:1,DocumentoTutor:'00000000001',NombreCompletoTutor:'Ana Muestra',TelefonoTutor:'809-000-0000',DireccionTutor:'Dirección de demostración',FechaIngresoStr:'01/10/2026'}]};
 const server=http.createServer(async(req,res)=>{const url=new URL(req.url,'http://localhost');const send=(status,data)=>{res.writeHead(status,{'Content-Type':'application/json'});res.end(JSON.stringify(data));};
  if(state.offline){send(503,{Codigo:503,Mensaje:'CORE de prueba apagado.'});return;}
  let raw='';for await(const c of req)raw+=c;let d=raw?JSON.parse(raw):null;
  if(url.pathname==='/api/seguridad/login'){const user=d?.Username==='admin'?admin:d?.Username==='operador'?operator:null;send(user&&d.PasswordHash===hash('Demo2026*')?200:401,user&&d.PasswordHash===hash('Demo2026*')?user:{Codigo:401,Mensaje:'Credenciales incorrectas.'});return;}
  if(req.headers['x-web-key']!==demoKey){send(401,{Codigo:401,Mensaje:'Clave incorrecta.'});return;}
  if(url.pathname==='/api/web/catalogo'){const id=Number(url.searchParams.get('usuarioId'));if(![1,2].includes(id)){send(403,{Codigo:403,Mensaje:'Usuario no disponible.'});return;}send(200,{centros:id===1?centers:centers.filter(c=>c.CentroId===2),beneficiarios:state.beneficiarios.filter(b=>id===1||b.CentroId===2)});return;}
  if(url.pathname==='/api/web/operaciones'){
   state.calls++;const digest=hash(JSON.stringify(d));const prior=state.receipts.get(d.TransaccionGuid);if(prior){send(prior.digest===digest?200:409,prior.digest===digest?prior.result:{Codigo:409,Mensaje:'GUID con otros datos.'});return;}
   const p=d.Datos;if(d.UsuarioId!==1&&p.CentroId!==2){send(403,{Codigo:403,Mensaje:'Centro no permitido.'});return;}
   let id,version=null;
   if(d.Tipo==='beneficiario.crear'){id=state.beneficiarios.length+1;version=1;const c=centers.find(c=>c.CentroId===p.CentroId);state.beneficiarios.push({BeneficiarioId:id,NombreBeneficiario:p.Nombres,ApellidoBeneficiario:p.Apellidos,FechaNacimientoStr:p.FechaNacimiento.split('-').reverse().join('/'),EdadAnios:3,VersionRegistro:1,...c,TutorId:id,DocumentoTutor:p.DocumentoTutor,NombreCompletoTutor:`${p.NombresTutor} ${p.ApellidosTutor}`,TelefonoTutor:p.TelefonoTutor,DireccionTutor:p.DireccionTutor});}
   else if(d.Tipo==='beneficiario.actualizar'){const b=state.beneficiarios.find(b=>b.BeneficiarioId===p.BeneficiarioId);if(!b||b.VersionRegistro!==p.Version){send(409,{Codigo:409,Mensaje:'Conflicto de versión.'});return;}b.NombreBeneficiario=p.Nombres;b.ApellidoBeneficiario=p.Apellidos;b.CentroId=p.CentroId;b.VersionRegistro++;id=b.BeneficiarioId;version=b.VersionRegistro;}
   else {id=1000+state.receipts.size;}
   const result={Codigo:200,Mensaje:'Confirmada.',TransaccionGuid:d.TransaccionGuid,ResultadoId:id,Version:version};state.receipts.set(d.TransaccionGuid,{digest,result});
   if(state.dropResponse){state.dropResponse=false;res.destroy();return;}send(200,result);return;
  }
  send(404,{Codigo:404});
 });
 return {server,state,centers,async start(){await new Promise(r=>server.listen(0,'127.0.0.1',r));this.url=`http://127.0.0.1:${server.address().port}/`;return this;},async close(){await new Promise(r=>{server.close(r);server.closeIdleConnections();});}};
}
