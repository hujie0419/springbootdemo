USE Gungnir
GO

IF EXISTS ( SELECT	*
			FROM	dbo.sysobjects
			WHERE	ID = OBJECT_ID(N'[dbo].[BI_SyncMessageProcess]')
					AND OBJECTPROPERTY(ID, N'IsProcedure') = 1 )
   DROP PROCEDURE [dbo].[BI_SyncMessageProcess]
GO
/*=============================================================
--FuncDes:	同步BI的消息推送数据
--Author	ModifyDate		Reason
--liuchao	2016-04-12		Create
=============================================================*/
CREATE PROCEDURE [dbo].[BI_SyncMessageProcess]
AS 

DECLARE @AndroidKey NVARCHAR(100)
DECLARE @AndroidValue NVARCHAR(500)
DECLARE @IOSKey NVARCHAR(100)
DECLARE @IOSValue NVARCHAR(500)


DECLARE @MaxLoopCount  TINYINT = 50
SELECT TOP 5000 * 
INTO #tempSyncMessage
FROM Gungnir..dw_MessageProcess(NOLOCK)
WHERE isSync = 0
ORDER BY pkid

SELECT PhoneNumber,activitytype,BeginSendTime, ROW_NUMBER() OVER (ORDER BY PhoneNumber) AS ID 
INTO #tempAppMessage
FROM #tempSyncMessage
WHERE PhoneNumber IS NOT NULL
GROUP BY PhoneNumber,activitytype,BeginSendTime

--SET IDENTITY_INSERT #tempSyncMessage ON

