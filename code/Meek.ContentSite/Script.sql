CREATE DATABASE [MeekContent]
GO
USE [MeekContent]
CREATE TABLE [dbo].[MeekContent](
	[Route] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Partial] [bit] NOT NULL,
	[Data] [ntext] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Title] [nvarchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
)

GO
ALTER TABLE [dbo].[MeekContent] ADD  CONSTRAINT [PK_Content] PRIMARY KEY 
(
	[Route]
)
GO
CREATE TABLE [dbo].[MeekFile](
	[Id] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[FileName] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ContentType] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Data] [image] NULL
)

GO
ALTER TABLE [dbo].[MeekFile] ADD  CONSTRAINT [PK_MeekFile] PRIMARY KEY 
(
	[Id]
)
GO
ALTER TABLE [dbo].[MeekContent] ADD  CONSTRAINT [UQ__Content__000000000000000C] UNIQUE 
(
	[Route]
)
GO
