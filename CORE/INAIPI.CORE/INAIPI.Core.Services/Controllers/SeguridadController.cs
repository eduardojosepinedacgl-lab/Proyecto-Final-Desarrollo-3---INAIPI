using INAIPI.Core.Services.Data;
using INAIPI.Core.Services.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
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

        [HttpGet]
        [Route("cajeros")]
        public HttpResponseMessage ObtenerCajerosActivos()
        {
            try
            {
                var cajeros = new List<object>();

                using (SqlConnection conn = AccesoDatos.ObtenerConexion())
                {
                    // Obtiene únicamente usuarios activos con el rol "Cajero"
                    const string query = @"
                        SELECT u.UsuarioId, u.Username, u.Nombres, u.Apellidos, u.PasswordHash, u.CentroId 
                        FROM dbo.tblUsuarios u
                        INNER JOIN dbo.tblRoles r ON u.RolId = r.RolId
                        WHERE r.NombreRol = 'Cajero' AND u.Estado = 1 AND r.Estado = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cajeros.Add(new
                                {
                                    UsuarioId = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    NombreCompleto = $"{reader.GetString(2)} {reader.GetString(3)}".Trim(),
                                    PasswordHash = reader.GetString(4), // Hash PBKDF2 o SHA256 ya generado en el servidor
                                    CentroId = reader.IsDBNull(5) ? 1 : reader.GetInt32(5)
                                });
                            }
                        }
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, cajeros);
            }
            catch (Exception ex)
            {
                Log.Error("Error al exportar catálogo de cajeros para sincronización local.", ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new RespuestaServicio
                {
                    Codigo = 500,
                    Mensaje = "Error al consultar cajeros.",
                    DetalleTecnico = ex.Message
                });
            }
        }
    }
}