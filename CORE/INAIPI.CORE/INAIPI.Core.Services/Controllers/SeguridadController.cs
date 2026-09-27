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
    [RoutePrefix("api/seguridad")]
    public class SeguridadController : ApiController
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SeguridadController));
        private readonly CoreRepository _repositorio = new CoreRepository();

        [HttpPost]
        [Route("login")]
        public HttpResponseMessage Login([FromBody] LoginRequestDTO solicitud)
        {
            if (solicitud == null || string.IsNullOrWhiteSpace(solicitud.Username) || string.IsNullOrWhiteSpace(solicitud.PasswordHash))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new RespuestaServicio
                {
                    Codigo = 400,
                    Mensaje = "Credenciales incompletas.",
                    DetalleTecnico = "Username y PasswordHash son obligatorios."
                });
            }

            try
            {
                UsuarioAutenticadoDTO usuario = _repositorio.ValidarCredenciales(solicitud.Username, solicitud.PasswordHash);
                if (usuario == null)
                {
                    Log.WarnFormat("Intento fallido de autenticación para el usuario: {0}", solicitud.Username);
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, new RespuestaServicio
                    {
                        Codigo = 401,
                        Mensaje = "Usuario o contraseña inválidos.",
                        DetalleTecnico = "Credenciales no coinciden en base de datos."
                    });
                }

                Log.InfoFormat("Autenticación exitosa para el usuario: {0} (Rol: {1})", usuario.Username, usuario.NombreRol);
                return Request.CreateResponse(HttpStatusCode.OK, usuario);
            }
            catch (Exception ex)
            {
                Log.Error("Excepción no controlada durante autenticación.", ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new RespuestaServicio
                {
                    Codigo = 500,
                    Mensaje = "Error interno en el servidor de autenticación.",
                    DetalleTecnico = ex.Message
                });
            }
        }
    }
}