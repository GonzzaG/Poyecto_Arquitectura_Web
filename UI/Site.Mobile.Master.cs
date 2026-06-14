using System;
using System.Net;
using System.Text.Json;
using System.Web;
using System.Web.UI;
using BEL;
using BEL.Constantes;
using Business.Services.Usuarios;
using System.Collections.Generic;

namespace UI
{
    public partial class Site_Mobile : MasterPage
    {
        private readonly UsuarioService _usuarioService = new UsuarioService();

        protected void Page_Load(object sender, EventArgs e)
        {
            ActualizarEstadoSesion();
        }

        protected void BtnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            if (Request.Cookies["AuthToken"] != null)
            {
                var cookie = new HttpCookie("AuthToken")
                {
                    Value = string.Empty,
                    Expires = DateTime.Now.AddDays(-1),
                    HttpOnly = true,
                    Secure = Request.IsSecureConnection,
                    Path = string.IsNullOrWhiteSpace(Request.ApplicationPath) ? "/" : Request.ApplicationPath
                };
                Response.Cookies.Add(cookie);
            }

            Response.Redirect(ResolveUrl("~/Login"), false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void ActualizarEstadoSesion()
        {
            Usuario usuario = ObtenerUsuarioAutenticado();
            RolesEnum? rol = usuario != null ? (RolesEnum?)usuario.IdRol : null;

            AplicarVisibilidadPorRol(rol);

            if (usuario != null)
            {
                string nombre = string.IsNullOrWhiteSpace(usuario.Nombre) ? usuario.Email : usuario.Nombre;
                string iniciales = ObtenerIniciales(nombre);

                // Mobile
                LblUsuarioLogueadoMobile.Text = nombre;
                LblAvatarMobile.Text = iniciales;
                LblEmailMobile.Text = usuario.Email;
            }
        }

        private string ObtenerIniciales(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return "?";
            var partes = nombre.Trim().Split(' ');
            if (partes.Length >= 2)
                return $"{partes[0][0]}{partes[1][0]}".ToUpper();
            return nombre.Substring(0, Math.Min(2, nombre.Length)).ToUpper();
        }

        private void AplicarVisibilidadPorRol(RolesEnum? rol)
        {
            // null = visible solo para usuarios no autenticados
            // RolesEnum = nivel mínimo de rol requerido
            var controles = new Dictionary<System.Web.UI.Control, RolesEnum?>
            {
                { PnlPublicLinks,       null                    },
                { PnlPublicLinksMobile, null                    },
                { PnlUserSession,       RolesEnum.CLIENTE       },
                { PnlUserSessionMobile, RolesEnum.CLIENTE       },
                { LnkBitacora,          RolesEnum.WEBMASTER     },
                { LnkBitacoraMobile,    RolesEnum.WEBMASTER     },
                { LnkBackup,            RolesEnum.WEBMASTER     },
                { LnkBackupMobile,    RolesEnum.WEBMASTER     },
            };

            foreach (var entry in controles)
            {
                entry.Key.Visible = entry.Value == null
                    ? !rol.HasValue
                    : rol.HasValue && NivelAcceso(rol.Value) >= NivelAcceso(entry.Value.Value);
            }
        }

        private static int NivelAcceso(RolesEnum rol)
        {
            switch (rol)
            {
                case RolesEnum.CLIENTE:         return 1;
                case RolesEnum.ADM_OPERACIONES: return 2;
                case RolesEnum.WEBMASTER:       return 3;
                default:                        return 0;
            }
        }

        private Usuario ObtenerUsuarioAutenticado()
        {
            HttpCookie authToken = Request.Cookies["AuthToken"];
            if (authToken == null || string.IsNullOrWhiteSpace(authToken.Value))
                return null;

            try
            {
                Cookie cookie = JsonSerializer.Deserialize<Cookie>(authToken.Value);
                if (cookie == null || string.IsNullOrWhiteSpace(cookie.Name))
                    return null;

                Usuario usuario = _usuarioService.ObtenerUsuarioAutenticado(cookie);
                if (usuario == null)
                    return null;

                Session["IdUsuario"] = usuario.IdUsuario;
                Session["UsuarioId"] = usuario.IdUsuario;
                Session["UsuarioEmail"] = usuario.Email;
                Session["Email"] = usuario.Email;
                Session["UsuarioNombre"] = string.IsNullOrWhiteSpace(usuario.Nombre) ? usuario.Email : usuario.Nombre;

                return usuario;
            }
            catch
            {
                return null;
            }
        }
    }
}
