using BEL;
using DAL.Repository.Backup;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Business.Services.Backup
{
    public class DatabaseBackupService
    {
        private readonly DatabaseBackupRepository _repository;

        public DatabaseBackupService()
        {
            _repository = new DatabaseBackupRepository();
        }

        public List<DatabaseBackup> ObtenerTodos()
        {
            return _repository.ObtenerTodos();
        }

        public DatabaseBackup ObtenerPorId(int id)
        {
            return _repository.ObtenerPorId(id);
        }

        public DatabaseBackup GenerarBackup(int userId)
        {

            string backupPath = ConfigurationManager.AppSettings["BackupPath"];

            if (string.IsNullOrEmpty(backupPath))
                throw new InvalidOperationException("La clave 'BackupPath' no está configurada en Web.config.");

            string fullPath = _repository.GenerarBackup(backupPath);

            // Validar que el archivo fue creado
            if (!File.Exists(fullPath))
                throw new IOException($"El archivo de backup no fue creado: {fullPath}");

            // Obtener tamaño en MB
            var fileInfo = new FileInfo(fullPath);
            decimal sizeMB = Math.Round((decimal)fileInfo.Length / (1024 * 1024), 2);

            // Registrar en la base de datos
            var backup = new DatabaseBackup
            {
                FileName = Path.GetFileName(fullPath),
                FilePath = fullPath,
                FileSizeMB = sizeMB,
                CreatedDate = DateTime.Now,
                CreatedByUserId = userId
            };

            _repository.Guardar(backup);

            return backup;
        }
    }
}
