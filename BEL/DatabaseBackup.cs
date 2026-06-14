using System;

namespace BEL
{
    public class DatabaseBackup
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public decimal FileSizeMB { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedByUserId { get; set; }

        public virtual Usuario CreatedByUser { get; set; }
    }
}
