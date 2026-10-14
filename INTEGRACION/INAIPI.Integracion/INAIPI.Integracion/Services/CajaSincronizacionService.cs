using INAIPI.Integracion.Data;
using INAIPI.Integracion.Models;
using log4net;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace INAIPI.Integracion.Services
{
    public class CajaSincronizacionService
    {
        private static readonly ILog Log=LogManager.GetLogger(typeof(CajaSincronizacionService));
        public async Task<RespuestaSND> SincronizarLoteAsync(LoteSincronizacionDTO lote)
        {
            if(lote==null || lote.Transacciones==null || lote.Transacciones.Count==0)
                return Rechazar("El lote de transacciones está vacío.");
            if(string.IsNullOrWhiteSpace(lote.TerminalId) || lote.TerminalId.Length>100)
                return Rechazar("TerminalId es obligatorio y admite hasta 100 caracteres.");
            if(lote.Transacciones.Any(t=>t==null))
                return Rechazar("El lote contiene una transacción nula.");
            if(lote.Transacciones.Any(t=>t.TransaccionGuid==Guid.Empty))
                return Rechazar("Cada transacción debe contener un TransaccionGuid válido.");
            if(lote.Transacciones.Any(t=>t.Monto<0))
                return Rechazar("El monto de una transacción no puede ser negativo.");
            if(lote.Transacciones.GroupBy(t=>t.TransaccionGuid).Any(g=>g.Count()>1))
                return Rechazar("El lote contiene TransaccionGuid repetidos.");
            Log.InfoFormat("Lote recibido con {0} transacciones.",lote.Transacciones.Count);
            try
            {
                await new PendientesRepository().GuardarLoteAsync(lote);
                Log.InfoFormat("Lote almacenado: {0} transacciones recibidas.",lote.Transacciones.Count);
            }
            catch(Exception ex)
            {
                Log.Error("No se pudo almacenar el lote.",ex);
                return RespuestaSND.Error(503,"No se pudo guardar el lote. No se garantiza su recepción; conserve los datos y reintente.");
            }
            return new RespuestaSND
            {
                Codigo=202,
                Mensaje="Lote almacenado. El procesador revisará las transacciones pendientes.",
                Datos=new { TotalRecibido=lote.Transacciones.Count, TerminalId=lote.TerminalId }
            };
        }
        private static RespuestaSND Rechazar(string mensaje)
        {
            Log.Warn(mensaje);
            return RespuestaSND.Error(400,mensaje);
        }
    }
}
