param(
    [Parameter(Mandatory=$true)][string]$BaseDeploymentPath,
    [Parameter(Mandatory=$true)][string]$ServicePackagePath,
    [Parameter(Mandatory=$true)][string]$OutputPath
)
$ErrorActionPreference = 'Stop'
$BaseDeploymentPath = [IO.Path]::GetFullPath($BaseDeploymentPath)
$ServicePackagePath = [IO.Path]::GetFullPath($ServicePackagePath)
$OutputPath = [IO.Path]::GetFullPath($OutputPath)
if (Test-Path -LiteralPath $OutputPath) { throw 'Use a new output directory.' }
if (-not (Test-Path -LiteralPath (Join-Path $ServicePackagePath 'manifest.json'))) { throw 'A verified service package is required.' }
$runtime = Join-Path $ServicePackagePath 'windows-service'
foreach ($entry in (Get-Content -LiteralPath (Join-Path $ServicePackagePath 'manifest.json') -Raw | ConvertFrom-Json)) {
    $path = [IO.Path]::GetFullPath((Join-Path $runtime $entry.Path))
    if (-not $path.StartsWith($runtime + '\', [StringComparison]::OrdinalIgnoreCase)) { throw 'Invalid manifest path.' }
    if ((Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash -ne $entry.SHA256) { throw "Manifest mismatch: $($entry.Path)" }
}
[void](New-Item -ItemType Directory -Path $OutputPath)
$serviceOnly = Join-Path $OutputPath 'service-package'
$complete = Join-Path $OutputPath 'complete-package'
[void](New-Item -ItemType Directory -Path $serviceOnly)
[void](New-Item -ItemType Directory -Path $complete)
foreach ($item in Get-ChildItem -LiteralPath $BaseDeploymentPath) {
    if ($item.Name -eq 'windows-service') { continue }
    Copy-Item -LiteralPath $item.FullName -Destination (Join-Path $complete $item.Name) -Recurse
}
foreach ($target in @($serviceOnly, $complete)) {
    Copy-Item -LiteralPath $runtime -Destination (Join-Path $target 'windows-service') -Recurse
    Copy-Item -LiteralPath (Join-Path $ServicePackagePath 'manifest.json') -Destination (Join-Path $target 'windows-service-manifest.json')
    Copy-Item -LiteralPath (Join-Path $ServicePackagePath 'startup-check.log') -Destination (Join-Path $target 'startup-check.log')
    foreach ($name in @('Update-WindowsService.ps1','WINDOWS-SERVICE-DEPLOYMENT.md')) {
        Copy-Item -LiteralPath (Join-Path $PSScriptRoot $name) -Destination (Join-Path $target $name)
    }
    @'
Windows service repair release — 2026-09-06

Verified: Unity.Container 5.8.13.0 and aligned Unity dependencies; 29 plugins;
startup dependency/configuration preflight; stale Unity rejection; invalid
domain and zero-receiver rejection; isolated email processing regression tests.

This is a service-only change to the previous complete deployment. API,
frontend and database utility files are retained from the selected base bundle.
Use Update-WindowsService.ps1 for an existing server so its settings/account
are preserved. No production queue processing or real SMTP delivery was
performed during package verification.
'@ | Set-Content -LiteralPath (Join-Path $target 'SERVICE-RELEASE-NOTES.txt') -Encoding UTF8
}
$repository = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
$deploymentDoc = [IO.File]::ReadAllText((Join-Path $repository 'docs\DEPLOYMENT.md')).Replace('../tools/deployment/WINDOWS-SERVICE-DEPLOYMENT.md','WINDOWS-SERVICE-DEPLOYMENT.md')
[IO.File]::WriteAllText((Join-Path $complete 'DEPLOYMENT.md'), $deploymentDoc)
Add-Type -AssemblyName System.IO.Compression.FileSystem
$serviceZip = Join-Path $OutputPath 'swiftfinancialz-windows-service-production-2026-09-06-windows.zip'
$completeZip = Join-Path $OutputPath 'swiftfinancialz-complete-production-2026-09-06-windows.zip'
[IO.Compression.ZipFile]::CreateFromDirectory($serviceOnly, $serviceZip)
[IO.Compression.ZipFile]::CreateFromDirectory($complete, $completeZip)
Get-FileHash -LiteralPath $serviceZip,$completeZip -Algorithm SHA256 | Format-List
