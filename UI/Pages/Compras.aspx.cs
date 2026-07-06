using BEL;
using BEL.Constantes;
using Business.Services.Pedido;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Pages
{
    public partial class Compras : BasePage
    {
        private static readonly IReadOnlyList<RolesEnum> RolesPermitidos = new[]
        {
            RolesEnum.WEBMASTER,
            RolesEnum.ADM_OPERACIONES
        };

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
                CargarCompras();
            }
        }

        private bool TieneAcceso()
        {
            try
            {
                return new VerificadorAcceso().VerificarAcceso(RolesPermitidos.ToList());
            }
            catch
            {
                return false;
            }
        }

        private void CargarCompras()
        {
            var compras = _pedidoService.ObtenerFinalizados()
                .Select(MapearCompra)
                .ToList();

            var sinResultados = !compras.Any();
            PanelSinResultados.Visible = sinResultados;
            RptCompras.Visible = !sinResultados;
            RptComprasMobile.Visible = !sinResultados;
            LblTotal.Text = $"{compras.Count} compras registradas";

            RptCompras.DataSource = compras;
            RptCompras.DataBind();
            RptComprasMobile.DataSource = compras;
            RptComprasMobile.DataBind();
        }

        private static CompraFila MapearCompra(Pedido pedido)
        {
            var productos = pedido.Detalles
                .OrderBy(x => x.Objeto.Nombre)
                .Select(x => $"{HttpUtility.HtmlEncode(x.Objeto.Nombre)} x{x.Cantidad}")
                .ToList();

            return new CompraFila
            {
                IdPedido = pedido.IdPedido,
                FechaHora = pedido.FechaHora,
                ClienteNombre = pedido.Usuario?.Nombre ?? "Sin nombre",
                ClienteEmail = pedido.Usuario?.Email ?? "Sin email",
                DireccionEnvio = pedido.DireccionEnvio,
                Total = pedido.Total,
                ProductosHtml = string.Join("<br />", productos),
                ProductosTexto = string.Join(", ", productos)
            };
        }

        private sealed class CompraFila
        {
            public int IdPedido { get; set; }
            public DateTime FechaHora { get; set; }
            public string ClienteNombre { get; set; }
            public string ClienteEmail { get; set; }
            public string DireccionEnvio { get; set; }
            public decimal Total { get; set; }
            public string ProductosHtml { get; set; }
            public string ProductosTexto { get; set; }
        }
    }
}
