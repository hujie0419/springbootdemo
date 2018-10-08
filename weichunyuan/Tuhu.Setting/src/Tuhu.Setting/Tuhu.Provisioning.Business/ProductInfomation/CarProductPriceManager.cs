using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Models;
using Tuhu.Provisioning.DataAccess.DAO.AboutProductPrice;
using Tuhu.Provisioning.DataAccess.Entity.ProductPrice;
using Tuhu.Service;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Request;
using Tuhu.Service.Product.Response;
using Tuhu.Service.Purchase;
using Tuhu.Service.Purchase.Models;
using Tuhu.Service.Purchase.Request;

namespace Tuhu.Provisioning.Business.ProductInfomation
{
    public class CarProductPriceManager
    {
        static readonly string conn_Gungnir = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        static readonly string conn_Gungnir_AlwaysOnRead = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(CarProductPriceManager));

        private static readonly IConnectionManager ConnectionManagerBI = new ConnectionManager(ConfigurationManager.ConnectionStrings["Tuhu_BI"].ConnectionString);
        private static readonly IDBScopeManager dbScopeManagerBI = new DBScopeManager(ConnectionManagerBI);


        /// <summary>
        /// 车品相关商品，价格信息查询
        /// </summary>
        /// <param name="catalogId">类别id</param>
        /// <param name="onSale">是否上架：-1.全部 0.否 1.是 </param>
        /// <param name="isDaifa">是否代发：-1.全部 0.否 1.是 </param>
        /// <param name="stockOut">是否缺货：-1.全部 0.否 1.是 </param>
        /// <param name="hasPintuanPrice">是否有拼团价格：-1.全部 0.否 1.是 </param>
        /// <param name="keyWord">搜索关键词</param>
        /// <param name="keyWordSearchType">关键字查询方式：1.商品名称 2.商品PID</param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static async Task<List<CarProductPriceModel>> GetCarProductMutliPriceByCatalog(string catalogId, int onSale,
            int isDaifa, int stockOut, int hasPintuanPrice, string keyWord, int keyWordSearchType, int page, int pageSize, string ProductName = "")
        {
            var list = new List<CarProductPriceModel>();
            var pids = new List<string>();
            try
            {
                var res = await GetPidsByFilter(catalogId, onSale, isDaifa, stockOut, hasPintuanPrice, keyWord, keyWordSearchType, page, pageSize, ProductName);
                if (!res.Success || res.Result.Pager.Total <= 0)
                    return list;

                pids = res.Result.Source.ToList();


                //IConnectionManager cm = new ConnectionManager(conn_Gungnir);
                using (var conn = new SqlConnection(conn_Gungnir))
                {
                    list = await FullPriceData(pids, conn);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return list;
            }
            return list;
        }

        /// <summary>
        /// 获取记录条数
        /// </summary>
        /// <param name="catalogId"></param>
        /// <param name="onSale"></param>
        /// <param name="isDaifa"></param>
        /// <param name="stockOut"></param>
        /// <param name="hasPintuanPrice"></param>
        /// <param name="keyWord"></param>
        /// <param name="keyWordSearchType"></param>
        /// <returns></returns>
        public static async Task<int> GetCarProductMutliPriceByCatalog_Count(string catalogId, int onSale,
            int isDaifa, int stockOut, int hasPintuanPrice, string keyWord, int keyWordSearchType, string ProductName = "")
        {
            var res = await GetPidsByFilter(catalogId, onSale, isDaifa, stockOut, hasPintuanPrice, keyWord, keyWordSearchType, 1, 10, ProductName);
            if (!res.Success)
                return 0;
            return res.Result.Pager.Total;
        }

        /// <summary>
        /// 查询商品相关信息（库存，活动价格，商品基本信息）
        /// </summary>
        /// <param name="pids"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        static async Task<List<CarProductPriceModel>> FullPriceData(List<string> pids, SqlConnection conn)
        {
            //logger.Info($"FullPriceData 调用接口 start");
            //try
            //{
            var list = await DalCarProductPrice.GetProductBaseInfoByCatalog(conn, pids);
            if (list == null || list.Count < 1)
                return list;

            //活动价格 activityType:1.天天秒杀 4.限时抢购
            var activityPrices = await DalCarProductPrice.GetActivityPricesByPids(conn, pids, DateTime.Now);

            //拼团价格
            var pintuanPirces = await DalCarProductPrice.GetPintuanPricesByPids(conn, pids, DateTime.Now);

            //采购相关价格信息
            //var carigouPrices = await DalCarProductPrice.GetCaigouPriceByPids(conn, pids);
            List<CarPriceManagementResponse> carigouPrices = new List<CarPriceManagementResponse>();
            using (PurchaseClient _OperationResult = new PurchaseClient())
            {
                List<CarPriceManagementRequest> req = pids.Select(p => new CarPriceManagementRequest() { PID = p }).ToList();
                var result = _OperationResult.SelectPurchaseInfoByPID(req);
                if (result.Success && result.Result.Any())
                {
                    carigouPrices = result.Result.ToList();
                }
            }

            //11410.广州保养仓库、 8598.上海保养仓库 、7295.武汉保养仓库、8634.北京保养仓库、缺少一个义乌（还未上线此仓库）
            var wareHouseIds = new List<string> { "8598", "7295", "11410", "8634" };

            //获取相关的库存 
            var stockInfos = dbScopeManagerBI.Execute(mana =>
                       DalCarProductPrice.GetStockQuantityByPids_new(mana, pids, wareHouseIds));
            //var stockInfos = await DalCarProductPrice.GetStockQuantityByPids(conn, pids, wareHouseIds);

            foreach (var item in list)
            {
                #region 字段赋值

                var tempPintuanPrices = pintuanPirces.Where(x => x.PID == item.PID);
                var temp0ActivityPrices = activityPrices?.Where(x => x.PID == item.PID && x.ActiveType == 0);
                var temp1ActivityPrices = activityPrices?.Where(x => x.PID == item.PID && x.ActiveType == 1);
                var temp4ActivityPrices = activityPrices?.Where(x => x.PID == item.PID && x.ActiveType == 4);
                //最小价格  
                item.FullNetActivityPrice = (temp0ActivityPrices.OrderBy(x => x.Price).FirstOrDefault()?.Price) ?? 0;
                item.DaydaySeckillPrice = (temp1ActivityPrices.OrderBy(x => x.Price).FirstOrDefault()?.Price) ?? 0;
                item.FlashSalePrice = (temp4ActivityPrices.OrderBy(x => x.Price).FirstOrDefault()?.Price) ?? 0;
                item.PintuanPrice = (tempPintuanPrices.OrderBy(x => x.Price).FirstOrDefault()?.Price) ?? 0;

                var tempCarigouPrices = carigouPrices?.Where(x => x.PID == item.PID);
                //采购相关价格信息
                item.PurchasePrice = tempCarigouPrices.OrderBy(p => p.PurchasePrice).FirstOrDefault()?.PurchasePrice ?? 0;
                item.ContractPrice = tempCarigouPrices.OrderBy(p => p.ContractPrice).FirstOrDefault()?.ContractPrice ?? 0;
                item.OfferPurchasePrice = tempCarigouPrices.OrderBy(p => p.OfferPurchasePrice).FirstOrDefault()?.OfferPurchasePrice ?? 0;
                item.OfferContractPrice = tempCarigouPrices.OrderBy(p => p.OfferContractPrice).FirstOrDefault()?.OfferContractPrice ?? 0;

                //历史价格
                item.FullNetActivityPriceList = temp0ActivityPrices.ToList();
                item.DaydaySeckillPriceList = temp1ActivityPrices.ToList();
                item.FlashSalePriceList = temp4ActivityPrices.ToList();
                item.PintuanPriceList = tempPintuanPrices.ToList();

                //库存
                var tempStockInfos8598 = stockInfos?.FirstOrDefault(x => x.PID == item.PID && x.WAREHOUSEID == 8598);
                item.SH_AvailableStockQuantity = tempStockInfos8598?.TotalAvailableStockQuantity ?? 0;
                item.SH_ZaituStockQuantity = tempStockInfos8598?.CaigouZaitu ?? 0;

                var tempStockInfos7295 = stockInfos?.FirstOrDefault(x => x.PID == item.PID && x.WAREHOUSEID == 7295);
                item.WH_AvailableStockQuantity = tempStockInfos7295?.TotalAvailableStockQuantity ?? 0;
                item.WH_ZaituStockQuantity = tempStockInfos7295?.CaigouZaitu ?? 0;

                var tempStockInfos11410 = stockInfos?.FirstOrDefault(x => x.PID == item.PID && x.WAREHOUSEID == 11410);
                item.GZ_AvailableStockQuantity = tempStockInfos11410?.TotalAvailableStockQuantity ?? 0;
                item.GZ_ZaituStockQuantity = tempStockInfos11410?.CaigouZaitu ?? 0;

                var tempStockInfos111 = stockInfos?.FirstOrDefault(x => x.PID == item.PID && x.WAREHOUSEID == 111);
                item.YW_AvailableStockQuantity = tempStockInfos111?.TotalAvailableStockQuantity ?? 0;
                item.YW_ZaituStockQuantity = tempStockInfos111?.CaigouZaitu ?? 0;

                item.TotalAvailableStockQuantity = stockInfos?.Sum(x => x.TotalAvailableStockQuantity) ?? 0;
                item.TotalZaituStockQuantity = stockInfos?.Sum(x => x.CaigouZaitu) ?? 0;

                #endregion
            }

            return list;
            //}
            //catch (Exception ex)
            //{
            //    logger.Info($"{nameof(CarProductPriceManager)}.{nameof(FullPriceData)} 调用接口 异常：{ex.Message}", ex);
            //    return null;
            //}

        }


        /// <summary>
        /// 查询商品相关信息（库存，活动价格，商品基本信息）
        /// </summary>
        /// <param name="pids"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        static async Task<List<CarProductPriceModel>> FullStockData(List<string> pids, SqlConnection conn, IDBScopeManager mana)
        {
            var list = await DalCarProductPrice.GetProductBaseInfoByCatalog(conn, pids);
            if (list == null || list.Count < 1)
                return list;

            //活动价格 activityType:1.天天秒杀 4.限时抢购
            var activityPrices = await DalCarProductPrice.GetActivityPricesByPids(conn, pids, DateTime.Now);

            //拼团价格
            var pintuanPirces = await DalCarProductPrice.GetPintuanPricesByPids(conn, pids, DateTime.Now);

            //采购相关价格信息
            //var carigouPrices = await DalCarProductPrice.GetCaigouPriceByPids(conn, pids);
            List<CarPriceManagementResponse> carigouPrices = new List<CarPriceManagementResponse>();
            using (PurchaseClient _OperationResult = new PurchaseClient())
            {
                List<CarPriceManagementRequest> req = pids.Select(p => new CarPriceManagementRequest() { PID = p }).ToList();
                var result = _OperationResult.SelectPurchaseInfoByPID(req);
                if (result.Success && result.Result.Any())
                {
                    carigouPrices = result.Result.ToList();
                }
            }


            //11410.广州保养仓库、 8598.上海保养仓库 、7295.武汉保养仓库、8634.北京保养仓库、缺少一个义乌（还未上线此仓库）
            var wareHouseIds = new List<string> { "8598", "7295", "11410", "8634" };
            var stockInfos = await DalCarProductPrice.GetStockQuantityByPids(conn, pids, wareHouseIds);

            foreach (var item in list)
            {
                #region 字段赋值

                var tempPintuanPrices = pintuanPirces.Where(x => x.PID == item.PID);
                var temp0ActivityPrices = activityPrices?.Where(x => x.PID == item.PID && x.ActiveType == 0);
                var temp1ActivityPrices = activityPrices?.Where(x => x.PID == item.PID && x.ActiveType == 1);
                var temp4ActivityPrices = activityPrices?.Where(x => x.PID == item.PID && x.ActiveType == 4);
                //最小价格  
                item.FullNetActivityPrice = (temp0ActivityPrices.OrderBy(x => x.Price).FirstOrDefault()?.Price) ?? 0;
                item.DaydaySeckillPrice = (temp1ActivityPrices.OrderBy(x => x.Price).FirstOrDefault()?.Price) ?? 0;
                item.FlashSalePrice = (temp4ActivityPrices.OrderBy(x => x.Price).FirstOrDefault()?.Price) ?? 0;
                item.PintuanPrice = (tempPintuanPrices.OrderBy(x => x.Price).FirstOrDefault()?.Price) ?? 0;

                var tempCarigouPrices = carigouPrices?.Where(x => x.PID == item.PID);
                //采购相关价格信息
                item.PurchasePrice = tempCarigouPrices.OrderBy(p => p.PurchasePrice).FirstOrDefault()?.PurchasePrice ?? 0;
                item.ContractPrice = tempCarigouPrices.OrderBy(p => p.ContractPrice).FirstOrDefault()?.ContractPrice ?? 0;
                item.OfferPurchasePrice = tempCarigouPrices.OrderBy(p => p.OfferPurchasePrice).FirstOrDefault()?.OfferPurchasePrice ?? 0;
                item.OfferContractPrice = tempCarigouPrices.OrderBy(p => p.OfferContractPrice).FirstOrDefault()?.OfferContractPrice ?? 0;

                //历史价格
                item.FullNetActivityPriceList = temp0ActivityPrices.ToList();
                item.DaydaySeckillPriceList = temp1ActivityPrices.ToList();
                item.FlashSalePriceList = temp4ActivityPrices.ToList();
                item.PintuanPriceList = tempPintuanPrices.ToList();

                //库存
                var tempStockInfos8598 = stockInfos?.FirstOrDefault(x => x.PID == item.PID && x.WAREHOUSEID == 8598);
                item.SH_AvailableStockQuantity = tempStockInfos8598?.TotalAvailableStockQuantity ?? 0;
                item.SH_ZaituStockQuantity = tempStockInfos8598?.CaigouZaitu ?? 0;

                var tempStockInfos7295 = stockInfos?.FirstOrDefault(x => x.PID == item.PID && x.WAREHOUSEID == 7295);
                item.WH_AvailableStockQuantity = tempStockInfos7295?.TotalAvailableStockQuantity ?? 0;
                item.WH_ZaituStockQuantity = tempStockInfos7295?.CaigouZaitu ?? 0;

                var tempStockInfos11410 = stockInfos?.FirstOrDefault(x => x.PID == item.PID && x.WAREHOUSEID == 11410);
                item.GZ_AvailableStockQuantity = tempStockInfos11410?.TotalAvailableStockQuantity ?? 0;
                item.GZ_ZaituStockQuantity = tempStockInfos11410?.CaigouZaitu ?? 0;

                var tempStockInfos111 = stockInfos?.FirstOrDefault(x => x.PID == item.PID && x.WAREHOUSEID == 111);
                item.YW_AvailableStockQuantity = tempStockInfos111?.TotalAvailableStockQuantity ?? 0;
                item.YW_ZaituStockQuantity = tempStockInfos111?.CaigouZaitu ?? 0;

                item.TotalAvailableStockQuantity = stockInfos?.Sum(x => x.TotalAvailableStockQuantity) ?? 0;
                item.TotalZaituStockQuantity = stockInfos?.Sum(x => x.CaigouZaitu) ?? 0;

                #endregion
            }

            return list;
        }

        /// <summary>
        /// 调用接口筛选pid
        /// </summary>
        /// <param name="catalogId"></param>
        /// <param name="onSale"></param>
        /// <param name="isDaifa"></param>
        /// <param name="stockOut"></param>
        /// <param name="hasPintuanPrice"></param>
        /// <param name="keyWord"></param>
        /// <param name="keyWordSearchType"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        static async Task<OperationResult<PagedModel<string>>> GetPidsByFilter(string catalogId, int onSale,
            int isDaifa, int stockOut, int hasPintuanPrice, string keyWord, int keyWordSearchType, int page, int pageSize, string ProductName = "")
        {
            using (var client = new ShopProductClient())
            {
                var dics = new Dictionary<string, ProductCommonQuery>();

                #region 查询条件 
                if (!string.IsNullOrWhiteSpace(catalogId))
                {
                    dics.Add(nameof(ProductCommonQueryModel.CategoryOid), new ProductCommonQuery
                    {
                        CompareType = CompareType.Equal,
                        Value = catalogId
                    });
                }
                else
                {
                    dics.Add("NodeNo", new ProductCommonQuery
                    {
                        CompareType = CompareType.Equal,
                        Value = 28349
                    });
                }

                if (!string.IsNullOrWhiteSpace(ProductName))
                {
                    dics.Add(nameof(ProductCommonQueryModel.ProductName), new ProductCommonQuery
                    {
                        CompareType = CompareType.Equal,
                        Value = ProductName
                    });
                }

                if (onSale != -1)
                {
                    dics.Add(nameof(ProductCommonQueryModel.IsOnSale), new ProductCommonQuery
                    {
                        CompareType = CompareType.Equal,
                        Value = (onSale == 1)
                    });
                }
                if (isDaifa != -1)
                {
                    dics.Add(nameof(ProductCommonQueryModel.IsDaiFa), new ProductCommonQuery
                    {
                        CompareType = CompareType.Equal,
                        Value = (isDaifa == 1)
                    });
                }
                if (stockOut != -1)
                {
                    dics.Add(nameof(ProductCommonQueryModel.Stockout), new ProductCommonQuery
                    {
                        CompareType = CompareType.Equal,
                        Value = (stockOut == 1)
                    });
                }
                if (hasPintuanPrice != -1)
                {
                    dics.Add(nameof(ProductCommonQueryModel.IsPinTuanProduct), new ProductCommonQuery
                    {
                        CompareType = CompareType.Equal,
                        Value = (hasPintuanPrice == 1)
                    });
                }
                if (!string.IsNullOrWhiteSpace(keyWord)
                    && keyWordSearchType >= 1)
                {
                    if (keyWordSearchType == 1)
                    {
                        dics.Add(nameof(ProductCommonQueryModel.ProductName), new ProductCommonQuery
                        {
                            CompareType = CompareType.Equal,
                            Value = keyWord
                        });
                    }
                    else
                    {
                        dics.Add(nameof(ProductCommonQueryModel.Pid), new ProductCommonQuery
                        {
                            CompareType = CompareType.Equal,
                            Value = keyWord
                        });
                    }
                }
                #endregion

                var inputProp = new ProductCommonQueryRequest
                {
                    AndQuerys = dics,
                    PageIndex = page,
                    PageSize = pageSize
                };

                logger.Info($"{nameof(CarProductPriceManager)}.{nameof(GetPidsByFilter)} 调用接口ProductCommonQueryAsync 入参：{JsonHelper.Serialize(inputProp)}");
                var res = await client.ProductCommonQueryAsync(inputProp);
                logger.Info($"{nameof(CarProductPriceManager)}.{nameof(GetPidsByFilter)} 调用接口ProductCommonQueryAsync 出参：res={JsonHelper.Serialize(res)} 入参：{JsonHelper.Serialize(inputProp)} ");

                res.ThrowIfException();

                return res;
            }
        }

        /// <summary>
        /// 获取 车品  详情
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static async Task<CarProductPriceModel> GetShopProductsByPidsAsync(string pid)
        {
            List<CarProductPriceModel> list = new List<CarProductPriceModel>();
            using (var conn = new SqlConnection(conn_Gungnir))
            {
                list = await FullPriceData(new List<string> { pid }, conn);
                return list.FirstOrDefault();
            }
        }

    }
}
