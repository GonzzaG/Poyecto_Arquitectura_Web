# 🆘 Solución del Error: AutomaticMigrationsDisabledException

## ❌ El Problema

Cuando ejecutas la aplicación en el navegador, ves:

```
Error de servidor en la aplicación '/'.

Error en runtime
Descripción: Se produjo una excepción al procesar la solicitud. 
Asimismo, se produjo otra excepción al ejecutar la página de error personalizada para la primera excepción. 
Se ha terminado la solicitud.
```

En los logs de Visual Studio (Debug Console) ves:

```
AutomaticMigrationsDisabledException: Unable to update database to match the current model 
because there are pending changes and automatic migration is disabled. 
Either write the pending model changes to a code-based migration or enable automatic migration.
```

## 🔍 ¿Por Qué Ocurre?

1. **Agregamos una nueva propiedad** `DVH` a la clase Usuario
2. **Creamos una migración** para agregar la columna a la BD
3. **Las migraciones automáticas están deshabilitadas** en Configuration.cs
4. **La BD aún no tiene la columna** DVH que el código espera
5. **Entity Framework detecta** esta inconsistencia y lanza el error

## ✅ Soluciones

### **Opción 1: A través de la Web (✅ RECOMENDADA)**

#### Paso 1: Ir a la página de Migraciones

```
URL: http://localhost:44389/Pages/ApplyMigrations.aspx
```

Si obtienes error 404, primero debes iniciar sesión como WEBMASTER:
1. Ir a http://localhost:44389/Login
2. Loguearte con una cuenta WEBMASTER
3. Luego acceder a /Pages/ApplyMigrations.aspx

#### Paso 2: Aplicar las Migraciones

En la página verás:
- Estado actual de migraciones
- Lista de migraciones pendientes
- Botón "Aplicar Migraciones Pendientes"

Haz clic en el botón y espera a que se complete.

#### Paso 3: Verificar

Si ves el mensaje: **"✓ Migraciones aplicadas exitosamente."**

¡Está hecho! Ahora puedes:
1. Actualizar el navegador
2. Usar la aplicación normalmente

---

### **Opción 2: Consola de Administrador de Paquetes (NuGet)**

#### Paso 1: Abre Package Manager Console

En Visual Studio:
```
Tools → NuGet Package Manager → Package Manager Console
```

#### Paso 2: Ejecuta el comando

```powershell
Update-Database -ProjectName DAL
```

#### Paso 3: Espera la confirmación

Deberías ver algo como:
```
Specify the '-Verbose' flag to view the SQL statements being applied to the target database.
Target database is: 'ArquitecturaWebHvacWebForms' (SqlServer).
No pending code-based migrations.
```

---

### **Opción 3: Habilitación Temporal de Migraciones Automáticas**

⚠️ **Esta es la solución más rápida pero menos recomendada**

#### Paso 1: Edita Configuration.cs

Archivo: `DAL\Migrations\Configuration.cs`

```csharp
public sealed class Configuration : DbMigrationsConfiguration<AppDbContext>
{
	public Configuration()
	{
		AutomaticMigrationsEnabled = true;  // ← Cambia a true
		AutomaticMigrationDataLossAllowed = false;
		ContextKey = "DAL.AppDbContext";
	}
}
```

#### Paso 2: Ejecuta la aplicación

- Presiona **F5** en Visual Studio
- La aplicación aplicará la migración automáticamente

#### Paso 3: Vuelve a cambiar a false (IMPORTANTE)

```csharp
AutomaticMigrationsEnabled = false;  // ← Vuelve a false
```

Guarda y recompila.

---

## 🔧 Verificación de que la Solución Funcionó

### Método 1: En la Aplicación Web
1. Accede a http://localhost:44389
2. No deberías ver el error rojo
3. Puedes navegar normalmente

### Método 2: En SQL Server Management Studio
1. Abre SQL Server Management Studio
2. Conecta a: `localhost\SQLEXPRESS`
3. Base de datos: `ArquitecturaWebHvacWebForms`
4. Expande: Tablas → dbo.USUARIO
5. Verifica que exista la columna `DVH` (tipo INT, permite NULL)

### Método 3: En el Código
1. En Visual Studio, abre el archivo de salida:
   `DAL\Migrations\__MigrationHistory` o verifica en la BD
2. La migración `202701011200000_AgregarDVHAUsuario` debe aparecer con fecha/hora de aplicación

---

## 🆘 Si Aún Hay Problemas

### Problema: No puedo acceder a ApplyMigrations.aspx

**Solución:**
1. Crea una cuenta WEBMASTER (si no tienes)
2. O modifica la visibilidad en el código (no recomendado)
3. Usa la Opción 2 (Package Manager Console)

---

### Problema: "The database already exists"

**Solución:**
Esto no es un error, es normal. La BD existe y se actualizará.

---

### Problema: "Timeout"

**Solución:**
Si la migración tarda mucho:
1. Cierra todas las conexiones a la BD
2. Reinicia Visual Studio
3. Intenta nuevamente

---

### Problema: Aún veo el error después de aplicar migraciones

**Solución:**
1. **Limpia el caché del navegador**: Ctrl + Shift + Delete
2. **Limpia los archivos temporales de ASP.NET**:
   ```
   Elimina: C:\Users\{TuUsuario}\AppData\Local\Temp\Temporary ASP.NET Files\
   ```
3. **Detén la aplicación** (Shift + F5)
4. **Limpia la solución**: Build → Clean Solution
5. **Recompila**: Build → Build Solution
6. **Ejecuta** (F5)

---

## 📊 Resumen Rápido

| Opción | Dificultad | Recomendación | Tiempo |
|--------|-----------|---------------|--------|
| Web (ApplyMigrations.aspx) | Fácil | ✅ Recomendada | 1 min |
| Package Manager Console | Media | ✅ Buena | 2 min |
| Migraciones Automáticas | Fácil | ⚠️ Evitar | 1 min |

---

## 🎯 Próximas Acciones

Una vez resuelta la migración:

1. **Valida la integridad de datos**:
   - Menú → "Integridad"
   - Si hay registros inválidos, haz clic en "Reparar"

2. **Continúa usando la aplicación normalmente**

3. **No necesitas hacer nada más** - La validación es automática en cada inicio

---

## 📞 Soporte Técnico

Si el problema persiste después de intentar las tres soluciones:
- Revisa los logs en: Visual Studio → Debug Console
- Busca mensajes de error específicos
- Verifica la conexión a SQL Server
- Contacta al equipo de desarrollo

---

**Estado:** 🟢 Solución Disponible - Aplica uno de los tres métodos arriba
