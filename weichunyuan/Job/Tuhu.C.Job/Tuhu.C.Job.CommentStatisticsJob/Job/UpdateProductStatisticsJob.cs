using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Common.Logging;
using Tuhu.C.Job.CommentStatisticsJob.Dal;
using System.Diagnostics;

namespace Tuhu.C.Job.CommentStatisticsJob.Job
{
    /// <summary>
    /// 产品信息统计（包含评论）
    /// </summary>
    [DisallowConcurrentExecution]
    public class UpdateProductStatisticsJob : IJob
    {
        private static ILog UpdateProductStatisticsLogger = LogManager.GetLogger<UpdateProductStatisticsJob>();
        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            UpdateProductStatisticsLogger.Info($"产品评分统计更新开始");
            int index = 1;
            try
            {
                var allProductComments = DalProductStatistics.GetProductStatisticsByPage();
                if (allProductComments != null && allProductComments.Any())
                {
                    allProductComments.GroupBy(g => g.ProductID).ForEach(f =>
                    {
                        try
                        {
                            var mergeResult = DalProductStatistics.MergeIntoProductStatistics(f.ToList());
                            if (!mergeResult)
                            {
                                UpdateProductStatisticsLogger.Error($"MergeIntoProductStatistics-->产品评分统计更新失败：ProductId={f.Key}");
                            }
                            //var dbProductStatistics = DalProductStatistics.GetProductStatisticsByProductId(f.Key);
                            //dbProductStatistics.Split(100).ForEach(oneList => {
                            //    var mergeResult = DalProductStatistics.MergeIntoProductStatistics(oneList?.ToList());
                            //    if (!mergeResult)
                            //    {
                            //        UpdateProductStatisticsLogger.Error($"MergeIntoProductStatistics-->产品评分统计更新失败：ProductId={f.Key}");
                            //    }
                            //});
                        }
                        catch (Exception ex)
                        {
                            UpdateProductStatisticsLogger.Info($"产品评分统计更新异常:ProductId={f.Key};{ex}");
                        }

                        index++;
                        UpdateProductStatisticsLogger.Info($"产品评分统计更新:第{index}个商品ProductId={f.Key}数据更新完成");
                    });
                    if (!DalProductStatistics.UpdatetCarPAR_CatalogProducts())//更新CarPAR_CatalogProducts中的OrderQuantity,SalesQuantity数据
                        UpdateProductStatisticsLogger.Info($"产品评分统计更新:OrderQuantity,SalesQuantity更新失败");
                }
            }
            catch (Exception ex)
            {
                UpdateProductStatisticsLogger.Info($"产品评分统计更新异常:{ex}");
            }

            watcher.Stop();
            UpdateProductStatisticsLogger.Info($"产品评分统计更新结束 time= {watcher.ElapsedMilliseconds}ms");
            
        }
    }
}
