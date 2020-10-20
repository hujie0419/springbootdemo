USE [Tuhu_work]
GO

/****** Object:  Table [dbo].[T_LuckyCharmUser]    Script Date: 2019/2/26 17:25:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[T_LuckyCharmUser](
	[PKID] [BIGINT] IDENTITY(1,1) NOT NULL,
	[ActivityId] [BIGINT] NOT NULL,
	[UserId] [BIGINT] NULL,
	[UserName] [VARCHAR](50) NOT NULL,
	[Phone] [CHAR](11) NOT NULL,
	[AreaId] [INT] NULL,
	[AreaName] [VARCHAR](100) NOT NULL,
	[IsDelete] [BIT] NOT NULL,
	[CheckState] [INT] NOT NULL,
	[CreateTime] [DATETIME] NOT NULL,
	[UpdateTime] [DATETIME] NOT NULL,
 CONSTRAINT [PK_T_ActivityUser] PRIMARY KEY CLUSTERED 
(
	[PKID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[T_LuckyCharmUser] ADD  CONSTRAINT [DF_T_LuckyCharmUser_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO

ALTER TABLE [dbo].[T_LuckyCharmUser] ADD  CONSTRAINT [DF_T_ActivityUser_CheckState]  DEFAULT ((0)) FOR [CheckState]
GO

ALTER TABLE [dbo].[T_LuckyCharmUser] ADD  CONSTRAINT [DF_T_ActivityUser_CreateTime]  DEFAULT (GETDATE()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[T_LuckyCharmUser] ADD  CONSTRAINT [DF_T_ActivityUser_UpdateTime]  DEFAULT (GETDATE()) FOR [UpdateTime]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_LuckyCharmUser', @level2type=N'COLUMN',@level2name=N'ActivityId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�û�ID  �ǵ�½�ɲ���' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_LuckyCharmUser', @level2type=N'COLUMN',@level2name=N'UserId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�û�����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_LuckyCharmUser', @level2type=N'COLUMN',@level2name=N'UserName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�û��ֻ�' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_LuckyCharmUser', @level2type=N'COLUMN',@level2name=N'Phone'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_LuckyCharmUser', @level2type=N'COLUMN',@level2name=N'AreaId'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��������' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_LuckyCharmUser', @level2type=N'COLUMN',@level2name=N'AreaName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0:�����  1:���ͨ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_LuckyCharmUser', @level2type=N'COLUMN',@level2name=N'CheckState'
GO




CREATE TABLE [dbo].[T_LuckyCharmActivity](
	[PKID] [BIGINT] IDENTITY(1,1) NOT NULL,
	[ActivityType] [INT] NOT NULL,
	[StarTime] [DATETIME] NOT NULL,
	[EndTime] [DATETIME] NOT NULL,
	[ActivityTitle] [VARCHAR](100) NOT NULL,
	[ActivitySlug] [VARCHAR](50) NULL,
	[ActivityDes] [VARCHAR](MAX) NULL,
	[IsDelete] [BIT] NOT NULL,
	[CreateTime] [DATETIME] NOT NULL,
	[UpdateTime] [DATETIME] NOT NULL,
 CONSTRAINT [PK_T_ActivitySet] PRIMARY KEY CLUSTERED 
(
	[PKID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[T_LuckyCharmActivity] ADD  CONSTRAINT [DF_T_ActivitySet_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO

ALTER TABLE [dbo].[T_LuckyCharmActivity] ADD  CONSTRAINT [DF_T_ActivitySet_CreateTime]  DEFAULT (GETDATE()) FOR [CreateTime]
GO

ALTER TABLE [dbo].[T_LuckyCharmActivity] ADD  CONSTRAINT [DF_T_ActivitySet_UpdateTime]  DEFAULT (GETDATE()) FOR [UpdateTime]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����� 0������' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_LuckyCharmActivity', @level2type=N'COLUMN',@level2name=N'ActivityType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'���ʼʱ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_LuckyCharmActivity', @level2type=N'COLUMN',@level2name=N'StarTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�����ʱ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_LuckyCharmActivity', @level2type=N'COLUMN',@level2name=N'EndTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_LuckyCharmActivity', @level2type=N'COLUMN',@level2name=N'ActivityTitle'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��ں�' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_LuckyCharmActivity', @level2type=N'COLUMN',@level2name=N'ActivitySlug'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_LuckyCharmActivity', @level2type=N'COLUMN',@level2name=N'ActivityDes'
GO

