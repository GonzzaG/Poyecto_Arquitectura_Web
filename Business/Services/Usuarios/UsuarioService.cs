using BEL;
using BEL.Constantes;
using BEL.Exceptions;
using DAL.Repository.Usuarios;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace Business.Services.Usuarios
{
    public class UsuarioService
    {
        private readonly UsuarioRepository _usuarioRepository;

        public UsuarioService()
        {
            _usuarioRepository = new UsuarioRepository();
        }

        public Usuario ObtenerUsuario(string email)
        {
            return _usuarioRepository.ObtenerUsuario(email);
        }
        public List<Usuario> ObtenerTodos()
        {
            return _usuarioRepository.ObtenerTodos();
        }

        public Usuario ObtenerUsuarioAutenticado(Cookie cookie)
        {
            var sessionManager = new SessionManager();
            if (cookie == null || !sessionManager.ValidarCookie(cookie))
            {
                return null;
            }

            return ObtenerUsuario(cookie.Name);
        }

        public Cookie Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                throw new ArgumentException("El email y la contraseña no pueden estar vacíos.");

            var usuario = ObtenerUsuario(email);
            if (usuario == null)
                throw new UnauthorizedAccessException("Credenciales inválidas.");

            if (usuario.FechaBloqueo.HasValue && usuario.FechaBloqueo.Value > DateTime.Now)
                throw new InvalidOperationException($"Cuenta bloqueada por múltiples intentos fallidos. Intente nuevamente después de las {usuario.FechaBloqueo.Value:HH:mm:ss}.");

            var sessionManager = new SessionManager();
            try
            {
                Cookie cookie = sessionManager.Login(email, password, usuario.Password);
                _usuarioRepository.ActualizarBloqueo(email, 0, null);
                return cookie;
            }
            catch (UnauthorizedAccessException)
            {
                int nuevosIntentos;
                DateTime? nuevaFechaBloqueo;

                // Bloqueo expirado: el primer intento fallido post-bloqueo reinicia el contador a 1
                if (usuario.FechaBloqueo.HasValue && usuario.FechaBloqueo.Value <= DateTime.Now)
                {
                    nuevosIntentos = 1;
                    nuevaFechaBloqueo = null;
                }
                else
                {
                    nuevosIntentos = usuario.IntentosFallidos + 1;
                    nuevaFechaBloqueo = nuevosIntentos >= 3 ? DateTime.Now.AddMinutes(15) : (DateTime?)null;
                }

                _usuarioRepository.ActualizarBloqueo(email, nuevosIntentos, nuevaFechaBloqueo);
                throw new AppException("Contraseña incorrecta");
            }
        }
        public bool ValidarAcceso(Cookie cookie, List<RolesEnum> roles)
        {
            if (roles is null || roles.Count == 0) return false;

            SessionManager sessionManager = new SessionManager();
            if (sessionManager.ValidarCookie(cookie))
            {
                var usuario = ObtenerUsuario(cookie.Name);
                if (usuario != null)
                {
                    return roles.Any(x => string.Equals(x.ToString(), usuario.Rol.Nombre, StringComparison.OrdinalIgnoreCase));

                }
                else
                {
                    throw new UnauthorizedAccessException("Usuario no encontrado.");
                }
            }
            else
            {
                throw new UnauthorizedAccessException("Acceso no autorizado.");
            }
        }

        public bool ValidarAcceso(Cookie cookie, RolesEnum rol)
        {
            SessionManager sessionManager = new SessionManager();
            if (sessionManager.ValidarCookie(cookie))
            {
                var usuario = ObtenerUsuario(cookie.Name);
                if (usuario != null)
                {
                    return ((rol.ToString()).ToUpper()).Contains((usuario.Rol.Nombre).ToUpper());

                }
                else
                {
                    throw new UnauthorizedAccessException("Usuario no encontrado.");
                }
            }
            else
            {
                throw new UnauthorizedAccessException("Acceso no autorizado.");
            }
        }
    }
}
