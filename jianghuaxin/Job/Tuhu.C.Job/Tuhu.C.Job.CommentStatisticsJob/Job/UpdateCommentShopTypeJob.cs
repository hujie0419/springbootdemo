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
    /// 门店评论分类统计（每天全量）
    /// </summary>
    [DisallowConcurrentExecution]
    class UpdateCommentShopTypeJob : IJob
    {
        private static ILog Logger = LogManager.GetLogger<UpdateCommentShopTypeJob>();
        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info($"更新门店评论的类型 job 执行开始");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Error($"更新门店评论的类型 更新异常：{ex}");
            }
            Logger.Info($"更新门店评论的类型 更新完成 time={watcher.ElapsedMilliseconds}ms");
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
            Parallel.ForEach(pageIndexList, new ParallelOptions() { MaxDegreeOfParallelism = 5 }, async f =>
            {
                var sw = new Stopwatch();
                sw.Start();
                //var task = BatchUpdate(f, pageSize, start, end);
                AsyncHelper.RunSync(() => BatchUpdate(f, pageSize, start, end));

                Logger.Info($"更新门店评论的类型 第{f}页  页数={pageSize} 更新完成 time={sw.ElapsedMilliseconds}ms");
                sw.Stop();
            });
        }

        private static async Task<bool> BatchUpdate(int pageIndex,int pageSize,DateTime start,DateTime end)
        {
            List<ShopCommentDataModel> comments = DalShopCommentSync.GetShopCommentsPage(pageIndex, pageSize, start.ToString("yyyy-MM-dd"), end.AddDays(1).ToString("yyyy-MM-dd")).ToList();

            #region 批量更新 
            //using (var client = new ShopCommentClient())
            //{
            //    var result = client.BatchGetShopCommentType(comments.Select(p => p.OrderId).ToList());
            //    if (result.Success && result.Result.Any())
            //    {
            //        Logger.Info($"更新门店评论的类型 BatchGetShopCommentType pageIndex ={pageIndex} 有效的 shoptype 数目 ={result.Result.Where(p => !string.IsNullOrWhiteSpace(p.ShopType)).Count()} time={result.ElapsedMilliseconds} ");
            //        foreach (var item in comments)
            //        {
            //            try
            //            {
            //                if (result.Success && result.Result.Any())
            //                {
            //                    string ShopType = result.Result.Where(p => p.OrderId == item.OrderId).FirstOrDefault()?.ShopType;
            //                    if (!string.IsNullOrWhiteSpace(ShopType))
            //                    {
            //                        if (DalShopCommentSync.UpdateShopCommentShopType(item.CommentId ?? 0, ShopType))
            //                        {
            //                            //logger.Info($"更新门店评论的类型 更新  完成 CommentId={item.CommentId}& ShopType={ShopType}");
            //                        }
            //                        else
            //                        {
            //                            Logger.Error($"更新门店评论的类型 UpdateShopCommentShopType 更新失败 CommentId={item.CommentId}& ShopType={ShopType} ");
            //                        }
            //                    }
            //                    else
            //                    {
            //                        //logger.Info($"更新门店评论的类型 BatchGetShopCommentType 更新 失败 CommentId={item.CommentId}& ShopType={ShopType}");
            //                    }
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Logger.Info($"更新门店评论的类型 BatchGetShopCommentType 更新 失败 CommentId={item.CommentId}& ex={ex.Message}");
            //            }
            //        }
            //    }
            //    else
            //    {
            //        Logger.Info($"更新门店评论的类型  更新失败 BatchGetShopCommentType 异常 message={result.ErrorMessage} & count ={result.Result.Count()}");
            //    }
            //}
            #endregion

            List<ShopCommentShopTypeModel> models = new List<ShopCommentShopTypeModel>();
            List<ShopCommentTypeRule> rules = await GetAllShopCommentTypeRules();

            List<TaskOrderModel> results = await GetProductDetail(comments.Select(p => p.OrderId).ToList());
            foreach (var item in results)
            {
                ShopCommentShopTypeModel model = new ShopCommentShopTypeModel() { OrderId = item.OrderId };
                model.ShopType = CalCulateShopCommentType(item, rules);
                models.Add(model);
            }

            foreach (var item in comments)
            {
                try
                {
                    string ShopType = models.Where(p => p.OrderId == item.OrderId).FirstOrDefault()?.ShopType;
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
                catch (Exception ex)
                {
                    Logger.Info($"更新门店评论的类型 BatchGetShopCommentType 更新 失败 CommentId={item.CommentId}& ex={ex.Message}");
                }
            }
            return true;
        }




        #region 门店评论 分类相关

        /// <summary>
        /// 获取订单中产品详情
        /// </summary>
        /// <param name="orderId">订单id</param>
        /// <returns></returns>
        public static async Task<List<TaskOrderModel>> GetProductDetail(List<int> orderIds)
        {
            List<TaskOrderModel> orderInfos = new List<TaskOrderModel> { };
            List<TaskProductModel> productList = new List<TaskProductModel>();
            //批量获取订单 详情
            orderInfos = await BatchFetchOrderList(orderIds, productList);
            //批量获取 产品 类目
            await BatchGetProductCategoryList(orderInfos, productList);
            return orderInfos;
        }

        /// <summary>
        /// 批量获取订单 信息
        /// </summary>
        /// <param name="orderIds"></param>
        /// <param name="productList"></param>
        /// <returns></returns>
        private static async Task<List<TaskOrderModel>> BatchFetchOrderList(List<int> orderIds, List<TaskProductModel> productList)
        {
            List<TaskOrderModel> orderInfos = new List<TaskOrderModel> { };
            try
            {
                using (var client = new OrderApiForCClient())
                {
                    foreach (var orderId in orderIds)
                    {
                        TaskOrderModel orderInfo = new TaskOrderModel() { OrderId = orderId };
                        var result = await client.FetchOrderAndListByOrderIdAsync(orderId);
                        if (!(result.Success && result.Result != null))
                        {
                            result = await client.FetchOrderAndListByOrderIdAsync(orderId, false);
                        }

                        if (result.Success && result.Result != null)
                        {
                            orderInfo.OrderType = result.Result.OrderType;
                            orderInfo.OrderStatus = result.Result.Status;
                            foreach (var item in result.Result.OrderListModel)
                            {
                                var productItem = new TaskProductModel
                                {
                                    PID = item.Pid,
                                    OrderNo = item.OrderNo,
                                    Count = item.Num
                                };
                                orderInfo.Items.Add(productItem);
                            }

                            if (orderInfo.Items.Any())
                            {
                                orderInfos.Add(orderInfo);
                                productList.AddRange(orderInfo.Items);
                            }
                            else
                            {
                                Logger.Warn($"未发现订单详情==>{orderId}");
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Warn(
                    $"BatchFetchOrderList 失败;Error:{ex.Message}", ex);
            }
            return orderInfos;
        }

        /// <summary>
        /// 批量获取 产品的 类目
        /// </summary>
        /// <param name="orderInfos"></param>
        /// <param name="productList"></param>
        private static async Task<bool> BatchGetProductCategoryList(List<TaskOrderModel> orderInfos, List<TaskProductModel> productList)
        {
            try
            {
                using (var client = new ProductClient())
                {
                    // var result = await client.SelectSkuProductListByPidsAsync(productList.Select(g => g.PID).ToList());
                    var resultCategory =
                        await client.GetCategoryInfoByPidsAsync(productList.Select(g => g.PID).Distinct().ToList());
                    if (resultCategory.Success)
                    {
                        foreach (var orderInfo in orderInfos)
                        {
                            foreach (var item in orderInfo.Items)
                            {
                                item.CategoryList = resultCategory.Result.FirstOrDefault(g => g.PID == item.PID)?.CategoryNamePath.Split('/')
                                                              .ToList() ?? new List<string>();
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Warn(
                      $"BatchGetProductCategoryList 失败;Error:{ex.Message}", ex);
                return false;
            }

        }

        /// <summary>
        /// 获取 门店评论 的 分类 规则 【加缓存】
        /// </summary>
        /// <returns></returns>
        public static async Task<List<ShopCommentTypeRule>> GetAllShopCommentTypeRules()
        {
            using (var client = CacheHelper.CreateCacheClient("Comment"))
            {
                var result = await client.GetOrSetAsync("ShopCommentTypeRule",
                    () => DalShopStatistics1.GetShopCommentTypeRule(), TimeSpan.FromMinutes(60));
                if (result.Success)
                    return result.Value;
                else
                {
                    Logger.Warn(
                        $" 缓存redis失败：ShopCommentTypeRule;Error:{result.Message}",
                        result.Exception);
                    return await DalShopStatistics1.GetShopCommentTypeRule();
                }
            }
        }

        /// <summary>
        /// 根据门店的评论信息 计算 评论的分类
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static string CalCulateShopCommentType(TaskOrderModel order, List<ShopCommentTypeRule> rules)
        {
            string type = "BY";//  若没有匹配到类型， 则 默认 为保养
            if (order.OrderType.Trim() == "1普通" || order.OrderType.Trim() == "普通")//订单的类型 是1普通 普通 ，订单二级特殊分类
            {
                //1普通下面的pid 分类
                List<string> productPID_TR = rules.Where(p => p.ShopType == "TR" && !string.IsNullOrWhiteSpace(p.PIDPrefix)).FirstOrDefault().PIDPrefix.Split(',').ToList();
                //1普通下面的 pid所属的类目 进行分类
                /// 保养类型 筛选 条件
                List<string> productCategory_BY = rules.Where(p => p.ShopType == "BY" && !string.IsNullOrWhiteSpace(p.ProductType)).FirstOrDefault().ProductType.Split(',').ToList();
                /// 安装类型 筛选 条件
                List<string> productCategory_FW = rules.Where(p => p.ShopType == "FW" && !string.IsNullOrWhiteSpace(p.ProductType)).FirstOrDefault().ProductType.Split(',').ToList();
                /// 美容类型 筛选 条件
                List<string> productCategory_MR = rules.Where(p => p.ShopType == "MR" && !string.IsNullOrWhiteSpace(p.ProductType)).FirstOrDefault().ProductType.Split(',').ToList();

                List<string> productCategory = new List<string>();

                foreach (var item in order.Items)
                {
                    productCategory.AddRange(item.CategoryList);
                }
                List<string> PIDs = order.Items.Select(p => p.PID).ToList();
                //保养 > 轮胎 > 车品[安装] > 美容
                if (productCategory_BY.Where(p => productCategory.Contains(p)).Count() > 0)
                {
                    type = "BY";
                }
                else if (productPID_TR.Where(p => PIDs.Any(f => f.StartsWith(p))).Count() > 0)
                {
                    type = "TR";
                }
                else if (productCategory_FW.Where(p => productCategory.Contains(p)).Count() > 0)
                {
                    type = "FW";
                }
                else if (productCategory_MR.Where(p => productCategory.Contains(p)).Count() > 0)
                {
                    type = "MR";
                }
            }
            else if (order.OrderType.Trim() == "25VIP客户" || order.OrderType.Trim() == "VIP客户")
            {
                List<string> OrderNos = order.Items.Select(p => p.OrderNo).ToList();
                if (OrderNos.Any(p => p.StartsWith("TR")))
                {
                    type = "TR";
                }
            }
            else //订单的类型 不是1普通 普通，直接根据订单类型 和 分类规则进行 分类
            {
                ShopCommentTypeRule rule = rules.Where(p => p.OrderType == order.OrderType).FirstOrDefault();
                if (rule != null)
                {
                    type = rule.ShopType;
                }
            }
            return type;
        }



        #endregion
    }
}
