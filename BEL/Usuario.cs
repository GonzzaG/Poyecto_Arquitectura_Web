using System.Collections.Generic;

namespace BEL
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Nombre { get; set; }
        public bool Inactivo { get; set; }

        public virtual Rol Rol { get; set; }
        public virtual ICollection<Pedido> Pedidos { get; set; }
        public virtual ICollection<Bitacora> Bitacoras { get; set; }

        public Usuario()
        {
            Pedidos = new HashSet<Pedido>();
            Bitacoras = new HashSet<Bitacora>();
        }
    }
}
