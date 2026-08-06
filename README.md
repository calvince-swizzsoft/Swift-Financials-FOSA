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

## Documentation

- [`docs/api/`](docs/api/README.md) — client integration specs for each
  Web API area (customers, accounts, branches, text alerts, ...).
- [`WebApplication1/Areas/FrontOffice/WORKFLOW.md`](WebApplication1/Areas/FrontOffice/WORKFLOW.md) —
  end-to-end functional workflow for the front office (teller transactions,
  maker-checker authorization, treasury cash movement, cheque lifecycle, end
  of day close, and ancillary processes like account closure and fixed
  deposits), including which parts are already ported to the new Web API
  and which remain.
