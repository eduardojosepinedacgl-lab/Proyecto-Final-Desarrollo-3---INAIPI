using System.Collections.Generic;

namespace INAIPI.Integracion.Models
{
    public class LoteSincronizacionDTO
    {
        public string TerminalId { get; set; }

        public List<TransaccionCajaDTO> Transacciones { get; set; }

        public LoteSincronizacionDTO()
        {
            Transacciones = new List<TransaccionCajaDTO>();
        }
    }
}
