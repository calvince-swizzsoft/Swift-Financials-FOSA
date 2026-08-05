# EF6 model/database sync: how it broke and how it's wired

Explainer for the `InvalidOperationException: The model backing the
'BoundedContextUnitOfWork' context has changed since the database was
created` error, the EF6 mechanisms behind it, and the alternatives to the
fix that's now in place. Read this before touching anything under
`Infrastructure.Data.MainBoundedContext/Migrations` or
`UnitOfWork/BoundedContextConfiguration.cs`.

## The moving parts

**`BoundedContextUnitOfWork`** (`Infrastructure.Data.MainBoundedContext/UnitOfWork/BoundedContextUnitOfWork.cs`)
is the one Code First `DbContext` for the whole solution. Its shape —
entities, navigation properties, `OnModelCreating` conventions, the
`Configurations.AddFromAssembly` fluent mappings — is "the model." Every
time an entity or mapping changes, the model changes.

**`__MigrationHistory`** is a table EF creates in the target database. Each
row records a migration that was applied, plus a compressed snapshot of the
model at that point (used to compute a comparable hash) and a `ContextKey`
column identifying which configuration wrote the row.

**A database initializer** (`IDatabaseInitializer<TContext>`) is the
strategy EF runs the first time a given `DbContext` type is used in an
AppDomain. The built-in ones:

| Initializer | Behavior |
|---|---|
| `CreateDatabaseIfNotExists<T>` (EF6 default if none is set) | Creates the DB if missing. If it already exists, compares the current model against `__MigrationHistory` and **throws** on any mismatch. Never alters an existing DB. |
| `MigrateDatabaseToLatestVersion<T, TConfig>` | Runs any pending migrations (scaffolded or, with `AutomaticMigrationsEnabled`, auto-generated) to bring the DB in line with the model. This is what `AutoConfiguration` (below) is. |
| `DropCreateDatabaseIfModelChanges<T>` / `DropCreateDatabaseAlways<T>` | Dev/test-only — drops and recreates the DB. Destructive; never point this at `SwiftFinancialsDB_Live`. |
| `null` (`Database.SetInitializer<T>(null)`) | Disables all automatic checking/creation. Assumes an external process (a DBA script, a deploy pipeline) keeps schema in sync. |

**`Infrastructure.Data.MainBoundedContext/Migrations/Configuration.cs`**
defines `Configuration : DbMigrationsConfiguration<BoundedContextUnitOfWork>`
with `AutomaticMigrationsEnabled = true` and
`AutomaticMigrationDataLossAllowed = true` — so schema diffs are inferred
from the model rather than hand-written migration files (there are no
scaffolded migration classes in this project at all). It also defines
`AutoConfiguration : MigrateDatabaseToLatestVersion<BoundedContextUnitOfWork, Configuration>`,
a ready-to-use initializer wrapping that migrations config.

**`BoundedContextConfiguration : DbConfiguration`**
(`UnitOfWork/BoundedContextConfiguration.cs`) is EF6's per-AppDomain config
hook, wired in via `codeConfigurationType` in each host's `Web.config`
(`WebApplication1`, `DistributedServices.MainBoundedContext`). Anything set
here applies solution-wide, to every `BoundedContextUnitOfWork` instance any
project creates.

**`SwiftFinancials.Utility`** is a console tool
(`SwiftFinancials.Utility.exe <ApplicationDomainName>`) that calls a WCF
method, `UtilityService.ConfigureApplicationDatabase()`
(`DistributedServices.MainBoundedContext/UtilityService.svc.cs`), which
manually does:
```csharp
var autoConfiguration = new AutoConfiguration(true);
var context = _dbContextFactory.CreateDbContext<BoundedContextUnitOfWork>(serviceHeader);
autoConfiguration.InitializeDatabase(context);
```
i.e. it constructs the migrations-based initializer itself and runs it
once, by hand, against whichever database `serviceHeader.ApplicationDomainName`
resolves to. This is the "run migrations" step referenced elsewhere in this
repo's history.

**Which database, actually**: `RuntimeContextFactory` resolves the
connection string by looking up `ConfigurationManager.ConnectionStrings[serviceHeader.ApplicationDomainName]`
in whichever host's config is active. Both `WebApplication1\Web.config` and
`DistributedServices.MainBoundedContext\Web.config` define
`ApplicationDomainName = "SwiftFin_Dev"` pointing at
**`SwiftFinancialsDB_Live`**. The class library's own
`Infrastructure.Data.MainBoundedContext\App.config` has a *different*,
unrelated connection string entry (`BoundedContextUnitOfWork` →
`SwiftFinancialsDB_DEV`, a database that doesn't even exist on this
machine) — that entry is only ever read by design-time EF tooling
(Package Manager Console), never by the running apps. Don't confuse the
two when debugging a "wrong database" symptom.

## What actually went wrong

1. A code change added a `Branch` navigation property to `Workflow`
   (`Domain.MainBoundedContext/.../WorkflowAgg/Workflow.cs`) — a model
   change, even though the backing `BranchId` column already existed.
2. Running `SwiftFinancials.Utility.exe` → `ConfigureApplicationDatabase`
   correctly applied an automatic migration and updated
   `SwiftFinancialsDB_Live`'s `__MigrationHistory` — confirmed directly via
   SQL, including the new `Workflow.BranchId` mapping and a fresh
   `AutomaticMigration` row.
