using Business.Services.Bitacora;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UI
{
    public partial class Bitacora : Page
    {
        private static readonly BitacoraService BitacoraService = new BitacoraService();

        private string TerminoBusqueda
        {
            get { return (ViewState["Busqueda"] as string) ?? string.Empty; }
            set { ViewState["Busqueda"] = value; }
        }

        private int PaginaActual
        {
            get { return (int)(ViewState["Pagina"] ?? 1); }
            set { ViewState["Pagina"] = value; }
        }

        private int TamanioPagina
        {
            get { return (int)(ViewState["Tamanio"] ?? 10); }
            set { ViewState["Tamanio"] = value; }
        }

        private int TotalPaginas
        {
            get { return (int)(ViewState["TotalPaginas"] ?? 1); }
            set { ViewState["TotalPaginas"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                TxtBusqueda.Text = string.Empty;
                DdlTamanio.SelectedValue = "10";
                PaginaActual = 1;
                TamanioPagina = 10;
                CargarEventos();
            }
        }

        protected void BtnFiltrar_Click(object sender, EventArgs e)
        {
            TerminoBusqueda = TxtBusqueda.Text.Trim();
            PaginaActual = 1; 
            CargarEventos();
        }

        protected void DdlTamanio_SelectedIndexChanged(object sender, EventArgs e)
        {
            int tamanio;
            if (int.TryParse(DdlTamanio.SelectedValue, out tamanio))
            {
                TamanioPagina = tamanio;
            }
            PaginaActual = 1; 
            CargarEventos();
        }

        protected void BtnAnterior_Click(object sender, EventArgs e)
        {
            if (PaginaActual > 1)
            {
                PaginaActual--;
                CargarEventos();
            }
        }

        protected void BtnSiguiente_Click(object sender, EventArgs e)
        {
            if (PaginaActual < TotalPaginas)
            {
                PaginaActual++;
                CargarEventos();
            }
        }

        private void CargarEventos()
        {
            int totalRegistros;
            var registros = BitacoraService.ObtenerFiltrado(
                TerminoBusqueda,
                PaginaActual,
                TamanioPagina,
                out totalRegistros);

            TotalPaginas = totalRegistros == 0
                ? 1
                : (int)Math.Ceiling((double)totalRegistros / TamanioPagina);

            if (PaginaActual > TotalPaginas) PaginaActual = TotalPaginas;

            var filas = registros
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

            bool sinResultados = filas == null || !filas.Any();
            PanelSinResultados.Visible = sinResultados;

            PanelPaginacion.Visible = TotalPaginas > 1;

            Eventos.Visible = !sinResultados;
            EventosMobile.Visible = !sinResultados;

            Eventos.DataSource = filas;
            Eventos.DataBind();
            EventosMobile.DataSource = filas;
            EventosMobile.DataBind();

            BtnAnterior.Enabled = PaginaActual > 1;
            BtnSiguiente.Enabled = PaginaActual < TotalPaginas;
            LblPagina.Text = string.Format("Página {0} de {1}", PaginaActual, TotalPaginas);
            LblTotal.Text = string.Format("{0} registros encontrados", totalRegistros);
        }

        private int? IntentarObtenerIdUsuario()
        {
            object idSesion = Session?["IdUsuario"] ?? Session?["UsuarioId"];
            if (idSesion == null) return null;
            int id;
            return int.TryParse(idSesion.ToString(), out id) ? (int?)id : null;
        }

        private string IntentarObtenerUsuarioEmail()
        {
            object emailSesion = Session?["UsuarioEmail"] ?? Session?["Email"];
            if (emailSesion != null && !string.IsNullOrWhiteSpace(emailSesion.ToString()))
                return emailSesion.ToString();
            if (User?.Identity != null && User.Identity.IsAuthenticated)
                return User.Identity.Name;
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
