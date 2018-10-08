using System;
using System.Linq;
using Quartz;
using Common.Logging;
using Tuhu.C.Job.CommentStatisticsJob.Dal;
using System.Threading.Tasks;
using System.Collections.Generic;
using Tuhu.C.Job.CommentStatisticsJob.Model;
using System.Diagnostics;
using Tuhu.Service.Comment;
using Tuhu.Service.Comment.Models;
using Tuhu.Nosql;
using Tuhu.Service;
using Tuhu.Service.Order;
using Tuhu.Service.Product;

namespace Tuhu.C.Job.CommentStatisticsJob.Job
{
    /// <summary>
    /// 门店 评论   分类类型 数据初始化（非每天全量 & 手动触发）
    /// </summary>
    [DisallowConcurrentExecution]
    class UpdateCommentShopTypeJob : IJob
    {
        private static ILog Logger = LogManager.GetLogger<UpdateCommentShopTypeJob>();
        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info($"更新门店评论的类型 UpdateCommentShopTypeJob 执行开始");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Error($"更新门店评论的类型 UpdateCommentShopTypeJob 更新异常：{ex}");
            }
            Logger.Info($"更新门店评论的类型 UpdateCommentShopTypeJob 更新完成 time={watcher.ElapsedMilliseconds}ms");
            watcher.Stop();
        }

        public static void  DoJob()
        {
            DateTime start = new DateTime(2016, 1, 1) ;
            DateTime end = DateTime.Now;

            int pageIndex = 1;
            int pageSize = 600;
            int total = DalShopCommentSync.GetShopCommentCountByTime(start.ToString("yyyy-MM-dd"), end.AddDays(1).ToString("yyyy-MM-dd"));
            int pageTotal = (total - 1) / pageSize + 1;
           
            List<int> pageIndexList = new List<int>();

            for (; pageIndex <= pageTotal; pageIndex++)
            {
                pageIndexList.Add(pageIndex);
            }
            //foreach (var f in pageIndexList)
            //{
            //    var sw = new Stopwatch();
            //    sw.Start();
            //    BatchUpdate(f, pageSize, start, end);
            //    logger.Info($"更新门店评论的类型 第{f}页  页数={pageSize} 更新完成 time={sw.ElapsedMilliseconds}ms");
            //    sw.Stop();
            //}
            Parallel.ForEach(pageIndexList, new ParallelOptions() { MaxDegreeOfParallelism = 5 },f =>
            {
                var sw = new Stopwatch();
                sw.Start();
                BatchUpdate(f, pageSize, start, end);
                //AsyncHelper.RunSync(() => BatchUpdate(f, pageSize, start, end));

                Logger.Info($"更新门店评论的类型 第{f}页  页数={pageSize} 更新完成 time={sw.ElapsedMilliseconds}ms");
                sw.Stop();
            });
        }

        private static  void BatchUpdate(int pageIndex,int pageSize,DateTime start,DateTime end)
        {
            List<ShopCommentDataModel> comments = DalShopCommentSync.GetShopCommentsPage(pageIndex, pageSize, start.ToString("yyyy-MM-dd"), end.AddDays(1).ToString("yyyy-MM-dd")).ToList();

            #region 批量更新 
            using (var client = new ShopCommentClient())
            {
                var result = client.BatchGetShopCommentType(comments.Select(p => p.OrderId).ToList());
                if (result.Success && result.Result.Any())
                {
                    Logger.Info($"更新门店评论的类型 BatchGetShopCommentType pageIndex ={pageIndex} 有效的 shoptype 数目 ={result.Result.Where(p => !string.IsNullOrWhiteSpace(p.ShopType)).Count()} time={result.ElapsedMilliseconds} ");
                    foreach (var item in comments)
                    {
                        try
                        {
                            if (result.Success && result.Result.Any())
                            {
                                string ShopType = result.Result.Where(p => p.OrderId == item.OrderId).FirstOrDefault()?.ShopType;
                                if (!string.IsNullOrWhiteSpace(ShopType))
                                {
                                    if (DalShopCommentSync.UpdateShopCommentShopType(item.CommentId ?? 0, ShopType))
                                    {
                                        //logger.Info($"更新门店评论的类型 更新  完成 CommentId={item.CommentId}& ShopType={ShopType}");
                                    }
                                    else
                                    {
                                        Logger.Error($"更新门店评论的类型 UpdateShopCommentShopType 更新失败 CommentId={item.CommentId}& ShopType={ShopType} ");
                                    }
                                }
                                else
                                {
                                    //logger.Info($"更新门店评论的类型 BatchGetShopCommentType 更新 失败 CommentId={item.CommentId}& ShopType={ShopType}");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Info($"更新门店评论的类型 BatchGetShopCommentType 更新 失败 CommentId={item.CommentId}& ex={ex.Message}");
                        }
                    }
                }
                else
                {
                    Logger.Info($"更新门店评论的类型  更新失败 BatchGetShopCommentType 异常 message={result.ErrorMessage} & count ={result.Result.Count()}");
                }
            }
            #endregion

        }

    }
}
