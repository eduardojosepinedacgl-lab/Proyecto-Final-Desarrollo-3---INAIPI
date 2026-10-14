using System;
using System.Collections.Generic;
using Newtonsoft.Json;

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

        // Propiedad que espera INAIPI.Integracion en su JSON:
        public string Descripcion
        {
            get => DescripcionServicio;
            set => DescripcionServicio = value;
        }

        public decimal Monto { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Observaciones { get; set; }
        public bool Sincronizado { get; set; }

        public TransaccionCajaDTO()
        {
            TransaccionGuid = Guid.NewGuid();
            FechaRegistro = DateTime.Now;
            Sincronizado = false;
        }
    }

    public class LoteSincronizacionCajaDTO
    {
        public string TerminalId { get; set; }
        public List<TransaccionCajaDTO> Transacciones { get; set; }

        public LoteSincronizacionCajaDTO()
        {
            TerminalId = Environment.MachineName; // O "CAJA-01"
            Transacciones = new List<TransaccionCajaDTO>();
        }
    }
}