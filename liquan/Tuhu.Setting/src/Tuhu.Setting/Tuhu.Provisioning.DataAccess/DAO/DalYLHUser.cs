using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Common;
using System.Data.SqlClient;
using System.Data;

namespace Tuhu.Provisioning.DataAccess.DAO
{
    public class DalYLHUser
    {
        /// <summary>
        /// 通过手机号获取用户ID
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public string GetUserId(string mobile)
        {
            using (
                var cmd=new SqlCommand(@"select u_user_id 
from [Tuhu_profiles].[dbo].[UserObject]
where [u_mobile_number]=@Mobile"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Mobile", mobile);
                var result = DbHelper.ExecuteScalar(cmd);
                return result == null ? string.Empty : result.ToString();
            }
        }

        public int GetYLHUserInfoPKID(string userid)
        {
            using (
                var cmd = new SqlCommand(@"SELECT pkid 
FROM [Tuhu_profiles].[dbo].[YLH_UserInfo]
WHERE u_user_id=@UserID"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@UserID", userid);
                var result = DbHelper.ExecuteScalar(cmd);
                return result == null ? -1 : Convert.ToInt16(result);
            }
        }


        public int InsertUserToObjectTable(tbl_UserObjectModel user)
        {
            string sql = @"insert into [Tuhu_profiles].[dbo].[UserObject]
(
[u_user_id],
[u_mobile_number],
[u_last_name],
[dt_birthday],
[Category],
[u_email_address]
)
values(
@UserId,
@Mobile,
@UserName,
@UserBirthday,
'YLH',
@UserEmail
)";
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@UserId", user.u_user_id);
                cmd.Parameters.AddWithValue("@Mobile", user.u_mobile_number);
                cmd.Parameters.AddWithValue("@UserName", user.u_last_name);
                cmd.Parameters.AddWithValue("@UserBirthday", user.dt_birthday);
                cmd.Parameters.AddWithValue("@UserEmail", user.u_email_address);

                var result = db.ExecuteNonQuery(cmd);
                return result;
            }
        }

