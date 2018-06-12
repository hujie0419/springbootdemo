USE Gungnir

GO
IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   ID = OBJECT_ID(N'[dbo].[UpdatePushMessage]')
                    AND OBJECTPROPERTY(ID, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].[UpdatePushMessage]
GO
/*=============================================================
--FuncDes: update message
--Author	ModifyDate	Reason
--liuchao   2016-03-18	create
=============================================================*/
CREATE PROCEDURE [dbo].[UpdatePushMessage]
	@Id INT,
    @Status NVARCHAR(20) ,
    @ActualSendTime DATETIME,
	@Note NVARCHAR(500),
    @UMMessageId NVARCHAR(200),
    @TuhuId NVARCHAR(50)
AS
    UPDATE  Gungnir..tbl_messageprocess
    SET     UpdatedTime = GETDATE() ,
            ActualSendTime = @ActualSendTime ,
            [Status] = @Status,
			[Note] = @Note,
			UMMessageId = @UMMessageId,
			TuhuId = @TuhuId
    WHERE   PKID = @Id