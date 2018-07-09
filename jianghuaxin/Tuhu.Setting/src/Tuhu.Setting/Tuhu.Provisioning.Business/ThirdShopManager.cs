using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Common.Models;
using Tuhu.Nosql;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public static class ThirdShopManager
    {

        public static Dictionary<string, decimal> SelectProductCouponPrice(IEnumerable<string> pids)
        {
            var result = DALThirdShop.SelectProductCouponPrice(pids);
            return result;
        }
        public static Dictionary<string, decimal?> BuildProductMin_maoliValue()
        {
            using (var client = CacheHelper.CreateCacheClient("TireCouponManage"))
            {
                var result = client.GetOrSet("ProductMin_maoliValue", () =>
                {
                    Dictionary<string, decimal?> dict = new Dictionary<string, decimal?>();
                    var items = FetchProductList(new ProductListRequest() { }, new PagerModel() { }, true);
                    if (items != null && items.Any())
                    {
                        foreach (var item in items)
                        {
                            if (item != null)
                            {
                                dict[item.PID] = item.Min_maoliValue;
                            }
                        }
                    }
                    return dict;
                }, TimeSpan.FromMinutes(10));
                return result.Value;
            }
        }
        public static List<ProductPriceModel> ForProductListAsync(ProductListRequest request, PagerModel pager,
            bool isExport = false)
        {
            //var task1 = Task.Run(() => BuildProductMin_maoliValue());
            //var task2 = Task.Run(() => FetchProductList(request, pager, true));
            //await Task.WhenAll(task1, task2);
            var result = FetchProductList(request, pager, true);
            if (!string.IsNullOrEmpty(request.Min_maoliValue) && (request.Min_maoliStatus == 1 || request.Min_maoliStatus == -1))
            {
                if (decimal.TryParse(request.Min_maoliValue, out var temp))
                {
                    result = request.Min_maoliStatus == 1 ?
                        result.Where(x => x.Min_maoliValue.HasValue && x.Min_maoliValue.Value >= temp)?.ToList()
                        : result.Where(x => x.Min_maoliValue.HasValue && x.Min_maoliValue.Value <= temp)?.ToList();
                }
            }

            if (!string.IsNullOrEmpty(request.Website_maoliValue) && (request.Website_maoliStatus == 1 || request.Website_maoliStatus == -1))
            {
                if (decimal.TryParse(request.Website_maoliValue, out var temp))
                {
                    result = request.Website_maoliStatus == 1 ?
                        result.Where(x => x.WebSiteCouponPrice.HasValue && x.WebSiteCouponPrice.Value >= temp)?.ToList()
                        : result.Where(x => x.WebSiteCouponPrice.HasValue && x.WebSiteCouponPrice.Value <= temp)?.ToList();
                }
            }

            return result?.ToList();
            //var result = await Task.WhenAll(Task.Run(() => { return BuildProductMin_maoliValue(); }},Task.);
        }
        public static List<ProductPriceModel> FetchProductList(ProductListRequest request, PagerModel pager,
            bool isExport = false)
        {
            var result = DALThirdShop.FetchProductList(request, pager, isExport);
            Parallel.ForEach(result, item =>
            {

                var lowestCouponPrice = new List<decimal>();
                if (item.Price != null)
                {
                    if (item.CanUseCoupon.HasValue && !item.CanUseCoupon.Value)
                    {
                        item.LowestPrice = item.Price;
                    }
                    else
                    {
                        item.LowestPrice = FetchLowestPrice("自有平台", item.Price ?? 0, request.MaxTireCount)?.Item1 ?? item.Price;
                    }
                    if (item.LowestPrice != null)
                    {
                        lowestCouponPrice.Add(item.LowestPrice.Value);
                    }
                }
                if (item.TBPrice != null)
                {
                    item.TBLowestPrice = FetchLowestPrice("途虎淘宝", item.TBPrice ?? 0, request.MaxTireCount)?.Item1 ?? item.TBPrice;
                    if (item.TBLowestPrice != null)
                    {
                        lowestCouponPrice.Add(item.TBLowestPrice.Value);
                    }
                }
                if (item.TB2Price != null)
                {
                    item.TB2LowestPrice =
                        FetchLowestPrice("途虎淘宝2", item.TB2Price ?? 0, request.MaxTireCount)?.Item1 ?? item.TB2Price;
                    if (item.TB2LowestPrice != null)
                    {
                        lowestCouponPrice.Add(item.TB2LowestPrice.Value);
                    }
                }
                if (item.TM1Price != null)
                {
                    item.TM1LowestPrice =
                        FetchLowestPrice("途虎天猫1", item.TM1Price ?? 0, request.MaxTireCount)?.Item1 ?? item.TM1Price;
                    if (item.TM1LowestPrice != null)
                    {
                        lowestCouponPrice.Add(item.TM1LowestPrice.Value);
                    }
                }
                if (item.TM2Price != null)
                {
                    item.TM2LowestPrice =
                        FetchLowestPrice("途虎天猫2", item.TM2Price ?? 0, request.MaxTireCount)?.Item1 ?? item.TM2Price;
                    if (item.TM2LowestPrice != null)
                    {
                        lowestCouponPrice.Add(item.TM2LowestPrice.Value);
                    }
                }
                if (item.TM3Price != null)
                {
                    item.TM3LowestPrice =
                        FetchLowestPrice("途虎天猫3", item.TM3Price ?? 0, request.MaxTireCount)?.Item1 ?? item.TM3Price;
                    if (item.TM3LowestPrice != null)
                    {
                        lowestCouponPrice.Add(item.TM3LowestPrice.Value);
                    }
                }
                if (item.TM4Price != null)
                {
                    item.TM4LowestPrice =
                        FetchLowestPrice("途虎天猫4", item.TM4Price ?? 0, request.MaxTireCount)?.Item1 ?? item.TM4Price;
                    if (item.TM4LowestPrice != null)
                    {
                        lowestCouponPrice.Add(item.TM4LowestPrice.Value);
                    }
                }
                if (item.JDPrice != null)
                {
                    item.JDLowestPrice = FetchLowestPrice("途虎京东", item.JDPrice ?? 0, request.MaxTireCount)?.Item1 ?? item.JDPrice;
                    if (item.JDLowestPrice != null)
                    {
                        lowestCouponPrice.Add(item.JDLowestPrice.Value);
                    }
                }
                if (item.JDFlagShipPrice != null)
                {
                    item.JDFlagShipLowestPrice =
                        FetchLowestPrice("途虎京东旗舰", item.JDFlagShipPrice ?? 0, request.MaxTireCount)?.Item1 ?? item.JDFlagShipPrice;
                    if (item.JDFlagShipLowestPrice != null)
                    {
                        lowestCouponPrice.Add(item.JDFlagShipLowestPrice.Value);
                    }
                }
                if (lowestCouponPrice.Any() && item.Cost != null)
                {
                    item.Min_maoliValue = lowestCouponPrice.Min() - item.Cost;
                }
            });
            return result.OrderByDescending(x=>x.num_threemonth).ToList();
        }


        public static List<TireCouponResultModel> TireCouponManage(string shopName = null)
        {
            var data = DALThirdShop.TireCouponManage(shopName);
            return data?.GroupBy(g => g.ShopName).Select(g => new TireCouponResultModel
            {
                ShopName = g.Key,
                CouponA = g.Where(t => t.CouponType == 1).ToList(),
                CouponB = g.Where(t => t.CouponType == 2).ToList(),
                CouponC = g.Where(t => t.CouponType == 3).ToList()
            })?.ToList() ?? new List<TireCouponResultModel>();
        }
        public static int AddTireCoupon(TireCouponModel request, string Operator)
        {
            var result = DALThirdShop.AddTireCoupon(request);
            if (result > 0)
            {
                var CouponName = request.Description;
                DALThirdShop.AddTireCouponLog(request.ShopName, request.CouponType, CouponName, request.EndTime, Operator, "添加");
                RemoveCache(request.ShopName);

            }
            return result;
        }


        public static int DeleteTireCoupon(int pkid, string Operator)
        {
            var result = DALThirdShop.DeleteTireCoupon(pkid);
            if (result.PKID > 0)
            {
                var CouponName = $"满{result.QualifiedPrice.ToString("00")}减{result.Reduce.ToString("00")}券";
                DALThirdShop.AddTireCouponLog(result.ShopName, result.CouponType, CouponName, result.EndTime, Operator, "删除");
                RemoveCache(result.ShopName);

            }
            return result.PKID;
        }
        public static List<TireCouponLogModel> FetchCouponLogByShopName(string ShopName)
            => DALThirdShop.FetchCouponLogByShopName(ShopName);
        public static int SetLowestLimitPrice(string PID, decimal? oldprice, decimal LowestLimitPrice, string Operator)
        {
            var result = DALThirdShop.SetLowestLimitPrice(PID, LowestLimitPrice);
            if (result > 0)
            {
                DALThirdShop.AddLowestLimitPriceLog(PID, oldprice, LowestLimitPrice, Operator);
            }
            return result;
        }

        public static List<LowestLimitLogModel> GetLowestLimitLog(string PID)
            => DALThirdShop.GetLowestLimitLog(PID);
        public static Tuple<decimal, List<TireCouponModel>, int> FetchLowestPrice(string ShopName, decimal Price, int MaxCount = 5)
        {
            var data = new TireCouponResultModel();
            using (var client = CacheHelper.CreateCacheClient("TireCouponManage"))
            {
                var cache = client.GetOrSet($"TireCoupon/{ShopName}", () => TireCouponManage(ShopName), TimeSpan.FromHours(3));
                if (cache.Success && cache.Value.Any())
                {
                    data = cache.Value.FirstOrDefault();
                }
                else
                {
                    data = TireCouponManage(ShopName).FirstOrDefault();
                }
            }
            if (data == null)
            {
                return null;
            }
            var EmptyCoupon = new TireCouponModel
            {
                PKID = 0,
                CouponType = 0,
                CouponUseRule = 0,
                QualifiedPrice = 1M,
                Reduce = 0M,
            };
            var CouponList = new List<TireCouponModel>();
            var Result = new Tuple<decimal, List<TireCouponModel>, int>(Price * MaxCount, CouponList, MaxCount);
            for (var Count = 1; Count <= MaxCount; Count++)
            {
                var maxPrice = Count * Price;
                var LowestPrice = Price * Count;
                var dataA = data.CouponA.Where(g => g.StartTime < DateTime.Now && g.EndTime > DateTime.Now && g.QualifiedPrice <= maxPrice).Distinct().ToList();
                dataA.Add(EmptyCoupon);
                foreach (var itemA in dataA)
                {
                    var PriceA = itemA.CouponUseRule == 0 ? (maxPrice - itemA.Reduce) : (maxPrice - itemA.Reduce * Math.Floor(maxPrice / itemA.QualifiedPrice));
                    var dataB = data.CouponB.Where(g => g.StartTime < DateTime.Now && g.EndTime > DateTime.Now && g.QualifiedPrice <= PriceA).Distinct().ToList();
                    dataB.Add(EmptyCoupon);
                    foreach (var itemB in dataB)
                    {
                        var PriceB = itemB.CouponUseRule == 0 ? (PriceA - itemB.Reduce) : (PriceA - itemB.Reduce * Math.Floor(PriceA / itemB.QualifiedPrice));
                        var dataC = data.CouponC.Where(g => g.StartTime < DateTime.Now && g.EndTime > DateTime.Now && g.QualifiedPrice <= PriceB).Distinct().ToList();
                        dataC.Add(EmptyCoupon);
                        foreach (var itemC in dataC)
                        {
                            var finalPrice = itemC == null ? PriceB : (itemC.CouponUseRule == 0 ? (PriceB - itemC.Reduce) : (PriceB - itemC.Reduce * Math.Floor(PriceB / itemC.QualifiedPrice)));
                            if (finalPrice <= LowestPrice)
                            {
                                LowestPrice = finalPrice;
                                CouponList = new List<TireCouponModel>() { itemA, itemB, itemC };
                            }
                        }
                    }
                }
                var stepPrice = Math.Round(LowestPrice / Count, 2);
                if (stepPrice < Result.Item1)
                {
                    Result = new Tuple<decimal, List<TireCouponModel>, int>(stepPrice, CouponList.Where(g => g.CouponType != 0).ToList(), Count);
                }
            }
            return Result;
        }
        private static bool RemoveCache(string ShopName)
        {
            using (var client = CacheHelper.CreateCacheClient("TireCouponManage"))
            {
                var result = client.Remove($"TireCoupon/{ShopName}");
                return result.Success;
            }
        }

        public static int FetchPurchaseRestriction()
            => DALThirdShop.FetchPurchaseRestriction();


    }
}