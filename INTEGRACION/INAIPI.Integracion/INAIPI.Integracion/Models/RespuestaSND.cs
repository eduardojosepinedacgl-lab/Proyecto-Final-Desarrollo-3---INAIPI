namespace INAIPI.Integracion.Models
{
    public class RespuestaSND
    {
        public int Codigo { get; set; }

        public string Mensaje { get; set; }

        public string DetalleTecnico { get; set; }

        public object Datos { get; set; }

        public static RespuestaSND Exito(string mensaje, object datos = null)
        {
            return new RespuestaSND
            {
                Codigo = 200,
                Mensaje = mensaje,
                Datos = datos,
                DetalleTecnico = null
            };
        }

        public static RespuestaSND Error(
            int codigo,
            string mensaje,
            string detalleTecnico = null)
        {
            return new RespuestaSND
            {
                Codigo = codigo,
                Mensaje = mensaje,
                DetalleTecnico = detalleTecnico,
                Datos = null
            };
        }
    }
}
