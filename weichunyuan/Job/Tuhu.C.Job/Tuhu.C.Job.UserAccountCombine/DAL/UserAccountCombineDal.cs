using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.UserAccountCombine.Model;
using System.Data;
using System.Data.SqlClient;
using Microsoft.SqlServer.Server;
using System.Configuration;
using Common.Logging;

namespace Tuhu.C.Job.UserAccountCombine.DAL
{
    public class UserAccountCombineDal
    {
        public static readonly ILog _sublogger = LogManager.GetLogger(typeof(UserAccountCombineDal));

        #region collectNeedCombineUserIdJob
        public static List<YewuThirdpartyInfo> GetNeedCombineYewuUsers()
        {
            string sql = @"SELECT yewu.PKID,yewu.UserId,yewu.ThirdpartyId,yewu.Channel
                            FROM Tuhu_profiles.dbo.YewuThirdpartyInfo (nolock) AS yewu
                            LEFT JOIN 
                            (
                            SELECT COUNT(*) AS countnum, ThirdpartyId, Channel 
							FROM (
							SELECT  TempA.ThirdpartyId, TempA.Channel
							FROM Tuhu_profiles..YewuThirdpartyInfo (NOLOCK) as TempA
							join Tuhu_profiles..YewuThirdpartyInfo (NOLOCK) as TempB
							on TempA.Channel=TempB.Channel and TempA.ThirdpartyId=TempB.ThirdpartyId
							and TempA.UserId!=TempB.UserId
							) as midtemp
                            GROUP BY ThirdpartyId, Channel
                            ) AS temp
                            ON temp.Channel = yewu.Channel AND temp.ThirdpartyId = yewu.ThirdpartyId
                            WHERE temp.countnum>1";
            using (var cmd = new SqlCommand(sql))
            {
                var result = DbHelper.ExecuteSelect<YewuThirdpartyInfo>(true, cmd);
                if (!result.Any()) return null;
                else return result.ToList();
            }
        }


        public static List<UserObjectModel> GetUsersByIds(List<Guid> userIds)
        {
            string Sql_Text_GetUsersByIds = @"SELECT uo.* 
                                                FROM Tuhu_profiles..UserObject(NOLOCK) uo 
                                                JOIN @TVP t 
                                                ON uo.UserID = t.Id";

            var ids = new List<SqlDataRecord>();

            foreach (var id in userIds)
            {
                var record = new SqlDataRecord(new SqlMetaData("Id", SqlDbType.UniqueIdentifier));

                record.SetValue(0, id);
                ids.Add(record);
            }

            using (var cmd = new SqlCommand(Sql_Text_GetUsersByIds))
            {
                var tvpParameter = cmd.Parameters.AddWithValue("@TVP", ids);
                tvpParameter.SqlDbType = SqlDbType.Structured;
                tvpParameter.TypeName = "[GuidTypeList]";

                return DbHelper.ExecuteSelect<UserObjectModel>(true, cmd).ToList();
            }
        }

        public static bool InsertNeedCombineUserId(NeedCombineUserId item, BaseDbHelper dbHelper)
        {
            string sql = @"INSERT INTO Tuhu_profiles.[dbo].[UAC_NeedCombineUserId]
                                ([SourceUserId]
                                ,[TargetUserId]
                                ,[Channel]
                                ,[ThirdpartyId])
                            VALUES
                                (@SourceUserId, @TargetUserId,@Channel,@ThirdpartyId)";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@SourceUserId", item.SourceUserId.ToString("d"));
                cmd.Parameters.AddWithValue("@TargetUserId", item.TargetUserId.ToString("d"));
                cmd.Parameters.AddWithValue("@Channel", item.Channel);
                cmd.Parameters.AddWithValue("@ThirdpartyId", item.ThirdpartyId);

                return dbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static List<NeedCombineUserId> GetAllExistNeedCombineUserIdList()
        {
            string sql = @"select * from Tuhu_profiles..UAC_NeedCombineUserId (nolock)";
            using (var cmd = new SqlCommand(sql))
            {
                var result = DbHelper.ExecuteSelect<NeedCombineUserId>(true, cmd);
                if (!result.Any()) return null;
                else return result.ToList();
            }
        }
        #endregion

        #region UserCombineAndRecordLog
        public static List<NeedCombineUserId> GetNeedCombineUserIdList()
        {
            string sql = @"select * from Tuhu_profiles..UAC_NeedCombineUserId (nolock)
                            where IsOperateSuccess=0";
            using (var cmd = new SqlCommand(sql))
            {
                var result = DbHelper.ExecuteSelect<NeedCombineUserId>(true, cmd);
                if (!result.Any()) return null;
                else return result.ToList();
            }
        }

        public static bool Update_NeedCombineUserId_IsOperateSuccess(NeedCombineUserId item)
        {
            string sql = @"UPDATE Tuhu_profiles..UAC_NeedCombineUserId
                                SET IsOperateSuccess = 1
                                where PKID=@PKID";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PKID", item.PKID);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static UserObjectModel GetUserById(Guid userId)
        {
            string sql = @"SELECT UserID
                            u_preferred_address,u_addresses,
                            dt_date_registered,dt_date_created
                            FROM Tuhu_profiles.dbo.UserObject(NOLOCK) 
                            WHERE UserID = @UserId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);

