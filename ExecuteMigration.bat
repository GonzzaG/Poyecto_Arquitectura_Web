@echo off
REM Script para aplicar migraciones de Entity Framework
REM Uso: ExecuteMigration.bat

cd /d "%~dp0"

REM Obtener la ruta de NuGet
set NUGET_PATH=.nuget\nuget.exe

if not exist "%NUGET_PATH%" (
	echo Descargando NuGet...
	powershell -Command "& {(New-Object Net.WebClient).DownloadFile('https://dist.nuget.org/win-x86-commandline/latest/nuget.exe', '.nuget\nuget.exe')}"
)

REM Restaurar paquetes
"%NUGET_PATH%" restore

REM Aplicar migraciones usando Package Manager Console
echo Aplicando migraciones de Entity Framework...
powershell -Command "& {Add-Type -Path '.\DAL\bin\Debug\EntityFramework.dll'; Add-Type -Path '.\DAL\bin\Debug\DAL.dll'; $config = New-Object DAL.Migrations.Configuration; $migrator = New-Object System.Data.Entity.Migrations.DbMigrator $config; $migrator.Update()}"

echo.
echo Migraciones completadas.
pause
