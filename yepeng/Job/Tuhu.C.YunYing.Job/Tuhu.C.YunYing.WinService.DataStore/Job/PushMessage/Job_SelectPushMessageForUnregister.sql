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
--lyy		2016-06-12	alter 移除更新操作
=============================================================*/
CREATE PROCEDURE [dbo].[Job_SelectPushMessageForUnregister]
@Now DATETIME
AS
    SELECT TOP 5000 
		PKID ,
        Subject ,
        Body ,
        MessageType ,
        BeginSendTime ,
        ExpiredTime ,
        CreatedTime ,
        UpdatedTime ,
        UserId ,
        PhoneNumber ,
        Status ,
        ActualSendTime ,
        Channel ,
        Note ,
        Host ,
        UMMessageId ,
        TuhuId ,
        AfterOpen ,
        ExKey1 ,
        ExValue1 ,
        ExKey2 ,
        ExValue2 ,
        Description ,
        ProcessType ,
        AppActivity ,
        ActivityType ,
        DeviceToken ,
        ExKey3 ,
        ExValue3 ,
        IsSmallLot ,
        InfoPKID ,
        CenterMsgType ,
        BigImagePath
    FROM    Gungnir..tbl_messageprocess (NOLOCK)
    WHERE   BeginSendTime <= @Now AND ISNULL(PhoneNumber, '') = '' AND DeviceToken != ''
            AND [Status] = 'New'
            AND ( ExpiredTime IS NULL
                  OR ExpiredTime > @Now
                )
