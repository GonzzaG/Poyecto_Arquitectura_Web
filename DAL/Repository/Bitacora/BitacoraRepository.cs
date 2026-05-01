using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using BEL;
using BEL.DTOs.Bitacora;

namespace DAL.Repository.Bitacora
{
    public sealed class BitacoraRepository
    {
        public void Insertar(BitacoraRegistroDto registro)
        {
            if (registro == null)
            {
                return;
            }

            try
            {
                using (var context = new AppDbContext())
                {
                    context.Bitacoras.Add(new BEL.Bitacora
                    {
                        FechaUtc = registro.FechaUtc == default(DateTime) ? DateTime.UtcNow : registro.FechaUtc,
                        TipoEvento = Limitar(registro.TipoEvento, 20, "ACCION"),
                        Modulo = Limitar(registro.Modulo, 80, "General"),
                        Accion = Limitar(registro.Accion, 120, "SinAccion"),
                        Resultado = Limitar(registro.Resultado, 20, "OK"),
                        Mensaje = Limitar(registro.Mensaje, 500, "Sin mensaje"),
                        Detalle = Limitar(registro.Detalle, 4000, null),
                        IdUsuario = registro.IdUsuario,
                        UsuarioEmail = Limitar(registro.UsuarioEmail, 160, null),
                        Url = Limitar(registro.Url, 260, null),
                        Ip = Limitar(registro.Ip, 64, null)
                    });

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("No se pudo persistir bitacora. " + ex);
                PersistirFallbackEnArchivo(registro, ex);
            }
        }

        public IList<BitacoraRegistroDto> ObtenerRecientes(int cantidadMaxima)
        {
            var cantidad = cantidadMaxima <= 0 ? 200 : cantidadMaxima;

            try
            {
                using (var context = new AppDbContext())
                {
                    return context.Bitacoras
                        .OrderByDescending(x => x.FechaUtc)
                        .Take(cantidad)
                        .Select(x => new BitacoraRegistroDto
                        {
                            FechaUtc = x.FechaUtc,
                            TipoEvento = x.TipoEvento,
                            Modulo = x.Modulo,
                            Accion = x.Accion,
                            Resultado = x.Resultado,
                            UsuarioEmail = x.UsuarioEmail,
                            Mensaje = x.Mensaje,
                            Url = x.Url,
                            IdUsuario = x.IdUsuario,
                            Ip = x.Ip,
                            Detalle = x.Detalle
                        })
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("No se pudo obtener bitacora. " + ex);
                return new List<BitacoraRegistroDto>();
            }
        }


        public IList<BitacoraRegistroDto> ObtenerFiltrado(
            string busqueda,
            int pagina,
            int tamanioPagina,
            out int totalRegistros)
        {
            var paginaSegura = pagina <= 0 ? 1 : pagina;
            var tamanioSeguro = tamanioPagina <= 0 ? 10 : tamanioPagina;

            try
            {
                using (var context = new AppDbContext())
                {
                    var query = context.Bitacoras.AsQueryable();

                    if (!string.IsNullOrWhiteSpace(busqueda))
                    {
                        query = query.Where(x =>
                            (x.TipoEvento != null && x.TipoEvento.Contains(busqueda)) ||
                            (x.Modulo != null && x.Modulo.Contains(busqueda)) ||
                            (x.Accion != null && x.Accion.Contains(busqueda)) ||
                            (x.Resultado != null && x.Resultado.Contains(busqueda)) ||
                            (x.UsuarioEmail != null && x.UsuarioEmail.Contains(busqueda)) ||
                            (x.Mensaje != null && x.Mensaje.Contains(busqueda)) ||
                            (x.Url != null && x.Url.Contains(busqueda)) ||
                            (x.Ip != null && x.Ip.Contains(busqueda)));
                    }

                    totalRegistros = query.Count();

                    return query
                        .OrderByDescending(x => x.FechaUtc)
                        .Skip((paginaSegura - 1) * tamanioSeguro)
                        .Take(tamanioSeguro)
                        .Select(x => new BitacoraRegistroDto
                        {
                            FechaUtc = x.FechaUtc,
                            TipoEvento = x.TipoEvento,
                            Modulo = x.Modulo,
                            Accion = x.Accion,
                            Resultado = x.Resultado,
                            UsuarioEmail = x.UsuarioEmail,
                            Mensaje = x.Mensaje,
                            Url = x.Url,
                            IdUsuario = x.IdUsuario,
                            Ip = x.Ip,
                            Detalle = x.Detalle
                        })
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("No se pudo obtener bitácora filtrada. " + ex);
                totalRegistros = 0;
                return new List<BitacoraRegistroDto>();
            }
        }



        private static string Limitar(string valor, int maxLength, string fallback)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return fallback;
            }

            var normalizado = valor.Trim();
            return normalizado.Length <= maxLength ? normalizado : normalizado.Substring(0, maxLength);
        }

        private static void PersistirFallbackEnArchivo(BitacoraRegistroDto registro, Exception exception)
        {
            try
            {
                var appDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
                Directory.CreateDirectory(appDataPath);

                var filePath = Path.Combine(appDataPath, "bitacora-fallback.log");
                var line = new StringBuilder();
                line.Append(DateTime.UtcNow.ToString("o"));
                line.Append(" | REF:");
                line.Append(string.IsNullOrWhiteSpace(registro?.CodigoCorrelacion) ? "-" : registro.CodigoCorrelacion);
                line.Append(" | MOD:");
                line.Append(registro?.Modulo ?? "-");
                line.Append(" | ACC:");
                line.Append(registro?.Accion ?? "-");
                line.Append(" | MSG:");
                line.Append(registro?.Mensaje ?? "-");
                line.Append(" | ERR:");
                line.Append(exception?.Message ?? "-");

                File.AppendAllText(filePath, line + Environment.NewLine);
            }
            catch
            {
            }
        }
    }
}
