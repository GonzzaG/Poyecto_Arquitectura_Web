using BEL;
using BEL.DTOs.Bitacora;
using Business.Services.Bitacora;
using Business.Services.Pedido;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UI.Pages
{
    public partial class Checkout : BasePage
    {
        private static readonly BitacoraService BitacoraService = new BitacoraService();
        private readonly PedidoService _pedidoService = new PedidoService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarCarrito();
            }
        }

        protected void btnFinalizarCompra_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            var items = ObtenerCarrito();
            if (!items.Any())
            {
                MostrarCarritoVacio();
                return;
            }

            var idUsuario = ObtenerIdUsuario();
            if (idUsuario <= 0)
            {
                Response.Redirect("~/Login");
                return;
            }

            try
            {
                var pedido = _pedidoService.CrearPedido(idUsuario, items, txtDireccionEnvio.Text);
                Session.Remove("Carrito");
                RegistrarCompraExitosa(pedido);
                ShowSuccess($"Compra confirmada. Pedido #{pedido.IdPedido}.", "Compra exitosa");
                lblMensaje.CssClass = "mt-4 block text-sm font-semibold text-green-700";
                lblMensaje.Text = $"Compra confirmada. Pedido #{pedido.IdPedido}.";
                btnFinalizarCompra.Enabled = false;
                CargarCarrito();
            }
            catch (Exception ex)
            {
                lblMensaje.CssClass = "mt-4 block text-sm font-semibold text-red-700";
                lblMensaje.Text = ex.Message;
            }
        }

        protected string ResolveImagenUrl(string imagenUrl)
        {
            if (string.IsNullOrWhiteSpace(imagenUrl))
            {
                return ResolveUrl("~/Images/Productos/placeholder.png");
            }

            return ResolveUrl(imagenUrl);
        }

        private void CargarCarrito()
        {
            var items = ObtenerCarrito();
            if (!items.Any())
            {
                MostrarCarritoVacio();
                return;
            }

            pnlSinCarrito.Visible = false;
            pnlCheckout.Visible = true;
            rptItems.DataSource = items;
            rptItems.DataBind();
            lblTotal.Text = "$ " + items.Sum(x => x.Subtotal).ToString("N2");
        }

        private void MostrarCarritoVacio()
        {
            pnlSinCarrito.Visible = true;
            pnlCheckout.Visible = false;
        }

        private List<CarritoItem> ObtenerCarrito()
        {
            var carritoSesion = Session["Carrito"] as List<CarritoItem>;
            if (carritoSesion != null && carritoSesion.Any())
            {
                return carritoSesion;
            }

            var idUsuario = ObtenerIdUsuario();
            if (idUsuario <= 0)
            {
                return new List<CarritoItem>();
            }

            var carritoPersistido = _pedidoService.ObtenerCarritoPendiente(idUsuario)
                .Select(x => new CarritoItem
                {
                    IdObjeto = x.IdObjeto,
                    Nombre = x.Nombre,
                    Precio = x.Precio,
                    ImagenUrl = ResolveImagenUrl(x.ImagenUrl),
                    Cantidad = x.Cantidad
                })
                .ToList();

            Session["Carrito"] = carritoPersistido;
            return carritoPersistido;
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

        private void RegistrarCompraExitosa(BEL.Pedido pedido)
        {
            BitacoraService.RegistrarAccion(new BitacoraRegistroDto
            {
                FechaUtc = DateTime.UtcNow,
                Modulo = "Compras",
                Accion = "CompraExitosa",
                Resultado = "OK",
                Mensaje = $"Compra exitosa. Pedido #{pedido.IdPedido}. Total: {pedido.Total:N2}.",
                Detalle = $"DireccionEnvio: {pedido.DireccionEnvio}",
                IdUsuario = pedido.IdUsuario,
                UsuarioEmail = ObtenerUsuarioEmail(),
                Url = Request?.RawUrl,
                Ip = Request?.UserHostAddress
            });
        }

        private string ObtenerUsuarioEmail()
        {
            return Session["UsuarioEmail"] as string
                ?? Session["Email"] as string;
        }
    }
}