WHILE (EXISTS (SELECT top 1 1 FROM  #tempSyncMessage(NOLOCK)) AND @MaxLoopCount > 0)
BEGIN
	SET @MaxLoopCount -= 1
	BEGIN TRY
		IF 1=1 -- 插入消息系统
		BEGIN
			DECLARE @ID INT;
			DECLARE @PhoneNumber NVARCHAR(32);
			DECLARE @ActivityType NVARCHAR(100);
			DECLARE @BeginSendTime DATETIME;
			WHILE(EXISTS(SELECT TOP 1 1  FROM  #tempAppMessage (NOLOCK) ))
			BEGIN
				SELECT TOP 1 @PhoneNumber = PhoneNumber, @ID = ID, @ActivityType= activitytype, @BeginSendTime = BeginSendTime  FROM  #tempAppMessage (NOLOCK) 
				SET @AndroidKey = NULL
				SET @AndroidValue = NULL
				SET @IOSKey = NULL
				SET @IOSValue = NULL

				SELECT 
					@AndroidKey = CASE WHEN Channel = 'Android' AND AppActivity <> '' THEN AppActivity ELSE NULL END,
					@AndroidValue = CASE WHEN Channel = 'Android' AND AppActivity <> '' AND ExKey1 <> '' AND ExKey2 IS NULL THEN 
						N'[{''' + ExKey1 + N''':''' + ExValue1 + N'''}]' 
						WHEN Channel = 'Android' AND AppActivity <> '' AND ExKey1 <> '' AND ExKey2 <> '' THEN
						N'[{''' + ExKey1 + N''':''' + ExValue1 + N''',''' + ExKey2 + N''':'''+ExValue2 + N'''}]'
						ELSE NULL END
				FROM #tempSyncMessage(NOLOCK) 
				WHERE Channel = 'Android' AND PhoneNumber = @PhoneNumber AND activitytype = @ActivityType AND BeginSendTime = @BeginSendTime 

				SELECT  
					@IOSKey = CASE WHEN Channel = 'IOS' AND ExKey1 = 'appoperateval' THEN ExValue1 ELSE NULL END,
					@IOSValue = CASE WHEN Channel = 'IOS' AND ExKey2 = 'keyvaluelenth' THEN ExValue2 ELSE NULL END
				FROM #tempSyncMessage(NOLOCK) 
				WHERE Channel = 'IOS' AND PhoneNumber = @PhoneNumber AND activitytype = @ActivityType AND BeginSendTime = @BeginSendTime  

				INSERT INTO  Gungnir..tbl_My_Center_News
						( UserObjectID ,
							News ,
							[Type] ,
							CreateTime ,
							UpdateTime ,
							Title ,
							HeadImage ,
							isdelete ,
							IOSKey ,
							IOSValue ,
							androidKey ,
							androidValue,
							BeginShowTime
						)
                    SELECT TOP 1
                            uo.UserID ,
                            mp.Body ,
                            N'1普通' ,
                            GETDATE() ,
                            GETDATE() ,
                            mp.[Subject] ,
                            HeadImage ,
                            0 ,
                            @IOSKey ,
                            @IOSValue ,
                            @AndroidKey ,
                            @AndroidValue ,
                            mp.BeginSendTime
                    FROM    #tempSyncMessage (NOLOCK) mp
                            INNER JOIN Tuhu_profiles..UserObject (NOLOCK) uo ON mp.PhoneNumber = uo.u_mobile_number COLLATE Chinese_PRC_CI_AS
                    WHERE   PhoneNumber = @PhoneNumber

                    DELETE  FROM #tempAppMessage
                    WHERE   ID = @ID

            END
		END 

		-- 插入推送系统
		INSERT INTO Gungnir..tbl_MessageProcess
				( [Subject] ,
				  Body ,
				  MessageType ,
				  BeginSendTime ,
				  ExpiredTime ,
				  CreatedTime ,
				  UpdatedTime ,
				  UserId ,
				  PhoneNumber ,
				  [Status] ,
				  Channel ,
				  Host ,
				  AfterOpen ,
				  ExKey1 ,
				  ExValue1 ,
				  ExKey2 ,
				  ExValue2 ,
				  [Description] ,
				  ProcessType ,
				  AppActivity,
				  [ActivityType],
				  DeviceToken
				)
		SELECT [Subject],
				Body,
				MessageType,
				ISNULL(BeginSendTime,GETDATE()),
				ExpiredTime,
				GETDATE(),
				GETDATE(),
				NULL,
				PhoneNumber,
				N'New',
				Channel,
				N'BI.Job',
				AfterOpen,
				ExKey1,
				ExValue1,
				ExKey2,
				ExValue2,
				[Description],
				ProcessType,
				AppActivity,
				[ActivityType],
				devicetoken
		FROM #tempSyncMessage(NOLOCK)

		IF ( @@ROWCOUNT > 0 AND @@ERROR = 0)
		BEGIN			
			UPDATE mp SET isSync = 1
			FROM Gungnir..dw_MessageProcess AS mp
			JOIN #tempSyncMessage sm ON mp.PKID = sm.PKID		
		END 
		DELETE FROM #tempSyncMessage
		INSERT INTO #tempSyncMessage 
		(
			   [PKID]
			  ,[Subject]
			  ,[Body]
			  ,[MessageType]
			  ,[BeginSendTime]
			  ,[ExpiredTime]
			  ,[CreatedTime]
			  ,[UpdatedTime]
			  ,[PhoneNumber]
			  ,[Status]
			  ,[Channel]
			  ,[AfterOpen]
			  ,[ExKey1]
			  ,[ExValue1]
			  ,[ExKey2]
			  ,[ExValue2]
			  ,[Description]
			  ,[ProcessType]
			  ,[AppActivity]
			  ,[IsTwice]
			  ,[HeadImage]
			  ,[isSync]
			  ,[devicetoken]
			  ,[ActivityType]
		)
			SELECT TOP 5000 
				[PKID]
			  ,[Subject]
			  ,[Body]
			  ,[MessageType]
			  ,[BeginSendTime]
			  ,[ExpiredTime]
			  ,[CreatedTime]
			  ,[UpdatedTime]
			  ,[PhoneNumber]
			  ,[Status]
			  ,[Channel]
			  ,[AfterOpen]
			  ,[ExKey1]
			  ,[ExValue1]
			  ,[ExKey2]
			  ,[ExValue2]
			  ,[Description]
			  ,[ProcessType]
			  ,[AppActivity]
			  ,[IsTwice]
			  ,[HeadImage]
			  ,[isSync]
			  ,[devicetoken]
			  ,[ActivityType]
			FROM Gungnir..dw_MessageProcess(NOLOCK) 
			WHERE isSync = 0 ORDER BY PKID
	END TRY
	BEGIN CATCH
		SET @MaxLoopCount = 0
	END CATCH 
END

DROP TABLE #tempSyncMessage
DROP TABLE #tempAppMessage

GO