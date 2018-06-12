USE [Gungnir]
GO
IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   ID = OBJECT_ID(N'[dbo].[CouponJob_UpdatePushTimesByCode]')
                    AND OBJECTPROPERTY(ID, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].[CouponJob_UpdatePushTimesByCode]
GO
/*=============================================================
--FuncDes: 更新优惠券推送次数
--Author	ModifyDate	Reason
--
=============================================================*/

CREATE PROCEDURE [dbo].[CouponJob_UpdatePushTimesByCode] ( @CodeStr NVARCHAR(MAX) )
AS
    WITH    codeList
              AS ( SELECT   *
                   FROM     Gungnir.dbo.SplitString(@CodeStr, ',', 1)
                 )
        UPDATE  ExciteCouponsRecord
        SET     PushTimes+= 1 ,
                PushDate = GETDATE()
        FROM    ExciteCouponsRecord cr
                JOIN codeList cl ON cr.Code = cl.Item;



GO


