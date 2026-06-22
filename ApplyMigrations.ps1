# Script para aplicar migraciones a la base de datos
# Este script debe ejecutarse en Visual Studio Package Manager Console

# Ejecuta Update-Database en el contexto correcto
Write-Host "Aplicando migraciones pendientes..." -ForegroundColor Green
Write-Host "Proyecto: DAL" -ForegroundColor Cyan

# Este comando debe ejecutarse en Visual Studio Package Manager Console
# Update-Database -ProjectName DAL -Verbose

# Alternativa: Usar ejecutable de migrations
Write-Host "`n=== INSTRUCCIONES ===" -ForegroundColor Yellow
Write-Host "1. Abre Visual Studio" -ForegroundColor White
Write-Host "2. Menú: Tools > NuGet Package Manager > Package Manager Console" -ForegroundColor White
Write-Host "3. En la consola, escribe:" -ForegroundColor White
Write-Host "   Update-Database -ProjectName DAL -Verbose" -ForegroundColor Cyan
Write-Host "4. Presiona Enter y espera a que termine" -ForegroundColor White
Write-Host "`nO usa la página web: http://localhost:44389/Pages/ApplyMigrations.aspx" -ForegroundColor Yellow
