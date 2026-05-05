using System;
using System.Net;
using System.Web.UI;
using BEL.DTOs.Bitacora;
using Business.Services.Bitacora;
using Business.Services.Usuarios;

namespace UI
{
    public partial class Login : Page
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
                Request.Form.Clear(); // Limpiar el formulario para evitar que los datos queden en memoria
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
                // Mostrar mensaje de error al usuario
                WebMessageBox.Show(this, "Ocurrió un error al intentar iniciar sesión. Por favor, inténtelo de nuevo más tarde.");
            }
        }
    }
}
