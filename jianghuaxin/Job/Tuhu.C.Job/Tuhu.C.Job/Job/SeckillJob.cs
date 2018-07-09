using Common.Logging;
using Quartz;
using System;
using System.Data;
using System.Data.SqlClient;
using Tuhu.Service.Activity;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class SeckillJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WeChatActivitySystemJob));
        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("启动任务");
            UpdateFalseSale();
            Logger.Info("结束任务");
        }

        /// <summary>
        /// 获取正在显示天天秒杀，限时抢购的GUID
        /// </summary>
        /// <returns></returns>
        private DataTable GetSeckillFalshSaleActivityGuid()
        {
            string sql = @"SELECT TOP 6  EDSI.FlashActivityGuid  
                        FROM Configuration.dbo.SE_EveryDaySeckill (NOLOCK) EDS LEFT JOIN Configuration.dbo.SE_EveryDaySeckillInfo (NOLOCK) EDSI 
                            ON EDS.ActivityGuid = EDSI.FK_EveryDaySeckill  
                      WHERE EDS.EndDate > GETDATE() 
                      ORDER BY EDS.StartDate ASC";
            using (var cmd = new SqlCommand(sql))
                return DbHelper.ExecuteQuery(true, cmd, _ => _);
        }


        private void UpdateFalseSale()
        {
            using (var client = new FlashSaleClient())
            {
                foreach (DataRow dr in GetSeckillFalshSaleActivityGuid()?.Rows)
                {
                    Guid activityGuid;
                    if (Guid.TryParse(dr[0].ToString(), out activityGuid))
                    {
                        client.UpdateFlashSaleDataToCouchBaseByActivityID(activityGuid);
                    }
                }
            }
        }


    }
}
