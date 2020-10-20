USE [Gungnir]
GO
IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   ID = OBJECT_ID(N'[dbo].[ProMotionCode_MeiJiYouHalfPriceQuan]')
                    AND OBJECTPROPERTY(ID, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].[ProMotionCode_MeiJiYouHalfPriceQuan]
GO
/*=============================================================
--FuncDes: ץȡ������ϵ���Ͱ�ۻ������Ϣ,���ڷ��Ż��۷�ȯ
--Author	ModifyDate	Reason
--huhengxing   2016-06-30	create
=============================================================*/
CREATE PROC [dbo].[ProMotionCode_MeiJiYouHalfPriceQuan]
AS
    SELECT	P.RuleID,
		O.UserID,
		O.UserTel,
		O.SumMoney,
		O.PKID AS OrderID
FROM	Gungnir..tbl_Order AS O WITH ( NOLOCK )
JOIN	Gungnir..tbl_PromotionCode AS P WITH ( NOLOCK )
		ON O.PKID = P.OrderId
WHERE	P.RuleID=1073 AND P.GetRuleID=363 
		AND O.Status = N'3Installed' 
        AND NOT EXISTS ( SELECT	1
						 FROM	tbl_PromotionCode AS PC WITH ( NOLOCK )
						 WHERE	PC.UserId = O.UserID
								AND PC.RuleID = 23
								AND PC.BatchID = O.PKID
								AND PC.CodeChannel = N'��ϵ���Ͱ�۹����ȯ' )