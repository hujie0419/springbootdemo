USE Gungnir
GO
IF EXISTS ( SELECT	*
			FROM	dbo.sysobjects
			WHERE	ID = OBJECT_ID(N'[dbo].[JobSchedulerService_SelectPingAnOrder]')
					AND OBJECTPROPERTY(ID, N'IsProcedure') = 1 )
   DROP PROCEDURE [dbo].[JobSchedulerService_SelectPingAnOrder]
GO
/*=============================================================
--FuncDes:	选择平安订单
--Author	ModifyDate		Reason
--pengwei	2014-05-27		Create

EXEC [JobSchedulerService_SelectPingAnOrder] '2014-01-10'
=============================================================*/
CREATE PROCEDURE [dbo].[JobSchedulerService_SelectPingAnOrder]
	   @LastExecuteDateTime DATETIME
AS
SELECT	CO.u_cartype_other,
		CO.u_carno,
		CO.u_cartype_description,
		ZC.CP_Brand,
		ZC.Name,
		ZC.DisplayName,
		ZC.CP_Tire_Width + '/' + ZC.CP_Tire_AspectRatio + 'R' + ZC.CP_Tire_Rim AS Size,
		OL.Num AS Quantity,
		O.UserName,
		O.UserTel,
		O.OrderDatetime,
		O.InstallShop,
		O.PKID AS OrderID,
		O.SumMoney,
		O.InstallMoney,
		OL.PKID AS OrderListID,
		O.Status AS OrderStatus,
		S.Province,
		S.City,
		O.OrderDatetime,
		O.BookDatetime,
		O.InstallDatetime
FROM	tbl_Order AS O
JOIN	tbl_OrderList AS OL
		ON O.PKID = OL.OrderID
LEFT	JOIN Tuhu_profiles..CarObject AS CO
		ON '{' + LOWER(CAST(O.CarID AS NVARCHAR(50))) + '}' = CO.u_car_id
LEFT JOIN Shops AS S
		ON O.InstallShopID = S.PKID
LEFT JOIN Tuhu_productcatalog..[CarPAR_zh-CN] AS ZC
		ON OL.PID = ZC.ProductID + '|' + ZC.VariantID COLLATE Chinese_PRC_CI_AS
WHERE	OrderChannel = N'b平安'
		AND O.InstallType = '1ShopInstall'
		AND (O.Status IN ('1Booked', '2Shipped')
			 AND O.OrderDatetime > @LastExecuteDateTime
			 OR O.Status = '3Installed'
			 AND O.InstallDatetime > @LastExecuteDateTime
			)
GO
