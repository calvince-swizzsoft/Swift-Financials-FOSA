$ErrorActionPreference='Stop'
$backendRoot = (Resolve-Path (Join-Path $PSScriptRoot '../../..')).Path
$cfg=[xml](Get-Content (Join-Path $backendRoot 'WebApplication1/Web.config') -Raw)
$builder=New-Object System.Data.SqlClient.SqlConnectionStringBuilder(($cfg.configuration.connectionStrings.add | Where-Object name -eq 'SwiftFin_Dev').connectionString)
$builder['Initial Catalog']='tempdb'
$cn=New-Object System.Data.SqlClient.SqlConnection($builder.ConnectionString)
$cn.Open()
try {
 $cmd=$cn.CreateCommand()
 $cmd.CommandText=@'
CREATE TABLE #Accounts(Id uniqueidentifier,AccountType int,AccountCode int,AccountName nvarchar(256));
CREATE TABLE #Journals(Id int,BranchId uniqueidentifier,PostingPeriodId uniqueidentifier,ValueDate datetime,CreatedDate datetime);
CREATE TABLE #Entries(JournalId int,ChartOfAccountId uniqueidentifier,Amount decimal(18,2));
CREATE TABLE #Loans(LoanProductId uniqueidentifier,BranchId uniqueidentifier,DisbursedDate datetime,DisbursedAmount decimal(18,2));
CREATE TABLE #Products(Id uniqueidentifier,Description nvarchar(256));
DECLARE @b uniqueidentifier='11111111-1111-1111-1111-111111111111', @p uniqueidentifier='22222222-2222-2222-2222-222222222222', @e uniqueidentifier='33333333-3333-3333-3333-333333333333', @i uniqueidentifier='44444444-4444-4444-4444-444444444444';
INSERT #Accounts VALUES(@e,5000,5001,'Expenses'),(@i,4000,4001,'Income');
INSERT #Journals VALUES
(1,@b,@p,'20260911 23:59:59.997','20260912'),(2,@b,@p,'20260910','20260910'),
(3,@b,@p,'20260912','20260911'),(4,NEWID(),@p,'20260910','20260910'),
(5,@b,NEWID(),'20260910','20260910'),(6,@b,@p,'20260831','20260910'),
(7,@b,@p,NULL,'20260911 23:59:59.997');
INSERT #Entries VALUES(1,@e,100),(2,@e,-20),(2,@i,-200),(3,@e,1000),(4,@e,1000),(5,@e,1000),(6,@e,1000),(7,@e,5);
INSERT #Products VALUES(@i,'Loan product');
INSERT #Loans VALUES(@i,@b,'20260911 23:59:59.997',300),(@i,@b,'20260912',1000),(@i,NEWID(),'20260911',1000),(@i,@b,'20260831',1000);
'@
 [void]$cmd.ExecuteNonQuery()
 $source=Get-Content (Join-Path $backendRoot 'Application.MainBoundedContext/AccountsModule/Services/BudgetAppService.cs') -Raw
 $queries=[regex]::Matches($source,'DatabaseSqlQuery<BudgetActualLineDTO>\(@"([\s\S]*?)", header')
 if($queries.Count -ne 2){throw 'Report SQL not found'}
 foreach($n in 0..1) {
  $cmd.Parameters.Clear()
  $cmd.CommandText=$queries[$n].Groups[1].Value.Replace('dbo.swiftFin_JournalEntries','#Entries').Replace('dbo.swiftFin_Journals','#Journals').Replace('dbo.swiftFin_ChartOfAccounts','#Accounts').Replace('dbo.swiftFin_LoanCases','#Loans').Replace('dbo.swiftFin_LoanProducts','#Products')
  [void]$cmd.Parameters.AddWithValue('@Branch',[guid]'11111111-1111-1111-1111-111111111111')
  [void]$cmd.Parameters.AddWithValue('@Period',[guid]'22222222-2222-2222-2222-222222222222')
  [void]$cmd.Parameters.AddWithValue('@Start',[datetime]'2026-09-01')
  [void]$cmd.Parameters.AddWithValue('@End',[datetime]'2026-09-12')
  $table=New-Object System.Data.DataTable
  $table.Load($cmd.ExecuteReader())
  if($n -eq 0) {
   if($table.Rows.Count -ne 2 -or ($table | Where-Object Section -eq 'Expenses').Actual -ne 85 -or ($table | Where-Object Section -eq 'Income').Actual -ne 200){throw 'GL query: incorrect scope/sign/date result'}
  } elseif($table.Rows.Count -ne 1 -or $table.Rows[0].Actual -ne 300){throw 'Loan query: incorrect scope/date result'}
 }
 Write-Output 'PASS: report SQL respects branch/period/start/end, value-date fallback, final fractional second, expense refunds, income signs and loan disbursement dates. Temporary tables only.'
} finally {$cn.Dispose()}
