param([Parameter(Mandatory=$true)][string]$ApiConfig)
$ErrorActionPreference='Stop'
$ApiConfig=(Resolve-Path -LiteralPath $ApiConfig).Path
$executable=Join-Path $PSScriptRoot 'bin/Debug/Sasra.Tests.exe'
if(!(Test-Path -LiteralPath $executable)){throw 'Build Sasra.Tests in Debug first.'}
$configPath=$executable+'.config'
$backup=if(Test-Path -LiteralPath $configPath){[IO.File]::ReadAllBytes($configPath)}else{$null}
try {
    $source=[xml](Get-Content -LiteralPath $ApiConfig -Raw)
    $target=[xml]'<configuration><configSections /></configuration>'
    $section=$source.configuration.configSections.SelectSingleNode("section[@name='serviceBrokerConfiguration']")
    $null=$target.configuration.SelectSingleNode('configSections').AppendChild($target.ImportNode($section,$true))
    $null=$target.configuration.AppendChild($target.ImportNode($source.configuration.serviceBrokerConfiguration,$true))
    $declaration=$target.CreateElement('section')
    $declaration.SetAttribute('name','entityFramework')
    $declaration.SetAttribute('type','System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework')
    $null=$target.configuration.SelectSingleNode('configSections').AppendChild($declaration)
    $ef=$target.CreateElement('entityFramework')
    $ef.SetAttribute('codeConfigurationType','CatalogueInstall+CatalogueConfiguration, Sasra.Tests')
    $null=$target.configuration.AppendChild($ef)
    $target.Save($configPath)
    & $executable --install-standard-catalogue $ApiConfig
    if($LASTEXITCODE -ne 0){throw 'Standard catalogue installation failed. Check the preceding error.'}
} finally {
    if($null -ne $backup){[IO.File]::WriteAllBytes($configPath,$backup)}
    elseif(Test-Path -LiteralPath $configPath){Remove-Item -LiteralPath $configPath}
}
