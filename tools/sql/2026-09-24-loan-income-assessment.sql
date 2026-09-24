SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF COL_LENGTH('dbo.swiftFin_LoanProducts','RequireIncomeAssessment') IS NULL
    ALTER TABLE dbo.swiftFin_LoanProducts ADD RequireIncomeAssessment bit NULL;
IF COL_LENGTH('dbo.swiftFin_LoanCases','RequireIncomeAssessment') IS NULL
    ALTER TABLE dbo.swiftFin_LoanCases ADD RequireIncomeAssessment bit NULL;
IF COL_LENGTH('dbo.swiftFin_LoanCases','IncomeAssessmentReference') IS NULL
    ALTER TABLE dbo.swiftFin_LoanCases ADD IncomeAssessmentReference nvarchar(512) NULL;
IF COL_LENGTH('dbo.swiftFin_LoanCases','IncomeAssessmentSignature') IS NULL
    ALTER TABLE dbo.swiftFin_LoanCases ADD IncomeAssessmentSignature nvarchar(64) NULL;
COMMIT TRANSACTION;
