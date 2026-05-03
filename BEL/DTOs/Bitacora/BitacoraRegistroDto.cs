    using System;

namespace BEL.DTOs.Bitacora
{
    public sealed class BitacoraRegistroDto
    {
        public DateTime FechaUtc { get; set; }
        public string TipoEvento { get; set; }
        public string Modulo { get; set; }
        public string Accion { get; set; }
        public string Resultado { get; set; }
        public string Mensaje { get; set; }
        public string Detalle { get; set; }
        public int? IdUsuario { get; set; }
        public string UsuarioEmail { get; set; }
        public string Url { get; set; }
        public string Ip { get; set; }
        public string CodigoCorrelacion { get; set; }
        public Exception Exception { get; set; }
    }
}
