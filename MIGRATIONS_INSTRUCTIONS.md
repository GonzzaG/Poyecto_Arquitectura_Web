# 📋 Instrucciones para Aplicar Migraciones - DVH

## Problema Inicial
Al ejecutar la aplicación, encontrará el error:
```
AutomaticMigrationsDisabledException: Unable to update database to match the current model 
because there are pending changes and automatic migration is disabled.
```

Esto se debe a que se agregó una **nueva columna DVH** a la tabla USUARIO que necesita ser aplicada en la base de datos.

---

## ✅ Solución: Aplicar las Migraciones

### Opción 1: A través de la Interfaz Web (Recomendado)

1. **Inicie sesión** en la aplicación con una cuenta de **WEBMASTER**
2. **Acceda al menú** de administración (parte superior derecha)
3. **Haga clic en "Migraciones"** (debe estar visible solo para WEBMASTER)
4. En la página que abre, verá:
   - El estado de las migraciones pendientes
   - La lista de migraciones aplicadas
5. **Haga clic en "Aplicar Migraciones Pendientes"**
6. La aplicación aplicará automáticamente la migración `202701011200000_AgregarDVHAUsuario`
7. ¡Listo! Verá un mensaje de confirmación

---

### Opción 2: Mediante Package Manager Console (NuGet)

Si prefiere aplicar las migraciones desde Visual Studio:

1. **Abra Visual Studio**
2. **Vaya a:** Tools → NuGet Package Manager → Package Manager Console
3. **Ejecute este comando:**
   ```powershell
   Update-Database -ProjectName DAL
   ```
4. La consola mostrará el progreso de la migración

---

### Opción 3: Mediante ef-cli (Entity Framework CLI)

Si tiene Entity Framework Core instalado:

```bash
dotnet ef database update
```

---

## 🔍 ¿Qué hace la migración?

La migración `202701011200000_AgregarDVHAUsuario` realiza:
- ✅ Agrega la columna `DVH` (int, nullable) a la tabla `USUARIO`
- ✅ Permite almacenar dígitos verificadores horizontales para validar integridad
- ✅ Habilita la validación automática de integridad de datos

---

## 🆘 Solución de Problemas

### Si sigue obteniendo errores después de aplicar la migración:

1. **Limpia el caché del navegador** (Ctrl + Shift + Delete)
2. **Reinicia la aplicación** en Visual Studio (Detener y luego F5)
3. **Verifica la base de datos**:
   - Abre SQL Server Management Studio
   - Conecta a `localhost\SQLEXPRESS`
   - Base de datos: `ArquitecturaWebHvacWebForms`
   - Tabla: `USUARIO`
   - Verifica que exista la columna `DVH`

### Si la migración no se aplica:

- Asegurate de estar logueado como **WEBMASTER**
- Verifica que la conexión a SQL Server sea correcta en `connectionStrings.config`
- Revisa los logs en la consola de depuración de Visual Studio

---

## ✨ Próximos Pasos

Una vez aplicada la migración:

1. **Valida la integridad** de los datos:
   - En el menú de WEBMASTER, haz clic en "Integridad"
   - La aplicación mostrará si hay registros con DVH inválido
   - Haz clic en "Reparar Registros Inválidos" si es necesario

2. **La aplicación validará automáticamente** la integridad cada vez que se inicie

---

## 📞 Soporte

Si necesitas ayuda adicional, contacta al equipo de desarrollo.
