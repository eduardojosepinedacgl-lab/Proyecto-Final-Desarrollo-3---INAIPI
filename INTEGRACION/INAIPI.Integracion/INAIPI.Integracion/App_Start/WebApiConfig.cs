using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Web.Http;
using INAIPI.Integracion.Filters;

namespace INAIPI.Integracion
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Filters.Add(new GlobalExceptionFilter());

            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}
