using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using Microsoft.AspNet.FriendlyUrls;

namespace UI
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapPageRoute("Login", "Login", "~/Pages/Login.aspx");
            routes.MapPageRoute("Registro", "Registro", "~/Pages/Registro.aspx");
            routes.MapPageRoute("Bitacora", "Bitacora", "~/Pages/Bitacora.aspx");
            var settings = new FriendlyUrlSettings();
            settings.AutoRedirectMode = RedirectMode.Permanent;
            routes.EnableFriendlyUrls(settings);
        }
    }
}
