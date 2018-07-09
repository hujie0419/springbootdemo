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
            Stopwatch watch = Stopwatch.StartNew();
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
            UpdateShopStatisticsLogger.Info($"GetShopStatisticsBy **:执行时间={watch.ElapsedMilliseconds};shopsModel.count={shopsModel.Count()}"); watch.Restart();

            //shopsModel = shopsModel.Where(p => p.ShopID == 38&&p.Type=="TR").ToList();

            //获取 不同 分类下的 默认好评数
            List<ShopDefaultFavourableStatisticsModel> defaultFavourableList = DalShopStatistics1.GetAllProductDefaultFavourableStatistics();
            foreach (var item in shopsModel)
            {
                var defaultFavourableStatistics = defaultFavourableList.Where(p => p.ShopID == item.ShopID && p.Type == item.Type).FirstOrDefault() ?? new ShopDefaultFavourableStatisticsModel();
                item.DefaultFavourableCount = defaultFavourableStatistics.DefaultFavourableCount;
                //重新计算评分
                if (item.CommentTimes > 0)
                {
                    item.Score = (item.CommentR1 + 5 * item.DefaultFavourableCount) / (item.CommentTimes + item.DefaultFavourableCount);
                }
            }
            UpdateShopStatisticsLogger.Info($"GetAllProductDefaultFavourableStatistics **:执行时间={watch.ElapsedMilliseconds};defaultFavourableList.count={shopsModel.Count()}"); watch.Restart();
            var shopsModel_ALL = new List<ShopStatisticsModel>();
            shopsModel.GroupBy(g => g.ShopID).ForEach(f =>
            {
                ShopStatisticsModel _ShopStatisticsModel = new ShopStatisticsModel()
                {
                    ShopID = f.Key,
                    Type = "ALL",
                    InstallQuantity = (f.ToList()?.Sum(e => e.InstallQuantity) ?? 0),
                    CommentR1 = (f.ToList()?.Sum(e => e.CommentR1) ?? 0),
                    CommentR2 = (f.ToList()?.Sum(e => e.CommentR2) ?? 0),
                    CommentR3 = (f.ToList()?.Sum(e => e.CommentR3) ?? 0),
                    CommentR4 = (f.ToList()?.Sum(e => e.CommentR4) ?? 0),
                    CommentTimes = (f.ToList()?.Sum(e => e.CommentTimes) ?? 0),
                    FavourableCount = (f.ToList()?.Sum(e => e.FavourableCount) ?? 0),
                    DefaultFavourableCount = (f.ToList()?.Sum(e => e.DefaultFavourableCount) ?? 0),
                    ApplyCommentRate1 = (f.ToList()?.Sum(e => e.ApplyCommentRate1) ?? 0),
                    ApplyCommentRate2 = (f.ToList()?.Sum(e => e.ApplyCommentRate2) ?? 0),
                    ApplyCommentRate3 = (f.ToList()?.Sum(e => e.ApplyCommentRate3) ?? 0),
                    ApplyCommentRate4 = (f.ToList()?.Sum(e => e.ApplyCommentRate4) ?? 0),
                    ApplyCommentTimes = (f.ToList()?.Sum(e => e.ApplyCommentTimes) ?? 0)
                };
                //重新计算评分
                if (_ShopStatisticsModel.CommentTimes > 0)
                {
                    _ShopStatisticsModel.Score = (_ShopStatisticsModel.CommentR1 + 5 * _ShopStatisticsModel.DefaultFavourableCount) / (_ShopStatisticsModel.CommentTimes + _ShopStatisticsModel.DefaultFavourableCount);
                }
                shopsModel_ALL.Add(_ShopStatisticsModel);
            });
            shopsModel.AddRange(shopsModel_ALL);

            int index = 0;
            if (shopsModel != null && shopsModel.Any())
            {

                Parallel.ForEach(shopsModel.GroupBy(g => g.ShopID), new ParallelOptions() { MaxDegreeOfParallelism = 5 }, e =>
                //shopsModel.GroupBy(g => g.ShopID).ForEach(e =>
                {
                    Stopwatch tempWatch = Stopwatch.StartNew();
                    try
                    {
                        var dbShopsModel = DalShopStatistics1.GetShopStatisticByShopIds(e.Key);//数据库ShopStatistic表里的数据
                        e.ToList()?.ForEach(f =>
                        {

                            var temp = dbShopsModel.Where(w => w.ShopID == f.ShopID && w.Type == f.Type).FirstOrDefault();

                            if (temp != null)//更新
                            {
                                //字段 备份
                                temp.CommentTimesB = f.CommentTimes;
                                temp.CommentR1B = f.CommentR1;
                                temp.CommentR2B = f.CommentR2;
                                temp.CommentR3B = f.CommentR3;
                                temp.CommentR4B = f.CommentR4;

                                //重新赋值
                                temp.CommentTimes = f.CommentTimes + f.DefaultFavourableCount;
                                temp.CommentR1 = f.CommentR1 + f.DefaultFavourableCount * 5;
                                temp.CommentR2 = f.CommentR2 + f.DefaultFavourableCount * 5;
                                temp.CommentR3 = f.CommentR3 + f.DefaultFavourableCount * 5;
                                temp.CommentR4 = f.CommentR4 + f.DefaultFavourableCount * 5;

                                temp.FavourableCount = f.FavourableCount;
                                temp.DefaultFavourableCount = f.DefaultFavourableCount;
                                temp.Score = f.CommentTimesB == 0 ? 0 : f.CommentR1B / f.CommentTimesB;
                                //temp.Score = f.CommentTimes > 0 ? (f. * p.CommentTimes + 5 * p.DefaultFavourableCount) / (p.CommentTimes + p.DefaultFavourableCount) : 0;
                                temp.InstallQuantity = f.InstallQuantity;

                                decimal times = f.ApplyCommentTimes + Convert.ToDecimal(f.DefaultFavourableCount) * 2 / 5;
                                times = times == 0 ? 1 : times;

                                temp.ApplyCommentRate2 = (f.ApplyCommentRate2 + Convert.ToDecimal(f.DefaultFavourableCount) * 5 * 2 / 5) / times;
                                temp.ApplyCommentRate3 = (f.ApplyCommentRate3 + Convert.ToDecimal(f.DefaultFavourableCount) * 5 * 2 / 5) / times;
                                temp.ApplyCommentRate4 = (f.ApplyCommentRate4 + Convert.ToDecimal(f.DefaultFavourableCount) * 5 * 2 / 5) / times;
                                temp.ApplyCommentRate1 = temp.ApplyCommentRate2 * 4/10 + temp.ApplyCommentRate3 * 3 / 10 + temp.ApplyCommentRate2 * 3/ 10;

                                temp.ApplyCommentTimes = f.ApplyCommentTimes + Convert.ToDecimal(f.DefaultFavourableCount) * 2 / 5;

                                if (!DalShopStatistics1.UpdateShopStatistic(temp))
                                    UpdateShopStatisticsLogger.Warn($"门店评分统计更新失败：ShopId={f.ShopID};Type={f.Type}");
                            }
                            else//新增
                            {
                                //字段 备份
                                f.CommentTimesB = f.CommentTimes;
                                f.CommentR1B = f.CommentR1;
                                f.CommentR2B = f.CommentR2;
                                f.CommentR3B = f.CommentR3;
                                f.CommentR4B = f.CommentR4;

                                //重新赋值
                                f.CommentTimes = f.CommentTimesB + f.DefaultFavourableCount;
                                f.CommentR1 = f.CommentR1 + f.DefaultFavourableCount * 5;
                                f.CommentR2 = f.CommentR2 + f.DefaultFavourableCount * 5;
                                f.CommentR3 = f.CommentR3 + f.DefaultFavourableCount * 5;
                                f.CommentR4 = f.CommentR4 + f.DefaultFavourableCount * 5;

                                f.FavourableCount = f.FavourableCount;
                                f.DefaultFavourableCount = f.DefaultFavourableCount;
                                f.Score = f.CommentTimesB == 0 ? 0 : f.CommentR1B / f.CommentTimesB;

                                f.InstallQuantity = f.InstallQuantity;

                                decimal times = f.ApplyCommentTimes + Convert.ToDecimal(f.DefaultFavourableCount) * 2 / 5;
                                times = times == 0 ? 1 : times;

                                f.ApplyCommentRate2 = (f.ApplyCommentRate2  + Convert.ToDecimal(f.DefaultFavourableCount) * 5 * 2 / 5) / times;
                                f.ApplyCommentRate3 = (f.ApplyCommentRate3  + Convert.ToDecimal(f.DefaultFavourableCount) * 5 * 2 / 5) / times;
                                f.ApplyCommentRate4 = (f.ApplyCommentRate4  + Convert.ToDecimal(f.DefaultFavourableCount) * 5 * 2 / 5) / times;
                                f.ApplyCommentRate1 = f.ApplyCommentRate2 * 4 / 10 + f.ApplyCommentRate3 * 3 / 10 + f.ApplyCommentRate2 * 3 / 10;

                                f.ApplyCommentTimes = f.ApplyCommentTimes + Convert.ToDecimal(f.DefaultFavourableCount) * 2 / 5;

                                if (!DalShopStatistics1.InsertShopStatistic(f))
                                    UpdateShopStatisticsLogger.Warn($"门店评分统计插入失败：Data={f}");
                            }
                        });
                        index++;
                        UpdateShopStatisticsLogger.Info($"门店评分统计更新:第{index}个门店ShopId={e.Key}数据更新完成，时间={tempWatch.ElapsedMilliseconds}");
                        tempWatch.Stop();
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

            watch.Stop();
        }
    }
}
