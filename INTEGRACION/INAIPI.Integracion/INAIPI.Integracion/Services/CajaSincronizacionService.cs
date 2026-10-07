using INAIPI.Integracion.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace INAIPI.Integracion.Services
{
    public class CajaSincronizacionService
    {
        public RespuestaSND ValidarLote(LoteSincronizacionDTO lote)
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

            return RespuestaSND.Exito(
                "Lote recibido y validado correctamente por el SND.",
                new
                {
                    TotalRecibido = lote.Transacciones.Count,
                    Estado = "RecibidoPorSND",
                    TerminalId = lote.TerminalId
                });
        }
    }
}
