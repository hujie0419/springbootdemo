using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common.Logging;
using Quartz;
using Tuhu.C.Job.CommentStatisticsJob.Dal;
using Tuhu.C.Job.CommentStatisticsJob.Model;
using Tuhu.Service.Comment.Models;

namespace Tuhu.C.Job.CommentStatisticsJob.Job
{
    [DisallowConcurrentExecution]
    public class ProductScoreStatisticsJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<ProductScoreStatisticsJob>();

        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info("门店产品评分每日统计Job开始执行");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Warn("门店产品评分每日统计Job", ex);
            }

            watcher.Stop();
            Logger.Info($"门店产品评分每日统计Job完成, 用时{watcher.ElapsedMilliseconds}毫秒");
        }

        private void DoJob()
        {
            var products = DalShopCommentSync.GetProductInfo();
            Logger.Info($"门店韩品评分统计，待统计保养团购商品共{products.Count}");
            var index = 1;        
            foreach (var product in products)
            {
                Logger.Info($"待统计保养团购产品{product}-->{index}/{products.Count}");
                index++;
                var data = DalShopCommentSync.GetProductScore(product);
                foreach (var item in data)
                {
                    var result = DalShopCommentSync.CreatOrUpdateRecord(item);
                    if (!result)
                    {
                        Logger.Warn(
                            $"门店产品评分统计插入或更新数据失败-->{item.Pid}/{item.ShopId}/{item.AvgScore}/{item.CommentCount}");
                    }
                }
            }
        }

        //private static List<ProductScoreModel> GetApprovedData(string pid)
        //{
        //    var result = new List<ProductScoreModel>();
        //    var client = ElasticsearchHelper.CreateClient();
        //    var array = new List<Func<QueryContainerDescriptor<ESShopCommentModel>, QueryContainer>>
        //    {
        //        q => q.Terms(f => f.Field(fd => fd.PIdList).Terms(pid)),
        //        q => q.Range(f => f.Field(fd => fd.InstallShopId).GreaterThan(0))
        //    };
        //    //ElasticsearchHelper.EnableDebug();
        //    var response = client.Search<ESShopCommentModel>(
        //        s =>
        //            s.Index("shopcomment")
        //                .Type("esshopcommentmodel")
        //                .Query(q => q.Bool(qb =>
        //                    qb.Must(array)))
        //                .Size(0)
        //                .Aggregations(ag =>
        //                    ag.Terms("shopId", t => t.Field(f => f.InstallShopId).Aggregations(
        //                        agg => agg.ValueCount("approvedCount", vc => vc.Field(fd => fd.CommentId))
        //                            .Average("approvedAvg", vc => vc.Field(fd => fd.CommentId))))
        //                ));
        //    if (response.IsValid && response.Aggregations.Any())
        //    {
        //        Logger.Info("test");
        //        var shopInfo = (BucketAggregate) response.Aggregations["shopId"];
        //        if (shopInfo != null && shopInfo.Items.Any())
        //        {
        //            foreach (var item in shopInfo.Items)
        //            {
        //                var data = (KeyedBucket) item;
        //                if (data != null && int.TryParse(data?.Key, out var value))
        //                {
        //                    var count = (int?) ((ValueAggregate) data.Aggregations["approvedCount"]).Value;
        //                    var avgScore = ((ValueAggregate) data.Aggregations["approvedAvg"]).Value;
        //                    result.Add(new ProductScoreModel
        //                    {
        //                        shopId = value,
        //                        approvedCount = count ?? 0,
        //                        approvedAvgScore = (decimal) (avgScore ?? 0)
        //                    });
        //                }
        //            }
        //        }
        //    }

        //    return result;

        //}
    }
}
