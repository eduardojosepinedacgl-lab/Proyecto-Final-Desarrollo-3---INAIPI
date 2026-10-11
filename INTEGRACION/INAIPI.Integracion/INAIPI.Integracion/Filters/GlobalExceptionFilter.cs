using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using INAIPI.Integracion.Helpers;

namespace INAIPI.Integracion.Filters
{
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            Exception excepcion = context.Exception;

            SNDLogger.Error("Error no controlado en el SND: " + excepcion.Message, excepcion);

            context.Response = context.Request.CreateResponse(
                HttpStatusCode.InternalServerError,
                new
                {
                    codigo = 500,
                    mensaje = "Ocurrió un error interno en el SND.",
                    fechaUtc = DateTime.UtcNow
                });
        }
    }
}
