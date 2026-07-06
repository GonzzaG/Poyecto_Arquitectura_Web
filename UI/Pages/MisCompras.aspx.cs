using BEL;
using BEL.Constantes;
using Business.Services.Pedido;
using System;
using System.Linq;
using System.Web;

namespace UI.Pages
{
    public partial class MisCompras : BasePage
    {
        private readonly PedidoService _pedidoService = new PedidoService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!TieneAcceso())
            {
                Response.Redirect("~/", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (!IsPostBack)
            {
                CargarMisCompras();
            }
        }

        private bool TieneAcceso()
        {
            try
            {
                return new VerificadorAcceso().VerificarAcceso(RolesEnum.CLIENTE);
            }
            catch
            {
                return false;
            }
        }

        private void CargarMisCompras()
        {
            var idUsuario = ObtenerIdUsuario();
            var compras = _pedidoService.ObtenerFinalizadosPorUsuario(idUsuario)
                .Select(MapearCompra)
                .ToList();

            var sinResultados = !compras.Any();
            PanelSinResultados.Visible = sinResultados;
            RptMisCompras.Visible = !sinResultados;
            RptMisComprasMobile.Visible = !sinResultados;
            LblTotal.Text = $"{compras.Count} compras confirmadas";

            RptMisCompras.DataSource = compras;
            RptMisCompras.DataBind();
            RptMisComprasMobile.DataSource = compras;
            RptMisComprasMobile.DataBind();
        }

        private static CompraFila MapearCompra(Pedido pedido)
        {
            var productosHtml = pedido.Detalles
                .OrderBy(x => x.Objeto.Nombre)
                .Select(x => $"{HttpUtility.HtmlEncode(x.Objeto.Nombre)} x{x.Cantidad}")
                .ToList();

            var productosTexto = pedido.Detalles
                .OrderBy(x => x.Objeto.Nombre)
                .Select(x => $"{x.Objeto.Nombre} x{x.Cantidad}")
                .ToList();

            return new CompraFila
            {
                IdPedido = pedido.IdPedido,
                FechaHora = pedido.FechaHora,
                Estado = pedido.Finalizado ? "Confirmada" : "Pendiente",
                DireccionEnvio = pedido.DireccionEnvio,
                Total = pedido.Total,
                ProductosHtml = string.Join("<br />", productosHtml),
                ProductosTexto = string.Join(", ", productosTexto)
            };
        }

        private int ObtenerIdUsuario()
        {
            if (Session["IdUsuario"] is int idUsuario)
            {
                return idUsuario;
            }

            if (Session["UsuarioId"] is int usuarioId)
            {
                return usuarioId;
            }

            return 0;
        }

        private sealed class CompraFila
        {
            public int IdPedido { get; set; }
            public DateTime FechaHora { get; set; }
            public string Estado { get; set; }
            public string DireccionEnvio { get; set; }
            public decimal Total { get; set; }
            public string ProductosHtml { get; set; }
            public string ProductosTexto { get; set; }
        }
    }
}
