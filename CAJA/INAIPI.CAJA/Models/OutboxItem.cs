using System;

namespace INAIPI.CAJA.Models
{
    public class OutboxItem
    {
        public Guid TransaccionGuid { get; set; }
        public string PayloadJson { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Sincronizado { get; set; }

        public OutboxItem()
        {
            FechaCreacion = DateTime.Now;
            Sincronizado = false;
        }
    }
}