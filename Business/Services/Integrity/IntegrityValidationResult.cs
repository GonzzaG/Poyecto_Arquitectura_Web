using System.Collections.Generic;
using System.Linq;
using BEL;

namespace Business.Services.Integrity
{
    public sealed class IntegrityValidationResult
    {
        public List<Usuario> UsuariosConDVHInvalido { get; set; }
        public bool DVVValido { get; set; }
        public int DVVRegistrado { get; set; }
        public int DVVCalculado { get; set; }

        public bool EsValido
        {
            get { return !UsuariosConDVHInvalido.Any() && DVVValido; }
        }

        public IntegrityValidationResult()
        {
            UsuariosConDVHInvalido = new List<Usuario>();
            DVVValido = true;
            DVVRegistrado = -1;
            DVVCalculado = -1;
        }
    }
}
