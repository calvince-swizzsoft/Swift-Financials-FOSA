-- Budget G/L balance: branch + account + posting period, through the full end date.
-- Run in the application's financial database. Safe to run again.
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
IF OBJECT_ID(N'dbo.sp_GetGlAccountBalanceByBranchAndPostingPeriod', N'P') IS NULL
    EXEC(N'CREATE PROCEDURE dbo.sp_GetGlAccountBalanceByBranchAndPostingPeriod AS BEGIN SET NOCOUNT ON; END');
GO
ALTER PROCEDURE dbo.sp_GetGlAccountBalanceByBranchAndPostingPeriod
    @BranchID uniqueidentifier,
    @ChartOfAccountID uniqueidentifier,
    @PostingPeriodId uniqueidentifier,
    @EndDate datetime,
    @TransactionDateFilter int = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF @EndDate IS NULL OR CONVERT(date, @EndDate) = '99991231'
        THROW 50001, 'Supply a valid end date before 9999-12-31.', 1;
    IF @TransactionDateFilter IS NULL OR @TransactionDateFilter NOT IN (1, 2)
        THROW 50002, 'TransactionDateFilter must be 1 (ValueDate) or 2 (CreatedDate).', 1;

    -- An exclusive next-day boundary includes fractional seconds after 23:59:59.
    DECLARE @EndExclusive datetime = DATEADD(day, 1, CONVERT(datetime, CONVERT(date, @EndDate)));

    SELECT ISNULL(SUM(e.Amount), 0) AS Balance
    FROM dbo.swiftFin_JournalEntries AS e
    INNER JOIN dbo.swiftFin_Journals AS j ON j.Id = e.JournalId
    WHERE e.ChartOfAccountId = @ChartOfAccountID
      AND j.BranchId = @BranchID
      AND j.PostingPeriodId = @PostingPeriodId
      AND ((@TransactionDateFilter = 1 AND j.ValueDate < @EndExclusive)
        OR (@TransactionDateFilter = 2 AND j.CreatedDate < @EndExclusive));
END;
GO
