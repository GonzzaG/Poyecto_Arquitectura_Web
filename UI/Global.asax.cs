using System;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using Business.Services.Database;
using Business.Services.Integrity;

namespace UI
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            SincronizarBaseDatosSiEstaHabilitado();
            ValidarIntegridadSiEstaHabilitado();
        }

        private static void SincronizarBaseDatosSiEstaHabilitado()
        {
            bool sincronizar;
            if (!bool.TryParse(ConfigurationManager.AppSettings["SincronizarBaseDatosAlIniciar"], out sincronizar) || !sincronizar)
            {
                return;
            }

            try
            {
                new DatabaseMigrationService().Sincronizar();
            }
            catch (Exception ex)
            {
                Trace.TraceError("No se pudo sincronizar la base de datos al iniciar la aplicacion. " + ex);
                throw;
            }
        }

        private static void ValidarIntegridadSiEstaHabilitado()
        {
            try
            {
                // BREAKPOINT: Inicio de validación de integridad
                var integrityService = new IntegrityValidationService();
                // BREAKPOINT: Verificar si existen registros inválidos
                if (integrityService.ExistenRegistrosInvalidos())
                {
                    // BREAKPOINT: Se encontraron registros inválidos
                    Trace.TraceWarning("Se detectaron registros de usuario con dígitos verificadores inválidos. Se requiere ejecutar la reparación desde el panel de administración.");
                }
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMsg += " | Causa: " + ex.InnerException.Message;
                }

                // Log detallado del error
                Trace.TraceError($"Error al validar integridad de datos: {errorMsg}");

                // Si es error de migración, loguear instrucciones
                if (errorMsg.Contains("DVH") && errorMsg.Contains("migración"))
                {
                    Trace.TraceWarning("INSTRUCCIÓN: Accede a /Pages/ApplyMigrations.aspx (como WEBMASTER) para aplicar las migraciones pendientes.");
                }
            }
        }
    }
}
