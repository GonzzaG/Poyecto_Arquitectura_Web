namespace DAL.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ROL",
                c => new
                {
                    id_rol = c.Int(nullable: false, identity: true),
                    nombre = c.String(nullable: false, maxLength: 80),
                })
                .PrimaryKey(t => t.id_rol);

            CreateTable(
                "dbo.USUARIO",
                c => new
                {
                    id_usuario = c.Int(nullable: false, identity: true),
                    id_rol = c.Int(nullable: false),
                    email = c.String(nullable: false, maxLength: 160),
                    password = c.String(nullable: false, maxLength: 255),
                    nombre = c.String(nullable: false, maxLength: 120),
                    inactivo = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.id_usuario)
                .ForeignKey("dbo.ROL", t => t.id_rol)
                .Index(t => t.id_rol)
                .Index(t => t.email, unique: true);

            CreateTable(
                "dbo.OBJETO",
                c => new
                {
                    id_objeto = c.Int(nullable: false, identity: true),
                    precio = c.Decimal(nullable: false, precision: 18, scale: 2),
                    nombre = c.String(nullable: false, maxLength: 120),
                    descripcion = c.String(nullable: false, maxLength: 600),
                    esProducto = c.Boolean(nullable: false),
                    stock = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.id_objeto);

            CreateTable(
                "dbo.PEDIDO",
                c => new
                {
                    id_pedido = c.Int(nullable: false, identity: true),
                    id_usuario = c.Int(nullable: false),
                    fecha_hora = c.DateTime(nullable: false),
                    total = c.Decimal(nullable: false, precision: 18, scale: 2),
                    direccion_envio = c.String(nullable: false, maxLength: 220),
                    finalizado = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.id_pedido)
                .ForeignKey("dbo.USUARIO", t => t.id_usuario)
                .Index(t => t.id_usuario);

            CreateTable(
                "dbo.DETALLE_PEDIDO",
                c => new
                {
                    id_detalle_pedido = c.Int(nullable: false, identity: true),
                    id_pedido = c.Int(nullable: false),
                    id_objeto = c.Int(nullable: false),
                    cantidad = c.Int(nullable: false),
                    finalizado = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.id_detalle_pedido)
                .ForeignKey("dbo.PEDIDO", t => t.id_pedido, cascadeDelete: true)
                .ForeignKey("dbo.OBJETO", t => t.id_objeto)
                .Index(t => t.id_pedido)
                .Index(t => t.id_objeto);

            CreateTable(
                "dbo.BITACORA",
                c => new
                {
                    id_bitacora = c.Int(nullable: false, identity: true),
                    modulo = c.String(nullable: false, maxLength: 80),
                    id_usuario = c.Int(nullable: false),
                    fecha_hora = c.DateTime(nullable: false),
                    criticidad = c.String(nullable: false, maxLength: 40),
                    dvh = c.String(nullable: false, maxLength: 255),
                })
                .PrimaryKey(t => t.id_bitacora)
                .ForeignKey("dbo.USUARIO", t => t.id_usuario)
                .Index(t => t.id_usuario);
        }

        public override void Down()
        {
            DropForeignKey("dbo.BITACORA", "id_usuario", "dbo.USUARIO");
            DropForeignKey("dbo.DETALLE_PEDIDO", "id_objeto", "dbo.OBJETO");
            DropForeignKey("dbo.DETALLE_PEDIDO", "id_pedido", "dbo.PEDIDO");
            DropForeignKey("dbo.PEDIDO", "id_usuario", "dbo.USUARIO");
            DropForeignKey("dbo.USUARIO", "id_rol", "dbo.ROL");
            DropIndex("dbo.BITACORA", new[] { "id_usuario" });
            DropIndex("dbo.DETALLE_PEDIDO", new[] { "id_objeto" });
            DropIndex("dbo.DETALLE_PEDIDO", new[] { "id_pedido" });
            DropIndex("dbo.PEDIDO", new[] { "id_usuario" });
            DropIndex("dbo.USUARIO", new[] { "email" });
            DropIndex("dbo.USUARIO", new[] { "id_rol" });
            DropTable("dbo.BITACORA");
            DropTable("dbo.DETALLE_PEDIDO");
            DropTable("dbo.PEDIDO");
            DropTable("dbo.OBJETO");
            DropTable("dbo.USUARIO");
            DropTable("dbo.ROL");
        }
    }
}
