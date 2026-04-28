using System;
using System.Collections.Generic;

namespace BEL
{
    public class Pedido
    {
        public int IdPedido { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaHora { get; set; }
        public decimal Total { get; set; }
        public string DireccionEnvio { get; set; }
        public bool Finalizado { get; set; }

        public virtual Usuario Usuario { get; set; }
        public virtual ICollection<DetallePedido> Detalles { get; set; }

        public Pedido()
        {
            Detalles = new HashSet<DetallePedido>();
        }
    }
}
