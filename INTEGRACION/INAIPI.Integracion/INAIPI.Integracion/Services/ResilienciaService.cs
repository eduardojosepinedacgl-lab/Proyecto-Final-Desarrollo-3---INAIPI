using System;
using System.Net.Http;
using System.Threading.Tasks;
using INAIPI.Integracion.Models;

namespace INAIPI.Integracion.Services
{
    public class ResilienciaService
    {
        private readonly CoreTransaccionalClient coreClient;

        public ResilienciaService()
        {
            coreClient = new CoreTransaccionalClient();
        }

        public async Task<RespuestaSND> EnviarConReintentosAsync(
            SolicitudAtencionCoreDTO solicitud)
        {
            int[] esperasSegundos = { 1, 2, 3 };

            for (int intento = 0; intento <= esperasSegundos.Length; intento++)
            {
                try
                {
                    RespuestaSND respuesta =
                        await coreClient.RegistrarAtencionAsync(solicitud);

                    bool esErrorServidor = respuesta.Codigo >= 500;

                    if (!esErrorServidor)
                    {
                        return respuesta;
                    }
                }
                catch (HttpRequestException)
                {
                }
                catch (TaskCanceledException)
                {
                }

                if (intento < esperasSegundos.Length)
                {
                    await Task.Delay(
                        TimeSpan.FromSeconds(esperasSegundos[intento]));
                }
            }

            return new RespuestaSND
            {
                Codigo = 503,
                Mensaje = "CORE no está disponible; la transacción sigue pendiente."
            };
        }
    }
}