        public int InsertUserObject(tbl_UserObjectModel user)
        {
            string sql = @"insert into Tuhu_profiles..UserObject
                            (
                                u_user_id,
                                u_mobile_number,
                                u_last_name,
                                u_email_address,
                                Category,
                                u_application_name
                            )
                            values
                            (
                                @UserId,
                                @MobileNumber,
                                @UserName,
                                @UserEmail,
                                @Category,
                                @Channel
                            )";
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@UserId", user.u_user_id);
                cmd.Parameters.AddWithValue("@MobileNumber", user.u_mobile_number);
                cmd.Parameters.AddWithValue("@UserName", user.u_last_name);
                cmd.Parameters.AddWithValue("@UserEmail", user.u_email_address);
                cmd.Parameters.AddWithValue("@Category", user.Category);
                cmd.Parameters.AddWithValue("@Channel", user.u_application_name);

                var result = db.ExecuteNonQuery(cmd);
                return result;
            }
        }

        public int InsertYLHUserInfo(YLHUserInfoModel info)
        {
            string sql = @"
insert into [Tuhu_profiles].[dbo].[YLH_UserInfo]
([u_user_id],[MemberNumber],[MemberName],[MemberAddress],[MemberPhone],
[Integration],[CreatedTime],[UpdatedTime],[MemberBirthday],[Tag])
values(
@UserId,
@MemberNumber,
@MemberName,
@MemberAddress,
@MemberPhone,
@Integration,
@CreatedTime,
@UpdatedTime,
@MemberBirthday,
@Tag
)";
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@UserId", info.u_user_id);
                cmd.Parameters.AddWithValue("@MemberNumber", info.MemberNumber);
                cmd.Parameters.AddWithValue("@MemberName", info.MemberName);
                cmd.Parameters.AddWithValue("@MemberAddress", info.MemberAddress);
                cmd.Parameters.AddWithValue("@MemberPhone", info.MemberPhone);
                cmd.Parameters.AddWithValue("@Integration", info.Integration);
                cmd.Parameters.AddWithValue("@CreatedTime", info.CreatedTime);
                cmd.Parameters.AddWithValue("@UpdatedTime", info.UpdatedTime);
                cmd.Parameters.AddWithValue("@MemberBirthday", info.MemberBirthday);
                cmd.Parameters.AddWithValue("@Tag", info.Tag);

                var result = db.ExecuteNonQuery(cmd);
                return result;
            }
        }

        public int InsertYLHVipCardInfo(YLHUserVipCardInfoModel info)
        {
            string sql = @"
insert into [Tuhu_profiles].[dbo].[YLH_UserVipCardInfo]
([u_user_id],[CarNumber],[CarFactory],[CarType],[VehicleAge],
[VipCardNumber],[Display_Card_NBR],[VipCardStatus],[VipCardType],
[RegisterPhone],[RegisterAddress],[RegisterDate],[CreatedTime],[UpdatedTime])
values(
@UserId,
@CarNumber,
@CarFactory,
@CarType,
@VehicleAge,
@VipCardNumber,
@Display_Card_NBR,
@VipCardStatus,
@VipCardType,
@RegisterPhone,
@RegisterAddress,
@RegisterDate,
@CreatedTime,
@UpdatedTime
)";
            using (var db = DbHelper.CreateDefaultDbHelper())
            {
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@UserId", info.u_user_id);
                cmd.Parameters.AddWithValue("@CarNumber", info.CarNumber);
                cmd.Parameters.AddWithValue("@CarFactory", info.CarFactory);
                cmd.Parameters.AddWithValue("@CarType", info.CarType);
                cmd.Parameters.AddWithValue("@VehicleAge", info.VehicleAge);
                cmd.Parameters.AddWithValue("@VipCardNumber", info.VipCardNumber);
                cmd.Parameters.AddWithValue("@Display_Card_NBR", info.Display_Card_NBR);
                cmd.Parameters.AddWithValue("@VipCardStatus", info.VipCardStatus);
                cmd.Parameters.AddWithValue("@VipCardType", info.VipCardType);
                cmd.Parameters.AddWithValue("@RegisterPhone", info.RegisterPhone);
                cmd.Parameters.AddWithValue("@RegisterAddress", info.RegisterAddress);
                cmd.Parameters.AddWithValue("@RegisterDate", info.RegisterDate);
                cmd.Parameters.AddWithValue("@CreatedTime", info.CreatedTime);
                cmd.Parameters.AddWithValue("@UpdatedTime", info.UpdatedTime);

                var result = db.ExecuteNonQuery(cmd);
                return result;
            }
        }

        public int CreatePromotionCode(PromotionCode model)
        {
            using (var cmd = new SqlCommand(@"INSERT	INTO Gungnir..tbl_PromotionCode
            (
                Code,
                UserId,
                StartTime,
                EndTime,
                CreateTime,
                Status,
                Description,
                Discount,
                MinMoney,
                CodeChannel,
                RuleID,
                PromtionName,
                Issuer,
				Creater
            )VALUES(
            RIGHT(ABS(CAST(CHECKSUM(NEWID()) AS BIGINT) * CHECKSUM(NEWID())), 12),
            @UserID,
            @StartDate,
            @EndDate,
            GETDATE(),
            0,
            @Description,
            @Discount,
            @Minmoney,
            @Channel,
            @RuleID,
            @PromtionName,
            @Issuer,
			@Creater);
            SELECT @@IDENTITY; "))
            {
                cmd.Parameters.AddWithValue("@UserID", model.UserID);
                cmd.Parameters.AddWithValue("@StartDate", model.StartTime);
                cmd.Parameters.AddWithValue("@EndDate", model.EndTime);
                cmd.Parameters.AddWithValue("@Description", model.Description);
                cmd.Parameters.AddWithValue("@Discount", model.Discount);
                cmd.Parameters.AddWithValue("@Minmoney", model.MinMoney);
                cmd.Parameters.AddWithValue("@Channel", model.CodeChannel);
                cmd.Parameters.AddWithValue("@RuleID", model.RuleID);
                cmd.Parameters.AddWithValue("@PromtionName", model.PromotionName);
                cmd.Parameters.AddWithValue("@Issuer", model.Issuer);
                cmd.Parameters.AddWithValue("@Creater", model.Creater);

                var obj = DbHelper.ExecuteScalar(cmd);
                if (obj != null) return int.Parse(obj.ToString());
                return 0;
            }
        }
    }
}
