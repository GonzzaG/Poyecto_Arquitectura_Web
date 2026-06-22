using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEL;

namespace DAL
{
    public class ValidarDVH
    {
        /// <summary>
        /// Calcula el dígito verificador horizontal para un registro de usuario.
        /// </summary>
        /// <param name="usuario">El objeto Usuario con los datos del registro</param>
        /// <returns>El dígito verificador calculado</returns>
        public static int CalcularDVH(Usuario usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            // Concatenar todos los valores del registro en orden
            string datosRegistro = ConcatenarDatosUsuario(usuario);

            // Calcular DVH usando suma de valores ASCII
            int dvh = CalcularDVHDesdeString(datosRegistro);

            return dvh;
        }

        /// <summary>
        /// Valida que el dígito verificador horizontal de un usuario sea correcto.
        /// </summary>
        /// <param name="usuario">El objeto Usuario a validar</param>
        /// <param name="dvhRegistrado">El DVH registrado en la base de datos</param>
        /// <returns>True si el DVH es válido, False en caso contrario</returns>
        public static bool ValidarRegistroDVH(Usuario usuario, int dvhRegistrado)
        {
            if (usuario == null)
                return false;

            int dvhCalculado = CalcularDVH(usuario);
            return dvhCalculado == dvhRegistrado;
        }

