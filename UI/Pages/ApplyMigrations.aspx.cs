using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Linq;
using DAL;
using DAL.Migrations;

namespace UI.Pages
{
    public partial class ApplyMigrations : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ValidarAcceso();
                CargarEstadoMigraciones();
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
        }

        private void CargarEstadoMigraciones()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var configuration = new Configuration();
                    var migrator = new DbMigrator(configuration);
                    var pendingMigrations = migrator.GetPendingMigrations();

                    LstMigraciones.Items.Clear();

                    var appliedMigrations = migrator.GetDatabaseMigrations().ToList();
                    foreach (var migration in appliedMigrations)
                    {
                        LstMigraciones.Items.Add(migration + " ✓ (Aplicada)");
                    }

                    if (pendingMigrations.Any())
                    {
                        LblEstado.Text = $"⚠ Hay {pendingMigrations.Count()} migración(es) pendiente(s) de aplicar.";
                        LblEstado.ForeColor = Color.Orange;
                        BtnAplicarMigraciones.Visible = true;

                        foreach (var pending in pendingMigrations)
                        {
                            LstMigraciones.Items.Add(pending + " ⏳ (Pendiente)");
                        }
                    }
                    else
                    {
                        LblEstado.Text = "✓ Todas las migraciones han sido aplicadas correctamente.";
                        LblEstado.ForeColor = Color.Green;
                        BtnAplicarMigraciones.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                LblEstado.Text = "Error al evaluar migraciones: " + ex.Message;
                LblEstado.ForeColor = Color.Red;
            }
        }

        protected void BtnAplicarMigraciones_Click(object sender, EventArgs e)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var configuration = new Configuration();
                    var migrator = new DbMigrator(configuration);
                    migrator.Update();

                    LblEstado.Text = "✓ Migraciones aplicadas exitosamente.";
                    LblEstado.ForeColor = Color.Green;
                    BtnAplicarMigraciones.Visible = false;

                    CargarEstadoMigraciones();
                }
            }
            catch (Exception ex)
            {
                LblEstado.Text = "Error al aplicar migraciones: " + ex.Message;
                LblEstado.ForeColor = Color.Red;
            }
        }
    }
}
