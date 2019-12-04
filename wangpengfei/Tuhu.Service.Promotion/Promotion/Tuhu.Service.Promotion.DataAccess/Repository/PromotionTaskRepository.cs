using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Promotion.DataAccess.Entity;
using System.Linq;
using Tuhu.Service.Promotion.DataAccess.IRepository;
using Tuhu.Service.Promotion.Request;

namespace Tuhu.Service.Promotion.DataAccess.Repository
{
    /// <summary>
    /// 优惠券任务
    /// </summary>
    public class PromotionTaskRepository : IPromotionTaskRepository
    {

        private string DBName = "Gungnir..tbl_PromotionTask";
        private readonly IDbHelperFactory _factory;


        public PromotionTaskRepository(IDbHelperFactory factory) => _factory = factory;

        /// <summary>
        /// 获取优惠券任务列表
        /// </summary
        /// <param name="PKID"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<List<PromotionTaskEntity>> GetPromotionTaskListAsync(GetPromotionTaskListRequest request, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"SELECT [PromotionTaskId]
                                  --,[TaskName]
                                  ,[TaskType]
                                  ,[TaskStartTime]
                                  ,[TaskEndTime]
                                  --,[ExecPeriod]
                                  --,[TaskStatus]
                                  --,[CreateTime]
                                  --,[SelectUserType]
                                  ,[FilterStartTime]
                                  ,[FilterEndTime]
                                  ,[Brand]
                                  ,[Category]
                                  ,[Pid]
                                  ,[SpendMoney]
                                  ,[PurchaseNum]
                                  ,[Area]
                                  ,[Channel]
                                  ,[OrderType]
                                  ,[InstallType]
                                  ,[OrderStatus]
                                  ,[Seable]
                                  ,[Vehicle]
                                  --,[UpdateTime]
                                  ,[CouponRulesIds]
                                  --,[Creater]
                                  --,[Auditor]
                                  --,[AuditTime]
                                  --,[ExecuteTime]
                                  --,[ShipStatus]
                                  --,[IsImmediately]
                                  --,[CloseTime]
                                  ,[IsLimitOnce]
                                  ,[SmsId]
                                  --,[PromotionTaskActivityId]
                                  ,[SmsParam]
                                  ,[ProductType]
                                  --,[GetQuantity]
                                  --,[SendFailedQuantity]
                              FROM {DBName} with (nolock)
                              where @TaskTime between TaskStartTime and TaskEndTime 
                        ";
            #endregion
            List<SqlParameter> sqlParaments = new List<SqlParameter>();

            sqlParaments.Add(new SqlParameter("@TaskTime", request.TaskTime.Value));
            //if (request.TaskTime.HasValue)
            //{
            //    sql += " and TaskStartTime <= @TaskTime  and TaskEndTime >= @TaskTime ";
            //    sqlParaments.Add(new SqlParameter("@TaskTime", request.TaskTime.Value));
            //}

            if (request.TaskType != 0)
            {
                sql += " and TaskType = @TaskType ";
                sqlParaments.Add(new SqlParameter("@TaskType", request.TaskType));
            }

            if (request.TaskStatus >= 0)
            {
                sql += " and TaskStatus = @TaskStatus ";
                sqlParaments.Add(new SqlParameter("@TaskStatus", request.TaskStatus));
            }

            using (var dbHelper = _factory.CreateDbHelper("Gungnir", true))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddRange(sqlParaments.ToArray());
                    var result = (await dbHelper.ExecuteSelectAsync<PromotionTaskEntity>(cmd, cancellationToken).ConfigureAwait(false));
                    return result.ToList();
                }
            }
        }

        /// <summary>
        /// 关闭任务
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<bool> ClosePromotionTaskByPKIDAsync(ClosePromotionTaskByPkidRequest request, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"UPDATE {DBName}
                               SET          
                                   TaskStatus = 2
                                   ,ExecuteTime = getdate()
                                  ,CloseTime = getdate()
                                where PromotionTaskId =@PromotionTaskId
                                ;";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Gungnir", false))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PromotionTaskId", request.PromotionTaskId));
                    var result = (await dbHelper.ExecuteNonQueryAsync(cmd, cancellationToken).ConfigureAwait(false));
                    return result > 0;
                }
            }
        }

        /// <summary>
        /// 更新任务的领取成功和失败数目
        /// </summary>
        /// <param name="PromotionTaskId">任务id</param>
        /// <param name="type">0=成功次数，1=失败次数</param>
        /// <param name="IncrementCount">增加的次数</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<bool> UpdatePromotionTaskSendCountAsync(int PromotionTaskId, int type, int IncrementCount, CancellationToken cancellationToken)
        {
            string updateContent = "";
            if (type == 0)
            {
                updateContent = "GetQuantity = GetQuantity + @IncrementCount";
            }
            else if (type == 1)
            {
                updateContent = "SendFailedQuantity = SendFailedQuantity + @IncrementCount";
            }
            else
            {
                return false;
            }

            #region sql
            string sql = $@"update  {DBName}
                            set  {updateContent}
                            where PromotionTaskId = @PromotionTaskId
                        ;";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Gungnir", false))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PromotionTaskId", PromotionTaskId));
                    cmd.Parameters.Add(new SqlParameter("@IncrementCount", IncrementCount));
                    return (await dbHelper.ExecuteNonQueryAsync(cmd, cancellationToken).ConfigureAwait(false)) > 0;
                }
            }
        }

        /// <summary>
        ///更新优惠券任务 发送成功和失败数目
        /// </summary>
        /// <param name="PromotionTaskId">任务id</param>
        /// <param name="successCount">成功数</param>
        /// <param name="failCount">失败数</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<bool> UpdatePromotionTaskCountAsync(int PromotionTaskId, int successCount, int failCount, CancellationToken cancellationToken)
        {
            #region sql
            string sql = $@"update  {DBName}
                            set  GetQuantity = GetQuantity + @successCount
                                 ,SendFailedQuantity = SendFailedQuantity + @failCount
                                 ,updateTime = getdate()
                            where PromotionTaskId = @PromotionTaskId
                        ;";
            #endregion

            using (var dbHelper = _factory.CreateDbHelper("Gungnir", false))
            {
                using (var cmd = new SqlCommand(sql))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@PromotionTaskId", PromotionTaskId));
                    cmd.Parameters.Add(new SqlParameter("@successCount", successCount));
                    cmd.Parameters.Add(new SqlParameter("@failCount", failCount));
                    return (await dbHelper.ExecuteNonQueryAsync(cmd, cancellationToken).ConfigureAwait(false)) > 0;
                }
            }
        }

    }
}
