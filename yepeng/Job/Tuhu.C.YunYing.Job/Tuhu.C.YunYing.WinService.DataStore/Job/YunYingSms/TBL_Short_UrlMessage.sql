USE Gungnir;
GO
--DROP table Short_UrlMessage
GO
CREATE TABLE [dbo].[Short_UrlMessage]
    (
      [PKID] [BIGINT] PRIMARY KEY NOT NULL IDENTITY(1, 1) ,
      [ShortUrl] [NVARCHAR](100) NULL ,
      [ShortKey] [NVARCHAR](100) NOT NULL ,
      [BaseSource] [NVARCHAR](100) NOT NULL DEFAULT N'Auto' ,
      [BaseUrl] [NVARCHAR](1000) NULL ,
      [BaseKey] [NVARCHAR](300) NOT NULL ,
      [CreateTime] [DATETIME] NOT NULL DEFAULT ( GETDATE() )
    )
ON  [PRIMARY];
GO
