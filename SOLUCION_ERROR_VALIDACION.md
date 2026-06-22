# ❌ Solución Error en Validación de Integridad DVH

## 🔴 Problema Detectado

El error ocurre porque **la columna DVH no está mapeada en Entity Framework** y/o **la migración no ha sido aplicada a la base de datos**.

### Errores Observados:
```
System.UnauthorizedAccessException
System.Data.Entity.Core.EntityCommandExecutionException
```

---

## ✅ Solución Paso a Paso

### Paso 1: Aplicar la Migración a la Base de Datos

**Opción A: Usar la Página Web (Recomendado)**

1. Abre la aplicación web en Visual Studio (presiona F5)
2. Navega a: `http://localhost:44389/Pages/ApplyMigrations.aspx`
   - Si la aplicación falla al iniciar, ve al Paso 1B primero
3. **Inicia sesión como WEBMASTER** (si se requiere autenticación)
4. Verás una lista de "migraciones pendientes"
5. Haz clic en el botón **"Aplicar Migraciones"**
6. Espera a que aparezca el mensaje "✓ Migraciones aplicadas exitosamente"

**Opción B: Usar Package Manager Console (Alternativa)**

1. Abre Visual Studio
2. Menú → Tools → NuGet Package Manager → Package Manager Console
3. En la consola que aparece en la parte inferior, escribe:
   ```powershell
   Update-Database -ProjectName DAL -Verbose
   ```
4. Presiona Enter y espera a que termine

---

### Paso 2: Verificar que la Migración se Aplicó Correctamente

**En SQL Server Management Studio:**

1. Abre SQL Server Management Studio
2. Conéctate a `localhost\SQLEXPRESS`
3. Expande: Databases → ArquitecturaWebHvacWebForms → Tables → dbo.USUARIO
4. Haz clic derecho → Script Table as → CREATE to → New Query Editor Window
5. Busca la línea con **`[DVH]`** en la definición de columnas

   Si ves esto, ✅ la migración se aplicó:
   ```sql
   [DVH] [int] NULL
   ```

**Desde Línea de Comandos PowerShell:**

```powershell
# Conecta a SQL Server y verifica la columna
sqlcmd -S localhost\SQLEXPRESS -d ArquitecturaWebHvacWebForms -Q "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'USUARIO' AND COLUMN_NAME = 'DVH'"
```

Si ves una fila con `DVH`, ✅ la migración se aplicó.

---

### Paso 3: Limpiar y Reconstruir la Solución

1. En Visual Studio, menú: Build → Clean Solution
2. Luego: Build → Build Solution
3. Espera a que aparezca "Build completed successfully" en la parte inferior

---

### Paso 4: Probar la Validación de Integridad

1. **Reinicia la aplicación** (cierra y abre F5)
2. Si todo está bien, deberías ver:
   - ✅ La aplicación se inicia sin errores
   - ✅ En el log (Output Window) verás: "Compilación correcta"
   - ✅ O un mensaje de advertencia si hay registros con DVH inválido

---

## 🔧 Cambios Realizados para Evitar Errores Futuros

Se han mejorado los siguientes archivos:

### 1. **DAL/AppDbContext.cs**
- ✅ Agregado mapeado: `modelBuilder.Entity<Usuario>().Property(x => x.DVH).HasColumnName("DVH");`
- Esto asegura que Entity Framework sepa dónde está la columna DVH en la BD

### 2. **DAL/ValidarDVH.cs**
- ✅ Mejorado manejo de excepciones
- ✅ Ahora detecta si la columna DVH no existe y da un mensaje claro
- ✅ Si hay error en un usuario específico, continúa validando los demás

### 3. **UI/Global.asax.cs**
- ✅ Mensajes de error más detallados
- ✅ Incluye instrucciones para aplicar migraciones

---

## 🛠️ Si Aún Hay Problemas

### Síntoma: "Invalid column name 'DVH'"

**Solución:**
```sql
-- Ejecuta esto en SQL Server Management Studio
ALTER TABLE dbo.USUARIO
ADD DVH INT NULL;
```

### Síntoma: "Entity Framework version mismatch"

**Solución:**
1. Abre Package Manager Console
2. Escribe:
   ```powershell
   Update-Database -ProjectName DAL -Force
   ```

### Síntoma: "Access denied" o "Permission denied"

**Solución:**
1. Verifica que estés conectado a SQL Server con permisos de administrador
2. En Connection String (UI/connectionStrings.config), asegúrate de:
   ```xml
   <add name="DefaultConnection" 
		connectionString="Data Source=localhost\SQLEXPRESS;Database=ArquitecturaWebHvacWebForms;Trusted_Connection=True;MultipleActiveResultSets=true" 
		providerName="System.Data.SqlClient" />
   ```

---

## 📋 Checklist de Verificación

- ✅ ¿Se aplicó la migración a la base de datos?
- ✅ ¿La columna DVH existe en la tabla USUARIO?
- ✅ ¿AppDbContext tiene el mapeado de DVH?
- ✅ ¿La solución compila sin errores?
- ✅ ¿La aplicación inicia sin excepciones?
- ✅ ¿Puedes acceder a /Pages/IntegrityValidation.aspx?

---

## 📞 Próximas Acciones

1. **Aplicar la migración** usando una de las opciones del Paso 1
2. **Verificar en SQL Server** que la columna DVH existe
3. **Reconstruir la solución** (Build → Clean + Build)
4. **Probar la aplicación** (F5)
5. **Acceder a IntegrityValidation.aspx** para validar/reparar registros

Si aún tienes problemas, verifica el Output Window (View → Output) para ver los logs detallados del error.

---

## 🎯 Estado Actual

| Tarea | Estado |
|-------|--------|
| DVH mapeado en AppDbContext | ✅ Completado |
| Manejo robusto de excepciones | ✅ Completado |
| Logs detallados de errores | ✅ Completado |
| Migración creada | ✅ Completado (archivo: DAL/Migrations/202701011200000_AgregarDVHAUsuario.cs) |
| **Migración aplicada a BD** | ⏳ **PENDIENTE** ← Hazlo ahora |

**¡La única tarea pendiente es aplicar la migración a la base de datos!**
