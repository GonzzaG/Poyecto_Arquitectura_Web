using BEL;
using BEL.Constantes;
using BEL.DTOs.Ventas;
using Business.Helper;
using Business.Services.Pedido;
using Business.Services.Usuarios;
using System;
using System.Collections.Generic;
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
        private const string ComparativasSesionKey = "VentasWebService.Comparativas";
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
            ComparativaVentasDto comparativa = _pedidoService.ObtenerComparativaProductosVendidos(desde, hasta);
            comparativa.IdConsulta = Guid.NewGuid().ToString("N");

            var comparativas = Context.Session[ComparativasSesionKey] as Dictionary<string, ComparativaVentasDto>;
            if (comparativas == null)
            {
                comparativas = new Dictionary<string, ComparativaVentasDto>();
                Context.Session[ComparativasSesionKey] = comparativas;
            }

            if (comparativas.Count >= 20)
            {
                comparativas.Clear();
            }

            comparativas[comparativa.IdConsulta] = comparativa;
            return comparativa;
        }

        [WebMethod(EnableSession = true, Description = "Genera el informe CSV de la comparativa que se encuentra visible.")]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public ReporteVentasDto GenerarInformeProductos(string idConsulta)
        {
            if (!ValidarAcceso())
            {
                return null;
            }

            var comparativas = Context.Session[ComparativasSesionKey] as Dictionary<string, ComparativaVentasDto>;
            ComparativaVentasDto comparativa;
            if (comparativas == null || string.IsNullOrWhiteSpace(idConsulta) ||
                !comparativas.TryGetValue(idConsulta, out comparativa))
            {
                throw new InvalidOperationException("La comparativa visible ya no está disponible. Actualice los gráficos e intente nuevamente.");
            }

            return _pedidoService.GenerarReporteComparativaProductosVendidos(comparativa);
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
