using System;

namespace INAIPI.Integracion.Models
{
    public class TransaccionCajaDTO
    {
        public Guid TransaccionGuid { get; set; }

        public int BeneficiarioId { get; set; }

        public int CentroId { get; set; }

        public decimal Monto { get; set; }

        public string Descripcion { get; set; }

        public DateTime FechaRegistro { get; set; }

        public bool Sincronizado { get; set; }
    }
}
