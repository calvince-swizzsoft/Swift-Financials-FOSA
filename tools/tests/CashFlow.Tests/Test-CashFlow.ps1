param([string]$Server = '(localdb)\MSSQLLocalDB')
$ErrorActionPreference = 'Stop'
# Temporary tables only: no application database, schema, mapping or ledger is modified.
$backendRoot = (Resolve-Path (Join-Path $PSScriptRoot '../../..')).Path
$source = Get-Content (Join-Path $backendRoot 'Application.MainBoundedContext/AccountsModule/Services/CashFlowSql.cs') -Raw
$query = $source.Substring($source.IndexOf('@"') + 2)
$query = $query.Substring(0, $query.LastIndexOf('";')).Replace('""','"')
$query = $query.Replace('dbo.swiftFin_ChartOfAccounts','#Accounts').Replace('dbo.swiftFin_JournalEntries','#Entries').Replace('dbo.swiftFin_Journals','#SourceJournals').Replace('dbo.swiftFin_CashFlowMappings','#Mappings')
$prepare = $query
$serviceSource = Get-Content (Join-Path $backendRoot 'Application.MainBoundedContext/AccountsModule/Services/CashFlowAppService.cs') -Raw
$detailQuery = [regex]::Match($serviceSource,'SELECT d.JournalId[\s\S]*?ONLY;').Value.Replace('dbo.swiftFin_Journals','#SourceJournals')
$query += @'
SELECT Section,Line,SUM(Receipts) Receipts,SUM(Payments) Payments,COUNT(*) DetailCount FROM #details GROUP BY Section,Line
UNION ALL SELECT 'Balance','Opening',OpeningCash,CAST(0 AS DECIMAL(18,2)),0 FROM #balances
UNION ALL SELECT 'Balance','Closing',ClosingCash,CAST(0 AS DECIMAL(18,2)),0 FROM #balances;
'@
$branchA = [guid]::NewGuid()
$branchB = [guid]::NewGuid()
$accounts = @{}
1..7 | ForEach-Object { $accounts[$_] = [guid]::NewGuid() }
$checks = 0
function Assert-Equal($actual, $expected, $label) {
    if ($actual -ne $expected) { throw "$label expected $expected; got $actual" }
    $script:checks++
}
function Run-Case([string]$name, [object[]]$journals, [scriptblock]$check, $branch = $null) {
    $connection = New-Object System.Data.SqlClient.SqlConnection "Data Source=$Server;Initial Catalog=tempdb;Integrated Security=True;Connect Timeout=30;TrustServerCertificate=True"
    try {
        $connection.Open()
        $command = $connection.CreateCommand()
        $command.CommandText = @'
CREATE TABLE #Accounts(Id UNIQUEIDENTIFIER,AccountCode INT,AccountName NVARCHAR(100));
CREATE TABLE #SourceJournals(Id UNIQUEIDENTIFIER,BranchId UNIQUEIDENTIFIER,Reference NVARCHAR(100),PrimaryDescription NVARCHAR(100),IsLocked BIT);
CREATE TABLE #Entries(JournalId UNIQUEIDENTIFIER,ChartOfAccountId UNIQUEIDENTIFIER,Amount DECIMAL(18,2),ValueDate DATETIME NULL,CreatedDate DATETIME);
CREATE TABLE #Mappings(ChartOfAccountId UNIQUEIDENTIFIER,Section VARCHAR(16),Line NVARCHAR(120));
'@
        [void]$command.ExecuteNonQuery()
        $mapping = @{ 1=@('Cash','Bank'); 2=@('Cash','Teller'); 3=@('Operating','Commissions'); 4=@('Operating','Expenses'); 6=@('Operating','Commissions'); 7=@('Exchange','Exchange effects') }
        foreach ($i in 1..7) {
            $command.CommandText = "INSERT #Accounts VALUES (@Id,@Code,'Fixture account');"
            $command.Parameters.Clear()
            [void]$command.Parameters.AddWithValue('@Id',$accounts[$i]); [void]$command.Parameters.AddWithValue('@Code',$i)
            if ($mapping.ContainsKey($i)) {
                $command.CommandText += ' INSERT #Mappings VALUES (@Id,@Section,@Line);'
                [void]$command.Parameters.AddWithValue('@Section',$mapping[$i][0]); [void]$command.Parameters.AddWithValue('@Line',$mapping[$i][1])
            }
            [void]$command.ExecuteNonQuery()
        }
        foreach ($journal in $journals) {
            $id=[guid]::NewGuid()
            $command.CommandText="INSERT #SourceJournals VALUES(@Id,@Branch,'REF','Fixture',@Locked)"
            $command.Parameters.Clear()
            [void]$command.Parameters.AddWithValue('@Id',$id)
            [void]$command.Parameters.AddWithValue('@Branch',$(if ($journal.Branch) { $journal.Branch } else { $branchA }))
            [void]$command.Parameters.AddWithValue('@Locked',[bool]$journal.Locked)
            [void]$command.ExecuteNonQuery()
            foreach ($leg in $journal.Legs) {
                $command.CommandText='INSERT #Entries VALUES(@Id,@Account,@Amount,@Date,@Created)'
                $command.Parameters.Clear()
                [void]$command.Parameters.AddWithValue('@Id',$id); [void]$command.Parameters.AddWithValue('@Account',$accounts[[int]$leg[0]])
                [void]$command.Parameters.AddWithValue('@Amount',[decimal]$leg[1])
                $date = if ($leg.Count -gt 2) { $leg[2] } elseif ($journal.Date) { $journal.Date } else { '2026-09-11T12:00:00' }
                [void]$command.Parameters.AddWithValue('@Date',$(if ($date -eq 'null') { [DBNull]::Value } else { [datetime]$date }))
                [void]$command.Parameters.AddWithValue('@Created',[datetime]'2026-09-11T12:00:00')
                [void]$command.ExecuteNonQuery()
            }
        }
        $command.CommandText=$query; $command.Parameters.Clear()
        [void]$command.Parameters.AddWithValue('@Start',[datetime]'2026-09-11'); [void]$command.Parameters.AddWithValue('@End',[datetime]'2026-09-12')
        $parameter = $command.Parameters.Add('@Branch',[System.Data.SqlDbType]::UniqueIdentifier)
        $parameter.Value=if ($branch) { $branch } else { [DBNull]::Value }
        $table = New-Object System.Data.DataTable
        $reader=$command.ExecuteReader(); $table.Load($reader); $reader.Close()
        $rows=@($table.Rows)
        & $check $rows
        $opening=($rows | Where-Object Line -eq 'Opening').Receipts
        $closing=($rows | Where-Object Line -eq 'Closing').Receipts
        $net=[decimal]0
        $rows | Where-Object Section -ne 'Balance' | ForEach-Object { $net += $_.Receipts-$_.Payments }
        Assert-Equal ($closing-$opening-$net) 0 "$name reconciles"
        if ($name -eq 'Paged drill-down') {
            # Parameters use sp_executesql, so generated temporary tables end with that command.
            $command.CommandText=$prepare+$detailQuery
            [void]$command.Parameters.AddWithValue('@Section','Operating'); [void]$command.Parameters.AddWithValue('@Line','Commissions')
            [void]$command.Parameters.AddWithValue('@Skip',20); [void]$command.Parameters.AddWithValue('@Take',20)
            $details=New-Object System.Data.DataTable
            $reader=$command.ExecuteReader(); $details.Load($reader); $reader.Close()
            Assert-Equal $details.Rows.Count 5 'Second page size'
            Assert-Equal $details.Rows[0].TotalCount 25 'Full count'
            Assert-Equal @($details.Rows | Select-Object -ExpandProperty JournalId -Unique).Count 5 'Distinct journal rows'
        }
        Write-Output "PASS $name"
    } finally { $connection.Dispose() }
}
Run-Case 'Opening balance and income' @(@{ Date='2026-09-10'; Legs=@(@(1,1000),@(5,-1000)) },@{ Legs=@(@(1,100),@(3,-100)) }) { param($r) Assert-Equal ($r | Where-Object Line -eq 'Opening').Receipts 1000 'Opening'; Assert-Equal ($r | Where-Object Section -eq 'Operating').Receipts 100 'Receipt' }
Run-Case 'Expense payment' @(@{ Legs=@(@(1,-40),@(4,40)) }) { param($r) Assert-Equal ($r | Where-Object Section -eq 'Operating').Payments 40 'Payment' }
Run-Case 'Internal cash transfer' @(@{ Legs=@(@(1,-200),@(2,200)) }) { param($r) Assert-Equal ($r | Where-Object Section -eq 'Internal').Receipts 200 'Internal receipts'; Assert-Equal @($r | Where-Object Section -eq 'Operating').Count 0 'No operating flow' }
Run-Case 'Noncash journal' @(@{ Legs=@(@(3,-90),@(4,90)) }) { param($r) Assert-Equal @($r | Where-Object Section -ne 'Balance').Count 0 'No cash flow' }
Run-Case 'Mixed journal' @(@{ Legs=@(@(1,100),@(2,-50),@(3,-100),@(4,50)) }) { param($r) Assert-Equal ($r | Where-Object Section -eq 'Review').Receipts 100 'Review receipts'; Assert-Equal ($r | Where-Object Section -eq 'Review').Payments 50 'Review payments' }
Run-Case 'Split counterpart mapping' @(@{ Legs=@(@(1,100),@(3,-70),@(6,-30)) }) { param($r) Assert-Equal ($r | Where-Object Section -eq 'Operating').Receipts 100 'Actual allocation'; Assert-Equal ($r | Where-Object Section -eq 'Operating').DetailCount 1 'One journal per line' }
Run-Case 'Unmapped counterpart' @(@{ Legs=@(@(1,25),@(5,-25)) }) { param($r) Assert-Equal ($r | Where-Object Section -eq 'Review').Receipts 25 'Unmapped cash' }
Run-Case 'Reversed original remains included' @(@{ Locked=$true; Legs=@(@(1,100),@(3,-100)) },@{ Legs=@(@(1,-100),@(3,100)) }) { param($r) Assert-Equal ($r | Where-Object Section -eq 'Operating').Receipts 100 'Original'; Assert-Equal ($r | Where-Object Section -eq 'Operating').Payments 100 'Reversal' }
Run-Case 'Cross-period journal' @(@{ Legs=@(@(1,100),@(3,-100,'2026-09-12')) }) { param($r) Assert-Equal ($r | Where-Object Section -eq 'Review').Receipts 100 'Date mismatch needs review' }
Run-Case 'Inclusive end date' @(@{ Date='2026-09-11T23:59:59'; Legs=@(@(1,12),@(3,-12)) },@{ Date='2026-09-12'; Legs=@(@(1,13),@(3,-13)) }) { param($r) Assert-Equal ($r | Where-Object Section -eq 'Operating').Receipts 12 'End boundary' }
Run-Case 'Creation-date fallback' @(@{ Date='null'; Legs=@(@(1,9),@(3,-9)) }) { param($r) Assert-Equal ($r | Where-Object Section -eq 'Operating').Receipts 9 'Fallback' }
Run-Case 'Branch scope' @(@{ Branch=$branchA; Legs=@(@(1,10),@(3,-10)) },@{ Branch=$branchB; Legs=@(@(1,20),@(3,-20)) }) { param($r) Assert-Equal ($r | Where-Object Section -eq 'Operating').Receipts 10 'Branch only' } $branchA
Run-Case 'Zero-net review stays visible' @(@{ Legs=@(@(1,100),@(2,-100),@(3,-50),@(4,50)) }) { param($r) Assert-Equal @($r | Where-Object Section -eq 'Review').Count 1 'Review not dropped'; Assert-Equal ($r | Where-Object Section -eq 'Review').Payments 100 'Gross review' }
Run-Case 'Exchange effects separated' @(@{ Legs=@(@(1,5),@(7,-5)) }) { param($r) Assert-Equal ($r | Where-Object Section -eq 'Exchange').Receipts 5 'Exchange effects' }
Run-Case 'Paged drill-down' @(1..25 | ForEach-Object { @{ Legs=@(@(1,10),@(3,-10)) } }) { param($r) Assert-Equal ($r | Where-Object Section -eq 'Operating').DetailCount 25 'All journals counted' }
Write-Output "$checks assertions passed; 15 SQL scenarios; temporary fixtures only."

