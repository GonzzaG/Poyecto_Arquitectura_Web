using System;
using System.Data.Entity.Migrations;
using DAL.Migrations;

class Program
{
    static void Main()
    {
        try
        {
            Console.WriteLine("╔════════════════════════════════════════════════╗");
            Console.WriteLine("║      Aplicando Migraciones de Base de Datos    ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝\n");

            var configuration = new Configuration();
            var migrator = new DbMigrator(configuration);

            Console.WriteLine("📋 Migraciones pendientes:");
            var pendingMigrations = migrator.GetPendingMigrations();

            if (pendingMigrations.Count() == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✅ No hay migraciones pendientes. Base de datos actualizada.");
                Console.ResetColor();
                return;
            }

            foreach (var migration in pendingMigrations)
            {
                Console.WriteLine($"  • {migration}");
            }

            Console.WriteLine("\n⏳ Aplicando migraciones...\n");
            migrator.Update();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n✅ ¡Migraciones aplicadas exitosamente!");
            Console.ResetColor();

            Console.WriteLine("\n📊 Migraciones aplicadas:");
            var appliedMigrations = migrator.GetDatabaseMigrations();
            foreach (var migration in appliedMigrations.OrderBy(x => x))
            {
                Console.WriteLine($"  ✓ {migration}");
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Error: {ex.Message}");
            Console.ResetColor();

            if (ex.InnerException != null)
            {
                Console.WriteLine($"\nCausa: {ex.InnerException.Message}");
            }

            Environment.Exit(1);
        }
    }
}
