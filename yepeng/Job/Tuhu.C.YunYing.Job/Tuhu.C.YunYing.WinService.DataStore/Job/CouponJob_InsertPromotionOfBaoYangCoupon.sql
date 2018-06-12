USE Gungnir

GO
IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   ID = OBJECT_ID(N'[dbo].[CouponJob_InsertPromotionOfBaoYangCoupon]')
                    AND OBJECTPROPERTY(ID, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].[CouponJob_InsertPromotionOfBaoYangCoupon]
GO
/*=============================================================
--FuncDes: Èû±£ÑøÓÅ»ÝÈ¯
--Author	ModifyDate	Reason
--

EXEC Gungnir.dbo.CouponJob_InsertPromotionOfBaoYangCoupon  
=============================================================*/

CREATE  PROCEDURE [dbo].[CouponJob_InsertPromotionOfBaoYangCoupon]
    (
      @Promotiontype INT ,
      @RuleId INT ,
      @DiscountOfTypeOne INT ,
      @DiscountOfTypeTwo INT ,
      @MinMoneyOfTypeOne INT ,
      @MinMoneyOfTypeTwo INT ,
      @CouponDescriptionOfTypeOne NVARCHAR(255) ,
      @CouponDescriptionOfTypeTwo NVARCHAR(255) ,
      @CodeChannel NVARCHAR(20)
    )
