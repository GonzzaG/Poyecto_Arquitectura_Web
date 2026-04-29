using System;
using System.Web.UI;
using BEL.DTOs.Bitacora;
using Business.Services.Bitacora;

namespace UI
{
    public partial class Login : Page
    {
        private static readonly BitacoraService BitacoraService = new BitacoraService();

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
        }
    }
}
