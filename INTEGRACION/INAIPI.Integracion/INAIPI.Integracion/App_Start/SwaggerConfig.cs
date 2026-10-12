using System.Web.Http;
using WebActivatorEx;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(
    typeof(INAIPI.Integracion.App_Start.SwaggerConfig),
    "Register")]

namespace INAIPI.Integracion.App_Start
{
    public static class SwaggerConfig
    {
        public static void Register()
        {
            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion(
                        "v1",
                        "INAIPI Integracion - SND");
                })
                .EnableSwaggerUi();
        }
    }
}
