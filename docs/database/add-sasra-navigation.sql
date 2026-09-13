-- Targeted navigation data migration; no schema changes or accounting data changes.
-- Canonical definition: NavigationMenu.cs, Code 26016 under Utilities (26011).
-- Mirrors Utility's existing Administrator grant convention without running its
-- unrelated database migrations, identity bootstrap or complete navigation seed.
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;
    DECLARE @Parent uniqueidentifier, @Item uniqueidentifier;
    SELECT @Parent=Id FROM dbo.swiftFin_NavigationItems WITH (UPDLOCK,HOLDLOCK)
      WHERE Code=26011 AND IsArea=1 AND AreaCode=26002;
    IF @Parent IS NULL THROW 51000, 'Command Hub Utilities (26011) must be seeded before SASRA Reports.', 1;
    IF EXISTS(SELECT 1 FROM dbo.swiftFin_NavigationItems WITH (UPDLOCK,HOLDLOCK)
        WHERE Code=26016 AND (ISNULL(Description,'')<>'SASRA Reports' OR AreaCode<>26011
          OR ISNULL(ControllerName,'')<>'SasraSetup' OR IsArea<>0 OR ParentId IS NULL OR ParentId<>@Parent))
        THROW 51001, 'Navigation code 26016 is already assigned to a different module. No changes applied.', 1;
    SELECT @Item=Id FROM dbo.swiftFin_NavigationItems WITH (UPDLOCK,HOLDLOCK) WHERE Code=26016;
    IF @Item IS NULL
    BEGIN
        SET @Item=NEWID();
        INSERT dbo.swiftFin_NavigationItems
          (Id,SequentialId,ParentId,Description,Icon,ControllerName,ActionName,Code,IsArea,AreaCode,AreaName,CreatedBy,CreatedDate)
        VALUES(@Item,NEWID(),@Parent,'SASRA Reports','fa fa-table','SasraSetup','Index',26016,0,26011,'Reports','SASRA navigation migration',SYSDATETIME());
    END;
    -- Grant only the existing all-modules Administrator role, never other roles.
    IF EXISTS(SELECT 1 FROM dbo.swiftFin_NavigationItemsInRoles WHERE RoleName='Administrator')
      AND NOT EXISTS(SELECT 1 FROM dbo.swiftFin_NavigationItemsInRoles WITH (UPDLOCK,HOLDLOCK)
                     WHERE RoleName='Administrator' AND NavigationItemId=@Item)
        INSERT dbo.swiftFin_NavigationItemsInRoles(Id,SequentialId,NavigationItemId,RoleName,CreatedBy,CreatedDate)
        VALUES(NEWID(),NEWID(),@Item,'Administrator','SASRA navigation migration',SYSDATETIME());
    COMMIT;
    SELECT n.Code,n.Description,n.AreaCode,p.Description AS Parent,
      (SELECT COUNT(*) FROM dbo.swiftFin_NavigationItemsInRoles r WHERE r.NavigationItemId=n.Id AND r.RoleName='Administrator') AS AdministratorGrants
    FROM dbo.swiftFin_NavigationItems n JOIN dbo.swiftFin_NavigationItems p ON p.Id=n.ParentId WHERE n.Code=26016;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT>0 ROLLBACK;
    THROW;
END CATCH;
