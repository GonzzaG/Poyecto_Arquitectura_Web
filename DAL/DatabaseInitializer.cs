using System.Data.Entity;

namespace DAL
{
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<AppDbContext>());

            using (var context = new AppDbContext())
            {
                context.Database.Initialize(false);
            }
        }
    }
}
