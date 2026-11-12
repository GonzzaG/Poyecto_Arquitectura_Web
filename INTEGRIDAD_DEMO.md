# Demostración de integridad: USUARIO y OBJETO

`OBJETO` contiene productos (`esProducto = 1`) y servicios (`esProducto = 0`). Alterar el precio, stock o nombre de una fila de producto directamente en SQL es "romper un producto". No es necesario borrar ni cambiar la estructura de la tabla.

## Preparación

1. Hacer un backup de la base antes de actualizar. La primera migración establece como referencia los datos existentes: deben ser los valores correctos antes de comenzar la prueba. No recupera alteraciones anteriores a su instalación.
2. Compilar la solución. Activar temporalmente `SincronizarBaseDatosAlIniciar` en `UI/Web.config`, iniciar la aplicación y devolver el valor a `false`. También se puede aplicar desde la consola de paquetes con `Update-Database -ProjectName DAL -StartupProjectName UI`.
3. La migración `202701021200000_IntegridadUsuariosObjetosSha256` convierte el DVH de USUARIO y el DVV a texto, agrega DVH a OBJETO y crea `INTEGRITY_SNAPSHOT`. Incluye metadata de EF para que se detecte como migración pendiente y contempla bases con o sin los verificadores anteriores.
4. Entrar como `webmaster@demo.com` / `Demo123!`. Abrir **Integridad** y verificar que ambas tablas estén válidas. Mantener esa sesión abierta y hacer un backup con la nueva referencia ya inicializada.

## Prueba de las dos tablas

1. Abrir `Tools/IntegridadDemo.sql` en SQL Server Management Studio, seleccionar la base de demostración y ejecutar. Modifica el nombre de `cliente@demo.com` y el precio del primer producto, sin cambiar los verificadores. Devuelve los IDs afectados.
2. En **Integridad**, pulsar **Validar integridad**. En **Tablas verificadas** deben aparecer `USUARIO` y `OBJETO` como **Dañada**, cada una con un registro afectado. En **Registros dañados** aparecen tabla, ID, nombre, problema detectado, DVH guardado y calculado.
3. Entrar como cliente o probar un login fallido: la evidencia debe seguir visible. El login valida antes de actualizar contadores y omite esas escrituras si hay inconsistencias. El resto del sitio entra en mantenimiento; el webmaster puede diagnosticar.
4. Pulsar **Recuperar valores originales**. Se restauran el nombre y precio anteriores desde la copia de la última escritura legítima, dentro de una transacción. Se vuelve a leer SQL y verificar ambas tablas antes de informar éxito.
5. Hacer una compra normal: el descuento de stock actualiza su DVH, DVV y copia de recuperación automáticamente, sin mostrar un falso error.

## Qué controla y qué recupera

- DVH: SHA-256 de todos los campos escalares de cada fila, incluido su ID. La representación distingue campos, nulos y vacíos y es independiente de la configuración regional.
- DVV: SHA-256 de la tabla y de sus IDs/hashes ordenados. Cada tabla tiene su propio DVV.
- La pantalla también distingue DVH alterado, filas eliminadas, filas agregadas desde SQL y copias de recuperación dañadas. Una fila eliminada puede identificarse por su ID gracias a la copia.
- Las modificaciones legítimas pasan por `AppDbContext.SaveChanges` (también `SaveChangesAsync`). Se valida el estado previo y se guardan datos, verificadores y copia en una misma transacción. Una escritura no puede aceptar silenciosamente corrupción previa.
- **Validar** no modifica datos. **Recalcular todo**, disponible para el webmaster, acepta los valores actuales de ambas tablas y actualiza DVH, DVV y copia de recuperación en una transacción. Permite salir del mantenimiento conservando los cambios hechos desde SQL. Después de recalcular, recuperar originales usa esa nueva referencia.
- La recuperación en pantalla restaura filas existentes con datos o DVH alterados. Para filas agregadas/eliminadas o para un DVV/copia no confiable, se requiere un backup completo; se evita modificar relaciones automáticamente.
- La referencia inicial se crea una sola vez, durante la nueva migración. Reiniciar o ejecutar `Update-Database` otra vez no acepta de nuevo los datos actuales ni recrea productos borrados desde SQL.

Los verificadores y su copia detectan cambios como los de esta demostración. No constituyen una firma contra un administrador que reescriba coordinadamente los datos, hashes y respaldo. El respaldo completo debe conservarse aparte.

## Comprobaciones reproducibles

Desde PowerShell en la raíz del repositorio:

```powershell
.\Tools\IntegrityChecks\Run.ps1
.\Tools\IntegrityChecks\Run.ps1 -Legacy
```

Requieren Visual Studio/MSBuild y SQL Server LocalDB. Se puede usar `-Server 'SERVIDOR\INSTANCIA'` con autenticación integrada. Cada ejecución crea una base `ArquitecturaWeb_IntegrityChecks_<guid>` exclusiva, hace las pruebas y elimina únicamente esa base al terminar. No utiliza la conexión de UI ni altera la base de trabajo.

El modo `-Legacy` prepara el esquema anterior con DVH/DVV enteros y verifica la actualización. Las pruebas abarcan alteraciones simultáneas, login correcto/fallido, recuperación real de valores, escritura async, compra/stock, modificación de verificadores, reinicio de migraciones, altas y bajas desde SQL y serialización estable.
