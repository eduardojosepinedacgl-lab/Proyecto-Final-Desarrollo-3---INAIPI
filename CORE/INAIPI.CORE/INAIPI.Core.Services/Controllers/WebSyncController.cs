using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using INAIPI.Core.Services.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using log4net;

namespace INAIPI.Core.Services.Controllers
{
    // WEB is the trusted server. Its private key must never reach the browser.
    [RoutePrefix("api/web")]
    public class WebSyncController : ApiController
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(WebSyncController));
        private bool Authorized()
        {
            string expected = ConfigurationManager.AppSettings["WebApiKey"];
            IEnumerable<string> values;
            if (String.IsNullOrEmpty(expected) || expected.Length < 32 || !Request.Headers.TryGetValues("X-Web-Key", out values)) return false;
            string provided = values.FirstOrDefault();
            if (provided == null || provided.Length > 256) return false;
            using (var sha = SHA256.Create())
            {
                byte[] a = sha.ComputeHash(Encoding.UTF8.GetBytes(expected)), b = sha.ComputeHash(Encoding.UTF8.GetBytes(provided));
                int difference = 0;
                for (int i = 0; i < a.Length; i++) difference |= a[i] ^ b[i];
                return difference == 0;
            }
        }
        private HttpResponseMessage Error(int status, string message)
        { return Request.CreateResponse((HttpStatusCode)status, new { Codigo = status, Mensaje = message }); }

        [HttpGet, Route("catalogo")]
        public HttpResponseMessage Catalogo(int usuarioId)
        {
            if (!Authorized()) return Error(401, "Clave WEB no valida.");
            try
            {
                int? center = null; bool admin = false;
                var centers = new List<object>();
                using (SqlConnection connection = AccesoDatos.ObtenerConexion())
                {
                    using (var cmd = new SqlCommand("SELECT u.CentroId,r.NombreRol FROM dbo.tblUsuarios u JOIN dbo.tblRoles r ON r.RolId=u.RolId WHERE u.UsuarioId=@id AND u.Estado=1 AND r.Estado=1", connection))
                    {
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = usuarioId;
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read()) return Error(403, "Usuario no disponible.");
                            admin = String.Equals(reader.GetString(1), "Administrador", StringComparison.OrdinalIgnoreCase);
                            center = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0);
                        }
                    }
                    if (!admin && !center.HasValue) return Error(403, "Usuario sin centro asignado.");
                    if (admin) center = null;
                    using (var cmd = new SqlCommand("SELECT CentroId,CodigoCentro,NombreCentro,TipoCentro,Direccion,Telefono FROM dbo.tblCentros WHERE Estado=1 AND (@center IS NULL OR CentroId=@center) ORDER BY NombreCentro", connection))
                    {
                        cmd.Parameters.Add("@center", SqlDbType.Int).Value = (object)center ?? DBNull.Value;
                        using (var reader = cmd.ExecuteReader()) while (reader.Read()) centers.Add(new { CentroId=reader.GetInt32(0), CodigoCentro=reader.GetString(1), NombreCentro=reader.GetString(2), TipoCentro=reader.GetString(3), Direccion=reader.GetString(4), Telefono=reader.GetString(5) });
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { centros=centers, beneficiarios=new CoreRepository().ConsultarBeneficiariosPorCentro(center,null,null) });
            }
            catch (Exception ex) { Log.Error("WEB: error de catalogo",ex); return Error(503,"No se pudo consultar el catalogo CORE."); }
        }

        public class Operation
        {
            public Guid TransaccionGuid { get; set; }
            public string Tipo { get; set; }
            public int UsuarioId { get; set; }
            public JObject Datos { get; set; }
        }
        [HttpPost, Route("operaciones")]
        public HttpResponseMessage Operaciones([FromBody] Operation operation)
        {
            if (!Authorized()) return Error(401,"Clave WEB no valida.");
            if (operation == null || operation.TransaccionGuid == Guid.Empty || operation.UsuarioId < 1 || operation.Datos == null || !new[]{"beneficiario.crear","beneficiario.actualizar","atencion.registrar"}.Contains(operation.Tipo)) return Error(400,"Operacion invalida.");
            string json = operation.Datos.ToString(Formatting.None);
            if (json.Length > 16000) return Error(400,"Datos demasiado extensos.");
            try
            {
                using (SqlConnection connection = AccesoDatos.ObtenerConexion())
                using (var cmd = new SqlCommand("dbo.ppWebProcesarOperacion",connection))
                {
                    cmd.CommandType=CommandType.StoredProcedure;
                    cmd.Parameters.Add("@TransaccionGuid",SqlDbType.UniqueIdentifier).Value=operation.TransaccionGuid;
                    cmd.Parameters.Add("@Tipo",SqlDbType.NVarChar,40).Value=operation.Tipo;
                    cmd.Parameters.Add("@UsuarioId",SqlDbType.Int).Value=operation.UsuarioId;
                    cmd.Parameters.Add("@Datos",SqlDbType.NVarChar,-1).Value=json;
                    using (var reader=cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return Error(503,"CORE no devolvio confirmacion.");
                        int code=Convert.ToInt32(reader["Codigo"]);
                        string message=Convert.ToString(reader["Mensaje"]);
                        if (code != 200) return Error(code,message);
                        return Request.CreateResponse(HttpStatusCode.OK,new { Codigo=200,Mensaje=message,TransaccionGuid=operation.TransaccionGuid,ResultadoId=Convert.ToInt32(reader["ResultadoId"]),Version=reader["Version"]==DBNull.Value?(int?)null:Convert.ToInt32(reader["Version"]) });
                    }
                }
            }
            catch(Exception ex) { Log.Error("WEB: error de operacion " + operation.TransaccionGuid,ex);return Error(503,"CORE no pudo guardar la operacion. Reintente con el mismo identificador."); }
        }
    }
}
