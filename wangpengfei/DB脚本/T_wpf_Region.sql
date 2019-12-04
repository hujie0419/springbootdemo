USE [Activity]
GO

/****** Object:  Table [dbo].[T_wpf_Region]    Script Date: 2019/11/29 19:40:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
drop table [T_wpf_Region]
CREATE TABLE [dbo].[T_wpf_Region](
	[PKID] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[SortNO] [int] NOT NULL,
	[Layer] [tinyint] NOT NULL,
	[ParentID] [int] NOT NULL,
	[ProvinceID] [int] NOT NULL,
	[ProvinceName] [nvarchar](50) NOT NULL,
	[CityID] [int] NOT NULL,
	[CityName] [nvarchar](50) NOT NULL,
	[AreaID] [int] NOT NULL,
	[AreaName] [nvarchar](50) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Status] [int] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[CreateUser] [nvarchar](20) NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
	[UpdateUser] [nvarchar](20) NOT NULL
 CONSTRAINT [PK_T_wpf_Region] PRIMARY KEY NONCLUSTERED 
(
	[PKID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[T_wpf_Region] ADD  CONSTRAINT [DF_T_wpf_Region_CreateTime]  DEFAULT (getdate()) FOR [CreateTime]
GO
ALTER TABLE [dbo].[T_wpf_Region] ADD  CONSTRAINT [DF_T_wpf_Region_UpdateTime]  DEFAULT (getdate()) FOR [UpdateTime]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'地区id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_wpf_Region', @level2type=N'COLUMN',@level2name=N'PKID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'名称' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_wpf_Region', @level2type=N'COLUMN',@level2name=N'Name'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'显示排序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_wpf_Region', @level2type=N'COLUMN',@level2name=N'SortNO'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'地区等级，省市区3级。后面扩展街道可以到4级' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_wpf_Region', @level2type=N'COLUMN',@level2name=N'Layer'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上一层ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_wpf_Region', @level2type=N'COLUMN',@level2name=N'ParentID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'省ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_wpf_Region', @level2type=N'COLUMN',@level2name=N'ProvinceID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'市ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_wpf_Region', @level2type=N'COLUMN',@level2name=N'CityID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'区ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_wpf_Region', @level2type=N'COLUMN',@level2name=N'AreaID'
GO


