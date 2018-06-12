USE Gungnir;

GO
IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   id = OBJECT_ID(N'[dbo].[UpdatePushMessage]')
                    AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].[UpdatePushMessage];
GO
/*=============================================================
--FuncDes: update message
--Author	ModifyDate	Reason
--liuchao   2016-03-18	create
--lyy		2016-06-12	alter-增加发送状态更新
=============================================================*/
CREATE PROCEDURE [dbo].[UpdatePushMessage]
    @Id INT ,
    @Status NVARCHAR(20) ,
    @ActualSendTime DATETIME ,
    @Note NVARCHAR(500) ,
    @UMMessageId NVARCHAR(200) ,
    @TuhuId NVARCHAR(50)
AS
    IF LOWER(@Status) = 'sending'
        UPDATE  Gungnir..tbl_MessageProcess WITH (ROWLOCK)
        SET     [UpdatedTime] = GETDATE() ,
                [Status] = @Status
        WHERE   PKID = @Id;
    ELSE
        UPDATE  Gungnir..tbl_MessageProcess WITH (ROWLOCK)
        SET     UpdatedTime = GETDATE() ,
                ActualSendTime = @ActualSendTime ,
                [Status] = @Status ,
                [Note] = @Note ,
                UMMessageId = @UMMessageId ,
                TuhuId = @TuhuId
        WHERE   PKID = @Id;