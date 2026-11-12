param([string]$Server = '(localdb)\MSSQLLocalDB', [switch]$Legacy)
$ErrorActionPreference = 'Stop'
$repoPath = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$msbuildPath = Join-Path ${env:ProgramFiles} 'Microsoft Visual Studio/2022/Community/MSBuild/Current/Bin/MSBuild.exe'
if (!(Test-Path -LiteralPath $msbuildPath)) { $msbuildPath = (Get-Command MSBuild.exe -ErrorAction Stop).Source }
& $msbuildPath (Join-Path $PSScriptRoot 'IntegrityChecks.csproj') /t:Build /v:minimal /nologo
if ($LASTEXITCODE -ne 0) { throw 'No se pudo compilar.' }
$exePath = Join-Path $PSScriptRoot 'bin/Debug/IntegrityChecks.exe'
[xml]$testConfig = Get-Content -LiteralPath ($exePath + '.config') -Raw
$connectionBuilder = New-Object System.Data.SqlClient.SqlConnectionStringBuilder
$connectionBuilder['Data Source'] = $Server
$connectionBuilder['Initial Catalog'] = 'ArquitecturaWeb_IntegrityChecks_' + [Guid]::NewGuid().ToString('N')
$connectionBuilder['Integrated Security'] = $true
$connectionBuilder['MultipleActiveResultSets'] = $true
$testConfig.configuration.connectionStrings.add.connectionString = $connectionBuilder.ConnectionString
$testConfig.Save($exePath + '.config')
if ($Legacy) { & $exePath --legacy } else { & $exePath }
if ($LASTEXITCODE -ne 0) { throw 'Fallaron las comprobaciones de integridad.' }
