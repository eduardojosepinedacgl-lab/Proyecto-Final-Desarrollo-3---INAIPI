using System;

namespace INAIPI.Integracion.Models
{
    public class SolicitudAtencionCoreDTO
    {
        public Guid TransaccionGuid { get; set; }
        public int BeneficiarioId { get; set; }
        public int CentroId { get; set; }
        public decimal Monto { get; set; }
        public string DescripcionServicio { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
