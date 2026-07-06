using System.Data.Entity.Migrations;
using System.Linq;
using BEL;

namespace DAL.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
            ContextKey = "DAL.AppDbContext";
        }

        protected override void Seed(AppDbContext context)
        {
            SeedProducto(context, "Split inverter 3000 frigorias", "Equipo split frio/calor con tecnologia inverter para ambientes medianos.", 920000m, 8, "~/Images/Productos/split-inverter.png");
            SeedProducto(context, "Termostato WiFi programable", "Termostato inteligente con control remoto y programacion semanal.", 185000m, 15, "~/Images/Productos/termostato-wifi.png");
            SeedProducto(context, "Filtro HEPA alta eficiencia", "Filtro HEPA para sistemas HVAC, retiene particulas finas y mejora la calidad del aire.", 76000m, 30, "~/Images/Productos/filtro-hepa.png");
            SeedProducto(context, "Compresor scroll 5TR", "Compresor scroll para equipos comerciales de climatizacion y refrigeracion.", 640000m, 6, "~/Images/Productos/compresor-scroll.png");
            SeedProducto(context, "Unidad condensadora comercial", "Unidad condensadora para camaras frigorificas y sistemas comerciales.", 1250000m, 4, "~/Images/Productos/unidad-condensadora.png");
            SeedProducto(context, "Ventilador centrifugo", "Ventilador centrifugo para conductos de ventilacion y extraccion.", 310000m, 10, "~/Images/Productos/ventilador-centrifugo.png");
        }

        private static void SeedProducto(AppDbContext context, string nombre, string descripcion, decimal precio, int stock, string imagenUrl)
        {
            if (context.Objetos.Any(x => x.Nombre == nombre))
            {
                return;
            }

            context.Objetos.Add(new Objeto
            {
                Nombre = nombre,
                Descripcion = descripcion,
                Precio = precio,
                EsProducto = true,
                Stock = stock,
                ImagenUrl = imagenUrl
            });
        }
    }
}
