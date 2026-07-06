using BEL;
using Business.Services.Objeto;
using Business.Services.Pedido;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;
using System.Web.Services;
using System.Web.UI;

namespace UI.Pages
{
    public partial class Catalogo : Page
    {
        private readonly ObjetoService _objetoService = new ObjetoService();
        private readonly PedidoService _pedidoService = new PedidoService();

        protected string CarritoInicialJson { get; private set; } = "[]";

        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.GetCurrent(Page).EnablePageMethods = true;

            if (!IsPostBack)
            {
                CargarCarritoInicial();
                CargarProductos();
            }
        }

        protected void btnConfirmarCompra_Click(object sender, EventArgs e)
        {
            string json = hfCarrito.Value;
            if (string.IsNullOrEmpty(json)) return;

            try
            {
                var items = JsonSerializer.Deserialize<List<CarritoItem>>(json);
                Session["Carrito"] = items;
                GuardarCarritoPendiente(items);
                Response.Redirect("~/Pages/Checkout.aspx");
            }
            catch { }
        }

        [WebMethod(EnableSession = true)]
        public static void GuardarCarrito(List<CarritoItem> items)
        {
            var carrito = items ?? new List<CarritoItem>();
            HttpContext.Current.Session["Carrito"] = carrito;

            var idUsuario = ObtenerIdUsuarioDesdeSesion();
            if (idUsuario > 0)
            {
                new PedidoService().GuardarCarritoPendiente(idUsuario, carrito);
            }
        }

        private void CargarProductos()
        {
            var productos = _objetoService.ObtenerProductos();
            pnlSinProductos.Visible = productos.Count == 0;
            rptProductos.DataSource = productos;
            rptProductos.DataBind();
        }

        private void CargarCarritoInicial()
        {
            var carrito = ObtenerCarritoInicial()
                .Select(x => new CarritoItem
                {
                    IdObjeto = x.IdObjeto,
                    Nombre = x.Nombre,
                    Precio = x.Precio,
                    ImagenUrl = ResolveImagenUrl(x.ImagenUrl),
                    Cantidad = x.Cantidad
                })
                .ToList();

            Session["Carrito"] = carrito;
            CarritoInicialJson = JsonSerializer.Serialize(carrito);
        }

        private List<CarritoItem> ObtenerCarritoInicial()
        {
            var idUsuario = ObtenerIdUsuario();
            if (idUsuario > 0)
            {
                var carritoPersistido = _pedidoService.ObtenerCarritoPendiente(idUsuario).ToList();
                if (carritoPersistido.Any())
                {
                    return carritoPersistido;
                }
            }

            return Session["Carrito"] as List<CarritoItem> ?? new List<CarritoItem>();
        }

        private void GuardarCarritoPendiente(List<CarritoItem> items)
        {
            var idUsuario = ObtenerIdUsuario();
            if (idUsuario > 0)
            {
                _pedidoService.GuardarCarritoPendiente(idUsuario, items ?? new List<CarritoItem>());
            }
        }

        protected string ResolveImagenUrl(string imagenUrl)
        {
            if (string.IsNullOrEmpty(imagenUrl))
                return ResolveUrl("~/Images/Productos/placeholder.png");
            return ResolveUrl(imagenUrl);
        }

        protected string GetOnClickCard(object dataItem)
        {
            var obj = dataItem as Objeto;
            if (obj == null) return "";
            string nombre = obj.Nombre.Replace("'", "\\'");
            string desc = obj.Descripcion.Replace("'", "\\'");
            string img = ResolveImagenUrl(obj.ImagenUrl);
            return string.Format("abrirSidebar({0}, '{1}', {2}, '{3}', {4}, '{5}')",
                obj.IdObjeto, nombre, obj.Precio.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture), desc, obj.Stock, img);
        }

        protected string GetOnClickBoton(object dataItem)
        {
            var obj = dataItem as Objeto;
            if (obj == null) return "";
            string nombre = obj.Nombre.Replace("'", "\\'");
            string img = ResolveImagenUrl(obj.ImagenUrl);
            return string.Format("event.stopPropagation(); agregarAlCarrito({0}, '{1}', {2}, '{3}', 1)",
                obj.IdObjeto, nombre, obj.Precio.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture), img);
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

        private static int ObtenerIdUsuarioDesdeSesion()
        {
            var session = HttpContext.Current?.Session;
            if (session == null)
            {
                return 0;
            }

            if (session["IdUsuario"] is int idUsuario)
            {
                return idUsuario;
            }

            if (session["UsuarioId"] is int usuarioId)
            {
                return usuarioId;
            }

            return 0;
        }
    }
}
