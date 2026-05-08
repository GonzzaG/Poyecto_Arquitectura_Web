using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using BEL;
using Services;
using DAL.Repository.Usuarios;
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
        public bool ValidarAcceso(Cookie cookie, string rol)
        {
            SessionManager sessionManager = new SessionManager();
            if (sessionManager.ValidarCookie(cookie))
            {
                var usuario = ObtenerUsuario(cookie.Name);
                if (usuario != null)
                {
                    return usuario.Rol.Nombre == rol;

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
