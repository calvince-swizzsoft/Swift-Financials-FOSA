SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF COL_LENGTH('dbo.swiftFin_Employees','EmploymentStartDate') IS NULL
    ALTER TABLE dbo.swiftFin_Employees ADD EmploymentStartDate datetime2(2) NULL;
IF COL_LENGTH('dbo.swiftFin_LeaveApplications','ChargedDates') IS NULL
    ALTER TABLE dbo.swiftFin_LeaveApplications ADD ChargedDates nvarchar(max) NULL;
IF COL_LENGTH('dbo.swiftFin_LeaveApplications','EffectiveReturnDate') IS NULL
    ALTER TABLE dbo.swiftFin_LeaveApplications ADD EffectiveReturnDate date NULL;
IF COL_LENGTH('dbo.swiftFin_LeaveApplications','NotificationPending') IS NULL
    ALTER TABLE dbo.swiftFin_LeaveApplications ADD NotificationPending bit NOT NULL CONSTRAINT DF_LeaveApplications_NotificationPending DEFAULT(0);
-- Snapshot legacy charged dates under the policy that exists at migration time.
-- Historical recalls have no return date: retain their existing full-cancellation treatment.
EXEC sp_executesql N'
IF EXISTS (SELECT 1 FROM dbo.swiftFin_LeaveApplications WHERE ChargedDates IS NULL AND DATEDIFF(day,Duration_StartDate,Duration_EndDate)>366)
    THROW 51000, ''Review legacy leave exceeding 367 calendar days before migration.'', 1;
;WITH n AS (SELECT 0 d UNION ALL SELECT d+1 FROM n WHERE d<366)
UPDATE a SET ChargedDates = COALESCE(STUFF((SELECT '',''+CONVERT(char(10),DATEADD(day,n.d,a.Duration_StartDate),23)
    FROM n WHERE n.d<=DATEDIFF(day,a.Duration_StartDate,a.Duration_EndDate)
    AND (t.ExcludeWeekends=0 OR ((DATEDIFF(day,CONVERT(date,''19000101''),DATEADD(day,n.d,a.Duration_StartDate))%7)+7)%7 < 5)
    AND (t.ExcludeHolidays=0 OR NOT EXISTS (SELECT 1 FROM dbo.swiftFin_Holidays h WHERE h.IsLocked=0 AND h.Duration_StartDate<=DATEADD(day,n.d,a.Duration_StartDate) AND h.Duration_EndDate>=DATEADD(day,n.d,a.Duration_StartDate)))
    ORDER BY n.d FOR XML PATH(''''),TYPE).value(''.'',''nvarchar(max)''),1,1,''''),'''')
FROM dbo.swiftFin_LeaveApplications a JOIN dbo.swiftFin_LeaveTypes t ON t.Id=a.LeaveTypeId
WHERE a.ChargedDates IS NULL OPTION(MAXRECURSION 367);';
COMMIT TRANSACTION;
