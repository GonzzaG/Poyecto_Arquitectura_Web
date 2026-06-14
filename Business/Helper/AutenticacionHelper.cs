using BEL;
using BEL.Constantes;
using Business.Services.Usuarios;
using System.Net;
using System.Text.Json;
using System.Web;

namespace Business.Helper
{
    public static class AutenticacionHelper
    {
        public static int NivelAcceso(RolesEnum rol)
        {
            switch (rol)
            {
                case RolesEnum.CLIENTE: return 1;
                case RolesEnum.ADM_OPERACIONES: return 2;
                case RolesEnum.WEBMASTER: return 3;
                default: return 0;
            }
        }

        public static Usuario ObtenerUsuarioDesdeCookie(UsuarioService usuarioService)
        {
            HttpCookie httpCookie = HttpContext.Current.Request.Cookies["AuthToken"];
            if (httpCookie == null || string.IsNullOrEmpty(httpCookie.Value))
                return null;

            try
            {
                Cookie cookie = JsonSerializer.Deserialize<Cookie>(httpCookie.Value);
                return usuarioService.ObtenerUsuarioAutenticado(cookie);
            }
            catch
            {
                return null;
            }
        }
    }
}
