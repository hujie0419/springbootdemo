USE [Activity]
GO
/****** Object:  Table [dbo].[T_Activity]    Script Date: 2018/7/9 13:07:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[T_Activity](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ActivityName] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[T_EnrollInfo]    Script Date: 2018/7/9 13:07:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[T_EnrollInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](50) NULL,
	[Tel] [nvarchar](50) NULL,
	[Area] [nvarchar](max) NULL,
	[EnrollStatus] [int] NULL,
	[CreatTime] [datetime] NULL,
	[UpdateTime] [datetime] NULL,
	[ActivityId] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[T_Activity] ON 

INSERT [dbo].[T_Activity] ([Id], [ActivityName]) VALUES (1, N'保养活动')
INSERT [dbo].[T_Activity] ([Id], [ActivityName]) VALUES (2, N'轮胎美容')
SET IDENTITY_INSERT [dbo].[T_Activity] OFF
SET IDENTITY_INSERT [dbo].[T_EnrollInfo] ON 

INSERT [dbo].[T_EnrollInfo] ([Id], [UserName], [Tel], [Area], [EnrollStatus], [CreatTime], [UpdateTime], [ActivityId]) VALUES (1, N'小明', N'12222490098', N'上海市嘉定区', 0, CAST(N'2018-07-02T16:29:39.403' AS DateTime), CAST(N'2018-07-05T19:57:01.090' AS DateTime), NULL)
INSERT [dbo].[T_EnrollInfo] ([Id], [UserName], [Tel], [Area], [EnrollStatus], [CreatTime], [UpdateTime], [ActivityId]) VALUES (2, N'小明', N'13427349009', N'上海市闵行区', 1, CAST(N'2018-07-02T16:30:41.850' AS DateTime), CAST(N'2018-07-06T17:35:00.063' AS DateTime), 1)
INSERT [dbo].[T_EnrollInfo] ([Id], [UserName], [Tel], [Area], [EnrollStatus], [CreatTime], [UpdateTime], [ActivityId]) VALUES (3, N'史塔克', N'12427349608', N'上海市闵行区', 1, CAST(N'2018-07-02T16:32:35.560' AS DateTime), CAST(N'2018-07-06T17:35:00.063' AS DateTime), 1)
INSERT [dbo].[T_EnrollInfo] ([Id], [UserName], [Tel], [Area], [EnrollStatus], [CreatTime], [UpdateTime], [ActivityId]) VALUES (4, N'二丫', N'14427349008', N'上海市闵行区', 1, CAST(N'2018-07-02T16:35:12.030' AS DateTime), CAST(N'2018-07-06T17:35:00.063' AS DateTime), 1)
INSERT [dbo].[T_EnrollInfo] ([Id], [UserName], [Tel], [Area], [EnrollStatus], [CreatTime], [UpdateTime], [ActivityId]) VALUES (5, N'三傻', N'16727349008', N'上海市闵行区', 1, CAST(N'2018-07-02T16:36:26.370' AS DateTime), CAST(N'2018-07-06T17:35:00.063' AS DateTime), 12)
INSERT [dbo].[T_EnrollInfo] ([Id], [UserName], [Tel], [Area], [EnrollStatus], [CreatTime], [UpdateTime], [ActivityId]) VALUES (6, N'雪诺', N'18927349008', N'上海市闵行区', 1, CAST(N'2018-07-02T16:36:52.260' AS DateTime), CAST(N'2018-07-06T17:35:00.063' AS DateTime), 2)
INSERT [dbo].[T_EnrollInfo] ([Id], [UserName], [Tel], [Area], [EnrollStatus], [CreatTime], [UpdateTime], [ActivityId]) VALUES (7, N'诺兰', N'12327349008', N'上海市闵行区', 1, CAST(N'2018-07-02T16:37:43.807' AS DateTime), CAST(N'2018-07-06T17:35:00.063' AS DateTime), 2)
INSERT [dbo].[T_EnrollInfo] ([Id], [UserName], [Tel], [Area], [EnrollStatus], [CreatTime], [UpdateTime], [ActivityId]) VALUES (8, N'小指头', N'17727349008', N'上海市闵行区', 1, CAST(N'2018-07-02T16:40:01.910' AS DateTime), CAST(N'2018-07-06T17:35:00.063' AS DateTime), 2)
INSERT [dbo].[T_EnrollInfo] ([Id], [UserName], [Tel], [Area], [EnrollStatus], [CreatTime], [UpdateTime], [ActivityId]) VALUES (9, N'姜华鑫', N'135743938', N'密山', 1, CAST(N'2018-07-05T15:32:06.840' AS DateTime), CAST(N'2018-07-05T19:56:22.217' AS DateTime), 2)
INSERT [dbo].[T_EnrollInfo] ([Id], [UserName], [Tel], [Area], [EnrollStatus], [CreatTime], [UpdateTime], [ActivityId]) VALUES (10, N'瑟西', N'175899309', N'兰斯尼特', 0, CAST(N'2018-07-05T15:53:49.447' AS DateTime), CAST(N'2018-07-05T15:53:49.447' AS DateTime), 2)
INSERT [dbo].[T_EnrollInfo] ([Id], [UserName], [Tel], [Area], [EnrollStatus], [CreatTime], [UpdateTime], [ActivityId]) VALUES (11, N'龙母', N'15465455', N'龙', 0, CAST(N'2018-07-05T15:54:16.003' AS DateTime), CAST(N'2018-07-05T15:54:16.003' AS DateTime), 2)
INSERT [dbo].[T_EnrollInfo] ([Id], [UserName], [Tel], [Area], [EnrollStatus], [CreatTime], [UpdateTime], [ActivityId]) VALUES (14, N'自来也', N'1929327489', N'无', 1, CAST(N'2018-07-05T16:11:46.187' AS DateTime), CAST(N'2018-07-05T16:11:46.187' AS DateTime), 2)
INSERT [dbo].[T_EnrollInfo] ([Id], [UserName], [Tel], [Area], [EnrollStatus], [CreatTime], [UpdateTime], [ActivityId]) VALUES (15, N'佩恩777', N'379473940', N'晓', 0, CAST(N'2018-07-05T17:40:24.670' AS DateTime), CAST(N'2018-07-05T17:54:19.023' AS DateTime), 2)
INSERT [dbo].[T_EnrollInfo] ([Id], [UserName], [Tel], [Area], [EnrollStatus], [CreatTime], [UpdateTime], [ActivityId]) VALUES (16, N'带土', N'3843094232', N'宇智波家族', 0, CAST(N'2018-07-05T19:25:05.557' AS DateTime), CAST(N'2018-07-05T19:26:29.413' AS DateTime), NULL)
INSERT [dbo].[T_EnrollInfo] ([Id], [UserName], [Tel], [Area], [EnrollStatus], [CreatTime], [UpdateTime], [ActivityId]) VALUES (24, N'二代', N'843909', N'保护村子', 1, CAST(N'2018-07-05T19:58:25.330' AS DateTime), CAST(N'2018-07-05T19:58:36.727' AS DateTime), 1)
INSERT [dbo].[T_EnrollInfo] ([Id], [UserName], [Tel], [Area], [EnrollStatus], [CreatTime], [UpdateTime], [ActivityId]) VALUES (18, N'纲手', N'723499', N'木叶村', 0, CAST(N'2018-07-05T19:35:04.540' AS DateTime), CAST(N'2018-07-05T19:35:04.540' AS DateTime), 1)
INSERT [dbo].[T_EnrollInfo] ([Id], [UserName], [Tel], [Area], [EnrollStatus], [CreatTime], [UpdateTime], [ActivityId]) VALUES (20, N'四代', N'3578789', N'木叶', 0, CAST(N'2018-07-05T19:54:30.817' AS DateTime), CAST(N'2018-07-05T19:55:30.560' AS DateTime), 1)
INSERT [dbo].[T_EnrollInfo] ([Id], [UserName], [Tel], [Area], [EnrollStatus], [CreatTime], [UpdateTime], [ActivityId]) VALUES (26, N'雏田444', N'1837439920', N'木叶村子', 1, CAST(N'2018-07-06T09:52:10.297' AS DateTime), CAST(N'2018-07-06T09:52:21.937' AS DateTime), NULL)
INSERT [dbo].[T_EnrollInfo] ([Id], [UserName], [Tel], [Area], [EnrollStatus], [CreatTime], [UpdateTime], [ActivityId]) VALUES (22, N'人', N'4', N'4', 0, CAST(N'2018-07-05T19:56:09.420' AS DateTime), CAST(N'2018-07-05T19:56:09.420' AS DateTime), 1)
INSERT [dbo].[T_EnrollInfo] ([Id], [UserName], [Tel], [Area], [EnrollStatus], [CreatTime], [UpdateTime], [ActivityId]) VALUES (27, N'小南77', N'728348504', N'晓', 0, CAST(N'2018-07-06T14:37:39.720' AS DateTime), CAST(N'2018-07-06T14:37:45.530' AS DateTime), NULL)
INSERT [dbo].[T_EnrollInfo] ([Id], [UserName], [Tel], [Area], [EnrollStatus], [CreatTime], [UpdateTime], [ActivityId]) VALUES (29, N'宁次777', N'1111111', N'白眼一族7', 0, CAST(N'2018-07-06T16:20:44.177' AS DateTime), CAST(N'2018-07-06T16:20:53.787' AS DateTime), 1)
INSERT [dbo].[T_EnrollInfo] ([Id], [UserName], [Tel], [Area], [EnrollStatus], [CreatTime], [UpdateTime], [ActivityId]) VALUES (12, N'佐助嗯嗯', N'13408150815', N'上海嘉定区', 1, CAST(N'2018-07-05T16:04:05.923' AS DateTime), CAST(N'2018-07-05T16:05:03.623' AS DateTime), NULL)
INSERT [dbo].[T_EnrollInfo] ([Id], [UserName], [Tel], [Area], [EnrollStatus], [CreatTime], [UpdateTime], [ActivityId]) VALUES (13, N'小李', N'1234483904', N'火影村', 1, CAST(N'2018-07-05T16:05:25.640' AS DateTime), CAST(N'2018-07-05T16:06:20.123' AS DateTime), 1)
INSERT [dbo].[T_EnrollInfo] ([Id], [UserName], [Tel], [Area], [EnrollStatus], [CreatTime], [UpdateTime], [ActivityId]) VALUES (28, N'小明', N'13427349008', N'上海市闵行区', 1, CAST(N'2018-07-06T16:18:17.497' AS DateTime), CAST(N'2018-07-06T17:35:00.063' AS DateTime), 1)
INSERT [dbo].[T_EnrollInfo] ([Id], [UserName], [Tel], [Area], [EnrollStatus], [CreatTime], [UpdateTime], [ActivityId]) VALUES (30, N'小樱555', N'182389483', N'木叶55', 1, CAST(N'2018-07-06T17:30:07.330' AS DateTime), CAST(N'2018-07-06T17:30:17.610' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[T_EnrollInfo] OFF
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'活动名字' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Activity', @level2type=N'COLUMN',@level2name=N'ActivityName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_EnrollInfo', @level2type=N'COLUMN',@level2name=N'UserName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'手机号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_EnrollInfo', @level2type=N'COLUMN',@level2name=N'Tel'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'区域信息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_EnrollInfo', @level2type=N'COLUMN',@level2name=N'Area'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'报名状态，0为未审核，1为已通过' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_EnrollInfo', @level2type=N'COLUMN',@level2name=N'EnrollStatus'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_EnrollInfo', @level2type=N'COLUMN',@level2name=N'UpdateTime'
GO
