using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using INAIPI.Integracion.Models;
using Newtonsoft.Json;

namespace INAIPI.Integracion.Services
{
    public class CoreTransaccionalClient
    {
        private readonly HttpClient httpClient;
        private readonly string coreBaseUrl;

        public CoreTransaccionalClient()
        {
            coreBaseUrl = System.Configuration.ConfigurationManager
                .AppSettings["CoreBaseUrl"];

            int timeoutSeconds = 10;

            int.TryParse(
                System.Configuration.ConfigurationManager
                    .AppSettings["RequestTimeoutSeconds"],
                out timeoutSeconds);

            if (timeoutSeconds <= 0)
            {
                timeoutSeconds = 10;
            }

            httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(timeoutSeconds)
            };
        }

        public async Task<RespuestaSND> RegistrarAtencionAsync(
            SolicitudAtencionCoreDTO solicitud)
        {
            string url = coreBaseUrl.TrimEnd('/') + "/api/atenciones";

            string json = JsonConvert.SerializeObject(solicitud);

            using (var contenido = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"))
            {
                HttpResponseMessage respuestaHttp =
                    await httpClient.PostAsync(url, contenido);

                string cuerpoRespuesta =
                    await respuestaHttp.Content.ReadAsStringAsync();

                if (!respuestaHttp.IsSuccessStatusCode)
                {
                    return new RespuestaSND
                    {
                        Codigo = (int)respuestaHttp.StatusCode,
                        Mensaje = "CORE respondió con un error.",
                        Datos = cuerpoRespuesta
                    };
                }

                RespuestaSND respuestaCore =
                    JsonConvert.DeserializeObject<RespuestaSND>(cuerpoRespuesta);

                if (respuestaCore == null)
                {
                    return new RespuestaSND
                    {
                        Codigo = 500,
                        Mensaje = "No se pudo interpretar la respuesta de CORE."
                    };
                }

                return respuestaCore;
            }
        }
    }
}
