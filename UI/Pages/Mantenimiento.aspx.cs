using System;
using System.Web;
using System.Web.UI;

namespace UI.Pages
{
    public partial class Mantenimiento : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void BtnCerrarSesionMantenimiento_Click(object sender, EventArgs e)
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
    }
}
