USE [Activity]
GO

/****** Object:  Table [dbo].[T_UserRegistrations]    Script Date: 2018/10/8 15:26:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[T_UserRegistrations](
	[PKID] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Phone] [nvarchar](20) NOT NULL,
	[Area] [nvarchar](100) NOT NULL,
	[ActivityId] [bigint] NOT NULL,
	[Status] [int] NOT NULL,
	[CreateDateTime] [datetime] NOT NULL,
	[LastUpdateDateTime] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PKID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[T_UserRegistrations] ADD  DEFAULT (getdate()) FOR [LastUpdateDateTime]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'主键' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_UserRegistrations', @level2type=N'COLUMN',@level2name=N'PKID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'用户姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_UserRegistrations', @level2type=N'COLUMN',@level2name=N'Name'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'手机号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_UserRegistrations', @level2type=N'COLUMN',@level2name=N'Phone'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'所在区域' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_UserRegistrations', @level2type=N'COLUMN',@level2name=N'Area'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'活动Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_UserRegistrations', @level2type=N'COLUMN',@level2name=N'ActivityId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'报名状态' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_UserRegistrations', @level2type=N'COLUMN',@level2name=N'Status'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_UserRegistrations', @level2type=N'COLUMN',@level2name=N'CreateDateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后更新时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_UserRegistrations', @level2type=N'COLUMN',@level2name=N'LastUpdateDateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否已删除' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_UserRegistrations', @level2type=N'COLUMN',@level2name=N'IsDeleted'
GO

/****** Object:  Table [dbo].[T_UserActivities]    Script Date: 2018/10/8 15:36:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[T_UserActivities](
	[PKID] [bigint] IDENTITY(1,1) NOT NULL,
	[ActivityName] [nvarchar](100) NOT NULL,
	[CreateDateTime] [datetime] NOT NULL,
	[LastUpdateDateTime] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PKID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'主键' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_UserActivities', @level2type=N'COLUMN',@level2name=N'PKID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'活动名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_UserActivities', @level2type=N'COLUMN',@level2name=N'ActivityName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_UserActivities', @level2type=N'COLUMN',@level2name=N'CreateDateTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后更新时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_UserActivities', @level2type=N'COLUMN',@level2name=N'LastUpdateDateTime'
GO
