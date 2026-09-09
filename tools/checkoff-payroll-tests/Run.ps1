$ErrorActionPreference='Stop'
$backendRoot=Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$backendBin=Join-Path $backendRoot 'WebApplication1\bin'
$testExe=Join-Path $env:TEMP ('checkoff-payroll-'+[guid]::NewGuid().ToString('N')+'.exe')
$refs=@('Application.MainBoundedContext.dll','Application.MainBoundedContext.DTO.dll','Application.Seedwork.dll','Infrastructure.Crosscutting.Framework.dll') | ForEach-Object { '/r:'+(Join-Path $backendBin $_) }
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo "/out:$testExe" $refs (Join-Path $PSScriptRoot 'Regression.cs')
if($LASTEXITCODE -ne 0) { throw 'Test compilation failed.' }
& $testExe $backendBin
if($LASTEXITCODE -ne 0) { throw 'Check-Off regression failed.' }
