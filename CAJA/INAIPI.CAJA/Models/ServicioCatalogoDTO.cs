using INAIPI.CAJA.Helpers;

namespace INAIPI.CAJA.Models
{
    public class ServicioCatalogoDTO
    {
        public int ComponenteId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public decimal PrecioFijo { get; set; }

        public decimal Itbis => ReglasNegocio.CalcularItbis(PrecioFijo);
        public decimal Total => ReglasNegocio.CalcularTotal(PrecioFijo);

        public string DescripcionConPrecio => $"{Nombre} (RD$ {PrecioFijo:N2} + ITBIS)";
    }
}