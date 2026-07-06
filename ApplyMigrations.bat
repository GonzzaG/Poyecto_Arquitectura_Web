@echo off
REM Script para aplicar migraciones usando Entity Framework Power Tools

setlocal enabledelayedexpansion

echo.
echo ╔════════════════════════════════════════════════════════════════╗
echo ║     Herramienta de Migraciones - Arquitectura Web HVAC         ║
echo ╚════════════════════════════════════════════════════════════════╝
echo.

REM Verifica si estamos en el directorio correcto
if not exist "DAL\" (
	echo ❌ Error: No se encontró la carpeta 'DAL'
	echo    Este script debe ejecutarse desde: C:\Users\vanina.garbisu\source\repos\Poyecto_Arquitectura_Web
	echo.
	pause
	exit /b 1
)

echo 📋 Opciones de ejecución:
echo.
echo   1. Aplicar migraciones usando Visual Studio (Recomendado)
echo   2. Aplicar migraciones usando línea de comandos
echo   3. Ver migraciones pendientes
echo.

set /p choice="Selecciona una opción (1-3): "

if "%choice%"=="1" (
	cls
	echo ✅ Abriendo Visual Studio Package Manager Console...
	echo.
	echo Instrucciones:
	echo   1. Se abrirá Visual Studio
	echo   2. Menú: Tools ^> NuGet Package Manager ^> Package Manager Console
	echo   3. Copia y pega este comando:
	echo.
	echo   Update-Database -ProjectName DAL -Verbose
	echo.
	echo   4. Presiona Enter
	echo.
	start ""
	pause
) else if "%choice%"=="2" (
	cls
	echo ⏳ Aplicando migraciones...
	echo.

	REM Intenta ejecutar dotnet ef migrations
	dotnet ef database update -p DAL -v

	if errorlevel 1 (
		echo.
		echo ❌ Error: dotnet ef no está instalado
		echo.
		echo Instala Entity Framework Core tools:
		echo   dotnet tool install --global dotnet-ef
		echo.
	) else (
		echo.
		echo ✅ Migraciones aplicadas exitosamente
		echo.
	)
	pause
) else if "%choice%"=="3" (
	cls
	echo 📋 Migraciones pendientes:
	echo.
	dotnet ef migrations list -p DAL
	pause
) else (
	echo ❌ Opción no válida
	pause
)

endlocal
