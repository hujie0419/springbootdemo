using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Common.Logging;
using Tuhu.C.Job.CommentStatisticsJob.Dal;
using System.Diagnostics;
using Tuhu.Service.Comment;

namespace Tuhu.C.Job.CommentStatisticsJob.Job
{
    public class UpdateProductShowCommentTimesJob : IJob
    {
        private static ILog Logger = LogManager.GetLogger<UpdateProductShowCommentTimesJob>();
        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("开始刷新");
            var allProductComments = DalProductStatistics.GetProductStatisticsByPage();
            if (allProductComments != null && allProductComments.Any())
            {
                var groupResult = allProductComments.GroupBy(g => g.ProductID);
                Dictionary<string, int> commentTimes = new Dictionary<string, int>();
                var productIds = groupResult.Select(x => x.Key).Split(50);
                try
                {
                    foreach (var items in productIds)
                    {
                        if (items != null && items.Any())
                        {
                            using (var clientComment = new ProductCommentClient())
                            {
                                var productComment = clientComment.GetCommentStatisticCount(items.ToList());
                                if (productComment.Success && productComment.Result != null)
                                {
                                    foreach (var kvp in productComment.Result)
                                    {
                                        if (kvp.Value != 0)
                                        {
                                            commentTimes[kvp.Key] = kvp.Value;
                                        }
                                    }
                                }
                                else
                                {
                                    Logger.Error($"comment服务异常.ex:{productComment.Exception}.msg:{productComment.ErrorMessage}");
                                }
                            }
                        }
                    }
                    if (commentTimes != null && commentTimes.Any())
                    {
                        var issuccess = DalProductStatistics.UpdateShowCommentTimes(commentTimes);
                        Logger.Info($"结束刷新.result:{issuccess}");
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.Error(ex);
                }
            }
            Logger.Info("结束刷新");
        }
    }
}
