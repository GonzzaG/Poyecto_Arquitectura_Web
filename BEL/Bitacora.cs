using System;

namespace BEL
{
    public class Bitacora
    {
        public int IdBitacora { get; set; }
        public string Modulo { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaHora { get; set; }
        public string Criticidad { get; set; }
        public string Dvh { get; set; }

        public virtual Usuario Usuario { get; set; }
    }
}
