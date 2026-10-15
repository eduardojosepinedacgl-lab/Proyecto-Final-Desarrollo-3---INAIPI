using System;
using System.Configuration;

namespace INAIPI.CAJA.Helpers
{
    public static class ReglasNegocio
    {
        /// <summary>
        /// Tasa oficial fija de ITBIS (18%) aplicada a todos los servicios en CAJA.
        /// </summary>
        public const decimal TasaItbis = 0.18m;

        public static decimal CalcularItbis(decimal subtotal)
        {
            if (subtotal <= 0m) return 0.00m;
            return Math.Round(subtotal * TasaItbis, 2, MidpointRounding.AwayFromZero);
        }

        public static decimal CalcularTotal(decimal subtotal)
        {
            if (subtotal <= 0m) return 0.00m;
            decimal subtotalRedondeado = Math.Round(subtotal, 2, MidpointRounding.AwayFromZero);
            decimal itbis = CalcularItbis(subtotalRedondeado);
            return Math.Round(subtotalRedondeado + itbis, 2, MidpointRounding.AwayFromZero);
        }

        public static int ObtenerCentroIdActual()
        {
            string valorConfig = ConfigurationManager.AppSettings["CentroId"];
            if (!string.IsNullOrWhiteSpace(valorConfig) && int.TryParse(valorConfig, out int centroId) && centroId > 0)
            {
                return centroId;
            }
            return 1;
        }
    }
}