using System.Collections.Generic;

namespace BEL.DTOs.Ventas
{
    public class ComparativaVentasDto
    {
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public int TotalUnidadesVendidas { get; set; }
        public int TotalProductosConVentas { get; set; }
        public int TotalPedidos { get; set; }
        public List<ProductoVendidoDto> Productos { get; set; }

        public ComparativaVentasDto()
        {
            Productos = new List<ProductoVendidoDto>();
        }
    }
}
