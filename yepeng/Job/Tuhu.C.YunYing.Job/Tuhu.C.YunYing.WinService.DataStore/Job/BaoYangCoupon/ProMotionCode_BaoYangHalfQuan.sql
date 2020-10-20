USE [Gungnir]
GO
IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   ID = OBJECT_ID(N'[dbo].[ProMotionCode_BaoYangHalfQuan]')
                    AND OBJECTPROPERTY(ID, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].[ProMotionCode_BaoYangHalfQuan]
GO
/*=============================================================
--FuncDes: ץȡ����618�����ձ������ȯ�������Ϣ,���ڷ��Ż���ȯ
--Author	ModifyDate	Reason
--huhengxing   2016-06-13	create
=============================================================*/
CREATE PROC [dbo].[ProMotionCode_BaoYangHalfQuan]
AS
    SELECT	P.RuleID,
		O.UserID,
		O.UserTel,
		O.SumMoney,
		O.PKID AS OrderID
FROM	Gungnir..tbl_Order AS O WITH ( NOLOCK )
JOIN	Gungnir..tbl_PromotionCode AS P WITH ( NOLOCK )
		ON O.PKID = P.OrderId
WHERE	P.GetRuleID=269 
		AND O.Status = N'3Installed' 
        AND NOT EXISTS ( SELECT	1
						 FROM	tbl_PromotionCode AS PC WITH ( NOLOCK )
						 WHERE	PC.UserId = O.UserID
								AND PC.RuleID = 23
								AND PC.BatchID = O.PKID
								AND PC.CodeChannel = N'618�����ձ������ȯ�' )