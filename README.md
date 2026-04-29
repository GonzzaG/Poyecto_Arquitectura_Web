# Guía de instalación del proyecto

## Requisitos previos

- Visual Studio 2019 o superior
- SQL Server (local o remoto)
- Git instalado
- Acceso al repositorio en GitHub

---

## 1. Clonar el repositorio (Pasar al punto 2 si ya lo tenes)

Abrí una terminal y ejecutá:

```bash
git clone <URL_DEL_REPOSITORIO>
```

Una vez clonado, posicionarse en la carpeta del proyecto:

```bash
cd <NOMBRE_DEL_PROYECTO>
```

---

## 2. Traer la rama master

```bash
git checkout master
git pull origin master
```

---

## 3. Configurar la cadena de conexión

El archivo `connectionStrings.config` **no está incluido en el repositorio** porque cada desarrollador tiene su propia base de datos local.

Pasos:

1. Ir a la carpeta del proyecto **UI**
2. Encontrar el archivo `connectionStrings.example.config`
3. Hacer una copia y renombrarla a `connectionStrings.config`
4. Abrir `connectionStrings.config` y reemplazar únicamente el nombre del servidor:

Server=TU_SERVIDOR_AQUI;


El resto de la cadena de conexión dejarlo exactamente igual.

---

## 4. Configurar sincronización de migraciones

El proyecto tiene un flag en `UI/Web.config` para decidir si al iniciar la aplicación debe sincronizar la base de datos con las migraciones de Entity Framework:

```xml
<appSettings>
  <add key="SincronizarBaseDatosAlIniciar" value="false" />
</appSettings>
```

El valor que se debe mandar a git es siempre:

```xml
value="false"
```

Para aplicar migraciones en una máquina local, cambiarlo temporalmente a:

```xml
value="true"
```

Luego levantar la aplicación. En el arranque, WebForms fuerza la inicialización de Entity Framework y aplica las migraciones pendientes sobre la conexión `DefaultConnection`.

Este flag es necesario porque la base de datos puede existir con el nombre correcto pero estar vacía. En ese caso, Entity Framework necesita ejecutar primero la migración inicial que crea las tablas base, y después aplicar las migraciones posteriores que ajustan la estructura.

Antes de commitear o pushear, revisar siempre que `SincronizarBaseDatosAlIniciar` haya vuelto a `false`.

---

## 5. Levantar el proyecto

Abrir la solución en Visual Studio y ejecutar el proyecto **UI** como Startup Project.

Si el flag `SincronizarBaseDatosAlIniciar` está en `true`, Entity Framework va a crear o actualizar la estructura de la base de datos al iniciar la aplicación. **No es necesario correr ningún comando en la consola de NuGet.**

---

## Aclaraciones importantes

- Si la base de datos no existe, EF puede crearla al iniciar con el flag de sincronización en `true`.
- Si ya existe pero está vacía, EF crea la estructura inicial y luego aplica las migraciones pendientes.
- Si ya existe pero le faltan migraciones, EF las aplica cuando `SincronizarBaseDatosAlIniciar` está en `true`.
- El valor versionado del flag debe quedar en `false`.
- Si ocurre un error en la aplicación y no se puede conectar a la base de datos, el sistema guarda el log en un archivo de texto como respaldo.

---

## ¿Problemas?

Si al levantar el proyecto aparece un error de conexión, verificar que:

- El nombre del servidor en `connectionStrings.config` sea correcto
- SQL Server esté corriendo
- Tengas permisos sobre la instancia
