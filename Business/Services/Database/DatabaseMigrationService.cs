using DAL;

namespace Business.Services.Database
{
    public sealed class DatabaseMigrationService
    {
        public void Sincronizar()
        {
            DatabaseSynchronization.Sincronizar();
        }
    }
}
