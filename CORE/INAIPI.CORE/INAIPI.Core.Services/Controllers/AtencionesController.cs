using INAIPI.Core.Services.Data;
using INAIPI.Core.Services.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        [HttpGet]
        [Route("catalogo")]
        public HttpResponseMessage ObtenerCatalogoPrecios()
        {
            try
            {
                var catalogo = new List<object>();
                using (SqlConnection conn = AccesoDatos.ObtenerConexion())
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT ComponenteId, Codigo, NombreComponente, DivisionResponsable, PrecioFijo FROM dbo.tblComponentesAtencion ORDER BY ComponenteId ASC", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            decimal precioFijo = Convert.ToDecimal(reader["PrecioFijo"]);
                            decimal itbis = Math.Round(precioFijo * 0.18m, 2, MidpointRounding.AwayFromZero);
                            catalogo.Add(new
                            {
                                ComponenteId = Convert.ToInt32(reader["ComponenteId"]),
                                Codigo = Convert.ToString(reader["Codigo"]),
                                Nombre = Convert.ToString(reader["NombreComponente"]),
                                DivisionResponsable = Convert.ToString(reader["DivisionResponsable"]),
                                PrecioFijo = precioFijo,
                                Itbis = itbis,
                                Total = precioFijo + itbis
                            });
                        }
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, catalogo);
            }
            catch (Exception ex)
            {
                Log.Error("Error al consultar el catálogo de precios fijos.", ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new RespuestaServicio
                {
                    Codigo = 500,
                    Mensaje = "No se pudo recuperar el catálogo de servicios.",
                    DetalleTecnico = ex.Message
                });
            }
        }

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

            if (solicitud.BeneficiarioId <= 0 || solicitud.CentroId <= 0 || solicitud.Monto <= 0 || string.IsNullOrWhiteSpace(solicitud.DescripcionServicio))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new RespuestaServicio
                {
                    Codigo = 400,
                    Mensaje = "Datos de atención incompletos o monto inválido.",
                    DetalleTecnico = "BeneficiarioId, CentroId, Monto (>0) y DescripcionServicio son requeridos."
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
                    Log.WarnFormat("Solicitud rechazada por validación de negocio/precio fijo. GUID: {0} - Detalle: {1}", solicitud.TransaccionGuid, respuesta.DetalleTecnico);
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