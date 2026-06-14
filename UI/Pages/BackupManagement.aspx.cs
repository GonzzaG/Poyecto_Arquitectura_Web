using BEL;
using BEL.Constantes;
using Business.Helper;
using Business.Services.Backup;
using Business.Services.Usuarios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UI.Pages
{
    public partial class BackupManagement : Page
    {
        private readonly DatabaseBackupService _backupService = new DatabaseBackupService();
        private readonly UsuarioService _usuarioService = new UsuarioService();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Verificar rol WebMaster
            if (!UsuarioTieneRol(RolesEnum.WEBMASTER))
            {
                MostrarMensaje("Acceso denegado. Solo los usuarios con rol WebMaster pueden acceder a esta sección.", esError: true);
                btnGenerarBackup.Visible = false;
                return;
            }

            if (!IsPostBack)
                CargarGrilla();
        }

        private void CargarGrilla()
        {
            var lista = _backupService.ObtenerTodos();
            PanelSinResultados.Visible = lista.Count == 0;

            rptBackups.DataSource = lista;
            rptBackups.DataBind();

            rptBackupsMobile.DataSource = lista;
            rptBackupsMobile.DataBind();
        }

        private void MostrarMensaje(string texto, bool esError)
        {
            var rol = Session["UsuarioRol"] as string;
            if(rol != RolesEnum.WEBMASTER.ToString())
            {
                pnlMensaje.Visible = true;
                string style = esError
                    ? "background:#fef2f2; color:#991b1b; border:1px solid #fecaca; border-radius:0.5rem; padding:0.75rem 1rem; font-size:0.875rem; font-weight:500;"
                    : "background:#f0fdf4; color:#166534; border:1px solid #bbf7d0; border-radius:0.5rem; padding:0.75rem 1rem; font-size:0.875rem; font-weight:500;";
                lblMensaje.Text = $"<div style=\"{style}\">{HttpUtility.HtmlEncode(texto)}</div>";
            }
           
        }

        protected void lbDescargar_Command(object sender, CommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);
            DatabaseBackup backup = _backupService.ObtenerPorId(id);

            if (backup == null || !File.Exists(backup.FilePath))
            {
                MostrarMensaje("El archivo no existe en el servidor.", esError: true);
                return;
            }

            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", $"attachment; filename=\"{backup.FileName}\"");
            Response.AddHeader("Content-Length", new FileInfo(backup.FilePath).Length.ToString());
            Response.TransmitFile(backup.FilePath);
            Response.End();
        }

        protected void btnGenerarBackup_Click(object sender, EventArgs e)
        {
            if (!UsuarioTieneRol(RolesEnum.WEBMASTER))
            {
                MostrarMensaje("Acceso denegado.", esError: true);
                return;
            }

            try
            {
                int userId = ObtenerUsuarioId();
                DatabaseBackup backup = _backupService.GenerarBackup(userId);
                MostrarMensaje($"Backup generado correctamente: {backup.FileName} ({backup.FileSizeMB} MB)", esError: false);
                CargarGrilla();
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al generar el backup: {ex.Message}", esError: true);
            }
        }

        protected void gvBackups_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Descargar")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                DatabaseBackup backup = _backupService.ObtenerPorId(id);

                if (backup == null || !File.Exists(backup.FilePath))
                {
                    MostrarMensaje("El archivo no existe en el servidor.", esError: true);
                    return;
                }

                Response.Clear();
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", $"attachment; filename=\"{backup.FileName}\"");
                Response.AddHeader("Content-Length", new FileInfo(backup.FilePath).Length.ToString());
                Response.TransmitFile(backup.FilePath);
                Response.End();
            }
        }

        private bool UsuarioTieneRol(RolesEnum rol)
        {
            Usuario usuario = AutenticacionHelper.ObtenerUsuarioDesdeCookie(_usuarioService);
            if (usuario == null) return false;
            return AutenticacionHelper.NivelAcceso((RolesEnum)usuario.IdRol)
                   >= AutenticacionHelper.NivelAcceso(rol);
        }

        private int ObtenerUsuarioId()
        {
            Usuario usuario = AutenticacionHelper.ObtenerUsuarioDesdeCookie(_usuarioService);
            if (usuario == null)
                throw new InvalidOperationException("No se encontró el usuario autenticado.");
            return usuario.IdUsuario;
        }
    }
}