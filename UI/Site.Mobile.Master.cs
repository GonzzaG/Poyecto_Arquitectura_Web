using System;
using System.Net;
using System.Text.Json;
using System.Web;
using System.Web.UI;
using BEL;
using Business.Services.Usuarios;

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
            string nombreUsuario = ObtenerNombreUsuarioLogueado();
            bool autenticado = !string.IsNullOrWhiteSpace(nombreUsuario);

            PnlPublicLinks.Visible = !autenticado;
            PnlPublicLinksMobile.Visible = !autenticado;
            PnlUserSession.Visible = autenticado;
            PnlUserSessionMobile.Visible = autenticado;

            if (autenticado)
            {
                LblUsuarioLogueado.Text = nombreUsuario;
                LblUsuarioLogueadoMobile.Text = nombreUsuario;
            }
        }

        private string ObtenerNombreUsuarioLogueado()
        {
            string nombreSesion = Session["UsuarioNombre"] as string;
            if (!string.IsNullOrWhiteSpace(nombreSesion))
            {
                return nombreSesion;
            }

            HttpCookie authToken = Request.Cookies["AuthToken"];
            if (authToken == null || string.IsNullOrWhiteSpace(authToken.Value))
            {
                return null;
            }

            try
            {
                Cookie cookie = JsonSerializer.Deserialize<Cookie>(authToken.Value);
                if (cookie == null || string.IsNullOrWhiteSpace(cookie.Name))
                {
                    return null;
                }

                Usuario usuario = _usuarioService.ObtenerUsuarioAutenticado(cookie);
                if (usuario == null)
                {
                    return null;
                }

                Session["IdUsuario"] = usuario.IdUsuario;
                Session["UsuarioId"] = usuario.IdUsuario;
                Session["UsuarioEmail"] = usuario.Email;
                Session["Email"] = usuario.Email;
                Session["UsuarioNombre"] = string.IsNullOrWhiteSpace(usuario.Nombre) ? usuario.Email : usuario.Nombre;

                return Session["UsuarioNombre"] as string;
            }
            catch
            {
                return null;
            }
        }
    }
}
