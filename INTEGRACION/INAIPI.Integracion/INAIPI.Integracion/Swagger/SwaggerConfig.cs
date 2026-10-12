using System.Web.Http;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(
    typeof(INAIPI.Integracion.Swagger.SwaggerConfig),
    "Register")]

namespace INAIPI.Integracion.Swagger
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var enableSwagger = System.Configuration.ConfigurationManager
                .AppSettings["EnableSwagger"];

            if (string.Equals(
                enableSwagger,
                "true",
                System.StringComparison.OrdinalIgnoreCase))
            {
                GlobalConfiguration.Configuration
                    .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "INAIPI Integracion - SND");
                        c.DescribeAllEnumsAsStrings();
                    })
                    .EnableSwaggerUi();
            }
        }
    }
}
