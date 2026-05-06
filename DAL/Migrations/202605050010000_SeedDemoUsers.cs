namespace DAL.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class SeedDemoUsers : DbMigration
    {
        public override void Up()
        {
            Sql(@"
DECLARE @PasswordHash NVARCHAR(255) = '588c55f3ce2b8569b153c5abbf13f9f74308b88a20017cc699b835cc93195d16';

IF NOT EXISTS (SELECT 1 FROM dbo.ROL WHERE nombre = 'WEBMASTER')
    INSERT INTO dbo.ROL (nombre) VALUES ('WEBMASTER');

IF NOT EXISTS (SELECT 1 FROM dbo.ROL WHERE nombre = 'CLIENTE')
    INSERT INTO dbo.ROL (nombre) VALUES ('CLIENTE');

IF NOT EXISTS (SELECT 1 FROM dbo.ROL WHERE nombre = 'ADM_OPERACIONES')
    INSERT INTO dbo.ROL (nombre) VALUES ('ADM_OPERACIONES');

DECLARE @RolWebmaster INT = (SELECT id_rol FROM dbo.ROL WHERE nombre = 'WEBMASTER');
DECLARE @RolCliente INT = (SELECT id_rol FROM dbo.ROL WHERE nombre = 'CLIENTE');
DECLARE @RolAdmOperaciones INT = (SELECT id_rol FROM dbo.ROL WHERE nombre = 'ADM_OPERACIONES');

IF EXISTS (SELECT 1 FROM dbo.USUARIO WHERE email = 'webmaster@demo.com')
BEGIN
    UPDATE dbo.USUARIO
    SET id_rol = @RolWebmaster,
        password = @PasswordHash,
        nombre = 'Usuario Webmaster',
        inactivo = 0
    WHERE email = 'webmaster@demo.com';
END
ELSE
BEGIN
    INSERT INTO dbo.USUARIO (id_rol, email, password, nombre, inactivo)
    VALUES (@RolWebmaster, 'webmaster@demo.com', @PasswordHash, 'Usuario Webmaster', 0);
END;

IF EXISTS (SELECT 1 FROM dbo.USUARIO WHERE email = 'cliente@demo.com')
BEGIN
    UPDATE dbo.USUARIO
    SET id_rol = @RolCliente,
        password = @PasswordHash,
        nombre = 'Usuario Cliente',
        inactivo = 0
    WHERE email = 'cliente@demo.com';
END
ELSE
BEGIN
    INSERT INTO dbo.USUARIO (id_rol, email, password, nombre, inactivo)
    VALUES (@RolCliente, 'cliente@demo.com', @PasswordHash, 'Usuario Cliente', 0);
END;

IF EXISTS (SELECT 1 FROM dbo.USUARIO WHERE email = 'adm.operaciones@demo.com')
BEGIN
    UPDATE dbo.USUARIO
    SET id_rol = @RolAdmOperaciones,
        password = @PasswordHash,
        nombre = 'Usuario ADM Operaciones',
        inactivo = 0
    WHERE email = 'adm.operaciones@demo.com';
END
ELSE
BEGIN
    INSERT INTO dbo.USUARIO (id_rol, email, password, nombre, inactivo)
    VALUES (@RolAdmOperaciones, 'adm.operaciones@demo.com', @PasswordHash, 'Usuario ADM Operaciones', 0);
END;
");
        }

        public override void Down()
        {
            Sql(@"
DELETE FROM dbo.USUARIO
WHERE email IN ('webmaster@demo.com', 'cliente@demo.com', 'adm.operaciones@demo.com');

DELETE FROM dbo.ROL
WHERE nombre IN ('WEBMASTER', 'CLIENTE', 'ADM_OPERACIONES')
  AND NOT EXISTS (
      SELECT 1
      FROM dbo.USUARIO
      WHERE dbo.USUARIO.id_rol = dbo.ROL.id_rol
  );
");
        }
    }
}
