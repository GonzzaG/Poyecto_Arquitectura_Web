using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UI.Controls
{
    public partial class NotificationModal : UserControl
    {
        public string Type { get; set; } = "info";
        public string Mode { get; set; } = "info";
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string ButtonText { get; set; } = "Aceptar";
        public bool ShowOnLoad { get; set; } = false;

        protected void Page_Load(object sender, EventArgs e) { }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (ShowOnLoad)
                RegisterOpenScript();
        }

        // ═══════════════════════════════════════════════════════════════════════
        //  API PÚBLICA
        // ═══════════════════════════════════════════════════════════════════════

        /// <summary>
        /// ERROR — solo la X para cerrar, sin botones en el footer.
        /// </summary>
        public void ShowError(string message, string title = "Ocurrió un error")
        {
            Type = "error";
            Mode = "error";
            Title = title;
            Message = message;
            ShowOnLoad = true;
        }

        /// <summary>
        /// ÉXITO — un botón "Aceptar" verde.
        /// </summary>
        public void ShowSuccess(string message, string title = "Operación exitosa")
        {
            Type = "success";
            Mode = "success";
            Title = title;
            Message = message;
            ButtonText = "Aceptar";
            ShowOnLoad = true;
        }

        /// <summary>
        /// INFORMACIÓN — un botón "Aceptar" azul.
        /// </summary>
        public void ShowInfo(string message, string title = "Información")
        {
            Type = "info";
            Mode = "info";
            Title = title;
            Message = message;
            ButtonText = "Aceptar";
            ShowOnLoad = true;
        }

        /// <summary>
        /// CONFIRMACIÓN — dos botones: "Cancelar" y un botón de acción personalizable.
        /// Usá para acciones destructivas o irreversibles.
        /// </summary>
        public void ShowConfirm(string message, string title = "¿Estás seguro?", string confirmText = "Confirmar")
        {
            Type = "info";
            Mode = "confirm";
            Title = title;
            Message = message;
            ButtonText = confirmText;
            ShowOnLoad = true;
        }

        private void RegisterOpenScript()
        {
            string script = string.Format(
                "NotificationModal.open({{ type:'{0}', mode:'{1}', title:'{2}', message:'{3}', btnText:'{4}' }});",
                Sanitize(Type),
                Sanitize(Mode),
                Sanitize(Title),
                Sanitize(Message),
                Sanitize(ButtonText)
            );

            ScriptManager.RegisterStartupScript(
                control: this,
                type: GetType(),
                key: "OpenNotificationModal",
                script: script,
                addScriptTags: true
            );
        }

        private static string Sanitize(string value)
            => (value ?? string.Empty)
                .Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\"", "\\\"")
                .Replace("\r", "")
                .Replace("\n", "\\n");
    }
}