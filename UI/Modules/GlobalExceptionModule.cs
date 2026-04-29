using System;
using System.Web;
using BEL.DTOs.Bitacora;
using Business.Services.Bitacora;

namespace UI.Modules
{
    public sealed class GlobalExceptionModule : IHttpModule
    {
        private static readonly BitacoraService BitacoraService = new BitacoraService();

        public void Init(HttpApplication context)
        {
            context.Error += OnApplicationError;
            context.EndRequest += OnEndRequest;
        }

        public void Dispose()
        {
        }

        private static void OnApplicationError(object sender, EventArgs e)
        {
            var application = sender as HttpApplication;
            if (application == null)
            {
                return;
            }

            var exception = application.Server.GetLastError();
            var request = application.Context?.Request;
            var currentPath = request?.AppRelativeCurrentExecutionFilePath ?? string.Empty;
            var codigoCorrelacion = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpperInvariant();

            if (currentPath.Equals("~/Pages/Error.aspx", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            try
            {
                BitacoraService.RegistrarError(new BitacoraRegistroDto
                {
                    Modulo = "HttpModule",
                    Exception = exception,
                    CodigoCorrelacion = codigoCorrelacion,
                    IdUsuario = ObtenerIdUsuario(application.Context),
                    UsuarioEmail = ObtenerUsuarioEmail(application.Context),
                    Url = request?.RawUrl,
                    Ip = request?.UserHostAddress
                });
            }
            catch
            {
            }

            try
            {
                application.Server.ClearError();
                application.Response.Redirect("~/Pages/Error.aspx?ref=" + codigoCorrelacion, false);
                application.Context.ApplicationInstance.CompleteRequest();
            }
            catch
            {
            }
        }

        private static void OnEndRequest(object sender, EventArgs e)
        {
            var application = sender as HttpApplication;
            if (application == null) return;

            // Solo interesa el 404
            if (application.Response.StatusCode != 404) return;

            // Evitar loop si ya estamos en Error.aspx
            var currentPath = application.Request?.AppRelativeCurrentExecutionFilePath ?? string.Empty;
            if (currentPath.StartsWith("~/Pages/Error.aspx", StringComparison.OrdinalIgnoreCase)) return;

            var codigoCorrelacion = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpperInvariant();

            try
            {
                BitacoraService.RegistrarError(new BitacoraRegistroDto
                {
                    Modulo = "HttpModule-404",
                    Exception = new HttpException(404, "Página no encontrada: " + application.Request?.RawUrl),
                    CodigoCorrelacion = codigoCorrelacion,
                    IdUsuario = ObtenerIdUsuario(application.Context),
                    UsuarioEmail = ObtenerUsuarioEmail(application.Context),
                    Url = application.Request?.RawUrl,
                    Ip = application.Request?.UserHostAddress
                });
            }
            catch { }

            application.Response.Redirect("~/Pages/Error.aspx?ref=" + codigoCorrelacion, false);
            application.Context.ApplicationInstance.CompleteRequest();
        }

        private static int? ObtenerIdUsuario(HttpContext context)
        {
            try
            {
                var valor = context?.Session?["IdUsuario"] ?? context?.Session?["UsuarioId"];
                if (valor == null)
                {
                    return null;
                }

                int idUsuario;
                return int.TryParse(valor.ToString(), out idUsuario) ? (int?)idUsuario : null;
            }
            catch
            {
                return null;
            }
        }

        private static string ObtenerUsuarioEmail(HttpContext context)
        {
            try
            {
                var valor = context?.Session?["UsuarioEmail"] ?? context?.Session?["Email"];
                if (valor != null && !string.IsNullOrWhiteSpace(valor.ToString()))
                {
                    return valor.ToString();
                }
            }
            catch
            {
            }

            if (context?.User?.Identity != null && context.User.Identity.IsAuthenticated)
            {
                return context.User.Identity.Name;
            }

            return null;
        }
    }
}
