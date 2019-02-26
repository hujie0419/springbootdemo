using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.PackageActivity;
using Tuhu.Provisioning.Business.Promotion;
using Tuhu.Provisioning.DataAccess.Entity.PackageActivity;
using Tuhu.Provisioning.Models;

namespace Tuhu.Provisioning.Controllers
{
    public class PackageActivityController : Controller
    {
        [PowerManage]
        public ActionResult PackageActivity()
        {
            return View();
        }

        public JsonResult GetPackageActivityConfig(string activityId, string activityName, DateTime? startTime, DateTime? endTime, int pageIndex, int pageSiz = 20)
        {
            PackageActivityManager manager = new PackageActivityManager();

            var result = manager.SelectPackageActivity(activityId, activityName, startTime, endTime, pageIndex, pageSiz);

            if (result != null && result.Any())
            {
                return Json(new { status = "success", data = result, count=result.FirstOrDefault().Total }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PackageActivityConfig(Guid? activityId)
        {
            PackageActivityViewModel result = new PackageActivityViewModel();
            PackageActivityManager manager = new PackageActivityManager();
            if (activityId != null && activityId != Guid.Empty)
            {
                var data = manager.SelectPackageActivityConfig(activityId ?? Guid.Empty);
                result = ConvertPackageActivity(data);
            }
            ViewData["PackageTypes"] = manager.GetBaoYangPacekageType();
            return View(result);
        }

        [PowerManage]
        public JsonResult UpsertPaceageActivityConfig(string jsonData)
        {
            PackageActivityManager manager = new PackageActivityManager();
            var result = false;
            string msg = "数据格式有误";
            var flagTwo = true;
            PackageActivityConfig oldData = null;

            if (!string.IsNullOrEmpty(jsonData))
            {
                var model = JsonConvert.DeserializeObject<PackageActivityConfig>(jsonData);
                if (model != null)
                {
                    if (model.IsTieredPricing)
                    {
                        var tierList = Enum.GetNames(typeof(TierType));
                        model.PromotionPrice = 0;
                        model.PriceConfig = model.PriceConfig?.Join(
                            tierList, x => x.TierType, y => y, (x, y) => new PackageActivityPriceConfig
                            {
                                TierType = y,
                                Price = x.Price,
                            }).GroupBy(x => x.TierType)
                            .Select(g => g.FirstOrDefault()).ToList() ?? new List<PackageActivityPriceConfig>();
                        if (model.PriceConfig.Count != tierList.Length ||
                            model.PriceConfig.Select(x => x.TierType).Distinct().Count() != tierList.Length)
                        {
                            return Json(new { status = false, msg = "分层定价填写有误" });
                        }
                    }
                    else
                    {
                        model.PriceConfig = new List<PackageActivityPriceConfig>();
                    }


                    if (model.ActivityId != Guid.Empty)
                    {
                        oldData = manager.SelectPackageActivityConfig(model.ActivityId);
                    }
                    var flag = ValidRoundConfig(model.RoundConfig, oldData);
                    if (flag.Item1)
                    {
                        model = ConvertOther(model);
                        if (model.PackageTypes.Contains("xby") && model.PackageTypes.Contains("dby"))
                        {
                            msg = "小保养服务和大保养服务不能同时勾选";
                        }
                        else
                        {
                            flagTwo = validQuantityPeruser(model, oldData);
                            if (flagTwo)
                            {
                                result = manager.UpsertPaceageActivityConfig(model, HttpContext.User.Identity.Name);
                            }
                            else
                            {
                                msg = "活动进行中时,每人每个项目限购数量只能调整的更大，而不能调小";
                            }
                        }
                    }
                    else
                    {
                        msg = flag.Item2;
                    }
                }
            }

            return Json(new { status = result, msg = msg }, JsonRequestBehavior.AllowGet);
        }

        [PowerManage]
        public JsonResult DeletePackageActivityConfig(Guid activityId)
        {
            PackageActivityManager manager = new PackageActivityManager();
            var result = false;

            result = manager.DeletePackageActivityConfig(activityId, HttpContext.User.Identity.Name);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectOperationLog(string objectId)
        {
            PackageActivityManager manager = new PackageActivityManager();
            var result = manager.SelectOperationLog(objectId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RefreshPackageBaoYangCache(Guid activityId)
        {
            PackageActivityManager manager = new PackageActivityManager();
            var result = manager.RefreshPackageBaoYangCache(activityId);
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        #region 私有转换


        private PackageActivityConfig ConvertOther(PackageActivityConfig model)
        {
            model.MaxSaleQuantity = model.RoundConfig.Select(x => x.LimitedQuantity).Sum();
            model.StartTime = model.RoundConfig.Select(x => x.StartTime).Min();
            model.EndTime = model.RoundConfig.Select(x => x.EndTime).Max();
            if (model.ShopConfig != null && model.ShopConfig.Any())
            {
                model.ShopConfig = model.ShopConfig.Distinct(new ListDistinctForShopConfig()).ToList();
            }
            if (model.ProductConfig != null && model.ProductConfig.Any())
            {
                model.ProductConfig = model.ProductConfig.Distinct(new ListDistinctForProductConfig()).ToList();
            }
            return model;
        }



        private bool validQuantityPeruser(PackageActivityConfig newData, PackageActivityConfig oldData)
        {
            var result = false;

            if (oldData == null || oldData.ActivityId == Guid.Empty)
            {
                result = true;
            }
            else
            {
                if (newData.StartTime < DateTime.Now)
                {
                    if (newData.ItemQuantityPerUser >= oldData.ItemQuantityPerUser)
                    {
                        result = true;
                    }
                }
                else
                {
                    result = true;
                }
            }

            return result;
        }

        private Tuple<bool,string> ValidRoundConfig(List<PackageActivityRoundConfig> data, PackageActivityConfig oldData)
        {
            var result = true;
            string msg = "";
            DateTime startTime = DateTime.MinValue;
            if (data != null && data.Any())
            {
                foreach (var item in data)
                {
                    if (item.EndTime < item.StartTime)
                    {
                        result = false;
                    }
                    else
                    {
                        if (startTime != DateTime.MinValue && startTime != item.StartTime)
                        {
                            result = false;
                        }
                    }
                    if (!result)
                    {
                        msg = "场次配置时间验证未通过";
                        break;
                    }
                    startTime = item.EndTime.AddSeconds(+1);
                }
                if (result && oldData != null && oldData.ActivityId != Guid.Empty)
                {
                    foreach (var item in oldData.RoundConfig)
                    {
                        var newData = data.Where(x => String.Equals(x.PKID, item.PKID) && x.PKID > 0).FirstOrDefault();
                        if (newData != null && newData.PKID > 0)
                        {
                            if (newData.StartTime < DateTime.Now)
                            {
                                if (newData.LimitedQuantity < item.LimitedQuantity)
                                {
                                    result = false;
                                }
                            }
                        }
                        if (!result)
                        {
                            msg = "正在进行中的场次限购数量不允许调低";
                            break;
                        }
                    }
                }
            }

            return Tuple.Create(result,msg);
        }
        private PackageActivityViewModel ConvertPackageActivity(PackageActivityConfig data)
        {
            PackageActivityViewModel result = new PackageActivityViewModel();
            if (data != null)
            {
                result = new PackageActivityViewModel
                {
                    ActivityId = data.ActivityId,
                    ActivityName = data.ActivityName,
                    StartTime = data.StartTime,
                    EndTime = data.EndTime,
                    MaxSaleQuantity = data.MaxSaleQuantity,
                    PackageTypes = data.PackageTypes,
                    IsChargeInstallFee = data.IsChargeInstallFee,
                    IsUsePromotion = data.IsUsePromotion,
                    InstallOrPayType = data.InstallOrPayType,
                    ItemQuantityPerUser = data.ItemQuantityPerUser,
                    PromotionPrice = data.PromotionPrice,
                    TipTextColor = data.TipTextColor,
                    ButtonBackgroundColor = data.ButtonBackgroundColor,
                    ButtonTextColor = data.ButtonTextColor,
                    BackgroundImg = data.BackgroundImg ,
                    OngoingButtonText = data.OngoingButtonText
                };
                result.ShopConfig = ConvertPackageActivityShopConfig(data.ShopConfig);
                result.RoundConfig = ConvertPackageActivityRoundConfig(data.RoundConfig);
                result.ProductConfigDetails = ConvertPackageActivityProductConfig(data.ProductConfig);
                result.VehicleConfig = data.VehicleConfig.Select(x => new VehicleConfig
                {
                    Vehicle = x.Vehicle,
                    AvgPrice = x.AvgPrice,
                    Brand = x.Brand,
                    VehicleID = x.VehicleID,
                }).ToList();
                var array = Enum.GetNames(typeof(TierType));
                result.IsTieredPricing = data.PriceConfig != null && data.PriceConfig.Any();
                result.PriceConfig = array.ToDictionary(tier => tier, tier =>
                {
                    var priceItem = data.PriceConfig?.FirstOrDefault(x => x.TierType == tier);
                    return (priceItem != null && priceItem.Price != null && priceItem.Price > 0) ?
                                priceItem.Price.Value.ToString("f2") : "/";
                });
            }
            return result;
        }

        private List<PackageActivityShop> ConvertPackageActivityShopConfig(List<PackageActivityShopConfig> data)
        {
            List<PackageActivityShop> result = new List<PackageActivityShop>();
            if (data != null && data.Any())
            {
                result = data.Select(x => new PackageActivityShop()
                {
                    ShopId = x.ShopId,
                    ShopName = x.ShopName,
                    ShopType = x.ShopType
                }).ToList();
            }
            return result;
        }

        private List<PackageActivityRound> ConvertPackageActivityRoundConfig(List<PackageActivityRoundConfig> data)
        {
            List<PackageActivityRound> result = new List<PackageActivityRound>();
            if (data != null && data.Any())
            {
                result = data.Select(x => new PackageActivityRound()
                {
                    PKID = x.PKID,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    LimitedQuantity = x.LimitedQuantity
                }).ToList();
            }
            return result;
        }

        private ProductDescriptionConfig ConvertPackageActivityProductConfig(List<PackageActivityProductConfig> data)
        {
            ProductDescriptionConfig result = new ProductDescriptionConfig()
            {
                Categories = new List<string>(),
                Pids = new List<string>(),
                BrandDetails = new List<ProductDescriptionBrand>()
            };
            if (data != null && data.Any())
            {
                foreach (var item in data)
                {
                    if (item.IsIngore && !result.IsIngore)
                    {
                        result.IsIngore = item.IsIngore;
                    }

                    if (!string.IsNullOrEmpty(item.CategoryName) && string.IsNullOrEmpty(item.Brand) && !result.Categories.Contains(item.CategoryName))
                    {
                        result.Categories.Add(item.CategoryName);
                    }

                    if (!string.IsNullOrEmpty(item.PID) && !result.Pids.Contains(item.PID))
                    {
                        result.Pids.Add(item.PID);
                    }

                    if (!string.IsNullOrEmpty(item.Brand))
                    {
                        if (result.BrandDetails.Where(o => o.BrandCategoryName.Equals(item.CategoryName)).Count() > 0)
                        {
                            foreach (var configItem in result.BrandDetails)
                            {
                                if (String.Equals(configItem.BrandCategoryName, item.CategoryName))
                                {
                                    if (String.Equals(configItem.BrandCategoryName, item.CategoryName) && !configItem.Brands.Contains(item.Brand))
                                    {
                                        configItem.Brands.Add(item.Brand);
                                    }
                                }

                            }
                        }
                        else
                        {
                            ProductDescriptionBrand brandItem = new ProductDescriptionBrand()
                            {
                                Brands = new List<string>()
                            };
                            brandItem.BrandCategoryName = item.CategoryName;
                            brandItem.Brands.Add(item.Brand);
                            result.BrandDetails.Add(brandItem);
                        }
                    }
                }
            }

            return result;
        }

        private class ListDistinctForShopConfig : IEqualityComparer<PackageActivityShopConfig>
        {
            public bool Equals(PackageActivityShopConfig x, PackageActivityShopConfig y)
            {
                if ((x.ShopId == y.ShopId && x.ShopId > 0 && y.ShopId > 0))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public int GetHashCode(PackageActivityShopConfig obj)
            {
                return 0;
            }
        }

        private class ListDistinctForProductConfig : IEqualityComparer<PackageActivityProductConfig>
        {
            public bool Equals(PackageActivityProductConfig x, PackageActivityProductConfig y)
            {
                if ((x.PID == y.PID && !string.IsNullOrEmpty(x.PID) && !string.IsNullOrEmpty(y.PID)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public int GetHashCode(PackageActivityProductConfig obj)
            {
                return 0;
            }
        }
        #endregion

        #region Vehicle

        public ActionResult GetAllVehicleBrandCategory()
        {
            var manager = new PackageActivityManager();
            var result = manager.GetAllVehicleBrandCategory();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllVehicleBrand()
        {
            var manager = new PackageActivityManager();
            var result = manager.GetAllVehicleBrand();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVehicles(List<string> series, List<string> brands, IEnumerable<string> excludeVids,
            double minPrice, double maxPrice, int index = 1, int size = 20)
        {
            if (minPrice > 0 && maxPrice <= minPrice)
            {
                return Json(new { status = false, msg = "价格输入不正确" }, JsonRequestBehavior.AllowGet);
            }
            series = series?.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            excludeVids = excludeVids?.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            brands = brands?.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            var manager = new PackageActivityManager();
            var result = manager.GetVehicles(series, brands, excludeVids, minPrice, maxPrice, index, size);
            return Json(new { status = true, data = result.Item2, total = result.Item1 }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}