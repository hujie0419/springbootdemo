USE [Activity]
GO

/****** Object:  Table [dbo].[T_wpf_UserActivityApply]    Script Date: 2019/11/29 19:40:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
drop table T_wpf_UserActivityApply
CREATE TABLE [dbo].[T_wpf_UserActivityApply](
	[PKID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[ActivityID] [int] NOT NULL,
	[ApplyTime] [datetime] NOT NULL,
	[PassTime] [datetime] NULL,
	[Remark] [nvarchar](100) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Status] [int] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[CreateUser] [nvarchar](20) NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
	[UpdateUser] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_T_wpf_UserActivityApply] PRIMARY KEY CLUSTERED 
(
	[PKID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].T_wpf_UserActivityApply ADD  CONSTRAINT [DF_T_wpf_UserActivityApply_CreateTime]  DEFAULT (getdate()) FOR [CreateTime]
GO
ALTER TABLE [dbo].T_wpf_UserActivityApply ADD  CONSTRAINT [DF_T_wpf_UserActivityApply_UpdateTime]  DEFAULT (getdate()) FOR [UpdateTime]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_wpf_UserActivityApply', @level2type=N'COLUMN',@level2name=N'PKID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�û�ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_wpf_UserActivityApply', @level2type=N'COLUMN',@level2name=N'UserID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_wpf_UserActivityApply', @level2type=N'COLUMN',@level2name=N'ActivityID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'����ʱ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_wpf_UserActivityApply', @level2type=N'COLUMN',@level2name=N'ApplyTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ͨ��ʱ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_wpf_UserActivityApply', @level2type=N'COLUMN',@level2name=N'PassTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'��ע' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_wpf_UserActivityApply', @level2type=N'COLUMN',@level2name=N'Remark'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'�߼�ɾ��' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_wpf_UserActivityApply', @level2type=N'COLUMN',@level2name=N'IsDeleted'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'״̬�������ö�ٺ����' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_wpf_UserActivityApply', @level2type=N'COLUMN',@level2name=N'Status'
GO


