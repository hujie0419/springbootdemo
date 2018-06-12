using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Member;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.Job
{
    [DisallowConcurrentExecution]
    public class RefreshRandomPromotionCacheJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RefreshRandomPromotionCacheJob));

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("开始刷新随机优惠券活动缓存");

            try
            {
                var allActivityId = GetAllActivityId();
                if (allActivityId != null && allActivityId.Any())
                {
                    foreach (var item in allActivityId)
                    {
                        using (var client = new PromotionClient())
                        {
                            var result = client.RefreshRandomPromotionCacheByKey(item);
                            result.ThrowIfException(true);
                            if (!result.Result)
                                Logger.Error("刷新随机优惠券活动缓存失败");
                        }
                    }
                }
                else
                {
                    Logger.Info("没有配置随机优惠券活动");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("刷新随机优惠券活动缓存Job异常", ex);
            }
        }

        public List<Guid> GetAllActivityId()
        {
            List<Guid> result = new List<Guid>();
            using (var cmd = new SqlCommand(@" SELECT ActivityId FROM   Activity..CouponActivityConfig WITH ( NOLOCK ) WHERE StartTime<=GETDATE() AND EndTime >=GETDATE() OR ValidDays > 0;"))
            {
                var dt = Tuhu.WebSite.Component.SystemFramework.DbHelper.ExecuteDataTable(true, cmd);
                result = dt.Rows.OfType<DataRow>().Select(x => Guid.Parse(x["ActivityId"].ToString())).ToList();
            }
            return result;
        }
    }
}
