USE Gungnir;

GO
IF EXISTS ( SELECT	*
			FROM	dbo.sysobjects
			WHERE	id = OBJECT_ID(N'[dbo].[CouponJob_InsertFreeOrderCoupon]')
					AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
   DROP PROCEDURE [dbo].[CouponJob_InsertFreeOrderCoupon];
GO
/*=============================================================
--FuncDes: 塞免单券
--Author	ModifyDate	Reason
--liuchao   2016-03-24	Create

EXEC Gungnir.dbo.CouponJob_InsertFreeOrderCoupon 2 , N'买轮胎送保养福利券，到店保养订单可使用'
=============================================================*/

CREATE  PROCEDURE [dbo].[CouponJob_InsertFreeOrderCoupon]
(
  @Promotiontype INT,
  @CodeChannel NVARCHAR(20),
  @Description NVARCHAR(255)
)
AS
BEGIN

	  DECLARE @Index INT  = 1;
	  DECLARE @Count INT = 0;
	  DECLARE @Code VARCHAR(20);
	  DECLARE @Channel NVARCHAR(1000) = N'1官网闪购,1网站,p电话,8安卓,8安卓A区,8安卓B区,8安卓C区,8安卓D区,8安卓E区,8安卓F区,8安卓G区,8安卓H区,8安卓I区,8安卓J区,8安卓车品区域,8安卓车品区域F1,8安卓闪购,8手机,8手机限时抢购,8手机集采,8限时抢购,8集采,c微信,JIOS,JIOSA区,JIOSB区,JIOSC区,JIOSD区,JIOSE区,JIOSF区,JIOSG区,JIOSH区,JIOSI区,JIOSJ区,JIOS车品区域,JIOS闪购,JIOS限时抢购,JIOS集采,kH5,mApp_h5';
	  DECLARE @BeginDate DATE = GETDATE();
	  DECLARE @EndDate DATE = DATEADD(MONTH, 1, @BeginDate);
	
	  SELECT	ROW_NUMBER() OVER ( ORDER BY tmp.OrderId ) AS [No],
				tmp.OrderId,
				tmp.UserID,
				CASE WHEN tmp.num = 1 THEN 1157
					 WHEN tmp.num = 2 THEN 1160
					 WHEN tmp.num = 3 THEN 1163
					 WHEN tmp.num >= 4 THEN 1166
				END AS RuleId
	  INTO		#tmp_users
	  FROM		( SELECT DISTINCT
							o.PKID AS OrderId,
							o.UserID,
							SUM(ol.Num) AS num
				  FROM		Gungnir.dbo.tbl_Order (NOLOCK) o
				  JOIN		Gungnir.dbo.tbl_OrderList (NOLOCK) ol
							ON o.PKID = ol.OrderID
				  JOIN		Tuhu_productcatalog.dbo.[CarPAR_zh-CN] (NOLOCK) cp
							ON ol.PID = cp.PID  COLLATE Chinese_PRC_CI_AS
				  JOIN		Tuhu_productcatalog..CarPAR_CatalogHierarchy (NOLOCK) ch
							ON ch.child_oid = cp.oid
				  JOIN		( SELECT	Item
							  FROM		Gungnir.dbo.SplitString(@Channel, ',', 1)
							) AS oo
							ON o.OrderChannel = oo.Item
				  WHERE		o.InstallType = N'1ShopInstall'
							AND o.[Status] = N'3Installed'
							AND ol.Deleted = 0
							AND ( ol.ProductType & 32 ) != 32
							AND cp.PID LIKE 'TR-%'
							AND o.OrderDatetime >= '2016-3-24 00:00:00'
							AND o.OrderDatetime <= '2016-4-1 10:00:00'
							AND ch.NodeNo LIKE '1.%'
							AND NOT EXISTS ( SELECT	1
											 FROM	Gungnir..TempCouponActivity (NOLOCK) tca
											 WHERE	tca.OrderId = o.PKID
													AND tca.ActivityNo = 1 )
				  GROUP BY	o.PKID,
							o.UserID
				) tmp;

	  SELECT	@Count = COUNT(1)
	  FROM		#tmp_users;

	  WHILE ( @Index <= @Count )
	  BEGIN TRY
			SET @Code = RIGHT(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT) * CHECKSUM(NEWID())), 12);
			BEGIN TRAN tt;
				  INSERT	INTO Gungnir.dbo.tbl_PromotionCode
							( Code,
							  UserId,
							  StartTime,
							  EndTime,
							  CreateTime,
							  Status,
							  Type,
							  Description,
							  Discount,
							  MinMoney,
							  RuleID,
							  DeviceID,
							  CodeChannel )
				  SELECT	@Code,
							UserID,
							@BeginDate,
							@EndDate,
							GETDATE(),
							0,
							@Promotiontype,
							@Description,
							0,
							0,
							RuleId,
							NULL,
							@CodeChannel
				  FROM		#tmp_users
				  WHERE		[No] = @Index;

				  IF ( @@ROWCOUNT > 0 )
				  BEGIN
						INSERT	INTO Gungnir..TempCouponActivity
								( ActivityNo,
								  Code,
								  UserId,
								  StartTime,
								  EndTime,
								  OrderId,
								  SendSMSTimes,
								  SendSMSDate,
								  CreatedTime )
						SELECT	1,
								@Code,
								UserID,
								@BeginDate,
								@EndDate,
								OrderId,
								0,
								NULL,
								GETDATE()
						FROM	#tmp_users
						WHERE	[No] = @Index;
				  END;
    
				  COMMIT TRAN tt;
				  SET @Index = @Index + 1; 
			END TRY
			BEGIN CATCH 
				  ROLLBACK TRAN tt;
				  SET @Index = @Index + 1;
			END CATCH; 

			DROP TABLE #tmp_users;
	  END;