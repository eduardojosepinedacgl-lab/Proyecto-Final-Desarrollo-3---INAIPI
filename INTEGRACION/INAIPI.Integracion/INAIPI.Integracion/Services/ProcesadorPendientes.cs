using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using INAIPI.Integracion.Data;
using log4net;

namespace INAIPI.Integracion.Services
{
    public static class ProcesadorPendientes
    {
        private static readonly ILog Log=LogManager.GetLogger(typeof(ProcesadorPendientes));
        private static int iniciado;
        public static void Iniciar()
        {
            if(Interlocked.CompareExchange(ref iniciado,1,0)!=0) return;
            try { HostingEnvironment.QueueBackgroundWorkItem(EjecutarAsync); }
            catch { Interlocked.Exchange(ref iniciado,0); throw; }
        }
        private static async Task EjecutarAsync(CancellationToken token)
        {
            Log.Info("Procesador de pendientes iniciado.");
            try
            {
                while(!token.IsCancellationRequested)
                {
                    try { await ProcesarCicloAsync(token); }
                    catch(OperationCanceledException) when(token.IsCancellationRequested) { break; }
                    catch(Exception ex) { Log.Error("Error en ciclo de pendientes.",ex); }
                    await Task.Delay(TimeSpan.FromSeconds(30),token);
                }
            }
            catch(OperationCanceledException) when(token.IsCancellationRequested) { }
            finally { Interlocked.Exchange(ref iniciado,0); Log.Info("Procesador detenido."); }
        }
        private static async Task ProcesarCicloAsync(CancellationToken token)
        {
            var config=ConfigurationManager.ConnectionStrings["CnIntegracionDb"];
            if(config==null) throw new ConfigurationErrorsException("Falta CnIntegracionDb.");
            using(var c=new SqlConnection(config.ConnectionString))
            {
                await c.OpenAsync(token);
                using(var cmd=new SqlCommand(@"DECLARE @Resultado INT;
EXEC @Resultado=sys.sp_getapplock @Resource=N'INAIPI_ProcesadorPendientes',
@LockMode='Exclusive',@LockOwner='Session',@LockTimeout=0;
SELECT @Resultado;",c))
                {
                    if(Convert.ToInt32(await cmd.ExecuteScalarAsync(token))<0) return;
                }
                try
                {
                    var repo=new PendientesRepository();
                    var servicio=new ResilienciaService();
                    var guids=await repo.ObtenerPendientesAsync();
                    Log.InfoFormat("Revision de pendientes: {0} seleccionadas.",guids.Count);
                    foreach(var guid in guids)
                    {
                        token.ThrowIfCancellationRequested();
                        try
                        {
                            var solicitud=await repo.ObtenerSolicitudAsync(guid);
                            var respuesta=await servicio.EnviarConReintentosAsync(solicitud);
                            string estado=respuesta.Codigo==200 ? "Sincronizada"
                                : respuesta.Codigo>=500 || respuesta.Codigo==408 || respuesta.Codigo==429
                                    ? "Pendiente" : "Rechazada";
                            await repo.ActualizarResultadoAsync(guid,estado,estado=="Sincronizada"?null:respuesta.Mensaje);
                            Log.InfoFormat("Transaccion {0}: {1}, codigo {2}.",guid,estado,respuesta.Codigo);
                        }
                        catch(Exception ex)
                        {
                            Log.Error(string.Format("Error procesando {0}.",guid),ex);
                            await repo.ActualizarResultadoAsync(guid,"Pendiente","Error inesperado; revisar logs.");
                        }
                    }
                }
                finally
                {
                    using(var cmd=new SqlCommand(@"EXEC sys.sp_releaseapplock
@Resource=N'INAIPI_ProcesadorPendientes',@LockOwner='Session';",c))
                        await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
