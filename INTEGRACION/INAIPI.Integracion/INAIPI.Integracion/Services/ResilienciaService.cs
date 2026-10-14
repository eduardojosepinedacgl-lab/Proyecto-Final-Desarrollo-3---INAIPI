using System;
using System.Net.Http;
using System.Threading.Tasks;
using INAIPI.Integracion.Models;
using log4net;

namespace INAIPI.Integracion.Services
{
    public class ResilienciaService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ResilienciaService));
        private readonly CoreTransaccionalClient coreClient;

        public ResilienciaService()
        {
            coreClient = new CoreTransaccionalClient();
        }

        public async Task<RespuestaSND> EnviarConReintentosAsync(SolicitudAtencionCoreDTO solicitud)
        {
            int[] esperasSegundos = { 1, 2, 3 };
            for (int intento = 0; intento <= esperasSegundos.Length; intento++)
            {
                Log.InfoFormat("Transaccion {0}: intento {1} de {2}.", solicitud.TransaccionGuid, intento + 1, esperasSegundos.Length + 1);
                try
                {
                    RespuestaSND respuesta = await coreClient.RegistrarAtencionAsync(solicitud);
                    bool esErrorServidor = respuesta.Codigo >= 500;
                    if (!esErrorServidor)
                    {
                        Log.InfoFormat("Transaccion {0}: envio finalizado con codigo {1}.", solicitud.TransaccionGuid, respuesta.Codigo);
                        return respuesta;
                    }
                    Log.WarnFormat("Transaccion {0}: CORE devolvio codigo {1}.", solicitud.TransaccionGuid, respuesta.Codigo);
                }
                catch (HttpRequestException ex)
                {
                    Log.Warn(string.Format("Transaccion {0}: fallo de conexion en intento {1}.", solicitud.TransaccionGuid, intento + 1), ex);
                }
                catch (TaskCanceledException ex)
                {
                    Log.Warn(string.Format("Transaccion {0}: solicitud cancelada o timeout en intento {1}.", solicitud.TransaccionGuid, intento + 1), ex);
                }
                if (intento < esperasSegundos.Length)
                {
                    Log.InfoFormat("Transaccion {0}: espera de {1} segundos antes de reintentar.", solicitud.TransaccionGuid, esperasSegundos[intento]);
                    await Task.Delay(TimeSpan.FromSeconds(esperasSegundos[intento]));
                }
            }
            Log.ErrorFormat("Transaccion {0}: intentos agotados, sin confirmacion de persistencia en CORE.", solicitud.TransaccionGuid);
            return new RespuestaSND
            {
                Codigo = 503,
                Mensaje = "CORE no está disponible; la transacción sigue pendiente."
            };
        }
    }
}
