using BEL;
using BEL.Constantes;
using Business.Helper;
using Business.Services.Integrity;
using Business.Services.Usuarios;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Web;
using System.Web.UI;

namespace UI
{
    public partial class SiteMaster : MasterPage
    {
        private readonly UsuarioService _usuarioService = new UsuarioService();
        private readonly IntegrityValidationService _integrityService = new IntegrityValidationService();

        protected void Page_Load(object sender, EventArgs e)
        {
            ActualizarEstadoSesion();
        }

        protected void BtnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            if (Request.Cookies["AuthToken"] != null)
            {
                var cookie = new HttpCookie("AuthToken")
                {
                    Value = string.Empty,
                    Expires = DateTime.Now.AddDays(-1),
                    HttpOnly = true,
                    Secure = Request.IsSecureConnection,
                    Path = string.IsNullOrWhiteSpace(Request.ApplicationPath) ? "/" : Request.ApplicationPath
                };
                Response.Cookies.Add(cookie);
            }

            Response.Redirect(ResolveUrl("~/Login"), false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void BtnValidarIntegridad_Click(object sender, EventArgs e)
        {
            try
            {
                var usuariosReparados = _integrityService.ReparaIntegridad();
                Application["IntegrityMaintenance"] = false;
                ShowSuccess($"Se recalcularon DVH/DVV. Usuarios con DVH reparado: {usuariosReparados.Count}.", "Integridad Validada");
            }
            catch (Exception ex)
            {
                ShowError("Error al validar integridad: " + ex.Message);
            }
        }

        private void ActualizarEstadoSesion()
        {
            Usuario usuario = ObtenerUsuarioAutenticado();
            RolesEnum? rol = usuario != null ? (RolesEnum?)usuario.IdRol : null;

            AplicarVisibilidadPorRol(rol);
            ActualizarAvisoIntegridad(rol);

            if (usuario != null)
            {
                string nombre = string.IsNullOrWhiteSpace(usuario.Nombre) ? usuario.Email : usuario.Nombre;
                string iniciales = ObtenerIniciales(nombre);

                // Desktop
                LblUsuarioLogueado.Text = nombre;
                LblAvatar.Text = iniciales;
                LblDropdownNombre.Text = nombre;
                LblDropdownEmail.Text = usuario.Email;

            }
        }

        private string ObtenerIniciales(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return "?";
            var partes = nombre.Trim().Split(' ');
            if (partes.Length >= 2)
                return $"{partes[0][0]}{partes[1][0]}".ToUpper();
            return nombre.Substring(0, Math.Min(2, nombre.Length)).ToUpper();
        }

        private void AplicarVisibilidadPorRol(RolesEnum? rol)
        {
            // null = visible solo para usuarios no autenticados
            // RolesEnum = nivel mínimo de rol requerido
            var controles = new Dictionary<System.Web.UI.Control, RolesEnum?>
            {
                { PnlPublicLinks,       null                    },
                { PnlPublicLinksMobile, null                    },
                { PnlUserSession,       RolesEnum.CLIENTE       },
                { PnlUserSessionMobile, RolesEnum.CLIENTE       },
                { LnkCompras,           RolesEnum.ADM_OPERACIONES },
                { LnkComprasMobile,     RolesEnum.ADM_OPERACIONES },
                { LnkBitacora,          RolesEnum.WEBMASTER     },
                { LnkBitacoraMobile,    RolesEnum.WEBMASTER     },
                { LnkIntegridad,        RolesEnum.WEBMASTER     },
                { LnkIntegridadMobile,  RolesEnum.WEBMASTER     },
                { LnkMigraciones,       RolesEnum.WEBMASTER     },
                { LnkMigracionesMobile, RolesEnum.WEBMASTER     },
                { LnkBackup,            RolesEnum.WEBMASTER     },
                { LnkBackupMobile,      RolesEnum.WEBMASTER     },
                { BtnValidarIntegridad, RolesEnum.WEBMASTER     },
            };

            foreach (var entry in controles)
            {
                entry.Key.Visible = entry.Value == null
                    ? !rol.HasValue
                    : rol.HasValue && NivelAcceso(rol.Value) >= NivelAcceso(entry.Value.Value);
            }

            LnkMisCompras.Visible = rol == RolesEnum.CLIENTE;
            LnkMisComprasMobile.Visible = rol == RolesEnum.CLIENTE;
        }

        private void ActualizarAvisoIntegridad(RolesEnum? rol)
        {
            bool mantenimiento = Application["IntegrityMaintenance"] is bool && (bool)Application["IntegrityMaintenance"];
            bool esWebmaster = rol.HasValue && NivelAcceso(rol.Value) >= NivelAcceso(RolesEnum.WEBMASTER);

            PnlIntegrityWarning.Visible = mantenimiento && esWebmaster;
        }

        private static int NivelAcceso(RolesEnum rol)
        {
            return AutenticacionHelper.NivelAcceso(rol);
        }

        private Usuario ObtenerUsuarioAutenticado()
        {
            HttpCookie authToken = Request.Cookies["AuthToken"];
            if (authToken == null || string.IsNullOrWhiteSpace(authToken.Value))
                return null;

            try
            {
                Cookie cookie = JsonSerializer.Deserialize<Cookie>(authToken.Value);
                if (cookie == null || string.IsNullOrWhiteSpace(cookie.Name))
                    return null;

                Usuario usuario = _usuarioService.ObtenerUsuarioAutenticado(cookie);
                if (usuario == null)
                    return null;

                Session["IdUsuario"] = usuario.IdUsuario;
                Session["UsuarioId"] = usuario.IdUsuario;
                Session["UsuarioEmail"] = usuario.Email;
                Session["Email"] = usuario.Email;
                Session["UsuarioNombre"] = string.IsNullOrWhiteSpace(usuario.Nombre) ? usuario.Email : usuario.Nombre;

                return usuario;
            }
            catch
            {
                return null;
            }
        }

        private Controls.NotificationModal Modal => AppModal;

        /// <summary>Solo X para cerrar. Sin botones.</summary>
        public void ShowError(string message, string title = "Ocurrió un error")
            => Modal.ShowError(message, title);

        /// <summary>Un botón "Aceptar" verde.</summary>
        public void ShowSuccess(string message, string title = "Operación exitosa")
            => Modal.ShowSuccess(message, title);

        /// <summary>Un botón "Aceptar" azul.</summary>
        public void ShowInfo(string message, string title = "Información")
            => Modal.ShowInfo(message, title);

        /// <summary>Dos botones: "Cancelar" + acción. Para confirmaciones.</summary>
        public void ShowConfirm(string message, string title = "¿Estás seguro?", string confirmText = "Confirmar")
            => Modal.ShowConfirm(message, title, confirmText);
    }
}
