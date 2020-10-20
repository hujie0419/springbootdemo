USE [Gungnir]
GO
IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   ID = OBJECT_ID(N'[dbo].[ProMotionCode_GodTireQuan]')
                    AND OBJECTPROPERTY(ID, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].[ProMotionCode_GodTireQuan]
GO
/*=============================================================
--FuncDes: ץȡ����618������2000Ԫ��̥ȯ�������Ϣ,���ڷ���2000Ԫ��̥ȯ
--Author	ModifyDate	Reason
--huhengxing   2016-06-13	create
=============================================================*/
CREATE PROC [dbo].[ProMotionCode_GodTireQuan]
AS
    SELECT	P.RuleID,
		O.UserID,
		O.UserTel,
		O.SumMoney,
		O.PKID AS OrderID
FROM	Gungnir..tbl_Order AS O WITH ( NOLOCK )
JOIN	Gungnir..tbl_PromotionCode AS P WITH ( NOLOCK )
		ON O.PKID = P.OrderId
WHERE	P.GetRuleID=270 
		AND O.Status = N'3Installed' 
        AND NOT EXISTS ( SELECT	1
						 FROM	tbl_PromotionCode AS PC WITH ( NOLOCK )
						 WHERE	PC.UserId = O.UserID
								AND PC.BatchID = O.PKID
								AND PC.CodeChannel = N'618������2000Ԫ��̥ȯ�' )