USE [Gungnir]
GO
IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   ID = OBJECT_ID(N'[dbo].[CouponJob_GetNotBaoYangUserDetailNeedSend]')
                    AND OBJECTPROPERTY(ID, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].[CouponJob_GetNotBaoYangUserDetailNeedSend]
GO
/*=============================================================
--FuncDes: 抓取需要发送的非保养用户信息
--Author	ModifyDate	Reason
--
=============================================================*/

CREATE PROCEDURE [dbo].[CouponJob_GetNotBaoYangUserDetailNeedSend]
AS
    SELECT  DISTINCT
            uo.UserID ,
            uo.u_mobile_number ,
            pui.Device_Tokens,
			pui.Channel,
            uo.u_email_address,
			cr.Code
    FROM    ExciteCouponsRecord cr WITH(NOLOCK)
            JOIN tbl_PromotionCode pc WITH(NOLOCK) ON cr.Code = pc.Code
            JOIN Tuhu_profiles..UserObject uo WITH(NOLOCK) ON cr.UserId = uo.UserID
			LEFT JOIN Tuhu_profiles..Push_UserInfo pui WITH(NOLOCK) ON pui.UserID=cr.UserId
    WHERE   pc.Status = 0
	        AND cr.Type=1
            AND  DATEDIFF(MONTH, cr.StartTime, GETDATE()) = 0 AND cr.SendTimes=0


		
GO


