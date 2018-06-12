USE [Gungnir]
GO
IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   ID = OBJECT_ID(N'[dbo].[ProMotionCode_HalfPriceQuan]')
                    AND OBJECTPROPERTY(ID, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].[ProMotionCode_HalfPriceQuan]
GO
/*=============================================================
--FuncDes: 抓取参与对折活动订单信息,用于发放活动半价券
--Author	ModifyDate	Reason
--huhengxing   2016-05-25	create
=============================================================*/
CREATE PROC [dbo].[ProMotionCode_HalfPriceQuan]
AS
    SELECT	P.RuleID,
		O.UserID,
		O.UserTel,
		O.SumMoney,
		O.PKID AS OrderID
FROM	Gungnir..tbl_Order AS O WITH ( NOLOCK )
JOIN	Gungnir..tbl_PromotionCode AS P WITH ( NOLOCK )
		ON O.PKID = P.OrderId
WHERE	P.GetRuleID=83 
		AND O.Status = N'3Installed' 
        AND NOT EXISTS ( SELECT	1
						 FROM	tbl_PromotionCode AS PC WITH ( NOLOCK )
						 WHERE	PC.UserId = O.UserID
								AND PC.RuleID = 23
								AND PC.BatchID = O.PKID
								AND PC.CodeChannel = N'保养活动半价券' )