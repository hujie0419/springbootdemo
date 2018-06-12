using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.SqlServer.Server;
using Tuhu.WebSite.Component.SystemFramework;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Model;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job;
using Common.Logging;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.DAL
{
    public static class DalCoupon
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DalCoupon));
        public const string LunTaiBaoYangFanQuanName = "双11轮胎返保养券活动";
        public const string LunTaiBaoYangFanQuanName1211 = "买轮胎送保养活动返券1211";

        private static readonly string ConnectionStr =
           System.Configuration.ConfigurationManager.ConnectionStrings["Gungnir"]?.ConnectionString;

        public static void InsertBaoYangPromotionData(int promotiontype, string couponDescriptionOfTypeOne, string couponDescriptionOfTypeTwo,
            int ruleId, string messageSubject,
            string messageDetail, int discountOfTypeOne, int discountOfTypeTwo, int minMoneyOfTypeOne, int minMoneyOfTypeTwo, string codeChannel)
        {
            DbCommand cmd = new SqlCommand("[Gungnir].[dbo].[CouponJob_InsertPromotionOfBaoYangCoupon]");
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@Promotiontype", promotiontype));
            cmd.Parameters.Add(new SqlParameter("@RuleId", ruleId));
            cmd.Parameters.Add(new SqlParameter("@DiscountOfTypeOne", discountOfTypeOne));
            cmd.Parameters.Add(new SqlParameter("@DiscountOfTypeTwo", discountOfTypeTwo));
            cmd.Parameters.Add(new SqlParameter("@MinMoneyOfTypeOne", minMoneyOfTypeOne));
            cmd.Parameters.Add(new SqlParameter("@MinMoneyOfTypeTwo", minMoneyOfTypeTwo));
            cmd.Parameters.Add(new SqlParameter("@CouponDescriptionOfTypeOne", couponDescriptionOfTypeOne));
            cmd.Parameters.Add(new SqlParameter("@CouponDescriptionOfTypeTwo", couponDescriptionOfTypeTwo));
            cmd.Parameters.Add(new SqlParameter("@CodeChannel", codeChannel));

            Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteNonQuery(cmd);
        }

        public static void InsertNonBaoYangPromotionData(int promotiontype, string descriptionOfTypeOne, string descriptionOfTypeTwo,
            int ruleId, string messageSubject,
            string messageDetail, int discountOfTypeOne, int discountOfTypeTwo, int minMoneyOfTypeOne, int minMoneyOfTypeTwo, string codeChannel)
        {
            DbCommand cmd = new SqlCommand("[Gungnir].[dbo].[CouponJob_InsertPromotionOfNotBaoYangCoupon]");
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@Promotiontype", promotiontype));
            cmd.Parameters.Add(new SqlParameter("@RuleId", ruleId));
            cmd.Parameters.Add(new SqlParameter("@DiscountOfTypeOne", discountOfTypeOne));
            cmd.Parameters.Add(new SqlParameter("@DiscountOfTypeTwo", discountOfTypeTwo));
            cmd.Parameters.Add(new SqlParameter("@MinMoneyOfTypeOne", minMoneyOfTypeOne));
            cmd.Parameters.Add(new SqlParameter("@MinMoneyOFTypeTwo", minMoneyOfTypeTwo));
            cmd.Parameters.Add(new SqlParameter("@DescriptionOfTypeOne", descriptionOfTypeOne));
            cmd.Parameters.Add(new SqlParameter("@DescriptionOfTypeTwo", descriptionOfTypeTwo));
            cmd.Parameters.Add(new SqlParameter("@CodeChannel", codeChannel));

            Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteNonQuery(cmd);
        }

        public static void InsertFreeOrderCoupon(int promotionType, string couponDescription, string codeChannel)
        {
            using (DbCommand cmd = new SqlCommand("[Gungnir].[dbo].[CouponJob_InsertFreeOrderCoupon]"))
            {
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Promotiontype", promotionType));
                cmd.Parameters.Add(new SqlParameter("@CodeChannel", codeChannel));
                cmd.Parameters.Add(new SqlParameter("@Description", couponDescription));

                Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteNonQuery(cmd);
            }
        }

        public static List<User> GetBaoYangUserDetailNeedPush()
        {
            var users = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, "[Gungnir].[dbo].[CouponJob_GetBaoYangUserDetailNeedPush]",
                CommandType.StoredProcedure).Rows.OfType<DataRow>().Select(x => new User
                {
                    UserId = x["UserID"] == null ? Guid.Empty : new Guid(x["UserID"].ToString()),
                    Cellphone = x["u_mobile_number"] == null ? string.Empty : x["u_mobile_number"].ToString(),
                    DeviceId = x["Device_Tokens"] == null ? string.Empty : x["Device_Tokens"].ToString(),
                    Channel = x["Channel"] == null ? string.Empty : x["Channel"].ToString(),
                    UserName = x["u_email_address"] == null ? string.Empty : x["u_email_address"].ToString(),
                    Code = x["Code"] == null ? string.Empty : x["Code"].ToString()
                }).ToList();

            return users;
        }

        public static List<User> GetBaoYangUserDetailNeedSend()
        {
            var users = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, "[Gungnir].[dbo].[CouponJob_GetBaoYangUserDetailNeedSend]",
                CommandType.StoredProcedure).Rows.OfType<DataRow>().Select(x => new User
                {
                    UserId = x["UserID"] == null ? Guid.Empty : new Guid(x["UserID"].ToString()),
                    Cellphone = x["u_mobile_number"] == null ? string.Empty : x["u_mobile_number"].ToString(),
                    DeviceId = x["Device_Tokens"] == null ? string.Empty : x["Device_Tokens"].ToString(),
                    Channel = x["Channel"] == null ? string.Empty : x["Channel"].ToString(),
                    UserName = x["u_email_address"] == null ? string.Empty : x["u_email_address"].ToString(),
                    Code = x["Code"] == null ? string.Empty : x["Code"].ToString()
                }).ToList();

            return users;
        }

        public static List<User> GetFreeOrderCouponUserNeedSend()
        {
            List<User> users = null;

            using (var cmd = new SqlCommand(@"
                    SELECT tca.UserId, uo.u_mobile_number AS Cellphone, tca.Code
                    FROM Gungnir.dbo.TempCouponActivity (NOLOCK) tca
                    JOIN Gungnir.dbo.tbl_PromotionCode (NOLOCK) pc ON tca.Code = pc.Code
                    JOIN Tuhu_profiles..UserObject (NOLOCK) uo ON pc.UserId = uo.UserID
                    WHERE pc.[Status] = 0 AND tca.ActivityNo = 1 AND tca.SendSMSTimes = 0
                "))
            {
                users = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd).Rows.OfType<DataRow>().Select(x => new User()
                {
                    UserId = x["UserID"] == null ? Guid.Empty : new Guid(x["UserID"].ToString()),
                    Cellphone = x["Cellphone"] == null ? string.Empty : x["Cellphone"].ToString(),
                    Code = x["Code"] == null ? string.Empty : x["Code"].ToString()
                }).ToList();
            }

            return users;
        }

        public static List<User> GetNonBaoYangUserDetailNeedPush()
        {
            var users = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, "[Gungnir].[dbo].[CouponJob_GetNotBaoYangUserDetailNeedPush]",
                CommandType.StoredProcedure).Rows.OfType<DataRow>().Select(x => new User
                {
                    UserId = x["UserID"] == null ? Guid.Empty : new Guid(x["UserID"].ToString()),
                    Cellphone = x["u_mobile_number"] == null ? string.Empty : x["u_mobile_number"].ToString(),
                    DeviceId = x["Device_Tokens"] == null ? string.Empty : x["Device_Tokens"].ToString(),
                    Channel = x["Channel"] == null ? string.Empty : x["Channel"].ToString(),
                    UserName = x["u_email_address"] == null ? string.Empty : x["u_email_address"].ToString(),
                    Code = x["Code"] == null ? string.Empty : x["Code"].ToString()
                }).ToList();
            return users;
        }

        public static List<User> GetNonBaoYangUserDetailNeedSend()
        {
            var users = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, "[Gungnir].[dbo].[CouponJob_GetNotBaoYangUserDetailNeedSend]",
                CommandType.StoredProcedure).Rows.OfType<DataRow>().Select(x => new User
                {
                    UserId = x["UserID"] == null ? Guid.Empty : new Guid(x["UserID"].ToString()),
                    Cellphone = x["u_mobile_number"] == null ? string.Empty : x["u_mobile_number"].ToString(),
                    DeviceId = x["Device_Tokens"] == null ? string.Empty : x["Device_Tokens"].ToString(),
                    Channel = x["Channel"] == null ? string.Empty : x["Channel"].ToString(),
                    UserName = x["u_email_address"] == null ? string.Empty : x["u_email_address"].ToString(),
                    Code = x["Code"] == null ? string.Empty : x["Code"].ToString()
                }).ToList();

            return users;
        }

        public static void UpdateSendTimesByCode(List<string> codes)
        {
            for (var i = 0; i < (codes.Count - 1) / 500 + 1; i++)
            {
                string codeStr;
                if (i == ((codes.Count - 1) / 500))
                {
                    codeStr = "";
                    for (var j = i * 500; j < codes.Count; j++)
                    {
                        codeStr += ',' + codes[j];
                    }
                    var parameter = new SqlParameter("@CodeStr", codeStr);
                    Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteNonQuery(false, "[Gungnir].[dbo].[CouponJob_UpdateSendTimesByCode]",
                        CommandType.StoredProcedure, parameter);
                }
                else
                {
                    codeStr = "";
                    for (var j = i * 500; j < (i + 1) * 500; j++)
                    {
                        codeStr += ',' + codes[j];
                    }
                    var parameter = new SqlParameter("@CodeStr", codeStr);
                    Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteNonQuery(false, "[Gungnir].[dbo].[CouponJob_UpdateSendTimesByCode]",
                        CommandType.StoredProcedure, parameter);
                }
            }
        }

        public static void UpdateSMSTimesByCode(List<string> codes)
        {
            if (codes != null && codes.Any())
            {
                for (int i = 0; i < (codes.Count - 1) / 500 + 1; i++)
                {
                    var updateCodes = codes.Skip(i * 500).Take(500);
                    var codeStr = string.Join(",", updateCodes);
                    using (var cmd = new SqlCommand(@"Gungnir..CouponJob_UpdateSMSTimesByCode"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Codes", codeStr);
                        Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteNonQuery(false, cmd);
                    }
                }
            }
        }

        public static void UpdatePushTimesByCode(List<string> codes)
        {
            for (var i = 0; i < (codes.Count - 1) / 500 + 1; i++)
            {
                string codeStr;
                if (i == ((codes.Count - 1) / 500))
                {
                    codeStr = "";
                    for (var j = i * 500; j < codes.Count; j++)
                    {
                        codeStr += ',' + codes[j];
                    }
                    var parameter = new SqlParameter("@CodeStr", codeStr);
                    Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteNonQuery(false, "[Gungnir].[dbo].[CouponJob_UpdatePushTimesByCode]",
                        CommandType.StoredProcedure, parameter);
                }
                else
                {
                    codeStr = "";
                    for (var j = i * 500; j < (i + 1) * 500; j++)
                    {
                        codeStr += ',' + codes[j];
                    }
                    var parameter = new SqlParameter("@CodeStr", codeStr);
                    Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteNonQuery(false, "[Gungnir].[dbo].[CouponJob_UpdatePushTimesByCode]",
                        CommandType.StoredProcedure, parameter);
                }
            }
        }

        public static void CouponPushMessageLog(List<CouponLogModel> users)
        {
            foreach (var user in users)
            {
                DbParameter[] parameters =
                {
                        new SqlParameter("@UserId", user.UserId),
                        new SqlParameter("@UserName", user.UserName),
                        new SqlParameter("@Channel", user.Channel),
                        new SqlParameter("@Status", user.Status),
                        new SqlParameter("@Subject", user.Subject),
                        new SqlParameter("@PhoneNum", user.PhoneNum)
                    };
                Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteNonQuery(false, "[Gungnir].[dbo].[CouponJob_CouponPushMessageFailLog]",
                    CommandType.StoredProcedure, parameters);
            }
        }

        public static DataTable GetAllEndLunTaiOrderList()
        {
            string sql = @"SELECT
                                                        O.UserID,
                                                        SUM(OL.Num) AS NUM
                                                FROM    Gungnir..tbl_Order AS O WITH(NOLOCK)
                                                JOIN    Gungnir.dbo.tbl_OrderList AS OL WITH(NOLOCK) ON O.PKID = OL.OrderID
                                                JOIN  Gungnir..tbl_ChannelDictionaries AS CD WITH(NOLOCK) ON CD.ChannelKey = O.OrderChannel
                                                WHERE((O.Status = '3Installed'
                                                                        AND O.InstallStatus = '2Installed'
                                                                        AND O.InstallShopID > 0
                                                                        AND O.InstallType = '1ShopInstall'
                                                                      )
                                                                      OR(ISNULL(O.InstallShopID, 0) = 0
                                                                           AND O.Status = '2Shipped'
                                                                           AND O.DeliveryStatus IN('3.5Signed')
                                                                         )
                                                                    )
                                                AND(O.OrderDatetime >= '2016-11-09 09:00:00' AND O.OrderDatetime <= '2016-11-14 12:00:00')
                                                AND OL.ProductType & 32 <>32
                                                AND OL.PID LIKE N'TR-%'
                                                AND (CD.ChannelType = N'自有渠道' or CD.ChannelType=N'H5合作渠道')
                                                AND O.Status <> '7Canceled'
                                                AND NOT EXISTS(SELECT 1
                                                                    FROM    SystemLog.dbo.tbl_PromotionCheJi AS PC WITH(NOLOCK)
                                                                    WHERE   PC.UserId = O.UserID
                                                                        AND PC.ActivityCode = @ActivityCode) GROUP BY O.UserID having(SUM(OL.Num)) > 3";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 5 * 60;
                cmd.Parameters.AddWithValue("@ActivityCode", LunTaiBaoYangFanQuanName);
                var dt = Tuhu.DbHelper.ExecuteQuery(cmd, one => one);
                return dt;
            }
        }

        public static List<OrderList> GetHasEndLunTaiOrderList()
        {
            string sql = @"SELECT
														O.UserID,
														OL.Num,
														O.Status,
														O.InstallStatus,
														O.InstallType,
														O.InstallShopID,
														O.DeliveryStatus
                                                FROM	Gungnir..tbl_Order AS O WITH ( NOLOCK )
												JOIN    Gungnir.dbo.tbl_OrderList AS OL WITH ( NOLOCK ) ON O.PKID = OL.OrderID
												JOIN  Gungnir..tbl_ChannelDictionaries AS CD WITH(NOLOCK) ON CD.ChannelKey=O.OrderChannel
                                                WHERE
                                                (O.OrderDatetime> = '2016-11-09 09:00:00' AND O.OrderDatetime <= '2016-11-14 12:00:00' )
                                                AND  OL.PID LIKE N'TR-%'
												AND (CD.ChannelType = N'自有渠道' or CD.ChannelType=N'H5合作渠道')
                                                AND OL.ProductType & 32 <>32
												AND  O.Status <> '7Canceled'
												AND
NOT EXISTS ( SELECT	1
						                                            FROM	SystemLog.dbo.tbl_PromotionCheJi AS PC WITH ( NOLOCK )
						                                            WHERE	PC.UserId = O.UserID
								                                        AND PC.ActivityCode = @ActivityCode )";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 5 * 60;
                cmd.Parameters.AddWithValue("@ActivityCode", LunTaiBaoYangFanQuanName);
                return Tuhu.DbHelper.ExecuteSelect<OrderList>(true, cmd).ToList();
            }
        }

        public static int CreateMultiplePromotionCode(PromotionCodeModel promotionCodeModel)
        {
            using (var db = DbHelper.CreateDbHelper(ConnectionStr))
            {
                try
                {
                    db.BeginTransaction();
                    using (var cmd1 = new SqlCommand(@"IF NOT EXISTS ( SELECT  1
                                                                                    FROM	SystemLog..tbl_PromotionCheJi AS PC WITH ( NOLOCK )
				                                                                    WHERE	PC.UserID = @UserID
				                                                                    AND ActivityCode = @ActivityCode )
	                                                                    BEGIN
	                                                                     INSERT	INTO SystemLog..tbl_PromotionCheJi
				                                                                    (
				                                                                      DeviceID,
				                                                                      ActivityCode,
				                                                                      UserID,
				                                                                      GetRuleID,
				                                                                      CreateTime,
				                                                                      LastUpdateTime,
				                                                                      Remark )
			                                                                    VALUES(
				                                                                    @DeviceID,
				                                                                    @ActivityCode,
				                                                                    @UserID,
				                                                                    @GetRuleID,
				                                                                    GETDATE(),
				                                                                    GETDATE(),
				                                                                    @Remark
				                                                                    )
		                                                                    SET @Result = @@ROWCOUNT
	                                                                    END
                                                                    ELSE
	                                                                    BEGIN
		                                                                    SET @Result = -1
	                                                                    END
                                                                    "))
                    {
                        cmd1.CommandType = CommandType.Text;
                        cmd1.Parameters.AddWithValue("@DeviceID", "");
                        cmd1.Parameters.AddWithValue("@ActivityCode", LunTaiBaoYangFanQuanName);
                        cmd1.Parameters.AddWithValue("@GetRuleID", promotionCodeModel.RuleId);
                        cmd1.Parameters.AddWithValue("@UserID", promotionCodeModel.UserId);
                        cmd1.Parameters.AddWithValue("@Remark", "用户" + promotionCodeModel.UserId + "双11轮胎返券" + promotionCodeModel.Number + "张");
                        cmd1.Parameters.Add("@Result", SqlDbType.Int).Direction = ParameterDirection.Output; //输出标示
                        db.ExecuteNonQuery(cmd1);
                        int result = Convert.ToInt32(cmd1.Parameters["@Result"].Value);
                        if (result <= 0)
                        {
                            db.Rollback();
                            return -1;
                        }
                    }
                    using (var cmd = new SqlCommand("[Gungnir].[dbo].[Beautify_CreatePromotionCode]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@StartTime", promotionCodeModel.StartTime.ToString());
                        cmd.Parameters.AddWithValue("@EndDateTime", promotionCodeModel.EndTime.ToString());
                        cmd.Parameters.AddWithValue("@Type", promotionCodeModel.Type);
                        cmd.Parameters.AddWithValue("@Description", promotionCodeModel.Description);
                        cmd.Parameters.AddWithValue("@Discount", promotionCodeModel.Discount);
                        cmd.Parameters.AddWithValue("@MinMoney", promotionCodeModel.MinMoney);
                        cmd.Parameters.AddWithValue("@Number", promotionCodeModel.Number);
                        cmd.Parameters.AddWithValue("@CodeChannel", promotionCodeModel.CodeChannel);
                        cmd.Parameters.AddWithValue("@UserID", promotionCodeModel.UserId);
                        cmd.Parameters.AddWithValue("@BatchID", promotionCodeModel.BatchId == null ? 0 : promotionCodeModel.BatchId.Value);
                        cmd.Parameters.AddWithValue("@RuleID", promotionCodeModel.RuleId);
                        cmd.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@Results",
                            DbType = DbType.Int32,
                            Direction = ParameterDirection.Output,
                            Value = 0
                        });

                        db.ExecuteNonQuery(cmd);
                        var result = Convert.ToInt32(cmd.Parameters["@Results"].Value);
                        if (result > 0) //领券成功
                        {
                            db.Commit();
                        }
                        else
                        {
                            db.Rollback();
                        }
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message + promotionCodeModel?.UserId, ex);
                    db.Rollback();
                    return -99;
                }
            }
        }

        public static DataTable Get1211AllEndLunTaiOrderList()
        {
            string sql = @"SELECT
                                                        O.UserID,
                                                        SUM(OL.Num) AS NUM
                                                FROM    Gungnir..tbl_Order AS O WITH(NOLOCK)
                                                JOIN    Gungnir.dbo.tbl_OrderList AS OL WITH(NOLOCK) ON O.PKID = OL.OrderID
                                                JOIN  Gungnir..tbl_ChannelDictionaries AS CD WITH(NOLOCK) ON CD.ChannelKey = O.OrderChannel
                                                WHERE ((O.Status = '3Installed'
                                                                        AND O.InstallStatus = '2Installed'
                                                                        AND O.InstallShopID > 0
                                                                        AND O.InstallType = '1ShopInstall'
                                                                     )
                                                                      OR(ISNULL(O.InstallShopID, 0) = 0
                                                                           AND O.Status = '2Shipped'
                                                                           AND O.DeliveryStatus IN('3.5Signed')
                                                                         )
                                                                    )
                                                AND (O.OrderDatetime >= '2016-11-30 12:00:00' AND O.OrderDatetime <= '2016-12-11 10:00:00')
                                                AND OL.ProductType & 32 <>32
                                                AND OL.PID LIKE N'TR-%'
                                                AND (CD.ChannelType = N'自有渠道' or CD.ChannelType=N'H5合作渠道')
                                                AND O.Status <> '7Canceled'
                                                AND NOT EXISTS(SELECT 1
                                                                    FROM    SystemLog.dbo.tbl_JobCreatePromotionLog AS PC WITH(NOLOCK)
                                                                    WHERE   PC.UserId = O.UserID
                                                                        AND PC.ActivityCode = @ActivityCode)
                                                GROUP BY O.UserID having(SUM(OL.Num)) > 3";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 5 * 60;
                cmd.Parameters.AddWithValue("@ActivityCode", LunTaiBaoYangFanQuanName1211);
                var dt = Tuhu.DbHelper.ExecuteQuery(cmd, one => one);
                return dt;
            }
        }

        public static List<OrderList> GetHas1211EndLunTaiOrderList()
        {
            string sql = @"SELECT
														O.UserID,
														OL.Num,
														O.Status,
														O.InstallStatus,
														O.InstallType,
														O.InstallShopID,
														O.DeliveryStatus
                                                FROM	Gungnir..tbl_Order AS O WITH ( NOLOCK )
												JOIN    Gungnir.dbo.tbl_OrderList AS OL WITH ( NOLOCK ) ON O.PKID = OL.OrderID
												JOIN  Gungnir..tbl_ChannelDictionaries AS CD WITH(NOLOCK) ON CD.ChannelKey=O.OrderChannel
                                                WHERE
                                                (O.OrderDatetime >= '2016-11-30 12:00:00' AND O.OrderDatetime <= '2016-12-11 10:00:00')
                                                AND  OL.PID LIKE N'TR-%'
												AND (CD.ChannelType = N'自有渠道' or CD.ChannelType=N'H5合作渠道')
                                                AND  OL.ProductType & 32 <>32
												AND  O.Status <> '7Canceled'
												AND
NOT EXISTS ( SELECT	1
						                                            FROM	SystemLog.dbo.tbl_JobCreatePromotionLog AS PC WITH ( NOLOCK )
						                                            WHERE	PC.UserId = O.UserID
								                                        AND PC.ActivityCode = @ActivityCode )";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 5 * 60;
                cmd.Parameters.AddWithValue("@ActivityCode", LunTaiBaoYangFanQuanName1211);
                return Tuhu.DbHelper.ExecuteSelect<OrderList>(true, cmd).ToList();
            }
        }

        public static int Create1211MultiplePromotionCode(PromotionCodeModel promotionCodeModel1, PromotionCodeModel promotionCodeModel2, int num)
        {
            using (var db = DbHelper.CreateDbHelper(ConnectionStr))
            {
                try
                {
                    db.BeginTransaction();
                    using (var cmd1 = new SqlCommand(@"IF NOT EXISTS ( SELECT  1
                                                                                    FROM	SystemLog..tbl_JobCreatePromotionLog AS PC WITH ( NOLOCK )
				                                                                    WHERE	PC.UserID = @UserID
				                                                                    AND ActivityCode = @ActivityCode )
	                                                                    BEGIN
	                                                                     INSERT	INTO SystemLog..tbl_JobCreatePromotionLog
				                                                                    (
				                                                                      ActivityCode,
				                                                                      UserID,
				                                                                      CreateTime,
				                                                                      LastUpdateTime,
				                                                                      Remark )
			                                                                    VALUES(
				                                                                    @ActivityCode,
				                                                                    @UserID,
				                                                                    GETDATE(),
				                                                                    GETDATE(),
				                                                                    @Remark
				                                                                    )
		                                                                    SET @Result = @@ROWCOUNT
	                                                                    END
                                                                    ELSE
	                                                                    BEGIN
		                                                                    SET @Result = -1
	                                                                    END
                                                                    "))
                    {
                        cmd1.CommandType = CommandType.Text;
                        cmd1.Parameters.AddWithValue("@ActivityCode", LunTaiBaoYangFanQuanName1211);
                        cmd1.Parameters.AddWithValue("@UserID", promotionCodeModel1.UserId);
                        cmd1.Parameters.AddWithValue("@Remark", "用户" + promotionCodeModel1.UserId + "买轮胎送保养活动返券" + (num > 2 ? 2 : 1) + "张");
                        cmd1.Parameters.Add("@Result", SqlDbType.Int).Direction = ParameterDirection.Output; //输出标示
                        db.ExecuteNonQuery(cmd1);
                        int result = Convert.ToInt32(cmd1.Parameters["@Result"].Value);
                        if (result <= 0)
                        {
                            db.Rollback();
                            return -1;
                        }
                    }

                    #region 第一张券

                    using (var cmd = new SqlCommand("[Gungnir].[dbo].[Beautify_CreatePromotionCode]"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@StartTime", promotionCodeModel1.StartTime.ToString());
                        cmd.Parameters.AddWithValue("@EndDateTime", promotionCodeModel1.EndTime.ToString());
                        cmd.Parameters.AddWithValue("@Type", promotionCodeModel1.Type);
                        cmd.Parameters.AddWithValue("@Description", promotionCodeModel1.Description);
                        cmd.Parameters.AddWithValue("@Discount", promotionCodeModel1.Discount);
                        cmd.Parameters.AddWithValue("@MinMoney", promotionCodeModel1.MinMoney);
                        cmd.Parameters.AddWithValue("@Number", promotionCodeModel1.Number);
                        cmd.Parameters.AddWithValue("@CodeChannel", promotionCodeModel1.CodeChannel);
                        cmd.Parameters.AddWithValue("@UserID", promotionCodeModel1.UserId);
                        cmd.Parameters.AddWithValue("@BatchID", promotionCodeModel1.BatchId == null ? 0 : promotionCodeModel1.BatchId.Value);
                        cmd.Parameters.AddWithValue("@RuleID", promotionCodeModel1.RuleId);
                        cmd.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@Results",
                            DbType = DbType.Int32,
                            Direction = ParameterDirection.Output,
                            Value = 0
                        });

                        db.ExecuteNonQuery(cmd);
                        var result = Convert.ToInt32(cmd.Parameters["@Results"].Value);
                        if (result > 0) //领券成功
                        {
                            if (num < 3) //只有一张券
                            {
                                db.Commit();
                                return result;
                            }
                        }
                        else
                        {
                            db.Rollback();
                            return result;
                        }
                    }

                    #endregion 第一张券

                    #region 第二张券

                    if (num >= 3)//有第二张券
                    {
                        using (var cmd2 = new SqlCommand("[Gungnir].[dbo].[Beautify_CreatePromotionCode]"))
                        {
                            cmd2.CommandType = CommandType.StoredProcedure;

                            cmd2.Parameters.AddWithValue("@StartTime", promotionCodeModel2.StartTime.ToString());
                            cmd2.Parameters.AddWithValue("@EndDateTime", promotionCodeModel2.EndTime.ToString());
                            cmd2.Parameters.AddWithValue("@Type", promotionCodeModel2.Type);
                            cmd2.Parameters.AddWithValue("@Description", promotionCodeModel2.Description);
                            cmd2.Parameters.AddWithValue("@Discount", promotionCodeModel2.Discount);
                            cmd2.Parameters.AddWithValue("@MinMoney", promotionCodeModel2.MinMoney);
                            cmd2.Parameters.AddWithValue("@Number", promotionCodeModel2.Number);
                            cmd2.Parameters.AddWithValue("@CodeChannel", promotionCodeModel2.CodeChannel);
                            cmd2.Parameters.AddWithValue("@UserID", promotionCodeModel2.UserId);
                            cmd2.Parameters.AddWithValue("@BatchID", promotionCodeModel2.BatchId == null ? 0 : promotionCodeModel2.BatchId.Value);
                            cmd2.Parameters.AddWithValue("@RuleID", promotionCodeModel2.RuleId);
                            cmd2.Parameters.Add(new SqlParameter()
                            {
                                ParameterName = "@Results",
                                DbType = DbType.Int32,
                                Direction = ParameterDirection.Output,
                                Value = 0
                            });

                            db.ExecuteNonQuery(cmd2);
                            var result = Convert.ToInt32(cmd2.Parameters["@Results"].Value);
                            if (result > 0) //领券成功
                            {
                                db.Commit();
                            }
                            else
                            {
                                db.Rollback();
                            }
                            return result;
                        }
                    }
                    else
                    {
                        return -2;
                    }

                    #endregion 第二张券
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message + promotionCodeModel1?.UserId, ex);
                    db.Rollback();
                    return -99;
                }
            }
        }

        public static DataTable Get1211BaoYangFreeOrderList()
        {
            string sql = @"SELECT	P.RuleID,
		                                                        O.UserID,
		                                                        O.UserTel,
		                                                        O.SumMoney,
		                                                        O.PKID AS OrderID
                                                        FROM	Gungnir..tbl_Order AS O WITH ( NOLOCK )
                                                        JOIN	Gungnir..tbl_PromotionCode AS P WITH ( NOLOCK )
		                                                        ON O.PKID = P.OrderId
                                                        WHERE	P.GetRuleID=1774
		                                                        AND  O.Status = N'3Installed' AND  O.InstallDatetime < (GETDATE() - 1)
                                                                AND NOT EXISTS ( SELECT	1
						                                                            FROM	Gungnir..tbl_PromotionCode AS PC WITH ( NOLOCK )
						                                                            WHERE	PC.UserId = O.UserID
								                                                        AND PC.RuleID = 1073
								                                                        AND PC.BatchID = O.PKID
								                                                        AND PC.CodeChannel = N'双12保养免单活动'
                                                                                        AND (PC.Status <> 3 OR (PC.Status = 3 AND PC.UsedTime IS NOT NULL))
                                                                                         )";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 5 * 60;
                var dt = Tuhu.DbHelper.ExecuteQuery(cmd, one => one);
                return dt;
            }
        }

        public static DataTable Get1219BaoYangFreeOrderList()
        {
            string sql = @"SELECT	P.RuleID,
		                                                        O.UserID,
		                                                        O.UserTel,
		                                                        O.SumMoney,
		                                                        O.PKID AS OrderID
                                                        FROM	Gungnir..tbl_Order AS O WITH ( NOLOCK )
                                                        JOIN	Gungnir..tbl_PromotionCode AS P WITH ( NOLOCK )
		                                                        ON O.PKID = P.OrderId
                                                        WHERE	( P.GetRuleID=1880 OR P.GetRuleID=1882 )
		                                                        AND  O.Status = N'3Installed' AND  O.InstallDatetime < (GETDATE() - 1)
                                                                AND NOT EXISTS ( SELECT	1
						                                                            FROM	Gungnir..tbl_PromotionCode AS PC WITH ( NOLOCK )
						                                                            WHERE	 PC.RuleID = 23
								                                                        AND PC.BatchID = O.PKID
								                                                        AND PC.CodeChannel = N'保养买一送一返券活动'
                                                                                        AND (PC.Status <> 3 OR (PC.Status = 3 AND PC.UsedTime IS NOT NULL))
                                                                                         )";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 5 * 60;
                var dt = Tuhu.DbHelper.ExecuteQuery(cmd, one => one);
                return dt;
            }
        }

        public static IEnumerable<OrderItem> SelectOrderListByOrderId(string orderId)
        {
            using (var cmd = new SqlCommand(@"SELECT
                                            PKID,OrderID,OrderNo,PID,Poid,Category,Name,
                                            Size,Remark,CarCode,CarName,Num,MarkedPrice,Discount,Price,TotalDiscount,
                                            TotalPrice,LastUpdateTime,PurchasePrice,Cost,InstallFee,Vendor,IsPaid,PaidVia,
                                            InstockDate,PaidDate,PurchaseStatus,IsInstallFeePaid,InstallFeePaidDate,Deleted,
                                            CreateDate,Commission,HCNum,OrigProdId,ProductType,ParentID,WeekYear,RefID,
                                            FUPID,PromotionCode,PromotionMoney,MatchedProducts,ExtCol,IsDaiFa,NodeNo,
                                            TotalManualDiscount,ListPrice,PayPrice,TuhuCost
                                            FROM Gungnir..tbl_OrderList WITH ( NOLOCK ) WHERE OrderID=@OrderID AND Deleted = 0 AND ( ProductType IS NULL OR ProductType & 32 <>32 ) "))
            {
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.Rows.OfType<DataRow>().Select(x => new OrderItem(x)).ToList();
                }
                return null;
            }
        }

        public static int CreatePromotionCode(PromotionCodeModel promotionCodeModel, BaseDbHelper db = null, string jobName = null,string departmentName=null,string intentionName=null)
        {
            try
            {
                using (var cmd = new SqlCommand("[Gungnir].[dbo].[Beautify_CreatePromotionCode]"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@StartTime", promotionCodeModel.StartTime.ToString());
                    cmd.Parameters.AddWithValue("@EndDateTime", promotionCodeModel.EndTime.ToString());
                    cmd.Parameters.AddWithValue("@Type", promotionCodeModel.Type);
                    cmd.Parameters.AddWithValue("@Description", promotionCodeModel.Description);
                    cmd.Parameters.AddWithValue("@Discount", promotionCodeModel.Discount);
                    cmd.Parameters.AddWithValue("@MinMoney", promotionCodeModel.MinMoney);
                    cmd.Parameters.AddWithValue("@Number", promotionCodeModel.Number);
                    cmd.Parameters.AddWithValue("@CodeChannel", promotionCodeModel.CodeChannel);
                    cmd.Parameters.AddWithValue("@UserID", promotionCodeModel.UserId);
                    cmd.Parameters.AddWithValue("@BatchID", promotionCodeModel.BatchId == null ? 0 : promotionCodeModel.BatchId.Value);
                    cmd.Parameters.AddWithValue("@RuleID", promotionCodeModel.RuleId);
                    cmd.Parameters.AddWithValue("@GetRuleID", promotionCodeModel.GetRuleId);
                    cmd.Parameters.AddWithValue("@PromtionName", promotionCodeModel.PromtionName);
                    cmd.Parameters.AddWithValue("@Issuer", "zhanglingjia@tuhu.cn");
                    cmd.Parameters.AddWithValue("@IssueChannle", "自动返券脚本");
                    cmd.Parameters.AddWithValue("@IssueChannleId", jobName);
                    cmd.Parameters.AddWithValue("@Creater", "zhanglingjia@tuhu.cn");
                    cmd.Parameters.AddWithValue("@DepartmentName", departmentName??"运营");
                    cmd.Parameters.AddWithValue("@IntentionName", intentionName ?? "常规促销");
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@Results",
                        DbType = DbType.Int32,
                        Direction = ParameterDirection.Output,
                        Value = 0
                    });
                    if (db != null)
                    {
                        db.ExecuteNonQuery(cmd);
                    }
                    else
                    {
                        DbHelper.ExecuteNonQuery(cmd);
                    }
                    var result = Convert.ToInt32(cmd.Parameters["@Results"].Value);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return -99;
            }
        }

        public static int GetOrderPromotionMoney(List<OrderItem> items, decimal oilPrice, decimal otherPrice)
        {
            var oil = items.Where(x => x.Pid.LastIndexOf("OL-") == 0).ToList();
            var other = items.Where(x => x.Pid.LastIndexOf("OL-") != 0).ToList();
            var oilMoney = oil.Select(x => x.TotalPrice).AsQueryable().Sum();
            if (oilMoney > oilPrice)
                oilMoney = oilPrice;

            var otherMoney = other.Select(x => x.TotalPrice).AsQueryable().Sum();
            var m = Convert.ToInt32((otherMoney + oilMoney).ToString("0"));
            if (m > otherPrice)
                m = Convert.ToInt32(otherPrice);
            return m;
        }

        public static DataTable Get0208BaoYangFreeOrderList()
        {
            string sql = @"SELECT	P.RuleID,
		                                                        O.UserID,
		                                                        O.UserTel,
		                                                        O.SumMoney,
		                                                        O.PKID AS OrderID
                                                        FROM	Gungnir..tbl_Order AS O WITH ( NOLOCK )
                                                        JOIN	Gungnir..tbl_PromotionCode AS P WITH ( NOLOCK )
		                                                        ON O.PKID = P.OrderId
                                                        WHERE	 P.GetRuleID=2468
		                                                        AND  O.Status = N'3Installed' AND  O.InstallDatetime < (GETDATE() - 1)
                                                                AND  NOT EXISTS ( SELECT	1
						                                                            FROM	Gungnir..tbl_PromotionCode AS PC WITH ( NOLOCK )
						                                                            WHERE	 PC.RuleID = 10604
								                                                        AND PC.BatchID = O.PKID
								                                                        AND PC.CodeChannel = N'开春保养买一送一返券活动'
                                                                                        AND (PC.Status <> 3 OR (PC.Status = 3 AND PC.UsedTime IS NOT NULL))
                                                                                         )";

            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 5 * 60;
                var dt = Tuhu.DbHelper.ExecuteQuery(cmd, one => one);
                return dt;
            }
        }

        public static DataTable Get0310BaoYangFreeOrderList()
        {
            //使用了(优惠券规则编号：2900/优惠券规则Guid：48a977d4-84c9-401e-a9c2-fabccb519785)的优惠券的订单
            string sql = @"SELECT    O.UserID,
		                             O.UserTel,
		                             O.SumMoney,
		                             O.PKID AS OrderID,
                                     IsMenDian=0
                         FROM	Gungnir..tbl_Order AS O WITH ( NOLOCK )
                         JOIN	Gungnir..tbl_PromotionCode AS P WITH ( NOLOCK )
		                         ON O.PKID = P.OrderId
                         WHERE	 P.GetRuleID=2900
		                          AND  O.Status = N'3Installed' AND  O.InstallDatetime < (GETDATE() - 1)
                                  AND  NOT EXISTS ( SELECT	1
						                            FROM	Gungnir..tbl_PromotionCode AS PC WITH ( NOLOCK )
						                            WHERE	 PC.RuleID = 29627
								                             AND PC.BatchID = O.PKID
								                             AND PC.CodeChannel = N'途虎工场店出光保养买一送一活动'
                                                             AND (PC.Status <> 3 OR (PC.Status = 3 AND PC.UsedTime IS NOT NULL))
                                                                                         )
                        UNION ALL
                        SELECT  O.UserID,
		                        O.UserTel,
		                        O.SumMoney,
		                        O.PKID AS OrderID,
                                IsMenDian=1
                        FROM gungnir.dbo.tbl_Order  O  with(nolock)
                        left join gungnir.dbo.tbl_orderlist OL with(nolock) on o.pkid=ol.orderid
                        left join Tuhu_productcatalog..vw_Products vw with(nolock) on ol.pid=vw.pid COLLATE Chinese_PRC_CI_AS
                        WHERE O.OrderChannel=N'u门店' and O.Status= N'3Installed' AND  O.InstallDatetime < (GETDATE() - 1)
                          and O.InstallShopId IN(SELECT pkid FROM gungnir.dbo.Shops  where shoptype&512=512 and RegionID in (5,1856))
                          and vw.Category=N'OIL' and vw.CP_Brand=N'出光/IDEMITSU'
                          AND  NOT EXISTS ( SELECT	1
						                            FROM	Gungnir..tbl_PromotionCode AS PC WITH ( NOLOCK )
						                            WHERE	 PC.RuleID = 29627
								                             AND PC.BatchID = O.PKID
								                             AND PC.CodeChannel = N'途虎工场店出光保养买一送一活动'
                                                             AND (PC.Status <> 3 OR (PC.Status = 3 AND PC.UsedTime IS NOT NULL))
                                                                                         )";
            //销售渠道：u门店，安装门店：所有深圳、东莞工场店，订单内包含出光品牌机油的保养订单
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 5 * 60;
                var dt = Tuhu.DbHelper.ExecuteQuery(cmd, one => one);
                return dt;
            }
        }

        public static IEnumerable<OrderItem> SelectNoFuOrderListByOrderId(List<string> orderIds)
        {
            string SQL = string.Format("SELECT Pid,Num,Price,TotalPrice FROM Gungnir..tbl_OrderList WITH ( NOLOCK ) WHERE OrderID IN({0}) AND Deleted = 0 AND ( ProductType IS NULL OR ProductType & 32 <>32 ) AND  PID NOT LIKE N'FU-%' ", string.Join(",", orderIds.Distinct()));
            using (var cmd = new SqlCommand(SQL))
            {
                var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.Rows.OfType<DataRow>().Select(x => new OrderItem(x)).ToList();
                }
                return null;
            }
        }

        public static IEnumerable<OrderItem> SelectOrderListByOrderIds(List<string> orderIds)
        {
            string SQL = string.Format(@"SELECT O.Pid,O.Num,O.Price,O.TotalPrice FROM Gungnir..tbl_OrderList O WITH ( NOLOCK )
                                        LEFT JOIN Tuhu_productcatalog..vw_Products vw with(nolock) on O.PID=vw.PID COLLATE Chinese_PRC_CI_AS
                                        WHERE OrderID IN({0}) AND Deleted = 0   AND vw.Category=N'OIL' and vw.CP_Brand=N'出光/IDEMITSU'", string.Join(",", orderIds.Distinct()));
            using (var cmd = new SqlCommand(SQL))
            {
                var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.Rows.OfType<DataRow>().Select(x => new OrderItem(x)).ToList();
                }
                return null;
            }
        }

        public static IEnumerable<OrderStatusModel> SelectSplitOrderListByOrderIds(IEnumerable<int> orderIds)
        {
            const string SQL = @"SELECT PKID,OrderNo,Status,PurchaseStatus,DeliveryStatus,PayStatus,InstallStatus,InvoiceStatus FROM Gungnir..tbl_Order WITH ( NOLOCK ) WHERE PKID IN({0}) ";
            using (var cmd = new SqlCommand(string.Format(SQL, string.Join(",", orderIds.Distinct()))))
            {
                return DbHelper.ExecuteSelect<OrderStatusModel>(cmd);
            }
        }

        public static int GetOrderPromotionMoneyFor0310(List<OrderItem> items)
        {
            var result = Convert.ToInt32(items.Sum(s => s.TotalPrice));
            return result;
        }

        public static List<Tuple<Guid, int, int, string>> Get0320TireOrders()
        {
            #region sql

            string sql = @"SELECT
        O.UserID,
		SUM(OL.Num) AS Num,
        O.PKID,
        O.UserTel
FROM    Gungnir..tbl_Order AS O WITH(NOLOCK)
JOIN    Gungnir.dbo.tbl_OrderList AS OL WITH(NOLOCK) ON O.PKID = OL.OrderID
JOIN  Gungnir..tbl_ChannelDictionaries AS CD WITH(NOLOCK) ON CD.ChannelKey = O.OrderChannel
WHERE((O.Status = '3Installed'
                        AND O.InstallStatus = '2Installed'
                        AND O.InstallShopID > 0
                        AND O.InstallType = '1ShopInstall'
                        AND O.InstallDatetime<(GetDate()-5)
                      )
                      OR(ISNULL(O.InstallShopID, 0) = 0
                           AND O.Status = '2Shipped'
                           AND O.DeliveryStatus IN('3.5Signed')
                           AND Exists(select 1 from  Gungnir.dbo.tbl_OrderDeliveryLog WITH(NOLOCK)  where OrderId=O.PKID AND  DeliveryStatus='3.5Signed' AND DeliveryDatetime<(GETDATE()-5))
                         )
                    )
AND(O.OrderDatetime >= '2017-3-16 00:00:00' AND O.OrderDatetime <= '2017-3-29 23:59:59')
AND OL.ProductType & 32 <>32
AND OL.PID LIKE N'TR-%'
AND (CD.ChannelType = N'自有渠道' or CD.ChannelType=N'H5合作渠道')
AND NOT EXISTS( SELECT	1
    FROM	Gungnir..tbl_PromotionCode AS PC WITH ( NOLOCK )
    WHERE	 PC.RuleID = 2006
             AND PC.BatchID = O.PKID
             AND PC.CodeChannel = N'轮胎节送保养返券'
             AND (PC.Status <> 3 OR (PC.Status = 3 AND PC.UsedTime IS NOT NULL))
			)
Group by O.UserID,O.PKID,O.UserTel";

            #endregion sql

            Func<DataTable, List<Tuple<Guid, int, int, string>>> action = delegate (DataTable dt)
            {
                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                {
                    List<Tuple<Guid, int, int, string>> result = new List<Tuple<Guid, int, int, string>>();
                    foreach (DataRow item in dt.Rows)
                    {
                        result.Add(Tuple.Create(Guid.Parse(item["UserID"].ToString()), int.Parse(item["Num"].ToString()), int.Parse(item["PKID"].ToString()), item["UserTel"].ToString()));
                    }
                    return result;
                }
                return null;
            };
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 5 * 60;
                return DbHelper.ExecuteQuery(true, cmd, action);
            }
        }

        public static int GetHasGetPromotionCodeCount(Guid userId)
        {
            string sql = @"
SELECT	count(1)
FROM	Gungnir..tbl_PromotionCode   WITH ( NOLOCK )
WHERE	RuleID = 2006  AND UserId=@UserId
	AND CodeChannel = N'轮胎节送保养返券'";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                return (int)DbHelper.ExecuteScalar(true, cmd);
            }
        }

        public static int GetHasGetPromotionCodeCountFor0330(Guid userId)
        {
            string sql = @"
SELECT	count(1)
FROM	Gungnir..tbl_PromotionCode   WITH ( NOLOCK )
WHERE	RuleID = 2006  AND UserId=@UserId
	AND CodeChannel = N'焕新去踏青活动返券'";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                return (int)DbHelper.ExecuteScalar(true, cmd);
            }
        }

        public static List<Tuple<Guid, int, int, string>> Get0330TireOrders()
        {
            #region sql

            string sql = @"SELECT
        O.UserID,
		SUM(OL.Num) AS Num,
        O.PKID,
        O.UserTel
FROM    Gungnir..tbl_Order AS O WITH(NOLOCK)
JOIN    Gungnir.dbo.tbl_OrderList AS OL WITH(NOLOCK) ON O.PKID = OL.OrderID
JOIN  Gungnir..tbl_ChannelDictionaries AS CD WITH(NOLOCK) ON CD.ChannelKey = O.OrderChannel
WHERE((O.Status = '3Installed'
                        AND O.InstallStatus = '2Installed'
                        AND O.InstallShopID > 0
                        AND O.InstallType = '1ShopInstall'
                        AND O.InstallDatetime<(GetDate()-1)
                      )
                      OR(ISNULL(O.InstallShopID, 0) = 0
                           AND O.Status = '2Shipped'
                           AND O.DeliveryStatus IN('3.5Signed')
                           AND Exists(select 1 from  Gungnir.dbo.tbl_OrderDeliveryLog WITH(NOLOCK)  where OrderId=O.PKID AND  DeliveryStatus='3.5Signed' AND DeliveryDatetime<(GETDATE()-1))
                         )
                    )
AND(O.OrderDatetime >= '2017-3-30 00:00:00' AND O.OrderDatetime <= '2017-4-12 23:59:59')
AND OL.ProductType & 32 <>32
AND OL.PID LIKE N'TR-%'
AND (CD.ChannelType = N'自有渠道' or CD.ChannelType=N'H5合作渠道')
AND NOT EXISTS( SELECT	1
    FROM	Gungnir..tbl_PromotionCode AS PC WITH ( NOLOCK )
    WHERE	 PC.RuleID = 2006
             AND PC.BatchID = O.PKID
             AND PC.CodeChannel = N'焕新去踏青活动返券'
             AND (PC.Status <> 3 OR (PC.Status = 3 AND PC.UsedTime IS NOT NULL))
			)
Group by O.UserID,O.PKID,O.UserTel";

            #endregion sql

            Func<DataTable, List<Tuple<Guid, int, int, string>>> action = delegate (DataTable dt)
            {
                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                {
                    List<Tuple<Guid, int, int, string>> result = new List<Tuple<Guid, int, int, string>>();
                    foreach (DataRow item in dt.Rows)
                    {
                        result.Add(Tuple.Create(Guid.Parse(item["UserID"].ToString()), int.Parse(item["Num"].ToString()), int.Parse(item["PKID"].ToString()), item["UserTel"].ToString()));
                    }
                    return result;
                }
                return null;
            };
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 5 * 60;
                return DbHelper.ExecuteQuery(true, cmd, action);
            }
        }

        public static List<Tuple<Guid, int, string>> Get0401TireOrders()
        {
            #region sql

            string sql = @"SELECT
        O.UserID,
        O.PKID,
        O.UserTel
FROM    Gungnir..tbl_Order AS O WITH(NOLOCK)
Left join Gungnir..tbl_OrderList OL WITH(NOLOCK) ON O.PKID = OL.OrderID
JOIN  Gungnir..tbl_ChannelDictionaries AS CD WITH(NOLOCK) ON CD.ChannelKey = O.OrderChannel
WHERE   OL.PID LIKE N'TR-%'
AND  CD.ChannelType = N'自有渠道'
AND  O.SubmitDate>@beginTime
AND NOT EXISTS( SELECT	1
    FROM	Gungnir..tbl_PromotionCode AS PC WITH ( NOLOCK )
    WHERE	 PC.RuleID = 29378
             AND PC.BatchID = O.PKID
             AND PC.CodeChannel = N'FT系列轮毂优惠券'
			)";

            #endregion sql

            Func<DataTable, List<Tuple<Guid, int, string>>> action = delegate (DataTable dt)
            {
                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                {
                    List<Tuple<Guid, int, string>> result = new List<Tuple<Guid, int, string>>();
                    foreach (DataRow item in dt.Rows)
                    {
                        result.Add(Tuple.Create(Guid.Parse(item["UserID"].ToString()), int.Parse(item["PKID"].ToString()), item["UserTel"].ToString()));
                    }
                    return result;
                }
                return null;
            };
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 5 * 60;
                cmd.Parameters.AddWithValue("@beginTime", DateTime.Now.AddHours(-2));
                return DbHelper.ExecuteQuery(true, cmd, action);
            }
        }

        public static bool GetHasGetPromotionCodeFor0401(Guid userId)
        {
            string sql = @"
SELECT	TOP 1 1
FROM	Gungnir..tbl_PromotionCode   WITH ( NOLOCK )
WHERE	RuleID = 29378  AND UserId=@UserId
	AND CodeChannel = N'FT系列轮毂优惠券'
AND  CreateTime Between @beginTime and @endTime
";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@beginTime", DateTime.Now.Date.AddDays(-DateTime.Now.Day + 1));
                cmd.Parameters.AddWithValue("@endTime", DateTime.Now.Date.AddDays(-DateTime.Now.Day + 1).AddMonths(1));
                var result = DbHelper.ExecuteScalar(true, cmd);
                return result == null ? false : true;
            }
        }

        public static DataTable Get0413BaoYangFreeOrderList()
        {
            //①使用了(优惠券规则编号：3295/优惠券规则Guid：aa04dd4d-ed8a-4ee1-b886-e1e12a73fc89)的优惠券的订单
            string sql = @"SELECT    O.UserID,
		                             O.UserTel,
		                             O.SumMoney,
		                             O.PKID AS OrderID,
                                     O.InstallDatetime
                         FROM	Gungnir..tbl_Order AS O WITH ( NOLOCK )
                         JOIN	Gungnir..tbl_PromotionCode AS P WITH ( NOLOCK )
		                         ON O.PKID = P.OrderId
                         WHERE	 P.GetRuleID=3295
		                         AND  O.Status = N'3Installed'
                                 AND  O.InstallDatetime < (GETDATE() - 5)
                                 AND  NOT EXISTS ( SELECT	1
						                            FROM	Gungnir..tbl_PromotionCode AS PC WITH ( NOLOCK )
						                            WHERE	 PC.RuleID = 2006
								                             AND PC.BatchID = O.PKID
								                             AND PC.CodeChannel = N'机油买一送一券'
                                                             AND (PC.Status <> 3 OR (PC.Status = 3 AND PC.UsedTime IS NOT NULL))
                                                                                         )
                        UNION ALL
                        SELECT  O.UserID,
		                        O.UserTel,
		                        O.SumMoney,
		                        O.PKID AS OrderID,
                                O.InstallDatetime
                        FROM gungnir.dbo.tbl_Order  O  with(nolock)
                        left join gungnir.dbo.tbl_orderlist OL with(nolock) on o.pkid=ol.orderid
                        left join Tuhu_productcatalog..vw_Products vw with(nolock) on ol.pid=vw.pid COLLATE Chinese_PRC_CI_AS
                        WHERE O.OrderChannel=N'u门店' and O.Status= N'3Installed' AND  O.InstallDatetime < (GETDATE() -5)
                          and vw.Category=N'OIL' and (vw.CP_Brand=N'出光/IDEMITSU' OR vw.CP_Brand=N'胜牌/Valvoline' OR vw.CP_Brand=N'海湾/GULF')
                          AND(O.OrderDatetime >= '2017-4-12 00:00:00' AND O.OrderDatetime <= '2017-4-18 23:59:59')
                          AND  NOT EXISTS ( SELECT	1
						                            FROM	Gungnir..tbl_PromotionCode AS PC WITH ( NOLOCK )
						                            WHERE	 PC.RuleID = 2006
								                             AND PC.BatchID = O.PKID
								                             AND PC.CodeChannel = N'机油买一送一券'
                                                             AND (PC.Status <> 3 OR (PC.Status = 3 AND PC.UsedTime IS NOT NULL))
                                                                                         )";
            //②销售渠道：u门店，订单内包含胜牌/海湾/出光任意品牌机油的保养订单,新增下单时间【下单时间：4.12-4.18】
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 5 * 60;
                var dt = Tuhu.DbHelper.ExecuteQuery(cmd, one => one);
                return dt;
            }
        }

        public static IEnumerable<OrderItem> SelectOrderListByOrderIdFor0413(IEnumerable<int> orderIds)
        {
            string SQL = string.Format(@"SELECT O.Pid,O.Num,O.Price,O.TotalPrice FROM Gungnir..tbl_OrderList O WITH ( NOLOCK )
                                        LEFT JOIN Tuhu_productcatalog..vw_Products vw with(nolock) on O.PID=vw.PID COLLATE Chinese_PRC_CI_AS
                                        WHERE OrderID IN({0}) AND Deleted = 0  AND vw.Category=N'OIL'", string.Join(",", orderIds));
            using (var cmd = new SqlCommand(SQL))
            {
                var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.Rows.OfType<DataRow>().Select(x => new OrderItem(x)).ToList();
                }
                return null;
            }
        }

        public static bool GetHasGetPromotionCodeFor0413(Guid userId)
        {
            string sql = @"
SELECT	TOP 1 1
FROM	Gungnir..tbl_PromotionCode   WITH ( NOLOCK )
WHERE	RuleID = 2006  AND UserId=@UserId
	AND CodeChannel = N'机油买一送一券'
";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                var result = DbHelper.ExecuteScalar(true, cmd);
                return result == null ? false : true;
            }
        }

        public static DataTable Get0421BaoYangFreeOrderList()
        {
            //使用了(优惠券规则编号：3421 / 优惠券规则Guid：4f789f41 - bdf2 - 4344 - b6c0 - 84ebeabd78d9)的优惠券的订单
            //订单状态：已安装
            //发券时间：订单安装完成后5天
            string sql = @"SELECT    O.UserID,
		                             O.UserTel,
		                             O.SumMoney,
		                             O.PKID AS OrderID,
                                     O.InstallDatetime
                         FROM	Gungnir..tbl_Order AS O WITH ( NOLOCK )
                         JOIN	Gungnir..tbl_PromotionCode AS P WITH ( NOLOCK )
		                         ON O.PKID = P.OrderId
                        WHERE	P.GetRuleID=3421
		                        AND  O.Status = N'3Installed'
                                AND  O.InstallDatetime < (GETDATE() - 5)
                                AND  NOT EXISTS ( SELECT	1
					                           FROM	Gungnir..tbl_PromotionCode AS PC WITH ( NOLOCK )
					                           WHERE	 PC.RuleID = 11144
							                             AND PC.BatchID = O.PKID
							                             AND PC.CodeChannel = N'出光机油抵用券'
                                                         AND (PC.Status <> 3 OR (PC.Status = 3 AND PC.UsedTime IS NOT NULL))
                                                                                     )";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 5 * 60;
                var dt = Tuhu.DbHelper.ExecuteQuery(cmd, one => one);
                return dt;
            }
        }

        public static IEnumerable<OrderItem> SelectOrderListByOrderIdFor0421(IEnumerable<int> orderIds)
        {
            //面值：与原订单内所有机油产品实付金额相同
            string SQL = string.Format(@"SELECT O.Pid,O.Num,O.Price,O.TotalPrice FROM Gungnir..tbl_OrderList O WITH ( NOLOCK )
                                        LEFT JOIN Tuhu_productcatalog..vw_Products vw with(nolock) on O.PID=vw.PID COLLATE Chinese_PRC_CI_AS
                                        WHERE OrderID IN({0}) AND Deleted = 0  AND vw.Category=N'OIL'", string.Join(",", orderIds));
            using (var cmd = new SqlCommand(SQL))
            {
                var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.Rows.OfType<DataRow>().Select(x => new OrderItem(x)).ToList();
                }
                return null;
            }
        }

        public static DataTable Get0424BaoYangFreeOrderList()
        {
            //①使用了(优惠券规则编号：3524 / 优惠券规则Guid：3ed65eed - 3064 - 42f8 - bee4 - cd666fd2864c)的优惠券的订单
            string sql = @"SELECT    O.UserID,
		                             O.UserTel,
		                             O.SumMoney,
		                             O.PKID AS OrderID,
                                     O.InstallDatetime
                         FROM	Gungnir..tbl_Order AS O WITH ( NOLOCK )
                         JOIN	Gungnir..tbl_PromotionCode AS P WITH ( NOLOCK )
		                         ON O.PKID = P.OrderId
                         WHERE P.GetRuleID = 3524
                                AND O.Status = N'3Installed'
                                AND O.InstallDatetime < (GETDATE() - 5)
                                AND NOT EXISTS(SELECT    1
                                                  FROM Gungnir..tbl_PromotionCode AS PC WITH(NOLOCK)
                                                  WHERE     PC.RuleID = 2006
                                                           AND PC.BatchID = O.PKID
                                                           AND PC.CodeChannel = N'20170425机油买一送一券3524'
                                                           AND(PC.Status <> 3 OR(PC.Status = 3 AND PC.UsedTime IS NOT NULL))
                                                                                       )
                        UNION ALL
                        SELECT  O.UserID,
		                        O.UserTel,
		                        O.SumMoney,
		                        O.PKID AS OrderID,
                                O.InstallDatetime
                        FROM gungnir.dbo.tbl_Order  O  with(nolock)
                        left join gungnir.dbo.tbl_orderlist OL with(nolock) on o.pkid=ol.orderid
                        left join Tuhu_productcatalog..vw_Products vw with(nolock) on ol.pid=vw.pid COLLATE Chinese_PRC_CI_AS
                        WHERE O.OrderChannel=N'u门店' and O.Status= N'3Installed' AND  O.InstallDatetime < (GETDATE() -5)
                          and vw.Category=N'OIL' and (vw.CP_Brand=N'出光/IDEMITSU' OR vw.CP_Brand=N'胜牌/Valvoline' OR vw.CP_Brand=N'海湾/GULF')
                          AND(O.OrderDatetime >= '2017-4-19 00:00:00' AND O.OrderDatetime <= '2017-5-31 23:59:59')
                          AND  NOT EXISTS ( SELECT	1
						                            FROM	Gungnir..tbl_PromotionCode AS PC WITH ( NOLOCK )
						                            WHERE	 PC.RuleID = 2006
								                             AND PC.BatchID = O.PKID
								                             AND PC.CodeChannel = N'20170425机油买一送一券3524'
                                                             AND (PC.Status <> 3 OR (PC.Status = 3 AND PC.UsedTime IS NOT NULL))
                                                                                         )";
            //②销售渠道：u门店，订单内包含胜牌 / 海湾 / 出光任意品牌机油的保养订单 下单时间：4.19 - 5.12
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 5 * 60;
                var dt = Tuhu.DbHelper.ExecuteQuery(cmd, one => one);
                return dt;
            }
        }


        public static DataTable Get20171111BaoYangFanQuanOrderList()
        {
            //①使用了(优惠券规则编号：8395 / 优惠券规则Guid：38d0b92a-498c-4c64-8173-a0b56f8cb347)的优惠券的订单
            string sql = @"SELECT    O.UserID,O.UserTel,
		                             O.SumMoney,
		                             O.PKID AS OrderID,
                                     O.InstallDatetime
                         FROM	Gungnir..tbl_Order AS O WITH ( NOLOCK )
                         JOIN	Gungnir..tbl_PromotionCode AS P WITH ( NOLOCK )
		                         ON O.PKID = P.OrderId
                         WHERE P.GetRuleID = 8395
                                AND O.Status = N'3Installed'
                                AND O.OrderChannel<>N'u门店'
                                AND O.InstallDatetime < (GETDATE() - 5)
                                AND O.OrderDatetime BETWEEN '2017/11/1' AND '2017/11/13 23:59:59'
                                AND NOT EXISTS(SELECT    1
                                                  FROM Gungnir..tbl_PromotionCode AS PC WITH(NOLOCK)
                                                  WHERE     PC.RuleID = 2006
                                                           AND PC.BatchID = O.PKID
                                                           AND PC.CodeChannel = N'20171111机油买一送一券8395'
                                                           AND(PC.Status <> 3 OR(PC.Status = 3 AND PC.UsedTime IS NOT NULL))
                                                                                       )";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 5 * 60;
                var dt = Tuhu.DbHelper.ExecuteQuery(true,cmd, one => one);
                return dt;
            }
        }
        public static DataTable GetBaoYangFanQuanOrderList(int installDay, int getRuleId,int ruleId,DateTime orderStartTime,DateTime orderEndTime,string channel)
        {
            //①使用了(优惠券规则编号：8395 / 优惠券规则Guid：38d0b92a-498c-4c64-8173-a0b56f8cb347)的优惠券的订单
            string sql = $@"SELECT    O.UserID,O.UserTel,
		                             O.SumMoney,
		                             O.PKID AS OrderID,
                                     O.InstallDatetime
                         FROM	Gungnir..tbl_Order AS O WITH ( NOLOCK )
                         JOIN	Gungnir..tbl_PromotionCode AS P WITH ( NOLOCK )
		                         ON O.PKID = P.OrderId
                         WHERE P.GetRuleID = {getRuleId}
                                AND O.Status = N'3Installed'
                                AND O.OrderChannel<>N'u门店'
                                AND O.InstallDatetime < (GETDATE() - {installDay})
                                AND O.OrderDatetime BETWEEN @OrderStartTime AND @OrderEndTime
                                AND NOT EXISTS(SELECT    1
                                                  FROM Gungnir..tbl_PromotionCode AS PC WITH(NOLOCK)
                                                  WHERE     PC.RuleID = {ruleId}
                                                           AND PC.BatchID = O.PKID
                                                           AND PC.CodeChannel = @Channel
                                                           AND(PC.Status <> 3 OR(PC.Status = 3 AND PC.UsedTime IS NOT NULL))
                                                                                       )";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 5 * 60;
                cmd.Parameters.AddWithValue("@OrderStartTime", orderStartTime);
                cmd.Parameters.AddWithValue("@OrderEndTime", orderEndTime);
                cmd.Parameters.AddWithValue("@Channel",channel);
                var dt = Tuhu.DbHelper.ExecuteQuery(true, cmd, one => one);
                return dt;
            }
        }
        public static bool GetHasGetPromotionCodeFor0424(Guid userId)
        {
            string sql = @"
SELECT	TOP 1 1
FROM	Gungnir..tbl_PromotionCode   WITH ( NOLOCK )
WHERE	RuleID = 2006  AND UserId=@UserId
	AND CodeChannel = N'20170425机油买一送一券3524'
";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                var result = DbHelper.ExecuteScalar(true, cmd);
                return result == null ? false : true;
            }
        }
        public static bool GetHasGetPromotionCodeFor20171111(Guid userId)
        {
            string sql = @"
SELECT	TOP 1 1
FROM	Gungnir..tbl_PromotionCode   WITH ( NOLOCK )
WHERE	RuleID = 2006  AND UserId=@UserId
	AND CodeChannel = N'20171111机油买一送一券8395'
";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                var result = DbHelper.ExecuteScalar(true, cmd);
                return result == null ? false : true;
            }
        }
        public static bool GetHasGetPromotionCode(Guid userId,string channel)
        {
            string sql = @"
SELECT	TOP 1 1
FROM	Gungnir..tbl_PromotionCode   WITH ( NOLOCK )
WHERE	RuleID = 2006  AND UserId=@UserId
	AND CodeChannel = @Channel
";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Channel", channel);
                var result = DbHelper.ExecuteScalar(true, cmd);
                return result == null ? false : true;
            }
        }
        public static IEnumerable<OrderItem> SelectOrderListByOrderIdFor0424(IEnumerable<int> orderIds)
        {
            string SQL = string.Format(@"SELECT O.Pid,O.Num,O.Price,O.TotalPrice FROM Gungnir..tbl_OrderList O WITH ( NOLOCK )
                                        LEFT JOIN Tuhu_productcatalog..vw_Products vw with(nolock) on O.PID=vw.PID COLLATE Chinese_PRC_CI_AS
                                        WHERE OrderID IN({0}) AND Deleted = 0  AND vw.Category=N'OIL'", string.Join(",", orderIds));
            using (var cmd = new SqlCommand(SQL))
            {
                var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.Rows.OfType<DataRow>().Select(x => new OrderItem(x)).ToList();
                }
                return null;
            }
        }
        /// <summary>
        /// 查出订单里所有机油的价格
        /// </summary>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        public static IEnumerable<OrderItem> SelectOrderListByOrderIdFor20171111(IEnumerable<int> orderIds)
        {
            string SQL = string.Format(@"SELECT O.Pid,O.Num,O.Price,O.TotalPrice FROM Gungnir..tbl_OrderList O WITH ( NOLOCK )
                                        LEFT JOIN Tuhu_productcatalog..vw_Products vw with(nolock) on O.PID=vw.PID COLLATE Chinese_PRC_CI_AS
                                        WHERE OrderID IN({0}) AND ISNULL(O.ProductType,0) & 32 <>32 AND Deleted = 0  AND vw.PID LIKE 'OL-%'", string.Join(",", orderIds));
            using (var cmd = new SqlCommand(SQL))
            {
                var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.Rows.OfType<DataRow>().Select(x => new OrderItem(x)).ToList();
                }
                return null;
            }
        }
        /// <summary>
        /// 查出订单里所有机油的价格
        /// </summary>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        public static IEnumerable<OrderItem> SelectOrderListByOrderIdForOL(IEnumerable<int> orderIds)
        {
            string SQL = string.Format(@"SELECT O.Pid,O.Num,O.Price,O.TotalPrice FROM Gungnir..tbl_OrderList O WITH ( NOLOCK )
                                        LEFT JOIN Tuhu_productcatalog..vw_Products vw with(nolock) on O.PID=vw.PID COLLATE Chinese_PRC_CI_AS
                                        WHERE OrderID IN({0}) AND ISNULL(O.ProductType,0) & 32 <>32 AND Deleted = 0  AND vw.PID LIKE 'OL-%'", string.Join(",", orderIds));
            using (var cmd = new SqlCommand(SQL))
            {
                var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.Rows.OfType<DataRow>().Select(x => new OrderItem(x)).ToList();
                }
                return null;
            }
        }

        public static string GetUserPhoneByUserId(Guid userId)
        {
            string sql = @"SELECT top 1 U_MOBILE_NUMBER FROM Tuhu_profiles..UserObject  WITH(NOLOCK)  WHERE UserId=@UserId";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                return DbHelper.ExecuteScalar(cmd).ToString();
            }
        }

        public static DataTable Get0630BaoYangFreeOrderList()
        {
            //①使用了(优惠券规则编号：5081 / 优惠券规则Guid：9c594813 - a486 - 4880 - 91a9 - 94ac0dfc4e4d)的优惠券的订单
            string sql = @"SELECT    O.UserID,
		                             O.UserTel,
		                             O.SumMoney,
		                             O.PKID AS OrderID,
								     O.ShippingMoney
                         FROM	Gungnir..tbl_Order AS O WITH ( NOLOCK )
                         JOIN	Gungnir..tbl_PromotionCode AS P WITH ( NOLOCK )
		                         ON O.PKID = P.OrderId
                         WHERE P.GetRuleID = 5081
                                AND O.Status = N'3Installed'
                                AND O.OrderChannel<>N'u门店'
                                AND NOT EXISTS(SELECT    1
                                                  FROM Gungnir..tbl_PromotionCode AS PC WITH(NOLOCK)
                                                  WHERE     PC.RuleID = 332
                                                           AND PC.BatchID = O.PKID
                                                           AND PC.CodeChannel = N'20170705机油买一送一券5081'
                                                           AND(PC.Status <> 3 OR(PC.Status = 3 AND PC.UsedTime IS NOT NULL))
                                                                                       )
                         ";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandTimeout = 5 * 60;
                var dt = Tuhu.DbHelper.ExecuteQuery(cmd, one => one);
                return dt;
            }
        }

        public static IEnumerable<OrderItem> SelectOrderListByOrderIdFor0630(IEnumerable<int> orderIds)
        {
            string SQL = string.Format(@"SELECT O.Pid,O.Num,O.Price,O.TotalPrice FROM Gungnir..tbl_OrderList O WITH ( NOLOCK )
                                        LEFT JOIN Tuhu_productcatalog..vw_Products vw with(nolock) on O.PID=vw.PID COLLATE Chinese_PRC_CI_AS
                                        WHERE OrderID IN({0}) AND Deleted = 0  AND O.PID NOT LIKE N'FU-%' ", string.Join(",", orderIds));
            using (var cmd = new SqlCommand(SQL))
            {
                var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.Rows.OfType<DataRow>().Select(x => new OrderItem(x)).ToList();
                }
                return null;
            }
        }

        public static DataTable SelectMaPaiTireOrders(string shopids)
        {
            string sql = $@"USE Gungnir;
SELECT  o.OrderNo ,
        o.PKID ,
        o.UserID ,
        SUM(l.Num) AS count
FROM    Gungnir..tbl_Order AS o WITH ( NOLOCK )
        JOIN Gungnir..SplitString('{shopids}', ',', 1) AS s ON o.InstallShopID = s.Item
        JOIN Gungnir..tbl_OrderList AS l WITH ( NOLOCK ) ON o.PKID = l.OrderID
WHERE   o.OrderChannel = N'u门店'
        AND l.PID = 'TR-CT-C2-MC5|17'
         AND o.OrderDatetime >= '{DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00")}'
         AND o.OrderDatetime >= '2017-8-28 00:00:00'
         AND o.OrderDatetime <= '2018-1-10 23:59:59'
        AND o.Status = N'3Installed'
        AND NOT EXISTS ( SELECT 1
                         FROM   Gungnir..tbl_PromotionCode AS PC WITH ( NOLOCK )
                         WHERE  PC.RuleID = 262
                                AND PC.BatchID = o.PKID )
GROUP BY o.OrderNo ,
        o.PKID ,
        o.UserID;";
            using (var cmd = new SqlCommand(sql))
            {
                var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                return dt;
            }
        }
        public static IEnumerable<OrderItem> SelectOrderListByOrderIdForJiYouFanQuan(IEnumerable<string> orderIds)
        {
            //面值：与原订单内所有机油产品实付金额相同
            string SQL = string.Format(@"SELECT O.Pid,O.Num,O.Price,O.TotalPrice FROM Gungnir..tbl_OrderList O WITH ( NOLOCK )
                                        LEFT JOIN Tuhu_productcatalog..vw_Products vw with(nolock) on O.PID=vw.PID COLLATE Chinese_PRC_CI_AS
                                        WHERE OrderID IN({0}) AND Deleted = 0  AND vw.Category=N'OIL'", string.Join(",", orderIds));
            using (var cmd = new SqlCommand(SQL))
            {
                var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.Rows.OfType<DataRow>().Select(x => new OrderItem(x)).ToList();
                }
                return null;
            }
        }
        public static DataTable GetJiYouFanQuanOrders()
        {
            string sql = @"SELECT  P.RuleID ,
        O.UserID ,
        O.UserTel ,
        O.SumMoney ,
        O.PKID AS OrderID
FROM    Gungnir..tbl_Order AS O WITH ( NOLOCK )
        JOIN Gungnir..tbl_PromotionCode AS P WITH ( NOLOCK ) ON O.PKID = P.OrderId
WHERE   P.GetRuleID = 6100
        AND O.OrderChannel <> N'u门店'
        AND O.Status = N'3Installed'
        AND O.OrderDatetime >= '2017/8/1'
        AND O.InstallDatetime < ( GETDATE() - 3 )
        AND NOT EXISTS ( SELECT 1
                         FROM   Gungnir..tbl_PromotionCode AS PC WITH ( NOLOCK )
                         WHERE  PC.RuleID = 2006
                                AND PC.BatchID = O.PKID
                                AND PC.CodeChannel = N'指定机油到店保养券'
                                AND ( PC.Status <> 3
                                      OR ( PC.Status = 3
                                           AND PC.UsedTime IS NOT NULL
                                         )
                                    ) );";
            using (var cmd = new SqlCommand(sql))
            {
                var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                return dt;
            }

        }


        public static DataTable Get0902BaoYangFreeOrderList()
        {
            string sql= @" SELECT  P.RuleID ,
        O.UserID ,
        O.UserTel ,
        O.SumMoney ,
        O.PKID AS OrderID
FROM    Gungnir..tbl_Order AS O WITH ( NOLOCK )
        JOIN Gungnir..tbl_PromotionCode AS P WITH ( NOLOCK ) ON O.PKID = P.OrderId
WHERE   P.GetRuleID = 6537
        AND O.Status = N'3Installed'
        AND O.OrderDatetime >= '2017/8/28'
        AND O.InstallDatetime < ( GETDATE() - 5 )
        AND NOT EXISTS ( SELECT 1
                         FROM   Gungnir..tbl_PromotionCode AS PC WITH ( NOLOCK )
                         WHERE  PC.RuleID = 2006
                                AND PC.BatchID = O.PKID
                                AND PC.CodeChannel = N'0902机油到店保养券'
                                AND ( PC.Status <> 3
                                      OR ( PC.Status = 3
                                           AND PC.UsedTime IS NOT NULL
                                         )
                                    ) );";
            using (var cmd = new SqlCommand(sql))
            {
                var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                return dt;
            }
        }


        public static DataTable Get180412ReturnGiftOrderList(IEnumerable<string> pids,IEnumerable<int> shopIds) {
            string sql = @"SELECT O.UserID,O.PKID,L.PID,L.Num,L.Price FROM Gungnir..tbl_Order AS O WITH(NOLOCK) JOIN Gungnir..tbl_OrderList AS L 
                          ON O.PKID=l.OrderID
                          WHERE O.OrderChannel IN(N'u自营门店',N'u门店',N'u门店2',N'u门店3')
                          AND O.OrderDatetime>=CAST(convert(varchar(10),DATEADD(DAY,-2,GETDATE()),120)+' 00:00:00' as datetime)
                          AND O.OrderDatetime<=DATEADD(second,-1,CAST(convert(varchar(10),GETDATE(),120)+' 00:00:00' as datetime))
                          AND O.OrderDatetime>='2018-04-12'
                          AND O.InstallStatus='2Installed'
                          AND L.PID IN(SELECT Item FROM  Gungnir..SplitString(@Pids,',',1))
                          AND O.InstallShopID IN(SELECT Item FROM  Gungnir..SplitString(@ShopIds,',',1))
                        ";
            using (var cmd = new SqlCommand(sql))
            {
                cmd.Parameters.AddWithValue("@Pids", string.Join(",", pids));
                cmd.Parameters.AddWithValue("@ShopIds", string.Join(",", shopIds));
                var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                return dt;
            }
        }
    }
}
