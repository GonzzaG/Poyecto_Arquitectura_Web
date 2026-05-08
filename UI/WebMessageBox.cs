using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace UI
{
    public static class WebMessageBox
    {
        public static void Show(this Page page, string message)
        {
            string safe = HttpUtility.JavaScriptStringEncode(message);
            page.ClientScript.RegisterStartupScript(
                page.GetType(),
                "MessageBox",
                $"<script>alert('{safe}');</script>"
            );
        }
    }
}