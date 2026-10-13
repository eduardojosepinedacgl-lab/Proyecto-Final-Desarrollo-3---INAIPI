using System;

namespace INAIPI.CAJA.Models
{
    public class TransaccionCajaDTO
    {
        public Guid TransaccionGuid { get; set; }
        public int BeneficiarioId { get; set; }
        public int CentroId { get; set; }
        public int ComponenteId { get; set; }
        public int UsuarioRegistroId { get; set; }
        public string DescripcionServicio { get; set; }
        public decimal Monto { get; set; }            
        public DateTime FechaRegistro { get; set; }   
        public string Observaciones { get; set; }     

        public TransaccionCajaDTO()
        {
            TransaccionGuid = Guid.NewGuid();
            FechaRegistro = DateTime.Now;
        }
    }
}