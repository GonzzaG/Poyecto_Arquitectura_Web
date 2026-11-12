using System;
using System.Data.Entity.Migrations;

namespace DAL.Migrations
{
    public sealed partial class IntegridadUsuariosObjetosSha256 : DbMigration
    {
        public override void Up()
        {
            // Las migraciones antiguas de DVH/DVV carecían de metadata y algunas bases
            // recibieron esas columnas por migración automática. Contemplar ambos casos.
            Sql(@"
IF COL_LENGTH('dbo.USUARIO', 'DVH') IS NULL
    ALTER TABLE dbo.USUARIO ADD DVH char(64) NULL;
ELSE
    ALTER TABLE dbo.USUARIO ALTER COLUMN DVH char(64) NULL;
IF COL_LENGTH('dbo.OBJETO', 'DVH') IS NULL
    ALTER TABLE dbo.OBJETO ADD DVH char(64) NULL;
ELSE
    ALTER TABLE dbo.OBJETO ALTER COLUMN DVH char(64) NULL;
IF OBJECT_ID('dbo.DIGITO_VERIFICADOR_VERTICAL', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.DIGITO_VERIFICADOR_VERTICAL (
        id_dvv int IDENTITY PRIMARY KEY, tabla nvarchar(80) NOT NULL, valor varchar(64) NOT NULL);
    CREATE UNIQUE INDEX IX_tabla ON dbo.DIGITO_VERIFICADOR_VERTICAL(tabla);
END
ELSE
    ALTER TABLE dbo.DIGITO_VERIFICADOR_VERTICAL ALTER COLUMN valor varchar(64) NOT NULL;
DELETE FROM dbo.DIGITO_VERIFICADOR_VERTICAL WHERE tabla IN ('USUARIO', 'OBJETO');
INSERT dbo.DIGITO_VERIFICADOR_VERTICAL(tabla, valor)
VALUES ('USUARIO', 'PENDIENTE_SHA256'), ('OBJETO', 'PENDIENTE_SHA256');");
            CreateTable("dbo.INTEGRITY_SNAPSHOT", c => new
            {
                Tabla = c.String(nullable: false, maxLength: 80),
                RegistroId = c.Int(nullable: false),
                Datos = c.String(nullable: false),
                DVH = c.String(nullable: false, maxLength: 64, fixedLength: true, unicode: false)
            }).PrimaryKey(t => new { t.Tabla, t.RegistroId });
        }

        public override void Down()
        {
            throw new NotSupportedException("La conversión a SHA-256 no es reversible. Restaure el backup previo a esta migración.");
        }
    }
}
