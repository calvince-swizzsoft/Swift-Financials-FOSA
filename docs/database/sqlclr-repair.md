# SQLCLR repair for `SwiftFinancialsDB_Live`

## Problem

SQL Server raised the following exception while the database initialization script tried to create CLR functions:

```text
System.Data.SqlClient.SqlException:
Assembly 'Infrastructure.Data.SQLCLR' was not found in the SQL catalog of
database 'SwiftFinancialsDB_Live'.
```

The SQLCLR project compiling successfully, or copying its DLL into an application's `bin` directory, does not register the assembly in a SQL Server database. SQLCLR assemblies must be installed in each database that uses them.

## Diagnosis

The local application connection points to:

```text
Server:   (local)
Database: SwiftFinancialsDB_Live
```

The live database was inspected and reported:

- SQL Server product version `17.0.1125.2`.
- CLR integration was enabled.
- `clr strict security` was enabled.
- `Infrastructure.Data.SQLCLR` was absent from `sys.assemblies`.
- The five CLR functions were absent from `sys.objects`.

The missing functions were:

- `dbo.FV`
- `dbo.IPmt`
- `dbo.Pmt`
- `dbo.PPmt`
- `dbo.RepaymentSchedule`

## Repair performed

The built assembly used for the repair was:

```text
Infrastructure.Data.SQLCLR/bin/Debug/Infrastructure.Data.SQLCLR.dll
```

Its assembly identity was:

```text
Infrastructure.Data.SQLCLR, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
```

The following database-side actions were performed:

1. Calculated the DLL's SHA-512 hash.
2. Added that hash with `sys.sp_add_trusted_assembly`.
3. Registered the DLL in `SwiftFinancialsDB_Live` using `CREATE ASSEMBLY` with `PERMISSION_SET = SAFE`.
4. Created the five SQL functions whose `EXTERNAL NAME` declarations reference the assembly.

The repair retained `clr strict security`. It did not enable database `TRUSTWORTHY` and did not weaken the assembly permission set.

## Source defect corrected

After the assembly was installed, creation of `dbo.Pmt` exposed a separate parameter-count mismatch.

The compiled CLR method accepts seven parameters:

```csharp
Pmt(
    int interestCalculationMode,
    int termInMonths,
    int paymentFrequencyPerYear,
    double APR,
    double PV,
    double FV,
    int Due)
```

The declaration in `stored procedures.txt` only contained the final six parameters. It was corrected to include `@interestCalculationMode` first:

```sql
CREATE FUNCTION [dbo].[Pmt]
(
    @interestCalculationMode [int],
    @termInMonths [int],
    @paymentFrequencyPerYear [int],
    @APR [float],
    @PV [float],
    @FV [float],
    @Due [int]
)
RETURNS [float] WITH EXECUTE AS CALLER
AS EXTERNAL NAME
    [Infrastructure.Data.SQLCLR].[UserDefinedFunctions].[Pmt];
```

## Verification

The final catalog check returned the assembly as `SAFE_ACCESS` and all five objects with CLR function types:

```sql
USE [SwiftFinancialsDB_Live];
GO

SELECT name, permission_set_desc
FROM sys.assemblies
WHERE name = N'Infrastructure.Data.SQLCLR';

SELECT name, type_desc
FROM sys.objects
WHERE name IN
(
    N'FV',
    N'IPmt',
    N'Pmt',
    N'PPmt',
    N'RepaymentSchedule'
)
ORDER BY name;
```

Observed result:

```text
Infrastructure.Data.SQLCLR  SAFE_ACCESS
FV                          CLR_SCALAR_FUNCTION
IPmt                        CLR_SCALAR_FUNCTION
Pmt                         CLR_SCALAR_FUNCTION
PPmt                        CLR_SCALAR_FUNCTION
RepaymentSchedule           CLR_TABLE_VALUED_FUNCTION
```

The functions were also invoked to confirm that SQL Server could load and execute the registered assembly.

## Framework target

The repaired DLL targets .NET Framework 4.7.2. This target loaded successfully in the current SQL Server CLR host. Retargeting to .NET Framework 4.8 was not required to resolve the missing-assembly exception.

SQL Server CLR requires .NET Framework assemblies; it does not host .NET Core or .NET 5-and-later assemblies.

## Future rebuilds and deployments

The trusted-assembly entry is based on the DLL's SHA-512 hash. Rebuilding or changing the SQLCLR project can produce a different hash. When deploying a changed DLL:

1. Calculate and trust the new DLL hash.
2. Update the registered database assembly with `ALTER ASSEMBLY`, or recreate it in a deployment that safely handles dependent functions.
3. Verify the assembly and functions in the target database.
4. Exercise at least one scalar function and the table-valued function.

Every database that uses these CLR functions must receive the SQLCLR deployment before `stored procedures.txt` attempts to create the functions.

