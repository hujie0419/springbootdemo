USE [Gungnir]
GO
IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   ID = OBJECT_ID(N'[dbo].[CouponJob_InsertPromotionOfNotBaoYangCoupon]')
                    AND OBJECTPROPERTY(ID, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].[CouponJob_InsertPromotionOfNotBaoYangCoupon]
GO
/*=============================================================
--FuncDes: 塞入非保养优惠券
--Author	ModifyDate	Reason
--
=============================================================*/

CREATE PROCEDURE [dbo].[CouponJob_InsertPromotionOfNotBaoYangCoupon]
    (
      @Promotiontype INT ,
      @RuleId INT ,
      @DiscountOfTypeOne INT ,
      @DiscountOfTypeTwo INT ,
      @MinMoneyOfTypeOne INT ,
      @MinMoneyOFTypeTwo INT ,
      @DescriptionOfTypeOne NVARCHAR(255) ,
      @DescriptionOfTypeTwo NVARCHAR(255) ,
      @CodeChannel NVARCHAR(20)
    )
AS
    DECLARE @totalCount INT;
    DECLARE @i INT;
    DECLARE @j INT;
    DECLARE @code1 VARCHAR(20);
    DECLARE @code2 VARCHAR(20);
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
                                o.OrderDatetime ,
                                ROW_NUMBER() OVER ( PARTITION BY o.UserID ORDER BY o.OrderDatetime DESC ) rownum
                      FROM      tbl_Order AS o WITH ( NOLOCK )
                                JOIN dbo.tbl_OrderList AS ol WITH ( NOLOCK ) ON ol.OrderID = o.PKID
                                JOIN Tuhu_productcatalog.dbo.[CarPAR_zh-CN] AS carpar
                                WITH ( NOLOCK ) ON carpar.PID = ol.PID COLLATE Chinese_PRC_CI_AS
                      WHERE     ( o.SumMoney + ISNULL(o.PromotionMoney, 0) ) >= 100
                               -- AND o.RegionID = 45
                                AND EXISTS ( SELECT 1
                                             FROM   tbl_region rg WITH ( NOLOCK )
                                             WHERE  rg.PKID = o.RegionID
                                                    AND ( rg.PKID IN ( 1, 2, 8,
                                                              4, 5, 7 )
                                                          OR rg.ParentID IN (
                                                          1, 2, 8, 4, 5, 7 )
                                                        ) )
                                AND o.Status = ( CASE WHEN o.InstallType = '3NoInstall'
                                                      THEN '2Shipped'
                                                      ELSE '3Installed'
                                                 END )
                                --AND DATEDIFF(DAY,
                                --             ( CASE WHEN o.InstallType = '3NoInstall'
                                --                    THEN o.DeliveryDatetime
                                --                    ELSE o.InstallDatetime
                                --               END ), GETDATE()) = 2
                                AND NOT EXISTS ( SELECT 1
                                                 FROM   ExciteCouponsRecord cr
                                                        WITH ( NOLOCK )
                                                 WHERE  cr.UserId = o.UserID
                                                        AND cr.Type = 1
                                                        AND GETDATE() >= cr.StartTime
                                                        AND GETDATE() <= cr.EndTime )
                                AND NOT EXISTS ( SELECT 1
                                                 FROM   tbl_Order oo WITH ( NOLOCK )
                                                        JOIN dbo.tbl_OrderList oll
                                                        WITH ( NOLOCK ) ON oo.PKID = oll.OrderID
                                                        JOIN Tuhu_productcatalog.dbo.[CarPAR_zh-CN] crr
                                                        WITH ( NOLOCK ) ON crr.PID = oll.PID COLLATE Chinese_PRC_CI_AS
                                                 WHERE  o.UserID = oo.UserID
                                                        AND crr.PrimaryParentCategory IN (
                                                        'OilFilter',
                                                        'AirFilter',
                                                        'FuelFilter' )
                                                        AND oo.Status = ( CASE
                                                              WHEN o.InstallType = '3NoInstall'
                                                              THEN '2Shipped'
                                                              ELSE '3Installed'
                                                              END ) )
                    ) AS li
            WHERE   li.rownum = 1;



    SELECT  @totalCount = COUNT(1)
    FROM    @listTemp;
	SELECT * FROM @listTemp;


    SET @i = 1;
    WHILE ( @i <= @totalCount )
        BEGIN TRY
            SELECT  @code1 = RIGHT(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT) * CHECKSUM(NEWID())), 12)
            INSERT  INTO dbo.tbl_PromotionCode
                    ( Code ,
                      UserId ,
                      StartTime ,
                      EndTime ,
                      CreateTime ,
                      OrderId ,
                      Status ,
                      Type ,
                      Description ,
                      Discount ,
                      MinMoney ,
                      RuleID ,
                      CodeChannel
                    )
                    SELECT  @code1 ,
                            a.UserID ,
                            CONVERT(NVARCHAR(10), GETDATE(), 120) ,
                            CONVERT(NVARCHAR(10), DATEADD(MONTH, 1, GETDATE()), 120) ,
                            GETDATE() ,
                            a.OrderID ,
                            0 ,
                            @Promotiontype ,
                            @DescriptionOfTypeOne ,
                            @DiscountOfTypeOne ,
                            @MinMoneyOfTypeOne ,
                            @RuleId ,
                            @CodeChannel
                    FROM    ( SELECT    UserID ,
                                        OrderID ,
                                        ROW_NUMBER() OVER ( ORDER BY OrderID ) AS num
                              FROM      @listTemp
                            ) AS a
                    WHERE   a.num = @i;
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
                    SELECT  @code1 ,
                            1 ,
                            b.UserID ,
                            GETDATE() ,
                            CONVERT(NVARCHAR(10), GETDATE(), 120) ,
                            CONVERT(NVARCHAR(10), DATEADD(MONTH, 1, GETDATE()), 120) ,
                            b.OrderID ,
                            0 ,
                            0
                    FROM    ( SELECT    UserID ,
                                        OrderID ,
                                        ROW_NUMBER() OVER ( ORDER BY OrderID ) AS num
                              FROM      @listTemp
                            ) AS b
                    WHERE   b.num = @i;
            SET @i += 1;
        END TRY 
        BEGIN CATCH
        END CATCH 

    SET @j = 1
    WHILE ( @j < @totalCount )
        BEGIN TRY 	          
            SELECT  @code2 = RIGHT(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT) * CHECKSUM(NEWID())), 12)
            INSERT  INTO dbo.tbl_PromotionCode
                    ( Code ,
                      UserId ,
                      StartTime ,
                      EndTime ,
                      CreateTime ,
                      OrderId ,
                      Status ,
                      Type ,
                      Description ,
                      Discount ,
                      MinMoney ,
                      RuleID ,
                      CodeChannel
                    )
                    SELECT  @code2 ,
                            a.UserID ,
                            CONVERT(NVARCHAR(10), GETDATE(), 120) ,
                            CONVERT(NVARCHAR(10), DATEADD(MONTH, 1, GETDATE()), 120) ,
                            GETDATE() ,
                            a.OrderID ,
                            0 ,
                            @Promotiontype ,
                            @DescriptionOfTypeTwo ,
                            @DiscountOfTypeTwo ,
                            @MinMoneyOFTypeTwo ,
                            @RuleId ,
                            @CodeChannel
                    FROM    ( SELECT    UserID ,
                                        OrderID ,
                                        ROW_NUMBER() OVER ( ORDER BY OrderID ) AS num
                              FROM      @listTemp
                            ) AS a
                    WHERE   a.num = @j;
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
                    SELECT  @code2 ,
                            1 ,
                            b.UserID ,
                            GETDATE() ,
                            CONVERT(NVARCHAR(10), GETDATE(), 120) ,
                            CONVERT(NVARCHAR(10), DATEADD(MONTH, 1, GETDATE()), 120) ,
                            b.OrderID ,
                            0 ,
                            0
                    FROM    ( SELECT    UserID ,
                                        OrderID ,
                                        ROW_NUMBER() OVER ( ORDER BY OrderID ) AS num
                              FROM      @listTemp
                            ) AS b
                    WHERE   b.num = @j;

            SET @j += 1;

        END TRY  
        BEGIN CATCH
        END CATCH 
GO


