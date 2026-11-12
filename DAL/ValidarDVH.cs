using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using BEL;

namespace DAL
{
    public static class ValidarDVH
    {
        internal const string Pendiente = "PENDIENTE_SHA256";
        private static readonly string[] Tablas = { "USUARIO", "OBJETO" };

        // XML distingue límites de campos, nulos y vacíos. Números y fechas son invariantes.
        private static XElement Campo(string nombre, object valor)
        {
            return valor == null ? new XElement(nombre, new XAttribute("null", true))
                : new XElement(nombre, Convert.ToString(valor, CultureInfo.InvariantCulture));
        }

        private static string Datos(Usuario u)
        {
            return new XElement("USUARIO", Campo("IdUsuario", u.IdUsuario), Campo("IdRol", u.IdRol),
                Campo("Email", u.Email), Campo("Password", u.Password), Campo("Nombre", u.Nombre),
                Campo("Inactivo", u.Inactivo), Campo("IntentosFallidos", u.IntentosFallidos),
                Campo("FechaBloqueo", u.FechaBloqueo?.ToString("O", CultureInfo.InvariantCulture)))
                .ToString(SaveOptions.DisableFormatting);
        }

        private static string Datos(Objeto o)
        {
            return new XElement("OBJETO", Campo("IdObjeto", o.IdObjeto),
                Campo("Precio", o.Precio.ToString("F2", CultureInfo.InvariantCulture)), Campo("Nombre", o.Nombre),
                Campo("Descripcion", o.Descripcion), Campo("EsProducto", o.EsProducto),
                Campo("Stock", o.Stock), Campo("ImagenUrl", o.ImagenUrl)).ToString(SaveOptions.DisableFormatting);
        }

        public static string CalcularDVH(Usuario usuario) { return Hash(Datos(usuario)); }
        public static string CalcularDVH(Objeto objeto) { return Hash(Datos(objeto)); }

        private static string Hash(string datos)
        {
            using (var sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(datos))).Replace("-", "");
        }

        private static string Vertical(string tabla, IEnumerable<IntegritySnapshot> filas)
        {
            return Hash(tabla + ":" + string.Join("|", filas.OrderBy(r => r.RegistroId)
                .Select(r => r.RegistroId.ToString(CultureInfo.InvariantCulture) + ":" + r.DVH)));
        }

        private static List<IntegritySnapshot> LeerDatos(AppDbContext context)
        {
            return context.Usuarios.AsNoTracking().ToList().Select(u => new IntegritySnapshot
            {
                Tabla = "USUARIO", RegistroId = u.IdUsuario, Datos = Datos(u), DVH = u.DVH
            }).Concat(context.Objetos.AsNoTracking().ToList().Select(o => new IntegritySnapshot
            {
                Tabla = "OBJETO", RegistroId = o.IdObjeto, Datos = Datos(o), DVH = o.DVH
            })).ToList();
        }

        public static IntegrityReport ValidarEstadoIntegridad()
        {
            using (var context = new AppDbContext())
            using (var transaction = context.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                var resultado = ValidarEstadoIntegridad(context);
                transaction.Commit();
                return resultado;
            }
        }

        internal static IntegrityReport ValidarEstadoIntegridad(AppDbContext context)
        {
            var actuales = LeerDatos(context);
            var copias = context.IntegritySnapshots.AsNoTracking().ToList();
            var verticales = context.DigitosVerificadoresVerticales.AsNoTracking().ToList();
            var resultado = new IntegrityReport();
            foreach (var tabla in Tablas)
            {
                var filas = actuales.Where(r => r.Tabla == tabla).ToDictionary(r => r.RegistroId);
                var respaldos = copias.Where(r => r.Tabla == tabla).ToDictionary(r => r.RegistroId);
                var dvv = verticales.SingleOrDefault(v => v.Tabla == tabla)?.Valor;
                var estado = new IntegrityTableResult
                {
                    Tabla = tabla, DVVRegistrado = dvv,
                    DVVCalculado = Vertical(tabla, filas.Values.Select(r => new IntegritySnapshot
                    { RegistroId = r.RegistroId, DVH = Hash(r.Datos) })),
                    RespaldoValido = respaldos.Values.All(r => Hash(r.Datos) == r.DVH)
                        && dvv == Vertical(tabla, respaldos.Values)
                };
                foreach (int id in filas.Keys.Union(respaldos.Keys).OrderBy(x => x))
                {
                    IntegritySnapshot fila, copia;
                    filas.TryGetValue(id, out fila);
                    respaldos.TryGetValue(id, out copia);
                    string descripcion;
                    try { descripcion = (string)XElement.Parse((fila ?? copia).Datos).Element("Nombre"); }
                    catch (System.Xml.XmlException) { descripcion = "Copia de recuperación ilegible"; }
                    string calculado = fila == null ? null : Hash(fila.Datos);
                    estado.Registros.Add(new IntegrityRecordResult
                    {
                        Tabla = tabla, RegistroId = id,
                        Descripcion = descripcion,
                        DVHRegistrado = fila?.DVH, DVHCalculado = calculado,
                        Estado = fila == null ? "Eliminado" : copia == null ? "Agregado sin respaldo"
                            : Hash(copia.Datos) != copia.DVH ? "Copia de recuperación alterada"
                            : calculado != copia.DVH ? "Datos alterados"
                            : fila.DVH != calculado ? "DVH alterado" : "Válido"
                    });
                }
                resultado.Tablas.Add(estado);
            }
            return resultado;
        }

