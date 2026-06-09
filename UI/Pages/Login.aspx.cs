using System;
using System.Net;
using System.Web;
using System.Web.UI;
using BEL;
using BEL.DTOs.Bitacora;
using Business.Services.Bitacora;
using Business.Services.Usuarios;
using System.Text.Json;
using BEL.Exceptions;

namespace UI
{
    public partial class Login : BasePage
    {
        private static readonly BitacoraService BitacoraService = new BitacoraService();
        private readonly UsuarioService UsuarioService = new UsuarioService();

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void SubmitLogin_Click(object sender, EventArgs e)
        {
            BitacoraService.RegistrarAccion(new BitacoraRegistroDto
            {
                FechaUtc = DateTime.UtcNow,
                Modulo = "Autenticacion",
                Accion = "IntentoLogin",
                Resultado = "OK",
                Mensaje = "Intento de inicio de sesión desde formulario.",
                UsuarioEmail = string.IsNullOrWhiteSpace(Email.Text) ? null : Email.Text.Trim(),
                Url = Request?.RawUrl,
                Ip = Request?.UserHostAddress
            });
            try
            {
                Cookie session = UsuarioService.Login(Email.Text.Trim(), Password.Text);
                string json = JsonSerializer.Serialize(session);
                HttpCookie authCookie = new HttpCookie("AuthToken", json)
                {
                    HttpOnly = true,
                    Secure = Request.IsSecureConnection,
                    Expires = session.Expires,
                    Path = string.IsNullOrWhiteSpace(Request.ApplicationPath) ? "/" : Request.ApplicationPath
                };
                HttpContext.Current.Response.Cookies.Clear();
                Response.Cookies.Add(authCookie);

                GuardarUsuarioEnSesion(Email.Text.Trim());
                ShowSuccess("Se inició sesión correctamente");

                BitacoraService.RegistrarAccion(new BitacoraRegistroDto
                {
                    FechaUtc = DateTime.UtcNow,
                    Modulo = "Autenticacion",
                    Accion = "LoginExitoso",
                    Resultado = "OK",
                    Mensaje = "Inicio de sesión exitoso.",
                    UsuarioEmail = Email.Text.Trim(),
                    Url = Request?.RawUrl,
                    Ip = Request?.UserHostAddress
                });

                Password.Text = string.Empty;
                SubmitLogin.Enabled = false;
                LblLoginExitoso.Text = "Ingreso correcto. Te estamos redirigiendo al inicio...";
                PanelLoginExitoso.Visible = true;

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "loginExitosoRedirect",
                    "setTimeout(function(){ window.location.href = '" + ResolveUrl("~/") + "'; }, 900);",
                    true);
            }
            catch(AppException ex)
            {
                ShowError(ex.Message, "Ocurrió un error");
            }
            catch (InvalidOperationException ex)
            {
                BitacoraService.RegistrarAccion(new BitacoraRegistroDto
                {
                    FechaUtc = DateTime.UtcNow,
                    Modulo = "Autenticacion",
                    Accion = "IntentoLogin",
                    Resultado = "BLOQUEADO",
                    Mensaje = ex.Message,
                    UsuarioEmail = string.IsNullOrWhiteSpace(Email.Text) ? null : Email.Text.Trim(),
                    Url = Request?.RawUrl,
                    Ip = Request?.UserHostAddress
                });

                ShowError(ex.Message, "Ocurrió un error");
            }
            catch (Exception ex)
            {
                BitacoraService.RegistrarAccion(new BitacoraRegistroDto
                {
                    FechaUtc = DateTime.UtcNow,
                    Modulo = "Autenticacion",
                    Accion = "IntentoLogin",
                    Resultado = "ERROR",
                    Mensaje = $"Error al intentar iniciar sesión: {ex.Message}",
                    UsuarioEmail = string.IsNullOrWhiteSpace(Email.Text) ? null : Email.Text.Trim(),
                    Url = Request?.RawUrl,
                    Ip = Request?.UserHostAddress,
                    Exception = ex
                });

                ShowError("Ocurrió un error al intentar iniciar sesión. Por favor, inténtelo de nuevo más tarde.", "Ocurrió un error");

            }
        }

        private void GuardarUsuarioEnSesion(string email)
        {
            Usuario usuario = UsuarioService.ObtenerUsuario(email);
            if (usuario == null)
            {
                return;
            }

            Session["IdUsuario"] = usuario.IdUsuario;
            Session["UsuarioId"] = usuario.IdUsuario;
            Session["UsuarioEmail"] = usuario.Email;
            Session["Email"] = usuario.Email;
            Session["UsuarioNombre"] = string.IsNullOrWhiteSpace(usuario.Nombre) ? usuario.Email : usuario.Nombre;
        }
    }
}
