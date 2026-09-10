param([Parameter(Mandatory=$true)][string]$PackagePath)
$ErrorActionPreference = 'Stop'
$PackagePath = [IO.Path]::GetFullPath($PackagePath)
$source = Join-Path $PackagePath 'windows-service'
$fixture = Join-Path $env:TEMP ('SwiftFinServiceTests-' + [guid]::NewGuid().ToString('N'))
Copy-Item -LiteralPath $source -Destination $fixture -Recurse
$exe = Join-Path $fixture 'SwiftFinancials.WindowsService.exe'
$configPath = $exe + '.config'
$originalConfig = [IO.File]::ReadAllText($configPath)
[xml]$packageConfig = $originalConfig
if ($null -eq $packageConfig.SelectSingleNode("/configuration/connectionStrings/add[@name='SwiftApis']")) { throw 'Package is missing the SwiftApis database connection.' }
if ($null -eq $packageConfig.SelectSingleNode("/configuration/textDispatcherConfiguration/textDispatcherSettings/add[@uniqueId='SwiftApis' and @enabled='1']")) { throw 'Package is missing the enabled SwiftApis text dispatcher setting.' }
if ($null -eq $packageConfig.SelectSingleNode("/configuration/accountAlertDispatcherConfiguration/accountAlertDispatcherSettings/add[@uniqueId='SwiftApis' and @enabled='1']")) { throw 'Package is missing the enabled SwiftApis account-alert dispatcher setting.' }
if ($packageConfig.SelectSingleNode("/configuration/accountAlertDispatcherConfiguration/accountAlertDispatcherSettings/add[@uniqueId='SwiftApis']").GetAttribute('templatesPath') -ne 'C:\swiftfin\windows-service\App_Data\AccountAlertTemplates') { throw 'SwiftApis account-alert templatesPath does not target the installed service directory.' }
if ($packageConfig.SelectSingleNode('/configuration/textDispatcherConfiguration/textDispatcherSettings').GetAttribute('logEnabled') -ne '1') { throw 'SMS provider logging is not enabled in the package.' }
$templates = Join-Path $source 'App_Data\AccountAlertTemplates'
if (@(Get-ChildItem -LiteralPath $templates -Filter '*_TextTemplate.cshtml' -ErrorAction SilentlyContinue).Count -eq 0) { throw 'Package contains no text-alert templates.' }
Write-Host 'PASS: SwiftApis dispatcher settings and text-alert templates are packaged.'

function Run-Check([string]$path, [string]$arguments) {
    $info = New-Object Diagnostics.ProcessStartInfo
    $info.FileName = $path
    $info.Arguments = $arguments
    $info.UseShellExecute = $false
    $info.CreateNoWindow = $true
    $info.RedirectStandardOutput = $true
    $info.RedirectStandardError = $true
    $process = [Diagnostics.Process]::Start($info)
    $stdout = $process.StandardOutput.ReadToEndAsync()
    $stderr = $process.StandardError.ReadToEndAsync()
    if (-not $process.WaitForExit(60000)) { $process.Kill(); throw 'Diagnostic timed out.' }
    $result = [pscustomobject]@{ Code=$process.ExitCode; Text=$stdout.GetAwaiter().GetResult() + $stderr.GetAwaiter().GetResult() }
    $process.Dispose()
    return $result
}
$result = Run-Check $exe '--check-startup'
if ($result.Code -ne 0) { throw $result.Text }
Write-Host 'PASS: packaged dependency/plugin startup.'

$repository = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
$oldUnity = Join-Path $repository 'packages\Unity.5.8.6\lib\net47\Unity.Container.dll'
Copy-Item -LiteralPath $oldUnity -Destination (Join-Path $fixture 'Unity.Container.dll') -Force
$result = Run-Check $exe '--check-startup'
if ($result.Code -eq 0 -or $result.Text -notmatch 'Unity.Container' -or $result.Text -match 'NullReferenceException') { throw 'Stale Unity was not reported as a dependency failure.' }
Write-Host 'PASS: stale Unity fails with a nonzero exit and an independent diagnostic.'
Copy-Item -LiteralPath (Join-Path $source 'Unity.Container.dll') -Destination (Join-Path $fixture 'Unity.Container.dll') -Force

[xml]$xml = $originalConfig
$entry = $xml.SelectSingleNode('//emailDispatcherSettings/*[@uniqueId]')
$entry.SetAttribute('uniqueId', 'MissingDomainForTest')
$xml.Save($configPath)
$result = Run-Check $exe '--check-startup'
if ($result.Code -eq 0 -or $result.Text -notmatch 'no matching named connection string') { throw 'Missing domain connection was not rejected.' }
Write-Host 'PASS: domain/connection mismatch fails preflight.'
[xml]$xml = $originalConfig
$xml.SelectSingleNode('//emailDispatcherSettings').SetAttribute('queueReceivers', '0')
$xml.Save($configPath)
$result = Run-Check $exe '--check-startup'
if ($result.Code -eq 0 -or $result.Text -notmatch 'queueReceivers') { throw 'Zero receivers were not rejected.' }
Write-Host 'PASS: disabled email receiver fails preflight.'
[IO.File]::WriteAllText($configPath, $originalConfig)

$testExe = Join-Path $fixture 'EmailProcessingTests.exe'
Copy-Item -LiteralPath $configPath -Destination ($testExe + '.config')
$references = @('Application.MainBoundedContext.dll','Application.MainBoundedContext.DTO.dll','Application.Seedwork.dll','Infrastructure.Crosscutting.Framework.dll','SwiftFinancials.EmailAlertDispatcher.dll','SwiftFinancials.AppServiceContainer.dll','Unity.Abstractions.dll','Unity.Container.dll') | ForEach-Object { '/r:' + (Join-Path $fixture $_) }
$csc = Join-Path $env:windir 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
& $csc /nologo /target:exe "/out:$testExe" /r:System.Configuration.dll @references (Join-Path $PSScriptRoot 'tests\EmailProcessingTests.cs')
if ($LASTEXITCODE -ne 0) { throw 'Email test compilation failed.' }
$result = Run-Check $testExe ''
if ($result.Code -ne 0) { throw $result.Text }
Write-Host $result.Text
Write-Host "Tests passed. Isolated fixture: $fixture"
