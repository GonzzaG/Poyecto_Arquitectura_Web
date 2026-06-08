namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregarColumnaIntentos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.USUARIO", "intentos_fallidos", c => c.Int(nullable: false));
            AddColumn("dbo.USUARIO", "fecha_bloqueo", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.USUARIO", "fecha_bloqueo");
            DropColumn("dbo.USUARIO", "intentos_fallidos");
        }
    }
}
