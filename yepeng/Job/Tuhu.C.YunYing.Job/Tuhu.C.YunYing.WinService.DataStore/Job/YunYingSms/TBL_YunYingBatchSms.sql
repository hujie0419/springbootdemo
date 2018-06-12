USE [Gungnir]
GO

--DROP TABLE tbl_YunYingBatchSms
--SMS同步表 *为同步时传入字段
GO
CREATE TABLE [dbo].[tbl_YunYingBatchSms](
	[PKID] [INT]  PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[PhoneNumber] [NVARCHAR](100) NOT NULL,--短信手机号  *
	[MsgBatchs] [NVARCHAR](100)	 NULL,--消息批次 *
	[MsgSubject] [NVARCHAR](500) NULL,--短信种类 *
	[MsgBody] [NVARCHAR](500) NULL,--短信内容 *
	[NeedSentTime] [DATETIME] NULL,--准备发送时间 *
	[SentTime] [DATETIME] NULL,--实际发送时间
	[MsgUrl] [NVARCHAR](1000) NULL,--跳转url *
	[ShortUrl] [NVARCHAR](300) NULL,--跳转url短链
	[Status] [NVARCHAR](30) NULL,--短信发送状态 0:未发送 1:发送中 2：发送成功 3：发送失败
	[IsActive] [BIT] NULL,--是否有效
	[IsMarkShort] [BIT] NULL,--是否已经生成短链　
	[CreatedTime] [DATETIME] NULL,--创建时间
	[RelatedUser] [NVARCHAR](50) NULL,--发布人 *
	[LastUpdateTime] [DATETIME] NULL,--最后更新时间
) 

GO

ALTER TABLE [dbo].[tbl_YunYingBatchSms] ADD  CONSTRAINT [DF_tbl_YunYingBatchSms_NeedSentTime]  DEFAULT (GETDATE()) FOR [NeedSentTime]
GO

ALTER TABLE [dbo].[tbl_YunYingBatchSms] ADD  CONSTRAINT [DF_tbl_YunYingBatchSms_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

ALTER TABLE [dbo].[tbl_YunYingBatchSms] ADD  CONSTRAINT [DF_tbl_YunYingBatchSms_CreatedTime]  DEFAULT (GETDATE()) FOR [CreatedTime]
GO

ALTER TABLE [dbo].[tbl_YunYingBatchSms] ADD  CONSTRAINT [DF_tbl_YunYingBatchSms_LastUpdateTime]  DEFAULT (GETDATE()) FOR [LastUpdateTime]
GO

ALTER TABLE [dbo].[tbl_YunYingBatchSms] ADD  CONSTRAINT [DF_tbl_YunYingBatchSms_Status]  DEFAULT ((N'New')) FOR [Status]
GO