        // Evita que una escritura legítima selle como válidos datos que ya estaban dañados.
        internal static void BloquearTablas(AppDbContext context)
        {
            foreach (string tabla in new[] { "USUARIO", "OBJETO", "DIGITO_VERIFICADOR_VERTICAL", "INTEGRITY_SNAPSHOT" })
                context.Database.SqlQuery<int>("SELECT COUNT(*) FROM dbo.[" + tabla + "] WITH (TABLOCKX, HOLDLOCK)").Single();
        }

        internal static void ExigirIntegridad(AppDbContext context)
        {
            if (!ValidarEstadoIntegridad(context).EsValido)
                throw new InvalidOperationException("Hay errores de integridad en USUARIO u OBJETO. Revise Integridad antes de guardar cambios.");
        }

        // Solo se llama dentro de la misma transacción que la escritura o la migración inicial.
        internal static void GuardarVerificadores(AppDbContext context)
        {
            var filas = LeerDatos(context);
            var copias = context.IntegritySnapshots.ToList();
            foreach (var copia in copias.Where(c => !filas.Any(f => f.Tabla == c.Tabla && f.RegistroId == c.RegistroId)))
                context.IntegritySnapshots.Remove(copia);
            foreach (var fila in filas)
            {
                fila.DVH = Hash(fila.Datos);
                string columnaId = fila.Tabla == "USUARIO" ? "id_usuario" : "id_objeto";
                context.Database.ExecuteSqlCommand("UPDATE dbo.[" + fila.Tabla + "] SET DVH = @p0 WHERE " + columnaId + " = @p1", fila.DVH, fila.RegistroId);
                foreach (var entry in context.ChangeTracker.Entries<Usuario>().Where(e => fila.Tabla == "USUARIO" && e.Entity.IdUsuario == fila.RegistroId))
                {
                    entry.Property(u => u.DVH).CurrentValue = fila.DVH;
                    entry.Property(u => u.DVH).OriginalValue = fila.DVH;
                }
                foreach (var entry in context.ChangeTracker.Entries<Objeto>().Where(e => fila.Tabla == "OBJETO" && e.Entity.IdObjeto == fila.RegistroId))
                {
                    entry.Property(o => o.DVH).CurrentValue = fila.DVH;
                    entry.Property(o => o.DVH).OriginalValue = fila.DVH;
                }
                var copia = copias.SingleOrDefault(c => c.Tabla == fila.Tabla && c.RegistroId == fila.RegistroId);
                if (copia == null)
                {
                    copia = new IntegritySnapshot { Tabla = fila.Tabla, RegistroId = fila.RegistroId };
                    context.IntegritySnapshots.Add(copia);
                }
                copia.Datos = fila.Datos;
                copia.DVH = fila.DVH;
            }
            foreach (string tabla in Tablas)
            {
                var dvv = context.DigitosVerificadoresVerticales.SingleOrDefault(v => v.Tabla == tabla);
                if (dvv == null)
                    throw new InvalidOperationException("Falta el DVV de " + tabla + ". Restaure un backup confiable.");
                dvv.Valor = Vertical(tabla, filas.Where(f => f.Tabla == tabla));
            }
        }

