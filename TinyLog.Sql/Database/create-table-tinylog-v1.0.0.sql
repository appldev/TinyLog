---
--- 	This Script contains the Core table Layout for an SQL Server based TinyLog table
---		Visit https://www.github.com/appldev/tinylog for more information and source code
---
---		RELEASE NOTES: This script is in compliance with the TinyLog Log Entry schema v1.0.x
---

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TinyLog](
	[Id] [uniqueidentifier] NOT NULL,
	[CorrelationId] [uniqueidentifier] NULL,
	[CreatedOn] [datetimeoffset](7) NOT NULL,
	[Title] [nvarchar](200) NULL,
	[Message] [nvarchar](max) NULL,
	[Source] [nvarchar](50) NULL,
	[Area] [nvarchar](50) NULL,
	[Client] [nvarchar](200) NULL,
	[ClientInfo] [nvarchar](max) NULL,
	[Severity] [nvarchar](20) NOT NULL,
	[CustomDataFormatter] [nvarchar](100) NULL,
	[CustomData] [nvarchar](max) NULL,
 CONSTRAINT [PK_TinyLog] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
---
--- Create basic indexes
---
CREATE CLUSTERED INDEX [IX_TinyLog_CreatedOn] ON [dbo].[TinyLog]
(
	[CreatedOn] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

CREATE NONCLUSTERED INDEX [IX_TinyLog_Source] ON [dbo].[TinyLog]
(
	[Source] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO



