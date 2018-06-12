USE Gungnir

GO
IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   ID = OBJECT_ID(N'[dbo].[Job_SelectPushMessage]')
                    AND OBJECTPROPERTY(ID, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].[Job_SelectPushMessage]
GO
/*=============================================================
--FuncDes: 获取推送的App消息
--Author	ModifyDate	Reason
--liuchao   2016-03-18	create
--exec [Job_SelectPushMessage] 'ios', '2016-4-21 00:00', 0
=============================================================*/
CREATE PROCEDURE [dbo].[Job_SelectPushMessage]
@Channel NVARCHAR(20),
@Now DATETIME,
@IsSmallLot BIT = null
AS
    SELECT TOP 3000
        mp.*,pu.Device_Tokens,ums.UserID AS [uid],
		ISNULL(ums.Notice,1) AS Notice, -- 通知默认是打开的
		ISNULL(ums.Privateletter, 0) AS Privateletter, --私信默认是关闭的
		ISNULL(ums.Radio,1) AS Radio, -- 广播默认是打开的
		ISNULL(ums.TimeSet,1) AS TimeSet -- 推送开关默认是打开的
    into #temp_messages 
    FROM    Gungnir..tbl_messageprocess (NOLOCK) mp
            INNER JOIN Tuhu_profiles..UserObject (NOLOCK) u 
				ON mp.PhoneNumber = u.u_mobile_number  COLLATE Chinese_PRC_CI_AS AND mp.Channel = @Channel
            INNER JOIN Tuhu_profiles..Push_UserInfo (NOLOCK) pu ON u.UserID = pu.UserID AND pu.Channel = @Channel
			LEFT JOIN Tuhu_profiles..UserMessageSetting (NOLOCK) ums ON u.UserID = ums.UserID
    WHERE   BeginSendTime <= @Now
            AND [Status] = 'New' AND IsSmallLot = @IsSmallLot AND PhoneNumber != ''
            AND ( mp.ExpiredTime IS NULL
                  OR mp.ExpiredTime > @Now
                ) 

	SELECT  * FROM #temp_messages

	UPDATE Gungnir..tbl_messageprocess
	SET [Status] = 'Sending',UpdatedTime = GETDATE()
	WHERE PKID IN (
		SELECT PKID FROM #temp_messages
	) 

	DROP TABLE #temp_messages
