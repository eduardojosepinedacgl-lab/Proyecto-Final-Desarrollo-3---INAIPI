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
 
    [RoutePrefix("api/beneficiarios")]

    public class BeneficiariosController : ApiController

    {

        private static readonly ILog Log = LogManager.GetLogger(typeof(BeneficiariosController));

        private readonly CoreRepository _repositorio = new CoreRepository();

        [HttpGet]
        [Route("padron")]
        public HttpResponseMessage ObtenerPadron()
        {
            // Aquí valida la llave de CAJA, igual que en el endpoint de cajeros
            try
            {
                List<BeneficiarioPadronDTO> padron = _repositorio.ObtenerPadronCaja();
                return Request.CreateResponse(HttpStatusCode.OK, padron);
            }
            catch (Exception ex)
            {
                Log.Error("Error al consultar el padrón para CAJA.", ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    new RespuestaServicio { Codigo = 500, Mensaje = "Error interno al recuperar el padrón." });
            }
        }

        [HttpGet]

        [Route("")]

        public HttpResponseMessage ObtenerPorCentro([FromUri] int? centroId = null, [FromUri] DateTime? inicio = null, [FromUri] DateTime? fin = null)

        {

            try

            {

                List<BeneficiarioConsultaDTO> resultado = _repositorio.ConsultarBeneficiariosPorCentro(centroId, inicio, fin);

                return Request.CreateResponse(HttpStatusCode.OK, resultado);

            }

            catch (Exception ex)

            {

                Log.Error("Error al consultar beneficiarios.", ex);

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new RespuestaServicio

                {

                    Codigo = 500,

                    Mensaje = "Error interno al recuperar beneficiarios.",

                    DetalleTecnico = ex.Message

                });

            }

        }


        [HttpPost]

        [Route("")]

        public HttpResponseMessage RegistrarBeneficiario([FromBody] SolicitudRegistroBeneficiarioDTO solicitud)

        {

            if (solicitud == null || solicitud.Beneficiario == null)

            {

                return Request.CreateResponse(HttpStatusCode.BadRequest, new RespuestaServicio

                {

                    Codigo = 400,

                    Mensaje = "Estructura de datos inválida.",

                    DetalleTecnico = "Cuerpo de solicitud vacío o nulo."

                });

            }


            if (string.IsNullOrWhiteSpace(solicitud.Beneficiario.Nombres) ||

            string.IsNullOrWhiteSpace(solicitud.Beneficiario.Apellidos) ||

            solicitud.Beneficiario.CentroId <= 0 ||

            string.IsNullOrWhiteSpace(solicitud.Beneficiario.DocumentoTutor) ||

            string.IsNullOrWhiteSpace(solicitud.NombresTutor) ||

            string.IsNullOrWhiteSpace(solicitud.ApellidosTutor) ||

            string.IsNullOrWhiteSpace(solicitud.TelefonoTutor) ||

            string.IsNullOrWhiteSpace(solicitud.DireccionTutor))

            {

                return Request.CreateResponse(HttpStatusCode.BadRequest, new RespuestaServicio

                {

                    Codigo = 400,

                    Mensaje = "Todos los campos del beneficiario, el centro asignado y los datos completos del tutor son obligatorios.",

                    DetalleTecnico = "Campos requeridos vacíos o nulos."

                });

            }


            try

            {

                solicitud.Beneficiario.DocumentoTutor = solicitud.Beneficiario.DocumentoTutor.Trim().Replace("-", "");


                RespuestaServicio respuesta = _repositorio.InsertarBeneficiario(

                solicitud.Beneficiario,

                solicitud.NombresTutor.Trim(),

                solicitud.ApellidosTutor.Trim(),

                solicitud.TelefonoTutor.Trim(),

                solicitud.DireccionTutor.Trim()

                );


                if (respuesta.Codigo == 200)

                {

                    Log.InfoFormat("Beneficiario registrado exitosamente. Detalle: {0}", respuesta.DetalleTecnico);

                    return Request.CreateResponse(HttpStatusCode.OK, respuesta);

                }


                if (respuesta.Codigo == 400)

                {

                    Log.WarnFormat("Validación rechazada al registrar beneficiario: {0}", respuesta.Mensaje);

                    return Request.CreateResponse(HttpStatusCode.BadRequest, respuesta);

                }


                Log.WarnFormat("Fallo al registrar beneficiario: {0}", respuesta.Mensaje);

                return Request.CreateResponse(HttpStatusCode.InternalServerError, respuesta);

            }

            catch (Exception ex)

            {

                Log.Error("Error no controlado al registrar beneficiario.", ex);

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new RespuestaServicio

                {

                    Codigo = 500,

                    Mensaje = "Excepción en el servidor CORE.",

                    DetalleTecnico = ex.Message

                });

            }

        }


        [HttpPut]

        [Route("{id:int}")]

        public HttpResponseMessage ActualizarBeneficiario(int id, [FromBody] BeneficiarioDTO dto)

        {

            if (dto == null || dto.BeneficiarioId != id)

            {

                return Request.CreateResponse(HttpStatusCode.BadRequest, new RespuestaServicio

                {

                    Codigo = 400,

                    Mensaje = "Identificador de beneficiario inconsistente.",

                    DetalleTecnico = "El parámetro de ruta no coincide con el cuerpo del mensaje."

                });

            }


            try

            {

                RespuestaServicio respuesta = _repositorio.ActualizarBeneficiario(dto);


                if (respuesta.Codigo == 200)

                {

                    Log.InfoFormat("Beneficiario ID {0} actualizado a versión {1}.", id, respuesta.DetalleTecnico);

                    return Request.CreateResponse(HttpStatusCode.OK, respuesta);

                }


                if (respuesta.Codigo == 400)

                {

                    return Request.CreateResponse(HttpStatusCode.BadRequest, respuesta);

                }


                if (respuesta.Codigo == 409)

                {

                    Log.WarnFormat("Colisión de concurrencia optimista en Beneficiario ID {0}.", id);

                    return Request.CreateResponse(HttpStatusCode.Conflict, respuesta);

                }


                if (respuesta.Codigo == 404)

                {

                    return Request.CreateResponse(HttpStatusCode.NotFound, respuesta);

                }


                return Request.CreateResponse(HttpStatusCode.InternalServerError, respuesta);

            }

            catch (Exception ex)

            {

                Log.Error(string.Format("Error al actualizar Beneficiario ID {0}.", id), ex);

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new RespuestaServicio

                {

                    Codigo = 500,

                    Mensaje = "Error interno al actualizar beneficiario.",

                    DetalleTecnico = ex.Message

                });

            }

        }

    }

}