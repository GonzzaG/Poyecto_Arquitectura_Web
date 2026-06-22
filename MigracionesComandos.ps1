# Script de Migraciones - Ejecuta en Visual Studio Package Manager Console
# ============================================================================

# PASO 1: Ver migraciones pendientes
Write-Host "`n╔════════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║        PASO 1: Ver todas las migraciones                       ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════════╝`n" -ForegroundColor Cyan

Write-Host "Ejecuta en Package Manager Console:" -ForegroundColor Yellow
Write-Host "`nGet-Migrations -ProjectName DAL`n" -ForegroundColor Green


# PASO 2: Crear nueva migración que capture cambios pendientes
Write-Host "╔════════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║      PASO 2: Crear migración con cambios pendientes            ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════════╝`n" -ForegroundColor Cyan

Write-Host "Ejecuta en Package Manager Console:" -ForegroundColor Yellow
Write-Host "`nAdd-Migration FinalizarDVHYMapeados -ProjectName DAL`n" -ForegroundColor Green

Write-Host "Esto creará un nuevo archivo de migración con los cambios pendientes." -ForegroundColor White
Write-Host "El archivo se llamará algo como: 202701011300000_FinalizarDVHYMapeados.cs`n" -ForegroundColor White


# PASO 3: Aplicar la migración
Write-Host "╔════════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║      PASO 3: Aplicar migraciones a la base de datos           ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════════╝`n" -ForegroundColor Cyan

Write-Host "Ejecuta en Package Manager Console:" -ForegroundColor Yellow
Write-Host "`nUpdate-Database -ProjectName DAL -Verbose`n" -ForegroundColor Green

Write-Host "Deberías ver algo como:" -ForegroundColor White
Write-Host "  Applying Code First Migration: 202701011300000_FinalizarDVHYMapeados" -ForegroundColor Gray
Write-Host "  Running Seed method." -ForegroundColor Gray
Write-Host "  Migrations completed successfully.`n" -ForegroundColor Gray


# RESUMEN DE COMANDOS
Write-Host "╔════════════════════════════════════════════════════════════════╗" -ForegroundColor Yellow
Write-Host "║                    RESUMEN DE COMANDOS                        ║" -ForegroundColor Yellow
Write-Host "╚════════════════════════════════════════════════════════════════╝`n" -ForegroundColor Yellow

Write-Host "Copia y pega EXACTAMENTE estos comandos en:" -ForegroundColor White
Write-Host "Tools → NuGet Package Manager → Package Manager Console`n" -ForegroundColor Cyan

Write-Host "Comando 1 (Ver migraciones):" -ForegroundColor Green
Write-Host "Get-Migrations -ProjectName DAL`n" -ForegroundColor White

Write-Host "Comando 2 (Crear migración):" -ForegroundColor Green
Write-Host "Add-Migration FinalizarDVHYMapeados -ProjectName DAL`n" -ForegroundColor White

Write-Host "Comando 3 (Aplicar a BD):" -ForegroundColor Green
Write-Host "Update-Database -ProjectName DAL -Verbose`n" -ForegroundColor White

Write-Host "O simplemente ejecuta los tres de una vez:" -ForegroundColor Green
Write-Host "Get-Migrations -ProjectName DAL; Add-Migration FinalizarDVHYMapeados -ProjectName DAL; Update-Database -ProjectName DAL -Verbose`n" -ForegroundColor Cyan


# SOLUCIÓN ALTERNATIVA SI FALLA
Write-Host "`n╔════════════════════════════════════════════════════════════════╗" -ForegroundColor Red
Write-Host "║              SI FALLA - SOLUCIÓN ALTERNATIVA                  ║" -ForegroundColor Red
Write-Host "╚════════════════════════════════════════════════════════════════╝`n" -ForegroundColor Red

Write-Host "Si Update-Database sigue fallando, intenta:" -ForegroundColor Yellow
Write-Host "`n1. Limpiar y reconstruir:" -ForegroundColor White
Write-Host "   Build → Clean Solution" -ForegroundColor Gray
Write-Host "   Build → Build Solution`n" -ForegroundColor Gray

Write-Host "2. Eliminar migraciones recientes y empezar de cero:" -ForegroundColor White
Write-Host "   Remove-Migration -ProjectName DAL -Force`n" -ForegroundColor Gray

Write-Host "3. Crear nueva migración desde cero:" -ForegroundColor White
Write-Host "   Add-Migration InitialCreate -ProjectName DAL -Force`n" -ForegroundColor Gray

Write-Host "4. Aplicar:" -ForegroundColor White
Write-Host "   Update-Database -ProjectName DAL -Verbose`n" -ForegroundColor Gray

Write-Host "`n⚠️ Nota: Si la migración fue parcialmente aplicada, puede que necesites" -ForegroundColor Yellow
Write-Host "   limpiar el historial en SQL Server o eliminar la base de datos y reconstruirla.`n" -ForegroundColor Yellow

Write-Host "Presiona cualquier tecla para cerrar..." -ForegroundColor Cyan
Read-Host
