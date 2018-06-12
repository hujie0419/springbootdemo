using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using System.Data;
using System.Data.SqlClient;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DALGetPromotionActivityConfig
    {

        private static AsyncDbHelper db = null;
        public DALGetPromotionActivityConfig()
        {
            db = DbHelper.CreateDefaultDbHelper();
        }


        public bool Add(SE_GetPromotionActivityConfig model)
        {
            bool result = false;
            StringBuilder sql = new StringBuilder();
            try
            {
                db.BeginTransaction();
                sql.Append(@" INSERT INTO Configuration.dbo.SE_GetPromotionActivityConfig
         ( ID ,
           ActivityName ,
           StartDateTime ,
           EndDateTime ,
           Channel ,
           Status ,
           TopBanner ,
           BottomBanner ,
           NumberCodeTimes ,
           ChartCodeTimes ,
           AutoCompleteTimes ,
           CodeErrorTimes ,
           LimitPhoneHourse ,
           NewUserValidMode ,
           SuccessfulTopBanner ,
           SuccessfulCenterBanner ,
           SuccessfulBottomBanner ,
           SuccessfulIOSUrl ,
           SuccessfulAndroidUrl ,
           SuccessfulJumpMode ,
           SuccessfulWaitTime ,
           FailTopBanner ,
           FailCenterBanner ,
           FailBottomBanner ,
           FailIOSUrl ,
           FailAndroidUrl ,
           FailJumpMode ,
           FailWaitTime ,
           CreateDateTime ,
           UpdateDateTime,
           IsNewUser,
           IsNeedCode,
            IsPostion,IsSendMsg,SendMsg,
            NewBottomBanner,NewPageIOSUrl,NewPageAndroidUrl,NewIOSUrl,NewAndroidUrl,SuccessfulPageIOSUrl,SuccessfulPageAndroidUrl,FailPageIOSUrl,FailPageAndroidUrl,TipStyle,NewJumpMode,NewWaitTime,
            NewUserTip,OldUserTip,ActivityNoStartTip,ActivityOverTip,CouponTip,PageTip,LimitUserTypeTip,AlreadyHadTip,DefaultTip,BlackTip,CardChannel,CardConsumedURL,CardExpireURL,
            CardGiftingURL,CardGiftTimeOutURL,CardDeleteURL,CardUnavailableURL,CardInvalidSerialCodeURL,TokenAccessFailedURL,CreatorUser,UpdateUser
         )");
                sql.Append(@" VALUES  ( @ID , -- ID - uniqueidentifier
           @ActivityName , -- ActivityName - nvarchar(100)
           @StartDateTime , -- StartDateTime - datetime
           @EndDateTime , -- EndDateTime - datetime
           @Channel, -- Channel - nvarchar(50)
           @Status , -- Status - bigint
           @TopBanner , -- TopBanner - nvarchar(500)
           @BottomBanner , -- BottomBanner - nvarchar(500)
           @NumberCodeTimes , -- NumberCodeTimes - int
           @ChartCodeTimes , -- ChartCodeTimes - int
           @AutoCompleteTimes , -- AutoCompleteTimes - int
           @CodeErrorTimes , -- CodeErrorTimes - int
           @LimitPhoneHourse, -- LimitPhoneHourse - int
           @NewUserValidMode , -- NewUserValidMode - int
           @SuccessfulTopBanner , -- SuccessfulTopBanner - nvarchar(500)
           @SuccessfulCenterBanner , -- SuccessfulCenterBanner - nvarchar(500)
           @SuccessfulBottomBanner , -- SuccessfulBottomBanner - nvarchar(500)
           @SuccessfulIOSUrl , -- SuccessfulIOSUrl - nvarchar(100)
           @SuccessfulAndroidUrl , -- SuccessfulAndroidUrl - nvarchar(100)
           @SuccessfulJumpMode , -- SuccessfulJumpMode - int
           @SuccessfulWaitTime , -- SuccessfulWaitTime - int
           @FailTopBanner , -- FailTopBanner - nvarchar(500)
           @FailCenterBanner , -- FailCenterBanner - nvarchar(500)
           @FailBottomBanner , -- FailBottomBanner - nvarchar(500)
           @FailIOSUrl , -- FailIOSUrl - nvarchar(100)
           @FailAndroidUrl , -- FailAndroidUrl - nvarchar(100)
           @FailJumpMode , -- FailJumpMode - int
           @FailWaitTime , -- FailWaitTime - int
           GETDATE() , -- CreateDateTime - datetime
           GETDATE(),  -- UpdateDateTime - datetime
           @IsNewUser,
           @IsNeedCode,
            @IsPostion,@IsSendMsg,@SendMsg,
                 @NewBottomBanner,@NewPageIOSUrl,@NewPageAndroidUrl,@NewIOSUrl,@NewAndroidUrl,@SuccessfulPageIOSUrl,@SuccessfulPageAndroidUrl,@FailPageIOSUrl,@FailPageAndroidUrl,@TipStyle,@NewJumpMode,@NewWaitTime,
             @NewUserTip,@OldUserTip,@ActivityNoStartTip,@ActivityOverTip,@CouponTip,@PageTip,@LimitUserTypeTip,@AlreadyHadTip,@DefaultTip,@BlackTip,@CardChannel,@CardConsumedURL,@CardExpireURL,
            @CardGiftingURL,@CardGiftTimeOutURL,@CardDeleteURL,@CardUnavailableURL,@CardInvalidSerialCodeURL,@TokenAccessFailedURL,@CreatorUser,@UpdateUser
         )");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql.ToString();
                model.ID = Guid.NewGuid();
                cmd.Parameters.AddWithValue("@ID", model.ID.Value);
                cmd.Parameters.AddWithValue("@ActivityName", model.ActivityName);
                cmd.Parameters.AddWithValue("@StartDateTime", model.StartDateTime);
                cmd.Parameters.AddWithValue("@EndDateTime", model.EndDateTime);
                cmd.Parameters.AddWithValue("@Channel", model.Channel);
                cmd.Parameters.AddWithValue("@Status", model.Status);
                cmd.Parameters.AddWithValue("@TopBanner", model.TopBanner);
                cmd.Parameters.AddWithValue("@BottomBanner", model.BottomBanner);
                cmd.Parameters.AddWithValue("@NumberCodeTimes", model.NumberCodeTimes);
                cmd.Parameters.AddWithValue("@ChartCodeTimes", model.ChartCodeTimes);
                cmd.Parameters.AddWithValue("@AutoCompleteTimes", model.AutoCompleteTimes);
                cmd.Parameters.AddWithValue("@CodeErrorTimes", model.CodeErrorTimes);
                cmd.Parameters.AddWithValue("@LimitPhoneHourse", model.LimitPhoneHourse);
                cmd.Parameters.AddWithValue("@NewUserValidMode", model.NewUserValidMode);
                cmd.Parameters.AddWithValue("@SuccessfulTopBanner", model.SuccessfulTopBanner);
                cmd.Parameters.AddWithValue("@SuccessfulCenterBanner", model.SuccessfulCenterBanner);
                cmd.Parameters.AddWithValue("@SuccessfulBottomBanner", model.SuccessfulBottomBanner);
                cmd.Parameters.AddWithValue("@SuccessfulIOSUrl", model.SuccessfulIOSUrl);
                cmd.Parameters.AddWithValue("@SuccessfulAndroidUrl", model.SuccessfulAndroidUrl);
                cmd.Parameters.AddWithValue("@SuccessfulJumpMode", model.SuccessfulJumpMode);
                cmd.Parameters.AddWithValue("@SuccessfulWaitTime", model.SuccessfulWaitTime);
                cmd.Parameters.AddWithValue("@FailTopBanner", model.FailTopBanner);
                cmd.Parameters.AddWithValue("@FailCenterBanner", model.FailCenterBanner);
                cmd.Parameters.AddWithValue("@FailBottomBanner", model.FailBottomBanner);
                cmd.Parameters.AddWithValue("@FailIOSUrl", model.FailIOSUrl);
                cmd.Parameters.AddWithValue("@FailAndroidUrl", model.FailAndroidUrl);
                cmd.Parameters.AddWithValue("@FailJumpMode", model.FailJumpMode);
                cmd.Parameters.AddWithValue("@FailWaitTime", model.FailWaitTime);
                cmd.Parameters.AddWithValue("@IsNewUser", model.IsNewUser);
                cmd.Parameters.AddWithValue("@IsNeedCode", model.IsNeedCode);
                cmd.Parameters.AddWithValue("@IsPostion", model.IsPostion);
                cmd.Parameters.AddWithValue("@IsSendMsg", model.IsSendMsg);
                cmd.Parameters.AddWithValue("@SendMsg", model.SendMsg);
                cmd.Parameters.AddWithValue("@NewBottomBanner", model.NewBottomBanner);
                cmd.Parameters.AddWithValue("@NewPageIOSUrl", model.NewPageIOSUrl);
                cmd.Parameters.AddWithValue("@NewPageAndroidUrl", model.NewPageAndroidUrl);
                cmd.Parameters.AddWithValue("@NewIOSUrl", model.NewIOSUrl);
                cmd.Parameters.AddWithValue("@NewAndroidUrl", model.NewAndroidUrl);
                cmd.Parameters.AddWithValue("@SuccessfulPageAndroidUrl", model.SuccessfulPageAndroidUrl);
                cmd.Parameters.AddWithValue("@SuccessfulPageIOSUrl", model.SuccessfulPageIOSUrl);
                cmd.Parameters.AddWithValue("@FailPageAndroidUrl", model.FailPageAndroidUrl);
                cmd.Parameters.AddWithValue("@FailPageIOSUrl", model.FailPageIOSUrl);
                cmd.Parameters.AddWithValue("@TipStyle", model.TipStyle);
                cmd.Parameters.AddWithValue("@NewJumpMode", model.NewJumpMode);
                cmd.Parameters.AddWithValue("@NewWaitTime",model.NewWaitTime);
                cmd.Parameters.AddWithValue("@NewUserTip", model.NewUserTip);
                cmd.Parameters.AddWithValue("@OldUserTip", model.OldUserTip);
                cmd.Parameters.AddWithValue("@ActivityNoStartTip", model.ActivityNoStartTip);
                cmd.Parameters.AddWithValue("@ActivityOverTip", model.ActivityOverTip);
                cmd.Parameters.AddWithValue("@CouponTip", model.CouponTip);
                cmd.Parameters.AddWithValue("@PageTip", model.PageTip);
                cmd.Parameters.AddWithValue("@LimitUserTypeTip", model.LimitUserTypeTip);
                cmd.Parameters.AddWithValue("@AlreadyHadTip", model.AlreadyHadTip);
                cmd.Parameters.AddWithValue("@DefaultTip", model.DefaultTip);
                cmd.Parameters.AddWithValue("@BlackTip", model.BlackTip);

                cmd.Parameters.AddWithValue("@CardChannel",model.CardChannel);
                cmd.Parameters.AddWithValue("@CardConsumedURL", model.CardConsumedURL);
                cmd.Parameters.AddWithValue("@CardExpireURL",model.CardExpireURL);
                cmd.Parameters.AddWithValue("@CardGiftingURL",model.CardGiftingURL);
                cmd.Parameters.AddWithValue("@CardGiftTimeOutURL", model.CardGiftTimeOutURL);
                cmd.Parameters.AddWithValue("@CardDeleteURL",model.CardDeleteURL);
                cmd.Parameters.AddWithValue("@CardUnavailableURL",model.CardUnavailableURL);
                cmd.Parameters.AddWithValue("@CardInvalidSerialCodeURL",model.CardInvalidSerialCodeURL);
                cmd.Parameters.AddWithValue("@TokenAccessFailedURL",model.TokenAccessFailedURL);
                cmd.Parameters.AddWithValue("@CreatorUser", model.CreatorUser);
                cmd.Parameters.AddWithValue("@UpdateUser",model.UpdateUser);
                db.ExecuteNonQuery(cmd);

                foreach (var item in model.CouponItems)
                {
                    item.FK_GetPromotionActivityID = model.ID.Value;
                    item.Status = true;
                    InsertCouponInfo(db, item);
                }

                db.Commit();
                result = true;

            }
            catch (Exception em)
            {
                db.Rollback();
                result = false;
            }
            finally
            {
                db.Dispose();
            }
            return result;
        }

        public DataTable GetList()
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                using (SqlCommand cmd = new SqlCommand(@"SELECT * FROM Configuration.dbo.SE_GetPromotionActivityConfig (NOLOCK)  ORDER BY CreateDateTime DESC  "))
                {
                    return dbHelper.ExecuteDataTable(cmd);
                }
            }
        }

        public int GetCouponHad(Guid ID)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                using (SqlCommand cmd = new SqlCommand(@" SELECT COUNT(*) FROM SystemLog.dbo.tbl_GetPromotionActivityLog (NOLOCK) WHERE ActivityID=@ActivityID "))
                {
                    cmd.Parameters.AddWithValue("@ActivityID", ID);
                    var obj = dbHelper.ExecuteScalar(cmd);
                    return obj == null ? 0 : Convert.ToInt32(obj);
                }
            }
        }

        public DataTable GetEntity(Guid ID)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                using (SqlCommand cmd = new SqlCommand(@" SELECT TOP 1 * FROM Configuration.dbo.SE_GetPromotionActivityConfig (NOLOCK)  WHERE ID=@ID "))
                {
                    cmd.Parameters.AddWithValue("@ID", ID);
                    return dbHelper.ExecuteDataTable(cmd);
                }
            }
        }


        public DataTable GetCouponInfo(Guid ID)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                using (SqlCommand cmd = new SqlCommand(@"SELECT A.FK_GetPromotionActivityID,A.GetRuleID,A.GetRuleGUID,
                A.VerificationMode, G.Term AS ValidDays, G.ValiStartDate AS ValidStartDateTime, G.ValiEndDate AS ValidEndDateTime,
                    G.SupportUserRange AS UserType, G.Description, G.Discount, G.Minmoney, G.SingleQuantity, G.Quantity, A.Status, A.GetUserType
                    FROM Configuration.dbo.SE_GetPromotionActivityCouponInfoConfig AS A WITH(NOLOCK)
                JOIN Activity..tbl_GetCouponRules AS G WITH(NOLOCK) ON A.GetRuleGUID = G.GetRuleGUID AND A.Status = 1
                WHERE FK_GetPromotionActivityID = @ID  "))
                {
                    cmd.Parameters.AddWithValue("@ID", ID);
                    return dbHelper.ExecuteDataTable(cmd);
                }
            }

        }


        public bool Update(SE_GetPromotionActivityConfig model)
        {
            bool result = false;
            try
            {
                StringBuilder sql = new StringBuilder(@"UPDATE configuration.dbo.SE_GetPromotionActivityConfig SET 
           ActivityName =@ActivityName,
          StartDateTime =@StartDateTime,
          EndDateTime =@EndDateTime,
          Channel =@Channel,
          [Status] =@Status ,
          TopBanner =@TopBanner,
          BottomBanner =@BottomBanner,
          NumberCodeTimes =@NumberCodeTimes,
          ChartCodeTimes =@ChartCodeTimes,
          AutoCompleteTimes =@AutoCompleteTimes,
          CodeErrorTimes =@CodeErrorTimes,
          LimitPhoneHourse =@LimitPhoneHourse,
          NewUserValidMode =@NewUserValidMode,
          SuccessfulTopBanner =@SuccessfulTopBanner,
          SuccessfulCenterBanner =@SuccessfulCenterBanner,
          SuccessfulBottomBanner=@SuccessfulBottomBanner ,
          SuccessfulIOSUrl =@SuccessfulIOSUrl,
          SuccessfulAndroidUrl =@SuccessfulAndroidUrl,
          SuccessfulJumpMode =@SuccessfulJumpMode,
          SuccessfulWaitTime=@SuccessfulWaitTime ,
          FailTopBanner =@FailTopBanner,
          FailCenterBanner =@FailCenterBanner,
          FailBottomBanner =@FailBottomBanner,
          FailIOSUrl =@FailIOSUrl,
          FailAndroidUrl =@FailAndroidUrl,
          FailJumpMode =@FailJumpMode,
          FailWaitTime =@FailWaitTime,
          UpdateDateTime=GETDATE(),
          IsNewUser=@IsNewUser,
          IsNeedCode=@IsNeedCode,
             IsPostion=@IsPostion,IsSendMsg=@IsSendMsg,SendMsg=@SendMsg,
          NewBottomBanner=@NewBottomBanner,NewPageIOSUrl=@NewPageIOSUrl,NewPageAndroidUrl=@NewPageAndroidUrl,
          NewIOSUrl=@NewIOSUrl,NewAndroidUrl=@NewAndroidUrl,SuccessfulPageIOSUrl=@SuccessfulPageIOSUrl,SuccessfulPageAndroidUrl=@SuccessfulPageAndroidUrl,FailPageIOSUrl=@FailPageIOSUrl,FailPageAndroidUrl=@FailPageAndroidUrl,TipStyle=@TipStyle,NewJumpMode=@NewJumpMode,NewWaitTime=@NewWaitTime,
           NewUserTip=@NewUserTip,OldUserTip=@OldUserTip,ActivityNoStartTip=@ActivityNoStartTip,ActivityOverTip=@ActivityOverTip,CouponTip=@CouponTip,PageTip=@PageTip,LimitUserTypeTip=@LimitUserTypeTip,AlreadyHadTip=@AlreadyHadTip,DefaultTip=@DefaultTip,BlackTip=@BlackTip,
          CardChannel=@CardChannel,CardConsumedURL=@CardConsumedURL,CardExpireURL=@CardExpireURL,
            CardGiftingURL=@CardGiftingURL,CardGiftTimeOutURL=@CardGiftTimeOutURL,CardDeleteURL=@CardDeleteURL,
          CardUnavailableURL=@CardUnavailableURL,CardInvalidSerialCodeURL=@CardInvalidSerialCodeURL,TokenAccessFailedURL=@TokenAccessFailedURL,UpdateUser=@UpdateUser
		  WHERE ID=@ID
        ");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("@ActivityName", model.ActivityName);
                cmd.Parameters.AddWithValue("@StartDateTime", model.StartDateTime);
                cmd.Parameters.AddWithValue("@EndDateTime", model.EndDateTime);
                cmd.Parameters.AddWithValue("@Channel", model.Channel);
                cmd.Parameters.AddWithValue("@Status", model.Status);
                cmd.Parameters.AddWithValue("@TopBanner", model.TopBanner);
                cmd.Parameters.AddWithValue("@BottomBanner", model.BottomBanner);
                cmd.Parameters.AddWithValue("@NumberCodeTimes", model.NumberCodeTimes);
                cmd.Parameters.AddWithValue("@ChartCodeTimes", model.ChartCodeTimes);
                cmd.Parameters.AddWithValue("@AutoCompleteTimes", model.AutoCompleteTimes);
                cmd.Parameters.AddWithValue("@CodeErrorTimes", model.CodeErrorTimes);
                cmd.Parameters.AddWithValue("@LimitPhoneHourse", model.LimitPhoneHourse);
                cmd.Parameters.AddWithValue("@NewUserValidMode", model.NewUserValidMode);
                cmd.Parameters.AddWithValue("@SuccessfulTopBanner", model.SuccessfulTopBanner);
                cmd.Parameters.AddWithValue("@SuccessfulCenterBanner", model.SuccessfulCenterBanner);
                cmd.Parameters.AddWithValue("@SuccessfulBottomBanner", model.SuccessfulBottomBanner);
                cmd.Parameters.AddWithValue("@SuccessfulIOSUrl", model.SuccessfulIOSUrl);
                cmd.Parameters.AddWithValue("@SuccessfulAndroidUrl", model.SuccessfulAndroidUrl);
                cmd.Parameters.AddWithValue("@SuccessfulJumpMode", model.SuccessfulJumpMode);
                cmd.Parameters.AddWithValue("@SuccessfulWaitTime", model.SuccessfulWaitTime);
                cmd.Parameters.AddWithValue("@FailTopBanner", model.FailTopBanner);
                cmd.Parameters.AddWithValue("@FailCenterBanner", model.FailCenterBanner);
                cmd.Parameters.AddWithValue("@FailBottomBanner", model.FailBottomBanner);
                cmd.Parameters.AddWithValue("@FailIOSUrl", model.FailIOSUrl);
                cmd.Parameters.AddWithValue("@FailAndroidUrl", model.FailAndroidUrl);
                cmd.Parameters.AddWithValue("@FailJumpMode", model.FailJumpMode);
                cmd.Parameters.AddWithValue("@FailWaitTime", model.FailWaitTime);
                cmd.Parameters.AddWithValue("@IsNewUser", model.IsNewUser);
                cmd.Parameters.AddWithValue("@IsNeedCode", model.IsNeedCode);
                cmd.Parameters.AddWithValue("@IsPostion", model.IsPostion);
                cmd.Parameters.AddWithValue("@IsSendMsg", model.IsSendMsg);
                cmd.Parameters.AddWithValue("@SendMsg", model.SendMsg);
                cmd.Parameters.AddWithValue("@NewBottomBanner", model.NewBottomBanner);
                cmd.Parameters.AddWithValue("@NewPageIOSUrl", model.NewPageIOSUrl);
                cmd.Parameters.AddWithValue("@NewPageAndroidUrl", model.NewPageAndroidUrl);
                cmd.Parameters.AddWithValue("@NewIOSUrl", model.NewIOSUrl);
                cmd.Parameters.AddWithValue("@NewAndroidUrl", model.NewAndroidUrl);
                cmd.Parameters.AddWithValue("@SuccessfulPageAndroidUrl", model.SuccessfulPageAndroidUrl);
                cmd.Parameters.AddWithValue("@SuccessfulPageIOSUrl", model.SuccessfulPageIOSUrl);
                cmd.Parameters.AddWithValue("@FailPageAndroidUrl", model.FailPageAndroidUrl);
                cmd.Parameters.AddWithValue("@FailPageIOSUrl", model.FailPageIOSUrl);
                cmd.Parameters.AddWithValue("@TipStyle", model.TipStyle);
                cmd.Parameters.AddWithValue("@NewJumpMode", model.NewJumpMode);
                cmd.Parameters.AddWithValue("@NewWaitTime", model.NewWaitTime);
                cmd.Parameters.AddWithValue("@NewUserTip", model.NewUserTip);
                cmd.Parameters.AddWithValue("@OldUserTip", model.OldUserTip);
                cmd.Parameters.AddWithValue("@ActivityNoStartTip", model.ActivityNoStartTip);
                cmd.Parameters.AddWithValue("@ActivityOverTip", model.ActivityOverTip);
                cmd.Parameters.AddWithValue("@CouponTip", model.CouponTip);
                cmd.Parameters.AddWithValue("@PageTip", model.PageTip);
                cmd.Parameters.AddWithValue("@LimitUserTypeTip", model.LimitUserTypeTip);
                cmd.Parameters.AddWithValue("@AlreadyHadTip", model.AlreadyHadTip);
                cmd.Parameters.AddWithValue("@DefaultTip", model.DefaultTip);
                cmd.Parameters.AddWithValue("@BlackTip", model.BlackTip);
                cmd.Parameters.AddWithValue("@ID", model.ID.Value);

                cmd.Parameters.AddWithValue("@CardChannel", model.CardChannel);
                cmd.Parameters.AddWithValue("@CardConsumedURL", model.CardConsumedURL);
                cmd.Parameters.AddWithValue("@CardExpireURL", model.CardExpireURL);
                cmd.Parameters.AddWithValue("@CardGiftingURL", model.CardGiftingURL);
                cmd.Parameters.AddWithValue("@CardGiftTimeOutURL", model.CardGiftTimeOutURL);
                cmd.Parameters.AddWithValue("@CardDeleteURL", model.CardDeleteURL);
                cmd.Parameters.AddWithValue("@CardUnavailableURL", model.CardUnavailableURL);
                cmd.Parameters.AddWithValue("@CardInvalidSerialCodeURL", model.CardInvalidSerialCodeURL);
                cmd.Parameters.AddWithValue("@TokenAccessFailedURL", model.TokenAccessFailedURL);
                cmd.Parameters.AddWithValue("@UpdateUser",model.UpdateUser);

                db.ExecuteNonQuery(cmd);

                DeleteCouponInfo(db, model.ID.Value);

                foreach (var item in model.CouponItems)
                {
                    item.FK_GetPromotionActivityID = model.ID.Value;
                    item.Status = true;
                    InsertCouponInfo(db, item);
                }


                db.Commit();
                result = true;
            }
            catch (Exception em)
            {
                db.Rollback();
                result = false;
            }
            finally
            {
                db.Dispose();
            }

            return result;
        }


        public bool Delete(Guid ID)
        {
            bool result = false;
            try
            {
                db.BeginTransaction();
                StringBuilder sql = new StringBuilder("DELETE FROM Configuration.dbo.SE_GetPromotionActivityConfig WHERE ID=@ID ");
                SqlCommand cmd = new SqlCommand(sql.ToString());
                cmd.Parameters.AddWithValue("@ID", ID);
                db.ExecuteNonQuery(cmd);
                sql = new StringBuilder();
                sql.Append("DELETE FROM Configuration.dbo.SE_GetPromotionActivityCouponInfoConfig WHERE FK_GetPromotionActivityID=@FK_GetPromotionActivityID ");
                cmd = new SqlCommand(sql.ToString());
                cmd.Parameters.AddWithValue("@FK_GetPromotionActivityID", ID);
                db.ExecuteNonQuery(cmd);
                db.Commit();
                result = true;
            }
            catch (Exception em)
            {
                db.Rollback();
            }
            finally
            {
                db.Dispose();
            }
            return result;
        }


        private void InsertCouponInfo(AsyncDbHelper db, SE_GetPromotionActivityCouponInfoConfig item)
        {
            StringBuilder sql = new StringBuilder();
            sql = new StringBuilder(@"INSERT INTO Configuration.dbo.SE_GetPromotionActivityCouponInfoConfig
        ( FK_GetPromotionActivityID ,
          GetRuleID ,
          GetRuleGUID ,
          VerificationMode ,
          ValidDays ,
          ValidStartDateTime ,
          ValidEndDateTime ,
          UserType ,
          Description ,
          Discount ,
          MinMoney ,
          SingleQuantity ,
          Quantity,
          [Status],
            GetUserType
        )
VALUES  ( @FK_GetPromotionActivityID , -- FK_GetPromotionActivityID - uniqueidentifier
          @GetRuleID , -- GetRuleID - int
          @GetRuleGUID , -- GetRuleGUID - uniqueidentifier
          @VerificationMode , -- VerificationMode - int
          @ValidDays , -- ValidDays - int
          @ValidStartDateTime , -- ValidStartDateTime - datetime
          @ValidEndDateTime , -- ValidEndDateTime - datetime
          @UserType , -- UserType - int
          @Description , -- Description - nvarchar(50)
          @Discount , -- Discount - money
          @MinMoney , -- MinMoney - money
          @SingleQuantity , -- SingleQuantity - int
          @Quantity,  -- Quantity - int
          @Status,
          @GetUserType
        )");
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("@FK_GetPromotionActivityID", item.FK_GetPromotionActivityID);
                cmd.Parameters.AddWithValue("@GetRuleID", item.GetRuleID);
                cmd.Parameters.AddWithValue("@GetRuleGUID", item.GetRuleGUID);
                cmd.Parameters.AddWithValue("@VerificationMode", item.VerificationMode);
                cmd.Parameters.AddWithValue("@ValidDays", item.ValidDays);
                cmd.Parameters.AddWithValue("@ValidStartDateTime", item.ValidStartDateTime);
                cmd.Parameters.AddWithValue("@ValidEndDateTime", item.ValidEndDateTime);
                cmd.Parameters.AddWithValue("@UserType", item.UserType);
                cmd.Parameters.AddWithValue("@Description", item.Description);
                cmd.Parameters.AddWithValue("@Discount", item.Discount);
                cmd.Parameters.AddWithValue("@MinMoney", item.MinMoney);
                cmd.Parameters.AddWithValue("@SingleQuantity", item.SingleQuantity);
                cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                cmd.Parameters.AddWithValue("@Status", item.Status);
                cmd.Parameters.AddWithValue("@GetUserType", item.GetUserType);
                db.ExecuteNonQuery(cmd);
            }
        }

        private void UpdateCouponInfo(AsyncDbHelper db, SE_GetPromotionActivityCouponInfoConfig item)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"UPDATE Configuration.dbo.SE_GetPromotionActivityCouponInfoConfig
          GetRuleID =@GetRuleID,
          GetRuleGUID=@GetRuleGUID ,
          VerificationMode=@VerificationMode ,
          ValidDays =@ValidDays,
          ValidStartDateTime =@ValidStartDateTime,
          ValidEndDateTime =@ValidEndDateTime,
          UserType =@UserType,
          [Description]=@Description,
          Discount=@Discount ,
          MinMoney =@MinMoney,
          SingleQuantity =@SingleQuantity,
          Quantity=@Quantity
WHERE   FK_GetPromotionActivityID=@FK_GetPromotionActivityID ");
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = sql.ToString();
                cmd.Parameters.AddWithValue("@FK_GetPromotionActivityID", item.FK_GetPromotionActivityID);
                cmd.Parameters.AddWithValue("@GetRuleID", item.GetRuleID);
                cmd.Parameters.AddWithValue("@GetRuleGUID", item.GetRuleGUID);
                cmd.Parameters.AddWithValue("@VerificationMode", item.VerificationMode);
                cmd.Parameters.AddWithValue("@ValidDays", item.ValidDays);
                cmd.Parameters.AddWithValue("@ValidStartDateTime", item.ValidStartDateTime);
                cmd.Parameters.AddWithValue("@ValidEndDateTime", item.ValidEndDateTime);
                cmd.Parameters.AddWithValue("@UserType", item.UserType);
                cmd.Parameters.AddWithValue("@Description", item.Description);
                cmd.Parameters.AddWithValue("@Discount", item.Discount);
                cmd.Parameters.AddWithValue("@MinMoney", item.MinMoney);
                cmd.Parameters.AddWithValue("@SingleQuantity", item.SingleQuantity);
                cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                db.ExecuteNonQuery(cmd);
            }
        }

        private void DeleteCouponInfo(AsyncDbHelper db, Guid ID)
        {
            //DELETE FROM Configuration.dbo.SE_GetPromotionActivityCouponInfoConfig WHERE FK_GetPromotionActivityID=@FK_GetPromotionActivityID 
            using (SqlCommand cmd = new SqlCommand("UPDATE Configuration.dbo.SE_GetPromotionActivityCouponInfoConfig SET Status=0 WHERE  FK_GetPromotionActivityID=@FK_GetPromotionActivityID "))
            {
                cmd.Parameters.AddWithValue("@FK_GetPromotionActivityID", ID);
                db.ExecuteNonQuery(cmd);
            }
        }

        private bool ExistCouponInfo(Guid ID, int ruleID)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 COUNT(*) FROM Configuration.dbo.SE_GetPromotionActivityCouponInfoConfig WHERE FK_GetPromotionActivityID=@FK_GetPromotionActivityID AND GetRuleID=@GetRuleID "))
            {
                cmd.Parameters.AddWithValue("@FK_GetPromotionActivityID", ID);
                cmd.Parameters.AddWithValue("@GetRuleID", ruleID);
                return (int)db.ExecuteScalar(cmd) > 0;
            }
        }


        public DataTable GetCouponInfoValidate(Guid guid)
        {
            using (SqlCommand cmd = new SqlCommand(@" SELECT PKID AS GetRuleID,GetRuleGUID AS GetRuleGUID,
 CASE Term WHEN NULL THEN 1 ELSE 0 END AS VerificationMode,
 Term AS ValidDays,
 ValiStartDate AS ValidStartDateTime,
 ValiEndDate AS ValidEndDateTime,
  SupportUserRange AS UserType,
  [Description] AS[Description],
  Discount,
  MinMoney,
  SingleQuantity,
  Quantity FROM Activity.dbo.tbl_GetCouponRules (NOLOCK) WHERE GetRuleGUID=@GetRuleGUID "))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@GetRuleGUID", guid);
                return db.ExecuteDataTable(cmd);
            }

        }



        public int GetPromotionActivityCountByID(Guid ID)
        {
            using (var dbHelper = new SqlDbHelper(ConnectionHelper.GetDecryptConn("Gungnir_AlwaysOnRead")))
            {
                string sql = string.Format(" SELECT count(1) FROM SystemLog.dbo.tbl_GetPromotionActivityLog (NOLOCK) WHERE ActivityID=@ActivityID ");
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@ActivityID", ID);
                var obj = dbHelper.ExecuteScalar(cmd);
                if (obj == null)
                    return 0;
                else
                    return (int)obj;
            }
        }


        ~DALGetPromotionActivityConfig()
        {
            db.Dispose();
        }

    }
}
