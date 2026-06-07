namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddBloqueoCuentaUsuario : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.USUARIO", "intentos_fallidos", c => c.Int(nullable: false, defaultValue: 0));
            AddColumn("dbo.USUARIO", "fecha_bloqueo", c => c.DateTime(nullable: true));
        }

        public override void Down()
        {
            DropColumn("dbo.USUARIO", "fecha_bloqueo");
            DropColumn("dbo.USUARIO", "intentos_fallidos");
        }
    }
}
