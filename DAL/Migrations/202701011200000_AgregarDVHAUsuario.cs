namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AgregarDVHAUsuario : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.USUARIO", "DVH", c => c.Int());
        }

        public override void Down()
        {
            DropColumn("dbo.USUARIO", "DVH");
        }
    }
}
