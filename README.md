# SwiftFinancialz

A large, long-running .NET Framework financial services (SACCO/microfinance)
system built on a layered/DDD architecture:

```
Domain.MainBoundedContext        aggregates, factories, specifications (per module: RegistryModule, AccountsModule, FrontOfficeModule, ...)
Application.MainBoundedContext   app services (business logic), one interface+impl per aggregate/feature
Application.MainBoundedContext.DTO  DTOs + BindingModels (validation) shared across all front ends
Infrastructure.Data.MainBoundedContext  EF mapping / repositories
DistributedServices.MainBoundedContext  legacy WCF (.svc) layer — being phased out
WebApplication1                  ASP.NET Web API — where active development happens
```

Full contributor notes (build instructions, architecture conventions, the
adapt-a-controller workflow, response envelope shape) live in
[`CLAUDE.md`](CLAUDE.md).

## Project direction

This codebase is mid-migration between two architectures, and current work is
aimed squarely at finishing that migration rather than growing the legacy
side further:

1. **Retire the WCF facade.** `DistributedServices.MainBoundedContext` (the
   `.svc` files) and the monolithic `IChannelService` it's built around are
   legacy plumbing being actively phased out, not extended. New work does not
   add to it.
2. **Go straight Web API, straight to the app service layer.**
   `WebApplication1` exposes `ApiController`s that call the focused
   per-aggregate app services in `Application.MainBoundedContext` directly —
   no facade, no channel service in between. Old MVC controllers in the
   reference codebase routed everything through `IChannelService`; the new
   controllers don't.
3. **Async/await is abandoned along with the WCF layer, not carried
   forward.** The `async Task`-based patterns you'll see throughout
   `DistributedServices.MainBoundedContext` were part of the WCF channel
   service strategy. The new `WebApplication1` controllers and the app
   service methods they call are written synchronously — this is
   intentional, not an oversight to "fix" by sprinkling `async`/`await`
   back in.

Progress is incremental and intentionally not synchronized across the
solution: old MVC controllers, WCF contracts, and `IChannelService` itself
are kept working on purpose while the new API surface is built out and
verified. See [`CLAUDE.md`](CLAUDE.md) for the full adapt-a-controller
workflow this migration follows.

## Getting the code and running it

```
git clone https://github.com/calvince-swizzsoft/Swift-Financials-FOSA.git
cd Swift-Financials-FOSA
```

This is an old-style (non-SDK) .NET Framework solution — there's no
top-level `.sln`, use [`SwiftFinancialz.slnx`](SwiftFinancialz.slnx). Every
project targets `.NET Framework 4.7.2`.

1. Open `SwiftFinancialz.slnx` in Visual Studio (2022/17.x or newer — a
   `18\Community` MSBuild toolset is what CI/local verification builds
   against) and let NuGet restore packages for all projects.
2. Provision a local SQL Server instance and create the databases referenced
   in `WebApplication1/Web.config`'s `<connectionStrings>`
   (`SwiftFinancialsDB_AuthStore`, `SwiftFinancialsDB_BLOBStore`, ...) — adjust
   the connection strings there to match your local server/credentials
   before running.
3. Set `WebApplication1` as the startup project and run (F5 / IIS Express) —
   that's the active Web API surface described above.
4. For one-off command-line verification builds of a single project (faster
   than a full solution build), invoke MSBuild directly:
   ```
   & "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" "<Project>\<Project>.csproj" /p:Configuration=Debug /nologo /v:minimal /t:Build
   ```

Because this is an old-style `.csproj` layout, every new `.cs` file needs an
explicit `<Compile Include="...">` entry added manually — the compiler will
not pick up new files automatically.

## Known issues

See [`KNOWNISSUES.md`](KNOWNISSUES.md) for known latent bugs and bug
patterns worth watching for elsewhere in the codebase.

## Documentation

- [`docs/api/`](docs/api/README.md) — client integration specs for each
  Web API area (customers, accounts, branches, text alerts, ...).
- [`docs/EMAIL-DELIVERY.md`](docs/EMAIL-DELIVERY.md) — end-to-end email
  delivery architecture: database persistence, MSMQ handoff, Windows Service
  dispatcher, SMTP transport, recovery scanning, runtime prerequisites, and
  operational/security cautions.
- [`docs/USER-ONBOARDING-AND-FIRST-LOGIN.md`](docs/USER-ONBOARDING-AND-FIRST-LOGIN.md) —
  user creation, centralized login-credentials email, first-login JWT gate,
  mandatory current-password confirmation/change, configuration, and test
  checklist.
- [`docs/CUSTOMER-EDITING.md`](docs/CUSTOMER-EDITING.md) — customer edit
  authorization, maker-checker staging and approval, workflow persistence,
  migration requirements, frontend behavior, and end-to-end test checklist.
- [`WebApplication1/Areas/FrontOffice/WORKFLOW.md`](WebApplication1/Areas/FrontOffice/WORKFLOW.md) —
  end-to-end functional workflow for the front office (teller transactions,
  maker-checker authorization, treasury cash movement, cheque lifecycle, end
  of day close, and ancillary processes like account closure and fixed
  deposits), including which parts are already ported to the new Web API
  and which remain.
- [`WebApplication1/Areas/FrontOffice/CHEQUE-PROCESSING-ANALYSIS.md`](WebApplication1/Areas/FrontOffice/CHEQUE-PROCESSING-ANALYSIS.md) —
  full-stack trace of every cheque capability (ChequeType, ChequeBook,
  ExternalCheque, Automated Clearing, InHouseCheque) across domain,
  application, DTO, infrastructure, WCF, and API layers, including GL
  account wiring per lifecycle stage and a wiring-correctness audit.
- [`WebApplication1/Areas/BackOffice/WORKFLOW.md`](WebApplication1/Areas/BackOffice/WORKFLOW.md) —
  end-to-end functional workflow for the back office loan origination
  pipeline (request intake, loan case registration, appraisal, approval,
  audit/verification, guarantor/collateral management, restructuring,
  cancellation, payroll check-off data capture, and disbursement), the
  full `LoanCase` state machine, and an implementation-status table — only
  disbursement batching is live so far, everything upstream is still
  unbuilt.
