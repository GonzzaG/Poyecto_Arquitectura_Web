using System.Data.Entity.Migrations;

namespace DAL.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
            ContextKey = "DAL.AppDbContext";
        }
    }
}
