USE Gungnir

GO
IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   ID = OBJECT_ID(N'[dbo].[Job_SelectPushMessageForUnregister]')
                    AND OBJECTPROPERTY(ID, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].[Job_SelectPushMessageForUnregister]
GO
/*=============================================================
--FuncDes: 获取非注册用户推送的App消息
--Author	ModifyDate	Reason
--liuchao   2016-04-15	create
=============================================================*/
CREATE PROCEDURE [dbo].[Job_SelectPushMessageForUnregister]
@Now DATETIME
AS
    SELECT *
    into #temp_messages
    FROM    Gungnir..tbl_messageprocess (NOLOCK)
    WHERE   BeginSendTime <= @Now AND ISNULL(PhoneNumber, '') = '' AND DeviceToken != ''
            AND [Status] = 'New'
            AND ( ExpiredTime IS NULL
                  OR ExpiredTime > @Now
                )

	SELECT  * FROM #temp_messages

	UPDATE mp SET mp.[Status] = 'Sending',mp.UpdatedTime = GETDATE()
	FROM  Gungnir..tbl_messageprocess  mp	
	JOIN #temp_messages AS tm ON mp.PKID = tm.PKID

	DROP TABLE #temp_messages
