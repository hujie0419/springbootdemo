USE [Gungnir]
GO
IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   ID = OBJECT_ID(N'[dbo].[ProMotionCode_LunGuHalfPriceQuan]')
                    AND OBJECTPROPERTY(ID, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].[ProMotionCode_LunGuHalfPriceQuan]
GO
/*=============================================================
--FuncDes: 抓取参与轮毂半价活动订单信息,用于发放活动半价券
--Author	ModifyDate	Reason
--huhengxing   2016-05-31	create
=============================================================*/
CREATE PROC [dbo].[ProMotionCode_LunGuHalfPriceQuan]
AS
    SELECT	P.RuleID,
		O.UserID,
		O.UserTel,
		O.SumMoney,
		O.PKID AS OrderID
FROM	Gungnir..tbl_Order AS O WITH ( NOLOCK )
JOIN	Gungnir..tbl_PromotionCode AS P WITH ( NOLOCK )
		ON O.PKID = P.OrderId
WHERE	P.RuleID=2436 AND P.GetRuleID=246 
		AND O.PayStatus = N'2Paid' 
        AND NOT EXISTS ( SELECT	1
						 FROM	tbl_PromotionCode AS PC WITH ( NOLOCK )
						 WHERE	PC.UserId = O.UserID
								AND PC.RuleID = 562
								AND PC.BatchID = O.PKID
								AND PC.CodeChannel = N'轮毂活动半价券' )