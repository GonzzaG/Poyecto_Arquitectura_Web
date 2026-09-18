namespace BEL.DTOs.Ventas
{
    public class ProductoVendidoDto
    {
        public int IdObjeto { get; set; }
        public string Nombre { get; set; }
        public int CantidadVendida { get; set; }
        public int CantidadPedidos { get; set; }
        public decimal ParticipacionPorcentaje { get; set; }
    }
}
