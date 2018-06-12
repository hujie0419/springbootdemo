USE [Gungnir];
GO
IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   id = OBJECT_ID(N'[dbo].[Short_InsertUrlMessage]')
                    AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].[Short_InsertUrlMessage];
GO
GO
/*=============================================================
--FuncDes:		插入短链记录表(自做增量，不更新)
--Author		ModifyDate		Reason
--liuyangyang	2016-05-10		Create 100W=5min
=============================================================*/
CREATE PROCEDURE [dbo].[Short_InsertUrlMessage]
    @ShortUrlBegin NVARCHAR(200) ,
    @BaseSource NVARCHAR(100) = N'Auto' ,
    @Short_BaseUrlMessage Short_BaseUrlMessage READONLY ,
    @Result BIGINT OUTPUT
AS
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION; 
            SELECT  @Result = 0;

            DECLARE @Times INT= 0;
            DECLARE @BatchSize INT= 20000;
            DECLARE @oldIndex BIGINT;
            DECLARE @beginIndex BIGINT;
            DECLARE @Total BIGINT;
            DECLARE @Short_BaseUrlMessage_cur Short_BaseUrlMessage;

            SELECT  @oldIndex = ISNULL(MAX(PKID), 0)
            FROM    Short_UrlMessage WITH ( HOLDLOCK );
            SELECT  @Total = COUNT(1)
            FROM    @Short_BaseUrlMessage;
			
            IF @Total < 50000
                BEGIN
					--插入null的shortkey
                    INSERT  INTO Short_UrlMessage
                            ( ShortUrl ,
                              ShortKey ,
                              BaseSource ,
                              BaseUrl ,
                              BaseKey
                            )
                            SELECT  @ShortUrlBegin ,
                                    @ShortUrlBegin ,
                                    @BaseSource ,
                                    BaseUrl ,
                                    BaseKey
                            FROM    @Short_BaseUrlMessage t;

                    UPDATE  Short_UrlMessage
                    SET     ShortUrl = @ShortUrlBegin + N'/'
                            + dbo.ConvertToShort62(PKID) ,
                            ShortKey = dbo.ConvertToShort62(PKID)
                    WHERE   PKID >= @oldIndex
                            AND ShortUrl = @ShortUrlBegin;
                END;	
            ELSE
                BEGIN
                    WHILE ( @Total >= @Times * @BatchSize )
                        BEGIN
                            INSERT  INTO Short_UrlMessage
                                    ( ShortUrl ,
                                      ShortKey ,
                                      BaseSource ,
                                      BaseUrl ,
                                      BaseKey
                                    )
                                    SELECT  @ShortUrlBegin ,
                                            @ShortUrlBegin ,
                                            @BaseSource ,
                                            BaseUrl ,
                                            BaseKey
                                    FROM    ( SELECT    [BaseUrl] ,
                                                        [BaseKey]
                                              FROM      @Short_BaseUrlMessage
                                              ORDER BY  PKID
                                                        OFFSET @Times
                                                        * @BatchSize ROWS
											  FETCH NEXT @BatchSize ROWS ONLY
                                            ) t;
							
                            UPDATE  Short_UrlMessage
                            SET     ShortUrl = ShortUrl + N'/'
                                    + dbo.ConvertToShort62(PKID) ,
                                    ShortKey = dbo.ConvertToShort62(PKID)
                            WHERE   PKID >= @oldIndex
                                    AND ShortUrl = @ShortUrlBegin;

                            SET @Times = @Times + 1;
                        END;	
                END;
          
            COMMIT TRAN; 
            SET @Result = @@IDENTITY;
        END TRY
        BEGIN CATCH
            SELECT  @Result = 0;
            ROLLBACK;
        END CATCH;
    END;
GO


