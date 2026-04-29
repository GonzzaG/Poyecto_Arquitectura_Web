using System;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using Business.Services.Database;

namespace UI
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            SincronizarBaseDatosSiEstaHabilitado();
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
    }
}
