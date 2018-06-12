using Common.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.C.SyncProductPriceJob.BusinessFacade;
using Tuhu.C.SyncProductPriceJob.DataAccess;
using Tuhu.C.SyncProductPriceJob.Models;

namespace Tuhu.C.SyncProductPriceJob.PriceManages
{
    public abstract class BasePriceManage :
        IPriceManage
    {
        protected readonly string ShopCode;
        protected readonly ILog Logger;

        protected int ErrorCount;
        protected int TotalCount;

        /// <summary>
        /// BasePriceManage ctor
        /// </summary>
        /// <param name="shopCode">shopCode</param>
        /// <param name="logger">logger</param>
        protected BasePriceManage(string shopCode, ILog logger)
        {
            ShopCode = shopCode;
            Logger = logger;
        }

        protected abstract IReadOnlyCollection<ProductPriceMappingModel> SyncProductMappingsInternal();

        protected abstract Task SyncPriceInternal(IReadOnlyCollection<ProductPriceMappingModel> items);

        public async Task SyncProductsPrice(IReadOnlyCollection<ProductPriceMappingModel> items)
        {
            if (items == null || items.Count == 0)
            {
                Logger.Info($"没有要同步价格的商品【{ShopCode}】");
                return;
            }

            TotalCount = 0;
            ErrorCount = 0;

            Logger.Info($"开始同步产品价格，【{ShopCode}】{items.Count}条映射关系");
            var stopWatch = Stopwatch.StartNew();
            await SyncPriceInternal(items);
            stopWatch.Stop();
            Logger.Info($"产品价格同步完成,{ShopCode}同步成功商品{TotalCount}个，失败{ErrorCount}个，耗时：{stopWatch.Elapsed:c}");
        }

        protected Task SaveProductPrice(IReadOnlyCollection<ProductPriceMappingModel> items, decimal? price = null)
        {
            if (price.HasValue)
            {
                if (price <= 0 || price >= 9999999)
                {
                    return Task.CompletedTask;
                }
                else
                {
                    foreach (var item in items)
                    {
                        item.Price = price.Value;
                    }
                }
            }
            return Task.WhenAll(items.Select(async item =>
            {
                if (item.Price <= 0 || item.Price == 9999999)
                {
                    return;
                }
                if (await ProductSystem.SavePrice(item))
                {
                    Interlocked.Increment(ref TotalCount);
                    Logger.Info(new LogModel { Message = $"商品'{item.ItemId}'价格{price}保存成功", ShopCode = ShopCode });
                }
                else
                {
                    Interlocked.Increment(ref ErrorCount);
                    Logger.Error(new LogModel
                    {
                        Message = $"商品'{item.ItemId}'价格保存失败，原因：商家外部编码没有对应的产品",
                        ShopCode = ShopCode
                    });
                }
            }));
        }

        public async Task SyncProductPrice()
        {
            await SyncProductsPrice((await Products.QueryProductMappings(ShopCode)).ToArray());
        }

        public async Task SyncProductMapping()
        {
            Logger.Info("开始同步产品映射关系");
            var stopwatch = Stopwatch.StartNew();
            var mappings = SyncProductMappingsInternal();
            var result = await Products.SaveProductMappings(mappings);
            stopwatch.Stop();
            if (result > 0)
            {
                Logger.Info($"产品映射关系同步完成，{ShopCode}同步{mappings.Count}条映射关系成功{result}条, 耗时：{stopwatch.Elapsed:c}");
            }
            else
            {
                Logger.Error("同步产品映射关系到数据库失败");
            }
        }
    }
}
