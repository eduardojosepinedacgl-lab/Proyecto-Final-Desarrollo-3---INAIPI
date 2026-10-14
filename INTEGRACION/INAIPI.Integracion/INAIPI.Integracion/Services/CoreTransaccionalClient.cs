using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using INAIPI.Integracion.Models;
using Newtonsoft.Json;
using log4net;

namespace INAIPI.Integracion.Services
{
    public class CoreTransaccionalClient
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CoreTransaccionalClient));
        private readonly HttpClient httpClient;
        private readonly string coreBaseUrl;

        public CoreTransaccionalClient()
        {
            coreBaseUrl = System.Configuration.ConfigurationManager.AppSettings["CoreBaseUrl"];
            int timeoutSeconds = 10;
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["RequestTimeoutSeconds"], out timeoutSeconds);
            if (timeoutSeconds <= 0)
            {
                timeoutSeconds = 10;
            }
            httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(timeoutSeconds) };
        }

        public async Task<RespuestaSND> RegistrarAtencionAsync(SolicitudAtencionCoreDTO solicitud)
        {
            string url = coreBaseUrl.TrimEnd('/') + "/api/atenciones";
            string json = JsonConvert.SerializeObject(solicitud);
            Log.InfoFormat("Transaccion {0}: enviando solicitud a CORE.", solicitud.TransaccionGuid);
            using (var contenido = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                HttpResponseMessage respuestaHttp = await httpClient.PostAsync(url, contenido);
                string cuerpoRespuesta = await respuestaHttp.Content.ReadAsStringAsync();
                Log.InfoFormat("Transaccion {0}: CORE respondio HTTP {1}.", solicitud.TransaccionGuid, (int)respuestaHttp.StatusCode);
                if (!respuestaHttp.IsSuccessStatusCode)
                {
                    Log.WarnFormat("Transaccion {0}: respuesta HTTP no exitosa, codigo {1}.", solicitud.TransaccionGuid, (int)respuestaHttp.StatusCode);
                    return new RespuestaSND
                    {
                        Codigo = (int)respuestaHttp.StatusCode,
                        Mensaje = "CORE respondió con un error.",
                        Datos = cuerpoRespuesta
                    };
                }
                RespuestaSND respuestaCore;
                try
                {
                    respuestaCore = JsonConvert.DeserializeObject<RespuestaSND>(cuerpoRespuesta);
                }
                catch (JsonException ex)
                {
                    Log.Error(string.Format("Transaccion {0}: error interpretando JSON de CORE.", solicitud.TransaccionGuid), ex);
                    throw;
                }
                if (respuestaCore == null)
                {
                    Log.ErrorFormat("Transaccion {0}: respuesta de CORE interpretada como null.", solicitud.TransaccionGuid);
                    return new RespuestaSND
                    {
                        Codigo = 500,
                        Mensaje = "No se pudo interpretar la respuesta de CORE."
                    };
                }
                Log.InfoFormat("Transaccion {0}: respuesta interpretada, codigo {1}.", solicitud.TransaccionGuid, respuestaCore.Codigo);
                return respuestaCore;
            }
        }
    }
}
