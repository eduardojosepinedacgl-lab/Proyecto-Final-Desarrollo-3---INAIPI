using System;
using System.Collections.Generic;
using INAIPI.CAJA.Data;
using INAIPI.CAJA.Models;
using Newtonsoft.Json;

namespace INAIPI.CAJA.Services
{
    public class SincronizadorBackground
    {
        private readonly LocalRepository _repo;
        private readonly ConexionSNDClient _sndClient;

        public SincronizadorBackground()
        {
            _repo = new LocalRepository();
            _sndClient = new ConexionSNDClient();
        }

        public bool ProcesarPendientes()
        {
            try
            {
                List<OutboxItem> pendientes = _repo.ObtenerTransaccionesPendientes();

                // Si la red está activa, intentamos procesar.
                bool redActiva = true;

                foreach (var item in pendientes)
                {
                    // Reconstruimos el objeto original a partir del JSON guardado
                    var transaccion = JsonConvert.DeserializeObject<TransaccionCajaDTO>(item.PayloadJson);

                    bool enviado = _sndClient.EnviarTransaccion(transaccion);

                    if (enviado)
                    {
                        // Si el CORE lo recibió bien, lo marcamos como sincronizado
                        _repo.MarcarComoSincronizado(item.TransaccionGuid);
                    }
                    else
                    {
                        INAIPI.CAJA.Helpers.CajaLogger.Info($"SND inaccesible. Transacción {item.TransaccionGuid} retenida en Outbox.");
                        redActiva = false;
                        break;
                    }
                }

                // Para simular el estado de conexión si no hay pendientes, hacemos un ping rápido
                // Como ConexionSNDClient es un mock, llamarlo con null nos dirá si "hay red"
                if (pendientes.Count == 0)
                {
                    redActiva = _sndClient.EnviarTransaccion(new TransaccionCajaDTO());
                }

                return redActiva;
            }
            catch
            {
                // Cualquier fallo de base de datos o HTTP tumba el estado a Offline
                return false;
            }
        }
    }
}