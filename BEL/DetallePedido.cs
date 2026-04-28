namespace BEL
{
    public class DetallePedido
    {
        public int IdDetallePedido { get; set; }
        public int IdPedido { get; set; }
        public int IdObjeto { get; set; }
        public int Cantidad { get; set; }
        public bool Finalizado { get; set; }

        public virtual Pedido Pedido { get; set; }
        public virtual Objeto Objeto { get; set; }
    }
}
