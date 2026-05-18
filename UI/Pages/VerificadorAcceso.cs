using Azure;
using BEL.Constantes;
using Business.Services.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Web;

namespace UI.Pages
{
    public class VerificadorAcceso
    {
        private readonly UsuarioService _usuarioService;

        public VerificadorAcceso()
        {
            _usuarioService = new UsuarioService();
        }

        public bool VerificarAcceso(List<RolesEnum> rol)
        {
            try
            {
                Cookie cookie = JsonSerializer.Deserialize<Cookie>(HttpContext.Current.Request.Cookies["AuthToken"].Value);
                return _usuarioService.ValidarAcceso(cookie, rol);
            }
            catch(Exception ex)
            {
                throw new Exception("No posee permisos para acceder a esta pagina", ex);
            }
        }

        public bool VerificarAcceso(RolesEnum rol)
        {
            try
            {
                Cookie cookie = JsonSerializer.Deserialize<Cookie>(HttpContext.Current.Request.Cookies["AuthToken"].Value);
                return _usuarioService.ValidarAcceso(cookie, rol);
            }
            catch (Exception ex)
            {
                throw new Exception("No posee permisos para acceder a esta pagina", ex);
            }
        }
    }
}