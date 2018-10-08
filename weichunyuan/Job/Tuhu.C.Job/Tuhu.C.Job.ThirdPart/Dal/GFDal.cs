using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.ThirdPart.Model;
using Tuhu.C.Job.ThirdPart.Model.GFCard;

namespace Tuhu.C.Job.ThirdPart.Dal
{
    public class GFDal
    {
        public static readonly string Tuhu_Groupon_ReadOnly_Str = ConfigurationManager.ConnectionStrings["Tuhu_Groupon_ReadOnly"].ToString();
        public static readonly string Tuhu_Groupon_Write_Str = ConfigurationManager.ConnectionStrings["Tuhu_Groupon"].ToString();
        private static GFDal uniqueInstance;
        private static readonly object locker = new object();
        private GFDal() { }
        public static GFDal GetInstance()
        {
            if (uniqueInstance == null)//避免每次都对线程对象加锁，减少开销
            {
                lock (locker)
                {
                    if (uniqueInstance == null)
                    {
                        uniqueInstance = new GFDal();
                    }
                }
            }

            return uniqueInstance;
        }
        /// <summary>
        /// 根据源文件名查询广发卡用户记录
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <returns></returns>
        public async Task<IEnumerable<GFBankCardRecord>> SelectGFBankCardRecordsBySourceFile(string sourceFile)
        {
            IEnumerable<GFBankCardRecord> result = null;
            var sql = @"
SELECT  A.PKID ,
        A.UserId ,
        A.Mobile ,
        A.UserName ,
        A.CardLevel ,
        A.BusinessType ,
        A.SourceFileName ,
        A.CreatedDateTime ,
        A.UpdatedDateTime
FROM    Tuhu_groupon..GFBankCardRecord AS A WITH ( NOLOCK )
WHERE   A.SourceFileName = @SourceFileName";
            using (var dbHelper = DbHelper.CreateDbHelper(Tuhu_Groupon_ReadOnly_Str))
            using (var cmd = dbHelper.CreateCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@SourceFileName", sourceFile));
                result = (await dbHelper.ExecuteSelectAsync<GFBankCardRecord>(cmd));
            }
            return result ?? new List<GFBankCardRecord>();
        }
        /// <summary>
        /// 根据源文件名查询发券任务
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <returns></returns>
        public async Task<IEnumerable<GFBankPromotionTask>> SelectGFBankPromotionTasksBySourceFile(string sourceFile)
        {
            IEnumerable<GFBankPromotionTask> result = null;
            var sql = @"SELECT  PKID ,
        UserId ,
        Mobile ,
        RuleGuid ,
        PromotionIds ,
        BusinessType ,
        Status ,
        SourceFileName ,
        CreatedDateTime ,
        UpdatedDateTime
FROM    Tuhu_groupon..GFBankPromotionTask WITH ( NOLOCK )
WHERE   SourceFileName = @SourceFileName
ORDER BY PKID DESC; ";
            using (var dbHelper = DbHelper.CreateDbHelper(Tuhu_Groupon_ReadOnly_Str))
            using (var cmd = dbHelper.CreateCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@SourceFileName", sourceFile));
                result = (await dbHelper.ExecuteSelectAsync<GFBankPromotionTask>(cmd));
            }
            return result ?? new List<GFBankPromotionTask>();
        }
        /// <summary>
        /// 查找创建的和失败的发券的任务
        /// </summary>
        /// <returns></returns>
        internal async Task<IEnumerable<GFBankPromotionTask>> SelectCreatedAndFailedGFBankPromotionTask(DateTime startTime, int pageSize, int pageIndex)
        {
            IEnumerable<GFBankPromotionTask> result = null;
            var sql = @"SELECT  A.PKID ,
        A.UserId ,
        A.Mobile ,
        A.RuleGuid ,
        A.PromotionIds ,
        A.BusinessType ,
        A.Status ,
        A.SourceFileName ,
        A.CreatedDateTime ,
        A.UpdatedDateTime
FROM    Tuhu_groupon..GFBankPromotionTask AS A WITH ( NOLOCK )
WHERE   ( A.Status = 'Created'
          OR A.Status = 'Failed'
        )
        AND A.CreatedDateTime > @StartTime
ORDER BY PKID DESC
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
        ONLY; ";
            using (var dbHelper = DbHelper.CreateDbHelper(Tuhu_Groupon_Write_Str))
            using (var cmd = dbHelper.CreateCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@StartTime", startTime));
                cmd.Parameters.Add(new SqlParameter("@PageIndex", pageIndex));
                cmd.Parameters.Add(new SqlParameter("@PageSize", pageSize));
                result = await dbHelper.ExecuteSelectAsync<GFBankPromotionTask>(cmd);
            }

            return result ?? new List<GFBankPromotionTask>();
        }
        internal async Task<IEnumerable<GFBankRedemptionCodeTask>> SelectCreatedAndFailedGFBankRedemptionCodeTask(DateTime startTime, int pageSize, int pageIndex)
        {
            IEnumerable<GFBankRedemptionCodeTask> result = null;
            var sql = @"SELECT  A.PKID ,
        A.UserId ,
        A.Mobile ,
        A.RedemptionCode ,
        A.BusinessType ,
        A.Status ,
        A.SourceFileName ,
        A.CreatedDateTime ,
        A.UpdatedDateTime
FROM    Tuhu_groupon..GFBankRedemptionCodeTask AS A WITH ( NOLOCK )
WHERE   ( A.Status = 'Created'
          OR A.Status = 'Failed'
        )
        AND A.CreatedDateTime > @StartTime
ORDER BY PKID DESC
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
        ONLY; ";
            using (var dbHelper = DbHelper.CreateDbHelper(Tuhu_Groupon_Write_Str))
            using (var cmd = dbHelper.CreateCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@StartTime", startTime));
                cmd.Parameters.Add(new SqlParameter("@PageIndex", pageIndex));
                cmd.Parameters.Add(new SqlParameter("@PageSize", pageSize));
                result = await dbHelper.ExecuteSelectAsync<GFBankRedemptionCodeTask>(cmd);
            }

            return result ?? new List<GFBankRedemptionCodeTask>();
        }
        /// <summary>
        /// 查询发短信任务
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        internal async Task<IEnumerable<GFBankSmsTask>> SelectGFSmsTasks(DateTime startTime, int pageSize, int pageIndex, string status)
        {
            IEnumerable<GFBankSmsTask> result = new List<GFBankSmsTask>();
            var sql = @"SELECT  B.Mobile
FROM    ( SELECT DISTINCT
                    A.Mobile
          FROM      Tuhu_groupon..GFBankPromotionTask AS A WITH ( NOLOCK )
          WHERE     A.Status = @Status
                    AND A.BusinessType = 'Activate'
                    AND A.CreatedDateTime > @StartTime
        ) AS B
ORDER BY B.Mobile DESC
        OFFSET ( @PageIndex - 1 ) * @PageSize ROWS FETCH NEXT @PageSize ROWS
        ONLY;";
            using (var dbHelper = DbHelper.CreateDbHelper(Tuhu_Groupon_Write_Str))
            using (var cmd = dbHelper.CreateCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@StartTime", startTime));
                cmd.Parameters.Add(new SqlParameter("@PageIndex", pageIndex));
                cmd.Parameters.Add(new SqlParameter("@PageSize", pageSize));
                cmd.Parameters.Add(new SqlParameter("@Status", status));
                result = await dbHelper.ExecuteSelectAsync<GFBankSmsTask>(cmd);
            }

            return result ;
        }
        /// <summary>
        /// 批量更新广发发短信状态
        /// </summary>
        /// <param name="smsTask"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        internal async Task<bool> BatchUpdateGFBankSmsTasksStatus(IEnumerable<GFBankSmsTask> smsTask, string status, DateTime startTime)
        {
            var sql = @"UPDATE  A
SET     A.Status = @PromotionTaskStatus
FROM    Tuhu_groupon..GFBankPromotionTask AS A WITH ( NOLOCK )
        JOIN Tuhu_groupon..SplitString(@Mobiles, ',', 1) AS B ON A.Mobile = B.Item
WHERE   A.CreatedDateTime > @StartTime;";
            using (var dbHelper = DbHelper.CreateDbHelper(Tuhu_Groupon_Write_Str))
            using (var cmd = dbHelper.CreateCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@PromotionTaskStatus", status));
                cmd.Parameters.Add(new SqlParameter("@Mobiles", string.Join(",", smsTask.Select(s => s.Mobile))));
                cmd.Parameters.Add(new SqlParameter("@StartTime",startTime));
                return (await dbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }
        /// <summary>
        /// 插入广发联名卡记录
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public async Task<bool> InsertGFBankCardRecord(GFBankCardRecord record)
        {
            var sql = @"IF NOT EXISTS ( SELECT  1
                FROM    Tuhu_groupon..GFBankCardRecord WITH ( NOLOCK )
                WHERE   Mobile = @Mobile
                        AND SourceFileName = @SourceFileName )
    INSERT  Tuhu_groupon..GFBankCardRecord
            ( UserId ,
              Mobile ,
              UserName ,
              CardLevel ,
              BusinessType ,
              SourceFileName 
            )
    VALUES  ( @UserId ,
              @Mobile ,
              @UserName ,
              @CardLevel ,
              @BusinessType ,
              @SourceFileName
            );";
            using (var dbHelper = DbHelper.CreateDbHelper(Tuhu_Groupon_Write_Str))
            using (var cmd = dbHelper.CreateCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@UserId", record.UserId));
                cmd.Parameters.Add(new SqlParameter("@Mobile", record.Mobile));
                cmd.Parameters.Add(new SqlParameter("@UserName", record.UserName));
                cmd.Parameters.Add(new SqlParameter("@CardLevel", record.CardLevel));
                cmd.Parameters.Add(new SqlParameter("@BusinessType", record.BusinessType));
                cmd.Parameters.Add(new SqlParameter("@SourceFileName", record.SourceFileName));
                return (await dbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }
        /// <summary>
        /// 插入广发联名卡发券任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task<bool> InsertGFBankPromotionTask(GFBankPromotionTask task)
        {
            var sql = @"INSERT  Tuhu_groupon..GFBankPromotionTask
        ( UserId ,
          Mobile ,
          RuleGuid ,
          BusinessType ,
          Status ,
          SourceFileName 
        )
VALUES  ( @UserId ,
          @Mobile ,
          @RuleGuid ,
          @BusinessType ,
          @Status ,
          @SourceFileName 
        );";
            using (var dbHelper = DbHelper.CreateDbHelper(Tuhu_Groupon_Write_Str))
            using (var cmd = dbHelper.CreateCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@UserId", task.UserId));
                cmd.Parameters.Add(new SqlParameter("@Mobile", task.Mobile));
                cmd.Parameters.Add(new SqlParameter("@RuleGuid", task.RuleGuid));
                cmd.Parameters.Add(new SqlParameter("@BusinessType", task.BusinessType));
                cmd.Parameters.Add(new SqlParameter("@Status", task.Status));
                cmd.Parameters.Add(new SqlParameter("@SourceFileName", task.SourceFileName));
                return (await dbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }

        public async Task<bool> InsertGFBankRedemptionCodeTask(GFBankRedemptionCodeTask task)
        {
            var sql = @"INSERT  INTO Tuhu_groupon..GFBankRedemptionCodeTask
        ( UserId ,
          Mobile ,
          RedemptionCode ,
          BusinessType ,
          Status ,
          SourceFileName ,
          CreatedDateTime ,
          UpdatedDateTime
        )
VALUES  ( @UserId , 
          @Mobile , 
          @RedemptionCode , 
          @BusinessType , 
          @Status , 
          @SourceFileName , 
          GETDATE() , 
          GETDATE()  
        );";
            using (var dbHelper = DbHelper.CreateDbHelper(Tuhu_Groupon_Write_Str))
            using (var cmd = dbHelper.CreateCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@UserId", task.UserId));
                cmd.Parameters.Add(new SqlParameter("@Mobile", task.Mobile));
                cmd.Parameters.Add(new SqlParameter("@RedemptionCode", task.RedemptionCode));
                cmd.Parameters.Add(new SqlParameter("@BusinessType", task.BusinessType));
                cmd.Parameters.Add(new SqlParameter("@Status", task.Status));
                cmd.Parameters.Add(new SqlParameter("@SourceFileName", task.SourceFileName));
                return (await dbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }
        /// <summary>
        /// 更新任务状态
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<bool> UpdateGFBankPromotionTaskStatus(int pkid, string status)
        {
            var sql = @"UPDATE  Tuhu_groupon..GFBankPromotionTask
SET     Status = @Status ,
        UpdatedDateTime = GETDATE()
WHERE   PKID = @PKID;";
            using (var dbHelper = DbHelper.CreateDbHelper(Tuhu_Groupon_Write_Str))
            using (var cmd = dbHelper.CreateCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@Status", status));
                cmd.Parameters.Add(new SqlParameter("@PKID", pkid));
                return (await dbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }
        /// <summary>
        /// 更新任务状态
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<bool> UpdateGFBankRedemptionCodeTaskStatus(int pkid, string status)
        {
            var sql = @"UPDATE  Tuhu_groupon..GFBankRedemptionCodeTask
SET     Status = @Status ,
        UpdatedDateTime = GETDATE()
WHERE   PKID = @PKID;";
            using (var dbHelper = DbHelper.CreateDbHelper(Tuhu_Groupon_Write_Str))
            using (var cmd = dbHelper.CreateCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@Status", status));
                cmd.Parameters.Add(new SqlParameter("@PKID", pkid));
                return (await dbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }
        /// <summary>
        /// 更新任务优惠券
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="promotionIds"></param>
        /// <returns></returns>
        public async Task<bool> UpdateGFBankPromotionTaskPromotionIds(int pkid, string promotionIds)
        {
            var sql = @"UPDATE  Tuhu_groupon..GFBankPromotionTask
SET     PromotionIds = @PromotionIds ,
        UpdatedDateTime = GETDATE()
WHERE   PKID = @PKID;";
            using (var dbHelper = DbHelper.CreateDbHelper(Tuhu_Groupon_Write_Str))
            using (var cmd = dbHelper.CreateCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@PromotionIds", promotionIds));
                cmd.Parameters.Add(new SqlParameter("@PKID", pkid));
                return (await dbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }
        /// <summary>
        /// 更新任务兑换码
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="redemptionCode"></param>
        /// <returns></returns>
        public async Task<bool> UpdateGFBankRedemptionCodeTaskRedemptionCode(int pkid, string redemptionCode)
        {
            var sql = @"UPDATE  Tuhu_groupon..GFBankRedemptionCodeTask
SET     RedemptionCode = @RedemptionCode ,
        UpdatedDateTime = GETDATE()
WHERE   PKID = @PKID;";
            using (var dbHelper = DbHelper.CreateDbHelper(Tuhu_Groupon_Write_Str))
            using (var cmd = dbHelper.CreateCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@RedemptionCode", redemptionCode));
                cmd.Parameters.Add(new SqlParameter("@PKID", pkid));
                return (await dbHelper.ExecuteNonQueryAsync(cmd)) > 0;
            }
        }
        /// <summary>
        /// 根据手机号和任务业务类型查任务记录
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="businessType"></param>
        /// <returns></returns>
        public async Task<IEnumerable<GFBankPromotionTask>> SelectGFBankPromotionTaskByMobile(string mobile)
        {            
            IEnumerable<GFBankPromotionTask> result = null;
            var sql = @"SELECT  A.PKID ,
        A.UserId ,
        A.Mobile ,
        A.RuleGuid ,
        A.PromotionIds ,
        A.BusinessType ,
        A.Status ,
        A.SourceFileName ,
        A.CreatedDateTime ,
        A.UpdatedDateTime
FROM    Tuhu_groupon..GFBankPromotionTask AS A WITH ( NOLOCK )
WHERE   A.Mobile = @Mobile 
ORDER BY PKID DESC; ";
            using (var dbHelper = DbHelper.CreateDbHelper(Tuhu_Groupon_ReadOnly_Str))
            using (var cmd = dbHelper.CreateCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@Mobile", mobile));
                result = await dbHelper.ExecuteSelectAsync<GFBankPromotionTask>(cmd);
            }

            return result ?? new List<GFBankPromotionTask>();
        }
        /// <summary>
        /// 根据手机号查询用户兑换码任务
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="businessType"></param>
        /// <returns></returns>
        public async Task<IEnumerable<GFBankRedemptionCodeTask>> SelectGFBankRedemptionCodeTaskByMobile(string mobile, string businessType)
        {
            IEnumerable<GFBankRedemptionCodeTask> result = null;
            var sql = @"SELECT  A.PKID ,
        A.UserId ,
        A.Mobile ,
        A.RedemptionCode ,
        A.BusinessType ,
        A.Status ,
        A.SourceFileName ,
        A.CreatedDateTime ,
        A.UpdatedDateTime
FROM    Tuhu_groupon..GFBankRedemptionCodeTask AS A WITH ( NOLOCK )
WHERE   A.Mobile = @Mobile
        AND A.BusinessType = @BusinessType
ORDER BY PKID DESC;";
            using (var dbHelper = DbHelper.CreateDbHelper(Tuhu_Groupon_ReadOnly_Str))
            using (var cmd = dbHelper.CreateCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@Mobile", mobile));
                cmd.Parameters.Add(new SqlParameter("@BusinessType", businessType));
                result = await dbHelper.ExecuteSelectAsync<GFBankRedemptionCodeTask>(cmd);
            }

            return result ?? new List<GFBankRedemptionCodeTask>();
        }
        /// <summary>
        /// 根据手机号和任务业务类型查兑换码任务记录
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="businessType"></param>
        /// <returns></returns>
        public async Task<IEnumerable<GFBankRedemptionCodeTask>> SelectGFBankRedemptionCodeTask(string mobile)
        {
            IEnumerable<GFBankRedemptionCodeTask> result = null;
            var sql = @"SELECT  A.PKID ,
        A.UserId ,
        A.Mobile ,
        A.RedemptionCode ,
        A.BusinessType ,
        A.Status ,
        A.SourceFileName ,
        A.CreatedDateTime ,
        A.UpdatedDateTime
FROM    Tuhu_groupon..GFBankRedemptionCodeTask AS A WITH ( NOLOCK )
WHERE   A.Mobile = @Mobile
ORDER BY PKID DESC;  ";
            using (var dbHelper = DbHelper.CreateDbHelper(Tuhu_Groupon_ReadOnly_Str))
            using (var cmd = dbHelper.CreateCommand(sql))
            {
                cmd.Parameters.Add(new SqlParameter("@Mobile", mobile));
                result = await dbHelper.ExecuteSelectAsync<GFBankRedemptionCodeTask>(cmd);
            }
            return result ?? new List<GFBankRedemptionCodeTask>();
        }
    }
}
