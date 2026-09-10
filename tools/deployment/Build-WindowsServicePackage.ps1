param(
    [Parameter(Mandatory=$true)][string]$BaseDeploymentPath,
    [Parameter(Mandatory=$true)][string]$OutputPath,
    [string]$MSBuildPath = 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe',
    [string]$ApplicationDomainName = 'SwiftApis',
    [string]$SourceDomainName = 'SwiftFin_Dev',
    [switch]$SkipBuild
)
$ErrorActionPreference = 'Stop'
$BaseDeploymentPath = [IO.Path]::GetFullPath($BaseDeploymentPath)
$OutputPath = [IO.Path]::GetFullPath($OutputPath)
$repository = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
$baseService = Join-Path $BaseDeploymentPath 'windows-service'
if (-not (Test-Path -LiteralPath $baseService)) { throw 'Base deployment must contain windows-service.' }
if (Test-Path -LiteralPath $OutputPath) { throw 'OutputPath must be new; existing deployments are never overwritten.' }
[void](New-Item -ItemType Directory -Path $OutputPath)
$destination = Join-Path $OutputPath 'windows-service'
[void](New-Item -ItemType Directory -Path $destination)

# Match the existing deployment's plugin set to source projects, including assemblies
# whose filenames differ from their project directory names.
$assemblyNames = @(Get-ChildItem -LiteralPath $baseService -Filter 'SwiftFinancials.*.dll' | ForEach-Object BaseName)
$projects = @(Get-ChildItem -LiteralPath $repository -Directory -Filter 'SwiftFinancials.*' | ForEach-Object {
    Get-ChildItem -LiteralPath $_.FullName -Filter '*.csproj'
} | Where-Object {
    [xml]$project = Get-Content -LiteralPath $_.FullName
    $name = $project.SelectSingleNode("//*[local-name()='AssemblyName']").InnerText
    $assemblyNames -contains $name
})
$hostProject = Join-Path $repository 'SwiftFinancials.WindowsService\SwiftFinancials.WindowsService.csproj'
$buildProjects = @($projects.FullName) + @($hostProject)
if (-not $SkipBuild) {
    foreach ($projectPath in $buildProjects) {
        $name = [IO.Path]::GetFileNameWithoutExtension($projectPath)
        Write-Host "Building $name"
        $buildOutput = & $MSBuildPath $projectPath /t:Build /p:Configuration=Release /p:PostBuildEvent= /nologo /v:minimal 2>&1
        $code = $LASTEXITCODE
        $buildOutput | Set-Content -LiteralPath (Join-Path $OutputPath ($name + '.build.log'))
        if ($code -ne 0) { $buildOutput | Select-Object -Last 50 | Write-Host; throw "Build failed: $name" }
    }
}

function Copy-Runtime([string]$sourceDirectory) {
    foreach ($file in Get-ChildItem -LiteralPath $sourceDirectory -Recurse -File) {
        if ($file.Extension -notin @('.dll', '.exe')) { continue }
        $relative = $file.FullName.Substring($sourceDirectory.Length + 1)
        $target = Join-Path $destination $relative
        if (Test-Path -LiteralPath $target) {
            $existingFile = Get-Item -LiteralPath $target
            if ($existingFile.Length -eq $file.Length -and $existingFile.LastWriteTimeUtc -eq $file.LastWriteTimeUtc) { continue }
            try {
                $incoming = [Reflection.AssemblyName]::GetAssemblyName($file.FullName)
                $current = [Reflection.AssemblyName]::GetAssemblyName($target)
                if ($current.Version -gt $incoming.Version) { continue }
                if ($current.Version -eq $incoming.Version -and $existingFile.LastWriteTimeUtc -gt $file.LastWriteTimeUtc) { continue }
            } catch [System.BadImageFormatException] { }
        }
        [void](New-Item -ItemType Directory -Path (Split-Path $target) -Force)
        Copy-Item -LiteralPath $file.FullName -Destination $target -Force
    }
}
foreach ($projectPath in $buildProjects) {
    Copy-Runtime (Join-Path (Split-Path $projectPath) 'bin\Release')
}

