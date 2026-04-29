using System;
using System.Linq;
using System.Web.UI;
using BEL.DTOs.Bitacora;
using Business.Services.Bitacora;

namespace UI
{
    public partial class Bitacora : Page
    {
        private static readonly BitacoraService BitacoraService = new BitacoraService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarEventos();
            }
        }

        private void CargarEventos()
        {
            var eventos = BitacoraService.ObtenerRecientes(200)
                .Select(x => new BitacoraFila
                {
                    FechaUtc = x.FechaUtc,
                    TipoEvento = x.TipoEvento,
                    Modulo = x.Modulo,
                    Accion = x.Accion,
                    Resultado = x.Resultado,
                    Usuario = string.IsNullOrWhiteSpace(x.UsuarioEmail) ? "Sistema" : x.UsuarioEmail,
                    Mensaje = x.Mensaje,
                    Url = x.Url
                })
                .ToList();

            Eventos.DataSource = eventos;
            Eventos.DataBind();

            EventosMobile.DataSource = eventos;
            EventosMobile.DataBind();
        }
        private int? IntentarObtenerIdUsuario()
        {
            object idSesion = Session?["IdUsuario"] ?? Session?["UsuarioId"];
            if (idSesion == null)
            {
                return null;
            }

            int idConvertido;
            return int.TryParse(idSesion.ToString(), out idConvertido) ? (int?)idConvertido : null;
        }

        private string IntentarObtenerUsuarioEmail()
        {
            object emailSesion = Session?["UsuarioEmail"] ?? Session?["Email"];
            if (emailSesion != null && !string.IsNullOrWhiteSpace(emailSesion.ToString()))
            {
                return emailSesion.ToString();
            }

            if (User?.Identity != null && User.Identity.IsAuthenticated)
            {
                return User.Identity.Name;
            }

            return null;
        }

        private sealed class BitacoraFila
        {
            public DateTime FechaUtc { get; set; }
            public string TipoEvento { get; set; }
            public string Modulo { get; set; }
            public string Accion { get; set; }
            public string Resultado { get; set; }
            public string Usuario { get; set; }
            public string Mensaje { get; set; }
            public string Url { get; set; }
        }
    }
}
