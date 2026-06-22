# 🔧 Aplicar Migraciones - Guía Paso a Paso

## ⚠️ Problema Actual
La base de datos no tiene la columna `DVH` en la tabla `USUARIO` porque las migraciones no se han aplicado.

## ✅ Solución: Aplicar Migraciones

### 📌 Opción 1: Usar Visual Studio Package Manager Console (RECOMENDADO)

#### Paso 1: Abre Visual Studio
- Si no está abierto, abre: `Poyecto_Arquitectura_Web.sln`

#### Paso 2: Abre Package Manager Console
```
Menú de Visual Studio → Tools → NuGet Package Manager → Package Manager Console
```

O presiona: `Ctrl + ` (acento grave)

#### Paso 3: Ejecuta el comando
En la consola que aparece, copia y pega exactamente esto:

```powershell
Update-Database -ProjectName DAL -Verbose
```

Presiona Enter y espera a que termine.

#### Paso 4: Verifica el resultado
Deberías ver algo como:
```
Applying Code First Migration: 202701011200000_AgregarDVHAUsuario
Running Seed method.
Migrations completed successfully.
```

✅ **Si ves "Migrations completed successfully" = Éxito**

---

### 📌 Opción 2: Usar Página Web de la Aplicación

#### Paso 1: Inicia la aplicación
En Visual Studio, presiona `F5` (o Debug → Start Debugging)

#### Paso 2: Navega a la página de migraciones
En el navegador, ve a:
```
http://localhost:44389/Pages/ApplyMigrations.aspx
```

#### Paso 3: Aplica las migraciones
- Si la página falla al cargar, intenta la Opción 1
- Si carga correctamente, verás un botón "Aplicar Migraciones"
- Haz clic en el botón
- Espera el mensaje "✓ Migraciones aplicadas exitosamente"

---

### 📌 Opción 3: Desde SQL Server Management Studio (Avanzado)

Si prefieres ejecutar SQL directamente:

#### Paso 1: Abre SQL Server Management Studio

#### Paso 2: Conéctate a la BD
```
Server: localhost\SQLEXPRESS
Database: ArquitecturaWebHvacWebForms
```

#### Paso 3: Ejecuta estos comandos SQL

```sql
-- Primero, registra la migración como aplicada
INSERT INTO __MigrationHistory (MigrationId, ContextKey, Model, ProductVersion)
VALUES (
	'202701011200000_AgregarDVHAUsuario',
	'DAL.Migrations.Configuration',
	0x,
	'6.0'
);

-- Luego, agrega la columna DVH a USUARIO
ALTER TABLE dbo.USUARIO
ADD [DVH] [int] NULL;
```

⚠️ **Nota:** Este método es manual y menos recomendado. Usa Opción 1 o 2.

---

## 🔍 Verificar que la Migración se Aplicó

### Método 1: Visual Studio
1. Abre: `DAL/Migrations/`
2. Verifica que el archivo `202701011200000_AgregarDVHAUsuario.cs` exista
3. En Package Manager Console, ejecuta:
   ```powershell
   Get-Migrations -ProjectName DAL
   ```

### Método 2: SQL Server Management Studio
1. Abre SQL Server Management Studio
2. Conéctate a: `localhost\SQLEXPRESS` → `ArquitecturaWebHvacWebForms`
3. Expande: Databases → ArquitecturaWebHvacWebForms → Tables → dbo.USUARIO
4. Verifica que exista la columna `DVH` (tipo: int, Allow Nulls: yes)

### Método 3: Query SQL
```sql
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'USUARIO'
ORDER BY ORDINAL_POSITION;
```

Si ves `DVH | int | YES` = ✅ Éxito

---

## ⚠️ Si Hay Errores

### Error: "Update-Database : El término no se reconoce"
**Causa:** Estás usando PowerShell regular en lugar de Package Manager Console
**Solución:** 
1. Abre Visual Studio
2. Menú → Tools → NuGet Package Manager → Package Manager Console
3. Repite el comando en ESA consola

### Error: "No pending Code First migrations"
**Causa:** Ya se aplicó la migración
**Solución:** 
1. Verifica en SQL Server que la columna DVH existe
2. Si existe, reinicia la aplicación

### Error: "The migrations configuration type in the assembly did not match"
**Causa:** Hay un problema con la configuración de migraciones
**Solución:**
1. Limpia la solución: Build → Clean Solution
2. Reconstruye: Build → Build Solution
3. Repite el comando `Update-Database`

### Error: "Login failed for user 'sa'"
**Causa:** Problema de conexión a SQL Server
**Solución:**
1. Verifica que SQL Server Express esté corriendo
2. Verifica la connection string en `UI/connectionStrings.config`
3. Asegúrate de que `Trusted_Connection=True`

---

## 📋 Checklist Final

Después de aplicar la migración, verifica:

- [ ] Package Manager Console mostró "Migrations completed successfully"
- [ ] SQL Server tiene la columna DVH en tabla USUARIO
- [ ] La aplicación inicia sin errores (F5)
- [ ] No hay excepciones en el Output Window
- [ ] Puedes acceder a http://localhost:44389/Pages/IntegrityValidation.aspx

---

## 🎯 Próximos Pasos

Una vez que la migración esté aplicada:

1. **Reinicia la aplicación** (F5)
2. **Abre la página de validación:** `/Pages/IntegrityValidation.aspx`
3. **Haz clic en "Validar Integridad"** para comprobar los registros
4. **Si hay registros inválidos, haz clic en "Reparar"**

---

## 💡 Consejos

- **Si algo falla**, intenta la Opción 1 (Package Manager Console) - es la más confiable
- **Guarda tu trabajo** antes de aplicar migraciones
- **Si necesitas revertir**, ejecuta: `Update-Database -TargetMigration "" -ProjectName DAL`
- **Para ver todas las migraciones aplicadas**, ejecuta: `Get-Migrations -ProjectName DAL` en Package Manager Console

---

## 📞 Resumen Rápido

| Acción | Comando |
|--------|---------|
| Aplicar migraciones | `Update-Database -ProjectName DAL -Verbose` (en Package Manager Console) |
| Ver migraciones | `Get-Migrations -ProjectName DAL` |
| Revertir migración | `Update-Database -TargetMigration "" -ProjectName DAL` |
| Crear nueva migración | `Add-Migration NombreMigracion -ProjectName DAL` |

**¡Ejecuta el comando de la Opción 1 ahora! ⬆️**
