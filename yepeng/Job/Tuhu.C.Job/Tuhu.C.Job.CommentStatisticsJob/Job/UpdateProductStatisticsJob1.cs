using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Common.Logging;
using Tuhu.C.Job.CommentStatisticsJob.Dal;
using System.Diagnostics;
using Tuhu.C.Job.CommentStatisticsJob.Model;

namespace Tuhu.C.Job.CommentStatisticsJob.Job
{
    /// <summary>
    /// 产品信息统计（包含评论）
    /// </summary>
    [DisallowConcurrentExecution]
    public class UpdateProductStatisticsJob1 : IJob
    {
        private static ILog UpdateProductStatisticsLogger = LogManager.GetLogger<UpdateProductStatisticsJob1>();
        public void Execute(IJobExecutionContext context)
        {
            Stopwatch watch = Stopwatch.StartNew();
            UpdateProductStatisticsLogger.Info($"产品评分统计更新开始");
            int index = 1;
            try
            {
                var allProductComments = DalProductStatistics1.GetProductStatisticsByPage();
                UpdateProductStatisticsLogger.Info($"GetProductStatisticsByPage:执行时间={watch.ElapsedMilliseconds};allProductComments.count={allProductComments.Count()}"); watch.Restart();
                //除去 temp的产品
                allProductComments = allProductComments.Where(p => p.ProductID.ToUpper().IndexOf("TEMP-")!=0).ToList();
                UpdateProductStatisticsLogger.Info($"除去 temp的产品:执行时间={watch.ElapsedMilliseconds};allProductComments.count={allProductComments.Count()}"); watch.Restart();
                //获取所有 产品的 默认好评数
                List<ProductCommentStatusModel> ProductCommentStatusList = DalProductStatistics1.GetAllProductDefaultFavourableStatistics().ToList();
                UpdateProductStatisticsLogger.Info($"GetAllProductDefaultFavourableStatistics:执行时间={watch.ElapsedMilliseconds};ProductCommentStatusList={ProductCommentStatusList.Count()}"); watch.Restart();
                List<ProductDefaultFavourableStatisticsModel> defaultFavourableStatisticsList = new List<ProductDefaultFavourableStatisticsModel>();
                foreach (var item in ProductCommentStatusList)
                {
                    ProductDefaultFavourableStatisticsModel model = new ProductDefaultFavourableStatisticsModel();
                    if (item.ProductId.Split('|').Count() < 2)
                    {
                        UpdateProductStatisticsLogger.Info($"ProductCommentStatus ProductId 是familyid:ProductId={item.ProductId};"); 
                        model.ProductFamilyId = item.ProductId.Split('|')[0];
                        model.VariantID = "0";
                        model.DefaultFavourableCount = item.DefaultFavourableCount;
                    }
                    else
                    {
                        model.ProductFamilyId = item.ProductId.Split('|')[0];
                        model.VariantID = item.ProductId.Split('|')[1];
                        model.DefaultFavourableCount = item.DefaultFavourableCount;
                    }
                    defaultFavourableStatisticsList.Add(model);
                }


                List<ProductDefaultFavourableStatisticsModel> defaultFavourableStatisticsList_Comment = new List<ProductDefaultFavourableStatisticsModel>();
                //所有的产品 的 默认好评数相同
                foreach (var item in defaultFavourableStatisticsList)
                {
                    ProductDefaultFavourableStatisticsModel model = new ProductDefaultFavourableStatisticsModel();
                    model.ProductFamilyId = item.ProductFamilyId;
                    model.VariantID = item.VariantID;
                    model.DefaultFavourableCount = defaultFavourableStatisticsList.Where(p => p.ProductFamilyId == item.ProductFamilyId).Sum(p => p.DefaultFavourableCount);
                    defaultFavourableStatisticsList_Comment.Add(model);
                }



                UpdateProductStatisticsLogger.Info($"GetAllProductDefaultFavourableStatistics:执行时间={watch.ElapsedMilliseconds};defaultFavourableStatisticsList={defaultFavourableStatisticsList.Count()}"); watch.Restart();


                if (allProductComments != null && allProductComments.Any())
                {
                    Parallel.ForEach(allProductComments.GroupBy(g => g.ProductID), new ParallelOptions() { MaxDegreeOfParallelism = 5 }, f =>
                    {
                        Stopwatch tempWatch = Stopwatch.StartNew();
                        try
                        {
                            var mergeResult = DalProductStatistics1.MergeIntoProductStatistics(f.ToList(), defaultFavourableStatisticsList_Comment);
                            if (!mergeResult)
                            {
                                UpdateProductStatisticsLogger.Error($"MergeIntoProductStatistics-->产品评分统计更新失败：ProductId={f.Key}:执行时间={tempWatch.ElapsedMilliseconds}");
                            }
                        }
                        catch (Exception ex)
                        {
                            UpdateProductStatisticsLogger.Error($"产品评分统计更新异常:ProductId={f.Key};{ex}:执行时间={tempWatch.ElapsedMilliseconds}");
                        }
                        index++;
                        UpdateProductStatisticsLogger.Info($"产品评分统计更新:第{index}个商品ProductId={f.Key}数据更新完成:执行时间={tempWatch.ElapsedMilliseconds}");
                    });

                };

                //allProductComments.GroupBy(g => g.ProductID).ForEach(f =>
                //{
                //    Stopwatch tempWatch = Stopwatch.StartNew();
                //    try
                //    {
                //        var mergeResult = DalProductStatistics1.MergeIntoProductStatistics(f.ToList(), defaultFavourableStatisticsList);
                //        if (!mergeResult)
                //        {
                //            UpdateProductStatisticsLogger.Error($"MergeIntoProductStatistics-->产品评分统计更新失败：ProductId={f.Key}:执行时间={tempWatch.ElapsedMilliseconds}");
                //        }
                //        //var dbProductStatistics = DalProductStatistics.GetProductStatisticsByProductId(f.Key);
                //        //dbProductStatistics.Split(100).ForEach(oneList => {
                //        //    var mergeResult = DalProductStatistics.MergeIntoProductStatistics(oneList?.ToList());
                //        //    if (!mergeResult)
                //        //    {
                //        //        UpdateProductStatisticsLogger.Error($"MergeIntoProductStatistics-->产品评分统计更新失败：ProductId={f.Key}");
                //        //    }
                //        //});
                //    }
                //    catch (Exception ex)
                //    {
                //        UpdateProductStatisticsLogger.Error($"产品评分统计更新异常:ProductId={f.Key};{ex}:执行时间={tempWatch.ElapsedMilliseconds}");
                //    }

                //    index++;
                //    UpdateProductStatisticsLogger.Info($"产品评分统计更新:第{index}个商品ProductId={f.Key}数据更新完成:执行时间={tempWatch.ElapsedMilliseconds}");
                //});
                if (!DalProductStatistics1.UpdatetCarPAR_CatalogProducts())//更新CarPAR_CatalogProducts中的OrderQuantity,SalesQuantity数据
                {
                    UpdateProductStatisticsLogger.Error($"产品评分统计更新:OrderQuantity,SalesQuantity更新失败");
                }
                else
                {
                    UpdateProductStatisticsLogger.Info($"UpdatetCarPAR_CatalogProducts 更新:OrderQuantity,SalesQuantity更新成功：执行时间={watch.ElapsedMilliseconds}");watch.Restart();
                }
            }
            catch (Exception ex)
            {
                UpdateProductStatisticsLogger.Error($"产品评分统计更新异常:{ex}");
            }

            UpdateProductStatisticsLogger.Info($"产品评分统计更新结束 time = {watch.ElapsedMilliseconds}");
            watch.Stop();
        }
    }
}
