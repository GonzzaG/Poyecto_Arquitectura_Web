using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using DAL;
using BEL;
namespace Services
{
    public class SessionManager
    {
        public bool ValidarCookie(Cookie cookie)
        {
            bool isValid = true;
            Sesion sesion = null;
            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
            {
                isValid = false;
            }
            var sesionRepository = new DAL.Repository.Sesiones.SesionRepository();
            if (isValid)
            {
                try
                {
                    sesion = sesionRepository.ObtenerSesion(cookie.Name);

                    if (sesion == null)
                    {
                        isValid = false;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            Encriptador encript = new Encriptador();
            if (isValid && sesion != null)
            {
                string expectedValue = BodyBuilding(sesion.IdSession, sesion.FechaCreacion);
                if (expectedValue != null)
                {
                    isValid = encript.VerificarEncriptacion(expectedValue, cookie.Value);
                    if (isValid)
                    {
                        {
                            isValid = DateTime.Now < sesion.FechaCreacion.AddHours(1);
                        }
                    }
                }
                //return cookie.Value == expectedValue && DateTime.Now < sesion.FechaCreacion.AddHours(1);
            }
            return isValid;
        }
        public Cookie Login(string email, string password, string actualPassword)
        {
            // Lógica de autenticación y creación de sesión
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(actualPassword))
            {
                throw new ArgumentException("El email y la contraseña no pueden estar vacíos.");
            }
            Encriptador encript = new Encriptador();
            if (encript.VerificarEncriptacion(password, actualPassword))
            {
                // Crear una cookie de sesión (HttpCookie permite almacenar keys dentro de la cookie)
                Guid sessionId = Guid.NewGuid();
                DateTime now = DateTime.Now;
                string body = BodyBuilding(sessionId, now);
                var sessionCookie = new Cookie(email, encript.Encriptar(body))
                {
                    Expires = DateTime.Now.AddHours(1) // La sesión expira en 1 hora
                };

                // Guardar la sesión en la base de datos
                var sesionRepository = new DAL.Repository.Sesiones.SesionRepository();
                sesionRepository.GuardarSesion(email, sessionId, now);

                return sessionCookie;
            }
            else
            {
                throw new UnauthorizedAccessException("Credenciales inválidas.");
            }
        }
        public string BodyBuilding(Guid sessionId, DateTime now)
        {
            return sessionId.ToString() + now.ToString();
        }
    }
}
