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
                    BtnReparar.Visible = false;
                    GvRegistrosInvalidos.Visible = false;
                    Application["IntegrityMaintenance"] = false;
                }
                else
                {
                    LblEstado.Text = $"Se detecto error de integridad. Usuarios con DVH invalido: {usuariosInvalidos.Count}. DVV valido: {(estado.DVVValido ? "si" : "no")}.";
                    LblEstado.ForeColor = System.Drawing.Color.OrangeRed;
                    BtnReparar.Visible = true;
                    Application["IntegrityMaintenance"] = true;

                    var registrosConDVH = usuariosInvalidos.Select(u => new
                    {
                        u.IdUsuario,
                        u.Email,
                        u.Nombre,
                        u.IdRol,
                        u.DVH,
                        DVHCorrecto = ValidarDVH.CalcularDVH(u)
                    }).ToList();

                    GvRegistrosInvalidos.Visible = true;
                    GvRegistrosInvalidos.DataSource = registrosConDVH;
                    GvRegistrosInvalidos.DataBind();

                    for (int i = 0; i < GvRegistrosInvalidos.Rows.Count; i++)
                    {
                        var lbl = GvRegistrosInvalidos.Rows[i].FindControl("LblDVHCorrecto") as System.Web.UI.WebControls.Label;
                        if (lbl != null && i < registrosConDVH.Count)
                        {
                            lbl.Text = registrosConDVH[i].DVHCorrecto.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Application["IntegrityMaintenance"] = true;
                LblEstado.Text = "Error al validar integridad: " + ex.Message;
                LblEstado.ForeColor = System.Drawing.Color.Red;
                BtnReparar.Visible = true;
            }
        }

        protected void BtnValidar_Click(object sender, EventArgs e)
        {
            CargarEstadoIntegridad();
        }

        protected void BtnReparar_Click(object sender, EventArgs e)
        {
            try
            {
                var usuariosReparados = _integrityService.ReparaIntegridad();
                Application["IntegrityMaintenance"] = false;
                LblEstado.Text = $"Se recalcularon DVH/DVV correctamente. Usuarios con DVH reparado: {usuariosReparados.Count}.";
                LblEstado.ForeColor = System.Drawing.Color.Green;
                BtnReparar.Visible = false;
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
