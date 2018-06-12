USE [Gungnir]
GO
IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   id = OBJECT_ID(N'[dbo].[Sms_UpdateYunYingBatchSms]')
                    AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].[Sms_UpdateYunYingBatchSms];
GO
/*=============================================================
--FuncDes:		更新短信发送详情的短链字段
--Author		ModifyDate		Reason
--liuyangyang	2016-05-18		Create
=============================================================*/
CREATE PROCEDURE [dbo].[Sms_UpdateYunYingBatchSms]
    @ShortBeginUrl NVARCHAR(50) ,
    @MsgSubject NVARCHAR(200) = NULL ,
    @Result INT OUTPUT
AS
    BEGIN

        DECLARE @Source NVARCHAR(100)= N'tbl_YunYingBatchSms';
        DECLARE @beginKey BIGINT= 0;
        DECLARE @endKey BIGINT= 0;
        DECLARE @shortEnd BIGINT;
        DECLARE @count INT= 0;
        DECLARE @Short_BaseUrlMessage_cur Short_BaseUrlMessage;

		--【插入控制表】
        INSERT  INTO @Short_BaseUrlMessage_cur
                ( PKID ,
                  BaseUrl ,
                  BaseKey
                )
                SELECT TOP 100000
                        ROW_NUMBER() OVER ( ORDER BY PKID ) ,
                        MsgUrl ,
                        PKID
                FROM    [dbo].[tbl_YunYingBatchSms](NOLOCK)
                WHERE   ( @MsgSubject IS NULL
                          OR LEN(@MsgSubject) = 0
                          OR @MsgSubject = MsgSubject
                        )
                        AND ISNULL(IsMarkShort, 0) = 0;
        SELECT  @count = COUNT(1)
        FROM    @Short_BaseUrlMessage_cur;

		--【设置短链表开始pkid】	
        SELECT  @beginKey = ISNULL(MAX(PKID), 0)
        FROM    Short_UrlMessage WITH ( HOLDLOCK );
		--【生成短链】
        EXEC [dbo].[Short_InsertUrlMessage] @ShortUrlBegin = @ShortBeginUrl,
            @BaseSource = @Source,
            @Short_BaseUrlMessage = @Short_BaseUrlMessage_cur,
            @Result = @shortEnd OUT;
			
        IF @shortEnd >= 0
            BEGIN	
                BEGIN TRY
					--【短链表允许冗余数据】
					-- 生成短链成功，但是更新源表失败的情况下：可以不用回滚短链表已经插入的数据，重新执行本过程
                    BEGIN TRANSACTION; 
					--【更新短链数据】
                    UPDATE  detail
                    SET     detail.ShortUrl = short.ShortUrl ,
                            IsMarkShort = 1
                    FROM    dbo.tbl_YunYingBatchSms detail WITH ( ROWLOCK ) ,
                            Short_UrlMessage short WITH ( NOLOCK )
                    WHERE   short.PKID >= @beginKey
                            AND short.BaseSource = @Source
                            AND short.BaseKey = detail.PKID;

                    COMMIT TRAN; 
                   
                END TRY

                BEGIN CATCH
                    SELECT  @Result = -1;
                    ROLLBACK;
                END CATCH;
            END;

        SET @Result = @count;
    END;

