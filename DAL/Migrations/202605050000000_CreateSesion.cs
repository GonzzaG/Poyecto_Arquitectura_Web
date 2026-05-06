namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class CreateSesion : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SESION",
                c => new
                {
                    id_sesion = c.Guid(nullable: false),
                    email = c.String(maxLength: 160),
                    fecha_creacion = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.id_sesion);
        }

        public override void Down()
        {
            DropTable("dbo.SESION");
        }
    }
}
