using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Tuhu.C.Job.CommentStatisticsJob.Dal;
using Tuhu.Service.Comment;

namespace Tuhu.C.Job.CommentStatisticsJob.Job
{
    /// <summary>
    /// 每天统计技师平均分
    /// </summary>
    [DisallowConcurrentExecution]
    public class TechnicianCommentStatisticsJob : IJob
    {
        ILog Logger = LogManager.GetLogger<TechnicianCommentStatisticsJob>();
        public void Execute(IJobExecutionContext context)
        {

            //分页读取技师信息
            //从Tuhu_comment..TechnicianComment 里统计每个技师的平均分
            //写入到 Tuhu_comment..TechnicianStatistics 里
            try
            {
                Logger.Info("开始执行");
                int pageIndex = 1;
                int pageSize = 1000;
                int total = DalShopStatistics.GetTechsCount();
                int pageTotal = (total - 1) / pageSize + 1;

                for (; pageIndex <= pageTotal; pageIndex++)
                {
                    var techs = DalShopStatistics.GetTechsPage(pageIndex, pageSize);
                    techs.ForEach((item) =>
                    {
                        //查询门店里的美容团购订单
                        var statistics = DalShopCommentSync.GetTechCommentStatistics(item.PKID);

                        Parallel.ForEach(statistics, new ParallelOptions() { MaxDegreeOfParallelism = 3 }, s =>
                        {
                            DalShopCommentSync.SyncTechCommentStatistics(s.ShopId, s.TechnicianId, s);
                        });
                    });
                }

                using (var client = new ShopCommentClient()) {
                    client.RefreshCommentStatisticsCache(1);
                }
                Logger.Info("执行结束");
            }
            catch (Exception e)
            {

                Logger.Error("执行异常", e);
            }
            
        }
    }
}