        /// <summary>
        /// Valida todos los registros de usuarios en la base de datos.
        /// </summary>
        /// <returns>Una lista de usuarios con DVH inválido, vacía si todos son válidos</returns>
        public static List<Usuario> ValidarDVHTablaUsuarios()
        {
            List<Usuario> usuariosInvalidos = new List<Usuario>();

            try
            {
                using (AppDbContext context = new AppDbContext())
                {
                    // BREAKPOINT: Cargando todos los usuarios de la BD
                    var usuarios = context.Usuarios.ToList();

                    foreach (var usuario in usuarios)
                    {
                        try
                        {
                            // BREAKPOINT: Calculando DVH para cada usuario
                            int dvhCalculado = CalcularDVH(usuario);

                            // BREAKPOINT: Validando si DVH coincide
                            if (!usuario.DVH.HasValue || usuario.DVH.Value != dvhCalculado)
                            {
                                // BREAKPOINT: Agregando usuario con DVH inválido a la lista
                                usuariosInvalidos.Add(usuario);
                            }
                        }
                        catch (Exception exUsuario)
                        {
                            // Log de usuario problemático pero continuar con los demás
                            System.Diagnostics.Trace.TraceWarning($"Error validando usuario {usuario?.IdUsuario}: {exUsuario.Message}");
                        }
                    }
                }
            }
            catch (System.Data.SqlClient.SqlException sqlEx)
            {
                // La columna DVH probablemente no existe en la BD
                if (sqlEx.Message.Contains("DVH") || sqlEx.Message.Contains("invalid column"))
                {
                    throw new Exception("La columna DVH no existe en la tabla USUARIO. Se requiere aplicar la migración. Error: " + sqlEx.Message);
                }
                throw new Exception("Error de SQL al validar DVH: " + sqlEx.Message);
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException efEx)
            {
                // Error de Entity Framework - probablemente columna no existe
                if (efEx.InnerException != null && efEx.InnerException.Message.Contains("DVH"))
                {
                    throw new Exception("La columna DVH no existe en la tabla USUARIO. Se requiere aplicar la migración.");
                }
                throw new Exception("Error de Entity Framework al validar DVH: " + efEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al validar DVH de la tabla usuarios: " + ex.Message, ex);
            }

            return usuariosInvalidos;
        }

        /// <summary>
        /// Repara los registros de usuarios con DVH inválido recalculando y actualizando los valores.
        /// </summary>
        /// <returns>Una lista de usuarios que fueron reparados</returns>
        public static List<Usuario> RepararDVHRegistrosInvalidos()
        {
            List<Usuario> usuariosReparados = new List<Usuario>();

            try
            {
                using (AppDbContext context = new AppDbContext())
                {
                    // BREAKPOINT: Cargando usuarios para reparación
                    var usuarios = context.Usuarios.ToList();

                    foreach (var usuario in usuarios)
                    {
                        try
                        {
                            // BREAKPOINT: Calculando DVH correcto
                            int dvhCalculado = CalcularDVH(usuario);

                            // BREAKPOINT: Verificando si necesita reparación
                            if (!usuario.DVH.HasValue || usuario.DVH.Value != dvhCalculado)
                            {
                                // BREAKPOINT: Actualizando DVH del usuario
                                usuario.DVH = dvhCalculado;
                                usuariosReparados.Add(usuario);
                            }
                        }
                        catch (Exception exUsuario)
                        {
                            System.Diagnostics.Trace.TraceWarning($"Error reparando usuario {usuario?.IdUsuario}: {exUsuario.Message}");
                        }
                    }

                    // BREAKPOINT: Guardando cambios en la base de datos
                    if (usuariosReparados.Count > 0)
                    {
                        try
                        {
                            context.SaveChanges();
                        }
                        catch (System.Data.Entity.Core.EntityCommandExecutionException saveEx)
                        {
                            if (saveEx.InnerException != null && saveEx.InnerException.Message.Contains("DVH"))
                            {
                                throw new Exception("No se puede guardar cambios: La columna DVH no existe. Se requiere aplicar la migración.");
                            }
                            throw;
                        }
                    }
                }
            }
            catch (System.Data.SqlClient.SqlException sqlEx)
            {
                if (sqlEx.Message.Contains("DVH") || sqlEx.Message.Contains("invalid column"))
                {
                    throw new Exception("La columna DVH no existe en la tabla USUARIO. Se requiere aplicar la migración. Error: " + sqlEx.Message);
                }
                throw new Exception("Error de SQL al reparar DVH: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al reparar DVH de la tabla usuarios: " + ex.Message, ex);
            }

            return usuariosReparados;
        }

        /// <summary>
        /// Recalcula y actualiza el DVH de un usuario específico.
        /// </summary>
        /// <param name="idUsuario">ID del usuario a actualizar</param>
        /// <returns>True si fue actualizado, False si no existía</returns>
        public static bool ActualizarDVHUsuario(int idUsuario)
        {
            try
            {
                using (AppDbContext context = new AppDbContext())
                {
                    var usuario = context.Usuarios.FirstOrDefault(u => u.IdUsuario == idUsuario);
                    if (usuario == null)
                        return false;

                    int dvhCalculado = CalcularDVH(usuario);
                    usuario.DVH = dvhCalculado;
                    context.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar DVH del usuario {idUsuario}: " + ex.Message);
            }
        }

        /// <summary>
        /// Concatena todos los datos del usuario en un string para calcular el DVH
        /// </summary>
        private static string ConcatenarDatosUsuario(Usuario usuario)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(usuario.IdUsuario.ToString());
            sb.Append(usuario.IdRol.ToString());
            sb.Append(usuario.Email ?? "");
            sb.Append(usuario.Password ?? "");
            sb.Append(usuario.Nombre ?? "");
            sb.Append(usuario.Inactivo ? "1" : "0");
            sb.Append(usuario.IntentosFallidos.ToString());
            sb.Append(usuario.FechaBloqueo?.ToString("yyyyMMddHHmmss") ?? "");

            return sb.ToString();
        }

        /// <summary>
        /// Calcula el DVH a partir de un string usando suma ponderada de valores ASCII
        /// </summary>
        private static int CalcularDVHDesdeString(string datos)
        {
            if (string.IsNullOrEmpty(datos))
                return 0;

            int suma = 0;
            int peso = 1;

            foreach (char c in datos)
            {
                suma += (int)c * peso;
                peso++;
            }

            // Obtener el dígito verificador (último dígito de la suma)
            int dvh = suma % 10;

            return dvh;
        }
    } 
}
