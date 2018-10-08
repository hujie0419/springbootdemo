using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.BaoYang;
using Tuhu.Provisioning.Business.Tire;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.BaoYang.Config;
using Tuhu.Service.Shop;
using Tuhu.Service.Shop.Models.Region;

namespace Tuhu.Provisioning.Controllers
{
    public class BaoYangActivitySettingController : Controller
    {

        private static readonly Lazy<BaoYangActivitySettingManager> lazyBaoYangActivitySettingManager = new Lazy<BaoYangActivitySettingManager>();

        private BaoYangActivitySettingManager BaoYangActivitySettingManager
        {
            get { return lazyBaoYangActivitySettingManager.Value; }
        }

        public ActionResult Index(int pageIndex = 1, int pageSize = 10)
        {
            int count = 0;
            string strSql = string.Empty;
            var modelList = BaoYangActivitySettingManager.GetUpkeepActivitySetting(strSql, pageSize, pageIndex, out count);
            var lists = new List<BaoYangActivitySetting>();
            foreach (IGrouping<int, BaoYangActivitySetting> item in modelList)
            {
                BaoYangActivitySetting model = new BaoYangActivitySetting();
                model.ActivityName = item.FirstOrDefault().ActivityName;
                model.ActivityNum = item.FirstOrDefault().ActivityNum;
                model.ActivityStatus = item.FirstOrDefault().ActivityStatus;
                model.ButtonChar = item.FirstOrDefault().ButtonChar;
                model.CheckStatus = item.FirstOrDefault().CheckStatus;
                model.CouponId = item.FirstOrDefault().CouponId;
                model.GetRuleGUID = item.FirstOrDefault().GetRuleGUID;
                model.CreateTime = item.FirstOrDefault().CreateTime;
                model.UpdateTime = item.FirstOrDefault().UpdateTime;
                model.Id = item.FirstOrDefault().Id;
                model.LayerImage = item.FirstOrDefault().LayerImage;
                model.LayerImage2 = item.FirstOrDefault().LayerImage2;
                model.LayerStatus = item.FirstOrDefault().LayerStatus;
                model.ActivityImage = item.FirstOrDefault().ActivityImage;
                model.StoreAuthentication = item.FirstOrDefault().StoreAuthentication;
                model.StoreAuthenticationName = item.FirstOrDefault().StoreAuthenticationName;

                foreach (var brands in item.Where(x => !string.IsNullOrWhiteSpace(x.RelevanceBrands)))
                {
                    model.RelevanceBrands += brands.RelevanceBrands + ";";
                }
                foreach (var series in item.Where(x => !string.IsNullOrWhiteSpace(x.RelevanceSeries)))
                {
                    model.RelevanceSeries += series.RelevanceSeries + ";";
                }
                foreach (var products in item.Where(x => !string.IsNullOrWhiteSpace(x.RelevanceProducts)))
                {
                    model.RelevanceProducts += products.RelevanceProducts + ";";
                }

                lists.Add(model);
            }

            var list = new OutData<List<BaoYangActivitySetting>, int>(lists.Skip(pageSize * (pageIndex - 1)).Take(pageSize).OrderByDescending(x => x.UpdateTime).ToList(), lists.Count());
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = list.OutValue
            };
            return View(new ListModel<BaoYangActivitySetting>(list.ReturnValue, pager));
        }

        public ActionResult Edit(int id = 0)
        {
            if (id == 0)
            {
                return View(new BaoYangActivitySetting());
            }
            else
            {
                return View(BaoYangActivitySettingManager.GetUpkeepActivitySettingById(id));
            }

        }

