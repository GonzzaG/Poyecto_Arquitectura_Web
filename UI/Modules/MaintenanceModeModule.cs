using System;
using System.Net;
using System.Text.Json;
using System.Web;
using BEL.Constantes;
using Business.Helper;
using Business.Services.Usuarios;

namespace UI.Modules
{
    public sealed class MaintenanceModeModule : IHttpModule
    {
        private static readonly UsuarioService UsuarioService = new UsuarioService();

        public void Init(HttpApplication context)
        {
            context.BeginRequest += OnBeginRequest;
        }

        public void Dispose()
        {
        }

        private static void OnBeginRequest(object sender, EventArgs e)
        {
            var application = sender as HttpApplication;
            if (application == null)
            {
                return;
            }

            var context = application.Context;
            if (!EstaEnMantenimiento(context) || EsRutaPermitida(context.Request))
            {
                return;
            }

            if (EsWebmaster(context))
            {
                return;
            }

            context.Response.Redirect("~/Pages/Mantenimiento.aspx", false);
            context.ApplicationInstance.CompleteRequest();
        }

        private static bool EstaEnMantenimiento(HttpContext context)
        {
            var valor = context?.Application?["IntegrityMaintenance"];
            return valor is bool && (bool)valor;
        }

        private static bool EsRutaPermitida(HttpRequest request)
        {
            string path = request.AppRelativeCurrentExecutionFilePath ?? string.Empty;

            return path.Equals("~/Pages/Mantenimiento.aspx", StringComparison.OrdinalIgnoreCase)
                || path.Equals("~/Pages/Login.aspx", StringComparison.OrdinalIgnoreCase)
                || path.Equals("~/Login", StringComparison.OrdinalIgnoreCase)
                || path.StartsWith("~/Content/", StringComparison.OrdinalIgnoreCase)
                || path.StartsWith("~/Scripts/", StringComparison.OrdinalIgnoreCase)
                || path.StartsWith("~/Images/", StringComparison.OrdinalIgnoreCase)
                || path.Equals("~/favicon.ico", StringComparison.OrdinalIgnoreCase);
        }

        private static bool EsWebmaster(HttpContext context)
        {
            try
            {
                HttpCookie httpCookie = context.Request.Cookies["AuthToken"];
                if (httpCookie == null || string.IsNullOrWhiteSpace(httpCookie.Value))
                {
                    return false;
                }

                Cookie cookie = JsonSerializer.Deserialize<Cookie>(httpCookie.Value);
                var usuario = UsuarioService.ObtenerUsuarioAutenticado(cookie);
                if (usuario == null)
                {
                    return false;
                }

                return AutenticacionHelper.NivelAcceso((RolesEnum)usuario.IdRol)
                    >= AutenticacionHelper.NivelAcceso(RolesEnum.WEBMASTER);
            }
            catch
            {
                return false;
            }
        }
    }
}
