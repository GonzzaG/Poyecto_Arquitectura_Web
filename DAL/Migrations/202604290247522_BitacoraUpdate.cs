namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BitacoraUpdate : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.BITACORA", new[] { "id_usuario" });
            AddColumn("dbo.BITACORA", "fecha_utc", c => c.DateTime(nullable: false));
            AddColumn("dbo.BITACORA", "tipo_evento", c => c.String(nullable: false, maxLength: 20));
            AddColumn("dbo.BITACORA", "accion", c => c.String(nullable: false, maxLength: 120));
            AddColumn("dbo.BITACORA", "resultado", c => c.String(nullable: false, maxLength: 20));
            AddColumn("dbo.BITACORA", "mensaje", c => c.String(nullable: false, maxLength: 500));
            AddColumn("dbo.BITACORA", "detalle", c => c.String(maxLength: 4000));
            AddColumn("dbo.BITACORA", "usuario_email", c => c.String(maxLength: 160));
            AddColumn("dbo.BITACORA", "url", c => c.String(maxLength: 260));
            AddColumn("dbo.BITACORA", "ip", c => c.String(maxLength: 64));
            AlterColumn("dbo.BITACORA", "id_usuario", c => c.Int());
            CreateIndex("dbo.BITACORA", "id_usuario");
            DropColumn("dbo.BITACORA", "fecha_hora");
            DropColumn("dbo.BITACORA", "criticidad");
            DropColumn("dbo.BITACORA", "dvh");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BITACORA", "dvh", c => c.String(nullable: false, maxLength: 255));
            AddColumn("dbo.BITACORA", "criticidad", c => c.String(nullable: false, maxLength: 40));
            AddColumn("dbo.BITACORA", "fecha_hora", c => c.DateTime(nullable: false));
            DropIndex("dbo.BITACORA", new[] { "id_usuario" });
            AlterColumn("dbo.BITACORA", "id_usuario", c => c.Int(nullable: false));
            DropColumn("dbo.BITACORA", "ip");
            DropColumn("dbo.BITACORA", "url");
            DropColumn("dbo.BITACORA", "usuario_email");
            DropColumn("dbo.BITACORA", "detalle");
            DropColumn("dbo.BITACORA", "mensaje");
            DropColumn("dbo.BITACORA", "resultado");
            DropColumn("dbo.BITACORA", "accion");
            DropColumn("dbo.BITACORA", "tipo_evento");
            DropColumn("dbo.BITACORA", "fecha_utc");
            CreateIndex("dbo.BITACORA", "id_usuario");
        }
    }
}
