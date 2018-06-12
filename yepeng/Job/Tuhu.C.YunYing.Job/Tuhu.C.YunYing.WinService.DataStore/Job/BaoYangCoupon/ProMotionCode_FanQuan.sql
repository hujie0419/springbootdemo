USE [Gungnir]
GO
IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   ID = OBJECT_ID(N'[dbo].[ProMotionCode_FanQuan]')
                    AND OBJECTPROPERTY(ID, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].[ProMotionCode_FanQuan]
GO
/*=============================================================
--FuncDes: 抓取使用了免单活动订单信息
--Author	ModifyDate	Reason
--huhengxing   2016-04-13	create
=============================================================*/
CREATE PROC [dbo].[ProMotionCode_FanQuan]
AS
    SELECT	P.RuleID,
		O.UserID,
		O.UserTel,
		O.SumMoney,
		O.PKID AS OrderID
FROM	Gungnir..tbl_Order AS O WITH ( NOLOCK )
JOIN	Gungnir..tbl_PromotionCode AS P WITH ( NOLOCK )
		ON O.PKID = P.OrderId
WHERE	P.GetRuleID = 68
		AND O.Status = N'3Installed' 
        AND NOT EXISTS ( SELECT	1
						 FROM	tbl_PromotionCode AS PC WITH ( NOLOCK )
						 WHERE	PC.UserId = O.UserID
								AND PC.RuleID = 23
								AND PC.BatchID = O.PKID
								AND PC.PromtionName = N'到店保养券' )