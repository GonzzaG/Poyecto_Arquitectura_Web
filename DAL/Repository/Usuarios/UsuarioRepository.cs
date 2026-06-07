using System;
using System.Collections.Generic;
using System.Linq;
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
                    Usuario usuario = context.Usuarios.FirstOrDefault(u => u.Email == email);
                    usuario.Rol = context.Roles.FirstOrDefault(r => r.IdRol == usuario.IdRol);
                    return usuario;
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

        public void ActualizarBloqueo(string email, int intentosFallidos, DateTime? fechaBloqueo)
        {
            using (var context = new AppDbContext())
            {
                var usuario = context.Usuarios.FirstOrDefault(u => u.Email == email);
                if (usuario != null)
                {
                    usuario.IntentosFallidos = intentosFallidos;
                    usuario.FechaBloqueo = fechaBloqueo;
                    context.SaveChanges();
                }
            }
        }
    }
}
