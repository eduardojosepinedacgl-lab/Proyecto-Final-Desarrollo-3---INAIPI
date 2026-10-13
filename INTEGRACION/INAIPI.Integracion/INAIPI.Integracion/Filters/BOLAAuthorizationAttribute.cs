using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using INAIPI.Integracion.Helpers;

namespace INAIPI.Integracion.Filters
{
    public class BOLAAuthorizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            ClaimsPrincipal identidad = actionContext.RequestContext.Principal as ClaimsPrincipal;

            if (identidad == null || identidad.Identity == null || !identidad.Identity.IsAuthenticated)
            {
                SNDLogger.Advertencia("Intento de acceso sin autenticación.");

                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.Unauthorized,
                    new { codigo = 401, mensaje = "Usuario no autenticado." });

                return;
            }

            string usuario = identidad.Identity.Name;

            object valorCentro = null;

            bool tieneCentro = actionContext.RequestContext.RouteData != null &&
                actionContext.RequestContext.RouteData.Values.TryGetValue("centroId", out valorCentro);

            if (!tieneCentro || valorCentro == null)
            {
                SNDLogger.Informacion("Solicitud autorizada para el usuario: " + usuario);
                return;
            }

            int centroSolicitado;

            if (!int.TryParse(valorCentro.ToString(), out centroSolicitado))
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new { codigo = 400, mensaje = "El centro indicado no es válido." });

                return;
            }

            Claim claimCentro = identidad.Claims != null
                ? identidad.Claims.FirstOrDefault(claim => claim.Type == "centroId")
                : null;

            if (claimCentro == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.Forbidden,
                    new { codigo = 403, mensaje = "El usuario no tiene un centro asignado." });

                return;
            }

            int centroUsuario;

            if (!int.TryParse(claimCentro.Value, out centroUsuario))
            {
                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.Forbidden,
                    new { codigo = 403, mensaje = "El centro asignado al usuario no es válido." });

                return;
            }

            if (centroUsuario != centroSolicitado)
            {
                SNDLogger.Advertencia("Intento de acceso BOLA detectado. Usuario: " + usuario + ". Centro solicitado: " + centroSolicitado + ". Centro asignado: " + centroUsuario + ".");

                actionContext.Response = actionContext.Request.CreateResponse(
                    HttpStatusCode.Forbidden,
                    new { codigo = 403, mensaje = "No tiene permiso para acceder a este centro." });
            }
        }
    }
}
