using INAIPI.Core.Services.Data;
using INAIPI.Core.Services.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using static INAIPI.Core.Services.Models.EntidadesDTO;

namespace INAIPI.Core.Services.Controllers
{
    [RoutePrefix("api/atenciones")]
    public class AtencionesController : ApiController
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(AtencionesController));
        private readonly CoreRepository _repositorio = new CoreRepository();

        [HttpPost]
        [Route("")]
        public HttpResponseMessage RegistrarAtencion([FromBody] SolicitudAtencionDTO solicitud)
        {
            if (solicitud == null || solicitud.TransaccionGuid == Guid.Empty)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new RespuestaServicio
                {
                    Codigo = 400,
                    Mensaje = "TransaccionGuid requerido para asegurar idempotencia.",
                    DetalleTecnico = "El objeto SolicitudAtencionDTO es inválido o no posee GUID."
                });
            }

            try
            {
                RespuestaServicio respuesta = _repositorio.RegistrarAtencion(solicitud);

                if (respuesta.Codigo == 200)
                {
                    Log.InfoFormat("Atención procesada. GUID: {0} - Mensaje: {1}", solicitud.TransaccionGuid, respuesta.Mensaje);
                    return Request.CreateResponse(HttpStatusCode.OK, respuesta);
                }

                if (respuesta.Codigo == 400)
                {
                    Log.WarnFormat("Solicitud rechazada. GUID: {0} - Detalle: {1}", solicitud.TransaccionGuid, respuesta.DetalleTecnico);
                    return Request.CreateResponse(HttpStatusCode.BadRequest, respuesta);
                }

                Log.ErrorFormat("Fallo en persistencia transaccional. GUID: {0} - Detalle: {1}", solicitud.TransaccionGuid, respuesta.DetalleTecnico);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, respuesta);
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Excepción crítica al asentar atención con GUID: {0}", solicitud.TransaccionGuid), ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new RespuestaServicio
                {
                    Codigo = 500,
                    Mensaje = "Excepción en el motor de persistencia del CORE.",
                    DetalleTecnico = ex.Message
                });
            }
        }
    }
}