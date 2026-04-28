using System.Data.Entity;
using BEL;

namespace DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=DefaultConnection")
        {
        }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Bitacora> Bitacoras { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallesPedido { get; set; }
        public DbSet<Objeto> Objetos { get; set; }

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
            modelBuilder.Entity<Bitacora>().Property(x => x.Modulo).HasColumnName("modulo").HasMaxLength(80).IsRequired();
            modelBuilder.Entity<Bitacora>().Property(x => x.IdUsuario).HasColumnName("id_usuario");
            modelBuilder.Entity<Bitacora>().Property(x => x.FechaHora).HasColumnName("fecha_hora");
            modelBuilder.Entity<Bitacora>().Property(x => x.Criticidad).HasColumnName("criticidad").HasMaxLength(40).IsRequired();
            modelBuilder.Entity<Bitacora>().Property(x => x.Dvh).HasColumnName("dvh").HasMaxLength(255).IsRequired();
            modelBuilder.Entity<Bitacora>()
                .HasRequired(x => x.Usuario)
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
        }
    }
}
