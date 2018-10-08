using System;
using System.Linq;
using Quartz;
using Common.Logging;
using Tuhu.C.Job.CommentStatisticsJob.Dal;
using System.Threading.Tasks;
using System.Collections.Generic;
using Tuhu.C.Job.CommentStatisticsJob.Model;
using System.Diagnostics;

namespace Tuhu.C.Job.CommentStatisticsJob.Job
{
    /// <summary>
    /// 门店评论分类统计（每天全量）
    /// </summary>
    [DisallowConcurrentExecution]
    class UpdateShopStatisticsJob1 : IJob
    {
        private static ILog UpdateShopStatisticsLogger = LogManager.GetLogger<UpdateShopStatisticsJob1>();
        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            UpdateShopStatisticsLogger.Info($"门店评分统计更新开始");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                UpdateShopStatisticsLogger.Error($"门店评分统计更新异常：{ex}");
            }
            watcher.Stop();
            UpdateShopStatisticsLogger.Info($"门店评分统计更新完成 time={watcher.ElapsedMilliseconds}ms");
        }

        public static void DoJob()
        {
            var shopsModel_TR = DalShopStatistics1.GetShopStatisticsByTR();//统计出来的数据
            var shopsModel_MR = DalShopStatistics1.GetShopStatisticsByMR();//统计出来的数据
            var shopsModel_FW = DalShopStatistics1.GetShopStatisticsByFW();//统计出来的数据
            var shopsModel_PQ = DalShopStatistics1.GetShopStatisticsByPQ();//统计出来的数据
            var shopsModel_BY = DalShopStatistics1.GetShopStatisticsByBY();//统计出来的数据
            Task.WaitAll(shopsModel_TR, shopsModel_MR, shopsModel_FW, shopsModel_PQ, shopsModel_BY);
            var shopsModel = new List<ShopStatisticsModel>();
            shopsModel.AddRange(shopsModel_TR.Result);
            shopsModel.AddRange(shopsModel_MR.Result);
            shopsModel.AddRange(shopsModel_FW.Result);
            shopsModel.AddRange(shopsModel_PQ.Result);
            shopsModel.AddRange(shopsModel_BY.Result);

            var shopsModel_ALL = new List<ShopStatisticsModel>();
            shopsModel.GroupBy(g => g.ShopID).ForEach(f =>
            {
                shopsModel_ALL.Add(new ShopStatisticsModel()
                {
                    ShopID = f.Key,
                    Type = "ALL",
                    InstallQuantity = (f.ToList()?.Sum(e => e.InstallQuantity) ?? 0),
                    CommentR1 = (f.ToList()?.Sum(e => e.CommentR1) ?? 0),
                    CommentR2 = (f.ToList()?.Sum(e => e.CommentR2) ?? 0),
                    CommentR3 = (f.ToList()?.Sum(e => e.CommentR3) ?? 0),
                    CommentR4 = (f.ToList()?.Sum(e => e.CommentR4) ?? 0),
                    CommentTimes = (f.ToList()?.Sum(e => e.CommentTimes) ?? 0),
                    ApplyCommentRate1 = (f.ToList()?.Sum(e => e.ApplyCommentRate1) ?? 0),
                    ApplyCommentRate2 = (f.ToList()?.Sum(e => e.ApplyCommentRate2) ?? 0),
                    ApplyCommentRate3 = (f.ToList()?.Sum(e => e.ApplyCommentRate3) ?? 0),
                    ApplyCommentRate4 = (f.ToList()?.Sum(e => e.ApplyCommentRate4) ?? 0),
                    ApplyCommentTimes = (f.ToList()?.Sum(e => e.ApplyCommentTimes) ?? 0)
                });
            });
            shopsModel.AddRange(shopsModel_ALL);
            int index = 0;
            if (shopsModel != null && shopsModel.Any())
            {
                Parallel.ForEach(shopsModel.GroupBy(g => g.ShopID), new ParallelOptions() { MaxDegreeOfParallelism = 5 }, e =>
                //shopsModel.GroupBy(g => g.ShopID).ForEach(e =>
                {
                    try
                    {
                        var dbShopsModel = DalShopStatistics.GetShopStatisticByShopIds(e.Key);//数据库ShopStatistic表里的数据
                        e.ToList()?.ForEach(f =>
                        {
                            var temp = dbShopsModel.Where(w => w.ShopID == f.ShopID && w.Type == f.Type).FirstOrDefault();
                            if (temp != null)//更新
                            {
                                temp.CommentR1 = f.CommentR1;
                                temp.CommentR2 = f.CommentR2;
                                temp.CommentR3 = f.CommentR3;
                                temp.CommentR4 = f.CommentR4;
                                temp.CommentTimes = f.CommentTimes;
                                temp.InstallQuantity = f.InstallQuantity;
                                temp.ApplyCommentTimes = f.ApplyCommentTimes;
                                temp.ApplyCommentRate1 = f.ApplyCommentRate1 / (f.ApplyCommentTimes <= 0 ? 1 : f.ApplyCommentTimes);
                                temp.ApplyCommentRate2 = f.ApplyCommentRate2 / (f.ApplyCommentTimes <= 0 ? 1 : f.ApplyCommentTimes);
                                temp.ApplyCommentRate3 = f.ApplyCommentRate3 / (f.ApplyCommentTimes <= 0 ? 1 : f.ApplyCommentTimes);
                                temp.ApplyCommentRate4 = f.ApplyCommentRate4 / (f.ApplyCommentTimes <= 0 ? 1 : f.ApplyCommentTimes);
                                if (!DalShopStatistics.UpdateShopStatistic(temp))
                                    UpdateShopStatisticsLogger.Warn($"门店评分统计更新失败：ShopId={f.ShopID};Type={f.Type}");
                            }
                            else//新增
                            {
                                f.CommentR1 = f.CommentR1;
                                f.CommentR2 = f.CommentR2;
                                f.CommentR3 = f.CommentR3;
                                f.CommentR4 = f.CommentR4;
                                f.CommentTimes = f.CommentTimes;
                                f.InstallQuantity = f.InstallQuantity;
                                f.ApplyCommentTimes = f.ApplyCommentTimes;
                                f.ApplyCommentRate1 = f.ApplyCommentRate1 / (f.ApplyCommentTimes <= 0 ? 1 : f.ApplyCommentTimes);
                                f.ApplyCommentRate2 = f.ApplyCommentRate2 / (f.ApplyCommentTimes <= 0 ? 1 : f.ApplyCommentTimes);
                                f.ApplyCommentRate3 = f.ApplyCommentRate3 / (f.ApplyCommentTimes <= 0 ? 1 : f.ApplyCommentTimes);
                                f.ApplyCommentRate4 = f.ApplyCommentRate4 / (f.ApplyCommentTimes <= 0 ? 1 : f.ApplyCommentTimes);
                                if (!DalShopStatistics.InsertShopStatistic(f))
                                    UpdateShopStatisticsLogger.Warn($"门店评分统计插入失败：Data={f}");
                            }
                        });
                        index++;
                        UpdateShopStatisticsLogger.Info($"门店评分统计更新:第{index}个门店ShopId={e.Key}数据更新完成");
                    }
                    catch (Exception ex)
                    {
                        UpdateShopStatisticsLogger.Error($"门店评分统计更新异常:ShopId={e.Key};Exception={ex}");
                    }
                });
                #region 重新统计评论总分
                //try
                //{
                //    var setResult = DalShopStatistics.SetTotalPoints();
                //    UpdateShopStatisticsLogger.Info($"门店评分统计更新:设置{setResult}个门店的总评分");
                //}
                //catch (Exception ex)
                //{
                //    UpdateShopStatisticsLogger.Error("门店评分总分统计异常", ex);
                //}
                #endregion
            }

        }
    }
}
