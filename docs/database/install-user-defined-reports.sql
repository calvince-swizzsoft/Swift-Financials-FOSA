IF OBJECT_ID('dbo.swiftFin_UserDefinedReportCategories', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.swiftFin_UserDefinedReportCategories
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_UserDefinedReportCategories PRIMARY KEY,
        Name NVARCHAR(150) NOT NULL,
        CreatedBy NVARCHAR(256) NULL,
        CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_UserDefinedReportCategories_CreatedDate DEFAULT SYSUTCDATETIME(),
        CONSTRAINT UQ_UserDefinedReportCategories_Name UNIQUE (Name)
    );
END;

IF OBJECT_ID('dbo.swiftFin_UserDefinedReports', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.swiftFin_UserDefinedReports
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_UserDefinedReports PRIMARY KEY,
        CategoryId INT NOT NULL,
        Name NVARCHAR(200) NOT NULL,
        Description NVARCHAR(1000) NULL,
        ReportPath NVARCHAR(500) NOT NULL,
        FileName NVARCHAR(260) NOT NULL,
        RdlContent VARBINARY(MAX) NOT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_UserDefinedReports_IsActive DEFAULT 1,
        CreatedBy NVARCHAR(256) NULL,
        CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_UserDefinedReports_CreatedDate DEFAULT SYSUTCDATETIME(),
        ModifiedBy NVARCHAR(256) NULL,
        ModifiedDate DATETIME2 NULL,
        CONSTRAINT FK_UserDefinedReports_Category FOREIGN KEY (CategoryId) REFERENCES dbo.swiftFin_UserDefinedReportCategories(Id),
        CONSTRAINT UQ_UserDefinedReports_Name UNIQUE (Name),
        CONSTRAINT UQ_UserDefinedReports_ReportPath UNIQUE (ReportPath)
    );
    CREATE INDEX IX_UserDefinedReports_CategoryId ON dbo.swiftFin_UserDefinedReports(CategoryId, IsActive, Name);
END;
