using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using log4net;
using log4net.Config;

namespace INAIPI.Core.AdminApp.Helpers
{
    internal class LoggerHelper
    {
        private static bool _inicializado = false;

        public static void InicializarLogging()
        {
            if (_inicializado) return;

            string rutaConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log4net.config");
            FileInfo archivoConfig = new FileInfo(rutaConfig);

            if (archivoConfig.Exists)
            {
                XmlConfigurator.ConfigureAndWatch(archivoConfig);
            }
            else
            {
                BasicConfigurator.Configure();
            }

            _inicializado = true;
        }

        public static ILog ObtenerLogger(Type tipo)
        {
            if (!_inicializado)
            {
                InicializarLogging();
            }
            return LogManager.GetLogger(tipo);
        }
    }
}
