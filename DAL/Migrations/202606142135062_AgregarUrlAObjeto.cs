namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregarUrlAObjeto : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OBJETO", "imagen_url", c => c.String(maxLength: 260));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OBJETO", "imagen_url");
        }
    }
}
