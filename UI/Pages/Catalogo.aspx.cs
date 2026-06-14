using BEL;
using Business.Services.Objeto;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Web.UI;

namespace UI.Pages
{
    public partial class Catalogo : Page
    {
        private readonly ObjetoService _objetoService = new ObjetoService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                CargarProductos();
        }

        protected void btnConfirmarCompra_Click(object sender, EventArgs e)
        {
            string json = hfCarrito.Value;
            if (string.IsNullOrEmpty(json)) return;

            try
            {
                var items = JsonSerializer.Deserialize<List<CarritoItem>>(json);
                Session["Carrito"] = items;
                // Próximo paso: redirigir a la pasarela de pago
                // Response.Redirect("~/Pages/Checkout.aspx");
            }
            catch { }
        }

        private void CargarProductos()
        {
            var productos = _objetoService.ObtenerProductos();
            pnlSinProductos.Visible = productos.Count == 0;
            rptProductos.DataSource = productos;
            rptProductos.DataBind();
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
    }
}