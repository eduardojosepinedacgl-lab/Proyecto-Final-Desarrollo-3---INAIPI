using INAIPI.Integracion.Models;
using INAIPI.Integracion.Services;
using System.Net;
using System.Web.Http;

namespace INAIPI.Integracion.Controllers
{
    [RoutePrefix("api/caja")]
    public class GatewayCajaController : ApiController
    {
        private readonly CajaSincronizacionService sincronizacionService;

        public GatewayCajaController()
        {
            sincronizacionService = new CajaSincronizacionService();
        }

        [HttpPost]
        [Route("sincronizar")]
        public IHttpActionResult Sincronizar(LoteSincronizacionDTO lote)
        {
            RespuestaSND respuesta = sincronizacionService.ValidarLote(lote);

            return Content((HttpStatusCode)respuesta.Codigo, respuesta);
        }
    }
}
