param([string]$ServiceDirectory = 'C:\swiftfin\windows-service')
$ErrorActionPreference = 'Stop'
$package = Join-Path $PSScriptRoot 'windows-service'
$target = [IO.Path]::GetFullPath($ServiceDirectory).TrimEnd('\')
if ($target -eq [IO.Path]::GetPathRoot($target).TrimEnd('\')) { throw 'ServiceDirectory cannot be a drive root.' }
$exeName = 'SwiftFinancials.WindowsService.exe'
$configName = $exeName + '.config'
if (-not (Test-Path -LiteralPath (Join-Path $package $exeName))) { throw 'The windows-service package folder is missing.' }
$service = Get-CimInstance Win32_Service -Filter "Name='SwiftFinancialsService'"
if ($null -eq $service) { throw 'SwiftFinancialsService is not installed. Follow WINDOWS-SERVICE-DEPLOYMENT.md for a new installation.' }
if ($service.PathName.Trim('"') -ine (Join-Path $target $exeName)) { throw 'The registered service executable differs from ServiceDirectory. No changes made.' }
$identity = switch ($service.StartName) {
    'LocalSystem' { 'NT AUTHORITY\SYSTEM' }
    'NT AUTHORITY\LocalService' { 'NT AUTHORITY\LOCAL SERVICE' }
    'NT AUTHORITY\NetworkService' { 'NT AUTHORITY\NETWORK SERVICE' }
    default { $service.StartName }
}
$sid = ([Security.Principal.NTAccount]::new($identity)).Translate([Security.Principal.SecurityIdentifier])

# Retain the server's credentials, domain identifiers, SMTP and queue settings.
$config = New-Object System.Xml.XmlDocument
$config.PreserveWhitespace = $true
$config.Load((Join-Path $target $configName))
[xml]$incoming = Get-Content -LiteralPath (Join-Path $package $configName)
$runtime = $config.ImportNode($incoming.configuration.runtime, $true)
$oldRuntime = $config.SelectSingleNode('/configuration/runtime')
if ($oldRuntime) { [void]$config.DocumentElement.ReplaceChild($runtime, $oldRuntime) }
else { [void]$config.DocumentElement.AppendChild($runtime) }

function Grant-Folder([string]$path, [Security.AccessControl.FileSystemRights]$rights) {
    [void](New-Item -ItemType Directory -Path $path -Force)
    $acl = Get-Acl -LiteralPath $path
    $rule = [Security.AccessControl.FileSystemAccessRule]::new($sid, $rights,
        [Security.AccessControl.InheritanceFlags]'ContainerInherit,ObjectInherit',
        [Security.AccessControl.PropagationFlags]::None, [Security.AccessControl.AccessControlType]::Allow)
    $acl.AddAccessRule($rule)
    Set-Acl -LiteralPath $path -AclObject $acl
}
Grant-Folder (Join-Path $env:ProgramData 'SwiftFinancials\Logs') 'Modify'
$logSetting = $config.SelectSingleNode("/configuration/appSettings/add[@key='serilog:write-to:RollingFile.pathFormat']")
if ($logSetting) {
    $logPath = [Environment]::ExpandEnvironmentVariables($logSetting.GetAttribute('value'))
    if (-not [IO.Path]::IsPathRooted($logPath)) { throw 'Configure an absolute Serilog file path before updating.' }
    Grant-Folder (Split-Path $logPath) 'Modify'
}

# Provision only explicitly configured local private queues and preserve existing messages.
Import-Module MSMQ -ErrorAction Stop
$queuePaths = @($config.SelectNodes('//*[@queuePath]') | ForEach-Object { $_.GetAttribute('queuePath') } | Sort-Object -Unique)
$localQueues = @(Get-MsmqQueue -QueueType Private)
foreach ($path in $queuePaths) {
    if ($path -notlike '.\private$\*') { continue }
    $name = $path.Substring(2)
    $queue = @($localQueues | Where-Object { $_.QueueName -ieq $name })
    if ($queue.Count -eq 0) {
        $queue = @(New-MsmqQueue -Name $name.Substring('private$\'.Length) -QueueType Private -Transactional)
        Write-Host "Created configured transactional queue: $name"
    }
    if (-not $queue[0].Transactional) { throw "Queue '$name' must be transactional. Existing queue/messages were not replaced." }
    $queue | Set-MsmqQueueACL -UserName $identity -Allow ReceiveMessage,WriteMessage,GetQueueProperties,GetQueuePermissions | Out-Null
}

$backup = Join-Path (Split-Path $target) ('service-backups\' + (Get-Date -Format 'yyyyMMdd-HHmmss-fff'))
[void](New-Item -ItemType Directory -Path $backup)
$files = @(Get-ChildItem -LiteralPath $package -Recurse -File)
$added = New-Object 'System.Collections.Generic.List[string]'
$wasRunning = $service.State -eq 'Running'
Stop-Service -Name SwiftFinancialsService
try {
    Copy-Item -LiteralPath $target -Destination (Join-Path $backup 'windows-service') -Recurse
    foreach ($file in $files) {
        $relative = $file.FullName.Substring($package.Length + 1)
        if ($relative -eq $configName) { continue }
        $destination = [IO.Path]::GetFullPath((Join-Path $target $relative))
        if (-not $destination.StartsWith($target + '\', [StringComparison]::OrdinalIgnoreCase)) { throw 'Invalid package path.' }
        if (-not (Test-Path -LiteralPath $destination)) { $added.Add($destination) }
        [void](New-Item -ItemType Directory -Path (Split-Path $destination) -Force)
        Copy-Item -LiteralPath $file.FullName -Destination $destination -Force
    }
    $config.Save((Join-Path $target $configName))
    Grant-Folder $target 'ReadAndExecute'
    & (Join-Path $target $exeName) --check-startup
    if ($LASTEXITCODE -ne 0) { throw 'Startup preflight failed; restoring previous files.' }
    Start-Service -Name SwiftFinancialsService
    Write-Host "Update applied. Existing account and settings preserved. Backup: $backup"
    Write-Host 'Verify startup logs and send one new test email through the application.'
} catch {
    $failure = $_
    $saved = Join-Path $backup 'windows-service'
    if (Test-Path -LiteralPath (Join-Path $saved $configName)) {
        foreach ($file in Get-ChildItem -LiteralPath $saved -Recurse -File) {
            $relative = $file.FullName.Substring($saved.Length + 1)
            Copy-Item -LiteralPath $file.FullName -Destination (Join-Path $target $relative) -Force
        }
        foreach ($path in $added) {
            if (-not $path.StartsWith($target + '\', [StringComparison]::OrdinalIgnoreCase)) { throw 'Invalid rollback path.' }
            if (Test-Path -LiteralPath $path) { Remove-Item -LiteralPath $path }
        }
    }
    if ($wasRunning) { Start-Service -Name SwiftFinancialsService }
    throw $failure
}
