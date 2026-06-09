using System.Web.UI;

namespace UI
{
    public abstract class BasePage : Page
    {
        protected SiteMaster SiteMaster => (SiteMaster)Master;

        /// <summary>Solo X para cerrar. Sin botones.</summary>
        protected void ShowError(string message, string title = "Ocurrió un error")
            => SiteMaster.ShowError(message, title);

        /// <summary>Un botón "Aceptar" verde.</summary>
        protected void ShowSuccess(string message, string title = "Operación exitosa")
            => SiteMaster.ShowSuccess(message, title);

        /// <summary>Un botón "Aceptar" azul.</summary>
        protected void ShowInfo(string message, string title = "Información")
            => SiteMaster.ShowInfo(message, title);

        /// <summary>Dos botones: "Cancelar" + acción. Para confirmaciones.</summary>
        protected void ShowConfirm(string message, string title = "¿Estás seguro?", string confirmText = "Confirmar")
            => SiteMaster.ShowConfirm(message, title, confirmText);
    }
}