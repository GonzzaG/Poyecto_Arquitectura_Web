using BEL;
using BEL.Constantes;
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
            {
                throw new ArgumentException("El email y la contraseña no pueden estar vacíos.");
            }
            var usuario = ObtenerUsuario(email);
            if (usuario != null)
            {
                var sessionManager = new SessionManager();
                return sessionManager.Login(email, password, usuario.Password);
            }
            else
            {
                throw new UnauthorizedAccessException("Credenciales inválidas.");
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
                    return roles.ToString().Contains(usuario.Rol.Nombre);

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
                    return rol.ToString().Contains(usuario.Rol.Nombre);

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
