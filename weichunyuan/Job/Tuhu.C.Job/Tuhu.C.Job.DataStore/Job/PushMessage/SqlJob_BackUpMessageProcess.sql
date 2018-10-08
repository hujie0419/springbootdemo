USE Gungnir
GO

IF EXISTS ( SELECT	*
			FROM	dbo.sysobjects
			WHERE	ID = OBJECT_ID(N'[dbo].[SqlJob_BackUpMessageProcess]')
					AND OBJECTPROPERTY(ID, N'IsProcedure') = 1 )
   DROP PROCEDURE [dbo].[SqlJob_BackUpMessageProcess]
GO
/*=============================================================
--FuncDes:	备份tbl_MessageProcess到history表
--Author	ModifyDate		Reason
--liuchao	2016-04-21		Create
=============================================================*/
CREATE PROCEDURE [dbo].[SqlJob_BackUpMessageProcess]
AS 
INSERT INTO Gungnir_History.[dbo].[tbl_MessageProcess]
           ([Subject]
           ,[Body]
           ,[MessageType]
           ,[BeginSendTime]
           ,[ExpiredTime]
           ,[CreatedTime]
           ,[UpdatedTime]
           ,[UserId]
           ,[PhoneNumber]
           ,[Status]
           ,[ActualSendTime]
           ,[Channel]
           ,[Note]
           ,[Host]
           ,[UMMessageId]
           ,[TuhuId]
           ,[AfterOpen]
           ,[ExKey1]
           ,[ExValue1]
           ,[ExKey2]
           ,[ExValue2]
           ,[Description]
           ,[ProcessType]
           ,[AppActivity]
           ,[ActivityType]
           ,[DeviceToken]
           ,[ExKey3]
           ,[ExValue3]
           ,[IsSmallLot])
  SELECT [Subject]
      ,[Body]
      ,[MessageType]
      ,[BeginSendTime]
      ,[ExpiredTime]
      ,[CreatedTime]
      ,[UpdatedTime]
      ,[UserId]
      ,[PhoneNumber]
      ,[Status]
      ,[ActualSendTime]
      ,[Channel]
      ,[Note]
      ,[Host]
      ,[UMMessageId]
      ,[TuhuId]
      ,[AfterOpen]
      ,[ExKey1]
      ,[ExValue1]
      ,[ExKey2]
      ,[ExValue2]
      ,[Description]
      ,[ProcessType]
      ,[AppActivity]
      ,[ActivityType]
      ,[DeviceToken]
      ,[ExKey3]
      ,[ExValue3]
      ,[IsSmallLot]
  FROM Gungnir.[dbo].[tbl_MessageProcess] (Nolock)
  WHERE BeginSendTime < DATEADD(DAY, -7, GETDATE())  and ([ExpiredTime] <  DATEADD(DAY, -3, GETDATE()) OR [Status] <> 'New')

  IF @@ERROR = 0 AND @@ROWCOUNT > 0
  BEGIN
	DELETE FROM Gungnir..[tbl_MessageProcess] 
	WHERE BeginSendTime < DATEADD(DAY, -7, GETDATE())  and ([ExpiredTime] <  DATEADD(DAY, -3, GETDATE()) OR [Status] <> 'New') 
  END 
GO

