using System.Collections.Generic;

namespace BEL
{
    public class Rol
    {
        public int IdRol { get; set; }
        public string Nombre { get; set; }

        public virtual ICollection<Usuario> Usuarios { get; set; }

        public Rol()
        {
            Usuarios = new HashSet<Usuario>();
        }
    }
}
