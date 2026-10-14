using System.Web.Http;
using INAIPI.Integracion.Services;

namespace INAIPI.Integracion
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            ProcesadorPendientes.Iniciar();
        }
    }
}
