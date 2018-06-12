USE Gungnir

GO
IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   ID = OBJECT_ID(N'[dbo].[Job_UpdatePushTaskMessage]')
                    AND OBJECTPROPERTY(ID, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].[Job_UpdatePushTaskMessage]
GO
/*=============================================================
--FuncDes: update message
--Author	ModifyDate	Reason
--liuchao   2016-04-20	create
=============================================================*/
CREATE PROCEDURE [dbo].[Job_UpdatePushTaskMessage]
	@Id INT,
    @Status NVARCHAR(20) ,
    @ActualSendTime DATETIME,
	@Note NVARCHAR(500),
    @UMTaskId NVARCHAR(200),
    @TuhuId NVARCHAR(50)
AS
    UPDATE  Gungnir..tbl_MessageProcessTask
    SET     UpdatedTime = GETDATE() ,
            ActualSendTime = @ActualSendTime ,
            [Status] = @Status,
			[Note] = @Note,
			UMTaskId = @UMTaskId,
			TuhuId = @TuhuId
    WHERE   PKID = @Id