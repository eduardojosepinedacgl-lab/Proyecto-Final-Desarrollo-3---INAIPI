import { WebError } from './validation.js';
export class CoreClient {
  constructor(url,key,timeout=8000) { this.base=new URL(url); if(!['http:','https:'].includes(this.base.protocol)) throw new Error('CORE_API_URL debe usar HTTP o HTTPS.'); this.key=key;this.timeout=timeout; this.state={online:null,checked:null,message:'Conexión todavía no comprobada.'}; }
  async call(path,method='GET',body) {
    try {
      const response=await fetch(new URL(path,this.base),{method,headers:{Accept:'application/json','X-Web-Key':this.key,...(body?{'Content-Type':'application/json'}:{})},body:body?JSON.stringify(body):undefined,signal:AbortSignal.timeout(this.timeout),redirect:'error'});
      let data; try{data=await response.json();}catch{throw new WebError(502,'El CORE no devolvió JSON válido. Compruebe su dirección.');}
      const code=Number(data?.Codigo)||response.status;
      if(!response.ok || code>=400) {
        const message=code===404?'Instale el controlador WebSync y el script SQL 08 en el CORE.':code===401 && !path.includes('login')?'La clave WEB_API_KEY no coincide con WebApiKey del CORE.':code>=500?'El CORE no puede procesar la operación. Se conservará pendiente.':data?.Mensaje||'El CORE rechazó la solicitud.';
        throw new WebError(code,message);
      }
      this.state={online:true,checked:new Date().toISOString(),message:'CORE conectado'}; return data;
    } catch(e) {
      let error=e;
      if(!e.status) { const tls=/CERT|SELF_SIGNED|ISSUER/.test(e.cause?.code||e.code||''); error=new WebError(503,tls?'Certificado del CORE no confiable. Instale su CA y arranque con npm start (--use-system-ca).':'No se pudo conectar al CORE. La operación permanece guardada.'); }
      const reachable=[400,403,409,422].includes(error.status);
      this.state={online:reachable,checked:new Date().toISOString(),message:reachable?'CORE conectado':error.message}; throw error;
    }
  }
}
export function worker(store,core,{interval=5000,baseDelay=5000}={}) {
  let running=null,stopped=false;
  async function run() {
    for(let i=0;i<20 && !stopped;i++) {
      const op=store.claim(core.timeout); if(!op) break;
      try {
        const result=await core.call('api/web/operaciones','POST',{TransaccionGuid:op.id,Tipo:op.kind,UsuarioId:op.actor,Datos:op.payload});
        if(result.Codigo!==200 || result.TransaccionGuid?.toLowerCase()!==op.id || !Number.isInteger(result.ResultadoId)) throw new WebError(502,'El CORE no confirmó el identificador de la operación.');
        store.finish(op,'confirmed',null,result);
      } catch(e) {
        const transient=e.status>=500 || [404,408,429].includes(e.status);
        store.finish(op,transient?'pending':'review',e.message,null,Date.now()+Math.min(300000,baseDelay*2**Math.min(op.attempts-1,8)));
      }
    }
  }
  const tick=()=> {if(stopped)return Promise.resolve();if(!running)running=run().catch(e=>console.error('Error de almacenamiento en sincronización:',e.message)).finally(()=>{running=null;});return running;};
  const timer=setInterval(tick,interval);timer.unref();
  return {tick,async stop(){stopped=true;clearInterval(timer);await running;}};
}