                return DbHelper.ExecuteFetch<UserObjectModel>(true, cmd);
            }
        }

        public static bool InsertRelatedTableCombineSuccessLog(RelatedTableCombineLog log, BaseDbHelper dbHelper)
        {
            string sql = @"INSERT INTO [Tuhu_profiles].[dbo].[UAC_RelatedTableCombineLog]
                                ([SourceUserId]
                                ,[TargetUserId]
                                ,[RelatedTableName]
                                ,[RelatedTablePK]
                                ,[UpdatedParameter]
                                ,[ActionName]
                                ,[SourceValue]
                                ,[TargetValue])
                            VALUES
                                (@SourceUserId,
                                @TargetUserId, 
                                @RelatedTableName, 
                                @RelatedTablePK,
                                @UpdatedParameter, 
                                @ActionName,
                                @SourceValue, 
                                @TargetValue)";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@SourceUserId", log.SourceUserId);
                cmd.Parameters.AddWithValue("@TargetUserId", log.TargetUserId);
                cmd.Parameters.AddWithValue("@RelatedTableName", log.RelatedTableName);
                cmd.Parameters.AddWithValue("@RelatedTablePK", log.RelatedTablePK);
                cmd.Parameters.AddWithValue("@UpdatedParameter", log.UpdatedParameter);
                cmd.Parameters.AddWithValue("@ActionName", log.ActionName);
                cmd.Parameters.AddWithValue("@SourceValue", log.SourceValue);
                cmd.Parameters.AddWithValue("@TargetValue", log.TargetValue);

                return dbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool InsertRelatedTableCombineFailLog(RelatedTableCombineFailLog failLog, BaseDbHelper dbHelper)
        {
            string sql = @"INSERT INTO [Tuhu_profiles].[dbo].[UAC_RelatedTableCombineFailLog]
                                ([SourceUserId]
                                ,[TargetUserId]
                                ,[RelatedTableName]
                                ,[RelatedTablePK]
                                ,[UpdatedParameter]
                                ,[ActionName]
                                ,[SourceValue]
                                ,[TargetValue]
                                ,[FailReason])
                            VALUES
                                (@SourceUserId,
                                @TargetUserId, 
                                @RelatedTableName, 
                                @RelatedTablePK,
                                @UpdatedParameter, 
                                @ActionName,
                                @SourceValue, 
                                @TargetValue,
                                @FailReason)";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@SourceUserId", failLog.SourceUserId);
                cmd.Parameters.AddWithValue("@TargetUserId", failLog.TargetUserId);
                cmd.Parameters.AddWithValue("@RelatedTableName", failLog.RelatedTableName);
                cmd.Parameters.AddWithValue("@RelatedTablePK", failLog.RelatedTablePK);
                cmd.Parameters.AddWithValue("@UpdatedParameter", failLog.UpdatedParameter);
                cmd.Parameters.AddWithValue("@ActionName", failLog.ActionName);
                cmd.Parameters.AddWithValue("@SourceValue", failLog.SourceValue);
                cmd.Parameters.AddWithValue("@TargetValue", failLog.TargetValue);
                cmd.Parameters.AddWithValue("@FailReason", failLog.FailReason);

                return dbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        #region updateRegion
        public static bool Update_UserObject_IsActive(Guid userId)
        {
            string sql = @"UPDATE [Tuhu_profiles].[dbo].[UserObject]
                                SET [IsActive] = 0
                                WHERE UserID=@userId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool Update_UserObject_PreferredAddress(Guid userId, string address)
        {
            string sql = @"UPDATE [Tuhu_profiles].[dbo].[UserObject]
                                SET u_preferred_address = @address
                                WHERE UserID=@userId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@address", address);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool Update_UserObject_Addresses(Guid userId, string addresses)
        {
            string sql = @"UPDATE [Tuhu_profiles].[dbo].[UserObject]
                                SET u_addresses = @addresses
                                WHERE UserID=@userId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@addresses", addresses);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool Update_Addresses_IsDefaultAddress(string addressId)
        {
            string sql = @"UPDATE [Tuhu_profiles].[dbo].[Addresses]
                                SET IsDefaultAddress = 1
                                where u_address_id=@addressId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@addressId", addressId);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool Update_YLHUserInfo_UserId(int PKID, string userId)
        {
            string sql = @"UPDATE [Tuhu_profiles].[dbo].[YLH_UserInfo]
                                SET u_user_id = @userId
                                where PKID=@PKID";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@PKID", PKID);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool Update_UserAuth_UserId(int PKID, Guid userId)
        {
            string sql = @"UPDATE Tuhu_profiles.[dbo].UserAuth
                                SET UserId = @userId
                                where PKID=@PKID";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@PKID", PKID);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool Update_YLHUserVipCardInfo_UserId(int PKID, string userId)
        {
            string sql = @"UPDATE [Tuhu_profiles].dbo.YLH_UserVipCardInfo
                                SET u_user_id = @userId
                                where PKID=@PKID";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@PKID", PKID);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool Update_YewuThirdpartyInfo_UserId(int PKID, Guid userId)
        {
            string sql = @"UPDATE Tuhu_profiles.[dbo].[YewuThirdpartyInfo]
                                SET UserId = @userId
                                where PKID=@PKID";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@PKID", PKID);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool Update_ShopReserve_UserID(int PKID, Guid userId)
        {
            string sql = @"UPDATE Tuhu_shop.[dbo].[ShopReserve]
                                SET UserID = @userId
                                where PKID=@PKID";

            string TuhuShop = ConfigurationManager.ConnectionStrings["Tuhu_shop"].ConnectionString;

            using (var dbHelper = DbHelper.CreateDbHelper(TuhuShop))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@PKID", PKID);
                    return dbHelper.ExecuteNonQuery(cmd) > 0;
                }
            }
            //using (var cmd = new SqlCommand(sql))
            //{
            //    cmd.Parameters.AddWithValue("@userId", userId);
            //    cmd.Parameters.AddWithValue("@PKID", PKID);
            //    return DbHelper.ExecuteNonQuery(cmd) > 0;
            //}
        }

        public static bool Update_ShopReceiveCheckThird_UserID(int PKID, Guid userId)
        {
            string sql = @"UPDATE Tuhu_shop.[dbo].[ShopReceiveCheckThird]
                                SET UserID = @userId
                                where PKID=@PKID";

            string TuhuShop = ConfigurationManager.ConnectionStrings["Tuhu_shop"].ConnectionString;

            using (var dbHelper = DbHelper.CreateDbHelper(TuhuShop))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@PKID", PKID);
                    return dbHelper.ExecuteNonQuery(cmd) > 0;
                }
            }
            //using (var cmd = new SqlCommand(sql))
            //{
            //    cmd.Parameters.AddWithValue("@userId", userId);
            //    cmd.Parameters.AddWithValue("@PKID", PKID);
            //    return DbHelper.ExecuteNonQuery(cmd) > 0;
            //}
        }

        public static bool Update_ShopReceiveCheckSecond_UserID(int PKID, Guid userId)
        {
            string sql = @"UPDATE Tuhu_shop.[dbo].[ShopReceiveCheckSecond]
                                SET UserID = @userId
                                where PKID=@PKID";

            string TuhuShop = ConfigurationManager.ConnectionStrings["Tuhu_shop"].ConnectionString;

            using (var dbHelper = DbHelper.CreateDbHelper(TuhuShop))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@PKID", PKID);
                    return dbHelper.ExecuteNonQuery(cmd) > 0;
                }
            }
            //using (var cmd = new SqlCommand(sql))
            //{
            //    cmd.Parameters.AddWithValue("@userId", userId);
            //    cmd.Parameters.AddWithValue("@PKID", PKID);
            //    return DbHelper.ExecuteNonQuery(cmd) > 0;
            //}
        }

        public static bool Update_ShopReceiveCheckFirst_UserID(int PKID, Guid userId)
        {
            string sql = @"UPDATE Tuhu_shop.[dbo].[ShopReceiveCheckFirst]
                                SET UserID = @userId
                                where PKID=@PKID";

            string TuhuShop = ConfigurationManager.ConnectionStrings["Tuhu_shop"].ConnectionString;

            using (var dbHelper = DbHelper.CreateDbHelper(TuhuShop))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@PKID", PKID);
                    return dbHelper.ExecuteNonQuery(cmd) > 0;
                }
            }
            //using (var cmd = new SqlCommand(sql))
            //{
            //    cmd.Parameters.AddWithValue("@userId", userId);
            //    cmd.Parameters.AddWithValue("@PKID", PKID);
            //    return DbHelper.ExecuteNonQuery(cmd) > 0;
            //}
        }

        public static bool Update_ShopReceiveOrder_UserID(int PKID, Guid userId)
        {
            string sql = @"UPDATE Tuhu_shop.[dbo].[ShopReceiveOrder]
                                SET UserID = @userId
                                where PKID=@PKID";

            string TuhuShop = ConfigurationManager.ConnectionStrings["Tuhu_shop"].ConnectionString;

            using (var dbHelper = DbHelper.CreateDbHelper(TuhuShop))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@PKID", PKID);
                    return dbHelper.ExecuteNonQuery(cmd) > 0;
                }
            }
            //using (var cmd = new SqlCommand(sql))
            //{
            //    cmd.Parameters.AddWithValue("@userId", userId);
            //    cmd.Parameters.AddWithValue("@PKID", PKID);
            //    return DbHelper.ExecuteNonQuery(cmd) > 0;
            //}
        }

        public static bool Update_ShopReceive_UserID(int PKID, Guid userId)
        {
            string sql = @"UPDATE Tuhu_shop.[dbo].[ShopReceive]
                                SET UserID = @userId
                                where PKID=@PKID";

            string TuhuShop = ConfigurationManager.ConnectionStrings["Tuhu_shop"].ConnectionString;

            using (var dbHelper = DbHelper.CreateDbHelper(TuhuShop))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@PKID", PKID);
                    return dbHelper.ExecuteNonQuery(cmd) > 0;
                }
            }
            //using (var cmd = new SqlCommand(sql))
            //{
            //    cmd.Parameters.AddWithValue("@userId", userId);
            //    cmd.Parameters.AddWithValue("@PKID", PKID);
            //    return DbHelper.ExecuteNonQuery(cmd) > 0;
            //}
        }

        public static bool Update_ShopReceiveNew_UserID(int PKID, Guid userId)
        {
            string sql = @"UPDATE Tuhu_shop.[dbo].[ShopReceiveNew]
                                SET UserID = @userId
                                where PKID=@PKID";

            string TuhuShop = ConfigurationManager.ConnectionStrings["Tuhu_shop"].ConnectionString;

            using (var dbHelper = DbHelper.CreateDbHelper(TuhuShop))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@PKID", PKID);
                    return dbHelper.ExecuteNonQuery(cmd) > 0;
                }
            }
            //using (var cmd = new SqlCommand(sql))
            //{
            //    cmd.Parameters.AddWithValue("@userId", userId);
            //    cmd.Parameters.AddWithValue("@PKID", PKID);
            //    return DbHelper.ExecuteNonQuery(cmd) > 0;
            //}
        }

        public static bool Update_Addresses_UserId(string addressId, Guid userId)
        {
            string sql = @"UPDATE [Tuhu_profiles].[dbo].[Addresses]
                                SET UserId = @userId
                                where u_address_id=@addressId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@addressId", addressId);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool Update_CarObject_UserId(string carId, string userId)
        {
            string sql = @"UPDATE [Tuhu_profiles].dbo.CarObject
                                SET u_user_id = @userId , IsDefaultCar=0 
                                where u_car_id=@carId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@carId", carId);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool Update_Order_UserID(int PKID, Guid userId)
        {
            string sql = @"UPDATE Gungnir.dbo.tbl_Order
                                SET UserID = @userId
                                where PKID=@PKID";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@PKID", PKID);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool Update_EndUserCase_EndUserGuid(int PKID, Guid endUserGuid)
        {
            string sql = @"UPDATE Gungnir.dbo.tbl_EndUserCase
                                SET EndUserGuid = @endUserGuid
                                where PKID=@PKID";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@endUserGuid", endUserGuid);
                cmd.Parameters.AddWithValue("@PKID", PKID);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool Update_CRMRequisition_EndUserGuid(int ID, Guid endUserGuid)
        {
            string sql = @"UPDATE Gungnir.dbo.tbl_CRM_Requisition
                                SET EndUserGuid = @endUserGuid
                                where ID=@ID";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@endUserGuid", endUserGuid.ToString("d"));
                cmd.Parameters.AddWithValue("@ID", ID);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool Update_CRMAppointment_UserID(int PKID, string userId)
        {
            string sql = @"UPDATE Tuhu_crm.dbo.CRMAppointment
                                SET UserID = @userId
                                where PKID=@PKID";
            string TuhuCRM = ConfigurationManager.ConnectionStrings["Tuhu_crm"].ConnectionString;

            using (var dbHelper = DbHelper.CreateDbHelper(TuhuCRM))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@PKID", PKID);
                    return dbHelper.ExecuteNonQuery(cmd) > 0;
                }
            }

            //using (var cmd = new SqlCommand(sql))
            //{
            //    cmd.Parameters.AddWithValue("@userId", userId);
            //    cmd.Parameters.AddWithValue("@PKID", PKID);
            //    return DbHelper.ExecuteNonQuery(cmd) > 0;
            //}
        }

        public static bool Update_CRMContactLog_UserID(int PKID, string userId)
        {
            string sql = @"UPDATE Tuhu_crm.dbo.CRMContactLog
                                SET UserID = @userId
                                where PKID=@PKID";
            string TuhuCRM = ConfigurationManager.ConnectionStrings["Tuhu_crm"].ConnectionString;

            using (var dbHelper = DbHelper.CreateDbHelper(TuhuCRM))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@PKID", PKID);
                    return dbHelper.ExecuteNonQuery(cmd) > 0;
                }
            }

            //using (var cmd = new SqlCommand(sql))
            //{
            //    cmd.Parameters.AddWithValue("@userId", userId);
            //    cmd.Parameters.AddWithValue("@PKID", PKID);
            //    return DbHelper.ExecuteNonQuery(cmd) > 0;
            //}
        }

        public static bool Update_CRMFlagInfo_UserId(int PKID, Guid userId)
        {
            string sql = @"UPDATE Tuhu_crm.dbo.CRMFlagInfo
                                SET UserId = @userId
                                where PKID=@PKID";
            string TuhuCRM = ConfigurationManager.ConnectionStrings["Tuhu_crm"].ConnectionString;

            using (var dbHelper = DbHelper.CreateDbHelper(TuhuCRM))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@PKID", PKID);
                    return dbHelper.ExecuteNonQuery(cmd) > 0;
                }
            }

            //using (var cmd = new SqlCommand(sql))
            //{
            //    cmd.Parameters.AddWithValue("@userId", userId);
            //    cmd.Parameters.AddWithValue("@PKID", PKID);
            //    return DbHelper.ExecuteNonQuery(cmd) > 0;
            //}
        }

        public static bool Update_PromotionCode_UserId(int PKID, Guid userId)
        {
            string sql = @"UPDATE Gungnir.dbo.tbl_PromotionCode
                                SET UserId = @userId
                                where PKID=@PKID";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@PKID", PKID);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool Update_UserIntegral_Status(Guid integralID, int status)
        {
            string sql = @"UPDATE [Tuhu_profiles].[dbo].[tbl_UserIntegral]
                                SET Status = @status
                                where IntegralID=@integralID";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@integralID", integralID);
                cmd.Parameters.AddWithValue("@status", status);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool Update_UserIntegral_Integral(Guid integralID, int integral)
        {
            string sql = @"UPDATE [Tuhu_profiles].[dbo].[tbl_UserIntegral]
                                SET Integral = @integral
                                where IntegralID=@integralID";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@integralID", integralID);
                cmd.Parameters.AddWithValue("@integral", integral);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool Insert_UserIntegral(UserIntegral integral)
        {
            string sql = @"INSERT INTO [Tuhu_profiles].[dbo].[tbl_UserIntegral]
                                ([IntegralID]
                                ,[UserID]
                                ,[Integral]
                                ,[Status])
                            VALUES
                                (@IntegralID,
                                @UserID, 
                                @Integral, 
                                @Status)";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@IntegralID", integral.IntegralID);
                cmd.Parameters.AddWithValue("@UserID", integral.UserID);
                cmd.Parameters.AddWithValue("@Integral", integral.Integral);
                cmd.Parameters.AddWithValue("@Status", integral.Status);

                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool Update_UserIntegralDetail_IntegralID(Guid integralDetailID, Guid tIntegralID)
        {
            string sql = @"UPDATE Tuhu_profiles.dbo.tbl_UserIntegralDetail
                                SET IntegralID = @tIntegralID
                                where IntegralDetailID=@integralDetailID";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@tIntegralID", tIntegralID);
                cmd.Parameters.AddWithValue("@integralDetailID", integralDetailID);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool Update_UserGradeStatisticsDetail_UserId(int PKID, Guid userId)
        {
            string sql = @"UPDATE Tuhu_profiles..UserGradeStatisticsDetail
                                SET UserID = @userId
                                where PKID=@PKID";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@PKID", PKID);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }
        #endregion

        #region combineAction_selectRegion
        public static Addresses GetAddressById(string addressId)
        {
            string sql = @"select top 1 u_address_id,IsDefaultAddress,UserId 
                            from [Tuhu_profiles].[dbo].[Addresses] (nolock)
                            where u_address_id=@addressId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@addressId", addressId);

                return DbHelper.ExecuteFetch<Addresses>(true, cmd);
            }
        }

        public static List<YLHUserInfo> GetYLHUserInfoById(string userId)
        {
            string sql = @"select PKID,u_user_id from Tuhu_profiles.[dbo].[YLH_UserInfo] (nolock)
                            where [u_user_id]=@userId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                var result = DbHelper.ExecuteSelect<YLHUserInfo>(true, cmd);
                if (!result.Any()) return null;
                else return result.ToList();
            }
        }

        public static List<UserAuth> GetUserAuthById(Guid userId)
        {
            string sql = @"select PKID,UserId from Tuhu_profiles..UserAuth (nolock)
                            where UserId=@userId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@userId", userId.ToString("d"));
                var result = DbHelper.ExecuteSelect<UserAuth>(true, cmd);
                if (!result.Any()) return null;
                else return result.ToList();
            }
        }

        public static List<YLHUserVipCardInfo> GetYLHUserVipCardInfoById(string userId)
        {
            string sql = @"select PKID,u_user_id from Tuhu_profiles.[dbo].[YLH_UserVipCardInfo] (nolock)
                            where [u_user_id]=@userId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                var result = DbHelper.ExecuteSelect<YLHUserVipCardInfo>(true, cmd);
                if (!result.Any()) return null;
                else return result.ToList();
            }
        }

        public static List<YewuThirdpartyInfo> GetYewuThirdpartyInfoById(Guid userId, string channel, string thirdPartyId)
        {
            string sql = @"select PKID,UserId,ThirdpartyId,Channel from Tuhu_profiles.[dbo].[YewuThirdpartyInfo]
                            where UserId=@userId and Channel=@channel and ThirdpartyId=@thirdPartyId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@userId", userId.ToString("d"));
                cmd.Parameters.AddWithValue("@channel", channel);
                cmd.Parameters.AddWithValue("@thirdPartyId", thirdPartyId);
                var result = DbHelper.ExecuteSelect<YewuThirdpartyInfo>(true, cmd);
                if (!result.Any()) return null;
                else return result.ToList();
            }
        }

        public static List<YewuThirdpartyInfo> GetYewuThirdpartyInfoByIdOnly(Guid userId)
        {
            string sql = @"select PKID,UserId,ThirdpartyId,Channel from Tuhu_profiles.[dbo].[YewuThirdpartyInfo]
                            where UserId=@userId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@userId", userId.ToString("d"));
                var result = DbHelper.ExecuteSelect<YewuThirdpartyInfo>(true, cmd);
                if (!result.Any()) return null;
                else return result.ToList();
            }
        }

        public static List<ShopReserve> GetShopReserveById(Guid userId)
        {
            string sql = @"select PKID,UserID from Tuhu_shop.[dbo].[ShopReserve] (nolock)
                            where UserID=@userId";

            string TuhuShop = ConfigurationManager.ConnectionStrings["Tuhu_shop"].ConnectionString;

            using (var dbHelper = DbHelper.CreateDbHelper(TuhuShop))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@userId", userId.ToString("d"));
                    var result = dbHelper.ExecuteSelect<ShopReserve>(cmd);
                    if (!result.Any()) return null;
                    else return result.ToList();
                }
            }
            //using (var cmd = new SqlCommand(sql))
            //{
            //    cmd.Parameters.AddWithValue("@userId", userId.ToString("d"));
            //    var result = DbHelper.ExecuteSelect<ShopReserve>(true, cmd);
            //    if (!result.Any()) return null;
            //    else return result.ToList();
            //}
        }

        public static List<ShopReceiveCheckThird> GetShopReceiveCheckThirdById(Guid userId)
        {
            string sql = @"select PKID,UserID from Tuhu_shop.[dbo].[ShopReceiveCheckThird] (nolock)
                            where UserID=@userId";

            string TuhuShop = ConfigurationManager.ConnectionStrings["Tuhu_shop"].ConnectionString;

            using (var dbHelper = DbHelper.CreateDbHelper(TuhuShop))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@userId", userId.ToString("d"));
                    var result = dbHelper.ExecuteSelect<ShopReceiveCheckThird>(cmd);
                    if (!result.Any()) return null;
                    else return result.ToList();
                }
            }
            //using (var cmd = new SqlCommand(sql))
            //{
            //    cmd.Parameters.AddWithValue("@userId", userId.ToString("d"));
            //    var result = DbHelper.ExecuteSelect<ShopReceiveCheckThird>(true, cmd);
            //    if (!result.Any()) return null;
            //    else return result.ToList();
            //}
        }

        public static List<ShopReceiveCheckSecond> GetShopReceiveCheckSecondById(Guid userId)
        {
            string sql = @"select PKID,UserID from Tuhu_shop.[dbo].[ShopReceiveCheckSecond] (nolock)
                            where UserID=@userId";

            string TuhuShop = ConfigurationManager.ConnectionStrings["Tuhu_shop"].ConnectionString;

            using (var dbHelper = DbHelper.CreateDbHelper(TuhuShop))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@userId", userId.ToString("d"));
                    var result = dbHelper.ExecuteSelect<ShopReceiveCheckSecond>(cmd);
                    if (!result.Any()) return null;
                    else return result.ToList();
                }
            }
            //using (var cmd = new SqlCommand(sql))
            //{
            //    cmd.Parameters.AddWithValue("@userId", userId.ToString("d"));
            //    var result = DbHelper.ExecuteSelect<ShopReceiveCheckSecond>(true, cmd);
            //    if (!result.Any()) return null;
            //    else return result.ToList();
            //}
        }

        public static List<ShopReceiveCheckFirst> GetShopReceiveCheckFirstById(Guid userId)
        {
            string sql = @"select PKID,UserID from Tuhu_shop.[dbo].[ShopReceiveCheckFirst] (nolock)
                            where UserID=@userId";

            string TuhuShop = ConfigurationManager.ConnectionStrings["Tuhu_shop"].ConnectionString;

            using (var dbHelper = DbHelper.CreateDbHelper(TuhuShop))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@userId", userId.ToString("d"));
                    var result = dbHelper.ExecuteSelect<ShopReceiveCheckFirst>(cmd);
                    if (!result.Any()) return null;
                    else return result.ToList();
                }
            }
            //using (var cmd = new SqlCommand(sql))
            //{
            //    cmd.Parameters.AddWithValue("@userId", userId.ToString("d"));
            //    var result = DbHelper.ExecuteSelect<ShopReceiveCheckFirst>(true, cmd);
            //    if (!result.Any()) return null;
            //    else return result.ToList();
            //}
        }

        public static List<ShopReceiveOrder> GetShopShopReceiveOrderById(Guid userId)
        {
            string sql = @"select PKID,UserID from Tuhu_shop.[dbo].[ShopReceiveOrder] (nolock)
                            where UserID=@userId";

            string TuhuShop = ConfigurationManager.ConnectionStrings["Tuhu_shop"].ConnectionString;

            using (var dbHelper = DbHelper.CreateDbHelper(TuhuShop))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@userId", userId.ToString("d"));
                    var result = dbHelper.ExecuteSelect<ShopReceiveOrder>(cmd);
                    if (!result.Any()) return null;
                    else return result.ToList();
                }
            }
            //using (var cmd = new SqlCommand(sql))
            //{
            //    cmd.Parameters.AddWithValue("@userId", userId.ToString("d"));
            //    var result = DbHelper.ExecuteSelect<ShopReceiveOrder>(true, cmd);
            //    if (!result.Any()) return null;
            //    else return result.ToList();
            //}
        }

        public static List<ShopReceive> GetShopReceiveById(Guid userId)
        {
            string sql = @"select PKID,UserID from Tuhu_shop.[dbo].[ShopReceive] (nolock)
                            where UserID=@userId";

            string TuhuShop = ConfigurationManager.ConnectionStrings["Tuhu_shop"].ConnectionString;

            using (var dbHelper = DbHelper.CreateDbHelper(TuhuShop))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@userId", userId.ToString("d"));
                    var result = dbHelper.ExecuteSelect<ShopReceive>(cmd);
                    if (!result.Any()) return null;
                    else return result.ToList();
                }
            }
            //using (var cmd = new SqlCommand(sql))
            //{
            //    cmd.Parameters.AddWithValue("@userId", userId.ToString("d"));
            //    var result = DbHelper.ExecuteSelect<ShopReceive>(true, cmd);
            //    if (!result.Any()) return null;
            //    else return result.ToList();
            //}
        }

        public static List<ShopReceiveNew> GetShopReceiveNewById(Guid userId)
        {
            string sql = @"select PKID,UserID from Tuhu_shop.[dbo].[ShopReceiveNew] (nolock)
                            where UserID=@userId";

            string TuhuShop = ConfigurationManager.ConnectionStrings["Tuhu_shop"].ConnectionString;

            using (var dbHelper = DbHelper.CreateDbHelper(TuhuShop))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@userId", userId.ToString("d"));
                    var result = dbHelper.ExecuteSelect<ShopReceiveNew>(cmd);
                    if (!result.Any()) return null;
                    else return result.ToList();
                }
            }
            //using (var cmd = new SqlCommand(sql))
            //{
            //    cmd.Parameters.AddWithValue("@userId", userId.ToString("d"));
            //    var result = DbHelper.ExecuteSelect<ShopReceiveNew>(true, cmd);
            //    if (!result.Any()) return null;
            //    else return result.ToList();
            //}
        }

        public static List<Addresses> GetAddressesById(Guid userId)
        {
            string sql = @"select u_address_id,UserId,IsDefaultAddress from [Tuhu_profiles].[dbo].[Addresses] (nolock)
                            where UserId=@userId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@userId", userId.ToString("d"));
                var result = DbHelper.ExecuteSelect<Addresses>(true, cmd);
                if (!result.Any()) return null;
                else return result.ToList();
            }
        }

        public static List<CarObject> GetCarObjectById(string userId)
        {
            string sql = @"select u_car_id,u_user_id from Tuhu_profiles.dbo.CarObject (nolock)
                            where [u_user_id]=@userId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                var result = DbHelper.ExecuteSelect<CarObject>(true, cmd);
                if (!result.Any()) return null;
                else return result.ToList();
            }
        }

        public static List<Order> GetOrderById(Guid userId)
        {
            string sql = @"select PKID,UserID from Gungnir.dbo.tbl_Order (nolock)
                            where UserID=@userId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@userId", userId.ToString("d"));
                var result = DbHelper.ExecuteSelect<Order>(true, cmd);
                if (!result.Any()) return null;
                else return result.ToList();
            }
        }

        public static List<EndUserCase> GetEndUserCaseById(Guid endUserGuid)
        {
            string sql = @"select PKID,EndUserGuid from Gungnir.dbo.tbl_EndUserCase (nolock)
                            where EndUserGuid=@endUserGuid";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@endUserGuid", endUserGuid.ToString("d"));
                var result = DbHelper.ExecuteSelect<EndUserCase>(true, cmd);
                if (!result.Any()) return null;
                else return result.ToList();
            }
        }

        public static List<CRMRequisition> GetCRMRequisitionById(string endUserGuid)
        {
            string sql = @"select ID,EndUserGuid from Gungnir.dbo.tbl_CRM_Requisition (nolock)
                            where EndUserGuid=@endUserGuid";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@endUserGuid", endUserGuid);
                var result = DbHelper.ExecuteSelect<CRMRequisition>(true, cmd);
                if (!result.Any()) return null;
                else return result.ToList();
            }
        }

        public static List<CRMAppointment> GetCRMAppointmentById(string userId)
        {
            string sql = @"select PKID,UserID from Tuhu_crm.dbo.CRMAppointment (nolock)
                            where UserID=@userId";
            string TuhuCRM = ConfigurationManager.ConnectionStrings["Tuhu_crm"].ConnectionString;

            using (var dbHelper = DbHelper.CreateDbHelper(TuhuCRM))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    var result = dbHelper.ExecuteSelect<CRMAppointment>(cmd);
                    if (!result.Any()) return null;
                    else return result.ToList();
                }
            }

            //using (var cmd = new SqlCommand(sql))
            //{
            //    cmd.Parameters.AddWithValue("@userId", userId);
            //    var result = DbHelper.ExecuteSelect<CRMAppointment>(cmd);
            //    if (!result.Any()) return null;
            //    else return result.ToList();
            //}
        }

        public static List<CRMContactLog> GetCRMContactLogById(string userId)
        {
            string sql = @"select PKID,UserID from Tuhu_crm.dbo.CRMContactLog (nolock)
                            where UserID=@userId";
            string TuhuCRM = ConfigurationManager.ConnectionStrings["Tuhu_crm"].ConnectionString;

            using (var dbHelper = DbHelper.CreateDbHelper(TuhuCRM))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    var result = dbHelper.ExecuteSelect<CRMContactLog>(cmd);
                    if (!result.Any()) return null;
                    else return result.ToList();
                }
            }

            //using (var cmd = new SqlCommand(sql))
            //{
            //    cmd.Parameters.AddWithValue("@userId", userId);
            //    var result = DbHelper.ExecuteSelect<CRMContactLog>(cmd);
            //    if (!result.Any()) return null;
            //    else return result.ToList();
            //}
        }

        public static List<CRMFlagInfo> GetCRMFlagInfoById(Guid userId)
        {
            string sql = @"select PKID,UserId from Tuhu_crm.dbo.CRMFlagInfo (nolock)
                            where UserId=@userId";
            string TuhuCRM = ConfigurationManager.ConnectionStrings["Tuhu_crm"].ConnectionString;

            using (var dbHelper = DbHelper.CreateDbHelper(TuhuCRM))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@userId", userId.ToString("d"));
                    var result = dbHelper.ExecuteSelect<CRMFlagInfo>(cmd);
                    if (!result.Any()) return null;
                    else return result.ToList();
                }
            }

            //using (var cmd = new SqlCommand(sql))
            //{
            //    cmd.Parameters.AddWithValue("@userId", userId.ToString("d"));
            //    var result = DbHelper.ExecuteSelect<CRMFlagInfo>(cmd);
            //    if (!result.Any()) return null;
            //    else return result.ToList();
            //}
        }

        public static List<PromotionCode> GetPromotionCodeById(Guid userId)
        {
            string sql = @"select PKID,UserId from Gungnir.dbo.tbl_PromotionCode (nolock)
                            where UserId=@userId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@userId", userId.ToString("d"));
                var result = DbHelper.ExecuteSelect<PromotionCode>(true, cmd);
                if (!result.Any()) return null;
                else return result.ToList();
            }
        }

        public static UserIntegral GetUserIntegralById(Guid userId)
        {
            string sql = @"select top 1 IntegralID,UserID,Integral,Status from Tuhu_profiles.dbo.tbl_UserIntegral (nolock)
                            where UserID=@userId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@userId", userId);

                return DbHelper.ExecuteFetch<UserIntegral>(true, cmd);
            }

        }

        public static List<UserIntegralDetail> GetUserIntegralDetailById(Guid integralID)
        {
            string sql = @"select IntegralDetailID,IntegralID from Tuhu_profiles.dbo.tbl_UserIntegralDetail (nolock)
                            where IntegralID=@integralID";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@integralID", integralID.ToString("d"));
                var result = DbHelper.ExecuteSelect<UserIntegralDetail>(true, cmd);
                if (!result.Any()) return null;
                else return result.ToList();
            }
        }

        public static List<UserGradeStatisticsDetail> GetUserGradeStatisticsDetailById(Guid userId)
        {
            string sql = @"select PKID,UserID from Tuhu_profiles..UserGradeStatisticsDetail with (nolock)
                            where UserID=@userId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@userId", userId.ToString("d"));

                var result = DbHelper.ExecuteSelect<UserGradeStatisticsDetail>(true, cmd);
                if (!result.Any()) return null;
                else return result.ToList();
            }
        }
        #endregion
        #endregion

        #region CleanDupYewuUserJob
        public static List<YewuThirdpartyInfo> GetDupYewuUsers()
        {
            string sql = @"SELECT yewu.PKID,yewu.UserId,yewu.ThirdpartyId,yewu.Channel
                            FROM Tuhu_profiles.dbo.YewuThirdpartyInfo (nolock) AS yewu
                            JOIN (
                            select dup.countnum,dup.UserId, dup.ThirdpartyId, dup.Channel 
                            from
                            (
                            select COUNT(*) AS countnum,UserId, ThirdpartyId, Channel 
                            FROM Tuhu_profiles..YewuThirdpartyInfo (NOLOCK)
                            group by UserId, ThirdpartyId, Channel ) as dup
                            where dup.countnum>1
                            ) as temp
                            ON temp.Channel = yewu.Channel AND temp.ThirdpartyId = yewu.ThirdpartyId and temp.UserId=yewu.UserId
                            order by yewu.UserId,yewu.ThirdpartyId,yewu.Channel";
            using (var cmd = new SqlCommand(sql))
            {
                var result = DbHelper.ExecuteSelect<YewuThirdpartyInfo>(true, cmd);
                if (!result.Any()) return null;
                else return result.ToList();
            }
        }

        public static bool DeleteDupYewuThirdpartyInfo(int pkid, BaseDbHelper dbHelper)
        {
            string sql = @"delete Tuhu_profiles..YewuThirdpartyInfo
                            where PKID=@PKID";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PKID", pkid);

                return dbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static bool RecordDeletedDupYewuThirdpartyInfo(YewuThirdpartyInfo item, BaseDbHelper dbHelper)
        {
            string sql = @"INSERT INTO [Tuhu_profiles].[dbo].[UAC_DeleteDupYewuThirdpartyInfo]
                                ([UserId]
                                ,[Channel]
                                ,[ThirdpartyId])
                            VALUES
                                (@UserId, @Channel, @ThirdpartyId)";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@UserId", item.UserId.ToString("d"));
                cmd.Parameters.AddWithValue("@Channel", item.Channel);
                cmd.Parameters.AddWithValue("@ThirdpartyId", item.ThirdpartyId);

                return dbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }
        #endregion

        #region CollectNeedCombineInUserTableJob
        public static List<UserObjectModel> GetNeedCombineInUserTable()
        {
            string sql = @"select u_user_id,UserID,u_mobile_number,IsActive,IsMobileVerify,dt_date_created, dt_date_registered, dt_last_logon, dt_date_last_changed,u_application_name
                            from Tuhu_profiles..UserObject (nolock)
                            where u_mobile_number in (
                            select u_mobile_number from (
                            select count(1) as countnum, u_mobile_number
                            from Tuhu_profiles..UserObject (nolock)
                            where IsActive=1 and u_mobile_number is not null and u_mobile_number!=''
                            group by u_mobile_number ) source
                            where source.countnum>1 )
                            order by u_mobile_number";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 120;
                var result = DbHelper.ExecuteSelect<UserObjectModel>(true, cmd);
                if (!result.Any()) return null;
                else return result.ToList();
            }
        }

        public static bool InsertNeedCombineUserIdViaPhone(NeedCombineUserIdViaPhone item, BaseDbHelper dbHelper)
        {
            string sql = @"INSERT INTO Tuhu_profiles.dbo.[UAC_NeedCombineUserIdViaMobile]
                                ([SourceUserId]
                                ,[TargetUserId]
                                ,[MobileNumber])
                            VALUES
                                (@SourceUserId, @TargetUserId,@MobileNumber)";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@SourceUserId", item.SourceUserId.ToString("d"));
                cmd.Parameters.AddWithValue("@TargetUserId", item.TargetUserId.ToString("d"));
                cmd.Parameters.AddWithValue("@MobileNumber", item.MobileNumber);

                return dbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        public static List<NeedCombineUserIdViaPhone> GetAllExistNeedCombineUserIdViaPhoneList()
        {
            string sql = @"select * from Tuhu_profiles..UAC_NeedCombineUserIdViaMobile (nolock)";
            using (var cmd = new SqlCommand(sql))
            {
                var result = DbHelper.ExecuteSelect<NeedCombineUserIdViaPhone>(true, cmd);
                if (!result.Any()) return null;
                else return result.ToList();
            }
        }

        public static List<UserAuth> GetUserAuthsByIds(List<UserObjectModel> userIds)
        {
            string Sql_Text_GetUsersByIds = @"SELECT ua.* 
                                                FROM Tuhu_profiles.[dbo].UserAuth (NOLOCK) ua
                                                JOIN @TVP t 
                                                ON ua.UserId = t.Id and ua.BindingStatus='Bound'";

            var ids = new List<SqlDataRecord>();

            foreach (var id in userIds)
            {
                var record = new SqlDataRecord(new SqlMetaData("Id", SqlDbType.UniqueIdentifier));

                record.SetValue(0, id.UserId.Value);
                ids.Add(record);
            }

            using (var cmd = new SqlCommand(Sql_Text_GetUsersByIds))
            {
                var tvpParameter = cmd.Parameters.AddWithValue("@TVP", ids);
                tvpParameter.SqlDbType = SqlDbType.Structured;
                tvpParameter.TypeName = "[GuidTypeList]";

                return DbHelper.ExecuteSelect<UserAuth>(true, cmd).ToList();
            }
        }

        public static List<YLHUserInfo> GetUserYLHInfosByIds(List<UserObjectModel> userIds)
        {
            string Sql_Text_GetUsersByIds = @"SELECT ylh.* 
                                                FROM Tuhu_profiles.[dbo].[YLH_UserInfo](NOLOCK) ylh 
                                                JOIN @TVP t 
                                                ON ylh.u_user_id = t.Id";

            var ids = new List<SqlDataRecord>();

            foreach (var id in userIds)
            {
                var record = new SqlDataRecord(new SqlMetaData("Id", SqlDbType.UniqueIdentifier));

                record.SetValue(0, id.UserId.Value);
                ids.Add(record);
            }

            using (var cmd = new SqlCommand(Sql_Text_GetUsersByIds))
            {
                var tvpParameter = cmd.Parameters.AddWithValue("@TVP", ids);
                tvpParameter.SqlDbType = SqlDbType.Structured;
                tvpParameter.TypeName = "[GuidTypeList]";

                return DbHelper.ExecuteSelect<YLHUserInfo>(true, cmd).ToList();
            }
        }
        #endregion

        #region UserCombineViaPhoneAndRecordLogJob
        public static List<NeedCombineUserIdViaPhone> GetNeedCombineUserIdViaPhoneList()
        {
            string sql = @"select * from Tuhu_profiles..UAC_NeedCombineUserIdViaMobile (nolock)
                            where IsOperateSuccess=0";
            using (var cmd = new SqlCommand(sql))
            {
                var result = DbHelper.ExecuteSelect<NeedCombineUserIdViaPhone>(true, cmd);
                if (!result.Any()) return null;
                else return result.ToList();
            }
        }

        public static bool Update_NeedCombineUserIdViaPhone_IsOperateSuccess(NeedCombineUserIdViaPhone item)
        {
            string sql = @"UPDATE Tuhu_profiles..[UAC_NeedCombineUserIdViaMobile]
                                SET IsOperateSuccess = 1, LastUpdateDataTime=GETDATE()
                                where PKID=@PKID";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@PKID", item.PKID);
                return DbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }
        #endregion

        #region BatchLogOffUserJob
        public static List<Guid> GetNeedLogOffUserIdsForChangeBind()
        {
            List<Guid> userIdList = new List<Guid>();

            string sql = @"select *
                            from tuhu_profiles..userobject with (nolock)
                            where u_mobile_number like '100%' and len(u_mobile_number)=14";
            using (var cmd = new SqlCommand(sql))
            {
                var result = DbHelper.ExecuteSelect<UserObjectModel>(cmd);

                foreach (var item in result.ToList())
                    userIdList.Add(item.UserId.Value);
            }
            return userIdList;
        }
        #endregion

        #region CollectUserDailyIncJob
        public static bool CheckDailyTotalDataExist(string date)
        {
            string sql = @"select top 1 1
                            from [Tuhu_profiles].dbo.UserGrowthSchedule with (nolock)
                            where DayString=@dateString and Channel='Total' and [Group]='Total'";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@dateString", date);
                var result = DbHelper.ExecuteScalar(cmd);

                return result != null;
            }
        }

        public static int GetDailyTotalNum(string date)
        {
            string sql = @"select top 1 IncNum
                            from [Tuhu_profiles].dbo.UserGrowthSchedule with (nolock)
                            where DayString=@dateString and Channel='Total' and [Group]='Total'";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@dateString", date);
                var result = DbHelper.ExecuteScalar(cmd);

                return Convert.ToInt32(result);
            }
        }

        public static int GetDailyIncCountNum(string date)
        {
            string sql = @"select sum(IncNum)
                            from [Tuhu_profiles].dbo.UserGrowthSchedule with (nolock)
                            where DayString=@dateString and Channel!='Total' and [Group]!='Total'";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@dateString", date);
                var result = DbHelper.ExecuteScalar(cmd);

                if (result.Equals(DBNull.Value)) return 0;
                else
                    return Convert.ToInt32(result);
            }
        }

        public static bool ClearDailyIncData(string date)
        {
            string sql = @"delete  
                            from [Tuhu_profiles].dbo.UserGrowthSchedule
                            where DayString=@dateString";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@dateString", date);
                var result = DbHelper.ExecuteNonQuery(cmd);

                return result > 0;
            }
        }

        public static UserDailyInc GetDailyIncTotalInfo(string date)
        {
            string sql = @"select count(1) as CountNum,
                                  convert(varchar(10),dt_date_created,120) as CreateTime,
                                  'Total' as Channel
                           from tuhu_profiles..userobject with (nolock)
                           where IsActive =1 and convert(varchar(10),dt_date_created,120) = @dateString
                           group by convert(varchar(10),dt_date_created,120)";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@dateString", date);
                return DbHelper.ExecuteFetch<UserDailyInc>(cmd);
            }
        }

        public static List<UserDailyInc> GetDailyIncInfos(string date)
        {
            string sql = @"select count(1) as CountNum, CreateTime, Channel
                           from (
                            select 
                            UserID,
                            CreateTime,
                            case 
                                when YewuChannel is null then UOChannel
                                when YewuChannel is not null then YewuChannel
                            end as Channel
                            from
                            (
                              select * from
                              (
	                            select UserID,
		                               convert(varchar(10),dt_date_created,120) as CreateTime,
		                               u_application_name as UOChannel
	                            from tuhu_profiles..userobject with (nolock)
	                            where IsActive =1 and convert(varchar(10),dt_date_created,120) =@dateString
                              ) as UO
                              left join 
                              (
                            	select distinct
                            		   UserId as YewuUserId,
                            		   Channel as YewuChannel
                            	from Tuhu_profiles..YewuThirdpartyInfo  with (nolock)
                            	where convert(varchar(10),CreatedTime,120)=@dateString
                              ) as Yewu
                              on UO.UserID=Yewu.YewuUserId
                            ) as Middle
                            ) as Temp
                            group by CreateTime, Channel
                            order by CreateTime, Channel";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@dateString", date);
                return DbHelper.ExecuteSelect<UserDailyInc>(cmd).ToList();
            }
        }

        public static UserDailyInc GetSpecifiedDailyIncInfo(string date, string channel)
        {
            string sql = @"select count(1) as CountNum,
                                  convert(varchar(10),dt_date_created,120) as CreateTime,
                                  u_application_name as Channel
                           from tuhu_profiles..userobject with (nolock)
                           where IsActive =1 and convert(varchar(10),dt_date_created,120) =@dateString
                                 and u_application_name=@channel and IsMobileVerify=1
                           group by convert(varchar(10),dt_date_created,120),u_application_name";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@dateString", date);
                cmd.Parameters.AddWithValue("@channel", channel);
                return DbHelper.ExecuteFetch<UserDailyInc>(cmd);
            }
        }

        public static bool InsertDailyIncInfos(List<UserGrowthSchedule> growthList)
        {
            using (var dbHelper = DbHelper.CreateDbHelper())
            {
                dbHelper.BeginTransaction();

                try
                {
                    foreach (var info in growthList)
                    {
                        var result = InsertDailyIncInfo(dbHelper, info);
                        if (result < 1)
                        {
                            dbHelper.Rollback();
                            return false;
                        }
                    }

                    dbHelper.Commit();
                }
                catch (Exception ex)
                {
                    dbHelper.Rollback();
                    _sublogger.Error(ex.Message, ex);
                    return false;
                }
                return true;
            }
        }

        public static int InsertDailyIncInfo(BaseDbHelper dbHelper, UserGrowthSchedule growth)
        {
            string sql = @"INSERT INTO [Tuhu_profiles].dbo.UserGrowthSchedule
           ([DayString]
           ,[Group]
           ,[Channel]
           ,[IncNum]
           ,[CreateTime]
           ,[LastUpdateDataTime])
     VALUES
           (@DayString
           ,@Group
           ,@Channel
           ,@IncNum
           ,GETDATE()
           ,GETDATE())";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@DayString", growth.DayString);
                cmd.Parameters.AddWithValue("@Group", growth.Group);
                cmd.Parameters.AddWithValue("@Channel", growth.Channel);
                cmd.Parameters.AddWithValue("@IncNum", growth.IncNum);
                return dbHelper.ExecuteNonQuery(cmd);
            }
        }

        public static List<UserChannelGroupV1> GetUserChannelGroupV1()
        {
            string sql = @"select * from [Configuration].dbo.UserChannelGroupV1 with (nolock)";

            using (var cmd = new SqlCommand(sql))
            {
                return DbHelper.ExecuteSelect<UserChannelGroupV1>(cmd).ToList();
            }
        }
        #endregion

        #region Monitor
        public static int GetCount_UnionIdBindMultiUserIds()
        {
            string sql = @"WITH UnionIdBindMultiUserIds
AS ( SELECT   DISTINCT UnionId
     FROM     Tuhu_profiles..UserAuth WITH ( NOLOCK )
     WHERE    AuthSource = 'Weixin'
              AND BindingStatus = 'Bound'
              AND UnionId IS NOT NULL
              AND UnionId <> ''
     GROUP BY UnionId
     HAVING   COUNT(DISTINCT UserId) > 1
   )
SELECT COUNT(1)
FROM   UnionIdBindMultiUserIds;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 120;
                var result = (int)DbHelper.ExecuteScalar(true, cmd);
                return result;
            }
        }

        public static int GetCount_UserIdBindMultiUnionIds()
        {
            string sql = @"WITH UserIdBindMultiUnionIds
AS ( SELECT   DISTINCT UserId
     FROM     Tuhu_profiles..UserAuth WITH ( NOLOCK )
     WHERE    AuthSource = 'Weixin'
              AND BindingStatus = 'Bound'
              AND UnionId IS NOT NULL
              AND UnionId <> ''
     GROUP BY UserId
     HAVING   COUNT(DISTINCT UnionId) > 1
   )
SELECT COUNT(1)
FROM   UserIdBindMultiUnionIds;";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 120;
                var result = (int)DbHelper.ExecuteScalar(true, cmd);
                return result;
            }
        }
        #endregion

        public static bool CheckSwitch(string runtimeSwitch)
        {
            var open = false;

            try
            {
                using (var command = new SqlCommand(@"SELECT [Value] FROM Gungnir.dbo.RuntimeSwitch with (NOLOCK) WHERE SwitchName = @SwitchName"))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SqlParameter("@SwitchName", runtimeSwitch));
                    open = (bool)DbHelper.ExecuteScalar(command);
                }
            }
            catch (Exception ex)
            {
                _sublogger.Error(ex.Message, ex);
            }

            return open;
        }
    }
}
