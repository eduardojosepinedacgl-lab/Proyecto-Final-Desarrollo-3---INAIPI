using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace INAIPI.Core.Services
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Habilitar enrutamiento por atributos
            config.MapHttpAttributeRoutes();

            // Ruta predeterminada
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Forzar salida en formato JSON excluyendo el formateador XML
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
        }
    }
}
