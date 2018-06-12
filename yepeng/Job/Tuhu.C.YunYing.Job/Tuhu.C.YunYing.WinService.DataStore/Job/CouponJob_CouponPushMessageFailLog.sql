USE [Gungnir]
GO
IF EXISTS ( SELECT  *
            FROM    dbo.sysobjects
            WHERE   ID = OBJECT_ID(N'[dbo].[CouponJob_CouponPushMessageFailLog]')
                    AND OBJECTPROPERTY(ID, N'IsProcedure') = 1 )
    DROP PROCEDURE [dbo].[CouponJob_CouponPushMessageFailLog]
GO
/*=============================================================
--FuncDes: 插入优惠券信息推送失败的日志
--Author	ModifyDate	Reason
--
=============================================================*/

CREATE  PROCEDURE [dbo].[CouponJob_CouponPushMessageFailLog]
(@UserId UNIQUEIDENTIFIER,
@UserName NVARCHAR(255),
@PhoneNum NVARCHAR(32),
@Channel NVARCHAR(20),
@Status BIT,
@Subject NVARCHAR(50)
)
AS
INSERT INTO [SystemLog]..[CouponPushMessageFailLog]
        ( [UserID] ,
          [UserName] ,
          [Channel] ,
          [Subject] ,
          [Status] ,
          [CreateTime],
		  [PhoneNum]
        )
VALUES  ( @UserId , -- UserID - uniqueidentifier
          @UserName , -- UserName - nvarchar(255)
          @Channel , -- Channel - nvarchar(50)
          @Subject , -- Subject - nvarchar(50)
          @Status , -- Status - bit
          GETDATE(),  -- CreateTime - datetime
		  @PhoneNum
        )

GO


