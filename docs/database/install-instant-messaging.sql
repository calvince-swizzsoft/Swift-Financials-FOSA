IF OBJECT_ID('dbo.swiftFin_InstantMessageConversations', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.swiftFin_InstantMessageConversations
    (
        Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_InstantMessageConversations PRIMARY KEY,
        Title NVARCHAR(200) NULL,
        IsGroup BIT NOT NULL,
        CreatedBy NVARCHAR(256) NOT NULL,
        CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_InstantMessageConversations_CreatedDate DEFAULT SYSUTCDATETIME(),
        ModifiedDate DATETIME2 NOT NULL CONSTRAINT DF_InstantMessageConversations_ModifiedDate DEFAULT SYSUTCDATETIME()
    );
END;

IF OBJECT_ID('dbo.swiftFin_InstantMessageConversationParticipants', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.swiftFin_InstantMessageConversationParticipants
    (
        ConversationId UNIQUEIDENTIFIER NOT NULL,
        UserName NVARCHAR(256) NOT NULL,
        JoinedDate DATETIME2 NOT NULL CONSTRAINT DF_InstantMessageParticipants_JoinedDate DEFAULT SYSUTCDATETIME(),
        LastReadDate DATETIME2 NULL,
        CONSTRAINT PK_InstantMessageParticipants PRIMARY KEY (ConversationId, UserName),
        CONSTRAINT FK_InstantMessageParticipants_Conversation FOREIGN KEY (ConversationId) REFERENCES dbo.swiftFin_InstantMessageConversations(Id) ON DELETE CASCADE
    );
    CREATE INDEX IX_InstantMessageParticipants_User ON dbo.swiftFin_InstantMessageConversationParticipants(UserName, ConversationId);
END;

IF OBJECT_ID('dbo.swiftFin_InstantMessages', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.swiftFin_InstantMessages
    (
        Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_InstantMessages PRIMARY KEY,
        ConversationId UNIQUEIDENTIFIER NOT NULL,
        SenderUserName NVARCHAR(256) NOT NULL,
        Body NVARCHAR(4000) NOT NULL,
        CreatedDate DATETIME2 NOT NULL CONSTRAINT DF_InstantMessages_CreatedDate DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_InstantMessages_Conversation FOREIGN KEY (ConversationId) REFERENCES dbo.swiftFin_InstantMessageConversations(Id) ON DELETE CASCADE
    );
    CREATE INDEX IX_InstantMessages_Conversation ON dbo.swiftFin_InstantMessages(ConversationId, Id);
END;