# Account-alert dispatchers compile Razor templates at runtime. They are data
# files rather than assemblies, so copy them explicitly into the deployed path.
$templatesSource = Join-Path $repository 'DistributedServices.MainBoundedContext\App_Data\AccountAlertTemplates'
$templatesDestination = Join-Path $destination 'App_Data\AccountAlertTemplates'
if (-not (Test-Path -LiteralPath $templatesSource)) { throw 'Account-alert templates source directory is missing.' }
[void](New-Item -ItemType Directory -Path $templatesDestination -Force)
Get-ChildItem -LiteralPath $templatesSource -Force | Copy-Item -Destination $templatesDestination -Recurse -Force
$textTemplateCount = @(Get-ChildItem -LiteralPath $templatesDestination -Filter '*_TextTemplate.cshtml').Count
if ($textTemplateCount -eq 0) { throw 'No text-alert templates were copied into the service package.' }

# Canonical host references override stale/transitive Unity meta-package binaries.
[xml]$hostXml = Get-Content -LiteralPath $hostProject
foreach ($reference in $hostXml.SelectNodes("//*[local-name()='Reference'][*[local-name()='HintPath']]")) {
    if ($reference.Include -notmatch '^(Unity\.|CommonServiceLocator,)') { continue }
    $hint = $reference.SelectSingleNode("*[local-name()='HintPath']").InnerText
    $file = [IO.Path]::GetFullPath((Join-Path (Split-Path $hostProject) $hint))
    $identity = [Reflection.AssemblyName]::GetAssemblyName($file)
    $expected = [Reflection.AssemblyName]::new($reference.Include)
    if ($identity.Version -ne $expected.Version) { throw "HintPath version mismatch: $($expected.Name)" }
    Copy-Item -LiteralPath $file -Destination (Join-Path $destination ([IO.Path]::GetFileName($file))) -Force
}

# Preserve the selected environment's credentials/domains and use the new build's
# runtime bindings. Never guess that a live installation's domain is SwiftApis.
$config = New-Object System.Xml.XmlDocument
$config.PreserveWhitespace = $true
$config.Load((Join-Path $baseService 'SwiftFinancials.WindowsService.exe.config'))

