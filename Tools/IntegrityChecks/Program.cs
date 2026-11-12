using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Resources;
using System.Text;
using System.Xml;
using BEL;
using Business.Services.Integrity;
using Business.Services.Usuarios;
using DAL;

class Program
{
    static int checks;
    static void Assert(bool ok, string message)
    {
        if (!ok) throw new Exception("FAIL: " + message);
        Console.WriteLine("PASS: " + message);
        checks++;
    }
    static void Sql(string sql, params object[] args)
    {
        using (var db = new AppDbContext()) db.Database.ExecuteSqlCommand(sql, args);
    }
    static IntegrityValidationResult Estado() { return new IntegrityValidationService().ValidarEstadoIntegridad(); }
    static bool Falla(Action action)
    {
        try { action(); return false; } catch (InvalidOperationException) { return true; }
    }
    static void Main(string[] args)
    {
        using (var context = new AppDbContext()) { }
        Database.SetInitializer<AppDbContext>(null);
        if (args.Length == 2 && args[0] == "--model")
        {
            string model;
            using (var context = new AppDbContext())
            using (var sw = new StringWriter())
            {
                using (var xml = XmlWriter.Create(sw)) EdmxWriter.WriteEdmx(context, xml);
                using (var ms = new MemoryStream())
                {
                    using (var zip = new GZipStream(ms, CompressionMode.Compress, true))
                    {
                        var data = Encoding.UTF8.GetBytes(sw.ToString().Replace("encoding=\"utf-16\"", "encoding=\"utf-8\""));
                        zip.Write(data, 0, data.Length);
                    }
                    model = Convert.ToBase64String(ms.ToArray());
                }
            }
            using (var writer = new ResXResourceWriter(args[1])) writer.AddResource("Target", model);
            Console.WriteLine("Migration model generated.");
            return;
        }
        using (var db = new AppDbContext())
        {
            if (!db.Database.Connection.Database.StartsWith("ArquitecturaWeb_IntegrityChecks_"))
                throw new Exception("Use exclusivamente una base temporal ArquitecturaWeb_IntegrityChecks_<guid>.");
            if (db.Database.Exists()) throw new Exception("La base temporal ya existe; no se modifica.");
        }
        try
        {
            var migrator = new DbMigrator(new DAL.Migrations.Configuration());
            Assert(migrator.GetLocalMigrations().Contains("202701021200000_IntegridadUsuariosObjetosSha256"), "Migración SHA-256 reconocida por EF");
            if (args.Contains("--legacy"))
            {
                // Preparar esquema anterior con DVH/DVV enteros y filas existentes.
                string script = new MigratorScriptingDecorator(migrator).ScriptUpdate(null, "202606142135062_AgregarUrlAObjeto");
                using (var db = new AppDbContext())
                {
                    var cs = new SqlConnectionStringBuilder(db.Database.Connection.ConnectionString);
                    string name = cs.InitialCatalog;
                    cs.InitialCatalog = "master";
                    using (var sql = new SqlConnection(cs.ConnectionString))
                    {
                        sql.Open();
                        using (var cmd = new SqlCommand("CREATE DATABASE [" + name + "]", sql)) cmd.ExecuteNonQuery();
                    }
                }
                SqlConnection.ClearAllPools(); // Invalidar el fallo de conexión previo a CREATE DATABASE.
                foreach (string batch in Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase))
                    if (!string.IsNullOrWhiteSpace(batch)) Sql(batch);
                Sql("ALTER TABLE dbo.USUARIO ADD DVH int NULL; CREATE TABLE dbo.DIGITO_VERIFICADOR_VERTICAL (id_dvv int IDENTITY PRIMARY KEY, tabla nvarchar(80) NOT NULL, valor int NOT NULL); CREATE UNIQUE INDEX IX_tabla ON dbo.DIGITO_VERIFICADOR_VERTICAL(tabla); INSERT dbo.DIGITO_VERIFICADOR_VERTICAL(tabla,valor) VALUES('USUARIO',5);");
                migrator = new DbMigrator(new DAL.Migrations.Configuration());
            }
            migrator.Update();
            Assert(!migrator.GetPendingMigrations().Any(), "Migración aplicada y modelo actualizado");
            Assert(Estado().EsValido, "Instalación limpia: ambas tablas válidas");
            int usuario, objeto;
            decimal precio;
            using (var db = new AppDbContext())
            {
                usuario = db.Usuarios.Single(u => u.Email == "cliente@demo.com").IdUsuario;
                var producto = db.Objetos.OrderBy(o => o.IdObjeto).First();
                objeto = producto.IdObjeto;
                precio = producto.Precio;
            }
            Sql(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../IntegridadDemo.sql")));
            var roto = Estado();
            Assert(roto.Tablas.All(t => !t.EsValido) && roto.RegistrosInvalidos.Count() == 2, "Detecta dos tablas y exactamente dos registros alterados");
            Assert(roto.RegistrosInvalidos.Any(r => r.Tabla == "USUARIO" && r.RegistroId == usuario)
                && roto.RegistrosInvalidos.Any(r => r.Tabla == "OBJETO" && r.RegistroId == objeto), "Identifica tabla e ID de cada registro");
            new UsuarioService().Login("cliente@demo.com", "Demo123!");
            new UsuarioService().Login("webmaster@demo.com", "Demo123!");
            Assert(Estado().RegistrosInvalidos.Count() == 2, "Login del usuario afectado y webmaster conserva la evidencia");
            try { new UsuarioService().Login("cliente@demo.com", "incorrecta"); } catch { }
            Assert(Estado().RegistrosInvalidos.Count() == 2, "Login fallido tampoco recalcula datos dañados");
            Assert(Falla(() => {
                using (var db = new AppDbContext()) { db.Objetos.Find(objeto).Stock--; db.SaveChanges(); }
            }), "Una escritura legítima no acepta la corrupción previa");
            Assert(new IntegrityValidationService().RestaurarDatosOriginales() == 2 && Estado().EsValido, "Recuperación restaura datos originales de ambas tablas");
            using (var db = new AppDbContext())
                Assert(db.Objetos.Find(objeto).Precio == precio && !db.Usuarios.Find(usuario).Nombre.Contains("ALTERADO"), "Recuperación devuelve precio y nombre anteriores");
            new UsuarioService().Login("cliente@demo.com", "Demo123!");
            Assert(Estado().EsValido, "Login normal mantiene DVH y DVV válidos");
            for (int intento = 0; intento < 3; intento++)
                try { new UsuarioService().Login("cliente@demo.com", "incorrecta"); } catch { }
            using (var db = new AppDbContext())
            {
                var u = db.Usuarios.Find(usuario);
                Assert(u.IntentosFallidos == 3 && u.FechaBloqueo.HasValue && Estado().EsValido,
                    "Bloqueo por intentos fallidos conserva la integridad con fechas normalizadas por SQL");
            }
            new DAL.Repository.Usuarios.UsuarioRepository().ActualizarBloqueo("cliente@demo.com", 0, null);
            using (var db = new AppDbContext())
            {
                var producto = db.Objetos.Find(objeto);
                producto.Stock++;
                db.SaveChangesAsync().GetAwaiter().GetResult();
            }
            Assert(Estado().EsValido, "Escritura async actualiza verificadores");
            new DAL.Repository.Pedido.PedidoRepository().CrearPedido(usuario,
                new[] { new CarritoItem { IdObjeto = objeto, Cantidad = 1 } }, "Dirección de prueba");
            Assert(Estado().EsValido, "Compra con descuento de stock mantiene ambas tablas válidas");
            Sql("UPDATE dbo.OBJETO SET DVH = REPLICATE('0',64) WHERE id_objeto = @p0", objeto);
            Assert(Estado().RegistrosInvalidos.Single().Estado == "DVH alterado", "Detecta alteración del DVH aunque los datos sean correctos");
            new IntegrityValidationService().RestaurarDatosOriginales();
            Sql("UPDATE dbo.DIGITO_VERIFICADOR_VERTICAL SET valor = REPLICATE('0',64) WHERE tabla = 'OBJETO'");
            Assert(!Estado().EsValido && !Estado().Tablas.Single(t => t.Tabla == "OBJETO").DVVValido, "Detecta DVV alterado");
            Assert(Falla(() => new IntegrityValidationService().RestaurarDatosOriginales()), "No recupera usando una referencia no confiable");
            // Revertir exactamente el valor esperado para continuar las pruebas.
            Sql("UPDATE dbo.DIGITO_VERIFICADOR_VERTICAL SET valor = @p0 WHERE tabla = 'OBJETO'", Estado().Tablas.Single(t => t.Tabla == "OBJETO").DVVCalculado);
            Sql("UPDATE dbo.OBJETO SET stock = stock + 10 WHERE id_objeto = @p0", objeto);
            migrator.Update();
            Assert(Estado().RegistrosInvalidos.Any(r => r.RegistroId == objeto && r.Tabla == "OBJETO"), "Ejecutar migraciones/Seed otra vez no oculta alteraciones");
            new IntegrityValidationService().RestaurarDatosOriginales();
            int extra;
            using (var db = new AppDbContext())
            {
                var nuevo = new Objeto { Nombre = "Temporal", Descripcion = "Prueba", Precio = 1, Stock = 1, EsProducto = true };
                db.Objetos.Add(nuevo); db.SaveChanges(); extra = nuevo.IdObjeto;
            }
            Assert(Estado().EsValido, "Alta legítima guarda DVH con el ID generado y respaldo");
            Sql("DELETE dbo.OBJETO WHERE id_objeto = @p0", extra);
            Assert(Estado().RegistrosInvalidos.Any(r => r.Estado == "Eliminado" && r.RegistroId == extra), "Identifica ID de fila eliminada desde SQL");
            Assert(Falla(() => new IntegrityValidationService().RestaurarDatosOriginales()), "Fila eliminada requiere backup y no genera falsa reparación");
            Sql("SET IDENTITY_INSERT dbo.OBJETO ON; INSERT dbo.OBJETO(id_objeto, precio, nombre, descripcion, esProducto, stock) VALUES(@p0,1,'Temporal','Prueba',1,1); SET IDENTITY_INSERT dbo.OBJETO OFF;", extra);
            new IntegrityValidationService().RestaurarDatosOriginales();
            Sql("INSERT dbo.OBJETO(precio,nombre,descripcion,esProducto,stock) VALUES(1,'Sin respaldo','Prueba',1,1)");
            Assert(Estado().RegistrosInvalidos.Single().Estado == "Agregado sin respaldo", "Detecta fila insertada directamente desde SQL");
            Sql("DELETE dbo.OBJETO WHERE nombre = 'Sin respaldo'");
            var a = new Objeto { IdObjeto = 5, Precio = 100.23m, Nombre = "ab", Descripcion = "c" };
            var b = new Objeto { IdObjeto = 5, Precio = 100.23m, Nombre = "a", Descripcion = "bc" };
            Assert(ValidarDVH.CalcularDVH(a) != ValidarDVH.CalcularDVH(b), "Campos con igual concatenación producen distintos hashes");
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("es-AR");
            string hash = ValidarDVH.CalcularDVH(a);
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            Assert(hash == ValidarDVH.CalcularDVH(a) && hash.Length == 64, "SHA-256 estable entre culturas y de 64 caracteres");
            Assert(Estado().EsValido, "Estado final válido");
            Sql("UPDATE dbo.INTEGRITY_SNAPSHOT SET Datos = '<ilegible' WHERE Tabla = 'OBJETO' AND RegistroId = @p0", objeto);
            Assert(!Estado().Tablas.Single(t => t.Tabla == "OBJETO").RespaldoValido, "Detecta copia de recuperación dañada sin ocultar el diagnóstico");
            Assert(Falla(() => new IntegrityValidationService().RestaurarDatosOriginales()), "Rechaza recuperación con copia dañada");
            new IntegrityValidationService().RecalcularTodo();
            Assert(Estado().EsValido, "Recálculo explícito reconstruye la referencia desde los datos actuales");
            Sql(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../IntegridadDemo.sql")));
            Assert(Estado().RegistrosInvalidos.Count() == 2, "Alteraciones siguen detectándose antes del recálculo explícito");
            int recalculados = new IntegrityValidationService().RecalcularTodo();
            Assert(recalculados > 0 && Estado().EsValido, "Recalcular todo valida ambas tablas y actualiza verificadores y copias");
            using (var db = new AppDbContext())
                Assert(db.Usuarios.Find(usuario).Nombre.Contains("ALTERADO") && db.Objetos.Find(objeto).Precio == precio + 123m,
                    "Recalcular conserva el nombre y el precio modificados desde SQL");
            Console.WriteLine("OK: " + checks + " comprobaciones.");
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); Environment.ExitCode = 1; }
        finally { using (var db = new AppDbContext()) db.Database.Delete(); }
    }
}