AS
    DECLARE @i INT;
    DECLARE @j INT;
    DECLARE @totalCount INT;
    DECLARE @Code1 VARCHAR(20);
    DECLARE @Code2 VARCHAR(20);
    DECLARE @listTemp TABLE
        (
          UserID UNIQUEIDENTIFIER ,
          OrderID INT
        );

    INSERT  INTO @listTemp
            SELECT  li.UserID ,
                    li.PKID
            FROM    ( SELECT    o.UserID ,
                                o.PKID ,
                                ROW_NUMBER() OVER ( PARTITION BY o.UserID ORDER BY o.OrderDatetime DESC ) rownum
                      FROM      tbl_Order AS o WITH ( NOLOCK )
                                JOIN dbo.tbl_OrderList AS ol WITH ( NOLOCK ) ON ol.OrderID = o.PKID
                                JOIN Tuhu_productcatalog.dbo.[CarPAR_zh-CN] AS carpar
                                WITH ( NOLOCK ) ON carpar.PID = ol.PID COLLATE Chinese_PRC_CI_AS
                      WHERE     ( o.SumMoney + ISNULL(o.PromotionMoney, 0) ) >= 95
                                AND carpar.PrimaryParentCategory IN (
                                'OilFilter', 'AirFilter', 'FuelFilter' )
                               -- AND o.RegionID = 45
                                AND DATEDIFF(DAY,
                                             ( CASE WHEN o.InstallType = '3NoInstall'
                                                    THEN o.DeliveryDatetime
                                                    ELSE o.InstallDatetime
                                               END ),
                                             DATEADD(MONTH, -3, GETDATE())) >= 0
                                AND DATEDIFF(DAY,
                                             ( CASE WHEN o.InstallType = '3NoInstall'
                                                    THEN o.DeliveryDatetime
                                                    ELSE o.InstallDatetime
                                               END ),
                                             DATEADD(MONTH, -3, GETDATE())) < 7
                                AND o.Status = ( CASE WHEN o.InstallType = '3NoInstall'
                                                      THEN '2Shipped'
                                                      ELSE '3Installed'
                                                 END )
                                AND NOT EXISTS ( SELECT 1
                                                 FROM   ExciteCouponsRecord cr
                                                        WITH ( NOLOCK )
                                                 WHERE  cr.UserId = o.UserID
                                                        AND cr.Type = 0
                                                        AND GETDATE() >= cr.StartTime
                                                        AND GETDATE() <= cr.EndTime )
                                AND NOT EXISTS ( SELECT 1
                                                 FROM   dbo.tbl_Order AS oo
                                                        WITH ( NOLOCK )
                                                        JOIN dbo.tbl_OrderList
                                                        AS oll WITH ( NOLOCK ) ON oll.OrderID = oo.PKID
                                                        JOIN Tuhu_productcatalog.dbo.[CarPAR_zh-CN]
                                                        AS car WITH ( NOLOCK ) ON car.PID = oll.PID COLLATE Chinese_PRC_CI_AS
                                                 WHERE  ( oo.SumMoney
                                                          + ISNULL(oo.PromotionMoney,
                                                              0) ) >= 95
                                                        AND oo.UserID = o.UserID
                                                        AND car.PrimaryParentCategory IN (
                                                        'OilFilter',
                                                        'AirFilter',
                                                        'FuelFilter' )
                                                        AND DATEDIFF(DAY,
                                                              DATEADD(MONTH,
                                                              -3, GETDATE()),
                                                              ( CASE
                                                              WHEN oo.InstallType = '3NoInstall'
                                                              THEN oo.DeliveryDatetime
                                                              ELSE oo.InstallDatetime
                                                              END )) > 0
                                                        AND DATEDIFF(DAY,
                                                              GETDATE(),
                                                              ( CASE
                                                              WHEN oo.InstallType = '3NoInstall'
                                                              THEN oo.DeliveryDatetime
                                                              ELSE oo.InstallDatetime
                                                              END )) <= 0
                                                        AND oo.Status = ( CASE
                                                              WHEN oo.InstallType = '3NoInstall'
                                                              THEN '2Shipped'
                                                              ELSE '3Installed'
                                                              END ) )
                    ) AS li
            WHERE   li.rownum = 1;
                 

    SELECT  @totalCount = COUNT(1)
    FROM    @listTemp;

    SET @i = 1;
    WHILE ( @i <= @totalCount )
        BEGIN TRY
            SET @Code1 = RIGHT(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT) * CHECKSUM(NEWID())), 12)
            INSERT  INTO dbo.tbl_PromotionCode
                    ( Code ,
                      UserId ,
                      StartTime ,
                      EndTime ,
                      CreateTime ,
                      Status ,
                      Type ,
                      Description ,
                      Discount ,
                      MinMoney ,
                      RuleID ,
                      DeviceID ,
                      CodeChannel
                    )
                    SELECT  @Code1 ,
                            a.UserID ,
                            CONVERT(NVARCHAR(10), GETDATE(), 120) ,
                            CONVERT(NVARCHAR(10), DATEADD(MONTH, 3, GETDATE()), 120) ,
                            GETDATE() ,
                            0 ,
                            @Promotiontype ,
                            @CouponDescriptionOfTypeOne ,
                            @DiscountOfTypeOne ,
                            @MinMoneyOfTypeOne ,
                            @RuleId ,
                            NULL ,
                            @CodeChannel
                    FROM    ( SELECT    UserID ,
                                        OrderID ,
                                        ROW_NUMBER() OVER ( ORDER BY OrderID ) AS num
                              FROM      @listTemp
                            ) AS a
                    WHERE   a.num = @i;
            IF ( @@ROWCOUNT > 0 )
                BEGIN 
                    INSERT  INTO ExciteCouponsRecord
                            ( Code ,
                              Type ,
                              UserId ,
                              CreateTime ,
                              StartTime ,
                              EndTime ,
                              OrderId ,
                              PushTimes ,
                              SendTimes
                            )
                            SELECT  @Code1 ,
                                    0 ,
                                    b.UserID ,
                                    GETDATE() ,
                                    CONVERT(NVARCHAR(10), GETDATE(), 120) ,
                                    CONVERT(NVARCHAR(10), DATEADD(MONTH, 3,
                                                              GETDATE()), 120) ,
                                    b.OrderID ,
                                    0 ,
                                    0
                            FROM    ( SELECT    t.UserID ,
                                                t.OrderID ,
                                                ROW_NUMBER() OVER ( ORDER BY OrderID ) AS num
                                      FROM      @listTemp AS t
                                    ) AS b
                            WHERE   b.num = @i;
                    SET @i += 1;
                END 
        

        END TRY
        BEGIN CATCH 
        END CATCH 
    SET @j = 1;
    WHILE ( @j <= @totalCount )
        BEGIN TRY 
            SET @Code2 = RIGHT(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT) * CHECKSUM(NEWID())), 12)
            INSERT  INTO dbo.tbl_PromotionCode
                    ( Code ,
                      UserId ,
                      StartTime ,
                      EndTime ,
                      CreateTime ,
                      Status ,
                      Type ,
                      Description ,
                      Discount ,
                      MinMoney ,
                      RuleID ,
                      DeviceID ,
                      CodeChannel
                    )
                    SELECT  @Code2 ,
                            a.UserID ,
                            CONVERT(NVARCHAR(10), GETDATE(), 120) ,
                            CONVERT(NVARCHAR(10), DATEADD(MONTH, 3, GETDATE()), 120) ,
                            GETDATE() ,
                            0 ,
                            @Promotiontype ,
                            @CouponDescriptionOfTypeTwo ,
                            @DiscountOfTypeTwo ,
                            @MinMoneyOfTypeTwo ,
                            @RuleId ,
                            NULL ,
                            @CodeChannel
                    FROM    ( SELECT    UserID ,
                                        OrderID ,
                                        ROW_NUMBER() OVER ( ORDER BY OrderID ) AS num
                              FROM      @listTemp
                            ) AS a
                    WHERE   a.num = @j;
            IF ( @@ROWCOUNT > 0 )
                BEGIN 
                    INSERT  INTO ExciteCouponsRecord
                            ( Code ,
                              Type ,
                              UserId ,
                              CreateTime ,
                              StartTime ,
                              EndTime ,
                              OrderId ,
                              PushTimes ,
                              SendTimes
                            )
                            SELECT  @Code2 ,
                                    0 ,
                                    b.UserID ,
                                    GETDATE() ,
                                    CONVERT(NVARCHAR(10), GETDATE(), 120) ,
                                    CONVERT(NVARCHAR(10), DATEADD(MONTH, 3,
                                                              GETDATE()), 120) ,
                                    b.OrderID ,
                                    0 ,
                                    0
                            FROM    ( SELECT    t.UserID ,
                                                t.OrderID ,
                                                ROW_NUMBER() OVER ( ORDER BY OrderID ) AS num
                                      FROM      @listTemp AS t
                                    ) AS b
                            WHERE   b.num = @j;
                    SET @j += 1;
                END           
        END TRY 
        BEGIN CATCH 
        END CATCH 
	

   
GO


