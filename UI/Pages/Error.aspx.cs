using System;
using System.Web.UI;
using System.Text.RegularExpressions;

namespace UI
{
    public partial class Error : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var referencia = Request.QueryString["ref"];
            if (string.IsNullOrWhiteSpace(referencia))
            {
                CodigoReferenciaLiteral.Text = "N/A";
                return;
            }

            var limpia = Regex.Replace(referencia.ToUpperInvariant(), "[^A-F0-9]", string.Empty);
            CodigoReferenciaLiteral.Text = limpia.Length > 8 ? limpia.Substring(0, 8) : limpia;
        }
    }
}
