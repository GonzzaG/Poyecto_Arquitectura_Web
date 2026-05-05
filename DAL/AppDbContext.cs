using System.Data.Entity;
using BEL;

namespace DAL
{
    public class AppDbContext : DbContext
    {
        static AppDbContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AppDbContext, Migrations.Configuration>());
        }

        public AppDbContext() : base("name=DefaultConnection")
        {
        }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Bitacora> Bitacoras { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallesPedido { get; set; }
        public DbSet<Objeto> Objetos { get; set; }

        public DbSet<Sesion> Sesions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Rol>().ToTable("ROL");
            modelBuilder.Entity<Rol>().HasKey(x => x.IdRol);
            modelBuilder.Entity<Rol>().Property(x => x.IdRol).HasColumnName("id_rol");
            modelBuilder.Entity<Rol>().Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(80).IsRequired();

            modelBuilder.Entity<Usuario>().ToTable("USUARIO");
            modelBuilder.Entity<Usuario>().HasKey(x => x.IdUsuario);
            modelBuilder.Entity<Usuario>().Property(x => x.IdUsuario).HasColumnName("id_usuario");
            modelBuilder.Entity<Usuario>().Property(x => x.IdRol).HasColumnName("id_rol");
            modelBuilder.Entity<Usuario>().Property(x => x.Email).HasColumnName("email").HasMaxLength(160).IsRequired();
            modelBuilder.Entity<Usuario>().Property(x => x.Password).HasColumnName("password").HasMaxLength(255).IsRequired();
            modelBuilder.Entity<Usuario>().Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(120).IsRequired();
            modelBuilder.Entity<Usuario>().Property(x => x.Inactivo).HasColumnName("inactivo");
            modelBuilder.Entity<Usuario>().HasIndex(x => x.Email).IsUnique();
            modelBuilder.Entity<Usuario>()
                .HasRequired(x => x.Rol)
                .WithMany(x => x.Usuarios)
                .HasForeignKey(x => x.IdRol)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Bitacora>().ToTable("BITACORA");
            modelBuilder.Entity<Bitacora>().HasKey(x => x.IdBitacora);
            modelBuilder.Entity<Bitacora>().Property(x => x.IdBitacora).HasColumnName("id_bitacora");
            modelBuilder.Entity<Bitacora>().Property(x => x.FechaUtc).HasColumnName("fecha_utc").IsRequired();
            modelBuilder.Entity<Bitacora>().Property(x => x.TipoEvento).HasColumnName("tipo_evento").HasMaxLength(20).IsRequired();
            modelBuilder.Entity<Bitacora>().Property(x => x.Modulo).HasColumnName("modulo").HasMaxLength(80).IsRequired();
            modelBuilder.Entity<Bitacora>().Property(x => x.Accion).HasColumnName("accion").HasMaxLength(120).IsRequired();
            modelBuilder.Entity<Bitacora>().Property(x => x.Resultado).HasColumnName("resultado").HasMaxLength(20).IsRequired();
            modelBuilder.Entity<Bitacora>().Property(x => x.Mensaje).HasColumnName("mensaje").HasMaxLength(500).IsRequired();
            modelBuilder.Entity<Bitacora>().Property(x => x.Detalle).HasColumnName("detalle").HasMaxLength(4000);
            modelBuilder.Entity<Bitacora>().Property(x => x.IdUsuario).HasColumnName("id_usuario");
            modelBuilder.Entity<Bitacora>().Property(x => x.UsuarioEmail).HasColumnName("usuario_email").HasMaxLength(160);
            modelBuilder.Entity<Bitacora>().Property(x => x.Url).HasColumnName("url").HasMaxLength(260);
            modelBuilder.Entity<Bitacora>().Property(x => x.Ip).HasColumnName("ip").HasMaxLength(64);
            modelBuilder.Entity<Bitacora>()
                .HasOptional(x => x.Usuario)
                .WithMany(x => x.Bitacoras)
                .HasForeignKey(x => x.IdUsuario)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Pedido>().ToTable("PEDIDO");
            modelBuilder.Entity<Pedido>().HasKey(x => x.IdPedido);
            modelBuilder.Entity<Pedido>().Property(x => x.IdPedido).HasColumnName("id_pedido");
            modelBuilder.Entity<Pedido>().Property(x => x.IdUsuario).HasColumnName("id_usuario");
            modelBuilder.Entity<Pedido>().Property(x => x.FechaHora).HasColumnName("fecha_hora");
            modelBuilder.Entity<Pedido>().Property(x => x.Total).HasColumnName("total").HasPrecision(18, 2);
            modelBuilder.Entity<Pedido>().Property(x => x.DireccionEnvio).HasColumnName("direccion_envio").HasMaxLength(220).IsRequired();
            modelBuilder.Entity<Pedido>().Property(x => x.Finalizado).HasColumnName("finalizado");
            modelBuilder.Entity<Pedido>()
                .HasRequired(x => x.Usuario)
                .WithMany(x => x.Pedidos)
                .HasForeignKey(x => x.IdUsuario)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DetallePedido>().ToTable("DETALLE_PEDIDO");
            modelBuilder.Entity<DetallePedido>().HasKey(x => x.IdDetallePedido);
            modelBuilder.Entity<DetallePedido>().Property(x => x.IdDetallePedido).HasColumnName("id_detalle_pedido");
            modelBuilder.Entity<DetallePedido>().Property(x => x.IdPedido).HasColumnName("id_pedido");
            modelBuilder.Entity<DetallePedido>().Property(x => x.IdObjeto).HasColumnName("id_objeto");
            modelBuilder.Entity<DetallePedido>().Property(x => x.Cantidad).HasColumnName("cantidad");
            modelBuilder.Entity<DetallePedido>().Property(x => x.Finalizado).HasColumnName("finalizado");
            modelBuilder.Entity<DetallePedido>()
                .HasRequired(x => x.Pedido)
                .WithMany(x => x.Detalles)
                .HasForeignKey(x => x.IdPedido)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<DetallePedido>()
                .HasRequired(x => x.Objeto)
                .WithMany(x => x.DetallesPedido)
                .HasForeignKey(x => x.IdObjeto)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Objeto>().ToTable("OBJETO");
            modelBuilder.Entity<Objeto>().HasKey(x => x.IdObjeto);
            modelBuilder.Entity<Objeto>().Property(x => x.IdObjeto).HasColumnName("id_objeto");
            modelBuilder.Entity<Objeto>().Property(x => x.Precio).HasColumnName("precio").HasPrecision(18, 2);
            modelBuilder.Entity<Objeto>().Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(120).IsRequired();
            modelBuilder.Entity<Objeto>().Property(x => x.Descripcion).HasColumnName("descripcion").HasMaxLength(600).IsRequired();
            modelBuilder.Entity<Objeto>().Property(x => x.EsProducto).HasColumnName("esProducto");
            modelBuilder.Entity<Objeto>().Property(x => x.Stock).HasColumnName("stock");

            modelBuilder.Entity<Sesion>().ToTable("SESION");
            modelBuilder.Entity<Sesion>().HasKey(x => x.IdSession);
            modelBuilder.Entity<Sesion>().Property(x => x.IdSession).HasColumnName("id_sesion");
            modelBuilder.Entity<Sesion>().Property(x => x.Email).HasColumnName("email").HasMaxLength(160);
            modelBuilder.Entity<Sesion>().Property(x => x.FechaCreacion).HasColumnName("fecha_creacion");
        }
    }
}
