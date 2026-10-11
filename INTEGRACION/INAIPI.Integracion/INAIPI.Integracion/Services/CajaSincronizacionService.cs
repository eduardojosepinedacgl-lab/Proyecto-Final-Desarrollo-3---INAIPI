using INAIPI.Integracion.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace INAIPI.Integracion.Services
{
    public class CajaSincronizacionService
    {
        public async Task<RespuestaSND> SincronizarLoteAsync(
            LoteSincronizacionDTO lote)
        {
            if (lote == null ||
                lote.Transacciones == null ||
                lote.Transacciones.Count == 0)
            {
                return RespuestaSND.Error(
                    400,
                    "El lote de transacciones está vacío.");
            }

            bool existeGuidInvalido = lote.Transacciones.Any(
                transaccion => transaccion.TransaccionGuid == Guid.Empty);

            if (existeGuidInvalido)
            {
                return RespuestaSND.Error(
                    400,
                    "Cada transacción debe contener un TransaccionGuid válido.");
            }

            bool existeMontoInvalido = lote.Transacciones.Any(
                transaccion => transaccion.Monto < 0);

            if (existeMontoInvalido)
            {
                return RespuestaSND.Error(
                    400,
                    "El monto de una transacción no puede ser negativo.");
            }

            List<Guid> guidsRepetidos = lote.Transacciones
                .GroupBy(transaccion => transaccion.TransaccionGuid)
                .Where(grupo => grupo.Count() > 1)
                .Select(grupo => grupo.Key)
                .ToList();

            if (guidsRepetidos.Count > 0)
            {
                return RespuestaSND.Error(
                    400,
                    "El lote contiene TransaccionGuid repetidos.");
            }

            ResilienciaService resilienciaService = new ResilienciaService();

            List<object> resultados = new List<object>();
            bool todasPersistidas = true;

            foreach (TransaccionCajaDTO transaccion in lote.Transacciones)
            {
                SolicitudAtencionCoreDTO solicitudCore =
                    new SolicitudAtencionCoreDTO
                    {
                        TransaccionGuid = transaccion.TransaccionGuid,
                        BeneficiarioId = transaccion.BeneficiarioId,
                        CentroId = transaccion.CentroId,
                        Monto = transaccion.Monto,
                        DescripcionServicio = transaccion.Descripcion,
                        FechaRegistro = transaccion.FechaRegistro
                    };

                RespuestaSND respuestaCore =
                    await resilienciaService.EnviarConReintentosAsync(
                        solicitudCore);

                bool persistida = respuestaCore.Codigo == 200;

                if (!persistida)
                {
                    todasPersistidas = false;
                }

                resultados.Add(new
                {
                    TransaccionGuid = transaccion.TransaccionGuid,
                    Estado = persistida
                        ? "PersistidoEnCore"
                        : "Pendiente",
                    CodigoCore = respuestaCore.Codigo,
                    Mensaje = persistida
                        ? "Transacción persistida correctamente en CORE."
                        : respuestaCore.Mensaje
                });
            }

            if (!todasPersistidas)
            {
                return RespuestaSND.Error(
                    503,
                    "CORE no está disponible; una o más transacciones siguen pendientes.");
            }

            return RespuestaSND.Exito(
                "Lote sincronizado correctamente con CORE.",
                new
                {
                    TotalRecibido = lote.Transacciones.Count,
                    Estado = "PersistidoEnCore",
                    TerminalId = lote.TerminalId,
                    Resultados = resultados
                });
        }
    }
}
