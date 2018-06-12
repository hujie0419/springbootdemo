using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Common.Logging;
using Tuhu.C.Job.UserShareJob.Dal;
using System.Threading;
using Newtonsoft.Json;

namespace Tuhu.C.Job.UserShareJob.Job
{
    [DisallowConcurrentExecution]
    public class UpdateCouponPriceJob : IJob
    {
        private static ILog UpdateCouponPriceLogger = LogManager.GetLogger<UpdateCouponPriceJob>();
        public void Execute(IJobExecutionContext context)
        {
            //            券后价 = min（京东自营，汽车超人零售）-10元；
            //若系统中无京东自营和汽车超人零售价格，则券后价 = 官网价格 - 10元；

            //券后价生效逻辑：
            //若成本价 + 20元 =< 券后价 < 官网价格，则自动生效；
            //若券后价 >= 官网价格 或 券后价<成本价+20元，则进入审批流程；
            //            1.如果没有京东自营和汽车超人价格，券后价为空；
            //2.官网价格低于京东自营（包含99999和为空）和汽车超人时，券后价为空；
            UpdateCouponPriceLogger.Info($"券后价每天更新job:{DateTime.Now}开始");
            try
            {
                if (DateTime.Now < new DateTime(2017, 6, 30, 18, 0, 0))
                {
                    DalCouponPrice.CleanSystemApproveProcess();
                    DalCouponPrice.CleanProductCache();
                }
                var couponPriceList = DalCouponPrice.GetCouponPrice().Distinct()?.ToList();
                if (couponPriceList == null || !couponPriceList.Any())
                {
                    UpdateCouponPriceLogger.Info($"券后价每天更新job:没有获取到数据");
                    return;
                }
                foreach (var item in couponPriceList)
                {
                    if (item.JDPrice != null || item.QCCRPrice != null)
                    {
                        if (item.JDPrice == null)
                        {
                            item.CouponPrice = item.GWPrice > item.QCCRPrice ? ((item.QCCRPrice ?? 0) - 10) : item.CouponPrice;
                        }
                        else if (item.QCCRPrice == null)
                        {
                            item.CouponPrice = item.GWPrice > item.JDPrice ? ((item.JDPrice ?? 0) - 10) : item.CouponPrice;
                        }
                        else
                        {
                            var minPrice = (item.JDPrice ?? 0) < (item.QCCRPrice ?? 0) ? (item.JDPrice ?? 0) : (item.QCCRPrice ?? 0);
                            if (item.GWPrice > minPrice)
                            {
                                item.CouponPrice = minPrice - 10;
                            }
                        }
                    }
                    //else
                    //{
                    //    item.CouponPrice = (item.GWPrice ?? 0) - 10;
                    //}
                    if (item.CouponPrice <= 0)
                        continue;

                    var isExistCouponPrice = DalCouponPrice.IsExistCouponPrice(item.PID);
                    if (item.CouponPrice >= item.CostPrice + 20 && item.CouponPrice < (item.GWPrice ?? 0))//自动生效
                    {
                        if (!DalCouponPrice.UpdateOrInsertCouponPrice(item.PID, item.CouponPrice, isExistCouponPrice.Item1))
                        {
                            UpdateCouponPriceLogger.Warn($"更新券后价异常：{JsonConvert.SerializeObject(item)}自动生效异常");
                            continue;
                        }

                        if (!DalCouponPrice.InsertCouponPriceHistory(isExistCouponPrice.Item2, item.CouponPrice, item.PID))
                            UpdateCouponPriceLogger.Warn($"更新券后价自动生效后记录异常：{JsonConvert.SerializeObject(item)}自动生效记录异常");
                        UpdateCouponPriceLogger.Info($"更新券后价成功：{JsonConvert.SerializeObject(item)}自动生效");
                    }
                    else//进入审批流程
                    {
                        if (DalCouponPrice.IsExistSystemApproveProcess(item.PID))//
                        {
                            if (!DalCouponPrice.UpdateApproveProcess(item.PID, item.Brand, item.CostPrice, item.Stock, item.WeekSaleCount, item.MonthSaleCount, null, item.JDPrice, item.QCCRPrice, item.DisplayName, isExistCouponPrice.Item2, item.CouponPrice, item.GWPrice))
                                UpdateCouponPriceLogger.Warn($"更新券后价异常：{JsonConvert.SerializeObject(item)}进入审核流程更新失败");
                            else
                            {
                                UpdateCouponPriceLogger.Warn($"更新券后价成功：{JsonConvert.SerializeObject(item)}进入审核流程更新成功");
                            }
                        }
                        else
                        {
                            if (!DalCouponPrice.InsertApproveProcess(item.PID, item.Brand, item.CostPrice, item.Stock, item.WeekSaleCount, item.MonthSaleCount, null, item.JDPrice, item.QCCRPrice, item.DisplayName, isExistCouponPrice.Item2, item.CouponPrice, item.GWPrice))
                                UpdateCouponPriceLogger.Warn($"更新券后价异常：{JsonConvert.SerializeObject(item)}进入审核流程插入失败");
                            else
                            {
                                UpdateCouponPriceLogger.Warn($"更新券后价异常：{JsonConvert.SerializeObject(item)}进入审核流程插入成功");
                            }
                        }
                    }
                }
                if (DateTime.Now < new DateTime(2017, 6, 30, 18, 0, 0))
                {
                    Thread.Sleep(5000);
                    DalCouponPrice.CleanProductCache(false);
                }
            }
            catch (Exception ex)
            {
                UpdateCouponPriceLogger.Error($"券后价更新异常：{ex}");
            }
            UpdateCouponPriceLogger.Info($"券后价每天更新job:{DateTime.Now}结束");
        }
    }
}
