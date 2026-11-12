using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public sealed class IntegrityReport
    {
        public List<IntegrityTableResult> Tablas { get; set; } = new List<IntegrityTableResult>();
        public bool EsValido { get { return Tablas.Count == 2 && Tablas.All(t => t.EsValido); } }
    }

    public sealed class IntegrityTableResult
    {
        public string Tabla { get; set; }
        public string DVVRegistrado { get; set; }
        public string DVVCalculado { get; set; }
        public bool RespaldoValido { get; set; }
        public bool DVVValido { get { return DVVRegistrado == DVVCalculado; } }
        public List<IntegrityRecordResult> Registros { get; set; } = new List<IntegrityRecordResult>();
        public int RegistrosInvalidos { get { return Registros.Count(r => !r.EsValido); } }
        public bool EsValido { get { return DVVValido && RespaldoValido && RegistrosInvalidos == 0; } }
        public string Estado { get { return EsValido ? "Válida" : "Dañada"; } }
        public string Detalle
        {
            get
            {
                if (!RespaldoValido) return "DVV o copia de recuperación inválidos. Restaurar un backup confiable.";
                return DVVValido ? "DVV válido" : "El contenido de la tabla no coincide con el DVV guardado.";
            }
        }
    }

    public sealed class IntegrityRecordResult
    {
        public string Tabla { get; set; }
        public int RegistroId { get; set; }
        public string Descripcion { get; set; }
        public string DVHRegistrado { get; set; }
        public string DVHCalculado { get; set; }
        public string Estado { get; set; }
        public bool EsValido { get { return Estado == "Válido"; } }
    }
}
