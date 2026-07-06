namespace DAL.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AgregarDVVUsuario : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DIGITO_VERIFICADOR_VERTICAL",
                c => new
                {
                    id_dvv = c.Int(nullable: false, identity: true),
                    tabla = c.String(nullable: false, maxLength: 80),
                    valor = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.id_dvv)
                .Index(t => t.tabla, unique: true);
        }

        public override void Down()
        {
            DropIndex("dbo.DIGITO_VERIFICADOR_VERTICAL", new[] { "tabla" });
            DropTable("dbo.DIGITO_VERIFICADOR_VERTICAL");
        }
    }
}
