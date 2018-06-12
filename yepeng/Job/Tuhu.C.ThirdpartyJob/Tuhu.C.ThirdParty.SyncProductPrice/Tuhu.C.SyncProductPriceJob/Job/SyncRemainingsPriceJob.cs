using Common.Logging;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.C.SyncProductPriceJob.DataAccess;
using Tuhu.C.SyncProductPriceJob.PriceManages;

namespace Tuhu.C.SyncProductPriceJob.Job
{
    /// <summary>
    /// 补充抓取价格Job
    /// </summary>

    public class SyncRemainingsPriceJob : BaseQuartzJob
    {
        public SyncRemainingsPriceJob() : base(LogManager.GetLogger<SyncRemainingsPriceJob>())
        {
        }

        protected override Task ExecuteAsync(IJobExecutionContext context) => Task.WhenAll(SyncSelfSupportPrice(), SyncThirdPartyPrice());

        /// <summary>
        /// 同步自营门店商品价格
        /// </summary>
        /// <returns></returns>
        private async Task SyncSelfSupportPrice()
        {
            var products = await Products.QueryProductsNeedSync(isThirdParty: false);
            //
            foreach (var productArray in products.GroupBy(_ => _.ShopCode))
            {
                try
                {
                    var shop = ShopManageConfiguration.ShopSessionPools[productArray.Key];

                    if (null == shop)
                    {
                        Logger.Warn($"找不到对应同步价格的门店服务，ShopCode:{productArray.Key}");
                        continue;
                    }

                    await PriceManageHelper.GetPriceMangeInstance(shop).SyncProductsPrice(productArray.ToArray());
                    Logger.Info($"自营门店产品价格同步完成（增量同步），ShopCode:【{productArray.Key}】");
                }
                catch (Exception e)
                {
                    Logger.Error($"自营门店产品价格同步出错（增量同步），ShopCode:【{productArray.Key}】", e);
                }
            }
        }

        /// <summary>
        /// 同步第三方门店商品价格
        /// </summary>
        /// <returns></returns>
        private async Task SyncThirdPartyPrice()
        {
            var thirdPartyProducts = await Products.QueryProductsNeedSync(isThirdParty: true);
            foreach (var group in thirdPartyProducts.GroupBy(_ => _.ShopCode))
            {
                if ("麦轮胎天猫".Equals(group.Key) || "麦轮胎官网".Equals(group.Key))
                {
                    continue;
                }
                int totalCount = 0, errorCount = 0;
                foreach (var row in group)
                {
                    try
                    {
                        var price = await SyncProductPriceJob.SyncThirdPartyPrice(row.ShopCode, row.Pid, row.ItemId, row.SkuId, row.ItemCode);
                        if (price.HasValue && price.Value > 0)
                        {
                            totalCount += 1;
                        }
                        else
                        {
                            errorCount += 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        errorCount += 1;
                        Logger.Error($"{row.ShopCode}: {row.Title}【{row.Pid}】同步价格出错（增量同步），{ex.UnwrapException().Message}");
                    }
                }
                Logger.Info($"第三方门店产品价格同步完成（增量同步），【{group.Key}】同步成功{totalCount}个产品，失败{errorCount}个");
            }
        }
    }
}
