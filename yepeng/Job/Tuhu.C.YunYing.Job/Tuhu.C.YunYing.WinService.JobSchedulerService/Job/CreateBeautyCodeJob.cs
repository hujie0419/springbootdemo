using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.KuaiXiu;
using Tuhu.Service.Order.Models;
using Tuhu.Service.UserAccount;
using Tuhu.Service.UserAccount.Enums;
using Tuhu.Service.UserAccount.Models;
using Tuhu.C.YunYing.WinService.JobSchedulerService.BLL;
using Tuhu.C.YunYing.WinService.JobSchedulerService.Model;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.Job
{
    [DisallowConcurrentExecution]
    public class CreateBeautyCodeJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CreateBeautyCodeJob));

        private static readonly string connectionRw = ConfigurationManager.ConnectionStrings["Tuhu_Groupon"]?.ConnectionString;

        private static readonly string connectionRo = ConfigurationManager.ConnectionStrings["Tuhu_Groupon_ReadOnly"]?.ConnectionString;

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("CreateBeautyCodeJob Start");
            try
            {
                var tasks = GetCreateBeautyCodeTasks();
                tasks.ForEach(item =>
                {
                    CreateBeautyServiceCodeByBeautyCodeTask(item);
                });
                var needCreateOrderBatchCodes = GetBatchCodesOfNeedCreateOrder();
                CreateOrderByBatchCode(needCreateOrderBatchCodes);
            }
            catch (Exception ex)
            {
                Logger.Error("CreateBeautyCodeJobRun", ex);
            }
            Logger.Info("CreateBeautyCodeJob End");
        }
        /// <summary>
        /// 根据批次号创建买断订单
        /// </summary>
        /// <param name="tasks"></param>

        private static void CreateOrderByBatchCode(IEnumerable<string> batchCodes)
        {
            if (batchCodes != null)
            {
                foreach (var batchCode in batchCodes)
                {
                    var totalTasks = GetCreateBeautyCodeTasksByBatchCode(batchCode);
                    if (totalTasks != null && totalTasks.Any())
                    {
                        var taskOne = totalTasks.First();
                        var packageDetailId = taskOne.PackageDetailId;
                        var settlementMethodAndPrice = SelectSettlementMethodAndPrice(packageDetailId);
                        if (string.Equals(settlementMethodAndPrice.Item1, "PreSettled", StringComparison.OrdinalIgnoreCase))
                        {
                            var status = SelectStatusByBatchCode(batchCode);
                            var isBuyOutOrderCreated = IsBuyOutOrderCreated(batchCode);
                            if (!isBuyOutOrderCreated && status != null && status.Count == 1 && status.Any(s => string.Equals("Completed", s, StringComparison.OrdinalIgnoreCase)))
                            {

                                var vipUserId = taskOne.VipUserId;
                                var pid = taskOne.ServiceId;
                                var name = taskOne.ServiceName;
                                var num = totalTasks.Sum(s => s.Quantity);
                                CreateOrderResult createOrderResult = null;
                                try
                                {
                                    createOrderResult = OrderBusiness.CreateServiceCodeOrderForVip(vipUserId, settlementMethodAndPrice.Item2, pid, name, num);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error($"导入用户创建订单失败,大客户Id:{vipUserId},批次号:{batchCode},服务码数量：{num}", ex);
                                }
                                if (createOrderResult != null && createOrderResult.OrderId > 0)
                                {
                                    Logger.Info($"导入用户创建大客户美容2B批量成功,OrderId:{createOrderResult.OrderId}");
                                    var updateOrderResult = UpdateBuyOutOrderIdByBatchCode(batchCode, createOrderResult.OrderId);
                                    if (!updateOrderResult)
                                        Logger.Error($"更新订单失败,batchCode:{batchCode},BuyOutOrderId:{createOrderResult.OrderId}");
                                }
                            }
                        }
                    }
                }
            }           
        }

        private static bool CreateBeautyServiceCodeByBeautyCodeTask(BeautyCodeTask item)
        {
            List<string> serviceCodes = null;
            Func<BeautyCodeTask, List<string>, bool> revertFunc = (it, sCodes) =>
            {
                var result = RevertVOLServiceCode(sCodes);
                using (var dbHelper = DbHelper.CreateDbHelper(connectionRw))
                {
                    UpdateBeautyCodeTaskStatus(dbHelper, it, "Failed");
                }
                return result;
            };

            try
            {
                var success = false;
                item.UserId = GetUserId(item.MobileNumber);
                if (item.UserId != Guid.Empty)
                {
                    serviceCodes = GenerateVOLServiceCode(item);
                    if (serviceCodes.Any() && serviceCodes.Count == item.Quantity)
                    {
                        success = CreateBeautyServiceCodeByBeautyCodeTask(item, serviceCodes);
                        if (success)
                        {
                            UpdateBeautyCodeTaskUserIdByPKID(item.PKID, item.UserId);
                        }
                    }
                    if (!success)
                    {
                        revertFunc(item, serviceCodes);
                    }
                }
                return success;
            }
            catch (Exception ex)
            {
                revertFunc(item, serviceCodes);
                Logger.Error("CreateBeautyServiceCode", ex);
                return false;
            }
        }



        private static bool CreateBeautyServiceCodeByBeautyCodeTask(BeautyCodeTask item, List<string> serviceCodes)
        {
            using (var dbHelper = DbHelper.CreateDbHelper(connectionRw))
            {
                var result = false;
                dbHelper.BeginTransaction();
                UpdateBeautyCodeTaskStatus(dbHelper, item, "Running");
                serviceCodes.ForEach(serviceCode =>
                {
                    result = CreateBeautyServiceCode(dbHelper, item, serviceCode);
                    if (!result) return;
                });
                if (!result)
                {
                    dbHelper.Rollback();
                    return false;
                }
                result = UpdateBeautyCodeTaskStatus(dbHelper, item, "Completed");
                if (!result)
                {
                    dbHelper.Rollback();
                    return false;
                }
                result = UpdateBeautyPackageDetail(dbHelper, item.Quantity, item.PackageDetailId);
                if (!result)
                {
                    dbHelper.Rollback();
                    return false;
                }
                dbHelper.Commit();
                return true;
            }
        }

        private static Guid GetUserId(string mobile)
        {
            using (var client = new UserAccountClient())
            {
                var serviceResult = client.GetUserByMobile(mobile);
                if (!serviceResult.Success)
                {
                    serviceResult.ThrowIfException(true);
                }
                var user = serviceResult.Result;
                if (user == null || user.UserId == Guid.Empty)
                {
                    serviceResult = client.CreateUserRequest(new CreateUserRequest
                    {
                        MobileNumber = mobile,
                        ChannelIn = nameof(ChannelIn.Import),
                        UserCategoryIn = nameof(UserCategoryIn.Tuhu)
                    });
                    if (!serviceResult.Success)
                    {
                        serviceResult.ThrowIfException(true);
                    }
                    user = serviceResult.Result;
                }
                return user == null ? Guid.Empty : user.UserId;
            }
        }

        private static List<string> GenerateVOLServiceCode(BeautyCodeTask item)
        {
            using (var client = new ServiceCodeClient())
            {
                var serviceResult = client.GenerateVOLServiceCode(new Service.KuaiXiu.Models.GenerateServiceCodeRequest
                {
                    GenerateNum = item.Quantity,
                    OverdueTime = item.EndTime,
                    ServiceId = item.ServiceId,
                    ServiceName = item.ServiceName,
                });
                serviceResult.ThrowIfException(true);
                return serviceResult.Result?.ToList() ?? new List<string>();
            }
        }

        private static bool RevertVOLServiceCode(List<string> serviceCodes)
        {
            try
            {
                if (serviceCodes != null && serviceCodes.Any())
                {
                    using (var client = new ServiceCodeClient())
                    {
                        var serviceResult = client.RevertVOLServiceCode(new Service.KuaiXiu.Models.RevertVOLServiceCodeRequest
                        {
                            ServiceCodes = serviceCodes
                        });
                        serviceResult.ThrowIfException(true);
                        return serviceResult.Result;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("RevertVOLServiceCode", ex);
                return false;
            }
            return true;
        }

        private static List<BeautyCodeTask> GetCreateBeautyCodeTasks()
        {
            using (var dbHelper = DbHelper.CreateDbHelper(connectionRw))
            using (var cmd = new SqlCommand(@"SELECT TOP 3000
        t.PKID ,
        t.MobileNumber ,
        t.Quantity ,
        t.StartTime ,
        t.EndTime ,
        d.PKID AS PackageDetailId ,
        t.Type ,
        d.PID AS ServiceId ,
        d.Name AS ServiceName ,
        m.VipUserId ,
        t.BatchCode ,
        t.Source
FROM    Tuhu_groupon..CreateBeautyCodeTask AS t WITH ( NOLOCK )
        INNER JOIN Tuhu_groupon..BeautyServicePackageDetail AS d WITH ( NOLOCK ) ON d.PKID = t.MappingId
        JOIN Tuhu_groupon..MrCooperateUserConfig AS m WITH ( NOLOCK ) ON d.CooperateId = m.PKID
WHERE   t.Status IN ( N'Failed', N'Preparing' )
        AND t.Type = N'serviceCode'
ORDER BY t.PKID;"))
            {
                var result = dbHelper.ExecuteSelect<BeautyCodeTask>(cmd);
                return result.ToList();
            }
        }

        public static List<BeautyCodeTask> GetCreateBeautyCodeTasksByBatchCode(string batchCode)
        {
            using (var dbHelper = DbHelper.CreateDbHelper(connectionRw))
            using (var cmd = new SqlCommand(@"SELECT  t.PKID ,
        t.MobileNumber ,
        t.Quantity ,
        t.StartTime ,
        t.EndTime ,
        d.PKID AS PackageDetailId ,
        t.Type ,
        d.PID AS ServiceId ,
        d.Name AS ServiceName ,
        m.VipUserId ,
        t.BatchCode ,
        t.Source
FROM    Tuhu_groupon..CreateBeautyCodeTask AS t WITH ( NOLOCK )
        INNER JOIN Tuhu_groupon..BeautyServicePackageDetail AS d WITH ( NOLOCK ) ON d.PKID = t.MappingId
        JOIN Tuhu_groupon..MrCooperateUserConfig AS m WITH ( NOLOCK ) ON d.CooperateId = m.PKID
WHERE   t.BatchCode = @BatchCode
ORDER BY t.PKID;"))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@BatchCode", batchCode);
                var result = dbHelper.ExecuteSelect<BeautyCodeTask>(cmd);
                return result.ToList();
            }
        }

        public static bool IsBuyOutOrderCreated(string batchCode)
        {
            using (var dbHelper = DbHelper.CreateDbHelper(connectionRw))
            using (var cmd = new SqlCommand(@"
SELECT TOP 1
        *
FROM    Tuhu_groupon..CreateBeautyCodeTask AS A WITH ( NOLOCK )
WHERE   A.BatchCode = @BatchCode
        AND A.BuyoutOrderId > 0"))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@BatchCode", batchCode);
                var result = dbHelper.ExecuteScalar(cmd);
                return result != null;
            }
        }

        private static bool CreateBeautyServiceCode(BaseDbHelper dbHelper, BeautyCodeTask item, string serviceCode)
        {
            using (var cmd = new SqlCommand(@"INSERT  INTO Tuhu_groupon..BeautyServicePackageDetailCode
        ( UserId ,
          PackageDetailId ,
          ServiceCode ,
          IsActive ,
          StartTime ,
          EndTime ,
          CreateTime ,
          UpdateTime ,
          PackageCode ,
          VipUserId ,
          ImportBatchCode ,
          Source
        )
VALUES  ( @UserId ,
          @PackageDetailId ,
          @ServiceCode ,
          1 ,
          @StartTime ,
          @EndTime ,
          GETDATE() ,
          NULL ,
          NULL ,
          @VipUserId ,
          @ImportBatchCode ,
          @Source 
        );"))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@UserId", item.UserId);
                cmd.Parameters.AddWithValue("@PackageDetailId", item.PackageDetailId);
                cmd.Parameters.AddWithValue("@ServiceCode", serviceCode);
                cmd.Parameters.AddWithValue("@StartTime", item.StartTime);
                cmd.Parameters.AddWithValue("@EndTime", item.EndTime);
                cmd.Parameters.AddWithValue("@VipUserId", item.VipUserId);
                cmd.Parameters.AddWithValue("@ImportBatchCode", item.BatchCode);
                cmd.Parameters.AddWithValue("@Source", item.Source);
                return dbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        private static bool UpdateBeautyCodeTaskStatus(BaseDbHelper dbHelper, BeautyCodeTask item, string status)
        {
            using (var cmd = new SqlCommand(@"UPDATE  Tuhu_groupon..CreateBeautyCodeTask
SET     Status = @Status ,
        LastUpdateDateTime = GETDATE()
WHERE   PKID = @PKID;"))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@PKID", item.PKID);
                cmd.Parameters.AddWithValue("@Status", status);
                return dbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

        private static bool UpdateBeautyPackageDetail(BaseDbHelper dbHelper, int quantity, int packageDetailId)
        {
            using (var cmd = new SqlCommand(@"UPDATE  Tuhu_groupon..BeautyServicePackageDetail
SET     ServiceCodeNum = ISNULL(ServiceCodeNum, 0) + @Quantity ,
        IsServiceCodeGenerated = 1
WHERE   PKID = @PackageDetailId;"))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@PackageDetailId", packageDetailId);
                return dbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }
        /// <summary>
        /// 根据批次查询当前批次任务完成的状态
        /// </summary>
        /// <param name="batchCode"></param>
        /// <returns></returns>
        private static List<string> SelectStatusByBatchCode(string batchCode)
        {

            using (var dbHelper = DbHelper.CreateDbHelper(connectionRw))
            using (var cmd = new SqlCommand(@"
SELECT DISTINCT
        A.Status
FROM    Tuhu_groupon..CreateBeautyCodeTask AS A WITH ( NOLOCK )
WHERE   A.BatchCode = @BatchCode;"))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@BatchCode", batchCode);
                return dbHelper.ExecuteQuery(cmd, dt =>
                {
                   var result = new List<string>();
                    if (dt != null)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            if (row != null)
                            {
                                var status = row.IsNull("Status") ? string.Empty : row.GetValue("Status");
                                if (!string.IsNullOrEmpty(status))
                                {
                                    result.Add(status);
                                }
                            }
                        }
                    }
                    return result;
                });
            }
        }
        /// <summary>
        /// 根据服务码配置id查询结算方式和结算价
        /// </summary>
        /// <param name="packageDetailId"></param>
        /// <returns></returns>
        private static Tuple<string, decimal> SelectSettlementMethodAndPrice(int packageDetailId)
        {
            using (var dbHelper = DbHelper.CreateDbHelper(connectionRo))
            using (var cmd = new SqlCommand(@"
SELECT  A.VipSettlementPrice ,
        A.SettlementMethod
FROM    Tuhu_groupon..BeautyServicePackageDetail AS A WITH ( NOLOCK )
WHERE   A.PKID = @PackageDetailId;"))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@PackageDetailId", packageDetailId);
                return dbHelper.ExecuteQuery(cmd, dt =>
                {
                    Tuple<string, decimal> result = null;
                    if (dt != null)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            if (row != null)
                            {
                                var settleMentMethod = row.IsNull("SettlementMethod") ? string.Empty : row.GetValue("SettlementMethod");
                                var vipSettlementPrice = row.IsNull("VipSettlementPrice") ? 0 : Convert.ToDecimal(row.GetValue("VipSettlementPrice"));
                                result = new Tuple<string, decimal>(settleMentMethod, vipSettlementPrice);
                            }
                        }
                    }
                    return result;
                });
            }
        }
        /// <summary>
        /// 根据批次号更新买断订单号
        /// </summary>
        /// <param name="batchCode"></param>
        /// <param name="buyoutOrderId"></param>
        /// <returns></returns>
        public static bool UpdateBuyOutOrderIdByBatchCode(string batchCode, int buyoutOrderId)
        {
            using (var dbHelper = DbHelper.CreateDbHelper(connectionRw))
            using (var cmd = new SqlCommand(@"UPDATE  Tuhu_groupon..CreateBeautyCodeTask
SET     BuyoutOrderId = @BuyoutOrderId
WHERE   BatchCode = @BatchCode;"))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@BuyoutOrderId", buyoutOrderId);
                cmd.Parameters.AddWithValue("@BatchCode", batchCode);
                return dbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }
        /// <summary>
        /// 查询需要创建订单的批次号
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetBatchCodesOfNeedCreateOrder()
        {
            using (var dbHelper = DbHelper.CreateDbHelper(connectionRw))
            using (var cmd = new SqlCommand(@"
SELECT DISTINCT
        A.BatchCode
FROM    Tuhu_groupon..CreateBeautyCodeTask AS A WITH ( NOLOCK )
        JOIN Tuhu_groupon..BeautyServicePackageDetail AS B WITH ( NOLOCK ) ON A.MappingId = B.PKID
WHERE   B.SettlementMethod = 'PreSettled'
        AND ( A.BuyoutOrderId = 0
              OR A.BuyoutOrderId IS NULL
            )"))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                return dbHelper.ExecuteQuery(cmd, (dt) =>
                {
                    var result = new List<string>();
                    if (dt != null && dt.Rows != null)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            var item = row.IsNull("BatchCode") ? null : row.GetValue("BatchCode").ToString();
                            if (!string.IsNullOrEmpty(item))
                                result.Add(item);
                        }
                    }
                    return result;
                });
            }
        }
        /// <summary>
        ///维护用户的userId
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool UpdateBeautyCodeTaskUserIdByPKID(long pkid, Guid userId)
        {
            using (var dbHelper = DbHelper.CreateDbHelper(connectionRw))
            using (var cmd = new SqlCommand(@"
UPDATE  Tuhu_groupon..CreateBeautyCodeTask
SET     UserId = @UserId ,
        LastUpdateDateTime = GETDATE()
WHERE   PKID = @PKID;"))
            {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@PKID", pkid);
                return dbHelper.ExecuteNonQuery(cmd) > 0;
            }
        }

    }
}
