# Income assessment migration reconciliation

The local income-assessment SQL script added four columns before EF automatic
migration recorded them. The migration SQL generator now recognizes only those
specific table/column pairs, verifies their SQL types, lengths and nullability,
and lets EF write its own migration-history snapshot. Unknown columns retain
normal EF behavior; incompatible existing definitions fail explicitly.

Verification on local development:
- Debug utility build passed in bin/MigrationVerification (normal Debug output
  was held open by the Visual Studio debugger).
- IncomeMigrationVerify.cs exercised missing, existing and incompatible columns
  on disposable SQL temporary tables, plus unchanged behavior for unrelated tables.
- Pending migration reviewed: four guarded column additions and EF history only.
- DbMigrator.Update completed, followed by Database.Initialize(true).
- Every loan-product row compared unchanged before and after reconciliation.

IncomeMigrationVerify.cs is a standalone .NET Framework verification harness.
Compile against EntityFramework.dll, EntityFramework.SqlServer.dll,
Infrastructure.Data.MainBoundedContext.dll, System.Configuration.dll and
System.Data.dll from the Debug utility output, with a copy of the utility's
runtime config named IncomeMigrationVerify.exe.config. Keep configuration and
generated migration previews in ignored bin output, never in source control.

Modes: test (temporary tables only), preview (writes migration-preview.sql),
apply (updates migration history/schema and verifies initialization). The harness
is restricted to (local), SwiftFinancialsDB_Live via the SwiftFin_Dev alias.
Review preview before apply; apply is a database-writing operation.