using System.Web;

namespace INAIPI.Integracion
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
        }
    }
}
