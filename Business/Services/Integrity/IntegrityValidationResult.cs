using System.Collections.Generic;
using System.Linq;
using DAL;

namespace Business.Services.Integrity
{
    public sealed class IntegrityValidationResult
    {
        public List<IntegrityTableResult> Tablas { get; set; } = new List<IntegrityTableResult>();
        public bool EsValido { get { return Tablas.Count == 2 && Tablas.All(t => t.EsValido); } }
        public IEnumerable<IntegrityRecordResult> RegistrosInvalidos
        {
            get { return Tablas.SelectMany(t => t.Registros).Where(r => !r.EsValido); }
        }
    }
}
