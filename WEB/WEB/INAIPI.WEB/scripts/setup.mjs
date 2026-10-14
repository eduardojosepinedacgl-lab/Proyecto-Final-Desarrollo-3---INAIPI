import { readFile, writeFile, copyFile, mkdir, access } from 'node:fs/promises';
import { randomBytes } from 'node:crypto';
import { dirname, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';
const web=resolve(dirname(fileURLToPath(import.meta.url)),'..'),core=resolve(web,'../../CORE/INAIPI.CORE/INAIPI.Core.Services');
const exists=async path=>{try{await access(path);return true;}catch{return false;}};
const envPath=resolve(web,'.env'),configPath=resolve(core,'Web.config'),projectPath=resolve(core,'INAIPI.Core.Services.csproj');
if(!await exists(envPath))await copyFile(resolve(web,'.env.example'),envPath);
let env=await readFile(envPath,'utf8');
let key=/^WEB_API_KEY=(.*)$/m.exec(env)?.[1].trim();
if(!/^[a-f0-9]{64}$/i.test(key||'')){key=randomBytes(32).toString('hex');env=/^WEB_API_KEY=.*$/m.test(env)?env.replace(/^WEB_API_KEY=.*$/m,`WEB_API_KEY=${key}`):env+`\nWEB_API_KEY=${key}\n`;await writeFile(envPath,env,{mode:0o600});}
await mkdir(resolve(web,'data'),{recursive:true,mode:0o700});
if(!await exists(configPath)||!await exists(projectPath)||!await exists(resolve(core,'Controllers/WebSyncController.cs'))){console.error('Se creó .env, pero falta la extensión CORE. Copie las carpetas WEB y CORE del ZIP en la raíz del repositorio completo, conservando su estructura, y vuelva a ejecutar npm run setup.');process.exitCode=1;}
else {
 let config=await readFile(configPath,'utf8');const setting=`<add key="WebApiKey" value="${key}" />`;
 const match=/<add\s+key=["']WebApiKey["'][^>]*\/\s*>/;
 if(match.test(config))config=config.replace(match,setting);
 else if(config.includes('</appSettings>'))config=config.replace('</appSettings>',`  ${setting}\n  </appSettings>`);
 else config=config.replace('</configuration>',`<appSettings>${setting}</appSettings>\n</configuration>`);
 const backup=configPath+'.antes-web-v2';if(!await exists(backup))await copyFile(configPath,backup);
 await writeFile(configPath,config);
 let project=await readFile(projectPath,'utf8');
 if(!/Compile Include="Controllers\\WebSyncController\.cs"/.test(project)){
   if(!/<Compile Include=/.test(project))throw new Error('No se encontró el grupo Compile del proyecto CORE.');
   project=project.replace(/<Compile Include=/,'<Compile Include="Controllers\\WebSyncController.cs" />\n    <Compile Include=');await writeFile(projectPath,project);
 }
 console.log('Configuración lista. Se preparó .env y la misma clave privada en el CORE.\n1. Ejecute CORE/INAIPI.CORE/08_Web_Persistencia.sql en INAIPI_Core.\n2. Recompile y ejecute INAIPI.Core.Services en Visual Studio.\n3. Compruebe CORE_API_URL en .env.\n4. Ejecute npm start. Personal: / · Clientes: /familias.');
}
