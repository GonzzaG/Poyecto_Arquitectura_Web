using System;
using System.Collections.Generic;
using System.Web.UI;

namespace UI
{
    public partial class Bitacora : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var eventos = new List<BitacoraEvento>
                {
                    new BitacoraEvento("Usuarios", "admin@hvac.local", DateTime.Today.AddHours(9).AddMinutes(15), "Media", "A91F-32B0"),
                    new BitacoraEvento("Pedidos", "ventas@hvac.local", DateTime.Today.AddHours(10).AddMinutes(40), "Alta", "B42C-10FE"),
                    new BitacoraEvento("Stock", "proveedor@hvac.local", DateTime.Today.AddHours(11).AddMinutes(5), "Baja", "C88D-77AC")
                };

                Eventos.DataSource = eventos;
                Eventos.DataBind();

                EventosMobile.DataSource = eventos;
                EventosMobile.DataBind();
            }
        }

        private sealed class BitacoraEvento
        {
            public BitacoraEvento(string modulo, string usuario, DateTime fechaHora, string criticidad, string dvh)
            {
                Modulo = modulo;
                Usuario = usuario;
                FechaHora = fechaHora;
                Criticidad = criticidad;
                Dvh = dvh;
            }

            public string Modulo { get; private set; }
            public string Usuario { get; private set; }
            public DateTime FechaHora { get; private set; }
            public string Criticidad { get; private set; }
            public string Dvh { get; private set; }
        }
    }
}
