using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Models;
using Tuhu.C.Job.CompetingProducts.ShopManages;
using Common.Logging;
using Tuhu.C.Job.CompetingProducts.Models;

namespace Tuhu.C.Job.CompetingProducts
{
    [DisallowConcurrentExecution]
    public class CompetingProductsJob : IJob
    {
        public static readonly ILog Logger = LogManager.GetLogger(typeof(CompetingProductsJob));
        public virtual void Execute(IJobExecutionContext context)
        {
            Logger.Info("开始同步竞品的价格");

            try
            {
                Task.WaitAll(SyncThirdPartyPrice());
            }
            finally
            {
                Logger.Info("本次共删除" + DbHelper.ExecuteNonQuery("UPDATE Tuhu_productcatalog..CompetingProductsMonitor WITH (ROWLOCK) SET Is_Deleted=1 WHERE ThirdParty IS NULL AND LastUpdateDateTime < CAST(DATEADD(DAY, -2, GETDATE()) AS DATE)") + "个商品");

                Logger.Info("完成同步竞品的价格");
            }
        }
        /// <summary>
        /// 查询需要更新的价格竞品信息
        /// </summary>
        /// <returns></returns>
        public async Task SyncThirdPartyPrice()
        {
            DataTable dt;
            using (var cmd = new SqlCommand(@"SELECT PKID,ShopCode, Pid, ItemID, SkuID,ItemCode FROM Tuhu_productcatalog..CompetingProductsMonitor WITH(NOLOCK) WHERE LastUpdateDateTime < CAST(GETDATE() AS DATE) AND ThirdParty IS NOT NULL AND Is_Deleted=0"))
            {
                dt = await DbHelper.ExecuteQueryAsync(cmd, _ => _);
            }

            if (dt == null || dt.Rows.Count == 0)
                return;

            await Task.WhenAll(dt.AsEnumerable().GroupBy(r => r["ShopCode"].ToString()).ParallelSelect(rows => Task.Run(() => SyncThirdPartyPrice(rows)), 10));
        }
        /// <summary>
        /// 循环调用【获取价格，执行修改】方法
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public async Task SyncThirdPartyPrice(IEnumerable<DataRow> rows)
        {
            foreach (var row in rows)
            {
                try
                {
                    await SyncThirdPartyPrice(long.Parse(row["PKID"].ToString()),row["ShopCode"].ToString(), row["Pid"].ToString(), Convert.ToInt64(row["ItemID"]), Convert.ToInt64(row["SkuID"]), row["ItemCode"].ToString());
                }
                catch (Exception ex)
                {
                    //Logger.Error($"{row[0]}: {row[1]}", ex);
                    Logger.Info(new LogModel() {Message=ex.Message,RefNo= $"{row[0]}: {row[1]}" });
                }
            }
        }
        /// <summary>
        /// 获取价格，执行修改
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="shopCode"></param>
        /// <param name="pid"></param>
        /// <param name="itemId"></param>
        /// <param name="skuId"></param>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        public static async Task<decimal?> SyncThirdPartyPrice(long pkid,string shopCode, string pid, long itemId, long skuId, string itemCode)
        {
            decimal price = 0;              //第三方价格
            string title = string.Empty;    //第三方标题
            switch (shopCode)
            {
                case "京东自营":
                    var jdPrice = await JingDongShopManage.GetJingDongPrice(skuId.ToString());
                    if (jdPrice != null)
                    {
                        price = jdPrice.Price;
                        title = jdPrice.Title;
                    }
                    break;
                case "麦轮胎官网":
                    var mailuntaiPrice = await GetItemPrice(itemId.ToString(), client => client.GetMailuntaiPriceAsync);
                    if (mailuntaiPrice != null)
                    {
                        price = mailuntaiPrice.Price;
                        title = mailuntaiPrice.Title;
                    }
                    break;
                case "养车无忧":
                    var yangche51Price = await GetItemPrice(itemCode, client => client.GetYangche51PriceAsync);
                    if (yangche51Price != null)
                    {
                        price = yangche51Price.Price;
                        title = yangche51Price.Title;
                    }
                    break;
                case "汽配龙":
                    await Task.Delay(100);
                    var qplPriceModel = await QPLShopManage.GetQPLPrice(pid);
                    if (qplPriceModel.Data!=null)
                    {
                        price = qplPriceModel.Data.ListPrice;
                        title = qplPriceModel.Data.ProductName;
                    }
                    break;
                case "汽车超人零售":
                    var qccrRetailPrice = await GetItemPrice(itemId.ToString(), client => client.GetQccrRetailPriceAsync);
                    if (qccrRetailPrice != null)
                    {
                        price = qccrRetailPrice.Price;
                        title = qccrRetailPrice.Title;
                    }
                    break;
                case "汽车超人批发":
                    var qccrTradePrice = await GetItemPrice(itemId.ToString(), client => client.GetQccrTradePriceAsync);
                    if (qccrTradePrice != null)
                    {
                        price = qccrTradePrice.Price;
                        title = qccrTradePrice.Title;
                    }
                    break;
                case "康众官网":
                    var carzonePrice = await GetItemPrice(itemCode, client => client.GetCarzonePriceFromAppAsync);
                    if (carzonePrice != null)
                    {
                        price = carzonePrice.Price;
                        title = carzonePrice.Title;
                    }
                    break;
                default:
                    var taobaoPrice = await TaobaoShopManage.GetTabaoPrice(itemId.ToString());
                    if (taobaoPrice != null)
                    {
                        price = taobaoPrice.Price;
                        title = taobaoPrice.Title;
                    }
                    break;
            }
            //获取到价格，执行修改
            if (price > 0)
            {
                using (var cmd = new SqlCommand(@"UPDATE	Tuhu_productcatalog..CompetingProductsMonitor WITH (ROWLOCK)
                                                    SET		Price = @Price,
                                                            Title = @Title,
                                                            LastUpdateDateTime = GETDATE()
                                                    WHERE	PKID=@PKID"))
                {
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@PKID", pkid);

                    await DbHelper.ExecuteNonQueryAsync(cmd);
                    return price;
                }
            }
            return null;
        }
        /// <summary>
        /// 获取第三方价格(京东、淘宝、汽配龙除外）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="func"></param>
        /// <returns></returns>
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
