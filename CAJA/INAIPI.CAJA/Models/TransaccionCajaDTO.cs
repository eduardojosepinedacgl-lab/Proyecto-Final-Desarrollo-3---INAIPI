using System;

namespace INAIPI.CAJA.Models
{
    public class TransaccionCajaDTO
    {
        public Guid TransaccionGuid { get; set; }
        public int BeneficiarioId { get; set; }
        public string DescripcionServicio { get; set; }
        public decimal Importe { get; set; }
        public DateTime Fecha { get; set; }

        public TransaccionCajaDTO()
        {
            // Generamos el GUID único para la idempotencia al instanciar
            TransaccionGuid = Guid.NewGuid();
            Fecha = DateTime.Now;
        }
    }
}