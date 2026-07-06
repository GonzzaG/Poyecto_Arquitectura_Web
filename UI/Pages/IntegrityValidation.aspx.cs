using System;
using System.Linq;
using BEL.Constantes;
using Business.Helper;
using Business.Services.Integrity;
using Business.Services.Usuarios;
using DAL;

namespace UI.Pages
{
    public partial class IntegrityValidation : System.Web.UI.Page
    {
        private readonly IntegrityValidationService _integrityService = new IntegrityValidationService();
        private readonly UsuarioService _usuarioService = new UsuarioService();

        protected void Page_Load(object sender, EventArgs e)
        {
            MostrarBotonRecalcular();

            if (!IsPostBack)
            {
                if (!ValidarAcceso())
                {
                    Response.Redirect("~/Login", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                CargarEstadoIntegridad();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            MostrarBotonRecalcular();
            base.OnPreRender(e);
        }

        private void MostrarBotonRecalcular()
        {
            LnkRecalcularTodos.Visible = true;
        }

        private bool ValidarAcceso()
        {
            var usuario = AutenticacionHelper.ObtenerUsuarioDesdeCookie(_usuarioService);
            if (usuario == null)
            {
                return false;
            }

            return AutenticacionHelper.NivelAcceso((RolesEnum)usuario.IdRol)
                >= AutenticacionHelper.NivelAcceso(RolesEnum.WEBMASTER);
        }

        private void CargarEstadoIntegridad()
        {
            try
            {
                var estado = _integrityService.ValidarEstadoIntegridad();
                var usuariosInvalidos = estado.UsuariosConDVHInvalido;

                LblDVV.Text = $"DVV registrado: {estado.DVVRegistrado} | DVV calculado: {estado.DVVCalculado}";
                LblDVV.ForeColor = estado.DVVValido ? System.Drawing.Color.Green : System.Drawing.Color.OrangeRed;

                if (estado.EsValido)
                {
                    LblEstado.Text = "Integridad confirmada: los DVH y el DVV de usuarios son validos.";
                    LblEstado.ForeColor = System.Drawing.Color.Green;
                    MostrarBotonRecalcular();
                    Application["IntegrityMaintenance"] = false;
                }
                else
                {
                    LblEstado.Text = $"Se detecto error de integridad. Usuarios con DVH invalido: {usuariosInvalidos.Count}. DVV valido: {(estado.DVVValido ? "si" : "no")}.";
                    LblEstado.ForeColor = System.Drawing.Color.OrangeRed;
                    MostrarBotonRecalcular();
                    Application["IntegrityMaintenance"] = true;
                }

                CargarUsuarios();
            }
            catch (Exception ex)
            {
                Application["IntegrityMaintenance"] = true;
                LblEstado.Text = "Error al validar integridad: " + ex.Message;
                LblEstado.ForeColor = System.Drawing.Color.Red;
                MostrarBotonRecalcular();
            }
        }

        private void CargarUsuarios()
        {
            var usuarios = _usuarioService.ObtenerTodos()
                .OrderBy(u => u.IdUsuario)
                .Select(u =>
                {
                    int dvhCalculado = ValidarDVH.CalcularDVH(u);
                    bool valido = u.DVH.HasValue && u.DVH.Value == dvhCalculado;

                    return new
                    {
                        u.IdUsuario,
                        u.Email,
                        u.Nombre,
                        u.IdRol,
                        DVH = u.DVH.HasValue ? u.DVH.Value.ToString() : "NULL",
                        DVHCorrecto = dvhCalculado,
                        Estado = valido ? "Valido" : "Recalcular"
                    };
                })
                .ToList();

            GvUsuarios.Visible = true;
            GvUsuarios.DataSource = usuarios;
            GvUsuarios.DataBind();
        }

        protected void BtnValidar_Click(object sender, EventArgs e)
        {
            CargarEstadoIntegridad();
        }

        protected void RecalcularTodos_Click(object sender, EventArgs e)
        {
            RecalcularTodos();
        }

        private void RecalcularTodos()
        {
            try
            {
                var usuariosReparados = _integrityService.ReparaIntegridad();
                Application["IntegrityMaintenance"] = false;
                LblEstado.Text = $"Se recalcularon DVH/DVV correctamente. Usuarios con DVH reparado: {usuariosReparados.Count}.";
                LblEstado.ForeColor = System.Drawing.Color.Green;
                MostrarBotonRecalcular();
                CargarEstadoIntegridad();
            }
            catch (Exception ex)
            {
                Application["IntegrityMaintenance"] = true;
                LblEstado.Text = "Error al reparar integridad: " + ex.Message;
                LblEstado.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}
