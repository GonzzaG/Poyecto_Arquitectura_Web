using System;

namespace DAL.Models.Ventas
{
    /// <summary>
    /// Proyección plana de lectura para que Business construya la comparativa de ventas.
    /// No forma parte del contrato expuesto por el web service.
    /// </summary>
    public class DetalleVentaLectura
    {
        public int IdPedido { get; set; }
        public DateTime FechaHora { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
    }
}
