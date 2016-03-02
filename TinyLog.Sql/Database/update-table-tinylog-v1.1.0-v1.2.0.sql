---
--- 	This Script contains the Core table Layout for an SQL Server based TinyLog table
---		Visit https://www.github.com/appldev/tinylog for more information and source code
---
---		RELEASE NOTES:	This script is in compliance with the TinyLog Log Entry schema v1.2.x
---		INSTALLATION:	Run all previous version scripts, before running this script.
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
CREATE TABLE dbo.Tmp_TinyLog
	(
	Id uniqueidentifier NOT NULL,
	CorrelationId uniqueidentifier NULL,
	CreatedOn datetimeoffset(7) NOT NULL,
	Title nvarchar(200) NULL,
	Message nvarchar(MAX) NULL,
	Source nvarchar(50) NULL,
	Area nvarchar(50) NULL,
	Client nvarchar(200) NULL,
	ClientInfo nvarchar(MAX) NULL,
	Severity nvarchar(20) NOT NULL,
	CustomDataFormatter nvarchar(100) NULL,
	CustomDataType nvarchar(100) NULL,
	CustomData nvarchar(MAX) NULL,
	Signature varchar(64) NULL,
	SignatureMethod varchar(10) NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_TinyLog SET (LOCK_ESCALATION = TABLE)
GO
IF EXISTS(SELECT * FROM dbo.TinyLog)
	 EXEC('INSERT INTO dbo.Tmp_TinyLog (Id, CorrelationId, CreatedOn, Title, Message, Source, Area, Client, ClientInfo, Severity, CustomDataFormatter, CustomData, Signature, SignatureMethod)
		SELECT Id, CorrelationId, CreatedOn, Title, Message, Source, Area, Client, ClientInfo, Severity, CustomDataFormatter, CustomData, Signature, SignatureMethod FROM dbo.TinyLog WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.TinyLog
GO
EXECUTE sp_rename N'dbo.Tmp_TinyLog', N'TinyLog', 'OBJECT' 
GO
ALTER TABLE dbo.TinyLog ADD CONSTRAINT
	PK_TinyLog PRIMARY KEY NONCLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE CLUSTERED INDEX IX_TinyLog_CreatedOn ON dbo.TinyLog
	(
	CreatedOn DESC
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_TinyLog_Source ON dbo.TinyLog
	(
	Source
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
COMMIT