3. **But nothing had ever called `Database.SetInitializer` for
   `BoundedContextUnitOfWork`.** `BoundedContextConfiguration` only set a
   SQL migration generator. So every ordinary code path (`Repository<T>.Get`,
   any `DbSet` access in `WebApplication1` or `DistributedServices`) still
   ran the *default* `CreateDatabaseIfNotExists` initializer the moment it
   first touched the context.
4. `CreateDatabaseIfNotExists`'s compatibility check reads
   `__MigrationHistory` keyed by the **`DbContext` type's own name**
   (`Infrastructure.Data.MainBoundedContext.UnitOfWork.BoundedContextUnitOfWork`).
   The rows the Utility tool writes are keyed by the **migrations
   configuration's type name**
   (`Infrastructure.Data.MainBoundedContext.Migrations.Configuration`) —
   confirmed via `SELECT DISTINCT ContextKey FROM __MigrationHistory`,
   which returned only the latter. Two different keys, so the default
   initializer never finds a compatible row and throws — regardless of how
   many times the Utility tool is re-run, because it's fixing a database
   the *other* check isn't looking at.
5. This had gone unnoticed before because it likely only ever "worked" on a
   database that didn't already exist yet — `CreateDatabaseIfNotExists`
   creates a fresh DB unconditionally with no compatibility check at all in
   that case. `SwiftFinancialsDB_Live` is long-lived, so this was the first
   time the mismatch actually got exercised.

## The fix that's in place

One line in `BoundedContextConfiguration`'s constructor:
```csharp
SetDatabaseInitializer(new AutoConfiguration(true));
```
This makes the *ambient* path (every ordinary `DbContext` use, in every
host — `WebApplication1`, `DistributedServices`, and any plugin/scheduler
host that loads a project touching `BoundedContextUnitOfWork`) run the
same migrations-aware initializer the Utility tool already used manually —
so all paths check/update the same history, under the same key, going
forward.

The constructor argument matters and is easy to get backwards:
`AutoConfiguration(bool useSuppliedContext)` maps to
`MigrateDatabaseToLatestVersion`'s `useSuppliedContext` flag.
- `true` — use the **actual context instance's** connection, i.e. whatever
  `RuntimeContextFactory` resolved from `ServiceHeader.ApplicationDomainName`
  (`SwiftFin_Dev` → `SwiftFinancialsDB_Live`). This is what the Utility
  tool's manual call already uses, and what's needed here.
- `false` — **ignore** the supplied context's connection and instead
  resolve one by EF naming convention. None of the host `Web.config`s
  define a connection string literally named `BoundedContextUnitOfWork`, so
  this falls through to the default `SqlConnectionFactory` convention (a
  `.\SQLEXPRESS`-style instance) — which doesn't exist here, producing
  `SqlException: ... Error Locating Server/Instance Specified` (error 26).
  This is a real mistake that was shipped briefly during this fix and
  immediately corrected — worth calling out explicitly since the two
  failure modes (EF model-hash mismatch vs. a `SqlException` about an
  unreachable server) look unrelated but came from one flipped boolean.

`AutomaticMigrationDataLossAllowed = true` is already set in
`Configuration.cs`, so this also auto-applies future model changes without
a separate manual Utility.exe run — worth knowing since that trades
convenience for the possibility of an automatic migration silently
dropping a column on a genuine breaking change.

## Alternatives considered (and why they weren't picked)

- **Set the `Configuration`'s `ContextKey` explicitly** to match
  `BoundedContextUnitOfWork`'s default key, instead of wiring
  `SetDatabaseInitializer`. Would resolve the key mismatch too, but leaves
  the ambient default (`CreateDatabaseIfNotExists`) in place, which still
  never *applies* a migration on its own — it only ever throws or creates
  fresh. You'd still need the manual Utility.exe step every time, just with
  a matching key.
- **Scaffold real migrations** (`Add-Migration`) instead of relying purely
  on `AutomaticMigrationsEnabled`. More explicit and reviewable (you get a
  diffable `Up()`/`Down()` per change, and CI can gate on "no pending model
  changes without a migration"), but a bigger process change than this
  incident called for. Worth revisiting if automatic migrations keep
  causing surprises — e.g. multiple `AutomaticMigration` rows landing the
  same day (as happened here) is a sign the model was drifting faster than
  anyone was reviewing.
- **`DropCreateDatabaseIfModelChanges`** — rejected outright for anything
  pointed at `SwiftFinancialsDB_Live`; this drops data.
- **Disable initialization entirely** (`SetDatabaseInitializer<BoundedContextUnitOfWork>(null)`)
  and rely solely on the Utility tool / DBA-run scripts for schema sync.
  More conservative (no risk of an automatic migration firing
  unexpectedly), but reintroduces exactly this failure mode: ordinary app
  code would get no warning at all if the model and DB drift — it would
  just throw whatever exception the actual mismatched call produces further
  downstream, or silently break at the SQL level, once no initializer
  guards the entry point.

## If this recurs

- Confirm which database is actually in play — check the *host's*
  `Web.config` (`WebApplication1`/`DistributedServices`) for the
  connection string named after whatever `ApplicationDomainName` is in use,
  not `Infrastructure.Data.MainBoundedContext`'s own `App.config`.
- `SELECT DISTINCT ContextKey FROM __MigrationHistory` on the target
  database — if you ever see more than one key, or the "wrong" key for
  whichever initializer is ambient, that's the same class of bug.
- The stack trace is diagnostic: `CreateDatabaseIfNotExists<T>.InitializeDatabase`
  means the ambient default is what ran — if you've set an initializer and
  still see this frame, the wiring isn't taking effect (check `Web.config`'s
  `codeConfigurationType` actually points at the assembly/type you expect).
