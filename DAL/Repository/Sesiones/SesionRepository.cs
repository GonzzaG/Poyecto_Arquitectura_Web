using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEL;

namespace DAL.Repository.Sesiones
{
    public class SesionRepository
    {
        public Sesion ObtenerSesion(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                using (var context = new AppDbContext())
                {
                    return context.Sesions.OrderByDescending(s => s.FechaCreacion).FirstOrDefault(s => s.Email == email);
                }
            }
            else
            {
                throw new ArgumentException("El email no puede ser nulo o vacío.", nameof(email));
            }
        }
        public void GuardarSesion(string email, Guid sessionId, DateTime now)
        {
            using (var context = new AppDbContext())
            {
                var sesion = new Sesion
                {
                    Email = email,
                    IdSession = sessionId,
                    FechaCreacion = DateTime.UtcNow
                };
                context.Sesions.Add(sesion);
                context.SaveChanges();
            }
        }
    }
}
