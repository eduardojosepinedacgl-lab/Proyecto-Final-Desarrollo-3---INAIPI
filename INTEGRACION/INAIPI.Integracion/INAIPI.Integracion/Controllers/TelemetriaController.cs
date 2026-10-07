using INAIPI.Integracion.Models;
using System;
using System.Net;
using System.Web.Http;

namespace INAIPI.Integracion.Controllers
{
    [RoutePrefix("api/telemetria")]
    public class TelemetriaController : ApiController
    {
        [HttpGet]
        [Route("ping")]
        public IHttpActionResult Ping()
        {
            return Content(
                HttpStatusCode.OK,
                RespuestaSND.Exito(
                    "SND disponible.",
                    new
                    {
                        Servicio = "INAIPI.Integracion",
                        Estado = "Online",
                        FechaUtc = DateTime.UtcNow
                    }));
        }
    }
}
