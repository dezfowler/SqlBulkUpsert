
USE [master]
GO

/****** Object:  Database [SqlBulkUpsertTestDb]	Script Date: 02/10/2011 22:47:12 ******/
IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'SqlBulkUpsertTestDb')
DROP DATABASE [SqlBulkUpsertTestDb]
GO

USE [master]
GO

/****** Object:  Database [SqlBulkUpsertTestDb]	Script Date: 02/10/2011 22:47:12 ******/
CREATE DATABASE [SqlBulkUpsertTestDb]
GO

USE [SqlBulkUpsertTestDb]
GO

/****** Object:  Table [dbo].[TestUpsert]	Script Date: 02/10/2011 22:48:06 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TestUpsert]') AND type in (N'U'))
DROP TABLE [dbo].[TestUpsert]
GO

USE [SqlBulkUpsertTestDb]
GO

/****** Object:  Table [dbo].[TestUpsert]	Script Date: 02/10/2011 22:48:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TestUpsert](
	[ident] int IDENTITY(1,1) NOT NULL,
	[key_part_1] nchar(4) NOT NULL,
	[key_part_2] smallint NOT NULL,
	[nullable_text] nvarchar(50) NULL,
	[nullable_number] int NULL,
	[nullable_datetimeoffset] datetimeoffset(7) NULL,
	[nullable_money] money NULL,
	[nullable_varbinary] varbinary(max) NULL,
	[nullable_image] image NULL,
	[nullable_xml] xml NULL,
	CONSTRAINT [PK_TestUpsert] PRIMARY KEY CLUSTERED 
	(
		[key_part_1] ASC,
		[key_part_2] ASC
	)
)

GO

