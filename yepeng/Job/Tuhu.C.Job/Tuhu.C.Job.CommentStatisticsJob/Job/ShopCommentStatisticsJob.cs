using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Tuhu.C.Job.CommentStatisticsJob.Dal;
using Tuhu.Nosql;
using Tuhu.Service.Comment;

namespace Tuhu.C.Job.CommentStatisticsJob.Job
{
    /// <summary>
    /// 统计产品在门店里的平均分
    /// </summary>
    [DisallowConcurrentExecution]
    public class ShopCommentStatisticsJob:IJob
    {
        private static ILog logger = LogManager.GetLogger<ShopCommentStatisticsJob>();
        private int a = 1;
        public void Execute(IJobExecutionContext context)
        {
            //分页读取门店信息
            //分页获取美容团购订单，对已经统计过的产品-shopid，不在统计，对没统计过的产品，进行统计
            //把统计的结果写入到 Tuhu_comment..ShopCommentStatistics

            try
            {
                logger.Info("开始执行");
                int pageIndex = 1;
                int pageSize = 1000;
                int total = DalShopStatistics.GetShopsCount();
                int pageTotal = (total - 1) / pageSize + 1;

                for (; pageIndex <= pageTotal; pageIndex++)
                {
                    logger.Info($"门店页码 {pageIndex}/{pageTotal}");
                    var shops = DalShopStatistics.GetShopsPage(pageIndex, pageSize);
                    shops.ForEach((item) =>
                    {
                        //查询门店里的美容团购订单
                        int shopPageIndex = 1;
                        int shopPageSize = 1000;
                        int shopTotal = DalShopCommentSync.GetShopCommentOrderCount(item.PKID);
                        int shopPageTotal = (shopTotal - 1) / shopPageSize + 1;
                        for (; shopPageIndex <= shopPageTotal; shopPageIndex++)
                        {
                            logger.Info($"门店评论页码 shopId:{item.PKID} {shopPageIndex}/{shopPageTotal}");
                            var orders = DalShopCommentSync.GetShopCommentOrder(shopPageIndex, shopPageSize, item.PKID);
                            var index = shopPageIndex;
                            Parallel.ForEach(orders, new ParallelOptions() {MaxDegreeOfParallelism = 5}, order =>
                            {
                                //获取这个订单下的所有产品
                                var orderProducts = DalShopCommentSync.GetShopOrderProduct(order.OrderId);
                                logger.Info($"门店评论页码 shopId:{item.PKID} {index}/{shopPageTotal} orderid:{order.OrderId}");
                                foreach (var product in orderProducts)
                                {
                                    using (var client =
                                        CacheHelper.CreateCounterClient(typeof(ShopCommentStatisticsJob).FullName,
                                            TimeSpan.FromHours(12)))
                                    {
                                        var response = client.Increment(item.PKID + product.Pid);
                                        if (response.Success && response.Value > 1)
                                        {
                                            continue;
                                        }
                                    }

                                    //获取这个门店下这个产品对应的所有订单号
                                    var productOrders =
                                        DalShopCommentSync.GetShopProductOrder(item.PKID, product.Pid);
                                    //根据这个产品的所有订单号统计评论的平均分，写入到这个产品下
                                    var statistics = DalShopCommentSync.GetShopCommentStatistics(item.PKID, product.Pid,
                                        productOrders.Select(x => x.CommentId));

                                    foreach (var s in statistics)
                                    {
                                        DalShopCommentSync.SyncShopCommentStatistics(s.ShopId, s);
                                    }
                                }

                            });
                            logger.Info($"门店评论页码 shopId:{item.PKID} {shopPageIndex}/{shopPageTotal} 执行结束");
                        }

                    });
                    logger.Info($"门店页码 {pageIndex}/{pageTotal} 执行结束");
                }
                using (var client = new ShopCommentClient())
                {
                    client.RefreshCommentStatisticsCache(2);
                }
                logger.Info("执行结束");
            }
            catch (Exception e)
            {

                logger.Error("执行异常", e);
            }
        }





    }
}
