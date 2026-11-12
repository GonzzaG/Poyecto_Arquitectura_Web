using DAL;

namespace Business.Services.Integrity
{
    public sealed class IntegrityValidationService
    {
        public IntegrityValidationResult ValidarEstadoIntegridad()
        {
            return new IntegrityValidationResult { Tablas = ValidarDVH.ValidarEstadoIntegridad().Tablas };
        }

        public int RestaurarDatosOriginales() { return ValidarDVH.RestaurarDatosOriginales(); }
        public int RecalcularTodo() { return ValidarDVH.RecalcularTodo(); }
        public bool ExistenRegistrosInvalidos() { return !ValidarEstadoIntegridad().EsValido; }
    }
}
