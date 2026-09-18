using BEL;
using BEL.Constantes;
using BEL.DTOs.Ventas;
using Business.Helper;
using Business.Services.Pedido;
using Business.Services.Usuarios;
using System;
using System.Globalization;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace UI.Services
{
    [WebService(Namespace = "http://gestclima.local/ventas")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    public class VentasWebService : WebService
    {
        private readonly PedidoService _pedidoService = new PedidoService();

        [WebMethod(EnableSession = true, Description = "Obtiene la comparativa de productos vendidos para un periodo.")]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public ComparativaVentasDto ObtenerComparativaProductos(string fechaDesde, string fechaHasta)
        {
            if (!ValidarAcceso())
            {
                return null;
            }

            DateTime? desde = ParsearFecha(fechaDesde, "fechaDesde");
            DateTime? hasta = ParsearFecha(fechaHasta, "fechaHasta");
            return _pedidoService.ObtenerComparativaProductosVendidos(desde, hasta);
        }

        private static DateTime? ParsearFecha(string valor, string nombreParametro)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return null;
            }

            DateTime fecha;
            if (!DateTime.TryParseExact(
                valor,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out fecha))
            {
                throw new ArgumentException($"El parámetro {nombreParametro} debe tener el formato yyyy-MM-dd.");
            }

            return fecha;
        }

        private static bool ValidarAcceso()
        {
            Usuario usuario = AutenticacionHelper.ObtenerUsuarioDesdeCookie(new UsuarioService());
            bool autorizado = usuario != null &&
                (usuario.IdRol == (int)RolesEnum.WEBMASTER ||
                 usuario.IdRol == (int)RolesEnum.ADM_OPERACIONES);

            if (autorizado)
            {
                return true;
            }

            HttpContext.Current.Response.StatusCode = 401;
            HttpContext.Current.Response.TrySkipIisCustomErrors = true;
            return false;
        }
    }
}
