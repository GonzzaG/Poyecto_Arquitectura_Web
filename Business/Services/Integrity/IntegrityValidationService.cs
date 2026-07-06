using System;
using System.Collections.Generic;
using BEL;
using DAL;

namespace Business.Services.Integrity
{
    public sealed class IntegrityValidationService
    {
        public IntegrityValidationResult ValidarEstadoIntegridad()
        {
            try
            {
                var resultado = new IntegrityValidationResult();
                resultado.UsuariosConDVHInvalido = ValidarDVH.ValidarDVHTablaUsuarios();
                resultado.DVVRegistrado = ValidarDVH.ObtenerDVVRegistradoUsuarios();
                resultado.DVVCalculado = ValidarDVH.CalcularDVVUsuarios();
                resultado.DVVValido = resultado.DVVRegistrado == resultado.DVVCalculado;

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al validar integridad: " + ex.Message, ex);
            }
        }

        public List<Usuario> ValidarIntegridad()
        {
            return ValidarEstadoIntegridad().UsuariosConDVHInvalido;
        }

        public List<Usuario> ReparaIntegridad()
        {
            try
            {
                return ValidarDVH.RepararDVHDVVUsuarios();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al reparar integridad: " + ex.Message, ex);
            }
        }

        public bool ExistenRegistrosInvalidos()
        {
            return !ValidarEstadoIntegridad().EsValido;
        }
    }
}
