using System;
using System.Linq;
using BEL.Constantes;
using Business.Helper;
using Business.Services.Integrity;
using Business.Services.Usuarios;

namespace UI.Pages
{
    public partial class IntegrityValidation : System.Web.UI.Page
    {
        private readonly IntegrityValidationService _integrityService = new IntegrityValidationService();
        private readonly UsuarioService _usuarioService = new UsuarioService();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Revisar también en POST: ocultar un botón no autoriza una recuperación.
            if (!ValidarAcceso())
            {
                BtnRestaurar.Visible = false;
                BtnRecalcularTodo.Visible = false;
                Response.Redirect("~/Login", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            if (!IsPostBack) CargarEstadoIntegridad();
        }

        private bool ValidarAcceso()
        {
            var usuario = AutenticacionHelper.ObtenerUsuarioDesdeCookie(_usuarioService);
            return usuario != null && AutenticacionHelper.NivelAcceso((RolesEnum)usuario.IdRol)
                >= AutenticacionHelper.NivelAcceso(RolesEnum.WEBMASTER);
        }

        private void CargarEstadoIntegridad()
        {
            BtnRestaurar.Enabled = false;
            try
            {
                var estado = _integrityService.ValidarEstadoIntegridad();
                Application["IntegrityMaintenance"] = !estado.EsValido;
                LblEstado.Text = estado.EsValido
                    ? "Integridad confirmada: USUARIO y OBJETO son válidas."
                    : "Tablas dañadas: " + string.Join(", ", estado.Tablas.Where(t => !t.EsValido).Select(t => t.Tabla))
                        + ". Registros afectados: " + estado.RegistrosInvalidos.Count() + ".";
                LblEstado.ForeColor = estado.EsValido ? System.Drawing.Color.Green : System.Drawing.Color.OrangeRed;
                GvTablas.DataSource = estado.Tablas;
                GvTablas.DataBind();
                GvRegistros.DataSource = estado.RegistrosInvalidos.ToList();
                GvRegistros.DataBind();
                BtnRestaurar.Enabled = !estado.EsValido && estado.Tablas.All(t => t.RespaldoValido)
                    && estado.RegistrosInvalidos.All(r => r.Estado == "Datos alterados" || r.Estado == "DVH alterado");
            }
            catch (Exception ex)
            {
                Application["IntegrityMaintenance"] = true;
                LblEstado.Text = Server.HtmlEncode("No se pudo validar integridad: " + ex.Message);
                LblEstado.ForeColor = System.Drawing.Color.Red;
                GvTablas.DataSource = null;
                GvTablas.DataBind();
                GvRegistros.DataSource = null;
                GvRegistros.DataBind();
            }
        }

        protected void BtnValidar_Click(object sender, EventArgs e)
        {
            if (!ValidarAcceso()) return;
            LblResultado.Text = "";
            CargarEstadoIntegridad();
        }

        protected void BtnRecalcularTodo_Click(object sender, EventArgs e)
        {
            if (!ValidarAcceso()) return;
            try
            {
                int cantidad = _integrityService.RecalcularTodo();
                LblResultado.Text = "Se recalcularon DVH y DVV de USUARIO y OBJETO. "
                    + cantidad + " registros aceptados con sus valores actuales.";
                LblResultado.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                Application["IntegrityMaintenance"] = true;
                LblResultado.Text = Server.HtmlEncode("No se pudo recalcular: " + ex.Message);
                LblResultado.ForeColor = System.Drawing.Color.Red;
            }
            CargarEstadoIntegridad();
        }

        protected void BtnRestaurar_Click(object sender, EventArgs e)
        {
            if (!ValidarAcceso()) return;
            try
            {
                int cantidad = _integrityService.RestaurarDatosOriginales();
                LblResultado.Text = "Se recuperaron los valores originales de " + cantidad + " registros.";
                LblResultado.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                LblResultado.Text = Server.HtmlEncode("No se recuperaron los datos: " + ex.Message);
                LblResultado.ForeColor = System.Drawing.Color.Red;
            }
            CargarEstadoIntegridad();
        }
    }
}
