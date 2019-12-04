USE [Activity]
GO

/****** Object:  Table [dbo].[T_wpf_User]    Script Date: 2019/11/29 19:40:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
drop table [T_wpf_User]
CREATE TABLE [dbo].[T_wpf_User](
	[PKID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](30) NOT NULL,
	[PassWord] [nvarchar](50) NOT NULL,
	[RealName] [nvarchar](20) NOT NULL,
	[Phone] [varchar](20) NOT NULL,
	[ProvinceID] [int] NOT NULL,
	[CityID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[Address] [nvarchar](100) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Status] [int] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[CreateUser] [nvarchar](20) NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
	[UpdateUser] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_T_wpf_User] PRIMARY KEY NONCLUSTERED 
(
	[PKID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].T_wpf_User ADD  CONSTRAINT [DF_T_wpf_User_CreateTime]  DEFAULT (getdate()) FOR [CreateTime]
GO
ALTER TABLE [dbo].T_wpf_User ADD  CONSTRAINT [DF_T_wpf_User_UpdateTime]  DEFAULT (getdate()) FOR [UpdateTime]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_wpf_User', @level2type=N'COLUMN',@level2name=N'PKID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_wpf_User', @level2type=N'COLUMN',@level2name=N'UserName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'真实姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_wpf_User', @level2type=N'COLUMN',@level2name=N'RealName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'手机' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_wpf_User', @level2type=N'COLUMN',@level2name=N'Phone'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'省ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_wpf_User', @level2type=N'COLUMN',@level2name=N'ProvinceID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'市ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_wpf_User', @level2type=N'COLUMN',@level2name=N'CityID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'区ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_wpf_User', @level2type=N'COLUMN',@level2name=N'AreaID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'逻辑删除' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_wpf_User', @level2type=N'COLUMN',@level2name=N'IsDeleted'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户表' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_wpf_User'
GO


