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
ALTER TABLE dbo.TinyLog ADD
	Signature varchar(64) NULL,
	SignatureMethod varchar(10) NULL
GO
ALTER TABLE dbo.TinyLog SET (LOCK_ESCALATION = TABLE)
GO
COMMIT