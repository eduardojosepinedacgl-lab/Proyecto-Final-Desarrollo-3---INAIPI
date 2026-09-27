using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace INAIPI.Core.Services
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // Inicialización de Log4Net desde Log4net.config
            string rutaConfigLog = Path.Combine(Server.MapPath("~"), "Log4net.config");
            FileInfo archivoInfo = new FileInfo(rutaConfigLog);
            if (archivoInfo.Exists)
            {
                XmlConfigurator.ConfigureAndWatch(archivoInfo);
            }
        }
    }
}