        [HttpPost]
        public ActionResult Edit(BaoYangActivitySetting model)
        {
            string js = "<script>alert(\"保存失败 \");location='/BaoYangActivitySetting/Index';</script>";
            if (model.Id != 0)
            {
                if (BaoYangActivitySettingManager.UpdateUpkeepActivitySetting(model))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return Content(js);
                }
            }
            else
            {
                if (BaoYangActivitySettingManager.InsertUpkeepActivitySetting(model))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return Content(js);
                }
            }
        }

        [HttpPost]
        public ActionResult Delete(int id, string activityNum)
        {
            return Json(BaoYangActivitySettingManager.DeleteUpkeepActivitySetting(id, activityNum));
        }


        public JsonResult BaoYangTypeConfig()
        {
            return Json(BaoYangActivitySettingManager.GetServiceTypeSetting(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Brand(string catalogNames, string brands)
        {
            ViewData["brands"] = brands;
            return View(BaoYangActivitySettingManager.GetRelevanceBrand(catalogNames));
        }

        public ActionResult Serie(string catalogNames, string Series)
        {
            //List<BaoYangActivityRelevance> list = BaoYangActivitySettingManager.GetRelevanceBrand(catalogNames);
            //string brands = "";
            //foreach (var item in list)
            //{
            //    brands += item.Brand + ",";
            //}
            ViewData["Series"] = Series;

            List<BaoYangActivityRelevance> result = BaoYangActivitySettingManager.GetRelevanceSeries(catalogNames);
            return View(result);
        }

        public ActionResult StoreAuthentication()
        {
            return View();
        }
        public ActionResult ShopAuthentication()
        {
            return View();
        }
        public ActionResult RelateList()
        {
            return View();
        }

        public ActionResult ServiceType(int id, string ActivityNum)
        {
            var manager = new BaoYangActivitySettingManager();
            var list = manager.GetBaoYangPackageDescription();
            ViewData["BaoYangActivityId"] = id;
            ViewData["ActivityNum"] = ActivityNum;
            return View(list);
        }

        //public ActionResult Item()
        //{
        //    XmlDocument doc = new XmlDocument();
        //    string url = Server.MapPath("/Models/BaoYangType.xml");
        //    doc.Load(url);    //加载Xml文件  
        //    XmlElement rootElem = doc.DocumentElement;   //获取根节点  
        //    XmlNodeList personNodes = rootElem.GetElementsByTagName("BaoYangPackage"); //获取BaoYangPackage子节点集合  

        //    List<BaoYangPackage> list = new List<BaoYangPackage>();
        //    foreach (XmlNode node in personNodes)
        //    {
        //        BaoYangPackage model = new BaoYangPackage();
        //        string type = ((XmlElement)node).GetAttribute("type");   //获取name属性值  
        //        string items = ((XmlElement)node).GetAttribute("items");
        //        string name = ((XmlElement)node).GetAttribute("zh");
        //        model.Type = type;
        //        model.Items = items;
        //        model.Name = name;
        //        list.Add(model);
        //    }
        //    return View(list);
        //}

        /// <summary>
        /// 保养活动关联配置
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ActivityNum"></param>
        /// <returns></returns>
        public ActionResult ItemList()
        {
            return View();
        }

        /// <summary>
        /// 根据活动Id获取保养活动关联配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetSettingItemByActivityId(int id)
        {
            if (id < 1)
            {
                return Json(new { Status = false, Msg = "未知的查询对象" }, JsonRequestBehavior.AllowGet);
            }
            var result = BaoYangActivitySettingManager.GetBaoYangActivitySettingItemByBaoYangActivityId(id);
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EditItem(string str,string ActivityNum)
        {
            List<BaoYangActivitySettingItem> list = JsonConvert.DeserializeObject<List<BaoYangActivitySettingItem>>(str);

            foreach (var item in list)
            {
                BaoYangActivitySettingManager.InsertBaoYangActivitySettingItem(item, ActivityNum);
            }

            return Json(1);

        }

        public JsonResult DeleteBaoYangActivitySettingItem(int id,int baoYangActivityId)
        {
            return Json(BaoYangActivitySettingManager.DeleteBaoYangActivitySettingItem(id, baoYangActivityId));
        }

        public ActionResult AddItem()
        {
            return View();
        }

        /// <summary>
        /// 新增保养活动关联配置
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetActivitySettingItemForAdd(string type)
        {
            List<BaoYangType> list = new List<BaoYangType>();
            var manager = new BaoYangActivitySettingManager();
            var package = manager.GetBaoYangPackageDescriptionByType(type);
            foreach (var item in package?.Items?.ToList() ?? new List<BaoYangTypeDescription>())
            {
                BaoYangType m = new BaoYangType
                {
                    Package = new BaoYangPackage()
                    {
                        Type = package.PackageType,
                        Name = package.ZhName,
                        Items = string.Join(",", package.Items.Select(v => v.BaoYangType))
                    },
                    Name = item.ZhName,
                    CatalogName = item.CatalogName,
                    Type = item.BaoYangType
                };
                if (!string.Equals(item.CatalogName, "Oil"))
                {
                    m.BrandList = BaoYangActivitySettingManager.GetRelevanceBrand(item.CatalogName)?.Select(s=>s.Brand)?.ToList();
                    m.SerieList = BaoYangActivitySettingManager.GetRelevanceSeries(item.CatalogName)?.Select(s => s.Series)?.ToList();
                }
                list.Add(m);
            }
            return Json(new { Status = list != null && list.Any(), Data = list });
        }

        /// <summary>
        /// 获取保养关联项目配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetBaoYangActivitySettingItemById(int id = 0)
        {
            if (id < 1)
            {
                return Json(new { Status = false, Msg = "未知的对象" }, JsonRequestBehavior.AllowGet);
            }
            var result = BaoYangActivitySettingManager.GetBaoYangActivitySettingItemById(id);
            List<BaoYangActivityRelevance> brandList = null;
            List<BaoYangActivityRelevance> seriesList = null;
            Dictionary<string, List<string>> brandWithSeries = null;
            if (result != null && !string.IsNullOrEmpty(result.ServicePackagesType) &&
                !string.IsNullOrEmpty(result.ServiceCatalog))
            {
                switch (result.ServicePackagesType)
                {
                    case "ffdyn":
                        brandList = BaoYangActivitySettingManager.GetRelevanceBrand("Dynamo");
                        seriesList = BaoYangActivitySettingManager.GetRelevanceSeries("Dynamo");
                        break;
                    case "dyn":
                        brandList = BaoYangActivitySettingManager.GetRelevanceBrand("Dynamo");
                        seriesList = BaoYangActivitySettingManager.GetRelevanceSeries("Dynamo");
                        break;
                    case "ys":
                        brandList = BaoYangActivitySettingManager.GetRelevanceBrand("Wiper");
                        seriesList = BaoYangActivitySettingManager.GetRelevanceSeries("Wiper");
                        break;
                    case "scp":
                        brandList = BaoYangActivitySettingManager.GetRelevanceBrand("BrakePad");
                        seriesList = BaoYangActivitySettingManager.GetRelevanceSeries("BrakePad");
                        break;
                    case "scpan":
                        brandList = BaoYangActivitySettingManager.GetRelevanceBrand("BrakeDisc");
                        seriesList = BaoYangActivitySettingManager.GetRelevanceSeries("BrakeDisc");
                        break;
                    default:
                        if (string.Equals(result.ServiceCatalog, "Oil", StringComparison.OrdinalIgnoreCase))
                        {
                             brandWithSeries= new BaoYangManager().GetOilBrandAndSeries();
                        }
                        else
                        {
                            brandList = BaoYangActivitySettingManager.GetRelevanceBrand(result.ServiceCatalog);
                            seriesList = BaoYangActivitySettingManager.GetRelevanceSeries(result.ServiceCatalog);
                        }
                        break;
                }
            }
            return Json(new
            {
                Status = result != null,
                Data = result,
                BrandList = brandList?.Select(s => s.Brand)?.Distinct()?.ToList(),
                SeriesList = seriesList?.Select(s => s.Series)?.Distinct()?.ToList(),
                BrandWithSeries= brandWithSeries
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更新保养活动关联配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateBaoYangActivitySettingItem(BaoYangActivitySettingItem model)
        {
            return Json(BaoYangActivitySettingManager.UpdateBaoYangActivitySettingItem(model));
        }


        #region 保养活动分车型配置
        public ActionResult BaoYangActivityVehicle()
        {
            return View();
        }

        public ActionResult GetAllVehicleDepartment()
        {
            var result = RecommendManager.GetVehicleDepartment();
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllOilLevel()
        {
            var result = null as List<string>;
            var oilLevelModels = BaoYangActivitySettingManager.GetAllOilLevel();

            if(oilLevelModels!=null)
            {
                result=oilLevelModels.Select(s => s.OilType).Distinct().ToList();
            }
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllOilViscosity()
        {
            var result = BaoYangActivitySettingManager.GetAllOilViscosity();
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllVehicleBrand()
        {
            var result = BaoYangActivitySettingManager.GetAllVehicleBrand();
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SelectBaoYangActivityVehicle(BaoYangActivityVehicleSearchModel model, int pageIndex=1, int pageSize=20)
        {
            if(model==null)
            {
                return Json(new { Status = false, Msg = "未知的查询对象" }, JsonRequestBehavior.AllowGet);
            }
            if (model.BrandCategories == null || !model.BrandCategories.Any() || model.Brands == null || !model.Brands.Any())
            {
                return Json(new { Status = false, Msg = "请选择车型信息" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(model.OilViscosity))
            {
                return Json(new { Status = false, Msg = "请选择机油等级" }, JsonRequestBehavior.AllowGet);
            }
            if(model.MaxPrice<model.MinPrice)
            {
                return Json(new { Status = false, Msg = "最高价不得小于最低价" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangActivitySettingManager();
            if (model.OilLevels != null && model.OilLevels.Any())
            {
                var oilLevels = manager.GetAllOilLevel();
                if (oilLevels == null || !oilLevels.Any())
                {
                    return Json(new { Status = false, Msg = "无法获取机油等级信息" }, JsonRequestBehavior.AllowGet);
                }
                model.OilLevels = oilLevels.Where(s => model.OilLevels.Contains(s.OilType)).Select(v => v.OilLevel).Distinct().ToList();
            }
            var result= manager.SelectBaoYangActivityVehicle(model, pageIndex, pageSize);
            var totalPage = (result.Item2 % pageSize == 0) ? ((int)result.Item2 / pageSize) : ((int)result.Item2 / pageSize + 1);
            return Json(new { Status = result.Item1 != null, Data = result.Item1, TotalCount = result.Item2, TotalPage = totalPage },
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBaoYangActivityNameByActivityId(string activityId)
        {
            var result = string.Empty;
            if (!string.IsNullOrWhiteSpace(activityId))
            {
                var manager = new BaoYangActivitySettingManager();
                result = manager.GetBaoYangActivityNameByActivityId(activityId);
            }
            return Json(new { Status = !string.IsNullOrWhiteSpace(result), Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MultiAddOrEditBaoYangActivityVehicle(List<string> vehicleIds, string activityId)
        {
            if (vehicleIds == null || !vehicleIds.Any())
            {
                return Json(new { Status = false, Msg = "请选择要编辑的对象" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(activityId))
            {
                return Json(new { Status = false, Msg = "活动不能为空" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangActivitySettingManager();
            var user = User.Identity.Name;
            var result = manager.MultiAddOrEditBaoYangActivityVehicle(vehicleIds, activityId, user);
            return Json(new { Status = result, Msg = $"批量编辑{(result.Item1 ? "成功" : "失败")},清除缓存{(result.Item2 ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MultDeleteBaoYangActivityVehicle(List<string> vehicleIds)
        {
            if (vehicleIds == null || !vehicleIds.Any())
            {
                return Json(new { Status = false, Msg = "未知的删除对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangActivitySettingManager();
            var user = User.Identity.Name;
            var result = manager.MultDeleteBaoYangActivityVehicle(vehicleIds, user);
            return Json(new { Status = result, Msg = $"批量删除{(result.Item1 ? "成功" : "失败")},清除缓存{(result.Item2 ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 保养活动分地区配置
        public ActionResult BaoYangActivityRegion()
        {
            return View();
        }

        /// <summary>
        /// 获取所有城市数据(到市一级)
        /// </summary>
        /// <returns>Json格式</returns>
        public ActionResult GetAllRegion()
        {
            var manager = new BaoYangActivitySettingManager();
            var regions = manager.GetAllRegion();
            return Json(new { Status = regions.Any(), Data = regions }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SelectBaoYangActivityRegion(List<int> regionIds, int pageIndex = 1, int pageSize = 20)
        {
            if (regionIds == null || !regionIds.Any())
            {
                return Json(new { Status = false, Msg = "请选择城市" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangActivitySettingManager();
            var result = manager.SelectBaoYangActivityRegion(regionIds, pageIndex, pageSize);
            var totalCount = regionIds.Count;
            var totalPage = (totalCount % pageSize == 0) ? ((int)totalCount / pageSize) : ((int)totalCount / pageSize + 1);
            return Json(new { Status = result != null, Data = result, TotalCount = totalCount, TotalPage = totalPage },
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult MultiAddOrEditBaoYangActivityRegion(List<int> regionIds, string activityId)
        {
            if (regionIds == null || !regionIds.Any())
            {
                return Json(new { Status = false, Msg = "请选择要编辑的对象" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(activityId))
            {
                return Json(new { Status = false, Msg = "活动不能为空" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangActivitySettingManager();
            var user = User.Identity.Name;
            var result = manager.MultiAddOrEditBaoYangActivityRegion(regionIds, activityId, user);
            return Json(new { Status = result, Msg = $"批量编辑{(result.Item1 ? "成功" : "失败")},清除缓存{(result.Item2 ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MultDeleteBaoYangActivityRegion(List<int> regionIds)
        {
            if (regionIds == null || !regionIds.Any())
            {
                return Json(new { Status = false, Msg = "未知的删除对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangActivitySettingManager();
            var user = User.Identity.Name;
            var result = manager.MultDeleteBaoYangActivityRegion(regionIds, user);
            return Json(new { Status = result, Msg = $"批量删除{(result.Item1 ? "成功" : "失败")},清除缓存{(result.Item2 ? "成功" : "失败")}" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 查询日志
        public ActionResult GetBaoYangOprLogByIdentityIdAndType(string identityId, string type)
        {
            if (string.IsNullOrWhiteSpace(identityId) || string.IsNullOrWhiteSpace(type))
            {
                return Json(new { Status = false, Msg = "未知的查询对象" }, JsonRequestBehavior.AllowGet);
            }
            var manager = new BaoYangActivitySettingManager();
            var result = manager.GetBaoYangOprLogByIdentityIdAndType(identityId, type);
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// 获取机油品牌系列
        /// </summary>
        /// <returns></returns>
        public ActionResult GetOilBrandAndSeries()
        {
            var result = new BaoYangManager().GetOilBrandAndSeries();
            return Json(new { Status = result != null, Data = result }, JsonRequestBehavior.AllowGet);
        }
    }
}
