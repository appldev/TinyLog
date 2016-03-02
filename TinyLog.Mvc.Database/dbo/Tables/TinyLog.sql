CREATE TABLE [dbo].[TinyLog] (
    [Id]                  UNIQUEIDENTIFIER   NOT NULL,
    [CorrelationId]       UNIQUEIDENTIFIER   NULL,
    [CreatedOn]           DATETIMEOFFSET (7) NOT NULL,
    [Title]               NVARCHAR (200)     NULL,
    [Message]             NVARCHAR (MAX)     NULL,
    [Source]              NVARCHAR (50)      NULL,
    [Area]                NVARCHAR (50)      NULL,
    [Client]              NVARCHAR (200)     NULL,
    [ClientInfo]          NVARCHAR (MAX)     NULL,
    [Severity]            NVARCHAR (20)      NOT NULL,
    [CustomDataFormatter] NVARCHAR (100)     NULL,
    [CustomDataType]      NVARCHAR (100)     NULL,
    [CustomData]          NVARCHAR (MAX)     NULL,
    [Signature]           VARCHAR (64)       NULL,
    [SignatureMethod]     VARCHAR (10)       NULL,
    CONSTRAINT [PK_TinyLog] PRIMARY KEY NONCLUSTERED ([Id] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_TinyLog_CreatedOn]
    ON [dbo].[TinyLog]([CreatedOn] DESC);


GO
CREATE NONCLUSTERED INDEX [IX_TinyLog_Source]
    ON [dbo].[TinyLog]([Source] ASC);

