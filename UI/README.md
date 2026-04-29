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

## 4. Levantar el proyecto

Abrir la solución en Visual Studio y ejecutar el proyecto **UI** como Startup Project.

Al iniciar por primera vez, Entity Framework va a crear la base de datos y aplicar todas las migraciones automáticamente. **No es necesario correr ningún comando en la consola de NuGet.**

---

## Aclaraciones importantes

- Si la base de datos no existe, EF la crea sola.
- Si ya existe pero le faltan tablas o migraciones, las aplica automáticamente.
- Si ocurre un error en la aplicación y no se puede conectar a la base de datos, el sistema guarda el log en un archivo de texto como respaldo.

---

## ¿Problemas?

Si al levantar el proyecto aparece un error de conexión, verificar que:

- El nombre del servidor en `connectionStrings.config` sea correcto
- SQL Server esté corriendo
- Tengas permisos sobre la instancia
