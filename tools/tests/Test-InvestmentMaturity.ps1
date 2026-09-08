param(
    [string]$Server = '.',
    [string]$SourcePath = (Join-Path $PSScriptRoot '../../Application.MainBoundedContext/Services/SqlCommandAppService.cs')
)
$ErrorActionPreference = 'Stop'

# Execute the production AppService query against session-local fixtures only.
# No application tables or stored procedures are modified.
$source = [IO.File]::ReadAllText((Resolve-Path -LiteralPath $SourcePath))
$match = [regex]::Match($source, 'private const string InvestmentMaturityBalanceSql = @"([\s\S]*?)";')
if (!$match.Success) { throw 'Production investment maturity query not found.' }
$query = $match.Groups[1].Value.Replace('dbo.swiftfin_JournalEntries', '#Entries').Replace('dbo.swiftfin_CustomerAccounts', '#Accounts').Replace('dbo.swiftfin_InvestmentProducts', '#Products')

$connection = New-Object System.Data.SqlClient.SqlConnection("Server=$Server;Database=tempdb;Integrated Security=True;TrustServerCertificate=True;Connect Timeout=15")
$connection.Open()
try {
    $setup = $connection.CreateCommand()
    $setup.CommandText = @'
CREATE TABLE #Products (Id uniqueidentifier, ChartOfAccountId uniqueidentifier, MaturityPeriod smallint);
CREATE TABLE #Accounts (Id uniqueidentifier, CustomerAccountType_TargetProductId uniqueidentifier, CustomerAccountType_ProductCode int);
CREATE TABLE #Entries (CustomerAccountId uniqueidentifier, ChartOfAccountId uniqueidentifier, Amount decimal(18,2), CreatedDate datetime, ValueDate datetime);
INSERT #Products VALUES ('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000002',90);
INSERT #Accounts VALUES ('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000001',3);
'@
    [void]$setup.ExecuteNonQuery()
    $command = $connection.CreateCommand()
    $command.CommandText = $query
    [void]$command.Parameters.AddWithValue('@CustomerAccountID', [guid]'00000000-0000-0000-0000-000000000003')
    [void]$command.Parameters.AddWithValue('@CustomerAccountType_TargetProductId', [guid]'00000000-0000-0000-0000-000000000001')
    [void]$command.Parameters.AddWithValue('@CustomerAccountType_ProductCode', 3)
    [void]$command.Parameters.AddWithValue('@CutoffDate', [datetime]'2026-04-01T00:00:00')
    $script:passed = 0
    function Check([string]$Name, [string]$Arrange, [decimal]$Expected) {
        if ($Arrange) {
            $setup.CommandText = $Arrange
            [void]$setup.ExecuteNonQuery()
        }
        $actual = [decimal]$command.ExecuteScalar()
        if ($actual -ne $Expected) { throw "$Name expected $Expected but received $actual" }
        # Exercise both SQL execution modes used by the shared service.
        $asyncActual = [decimal]$command.ExecuteScalarAsync().GetAwaiter().GetResult()
        if ($asyncActual -ne $Expected) { throw "$Name async expected $Expected but received $asyncActual" }
        $script:passed++
        Write-Output "PASS $Name = $actual (sync/async)"
    }
    Check 'Empty account' '' 0
    Check 'January/February contributions, 90 days; exact boundary counts' @'
INSERT #Entries VALUES
('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000002',-20000,'20260101','20260101'),
('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000002',-10000,'20260201','20260201');
'@ 20000
    Check 'Zero days includes both contributions' 'UPDATE #Products SET MaturityPeriod=0;' 30000
    Check 'Zero days includes contribution at calculation time' @'
INSERT #Entries VALUES ('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000002',-5000,'20260401','20260401');
'@ 35000
    Check 'Future credit excluded even with zero days' @'
INSERT #Entries VALUES ('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000002',-5000,'20260402','20260402');
'@ 35000
    Check 'Positive maturity restored after product edit' 'UPDATE #Products SET MaturityPeriod=90;' 20000
    $command.Parameters['@CutoffDate'].Value = [datetime]'2026-03-31T23:59:59'
    Check 'One second before maturity excludes January credit' '' 0
    $command.Parameters['@CutoffDate'].Value = [datetime]'2026-04-01T00:00:00'
    Check 'Future debit excluded' @'
INSERT #Entries VALUES ('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000002',6000,'20260402','20260402');
'@ 20000
    Check 'Recent withdrawal reduces matured balance immediately' @'
INSERT #Entries VALUES ('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000002',3000,'20260320','20260320');
'@ 17000
    Check 'Backdated value date does not bypass waiting period' @'
INSERT #Entries VALUES ('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000002',-10000,'20260320','20250101');
'@ 17000
    Check 'Other account and G/L entries excluded' @'
INSERT #Entries VALUES
('00000000-0000-0000-0000-000000000004','00000000-0000-0000-0000-000000000002',-900000,'20250101','20250101'),
('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000005',-900000,'20250101','20250101');
'@ 17000
    Check 'Debit above mature contributions cannot create positive security' @'
INSERT #Entries VALUES ('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000002',25000,'20260321','20260321');
'@ 0
    Check 'Zero days uses net balance after withdrawals' 'UPDATE #Products SET MaturityPeriod=0;' 17000
    $command.Parameters['@CustomerAccountType_TargetProductId'].Value = [guid]'00000000-0000-0000-0000-000000000099'
    Check 'Wrong product cannot return another balance' '' 0
    $command.Parameters['@CustomerAccountType_TargetProductId'].Value = [guid]'00000000-0000-0000-0000-000000000001'
    Check 'Overdrawn balance stays zero with zero maturity' @'
INSERT #Entries VALUES ('00000000-0000-0000-0000-000000000003','00000000-0000-0000-0000-000000000002',50000,'20260322','20260322');
'@ 0
    Write-Output "$script:passed investment maturity scenarios passed in both execution modes."
} finally {
    $connection.Dispose()
}
