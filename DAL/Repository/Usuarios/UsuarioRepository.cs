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
                using (var context = new AppDbContext())
                {
                    return context.Usuarios.FirstOrDefault(u => u.Email == email);
                }
            }
            else
            {
                throw new ArgumentException("El email no puede ser nulo o vacío.", nameof(email));
            }
        }
        public List<Usuario> ObtenerTodos()
        {
            using (var context = new AppDbContext())
            {
                return context.Usuarios.ToList();
            }
        }
    }
}
