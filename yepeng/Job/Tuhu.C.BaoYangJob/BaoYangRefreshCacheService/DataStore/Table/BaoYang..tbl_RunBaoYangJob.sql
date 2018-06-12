USE [BaoYang]
GO

/****** Object:  Table [dbo].[tbl_RunBaoYangJob]    Script Date: 2016/5/20 14:56:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_RunBaoYangJob](
	[PKID] [int] IDENTITY(1,1) NOT NULL,
	[JobName] [nvarchar](100) NOT NULL,
	[CreatedTime] [datetime] NOT NULL,
	[UpdatedTime] [datetime] NULL,
	[RunStatus] [nvarchar](20) NULL,
 CONSTRAINT [PK_tbl_RunBaoYangJob] PRIMARY KEY CLUSTERED 
(
	[PKID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO