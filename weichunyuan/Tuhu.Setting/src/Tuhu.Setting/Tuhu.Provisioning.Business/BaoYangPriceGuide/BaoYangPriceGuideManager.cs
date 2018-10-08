using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using QPL.WCF;
using QPL.WebService.TuHu.Core.Model;
using QPL.WebService.TuHu.Service;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.SprayPaintVehicle;
using Tuhu.Provisioning.Business.Tire;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Models;

namespace Tuhu.Provisioning.Business.BaoYangPriceGuide
{
    public class BaoYangPriceGuideManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger(nameof(BaoYangPriceGuideManager));

        public Tuple<bool, List<BaoYangPriceGuideList>> SelectBaoYangPriceGuide(BaoYangPriceSelectModel param, PagerModel pager)
        {
            List<BaoYangPriceGuideList> result = new List<BaoYangPriceGuideList>();
            bool flag;
            try
            {
                flag = true;
                var baseData = DalBaoYangPriceGuide.SelectBaoYangProductInfo(param);
                baseData = IntegrateProductInfo(baseData, param);
                Func<BaoYangPriceGuideList, bool> func = (list) =>
                {
                    bool filter = true;
                    //勾选库存
                    if (param.TotalStock != null)
                    {
                        filter = list.totalstock > 0;
                        if (param.MatchTotalStock.Equals(1))
                        {
                            filter = filter && list.totalstock >= param.TotalStock;
                        }
                        if (param.MatchTotalStock.Equals(-1))
                        {
                            filter = filter && list.totalstock <= param.TotalStock;
                        }
                    }
                    //勾选周转时间
                    if (param.TurnoverDays != null)
                    {
                        filter = filter && list.totalstock > 0 && list.num_month > 0;
                        if (list.totalstock != null && list.num_month != null && list.totalstock > 0 && list.num_month > 0)
                        {
                            var turnoverDays = Math.Ceiling((decimal)list.totalstock / list.num_month.Value * 30);
                            if (param.MatchTurnoverDays.Equals(1))
                            {
                                filter = filter && turnoverDays >= param.TurnoverDays;
                            }
                            if (param.MatchTurnoverDays.Equals(-1))
                            {
                                filter = filter && turnoverDays <= param.TurnoverDays;
                            }
                        }
                    }
                    //勾选毛利额
                    if (param.MaoLiE != null)
                    {
                        filter = filter && list.Price > 0 && list.cost > 0;
                        if (param.MatchMaoLiE.Equals(1))
                        {
                            filter = filter && (list.Price - list.cost ?? 0) >= param.MaoLiE;
                        }
                        if (param.MatchMaoLiE.Equals(-1))
                        {
                            filter = filter && (list.Price - list.cost ?? 0) <= param.MaoLiE;
                        }
                    }
                    //勾选毛利率
                    if (param.MaoLiLv != null)
                    {
                        filter = filter && list.Price > 0;
                        if (list.Price > 0)
                        {
                            if (param.MatchMaoLiLv.Equals(1))
                            {
                                filter = filter && (list.Price - list.cost ?? 0) / list.Price >= param.MaoLiLv / 100;
                            }
                            if (param.MatchMaoLiLv.Equals(-1))
                            {
                                filter = filter && (list.Price - list.cost ?? 0) / list.Price <= param.MaoLiLv / 100;
                            }
                        }
                    }
                    //勾选汽配龙毛利额
                    if (param.QplMaoLiE != null)
                    {
                        filter = filter && list.QPLPrice > 0 && list.cost > 0;
                        if (param.MatchQplMaoLiE.Equals(1))
                        {
                            filter = filter && (list.QPLPrice - list.cost ?? 0) >= param.QplMaoLiE;
                        }
                        if (param.MatchQplMaoLiE.Equals(-1))
                        {
                            filter = filter && (list.QPLPrice - list.cost ?? 0) <= param.QplMaoLiE;
                        }
                    }
                    //勾选汽配龙毛利率
                    if (param.QplMaoLiLv != null)
                    {
                        filter = filter && list.QPLPrice > 0;
                        if (list.QPLPrice > 0)
                        {
                            if (param.MatchQplMaoLiLv.Equals(1))
                            {
                                filter = filter && (list.QPLPrice - list.cost ?? 0) / list.QPLPrice >= param.QplMaoLiLv / 100;
                            }
                            if (param.MatchQplMaoLiLv.Equals(-1))
                            {
                                filter = filter && (list.QPLPrice - list.cost ?? 0) / list.QPLPrice <= param.QplMaoLiLv / 100;
                            }
                        }
                    }
                    //勾选工场店毛利额
                    if (param.ShopMaoLiE != null)
                    {
                        filter = filter && list.Price > 0 && list.QPLPrice > 0;
                        if (param.MatchShopMaoLiE.Equals(1))
                        {
                            filter = filter && (list.Price - list.QPLPrice ?? 0) >= param.ShopMaoLiE;
                        }
                        if (param.MatchShopMaoLiE.Equals(-1))
                        {
                            filter = filter && (list.Price - list.QPLPrice ?? 0) <= param.ShopMaoLiE;
                        }
                    }
                    //勾选工场店毛利率
                    if (param.ShopMaoLiLv != null)
                    {
                        filter = filter && list.QPLPrice > 0 && list.Price > 0;
                        if (list.QPLPrice > 0 && list.Price > 0)
                        {
                            if (param.MatchShopMaoLiLv.Equals(1))
                            {
                                filter = filter && (list.Price - list.QPLPrice ?? 0) / list.Price >= param.ShopMaoLiLv / 100;
                            }
                            if (param.MatchShopMaoLiLv.Equals(-1))
                            {
                                filter = filter && (list.Price - list.QPLPrice ?? 0) / list.Price <= param.ShopMaoLiLv / 100;
                            }
                        }
                    }
                    //勾选实际指导价
                    if (param.PCPricePer != null)
                    {
                        filter = filter && list.Price > 0;
                        if (param.MatchPCPricePer.Equals(1))
                        {
                            filter = filter &&
                                     (list.Price - list.ActualGuidePrice) / list.ActualGuidePrice >=
                                     param.PCPricePer / 100;
                        }
                        if (param.MatchPCPricePer.Equals(-1))
                        {
                            filter = filter &&
                                     (list.Price - list.ActualGuidePrice) / list.ActualGuidePrice <=
                                     param.PCPricePer / 100;
                        }
                    }
                    //勾选价格对比
                    if (!string.IsNullOrWhiteSpace(param.SitePrices) && param.SitePrices.Split(',').Any())
                    {
                        var propertys = list.GetType().GetProperties();
                        var contrast = param.Contrast.Equals(1) ? 1 : 0;
                        decimal price = 0;
                        var sitePrice = propertys.FirstOrDefault(p => p.Name.Equals(param.SitePrice));
                        if (sitePrice?.GetValue(list) != null)
                        {
                            price = (decimal)sitePrice.GetValue(list);
                        }
                        filter = filter && price > 0;
                        if (price > 0)
                        {
                            var siteResult = true;
                            var count = 0;
                            foreach (var site in param.SitePrices.Split(','))
                            {
                                foreach (PropertyInfo t in propertys)
                                {
                                    if (site.Equals(t.Name))
                                    {
                                        if (count == 0)
                                        {
                                            siteResult = PriceJudge(price, (decimal?)t.GetValue(list),
                                                param.Proportion, contrast);
                                            count++;
                                        }
                                        else
                                        {
                                            siteResult = siteResult || PriceJudge(price, (decimal?)t.GetValue(list), param.Proportion, contrast);
                                        }
                                    }
                                }
                            }
                            filter = filter && siteResult;
                        }

                    }
                    return filter;
                };

                var filterData = baseData.Where(func);
                if (filterData.Any())
                {
                    filterData = filterData.OrderByDescending(t => t.num_month);
                    pager.TotalItem = filterData.Count();
                    result = filterData.Skip(pager.PageSize * (pager.CurrentPage - 1)).Take(pager.PageSize).Select(i => i).ToList();
                    AppendShopStock(result);
                    GetDisPlayName(result);
                    var pageNum = result.Count / 100 + (result.Count % 100 > 0 ? 1 : 0);
                    for (var i = 0; i < pageNum; i++)
                    {
                        var currentData = result.Skip(100 * (i - 1)).Take(100).Select(t => t).ToList();
                        AppendQplPrice(currentData);
                        GetFlashSalePrice(currentData);
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "SelectBaoYangPriceGuide");
                flag = false;
            }
            return Tuple.Create(flag, result);
        }

        public bool PriceJudge(decimal price, decimal? otherPrice, double proportion, int judge)
        {
            if (otherPrice != null && otherPrice > 0)
            {
                if (judge.Equals(0))
                {
                    return (double)(otherPrice / (price.Equals(0) ? 1 : price)) <= proportion;
                }
                else
                {
                    return (double)(otherPrice / (price.Equals(0) ? 1 : price)) >= proportion;
                }
            }
            else
            {
                return false;
            }
        }

        public List<BaoYangPriceGuideList> IntegrateProductInfo(List<BaoYangPriceGuideList> baseData, BaoYangPriceSelectModel param)
        {
            try
            {
                var warnLine = DalBaoYangPriceGuide.SelectWarningLine();
                var priceWeight = DalBaoYangPriceGuide.SelectAllWeight();
                var pageNum = baseData.Count / 100 + (baseData.Count % 100 > 0 ? 1 : 0);
                if (param.QplMaoLiE != null || param.QplMaoLiLv != null || param.ShopMaoLiE != null || param.ShopMaoLiLv != null
                    || (!string.IsNullOrWhiteSpace(param.SitePrices) && param.SitePrices.Contains("QPLPrice"))
                    || (!string.IsNullOrWhiteSpace(param.SitePrice) && param.SitePrice.Equals("QPLPrice")))
                {
                    for (var i = 0; i < pageNum; i++)
                    {
                        var currentData = baseData.Skip(100 * (i - 1)).Take(100).Select(t => t).ToList();
                        AppendQplPrice(currentData);
                    }
                }
                foreach (var list in baseData)
                {
                    //获取加权值
                    list.JiaQuan = (int)((priceWeight.FirstOrDefault(p => p.WeightType.Equals("Base"))?
                        .WeightValue ?? 0)
                                          + (priceWeight.FirstOrDefault(
                                              p => p.WeightType.Equals("Brand")
                                              && p.WeightName.Equals(list.Brand)
                                              && p.CategoryName.Equals(list.Category))?
                                              .WeightValue ?? 0)
                                          + (priceWeight.FirstOrDefault(
                                              p => p.WeightType.Equals("Category") && p.WeightName.Equals(list.Category))
                                              ?
                                              .WeightValue ?? 0));
                    var theoryGuidePrice = list.cost == null ? (decimal?)null : list.cost.Value * (100 + list.JiaQuan) / 100;
                    decimal? actualGuidePrice = null;
                    if (list.cost == null || list.cost == 0)
                    {
                        if (list.JDSelfPrice > 0)
                        {
                            actualGuidePrice = list.JDSelfPrice;
                        }
                    }
                    else
                    {
                        if (list.JDSelfPrice == null)
                        {
                            actualGuidePrice = theoryGuidePrice;
                        }
                        else
                        {
                            actualGuidePrice = Math.Min(theoryGuidePrice.Value, list.JDSelfPrice.Value);
                        }
                    }
                    list.TheoryGuidePrice = theoryGuidePrice;
                    list.ActualGuidePrice = actualGuidePrice;
                    //获取预警线
                    var currentWarnLine =
                        warnLine.FirstOrDefault(
                            p => p.MinGuidePrice <= (actualGuidePrice ?? 0) && p.MaxGuidePrice > (actualGuidePrice ?? 0));
                    if (currentWarnLine != null)
                    {
                        list.UpperLimit = currentWarnLine.UpperLimit;
                        list.LowerLimit = currentWarnLine.LowerLimit;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "IntegrateProductInfo");
            }
            return baseData;
        }

        public void AppendShopStock(List<BaoYangPriceGuideList> baseData)
        {
            try
            {
                var workShopList = DalBaoYangPriceGuide.WorkShopIds().AsEnumerable();
                var workShopIds = string.Join(",", workShopList);
                var shopStock =
                DalBaoYangPriceGuide.SelectBaoYangShopStockSum(string.Join(",",
                    baseData.Select(p => p.PID).AsEnumerable()), workShopIds);
                foreach (var list in baseData)
                {
                    if (shopStock.ContainsKey(list.PID))
                    {
                        list.ShopStock = shopStock[list.PID];
                    }
                }
            }
            catch (Exception ex)
            {

                Logger.Log(Level.Error, ex, "AppendShopStock");
            }
        }

        /// <summary>
        /// 查询汽配龙价格
        /// </summary>
        /// <param name="baseData"></param>
        public void AppendQplPrice(List<BaoYangPriceGuideList> baseData)
        {
            try
            {
                var qplPrices = new List<T_ProductModel>();
                var pids = baseData.Select(t => t.PID).ToList();
                var pidData = ServiceHelper<ITuHuTaskService>.Invoke(x => x.GetUserProductPreiceByUseridAndSkuList("970d5eabb66c42d0aeaf4b360c42e451", pids));
                if (pidData.Success && pidData.Result.Result != null)
                {
                    qplPrices = pidData.Result.Result.Data ?? new List<T_ProductModel>();
                }
                foreach (var list in baseData)
                {
                    foreach (var price in qplPrices)
                    {
                        if (price.UserSKU.Equals(list.PID))
                        {
                            list.QPLPrice = price.Price;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "AppendQplPrice");
            }
        }

        /// <summary>
        /// 查询品类
        /// </summary>
        /// <param name="baseData"></param>
        public void GetDisPlayName(List<BaoYangPriceGuideList> baseData)
        {
            try
            {
                using (var client = new ProductClient())
                {
                    var data = client.SelectProductAllCategory();
                    if (data.Success && data.Result.Any())
                    {
                        foreach (var list in baseData)
                        {
                            var item = data.Result.FirstOrDefault(p => p.CategoryName.Equals(list.Category));
                            if (item != null)
                            {
                                list.ThirdType = item.DisplayName;
                                var nodeNoItem = item.NodeNo.Split('.'); 
                                if (nodeNoItem.Length >= 2)
                                {
                                    var parentOid = nodeNoItem[1];
                                    var parentItem =
                                  data.Result.FirstOrDefault(p => p.NodeNo.Split('.').Last().Equals(parentOid));
                                    if (parentItem != null)
                                    {
                                        list.SecondType = parentItem.DisplayName;
                                    }
                                } 
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetDisPlayName");
            }
        }

        /// <summary>
        /// 查询活动价
        /// </summary>
        /// <param name="baseData"></param>
        public void GetFlashSalePrice(List<BaoYangPriceGuideList> baseData)
        {
            try
            {
                var pids = baseData.Select(p => p.PID).AsEnumerable();
                var prices = DalBaoYangPriceGuide.GetFlashSalePriceByPids(string.Join(",", pids));
                foreach (var list in baseData)
                {
                    var pidFlashPrices = prices?.Where(p => p.PID.Equals(list.PID));
                    if (pidFlashPrices != null && pidFlashPrices.Any())
                    {
                        list.FlashSalePrice = pidFlashPrices.Select(p => p.Price).Min();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetFlashSalePrice");
            }
        }

        public List<string> SelectBaoYangBrands(string firstType, string secondType, string thirdType)
        {
            var result = new List<string>();
            try
            {
                result = DalBaoYangPriceGuide.SelectBaoYangBrands(firstType, secondType, thirdType);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "SelectBaoYangBrands");
            }
            return result;
        }

        public Tuple<bool, List<Product_SalespredictData>> SelectStock(string pid)
        {
            var result = new List<Product_SalespredictData>();
            var flag = false;
            try
            {
                result = DalBaoYangPriceGuide.SelectProductSalespredictData(pid);
                flag = true;
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "SelectStock");
            }
            return Tuple.Create(flag, result);
        }

        public IEnumerable<BaoYangWarningLine> SelectWarningLine() => DalBaoYangPriceGuide.SelectWarningLine();

        public int SavePriceWeight(BaoYangPriceWeight model)
        {
            if (model.PKID.GetValueOrDefault(0) > 0)
            {
                if (model.WeightValue.GetValueOrDefault(0) == 0 && model.WeightType != "Base")
                    return DalBaoYangPriceGuide.DeleteGuidePara(model.PKID.Value);
                else
                    return DalBaoYangPriceGuide.UpdateGuidePara(model);
            }
            else
            {
                if (model.WeightValue.GetValueOrDefault(0) != 0)
                    return DalBaoYangPriceGuide.InsertGuidePara(model);
                else
                    return 99;
            }
        }

        public ProductPriceWeight SelectGuideParaByType(string firstType, string secondType, string thirdType)
        {
            var priceWeight = DalBaoYangPriceGuide.SelectAllWeight();
            var baseValue = priceWeight.FirstOrDefault(p => p.WeightType.Equals("Base"));
            ProductPriceWeight result = new ProductPriceWeight
            {
                CategoryWeights = new Dictionary<string, List<CategoryWeight>>(),
                BaseWeight = Tuple.Create(baseValue?.PKID, (int)(baseValue?.WeightValue ?? 0))
            };
            try
            {
                var data = DalBaoYangPriceGuide.SelectGuideParaByType(firstType, secondType, thirdType);
                if (data != null && data.Rows.Count > 0)
                {
                    var dic = data.AsEnumerable()
                        .GroupBy(p => (int)p["ParentOid"])
                        .ToDictionary(t => t.Key, o => o.Select(p => p));
                    foreach (var keyValue in dic)
                    {
                        List<CategoryWeight> categorys = new List<CategoryWeight>();
                        var parentNode = DalBaoYangPriceGuide.SelectProductCategoryByOid(keyValue.Key);
                        foreach (var node in keyValue.Value)
                        {
                            var currentCategory =
                                categorys.FirstOrDefault(p => p.WeightName.Equals(node["Link"].ToString()));
                            var categoryWeight = priceWeight.FirstOrDefault(
                                                 p => p.WeightType.Equals("Category")
                                                 && p.WeightName.Equals(node["Link"].ToString()));
                            if (currentCategory == null)
                            {
                                CategoryWeight category = new CategoryWeight
                                {
                                    Pkid = categoryWeight?.PKID,
                                    WeightName = node["Link"].ToString(),
                                    DisplayName = node["Item"].ToString(),
                                    WeightValue = (int)(categoryWeight?.WeightValue ?? 0),
                                };
                                if (!string.IsNullOrWhiteSpace(node["CP_Brand"].ToString()))
                                {
                                    var brand = priceWeight.FirstOrDefault(
                                        p => p.WeightType.Equals("Brand")
                                             && p.WeightName.Equals(node["CP_Brand"].ToString())
                                             && p.CategoryName.Equals(node["Link"].ToString()));
                                    category.Brands = new List<BrandWeight>
                                    {
                                        new BrandWeight
                                        {
                                            Pkid = brand?.PKID,
                                            WeightName = node["CP_Brand"].ToString(),
                                            WeightValue = (int) (brand?.WeightValue ?? 0)
                                        }
                                    };
                                }
                                categorys.Add(category);
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(node["CP_Brand"].ToString()))
                                {
                                    var brand = priceWeight.FirstOrDefault(
                                                p => p.WeightType.Equals("Brand")
                                                && p.WeightName.Equals(node["CP_Brand"].ToString())
                                                && p.CategoryName.Equals(node["Link"].ToString()));
                                    if (currentCategory.Brands == null)
                                    {
                                        currentCategory.Brands = new List<BrandWeight>();
                                    }
                                    currentCategory.Brands.Add(new BrandWeight
                                    {
                                        Pkid = brand?.PKID,
                                        WeightName = node["CP_Brand"].ToString(),
                                        WeightValue = (int)(brand?.WeightValue ?? 0)
                                    });
                                }
                            }
                        }
                        result.CategoryWeights[parentNode.Item2] = categorys;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "SelectGuideParaByType");
            }
            return result;
        }

        public int UpdateWarningLine(BaoYangWarningLine model) => DalBaoYangPriceGuide.UpdateWarningLine(model);

        public static IEnumerable<ConfigHistory> SelectWeightOprLog(string objectType, string afterValue) => DalBaoYangPriceGuide.SelectWeightOprLog(objectType, afterValue);

        public static IEnumerable<ConfigHistory> SelectWarnOprLog(string objectType, string afterValue) => DalBaoYangPriceGuide.SelectWarnOprLog(objectType, afterValue);

        public static int ApplyUpdatePrice(PriceUpdateAuditModel model) => DalBaoYangPriceGuide.ApplyUpdatePrice(model);

        public static IEnumerable<PriceUpdateAuditModel> SelectNeedAuditBaoYang() => DalBaoYangPriceGuide.SelectNeedAuditBaoYang();

        public static IEnumerable<PriceUpdateAuditModel> SelectAuditLogByPID(string pID, PagerModel pager) => DalBaoYangPriceGuide.SelectAuditLogByPID(pID, pager);

        public static int GotoAudit(bool isAccess, string auther, string pid, decimal? cost, decimal? PurchasePrice, int? totalstock, int? num_week, int? num_month, decimal? guidePrice, decimal nowPrice, string maoliLv, string chaochu, decimal? jdself, decimal? maolie) => DalBaoYangPriceGuide.GotoAudit(isAccess, auther, pid, cost, PurchasePrice, totalstock, num_week, num_month, guidePrice, nowPrice, maoliLv, chaochu, jdself, maolie);

        public static PriceUpdateAuditModel FetchPriceAudit(int pkid) => DalBaoYangPriceGuide.FetchPriceAudit(pkid);

        public static IEnumerable<BaoYangShopStock> SelectBaoYangShopStocks(string pid)
        {
            var baseData = DalBaoYangPriceGuide.SelectBaoYangShopStock(pid);
            var stockSale = DalBaoYangPriceGuide.SelectShopSaleNum(pid);
            var workShopIds = DalBaoYangPriceGuide.WorkShopIds();
            if (baseData.Any())
            {
                baseData = baseData.Where(p => workShopIds.Contains(p.ShopId));
                foreach (var shop in baseData)
                {
                    if (stockSale.Any(p => p.ShopId.Equals(shop.ShopId)))
                    {
                        shop.SaleNum = stockSale.FirstOrDefault(p => p.ShopId.Equals(shop.ShopId)).SaleNum;
                    }
                }
            }
            return baseData;
        }

        public static Dictionary<string, string> SelectProductCategoryByParentOid(int oid) => DalBaoYangPriceGuide.SelectProductCategoryByParentOid(oid);

        public static Tuple<string, string> SelectProductCategoryByOid(int oid) => DalBaoYangPriceGuide.SelectProductCategoryByOid(oid);
    }
}
