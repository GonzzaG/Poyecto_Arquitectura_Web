using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEL;

namespace DAL.Repository.Usuarios
{
    public class UsuarioRepository
    {
        public Usuario ObtenerUsuario(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {

            }
            else
            {
                throw new ArgumentException("El email no puede ser nulo o vacío.", nameof(email));
            }
            // Lógica para obtener un usuario por su email
            return null;
        }
    }
}
