using BEL;
using BEL.DTOs.Ventas;
using DAL.Repository.Pedido;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Business.Services.Pedido
{
    public class PedidoService
    {
        private readonly PedidoRepository _repository;

        public PedidoService()
        {
            _repository = new PedidoRepository();
        }

        public BEL.Pedido CrearPedido(int idUsuario, IEnumerable<CarritoItem> items, string direccionEnvio)
        {
            if (idUsuario <= 0)
            {
                throw new InvalidOperationException("Debe iniciar sesion para finalizar la compra.");
            }

            if (string.IsNullOrWhiteSpace(direccionEnvio))
            {
                throw new ArgumentException("Debe ingresar una direccion de envio.");
            }

            if (direccionEnvio.Trim().Length > 220)
            {
                throw new ArgumentException("La direccion de envio no puede superar los 220 caracteres.");
            }

            return _repository.CrearPedido(idUsuario, items, direccionEnvio);
        }

        public void GuardarCarritoPendiente(int idUsuario, IEnumerable<CarritoItem> items)
        {
            if (idUsuario <= 0)
            {
                throw new InvalidOperationException("Debe iniciar sesion para guardar el carrito.");
            }

            _repository.GuardarCarritoPendiente(idUsuario, items);
        }

        public IList<CarritoItem> ObtenerCarritoPendiente(int idUsuario)
        {
            if (idUsuario <= 0)
            {
                return new List<CarritoItem>();
            }

            return _repository.ObtenerCarritoPendiente(idUsuario);
        }

        public IList<BEL.Pedido> ObtenerFinalizados()
        {
            return _repository.ObtenerFinalizados();
        }

        public IList<BEL.Pedido> ObtenerFinalizadosPorUsuario(int idUsuario)
        {
            if (idUsuario <= 0)
            {
                return new List<BEL.Pedido>();
            }

            return _repository.ObtenerFinalizadosPorUsuario(idUsuario);
        }

        public ComparativaVentasDto ObtenerComparativaProductosVendidos(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            DateTime? desdeInclusive = fechaDesde?.Date;
            DateTime? hastaInclusive = fechaHasta?.Date;

            if (desdeInclusive.HasValue && hastaInclusive.HasValue && desdeInclusive.Value > hastaInclusive.Value)
            {
                throw new ArgumentException("La fecha desde no puede ser posterior a la fecha hasta.");
            }

            DateTime? hastaExclusive = hastaInclusive.HasValue && hastaInclusive.Value < DateTime.MaxValue.Date
                ? hastaInclusive.Value.AddDays(1)
                : (DateTime?)null;
            var detalles = _repository.ObtenerDetallesProductosVendidos(desdeInclusive, hastaExclusive);
            var productos = detalles
                .GroupBy(x => new { x.IdProducto, x.NombreProducto })
                .Select(grupo => new ProductoVendidoDto
                {
                    IdObjeto = grupo.Key.IdProducto,
                    Nombre = grupo.Key.NombreProducto,
                    CantidadVendida = grupo.Sum(x => x.Cantidad),
                    CantidadPedidos = grupo.Select(x => x.IdPedido).Distinct().Count()
                })
                .OrderByDescending(x => x.CantidadVendida)
                .ThenBy(x => x.Nombre)
                .ToList();

            int totalUnidades = productos.Sum(x => x.CantidadVendida);
            int totalPedidos = detalles.Select(x => x.IdPedido).Distinct().Count();

            foreach (var producto in productos)
            {
                producto.ParticipacionPorcentaje = totalUnidades == 0
                    ? 0
                    : Math.Round(producto.CantidadVendida * 100m / totalUnidades, 2);
            }

            return new ComparativaVentasDto
            {
                FechaDesde = desdeInclusive?.ToString("yyyy-MM-dd"),
                FechaHasta = hastaInclusive?.ToString("yyyy-MM-dd"),
                TotalUnidadesVendidas = totalUnidades,
                TotalProductosConVentas = productos.Count,
                TotalPedidos = totalPedidos,
                Productos = productos
            };
        }

        public ReporteVentasDto GenerarReporteComparativaProductosVendidos(ComparativaVentasDto comparativa)
        {
            if (comparativa == null)
            {
                throw new ArgumentNullException(nameof(comparativa));
            }

            var cultura = CultureInfo.GetCultureInfo("es-AR");
            var contenido = new StringBuilder();
            contenido.AppendLine("sep=;");
            contenido.AppendLine("Informe de productos vendidos");
            contenido.AppendLine($"Período;{EscaparCsv(ObtenerDescripcionPeriodo(comparativa))}");
            contenido.AppendLine($"Generado;{DateTime.Now.ToString("dd/MM/yyyy HH:mm", cultura)}");
            contenido.AppendLine($"Total de unidades vendidas;{comparativa.TotalUnidadesVendidas}");
            contenido.AppendLine($"Total de pedidos;{comparativa.TotalPedidos}");
            contenido.AppendLine($"Productos con ventas;{comparativa.TotalProductosConVentas}");
            contenido.AppendLine();
            contenido.AppendLine("Posición;Producto;Unidades vendidas;Pedidos;Participación");

            int posicion = 1;
            foreach (var producto in comparativa.Productos ?? new List<ProductoVendidoDto>())
            {
                contenido.AppendLine(string.Join(";", new[]
                {
                    posicion.ToString(cultura),
                    EscaparCsv(producto.Nombre),
                    producto.CantidadVendida.ToString(cultura),
                    producto.CantidadPedidos.ToString(cultura),
                    EscaparCsv(producto.ParticipacionPorcentaje.ToString("N2", cultura) + " %")
                }));
                posicion++;
            }

            byte[] bytes = Encoding.UTF8.GetBytes("\uFEFF" + contenido);
            return new ReporteVentasDto
            {
                NombreArchivo = ObtenerNombreArchivo(comparativa),
                TipoContenido = "text/csv;charset=utf-8",
                ContenidoBase64 = Convert.ToBase64String(bytes)
            };
        }

        private static string ObtenerDescripcionPeriodo(ComparativaVentasDto comparativa)
        {
            if (string.IsNullOrWhiteSpace(comparativa.FechaDesde) && string.IsNullOrWhiteSpace(comparativa.FechaHasta))
            {
                return "Histórico completo";
            }

            string desde = FormatearFechaReporte(comparativa.FechaDesde) ?? "Inicio";
            string hasta = FormatearFechaReporte(comparativa.FechaHasta) ?? "Actualidad";
            return $"Desde {desde} hasta {hasta}";
        }

        private static string ObtenerNombreArchivo(ComparativaVentasDto comparativa)
        {
            string periodo = string.IsNullOrWhiteSpace(comparativa.FechaDesde) && string.IsNullOrWhiteSpace(comparativa.FechaHasta)
                ? "historico"
                : $"{comparativa.FechaDesde ?? "inicio"}-a-{comparativa.FechaHasta ?? "actualidad"}";
            return $"informe-productos-vendidos-{periodo}.csv";
        }

        private static string FormatearFechaReporte(string fecha)
        {
            DateTime valor;
            return DateTime.TryParseExact(fecha, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out valor)
                ? valor.ToString("dd/MM/yyyy")
                : null;
        }

        private static string EscaparCsv(string valor)
        {
            string texto = valor ?? string.Empty;
            return texto.IndexOfAny(new[] { ';', '"', '\r', '\n' }) >= 0
                ? $"\"{texto.Replace("\"", "\"\"")}\""
                : texto;
        }
    }
}
