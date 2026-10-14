import { mkdtemp, rm } from 'node:fs/promises';
import { tmpdir } from 'node:os';
import { join } from 'node:path';
import { createInterface } from 'node:readline';
import { createApp } from '../server.js';
import { mockCore, demoKey } from '../test/mock-core.js';
const dataDir=await mkdtemp(join(tmpdir(),'inaipi-demo-'));
const core=await mockCore().start();
const app=createApp({dataDir,coreUrl:core.url,key:demoKey,interval:1000,baseDelay:1000});
await new Promise(r=>app.server.listen(3001,'127.0.0.1',r));
console.log('DEMO CON DATOS FICTICIOS · No conecta al CORE real.\nPersonal: http://127.0.0.1:3001 · admin / Demo2026*\nClientes: http://127.0.0.1:3001/familias · crea tu propia cuenta\nPrimero inicia sesión como admin para publicar los centros.\nEscribe d + Enter para desconectar el CORE simulado; c + Enter para conectarlo.\nLos datos de esta demo se eliminan al salir.');
const input=createInterface({input:process.stdin,output:process.stdout});input.on('line',line=>{if(line.trim()==='d'){core.state.offline=true;console.log('CORE simulado desconectado.');}if(line.trim()==='c'){core.state.offline=false;console.log('CORE simulado conectado.');void app.sync.tick();}});
let closing=false;async function close(){if(closing)return;closing=true;input.close();await app.close();await core.close();await rm(dataDir,{recursive:true,force:true});process.exit(0);}
process.once('SIGINT',close);process.once('SIGTERM',close);
