using System;
using System.Collections.Generic;
using System.Configuration;
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

        /// <summary>
        /// Precio base fijo del servicio sin impuestos.
        /// </summary>
        public decimal Subtotal { get; set; }

        /// <summary>
        /// Monto correspondiente al ITBIS fijo (18%).
        /// </summary>
        public decimal Itbis { get; set; }

        /// <summary>
        /// Total cobrado (Subtotal + Itbis). Mantiene el nombre Monto por compatibilidad con Integración y CORE.
        /// </summary>
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
            string terminalConfig = ConfigurationManager.AppSettings["TerminalId"];
            TerminalId = !string.IsNullOrWhiteSpace(terminalConfig) ? terminalConfig : Environment.MachineName;
            Transacciones = new List<TransaccionCajaDTO>();
        }
    }
}