using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Tuhu.C.Job.Initialization.Dal;
using Tuhu.C.Job.Initialization.Model;

namespace Tuhu.C.Job.Initialization.Job
{
    [DisallowConcurrentExecution]
    public class PinTuanProductStockJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<PinTuanProductStockJob>();
        private static readonly int XuStock = 500;
        public void Execute(IJobExecutionContext context)
        {

            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info("拼团产品库存初始化Job开始执行");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Warn("拼团产品库存初始化Job", ex);
            }

            watcher.Stop();
            Logger.Info($"拼团产品库存初始化Job完成, 用时{watcher.ElapsedMilliseconds}毫秒");
        }

        private static void DoJob()
        {
            var products = DalPinTuan.GetPinTuanProduct();
            var index = 0;
            foreach (var product in products)
            {
                Logger.Info($"初始化拼团产品库存，设置第{index}/{products.Count}个商品-->{product}");
                index++;
                var list = DalPinTuan.GetPinTuanProductList(product);
                if (product.StartsWith("XU-"))
                {
                    foreach (var item in list)
                    {
                        var result = DalPinTuan.SetPinTuanStock(item.ProductGroupId, item.PId, XuStock);
                        if (result)
                            Logger.Info($"产品{item.ProductGroupId}/{item.PId}库存设置为{XuStock}");
                    }
                }
                else
                {
                    var stock = DalPinTuan.GetOriginalStockCount(product);
                    // 可以查到库存
                    if (stock.Any())
                    {
                        // 一分团商品
                        var lowPriceProduct = list.Where(g => g.ActivityPrice == 0.01M)?.ToList() ?? new List<PinTuanProductModel>();
                        // 存在一分团商品
                        if (lowPriceProduct.Any())
                        {
                            var stock1 = stock.FirstOrDefault(g => g.Warehouseid == 28790 && g.StockCount > 0) ?? new PinTuanOriginalStockModel();
                            var count = (int)Math.Floor((double)(stock1.StockCount - 50) / (lowPriceProduct.Count));
                            if (count > 1)
                            {
                                foreach (var item in lowPriceProduct)
                                {
                                    var result = DalPinTuan.SetPinTuanStock(item.ProductGroupId, item.PId, count);
                                    if (result)
                                        Logger.Info($"产品{item.ProductGroupId}/{item.PId}库存设置为{count}");
                                }
                            }
                            else
                            {
                                Logger.Error($"产品PID-->{product}，存在一分团产品，义乌仓库存不足");
                            }
                            // 一分团商品                    
                            var normalPriceProduct = list.Where(g => g.ActivityPrice != 0.01M)?.ToList() ?? new List<PinTuanProductModel>();
                            if (normalPriceProduct.Any())
                            {
                                var stock2Count = stock.Where(g => g.Warehouseid != 28790).Sum(g => g.Warehouseid);
                                var count2 = (int)Math.Floor((double)(stock2Count - 50) / (normalPriceProduct.Count()));
                                if (count2 > 0)
                                {
                                    foreach (var item in normalPriceProduct)
                                    {
                                        var result = DalPinTuan.SetPinTuanStock(item.ProductGroupId, item.PId, count2);
                                        if (result)
                                            Logger.Info($"产品{item.ProductGroupId}/{item.PId}库存设置为{count2}");
                                    }
                                }
                                else
                                {
                                    Logger.Error($"产品PID-->{product}，库存不足");
                                }
                            }
                        }
                        // 不存在一分团商品
                        else
                        {
                            var stockCount = stock.Sum(g => g.StockCount);
                            if (stockCount > 50 && list.Any())
                            {
                                var count = (int)(Math.Floor((double)(stockCount - 50) / (list.Count)));
                                if (count > 0)
                                {
                                    foreach (var item in list)
                                    {
                                        var result = DalPinTuan.SetPinTuanStock(item.ProductGroupId, item.PId, count);
                                        if (result)
                                            Logger.Info($"产品{item.ProductGroupId}/{item.PId}库存设置为{count}");
                                    }
                                }
                                else
                                {
                                    Logger.Error($"产品PID-->{product}，库存不足");
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in list)
                        {
                            var result = DalPinTuan.SetPinTuanStock(item.ProductGroupId, item.PId, XuStock);
                            if (result)
                                Logger.Info($"产品{item.ProductGroupId}/{item.PId}库存设置为{XuStock}");
                        }
                    }
                }
            }
        }
    }
}

