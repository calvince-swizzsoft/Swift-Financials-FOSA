namespace Application.MainBoundedContext.AccountsModule.Services
{
    internal static class CashFlowSql
    {
        internal const string Prepare = @"SET NOCOUNT ON;
SELECT ChartOfAccountId, Section, Line INTO #mapping FROM dbo.swiftFin_CashFlowMappings;
SELECT
 COALESCE(SUM(CASE WHEN COALESCE(e.ValueDate,e.CreatedDate)<@Start THEN e.Amount ELSE 0 END),0) OpeningCash,
 COALESCE(SUM(e.Amount),0) ClosingCash
INTO #balances
FROM dbo.swiftFin_JournalEntries e
JOIN dbo.swiftFin_Journals j ON j.Id=e.JournalId
JOIN #mapping m ON m.ChartOfAccountId=e.ChartOfAccountId AND m.Section='Cash'
WHERE COALESCE(e.ValueDate,e.CreatedDate)<@End AND (@Branch IS NULL OR j.BranchId=@Branch);

SELECT e.JournalId, e.Amount, COALESCE(e.ValueDate,e.CreatedDate) ValueDate
INTO #cash
FROM dbo.swiftFin_JournalEntries e
JOIN dbo.swiftFin_Journals j ON j.Id=e.JournalId
JOIN #mapping m ON m.ChartOfAccountId=e.ChartOfAccountId AND m.Section='Cash'
WHERE COALESCE(e.ValueDate,e.CreatedDate)>=@Start AND COALESCE(e.ValueDate,e.CreatedDate)<@End
 AND (@Branch IS NULL OR j.BranchId=@Branch) AND e.Amount<>0;
CREATE INDEX IX_cash_journal ON #cash(JournalId);

-- Read actual journal legs; contra fields and transaction codes cannot establish a cash-flow allocation.
SELECT e.JournalId, e.ChartOfAccountId, e.Amount, COALESCE(e.ValueDate,e.CreatedDate) ValueDate,
 m.Section, m.Line, CASE WHEN m.Section='Cash' THEN 1 ELSE 0 END IsCash
INTO #legs
FROM dbo.swiftFin_JournalEntries e
LEFT JOIN #mapping m ON m.ChartOfAccountId=e.ChartOfAccountId
WHERE e.Amount<>0 AND EXISTS (SELECT 1 FROM #cash c WHERE c.JournalId=e.JournalId);
CREATE INDEX IX_legs_journal ON #legs(JournalId);

SELECT JournalId,
 CASE WHEN SUM(Amount)=0 AND MIN(ValueDate)>=@Start AND MAX(ValueDate)<@End THEN
   CASE WHEN MIN(IsCash)=1 THEN 'Internal'
   WHEN (SUM(CASE WHEN IsCash=1 AND Amount<0 THEN 1 ELSE 0 END)=0
       AND SUM(CASE WHEN IsCash=0 AND Amount>0 THEN 1 ELSE 0 END)=0)
     OR (SUM(CASE WHEN IsCash=1 AND Amount>0 THEN 1 ELSE 0 END)=0
       AND SUM(CASE WHEN IsCash=0 AND Amount<0 THEN 1 ELSE 0 END)=0)
   THEN 'Classified' ELSE 'Review' END
 ELSE 'Review' END Treatment
INTO #journals FROM #legs GROUP BY JournalId;

SELECT l.JournalId, MAX(l.ValueDate) ValueDate,
 CAST(COALESCE(l.Section,'Review') AS VARCHAR(16)) Section,
 CAST(COALESCE(l.Line,N'Unmapped: '+CONVERT(NVARCHAR(20),a.AccountCode)+N' '+a.AccountName) AS NVARCHAR(240)) Line,
 SUM(CASE WHEN l.Amount<0 THEN -l.Amount ELSE 0 END) Receipts,
 SUM(CASE WHEN l.Amount>0 THEN l.Amount ELSE 0 END) Payments
INTO #flows
FROM #legs l JOIN #journals j ON j.JournalId=l.JournalId
JOIN dbo.swiftFin_ChartOfAccounts a ON a.Id=l.ChartOfAccountId
WHERE j.Treatment='Classified' AND l.IsCash=0
GROUP BY l.JournalId,l.Section,l.Line,a.AccountCode,a.AccountName
UNION ALL
SELECT c.JournalId,MAX(c.ValueDate),
 CASE WHEN j.Treatment='Internal' THEN 'Internal' ELSE 'Review' END,
 CASE WHEN j.Treatment='Internal' THEN N'Transfers within cash and cash equivalents' ELSE N'Mixed, unbalanced or cross-period journal' END,
 SUM(CASE WHEN c.Amount>0 THEN c.Amount ELSE 0 END),
 SUM(CASE WHEN c.Amount<0 THEN -c.Amount ELSE 0 END)
FROM #cash c JOIN #journals j ON j.JournalId=c.JournalId
WHERE j.Treatment<>'Classified' GROUP BY c.JournalId,j.Treatment;
-- A journal may contain several accounts mapped to the same report line.
SELECT JournalId,MAX(ValueDate) ValueDate,Section,Line,SUM(Receipts) Receipts,SUM(Payments) Payments
INTO #details FROM #flows GROUP BY JournalId,Section,Line;
";
    }
}
