using System;
using log4net;

namespace INAIPI.Integracion.Helpers
{
    public static class SNDLogger
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SNDLogger));

        public static void Informacion(string mensaje)
        {
            logger.Info(mensaje);
        }

        public static void Advertencia(string mensaje)
        {
            logger.Warn(mensaje);
        }

        public static void Error(string mensaje, Exception excepcion = null)
        {
            if (excepcion == null)
            {
                logger.Error(mensaje);
            }
            else
            {
                logger.Error(mensaje, excepcion);
            }
        }
    }
}
