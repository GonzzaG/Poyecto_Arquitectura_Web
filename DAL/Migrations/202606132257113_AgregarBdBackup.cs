namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregarBdBackup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DATABASE_BACKUP",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        file_name = c.String(nullable: false, maxLength: 260),
                        file_path = c.String(nullable: false, maxLength: 500),
                        file_size_mb = c.Decimal(nullable: false, precision: 18, scale: 2),
                        created_date = c.DateTime(nullable: false),
                        created_by_user_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.USUARIO", t => t.created_by_user_id)
                .Index(t => t.created_by_user_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DATABASE_BACKUP", "created_by_user_id", "dbo.USUARIO");
            DropIndex("dbo.DATABASE_BACKUP", new[] { "created_by_user_id" });
            DropTable("dbo.DATABASE_BACKUP");
        }
    }
}
