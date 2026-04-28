using System.Collections.Generic;

namespace BEL
{
    public class Objeto
    {
        public int IdObjeto { get; set; }
        public decimal Precio { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool EsProducto { get; set; }
        public int Stock { get; set; }

        public virtual ICollection<DetallePedido> DetallesPedido { get; set; }

        public Objeto()
        {
            DetallesPedido = new HashSet<DetallePedido>();
        }
    }
}
