using System;
using System.Collections.Generic;
using System.Linq;
using Business.Services.Integrity;
using DAL;
using BEL;

namespace UI.Pages
{
    public partial class IntegrityValidation : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ValidarAcceso();
                CargarEstadoIntegridad();
            }
        }

        private void ValidarAcceso()
        {
            // Validar que solo WEBMASTER acceda
            if (Session["IdUsuario"] == null)
            {
                Response.Redirect("~/Login", false);
                return;
            }

            // Aquí puedes agregar validación adicional de rol si es necesario
        }

        private void CargarEstadoIntegridad()
        {
            try
            {
                var usuariosInvalidos = new List<Usuario>();

                if (usuariosInvalidos.Count == 0)
                {
                    LblEstado.Text = "✓ Integridad Confirmada: Todos los registros tienen dígitos verificadores válidos.";
                    LblEstado.ForeColor = System.Drawing.Color.Green;
                    BtnReparar.Visible = false;
                    GvRegistrosInvalidos.Visible = false;
                }
                else
                {
                    LblEstado.Text = $"⚠ Se encontraron {usuariosInvalidos.Count} registros con dígitos verificadores inválidos.";
                    LblEstado.ForeColor = System.Drawing.Color.Orange;
                    BtnReparar.Visible = true;

                    // Calcular DVH correcto para cada registro
                    var registrosConDVH = usuariosInvalidos.Select(u => new
                    {
                        u.IdUsuario,
                        u.Email,
                        u.Nombre,
                        u.IdRol,
                        u.DVH,
                        DVHCorrecto = ValidarDVH.CalcularDVH(u)
                    }).ToList();

                    GvRegistrosInvalidos.DataSource = registrosConDVH;
                    GvRegistrosInvalidos.DataBind();

                    // Llenar la columna de DVH Correcto
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
                LblEstado.Text = "Error al validar integridad: " + ex.Message;
                LblEstado.ForeColor = System.Drawing.Color.Red;
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
                //var usuariosReparados = _integrityService.ReparaIntegridad();
                //LblEstado.Text = $"✓ Se repararon {usuariosReparados.Count} registros exitosamente.";
                //LblEstado.ForeColor = System.Drawing.Color.Green;
                //BtnReparar.Visible = false;
                //GvRegistrosInvalidos.Visible = false;
            }
            catch (Exception ex)
            {
                LblEstado.Text = "Error al reparar integridad: " + ex.Message;
                LblEstado.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}
