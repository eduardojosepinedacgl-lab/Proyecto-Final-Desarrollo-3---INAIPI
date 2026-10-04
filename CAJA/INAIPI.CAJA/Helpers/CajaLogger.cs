using System;
using log4net;

namespace INAIPI.CAJA.Helpers
{
    public static class CajaLogger
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CajaLogger));

        public static void Info(string mensaje)
        {
            log.Info(mensaje);
        }

        public static void Error(string mensaje, Exception ex = null)
        {
            if (ex == null)
                log.Error(mensaje);
            else
                log.Error(mensaje, ex);
        }
    }
}