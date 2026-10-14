using INAIPI.Integracion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;

namespace INAIPI.Integracion.Services
{
    public class CajaSincronizacionService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CajaSincronizacionService));

        public async Task<RespuestaSND> SincronizarLoteAsync(LoteSincronizacionDTO lote)
        {
            if (lote == null || lote.Transacciones == null || lote.Transacciones.Count == 0)
            {
                Log.Warn("Lote rechazado: no contiene transacciones.");
                return RespuestaSND.Error(400, "El lote de transacciones está vacío.");
            }
            Log.InfoFormat("Lote recibido con {0} transacciones.", lote.Transacciones.Count);
            bool existeGuidInvalido = lote.Transacciones.Any(transaccion => transaccion.TransaccionGuid == Guid.Empty);
            if (existeGuidInvalido)
            {
                Log.Warn("Lote rechazado: contiene un TransaccionGuid vacio.");
                return RespuestaSND.Error(400, "Cada transacción debe contener un TransaccionGuid válido.");
            }
            bool existeMontoInvalido = lote.Transacciones.Any(transaccion => transaccion.Monto < 0);
            if (existeMontoInvalido)
            {
                Log.Warn("Lote rechazado: contiene un monto negativo.");
                return RespuestaSND.Error(400, "El monto de una transacción no puede ser negativo.");
            }
            List<Guid> guidsRepetidos = lote.Transacciones
                .GroupBy(transaccion => transaccion.TransaccionGuid)
                .Where(grupo => grupo.Count() > 1)
                .Select(grupo => grupo.Key).ToList();
            if (guidsRepetidos.Count > 0)
            {
                Log.WarnFormat("Lote rechazado: contiene {0} GUID repetidos.", guidsRepetidos.Count);
                return RespuestaSND.Error(400, "El lote contiene TransaccionGuid repetidos.");
            }
            ResilienciaService resilienciaService = new ResilienciaService();
            List<object> resultados = new List<object>();
            bool todasPersistidas = true;
            int totalPersistidas = 0;
            foreach (TransaccionCajaDTO transaccion in lote.Transacciones)
            {
                Log.InfoFormat("Procesando transaccion {0}.", transaccion.TransaccionGuid);
                SolicitudAtencionCoreDTO solicitudCore = new SolicitudAtencionCoreDTO
                {
                    TransaccionGuid = transaccion.TransaccionGuid,
                    BeneficiarioId = transaccion.BeneficiarioId,
                    CentroId = transaccion.CentroId,
                    Monto = transaccion.Monto,
                    DescripcionServicio = transaccion.Descripcion,
                    FechaRegistro = transaccion.FechaRegistro
                };
                RespuestaSND respuestaCore = await resilienciaService.EnviarConReintentosAsync(solicitudCore);
                bool persistida = respuestaCore.Codigo == 200;
                if (!persistida)
                {
                    todasPersistidas = false;
                    Log.WarnFormat("Transaccion {0}: sin confirmacion de persistencia, codigo {1}.", transaccion.TransaccionGuid, respuestaCore.Codigo);
                }
                else
                {
                    totalPersistidas++;
                    Log.InfoFormat("Transaccion {0}: persistencia confirmada por CORE.", transaccion.TransaccionGuid);
                }
                resultados.Add(new
                {
                    TransaccionGuid = transaccion.TransaccionGuid,
                    Estado = persistida ? "PersistidoEnCore" : "Pendiente",
                    CodigoCore = respuestaCore.Codigo,
                    Mensaje = persistida ? "Transacción persistida correctamente en CORE." : respuestaCore.Mensaje
                });
            }
            Log.InfoFormat("Lote finalizado. Recibidas: {0}. Persistidas: {1}. Sin confirmacion: {2}.", lote.Transacciones.Count, totalPersistidas, lote.Transacciones.Count - totalPersistidas);
            if (!todasPersistidas)
            {
                Log.Warn("La sincronizacion del lote no se completo totalmente.");
                return RespuestaSND.Error(503, "CORE no está disponible; una o más transacciones siguen pendientes.");
            }
            return RespuestaSND.Exito("Lote sincronizado correctamente con CORE.", new
            {
                TotalRecibido = lote.Transacciones.Count,
                Estado = "PersistidoEnCore",
                TerminalId = lote.TerminalId,
                Resultados = resultados
            });
        }
    }
}
