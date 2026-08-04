-- =============================================================================
-- Initializes SwiftFinancialsDB_BLOBStore, the database WebApplication1's
-- "BLOBStore" connection string (Web.config) points at.
--
-- Backs MediaAppService (Application.MainBoundedContext/Services/MediaAppService.cs)
-- - GetFile/GetMedia, PostFile, PostImage - which store binary files (company
-- logos, generated statement PDFs if ever persisted, etc.) in a single table,
-- `swift_media`, keyed by a GUID "SKU" (e.g. a company's Id, used as the logo key
-- in PrintGeneralLedgerTransactionsByCustomerAccountIdAndDateRange).
--
-- The table uses SQL Server FILESTREAM for the `content` column - that's what
-- lets MediaAppService.cs stream file bytes via SqlFileStream instead of loading
-- the whole blob into a VARBINARY(MAX) in memory. This is why setup has an extra
-- step beyond a normal CREATE DATABASE: FILESTREAM must be enabled at the SQL
-- Server INSTANCE level before this script's CREATE DATABASE will work.
-- =============================================================================


-- -----------------------------------------------------------------------------
-- STEP 0 - manual, one-time, per SQL Server instance. Cannot be scripted:
--
-- 1. Open "SQL Server Configuration Manager" -> SQL Server Services ->
--    right-click your instance (e.g. "SQL Server (MSSQLSERVER)") -> Properties
--    -> FILESTREAM tab -> check both:
--      [x] Enable FILESTREAM for Transact-SQL access
--      [x] Enable FILESTREAM for file I/O streaming access
--    (Windows share name can be left as the default.) Click OK.
--
-- 2. Restart the SQL Server service (from the same Configuration Manager window,
--    or `net stop MSSQLSERVER && net start MSSQLSERVER` in an admin shell -
--    adjust the service name if your instance is named, e.g. MSSQL$SQLEXPRESS).
--
-- 3. Then run the T-SQL below (this file, from STEP 1 onward) in SSMS/sqlcmd.
-- -----------------------------------------------------------------------------


-- STEP 1 - enable FILESTREAM access at the instance level (T-SQL side of the
-- same switch as the Configuration Manager step above; both are required).
EXEC sp_configure 'filestream access level', 2;
RECONFIGURE;
GO

-- STEP 1b - verify the Configuration Manager step actually took effect before
-- going any further. FilestreamEffectiveLevel is the real gate - sp_configure
-- above only sets the T-SQL-visible value, not whether the service itself was
-- started with FILESTREAM support.
--   0 = disabled -> Configuration Manager step wasn't applied, or the service
--       wasn't restarted after applying it, or you restarted the wrong service
--       (check the exact instance name in Configuration Manager, e.g. named
--       instances run as "SQL Server (INSTANCENAME)", not "(MSSQLSERVER)").
--   3 = enabled and matches what STEP 1 requested - safe to continue.
SELECT SERVERPROPERTY('FilestreamConfiguredLevel') AS ConfiguredLevel,
       SERVERPROPERTY('FilestreamEffectiveLevel')  AS EffectiveLevel;
GO
-- Do not proceed past this point until EffectiveLevel is non-zero.


-- STEP 2 - create the database with a FILESTREAM filegroup.
-- Paths set to match this instance's data directory
-- (C:\Program Files\Microsoft SQL Server\MSSQL17.MSSQLSERVER\MSSQL\DATA).
-- The FILESTREAM directory (SwiftFinancialsDB_BLOBStore_FSData) must NOT already
-- exist - SQL Server creates it.
IF DB_ID('SwiftFinancialsDB_BLOBStore') IS NULL
BEGIN
    CREATE DATABASE SwiftFinancialsDB_BLOBStore
    ON PRIMARY
    ( NAME = SwiftFinancialsDB_BLOBStore_data,
      FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL17.MSSQLSERVER\MSSQL\DATA\SwiftFinancialsDB_BLOBStore.mdf' ),
    FILEGROUP BLOBFileGroup CONTAINS FILESTREAM
    ( NAME = SwiftFinancialsDB_BLOBStore_blobs,
      FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL17.MSSQLSERVER\MSSQL\DATA\SwiftFinancialsDB_BLOBStore_FSData' )
    LOG ON
    ( NAME = SwiftFinancialsDB_BLOBStore_log,
      FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL17.MSSQLSERVER\MSSQL\DATA\SwiftFinancialsDB_BLOBStore.ldf' );
END
GO


-- STEP 3 - create the table MediaAppService.cs actually queries.
-- Column list matches the exact SELECT/INSERT in MediaAppService.GetFile/PostFile:
--   media_sku, file_name, file_remarks, content_type, content_coding, content, created_by
-- media_sku doubles as the required FILESTREAM ROWGUIDCOL (must be unique per row -
-- the app enforces this itself via DELETE-then-INSERT on the same media_sku before
-- every write, so a single UNIQUEIDENTIFIER column can serve both purposes).
USE SwiftFinancialsDB_BLOBStore;
GO

IF OBJECT_ID('dbo.swift_media', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.swift_media
    (
        media_sku      UNIQUEIDENTIFIER NOT NULL ROWGUIDCOL UNIQUE,
        file_name      VARCHAR(256)     NOT NULL,
        file_remarks   VARCHAR(256)     NULL,
        content_type   VARCHAR(256)     NOT NULL,
        content_coding VARCHAR(256)     NULL,
        content        VARBINARY(MAX) FILESTREAM NOT NULL,
        created_by     VARCHAR(256)     NOT NULL
    ) FILESTREAM_ON BLOBFileGroup;
END
GO


-- STEP 4 - confirm the app's login can actually use this database.
-- 'sa' is a reserved principal with implicit sysadmin access to every database
-- on the instance already - it cannot be (and never needs to be) mapped with
-- CREATE USER. Nothing to do here for 'sa'. If your Web.config connection
-- string uses a different, non-sysadmin login instead, map it explicitly:
--   CREATE USER [your_login] FOR LOGIN [your_login];
--   ALTER ROLE db_owner ADD MEMBER [your_login];


-- STEP 5 - sanity check. Should return 0 rows (empty table, no error) once
-- everything above succeeded - confirming the "cannot open database" error is gone
-- and the swift_media table exists and is queryable.
SELECT * FROM dbo.swift_media;
