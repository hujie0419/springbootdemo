USE Gungnir

GO
IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   ID = OBJECT_ID(N'[dbo].[CouponJob_UpdateSMSTimesByCode]')
                    AND OBJECTPROPERTY(ID, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].[CouponJob_UpdateSMSTimesByCode]
GO
/*=============================================================
--FuncDes: 更新sms time
--Author	ModifyDate	Reason
--liuchao   2016-03-25	Create

EXEC Gungnir.dbo.CouponJob_UpdateSMSTimesByCode '209516560,1248054880'
=============================================================*/

CREATE PROCEDURE [dbo].[CouponJob_UpdateSMSTimesByCode] ( @Codes NVARCHAR(MAX) )
AS
    WITH    codeList
              AS ( SELECT   *
                   FROM     Gungnir.dbo.SplitString(@Codes, ',', 1)
                 )
        UPDATE  TempCouponActivity
        SET     SendSMSTimes += 1 ,
                SendSMSDate = GETDATE()
        FROM    Gungnir..TempCouponActivity cr
                JOIN codeList cl ON cr.Code = cl.Item;