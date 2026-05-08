using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.Json;
using Business.Services.Usuarios;
using System.Net;
using Azure;

namespace UI.Pages
{
    public class VerificadorAcceso
    {
        private readonly UsuarioService _usuarioService;

        public VerificadorAcceso()
        {
            _usuarioService = new UsuarioService();
        }

        public bool VerificarAcceso(string rol)
        {
            Cookie cookie = JsonSerializer.Deserialize<Cookie>(HttpContext.Current.Request.Cookies["AuthToken"].Value);
            return _usuarioService.ValidarAcceso(cookie, rol);
        }
    }
}