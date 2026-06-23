using System;
using System.Collections.Generic;
using System.Linq;
using BEL;
using BEL.Exceptions;
using DAL;

namespace Business.Services.Integrity
{
    public sealed class IntegrityValidationService
    {
        /// <summary>
        /// Valida la integridad de los dígitos verificadores horizontales de la tabla USUARIO.
        /// </summary>
        /// <returns>Una lista de usuarios con DVH inválido</returns>
        public List<Usuario> ValidarIntegridad()
        {
            try
            {
                // BREAKPOINT: Iniciando validación de integridad
                var resultado = ValidarDVH.ValidarDVHTablaUsuarios();

                if (resultado != null && resultado.Count > 0)
                {

                    var mensaje = string.Concat(resultado);
                    throw new Exception("Los siguientes usuarios se encuentran corruptos: " + mensaje);
                }
                // BREAKPOINT: Validación completada - revisar resultado
                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al validar integridad: " + ex.Message);
            }
        }

        /// <summary>
        /// Repara los registros con DVH inválido recalculando y actualizando los valores correctos.
        /// </summary>
        /// <returns>Una lista de usuarios que fueron reparados</returns>
        public List<Usuario> ReparaIntegridad()
        {
            try
            {
                // BREAKPOINT: Iniciando reparación de integridad
                var resultado = ValidarDVH.RepararDVHRegistrosInvalidos();
                // BREAKPOINT: Reparación completada - revisar cantidad de registros reparados
                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al reparar integridad: " + ex.Message);
            }
        }

        /// <summary>
        /// Verifica si existen registros con DVH inválido.
        /// </summary>
        /// <returns>True si hay registros inválidos, False en caso contrario</returns>
        public bool ExistenRegistrosInvalidos()
        {
            // BREAKPOINT: Verificando si existen registros inválidos
            var usuariosInvalidos = ValidarDVH.ValidarDVHTablaUsuarios();

            if (usuariosInvalidos != null && usuariosInvalidos.Count > 0)
            {

                var mensaje = string.Concat(usuariosInvalidos.Select(u => u.IdUsuario) + " ");

                throw new AppException("Los siguientes usuarios se encuentran corruptos: " + mensaje);
            }

            // BREAKPOINT: Resultado de la verificación
            return usuariosInvalidos.Count > 0;

        }
    }
}
