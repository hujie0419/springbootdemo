USE Gungnir

GO
IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   ID = OBJECT_ID(N'[dbo].[Job_SelectTaskMessage]')
                    AND OBJECTPROPERTY(ID, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].[Job_SelectTaskMessage]
GO
/*=============================================================
--FuncDes: 获取推送的App广播消息
--Author	ModifyDate	Reason
--liuchao   2016-04-20	create
--lyy		2016-06-12	alter 移除更新操作
=============================================================*/
CREATE PROCEDURE [dbo].[Job_SelectTaskMessage] 
@Now DATETIME
AS
    SELECT TOP 4 [PKID] ,
            [Subject] ,
            [Body] ,
            [BeginSendTime] ,
            [ExpiredTime] ,
            [CreatedTime] ,
            [UpdatedTime] ,
            [Status] ,
            [ActualSendTime] ,
            [Channel] ,
            [Note] ,
            [Host] ,
            [UMTaskId] ,
            [TuhuId] ,
            [AfterOpen] ,
            [ExKey1] ,
            [ExValue1] ,
            [ExKey2] ,
            [ExValue2] ,
            [ExKey3] ,
            [ExValue3] ,
            [Description] ,
            [AppActivity],
			[IOSShowBadge],
            [Tags],
            [TaskType]
    FROM    Gungnir.[dbo].[tbl_MessageProcessTask](NOLOCK)
    WHERE   [Status] = 'New'
            AND BeginSendTime <= @Now
            AND ( ExpiredTime IS NULL
                  OR ExpiredTime > @Now
                )