# API-originated queues identify their database/domain by ApplicationDomainName.
# Clone the selected environment's known-good settings so the service resolves
# the same database and provider credentials without hard-coding live secrets.
$sourceConnection = $config.SelectSingleNode("/configuration/connectionStrings/add[@name='$SourceDomainName']")
$targetConnection = $config.SelectSingleNode("/configuration/connectionStrings/add[@name='$ApplicationDomainName']")
if ($null -eq $targetConnection) {
    if ($null -eq $sourceConnection) { throw "Base deployment has no '$SourceDomainName' connection to clone for '$ApplicationDomainName'." }
    $targetConnection = $sourceConnection.CloneNode($true)
    $targetConnection.SetAttribute('name', $ApplicationDomainName)
    [void]$sourceConnection.ParentNode.AppendChild($targetConnection)
}
function Ensure-DomainSetting([string]$settingsXPath) {
    $settings = $config.SelectSingleNode($settingsXPath)
    if ($null -eq $settings) { throw "Missing dispatcher settings: $settingsXPath" }
    $existing = $settings.SelectSingleNode("add[@uniqueId='$ApplicationDomainName']")
    if ($null -eq $existing) {
        $source = $settings.SelectSingleNode("add[@uniqueId='$SourceDomainName']")
        if ($null -eq $source) { throw "No '$SourceDomainName' dispatcher setting to clone under $settingsXPath." }
        $existing = $source.CloneNode($true)
        $existing.SetAttribute('uniqueId', $ApplicationDomainName)
        [void]$settings.AppendChild($existing)
    }
    $existing.SetAttribute('enabled', '1')
}
Ensure-DomainSetting '/configuration/textDispatcherConfiguration/textDispatcherSettings'
Ensure-DomainSetting '/configuration/accountAlertDispatcherConfiguration/accountAlertDispatcherSettings'
$textSettings = $config.SelectSingleNode('/configuration/textDispatcherConfiguration/textDispatcherSettings')
$textSettings.SetAttribute('logEnabled', '1')
$installedTemplatesPath = 'C:\swiftfin\windows-service\App_Data\AccountAlertTemplates'
foreach ($accountSetting in $config.SelectNodes('/configuration/accountAlertDispatcherConfiguration/accountAlertDispatcherSettings/add')) {
    $accountSetting.SetAttribute('templatesPath', $installedTemplatesPath)
}
[xml]$built = Get-Content -LiteralPath (Join-Path $repository 'SwiftFinancials.WindowsService\bin\Release\SwiftFinancials.WindowsService.exe.config')
$runtime = $config.ImportNode($built.configuration.runtime, $true)
$oldRuntime = $config.SelectSingleNode('/configuration/runtime')
if ($oldRuntime) { [void]$config.DocumentElement.ReplaceChild($runtime, $oldRuntime) }
else { [void]$config.DocumentElement.AppendChild($runtime) }
$ns = 'urn:schemas-microsoft-com:asm.v1'
$binding = $runtime.SelectSingleNode("*[local-name()='assemblyBinding']")
if ($null -eq $binding) { $binding = $runtime.AppendChild($config.CreateElement('assemblyBinding', $ns)) }
foreach ($file in Get-ChildItem -LiteralPath $destination -Filter '*.dll') {
    if ($file.Name -notmatch '^(Unity\.|CommonServiceLocator\.)') { continue }
    $identity = [Reflection.AssemblyName]::GetAssemblyName($file.FullName)
    foreach ($old in $runtime.SelectNodes(".//*[local-name()='dependentAssembly'][*[local-name()='assemblyIdentity' and @name='$($identity.Name)']]")) {
        [void]$old.ParentNode.RemoveChild($old)
    }
    $token = ([BitConverter]::ToString($identity.GetPublicKeyToken())).Replace('-', '').ToLowerInvariant()
    $fragment = $config.CreateDocumentFragment()
    $fragment.InnerXml = '<dependentAssembly xmlns="' + $ns + '"><assemblyIdentity name="' + $identity.Name + '" publicKeyToken="' + $token + '" culture="neutral"/><bindingRedirect oldVersion="0.0.0.0-' + $identity.Version + '" newVersion="' + $identity.Version + '"/></dependentAssembly>'
    [void]$binding.AppendChild($fragment)
}
$config.Save((Join-Path $destination 'SwiftFinancials.WindowsService.exe.config'))
foreach ($name in @('Update-WindowsService.ps1', 'WINDOWS-SERVICE-DEPLOYMENT.md')) {
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot $name) -Destination (Join-Path $OutputPath $name)
}
& (Join-Path $destination 'SwiftFinancials.WindowsService.exe') --check-startup 2>&1 | Tee-Object -FilePath (Join-Path $OutputPath 'startup-check.log')
if ($LASTEXITCODE -ne 0) { throw 'Package preflight failed. No deployment ZIP was produced.' }

$manifest = @(Get-ChildItem -LiteralPath $destination -Recurse -File | ForEach-Object {
    $assembly = $null
    if ($_.Extension -eq '.dll' -or $_.Extension -eq '.exe') {
        try { $assembly = [Reflection.AssemblyName]::GetAssemblyName($_.FullName).FullName } catch [System.BadImageFormatException] { }
    }
    [pscustomobject]@{ Path=$_.FullName.Substring($destination.Length+1); Assembly=$assembly; SHA256=(Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash }
})
$manifest | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath (Join-Path $OutputPath 'manifest.json') -Encoding UTF8
Add-Type -AssemblyName System.IO.Compression.FileSystem
[IO.Compression.ZipFile]::CreateFromDirectory($OutputPath, ($OutputPath + '.zip'))
Write-Host "Verified package: $OutputPath.zip"