        internal static void InicializarDuranteMigracion(AppDbContext context)
        {
            var verticales = context.DigitosVerificadoresVerticales.ToList();
            if (Tablas.All(t => verticales.Any(v => v.Tabla == t && v.Valor == Pendiente)))
            {
                context.OmitirControlIntegridad = true;
                try
                {
                    context.SaveChanges(); // SQL normaliza fechas/decimales y asigna IDs.
                    using (var tx = context.Database.BeginTransaction(IsolationLevel.Serializable))
                    {
                        BloquearTablas(context);
                        if (context.IntegritySnapshots.Any())
                            throw new InvalidOperationException("Ya existe una copia de integridad. No se permite reinicializarla.");
                        GuardarVerificadores(context);
                        context.SaveChanges();
                        tx.Commit();
                    }
                }
                finally { context.OmitirControlIntegridad = false; }
            }
            else
            {
                context.SaveChanges(); // Seed posterior: conserva y valida el respaldo existente.
            }
        }

        public static int RecalcularTodo()
        {
            using (var context = new AppDbContext())
            using (var tx = context.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                BloquearTablas(context);
                // Acción explícita: los datos actuales pasan a ser la nueva referencia.
                context.OmitirControlIntegridad = true;
                foreach (string tabla in Tablas)
                    if (!context.DigitosVerificadoresVerticales.Any(v => v.Tabla == tabla))
                        context.DigitosVerificadoresVerticales.Add(new DigitoVerificadorVertical
                        { Tabla = tabla, Valor = Pendiente });
                context.SaveChanges();
                GuardarVerificadores(context);
                context.SaveChanges();
                ExigirIntegridad(context);
                int cantidad = context.Usuarios.Count() + context.Objetos.Count();
                tx.Commit();
                return cantidad;
            }
        }

        public static int RestaurarDatosOriginales()
        {
            using (var context = new AppDbContext())
            using (var tx = context.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                BloquearTablas(context);
                var estado = ValidarEstadoIntegridad(context);
                if (estado.Tablas.Any(t => !t.RespaldoValido))
                    throw new InvalidOperationException("El DVV o la copia de recuperación no son confiables. Restaure un backup de la base de datos.");
                var danados = estado.Tablas.SelectMany(t => t.Registros).Where(r => !r.EsValido).ToList();
                if (danados.Any(r => r.Estado == "Eliminado" || r.Estado == "Agregado sin respaldo"))
                    throw new InvalidOperationException("Hay filas eliminadas o agregadas desde SQL. Se identificaron sus IDs; restaure un backup para conservar las relaciones.");
                foreach (var registro in danados)
                {
                    var copia = context.IntegritySnapshots.Single(c => c.Tabla == registro.Tabla && c.RegistroId == registro.RegistroId);
                    var xml = XElement.Parse(copia.Datos);
                    if (registro.Tabla == "USUARIO")
                    {
                        var u = context.Usuarios.Single(x => x.IdUsuario == registro.RegistroId);
                        u.IdRol = (int)xml.Element("IdRol");
                        u.Email = (string)xml.Element("Email");
                        u.Password = (string)xml.Element("Password");
                        u.Nombre = (string)xml.Element("Nombre");
                        u.Inactivo = bool.Parse((string)xml.Element("Inactivo"));
                        u.IntentosFallidos = (int)xml.Element("IntentosFallidos");
                        var fecha = xml.Element("FechaBloqueo");
                        u.FechaBloqueo = fecha.Attribute("null") != null ? (DateTime?)null
                            : DateTime.Parse(fecha.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                        u.DVH = copia.DVH;
                    }
                    else
                    {
                        var o = context.Objetos.Single(x => x.IdObjeto == registro.RegistroId);
                        o.Precio = (decimal)xml.Element("Precio");
                        o.Nombre = (string)xml.Element("Nombre");
                        o.Descripcion = (string)xml.Element("Descripcion");
                        o.EsProducto = bool.Parse((string)xml.Element("EsProducto"));
                        o.Stock = (int)xml.Element("Stock");
                        var imagen = xml.Element("ImagenUrl");
                        o.ImagenUrl = imagen.Attribute("null") != null ? null : imagen.Value;
                        o.DVH = copia.DVH;
                    }
                }
                context.OmitirControlIntegridad = true;
                context.SaveChanges();
                ExigirIntegridad(context);
                tx.Commit();
                return danados.Count;
            }
        }
    }
}
