-- Ejecutar en la base de demostración después de aplicar la migración SHA-256.
-- Primero comprobar desde la web que USUARIO y OBJETO están válidas.
-- Mantener abierta una sesión de webmaster y contar con un backup previo.
-- Este script altera un usuario cliente y un producto, conservando sus verificadores.
SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @Usuario int = (SELECT id_usuario FROM dbo.USUARIO WHERE email = 'cliente@demo.com');
DECLARE @Objeto int = (SELECT TOP (1) id_objeto FROM dbo.OBJETO WHERE esProducto = 1 ORDER BY id_objeto);

IF @Usuario IS NULL OR @Objeto IS NULL
BEGIN
    ROLLBACK;
    THROW 50000, 'Falta el usuario demo o un producto. No se alteró ningún registro.', 1;
END;

UPDATE dbo.USUARIO SET nombre = nombre + ' ALTERADO' WHERE id_usuario = @Usuario;
UPDATE dbo.OBJETO SET precio = precio + 123.00 WHERE id_objeto = @Objeto;

SELECT 'USUARIO' AS tabla, id_usuario AS registro, nombre, DVH FROM dbo.USUARIO WHERE id_usuario = @Usuario;
SELECT 'OBJETO' AS tabla, id_objeto AS registro, nombre, precio, DVH FROM dbo.OBJETO WHERE id_objeto = @Objeto;
COMMIT;

-- En la web: Integridad > Validar integridad.
-- Resultado: las dos tablas dañadas y los dos IDs de arriba en Registros dañados.
-- Recuperar valores originales restaura nombre/precio previos desde INTEGRITY_SNAPSHOT.
