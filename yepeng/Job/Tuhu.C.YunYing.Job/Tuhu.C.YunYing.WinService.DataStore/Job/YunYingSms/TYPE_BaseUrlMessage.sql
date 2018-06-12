USE [Gungnir]
/****** Object:  UserDefinedTableType [dbo].[Short_BaseUrlMessage]    Script Date: 2016/5/16 11:34:27 ******/
CREATE TYPE [dbo].[Short_BaseUrlMessage] AS TABLE(
	[PKID] INT NOT NULL ,
	[BaseUrl] [nvarchar] (1000) NULL,
	[BaseKey] [nvarchar] (100) NULL
)
GO