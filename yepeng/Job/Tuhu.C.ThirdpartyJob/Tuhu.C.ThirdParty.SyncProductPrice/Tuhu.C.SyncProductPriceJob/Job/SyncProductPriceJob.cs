using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.C.SyncProductPriceJob.DataAccess;
using Tuhu.C.SyncProductPriceJob.PriceManages;
using Tuhu.Service;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Models;

namespace Tuhu.C.SyncProductPriceJob.Job
{
    [DisallowConcurrentExecution]
    public class SyncProductPriceJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SyncProductPriceJob));

        private static readonly QipeilongPriceManage Qipeilong = new QipeilongPriceManage("汽配龙");

        public virtual void Execute(IJobExecutionContext context)
        {
            Logger.Info("开始同步商铺的价格");

            try
            {
                Task.WaitAll(SyncPrice(), SyncThirdPartyPrice());
            }
            catch (Exception ex)
            {
                Logger.ErrorException(ex);
            }
            finally
            {
                //Logger.Info($"本次共删除{ Products.DeleteProduct() }个商品");

                Logger.Info("完成同步商铺的价格");
            }
        }

        public Task SyncPrice() => Task.WhenAll(ShopManageConfiguration.ShopSessionPools.Values
            .Where(s => s.ShopType != ShopTypes.Jingdong && s.ShopType != ShopTypes.Taobao).Select(SyncPrice));

        public Task SyncPrice(Shop shop) =>
            PriceManageHelper.GetPriceMangeInstance(shop)
                .SyncProductPrice();

        public async Task SyncThirdPartyPrice()
        {
            var dt = await Products.QueryThirdPartyProducts();

            if (dt == null || dt.Rows.Count == 0)
                return;

            await Task.WhenAll(dt.AsEnumerable().GroupBy(r => r["ShopCode"].ToString()).ParallelSelect(rows =>
            {
                if ("麦轮胎天猫".Equals(rows.Key) || "麦轮胎官网".Equals(rows.Key))
                {
                    return Task.CompletedTask;
                }
                return Task.Run(() => SyncThirdPartyPrice(rows));
            }, 10));
        }

        private async Task SyncThirdPartyPrice(IEnumerable<DataRow> rows)
        {
            int totalCount = 0, errorCount = 0;
            foreach (var row in rows)
            {
                try
                {
                    var price = await SyncThirdPartyPrice(row["ShopCode"].ToString(), row["Pid"].ToString(), Convert.ToInt64(row["ItemID"]), Convert.ToInt64(row["SkuID"]), row["ItemCode"].ToString());
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
                    Logger.Error($"{row[0]}: {row[1]}同步价格出错，{ex.UnwrapException().Message}");
                }
            }
            Logger.Info($"第三方门店产品价格同步完成，{rows.First()["ShopCode"]}同步成功{totalCount}个产品，失败{errorCount}个");
        }

        public static async Task<decimal?> SyncThirdPartyPrice(string shopCode, string pid, long itemId, long skuId, string itemCode)
        {
            switch (shopCode)
            {
                case "京东自营":
                    var jdPrice = await JingDongPriceManage.GetJingDongPrice(skuId.ToString());
                    if (jdPrice != null)
                    {
                        await Products.UpdatePriceBySkuId(jdPrice, shopCode, skuId);
                        return jdPrice.Price;
                    }
                    break;

                case "麦轮胎天猫":
                case "麦轮胎官网":
                    break;

                case "养车无忧":
                    var yangche51Price = await GetItemPrice(itemCode, client => client.GetYangche51PriceAsync);
                    if (yangche51Price != null)
                    {
                        await Products.UpdatePriceByItemCode(yangche51Price, shopCode, itemCode);
                        return yangche51Price.Price;
                    }
                    break;

                case "汽配龙":
                    await Task.Delay(100);
                    var qplPriceModel = await Qipeilong.GetQplPrice(pid);
                    if (qplPriceModel != null)
                    {
                        await Products.UpdatePriceByPid(new ItemPriceModel(qplPriceModel.ListPrice, qplPriceModel.ProductName), shopCode, pid);
                        return qplPriceModel.ListPrice;
                    }
                    break;

                case "汽车超人零售":
                    var qccrRetailPrice = await GetItemPrice(itemId.ToString(), client => client.GetQccrRetailPriceAsync);
                    if (qccrRetailPrice != null)
                    {
                        await Products.UpdatePriceByItemId(qccrRetailPrice, shopCode, itemId);
                        return qccrRetailPrice.Price;
                    }
                    break;

                case "汽车超人批发":
                    var qccrTradePrice = await GetItemPrice(itemId.ToString(), client => client.GetQccrTradePriceAsync);
                    if (qccrTradePrice != null)
                    {
                        await Products.UpdatePriceByItemId(qccrTradePrice, shopCode, itemId);
                        return qccrTradePrice.Price;
                    }
                    break;

                case "康众官网":
                    var carzonePrice = await GetItemPrice(itemCode, client => client.GetCarzonePriceFromAppAsync);
                    if (carzonePrice != null)
                    {
                        await Products.UpdatePriceByItemCode(carzonePrice, shopCode, itemCode);
                        return carzonePrice.Price;
                    }
                    break;

                default:
                    var taobaoPrice = await TaobaoPriceManage.GetTabaoPrice(itemId.ToString());
                    if (taobaoPrice != null)
                    {
                        await Products.UpdatePriceByItemId(taobaoPrice, shopCode, itemId);
                        return taobaoPrice.Price;
                    }
                    break;
            }

            return null;
        }

        public static async Task<ItemPriceModel> GetItemPrice(string id, Func<IHttpProxyClient, Func<string, Task<OperationResult<ItemPriceModel>>>> func)
        {
            using (var client = new HttpProxyClient())
            {
                var result = await func(client)(id);

                result.ThrowIfException(true);

                return result.Result;
            }
        }
    }
}
