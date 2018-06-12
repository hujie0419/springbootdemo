USE Gungnir;

GO
IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   id = OBJECT_ID(N'[dbo].[Job_UpdatePushTaskMessage]')
                    AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].[Job_UpdatePushTaskMessage];
GO
/*=============================================================
--FuncDes: update message
--Author	ModifyDate	Reason
--liuchao   2016-04-20	create
--lyy		2016-06-12	alter-增加发送状态更新
=============================================================*/
CREATE PROCEDURE [dbo].[Job_UpdatePushTaskMessage]
    @Id INT ,
    @Status NVARCHAR(20) ,
    @ActualSendTime DATETIME ,
    @Note NVARCHAR(500) ,
    @UMTaskId NVARCHAR(200) ,
    @TuhuId NVARCHAR(50)
AS
    IF LOWER(@Status) = 'sending'
        UPDATE  Gungnir..tbl_MessageProcessTask WITH (ROWLOCK)
        SET     [UpdatedTime] = GETDATE() ,
                [Status] = @Status
        WHERE   PKID = @Id;
    ELSE
        UPDATE  Gungnir..tbl_MessageProcessTask WITH (ROWLOCK)
        SET     UpdatedTime = GETDATE() ,
                ActualSendTime = @ActualSendTime ,
                [Status] = @Status ,
                [Note] = @Note ,
                UMTaskId = @UMTaskId ,
                TuhuId = @TuhuId
        WHERE   PKID = @Id;