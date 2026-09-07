# Windows service deployment and recovery

The service is registered as **SwiftFinancialsService** and runs
`SwiftFinancials.WindowsService.exe`. A Running status alone is not delivery
evidence. The repaired host initializes dependencies synchronously and fails
startup when dependency loading, plugin activation, or required email
configuration validation fails.

## Update an existing server

Extract the new package on the server, then run its updater from an elevated
Windows PowerShell session:

```powershell
& .\Update-WindowsService.ps1 -ServiceDirectory 'C:\swiftfin\windows-service'
```

The updater preserves the existing service account, database credentials, SMTP,
domain identifiers and queue paths. It merges the new runtime binding redirects,
backs up existing files, applies log-folder and configured local queue permissions
to the actual service identity, runs `--check-startup`, then starts the service.
If replacement or preflight fails, it restores the saved files. Queue/log ACL
grants are additive and are not rolled back. Missing configured local private
queues are created as transactional queues. Existing queues/messages are never
replaced or purged; a nontransactional configured queue stops the update for
separate review. Newly created queues are retained if an update is rolled back.

Do not install a second service called `SwiftFinancials.WindowsService` alongside
`SwiftFinancialsService`. Older installers incorrectly registered both names.
The corrected installer registers only `SwiftFinancialsService`. Review any old
duplicate registration separately; the updater does not delete services.

## New installation

Install MSMQ, deploy the complete `windows-service` folder, and configure the
environment first. From elevated Windows PowerShell:

```powershell
& "$env:windir\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe" 'C:\swiftfin\windows-service\SwiftFinancials.WindowsService.exe'
& 'C:\swiftfin\windows-service\SwiftFinancials.WindowsService.exe' --check-startup
if ($LASTEXITCODE -ne 0) { throw 'Fix the startup diagnostic before starting the service.' }
Start-Service -Name SwiftFinancialsService
```

The installer retains the existing source default of LocalSystem for new installs;
an existing Local Service or dedicated-account installation keeps its identity.
If selecting a different identity, grant Read/Execute to the application folder,
Modify to the log folders, appropriate attachment/template access, and Receive
and Send rights on its configured queues. Administrator queue access does not
grant the service account access. The updater can apply these folder/queue grants
after the service is installed; it also provisions configured local queues.

## Configuration contract

- The queued application's domain identifier, email dispatcher `uniqueId`, and
  named business database connection must match. Inspect the deployed API and
  service; do not blindly rename identifiers to `SwiftApis` or `SwiftFin_Dev`.
  Both API and service must read the same business database.
- Keep AuthStore/BLOBStore connections and SMTP credentials appropriate to the
  server. The database's physical name need not equal its connection-string name.
- The outer service broker's domain/key has its own lookup contract; do not change
  it as a blanket replacement while correcting email settings.
- Use at least one email receiver and an absolute logging path. The scheduled
  queuer's `enabled`, cron expression, `queueDaysCap`, and zero-retry filter control
  which older Pending records get re-enqueued. An empty queue does not mean sent:
  queue messages also have a configured expiry.
- Email domain mismatches and missing database records now throw and roll back
  the MSMQ receive instead of silently acknowledging the message. Delivery is
  still at least once: an SMTP acceptance followed by a database/transaction
  failure can result in a retry. There is no new dead-letter/backoff policy.

## Diagnostics and verification

```powershell
& 'C:\swiftfin\windows-service\SwiftFinancials.WindowsService.exe' --check-startup
$LASTEXITCODE
Get-MsmqQueue -QueueType Private | Select-Object QueueName, MessageCount
```

`--check-startup` checks factories, plugin construction, and email domain/connection
configuration. It does not start jobs, open queues, connect to SQL, or send mail.
Its exit code is 0 on success and nonzero on failure. Running it as Administrator
does not prove the service account's file, queue or network permissions.

Independent startup diagnostics go to `%ProgramData%\SwiftFinancials\Logs\startup-YYYYMMDD.log`,
with a `%TEMP%\SwiftFinancials-startup.log` fallback under the process identity.
Failures also attempt the Windows Application event log under `SwiftFinancialsService`.
Normal runtime errors remain in the Serilog path configured in the executable's
`.config`. Console diagnostics work even when Unity or Serilog cannot initialize.

After updating, verify startup diagnostics, compose a new test email through the
application, and check both its status and the recipient inbox. Direct SMTP success
only proves that test process can send; it bypasses the application's queue pipeline.
Delivered means the SMTP server accepted the email, not guaranteed inbox delivery.

The audit-trail and eStatement projects in this checkout contain placeholder
classes, not exported consumers. Their DLL presence does not prove those queues
are supported. This release does not implement those separate features.

## Reproduce the package

```powershell
& .\tools\deployment\Build-WindowsServicePackage.ps1 -BaseDeploymentPath '.\deployment-artifacts\complete-production-2026-09-04' -OutputPath '.\deployment-artifacts\windows-service-production-2026-09-06'
```

Restore repository NuGet dependencies before building on a clean machine. The
builder builds the selected plugin set and host, stages runtime files in a new
folder, selects the host's canonical Unity packages, preserves the chosen base
environment configuration, regenerates Unity redirects from actual assembly
identities, runs startup preflight and records SHA-256 hashes before producing
the ZIP. Do not package an arbitrary old `bin\Release` folder or copy DLLs from
the obsolete Unity 5.8.6 meta-package over host dependencies.

The environment-specific configuration and backups can contain credentials.
Distribute the production package and backup directory only to deployment operators.
