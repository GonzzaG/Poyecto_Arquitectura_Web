using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using BEL;

namespace DAL.Repository.Backup
{
    public class DatabaseBackupRepository
    {
        public List<DatabaseBackup> ObtenerTodos()
        {
            using (var context = new AppDbContext())
            {
                return context.DatabaseBackups
                    .Include("CreatedByUser")
                    .OrderByDescending(b => b.CreatedDate)
                    .ToList();
            }
        }

        public DatabaseBackup ObtenerPorId(int id)
        {
            using (var context = new AppDbContext())
            {
                return context.DatabaseBackups
                    .Include("CreatedByUser")
                    .FirstOrDefault(b => b.Id == id);
            }
        }

        public void Guardar(DatabaseBackup backup)
        {
            using (var context = new AppDbContext())
            {
                context.DatabaseBackups.Add(backup);
                context.SaveChanges();
            }
        }

        public string GenerarBackup(string backupPath)
        {

            var connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            var builder = new SqlConnectionStringBuilder(connString);
            string databaseName = builder.InitialCatalog;

            // Nombre del archivo con formato requerido
            string fileName = $"{databaseName}_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
            string fullPath = Path.Combine(backupPath, fileName);

            // Asegurar que la carpeta existe
            if (!Directory.Exists(backupPath))
                Directory.CreateDirectory(backupPath);

            // Ejecutar BACKUP DATABASE
            string sql = $"BACKUP DATABASE [{databaseName}] TO DISK = N'{fullPath}' WITH NOFORMAT, NOINIT, NAME = N'{databaseName}-Full Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10";

            using (var connection = new SqlConnection(connString))
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    // Timeout extendido para bases grandes
                    command.CommandTimeout = 300;
                    command.ExecuteNonQuery();
                }
            }

            return fullPath;
        }
    }
}