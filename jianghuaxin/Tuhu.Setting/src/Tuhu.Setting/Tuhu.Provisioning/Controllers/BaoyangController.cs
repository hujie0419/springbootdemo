using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.BaoYang;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.BaoYang;
using Tuhu.Provisioning.Models;
using Tuhu.Service.BaoYang;

namespace Tuhu.Provisioning.Controllers
{
    public class BaoyangController : Controller
    {
        //
        // GET: /Baoyang/

        public ActionResult Index()
        {
            IEnumerable<BaoYangModel> list = BaoYangManager.SelectBaoYangActivityStyle();
            return View(list);
        }

        public ActionResult BaoyangpageconfigListItem(string pkid, string pagetype)
        {
            if (pkid != null)
            {
                BaoYangModel model = BaoYangManager.GetBaoYangModelByPKID(Int32.Parse(pkid));
                if (pagetype != null && pagetype.Length > 0)
                {
                    return View("BaoyangindexconfigItem", model);
                }
                else
                {
                    return View(model);
                }

            }
            else
            {
                if (pagetype != null && pagetype.Length > 0)
                {
                    return View("BaoyangindexconfigItem");
                }
                else
                {
                    return View();
                }
            }
        }


        /// <summary>
        /// 添加保养项目2
        /// </summary>
        /// <param name="model"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BaoyangpageconfigListItem(BaoYangModel model, string type)
        {
            if (!string.IsNullOrEmpty(model.ImgCode))
            {
                model.byType = Server.UrlDecode(model.byType);
                model.ImgCode = Server.UrlDecode(model.ImgCode);
            }
            if (type != null && type.Length > 0)
            {
                model.PageType = "index";
            }
            else
            {
                model.PageType = "baoyang";
            }
            int result = BaoYangManager.UpdateBaoYangModel(model);
            return Json(result);
        }


        public ActionResult GetBaoyangIndexConfigItemList()
        {
            var result = BaoYangManager.GetBaoyangIndexConfigItemList();
            return Json(result);
        }


        /// <summary>
        /// 添加活动项目1
        /// </summary>
        /// <returns></returns>
        public ActionResult AddActivityList(string type, string pkid)
        {
            //BaoYangModel model = BaoYangManager.GetBaoYangModelByPKID(Int32.Parse(pkid));
            ViewBag.BaoYangActivityStyleID = pkid;
            ViewBag.pagetype = type ?? "";
            return View();
        }

        /// <summary>
        /// 添加活动
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddActivityList(BaoYangItemModel model)
        {
            var result = BaoYangManager.InsertBaoyangList(model);
            return Json(result);
        }

        public ActionResult GetBaoyangList(string BaoYangActivityStyleID)
        {
            var BaoYangActivityItemList = BaoYangManager.GetBaoyangListByBaoYangActivityStyleID(BaoYangActivityStyleID);
            return Json(BaoYangActivityItemList);
        }

        public ActionResult DeleteActivityItem(string PKID)
        {
            bool result = BaoYangManager.DeleteActivityItemByPkid(int.Parse(PKID));
            return Json(result);
        }

        #region 保养推荐产品配置
        //保养推荐产品配置主页
        public ActionResult BaoYangRecommendProductsSetIndex()
        {
            BaoYangRulesModel model = new BaoYangRulesModel();
            model.SkuItems = new List<Tuple<string, Tuple<string, bool>>>();
            model.BrandItems = new List<Tuple<string, Tuple<string, bool>>>();
            model.BrandPriceItems =  new List<Tuple<string, Tuple<string, bool>>>();

            string PriorityField = "Brand";
            int JYenable = 0;//机油当前的状态（停用/启用）
            int XDCenable = 0;//蓄电池当前的状态（停用/启用）
            var result = BaoYangManager.GetBaoYangPriority(PriorityField);
            var data = BaoYangManager.GetRecommandProductsPriority(PriorityField);//为机油推荐配置单独创建一个新表

            var skuResult = BaoYangManager.GetBaoYangPriority("sku");
            var brandResult = BaoYangManager.GetBaoYangPriority("brand");
            var brandPriceResult = BaoYangManager.GetBaoYangPriority("brandPrice");
            var configManager = new BaoYangConfigManager();
            var config = configManager.GetRecommendConfig();
            if (config != null && config.Sku != null)
            {
                foreach (var item in config.Sku)
                {
                    model.SkuItems.Add(Tuple.Create(item.Category,
                        Tuple.Create(item.Name,
                            skuResult != null && skuResult.Any(r => r.PartName.Equals(item.Name) && r.Enabled == 0))));
                }
            }

            if (config != null && config.Brand != null)
            {
                foreach (var item in config.Brand)
                {
                    model.BrandItems.Add(Tuple.Create(item.Category,
                        Tuple.Create(item.Name,
                            brandResult != null && brandResult.Any(r => r.PartName.Equals(item.Name) && r.Enabled == 0))));
                }
            }

            if (config != null && config.BrandPrice != null)
            {
                foreach (var item in config.BrandPrice)
                {
                    model.BrandPriceItems.Add(Tuple.Create(item.Category,
                        Tuple.Create(item.Name,
                        brandPriceResult != null && brandPriceResult.Any(r => r.PartName.Equals(item.Name) && r.Enabled == 0))));
                }
            }

            if (result != null)
            {
                var JYresult = data.Where(r => r.PartName == "机油").FirstOrDefault();//从新表中获取机油的配置信息
                var XDCresult = result.Where(r => r.PartName == "蓄电池").FirstOrDefault();

                if (JYresult != null)
                    JYenable = JYresult.Enabled;
                if (XDCresult != null)
                    XDCenable = XDCresult.Enabled;

                ViewData["JYEnable"] = JYenable;
                ViewData["XDCEnable"] = XDCenable;
            }

            var fdyList = BaoYangManager.GetAntifreezeSetting();
            if (fdyList != null && fdyList.Count() > 0)
            {
                model.AntifreezeStatus = fdyList.Where(x => x.Status == 0).Count() == 0;
            }

            return View(model);
        }


        //加载机油编辑页面和，下拉框的初始数据
        public ActionResult JYEditBaoYangRecommendProducts()
        {
            string PrimaryParentCategory = "Oil";//类目
            var Branddt = BaoYangManager.GetBaoYangCP_Brand(PrimaryParentCategory);

            Dictionary<int, string> dicBrand = new Dictionary<int, string>();

            dicBrand.Add(0, "无");
            if (Branddt.Count > 0)
            {
                for (int i = 0; i < Branddt.Count; i++)
                {
                    dicBrand.Add(i + 1, Branddt[i]);//机油品牌不作处理
                }
                ViewData["CP_Brand"] = dicBrand;
            }
            return View();
        }

        //加载编辑页面和下拉框的初始数据
        public ActionResult BrandPriceEditBaoYangRecommendProducts(string PrimaryParentCategory)
        {
            List<string> Branddt = new List<string>();
            List<string> seriesName = new List<string>();
            List<string> seriesKey = new List<string>();
            Dictionary<int, string> dicBrand = new Dictionary<int, string>();
            var configManager = new BaoYangConfigManager();
            var config = configManager.GetRecommendConfig();
            if (config != null && config.BrandPrice != null)
            {
                foreach (var item in config.BrandPrice)
                {
                    if (item.Category.Equals(PrimaryParentCategory))
                    {
                        ViewBag.PartName = item.Name;
                        seriesName = string.Join(",", item.SeriesName).Split(',').ToList();
                        seriesKey = string.Join(",", item.SeriesKey).Split(',').ToList();
                        break;
                    }
                }
            }
            if (!string.IsNullOrEmpty(PrimaryParentCategory))
            {
                ViewBag.PrimaryParentCategory = PrimaryParentCategory;
                if (seriesName != null && seriesName.Count() > 0)
                {
                    ViewBag.FirstSeriesName = seriesName[0].ToString();
                    ViewBag.SecondSeriesName = seriesName[1].ToString();
                    ViewBag.ThirdSeriesName = seriesName[2].ToString();
                }
                if (seriesKey != null && seriesKey.Count() > 0)
                {
                    ViewBag.FirstSeriesKey = seriesKey[0].ToString();
                    ViewBag.SecondSeriesKey = seriesKey[1].ToString();
                    ViewBag.ThirdSeriesKey = seriesKey[2].ToString();
                }
                if (PrimaryParentCategory == "Dynamo")
                {
                    Branddt = BaoYangManager.GetDynamoCP_ShuXing(PrimaryParentCategory);
                }
                else
                {
                    Branddt = BaoYangManager.GetBaoYangCP_Brand(PrimaryParentCategory);
                }
            }

            dicBrand.Add(0, "无");
            if (Branddt.Count > 0)
            {
                for (int i = 0; i < Branddt.Count; i++)
                {
                    dicBrand.Add(i + 1, Branddt[i].Split('/')[0]);
                }
                ViewData["CP_Brand"] = dicBrand;
            }

            return View();
        }

        //加载蓄电池的，修改页面
        public ActionResult BatteryEditBaoYangRecommendProducts()
        {
            string partName = "蓄电池";
            var Branddt = BaoYangManager.GetBatteryCP_Brand(partName);
            Dictionary<int, string> dicBrand = new Dictionary<int, string>();
            dicBrand.Add(0, "无");
            if (Branddt.Rows.Count > 0)
            {
                for (int i = 0; i < Branddt.Rows.Count; i++)
                {
                    dicBrand.Add(i + 1, Regex.Replace(Branddt.Rows[i][0].ToString().Split('/')[0], @"\s", ""));
                }
                ViewData["CP_Brand"] = dicBrand;
            }
            return View();
        }



        //获取修改页面的数据
        public ActionResult BaoYangRecommendByProjectName(string parameters)
        {
            string PartName = string.Empty;
            string PriorityField = "Brand";//获取优先级按品牌来分的

            if (parameters == "jyv")
            {
                PartName = "机油滤清器";
                //PrimaryParentCategory = "OilFilter";
            }
            if (parameters == "kv")
            {
                PartName = "空气滤清器";
                //PrimaryParentCategory = "AirFilter";
            }
            if (parameters == "xdc")
            {
                PartName = "蓄电池";
            }
            if (parameters == "Wiper")
            {
                PriorityField = "BrandPrice";
                PartName = "雨刷";
            }
            if (parameters == "Sparkplug")
            {
                PriorityField = "BrandPrice";
                PartName = "火花塞";
            }
            if (parameters == "Dynamo")
            {
                PriorityField = "BrandPrice";
                PartName = "车灯";
            }
            // return Json(brands.Select(o => Regex.Replace(o.Split('/')[0], @"\s", "")), JsonRequestBehavior.AllowGet);

            var result = BaoYangManager.GetBaoYangPriorityByPartNameAndField(PartName, PriorityField);
            //if (parameters == "ys" && result.Count() > 0) {
            //    result=result.Where(o=>Regex.Replace(o.FirstPriority))
            //}
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPriorityRules(string partName, string priorityField)
        {
            var result = BaoYangManager.GetBaoYangPriorityByPartNameAndField(partName, priorityField);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 关闭或启用模块通过partName(新表目前只有机油)
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="onOrof"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetProductsRecommendIsEnable(string parameters, int onOrof, string category = "")
        {
            const string KEY = "BaoYangPrioritySetting/";
            string PartName = parameters;
            if (parameters == "jy") { PartName = "机油"; }
            var userName = HttpContext.User.Identity.Name;

            var oldData = BaoYangManager.GetJYPriorityDetailsByPartName(PartName).ToList();
            var result = BaoYangManager.setProductsRecommendIsEnable(PartName, onOrof);

            if (result)
            {
                var newData = BaoYangManager.GetJYPriorityDetailsByPartName(PartName).ToList();
                BaoYangManager.SendEmailToUserForJY(oldData, newData, HttpContext.User.Identity.Name);

                var log = new Tuhu.Provisioning.DataAccess.Entity.OprLog
                {
                    ObjectID = 0,
                    ObjectType = "BaoYang",
                    BeforeValue = "Update",
                    AfterValue = "Enabled="+onOrof,
                    Author = userName,
                    Operation = "保养机油推荐启用状态修改"
                };
                new OprLogManager().AddOprLog(log);
            }

            return Json(result);
        }

        /// <summary>
        /// 获取保养默认推荐的信息(采用新表，目前只有机油配置使用)
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetRecommendPriorityByProjectName(string parameters)
        {
            string partName = string.Empty;
            string type = "Brand";
            if (parameters == "jy") { partName = "机油"; }

            var result = BaoYangManager.SelectRecommendPriorityByProjectName(partName, type);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存保养推荐页面的数据，根据名称和属性查找，如果存在此条数据，则修改，否则做添加操作(采用新表，目前只有机油)
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        //[PowerManage]
        [HttpPost]
        public ActionResult UpsertRecommendPrioritySetting(string project)
        {
            const string KEY = "BaoYangPrioritySetting/";
            var userName = HttpContext.User.Identity.Name;
            bool result = false;

            List<BaoYangProductRecommendModel> projectList = null;
            if (!string.IsNullOrEmpty(project))
            {
                projectList = JsonConvert.DeserializeObject<List<BaoYangProductRecommendModel>>(project);
                var oldData = BaoYangManager.GetJYPriorityDetailsByPartName(projectList.FirstOrDefault().PartName).ToList();
                result = BaoYangManager.UpsertRecommendPrioritySetting(projectList, userName);
                if (result)
                {
                    var newData = BaoYangManager.GetJYPriorityDetailsByPartName(projectList.FirstOrDefault().PartName).ToList();
                    BaoYangManager.SendEmailToUserForJY(oldData, newData, userName);
                }
            }

            if (result)
            {
                var log = new Tuhu.Provisioning.DataAccess.Entity.OprLog
                {
                    ObjectID = 0,
                    ObjectType = "BaoYang",
                    BeforeValue = "update",
                    AfterValue = project,
                    Author = userName,
                    Operation = "更改保养机油默认推荐配置信息"
                };
                new OprLogManager().AddOprLog(log);
            }

            return Json(result);
        }

        //[PowerManage]
        [HttpPost]
        public ActionResult setIsEnable(string parameters, int onOrof, string category = "")
        {
            const string KEY = "BaoYangPrioritySetting/";

            string PartName = parameters;

            if (parameters == "jyv")
            {
                PartName = "机油滤清器";
            }
            if (parameters == "kv")
            {
                PartName = "空气滤清器";
            }
            if (parameters == "xdc")
            {
                PartName = "蓄电池";
            }
            var oldData = BaoYangManager.GetBaoYangPriorityDetailsByPartName(PartName).ToList();
            var result = BaoYangManager.setIsEnable(PartName, onOrof);
            if (result)
            {
                var newData = BaoYangManager.GetBaoYangPriorityDetailsByPartName(PartName).ToList();
                BaoYangManager.SendEmailToUser(oldData, newData, HttpContext.User.Identity.Name);
            }

            return Json(result);
        }

        //[PowerManage]
        [HttpPost] 
        public ActionResult SaveRecommendProducts(string project)
        {
            const string KEY = "BaoYangPrioritySetting/";
            var userName = HttpContext.User.Identity.Name;
            bool result = false;

            List<PrioritySettingModel> projectList = null;
            if (!string.IsNullOrEmpty(project))
            {
                projectList = JsonConvert.DeserializeObject<List<PrioritySettingModel>>(project);
                var oldData = BaoYangManager.GetBaoYangPriorityDetailsByPartName(projectList.FirstOrDefault().PartName).ToList();
                result = BaoYangManager.SaveRecommendProducts(projectList, userName);
                if (result)
                {
                    var newData= BaoYangManager.GetBaoYangPriorityDetailsByPartName(projectList.FirstOrDefault().PartName).ToList();
                    BaoYangManager.SendEmailToUser(oldData, newData, HttpContext.User.Identity.Name);
                }
            }

            return Json(result);
        }

        #endregion

        #region SKU配置

        [HttpGet]
        public JsonResult GetProductNameByPid(string pid, string category)
        {
            string productName = BaoYangManager.GetProductNameByPid(pid, category);

            return Json(productName, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 品牌配置

        [HttpGet]
        public JsonResult GetBrands(string productCategory)
        {
            var brands = BaoYangManager.GetBaoYangCP_Brand(productCategory);

            return Json(brands.Select(o => Regex.Replace(o.Split('/')[0], @"\s", "")), JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region 防冻液相关配置
        [HttpPost]
        public ActionResult SetAntifreezeStatus(byte Status)
        {
            var oldData = BaoYangManager.GetAntifreezeSetting().ToList();
            var result = BaoYangManager.SetAntifreezeStatus(Status);
            if (result)
            {
                var newData = BaoYangManager.GetAntifreezeSetting().ToList();
                BaoYangManager.SendEmailToUser(oldData, newData, HttpContext.User.Identity.Name);
            }
            return Json(new { IsSuccess =result });
        }

        //加载防冻液编辑页面
        public ActionResult AntifreezeEditPage()
        {
            return View(Tuple.Create(BaoYangManager.GetBaoYangCP_Brand("Antifreeze"), GetProvinceList()));
        }
        [HttpGet]
        public ActionResult GetAntifreezeData()
        {
            var fdyList = BaoYangManager.GetAntifreezeSetting();
            return Json(new { SettingList = fdyList }, JsonRequestBehavior.AllowGet);
        }
        public IEnumerable<ProvinceItem> GetProvinceList()
        {
            var RegionKey = "ProvinceKey/Antifreeze";
            IEnumerable<ProvinceItem> ProvinceList = null;
            if (HttpRuntime.Cache[RegionKey] != null)
            {
                ProvinceList = (IEnumerable<ProvinceItem>)HttpRuntime.Cache[RegionKey];
            }
            else
            {
                ProvinceList = BaoYangManager.GetProvinceList();
                if (ProvinceList.Count() > 0)
                {
                    HttpRuntime.Cache.Add(RegionKey, ProvinceList, null, DateTime.Now.AddDays(10), Cache.NoSlidingExpiration, CacheItemPriority.AboveNormal, null);
                }
            }
            return ProvinceList;
        }

        //[PowerManage]
        [HttpPost]
        public ActionResult SaveAntifreezeSetting(int Code, string Brands, string ProvinceIds, string ProvinceNames, string FreezingPoint)
        {
            var ErrorMessage = string.Empty;
            AntifreezeSettingModel AntifreezeSettingItem = new AntifreezeSettingModel { Brand = Brands, ProvinceIds = ProvinceIds, ProvinceNames = ProvinceNames, FreezingPoint = FreezingPoint, PKID = Code };
            var oldData = BaoYangManager.GetAntifreezeSetting().ToList();
            var Result = BaoYangManager.SaveAntifreezeSetting(Code == 0 ? 0 : 1, AntifreezeSettingItem, out ErrorMessage);
            if (Result)
            {
                ThreadPool.QueueUserWorkItem(o =>
                {
                    var newData = BaoYangManager.GetAntifreezeSetting().ToList();
                    BaoYangManager.SendEmailToUser(oldData, newData, HttpContext.User.Identity.Name);
                });
            }
            return Json(new { IsSuccess = Result, Msg = ErrorMessage });
        }
        #endregion


        #region 保养特殊车型推荐配置

        public ActionResult BaoYangPriorityVehicleSetting(string key, string partName)
        {
            ViewBag.Key = key;
            ViewBag.PartName = partName;
			if(string.Equals(key, "oilnew", StringComparison.OrdinalIgnoreCase))
            {
                var result = new BaoYangConfigManager().ViscosityList();
                ViewData["Viscosities"] = result;
                return View("BaoYangOilPriorityVehicleSetting");
            }
            if(string.Equals(partName, "机油"))
            {
                var list = new BaoYangConfigManager().ViscosityList();
                var result = new List<SelectListItem>();
                result.Add(new SelectListItem { Text = "全部", Value = "", Selected = true });
                result.AddRange(list.Select(x => new SelectListItem { Text = x, Value = x }).ToList());
                ViewData["Viscosity"] = result;
            }
            return View();
        }

        /// <summary>
        /// 根据partName获取当前车型的配置的信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="partName"></param>
        /// <param name="brand"></param>
        /// <param name="vehicleID"></param>
        /// <param name="staPrice">车价范围---开始车价</param>
        /// <param name="endPrice">车价范围---结束车价</param>
        /// <param name="isConfig">是否显示已配置的车型 0 不显示  1  显示</param>
        /// <returns></returns>
        public ActionResult BaoYangVehicleConfigTable(int pageIndex, int pageSize, string partName, string brand, string vehicleID,
            string staPrice, string endPrice, int isConfig, string priorityType1, string firstPriority, 
            string priorityType2, string secondPriority, string viscosity)
        {
            int totalCount = 0;
            BaoYangManager manager = new BaoYangManager();
            List<BaoYangPriorityVehicleSettingModel> result = new List<BaoYangPriorityVehicleSettingModel>();
            Tuple<List<BaoYangPriorityVehicleSettingModel>, int> data = null;

            if (string.IsNullOrEmpty(brand) || brand == "null") { brand = string.Empty; }
            if (string.IsNullOrEmpty(vehicleID) || vehicleID == "null") { vehicleID = string.Empty; }

            data = manager.SelectBaoYangVehicleSetting(pageIndex, pageSize, partName, brand, vehicleID, staPrice,
                endPrice, isConfig, priorityType1, firstPriority, priorityType2, secondPriority, viscosity);

            if (data != null && data.Item1.Any())
            {
                result = data.Item1;
                totalCount = data.Item2;
            }

            var list = new OutData<List<BaoYangPriorityVehicleSettingModel>, int>(result, totalCount);
            ViewBag.TotalCount = totalCount;

            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = totalCount
            };
            ViewBag.PartName = partName;
            return this.View(new ListModel<BaoYangPriorityVehicleSettingModel>(list.ReturnValue, pager));
        }

        public ActionResult BaoYangVehicleOilSetting()
        {
            return View();
        }

        public ActionResult BaoYangVehicleOilSettingList(int pageIndex, int pageSize, string brand,
            string vehicleID, string paiLiang, int isConfig)
        {
            int totalCount = 0;
            BaoYangManager manager = new BaoYangManager();
            List<VehicleOilSetting> result = new List<VehicleOilSetting>();
            Tuple<List<VehicleOilSetting>, int> data = null;

            if (string.IsNullOrEmpty(brand) || brand == "null")
            {
                brand = string.Empty;
            }
            if (string.IsNullOrEmpty(vehicleID) || vehicleID == "null")
            {
                vehicleID = string.Empty;
            }
            if (string.IsNullOrEmpty(paiLiang) || paiLiang == "null")
            {
                paiLiang = string.Empty;
            }

            data = manager.SelectVehicleOilSetting(pageIndex, pageSize, brand, vehicleID, paiLiang, isConfig);

            if (data != null && data.Item1.Any())
            {
                result = data.Item1;
                totalCount = data.Item2;
            }

            var list = new OutData<List<VehicleOilSetting>, int>(result, totalCount);
            ViewBag.TotalCount = totalCount;

            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = totalCount
            };

            return this.View(new ListModel<VehicleOilSetting>(list.ReturnValue, pager));
        }

        [PowerManage]
        public JsonResult UpsertVehicleOilViscosity(string vehicleId, string paiLiang, string recommendViscosity)
        {
            var user = HttpContext.User.Identity.Name;
            BaoYangManager manager = new BaoYangManager();
            var result = manager.UpsertVehicleOilViscosity(vehicleId, paiLiang, recommendViscosity, user);

            if (result)
            {
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        [PowerManage]
        public JsonResult BatchUpsertVehicleOilViscosity(string batchVehicle, string recommendViscosity)
        {
            var user = HttpContext.User.Identity.Name;
            BaoYangManager manager = new BaoYangManager();
            var batchVehicleModels = JsonConvert.DeserializeObject<List<VehicleOilSetting>>(batchVehicle);
            var result = manager.BatchUpsertVehicleOilViscosity(batchVehicleModels, recommendViscosity, user);
            if (result)
            {
                return Json(new {status = "success"}, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }          
        }

        [PowerManage]
        public JsonResult DeleteVehicleOilViscosity(string vehicleId, string paiLiang)
        {
            BaoYangManager manager = new BaoYangManager();
            var result = manager.DeleteVehicleOilViscosity(vehicleId, paiLiang);

            if (result)
            {
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取所有车型的品牌
        /// </summary>
        /// <returns></returns>
        public JsonResult GetVehicleBrands()
        {
            BaoYangManager manager = new BaoYangManager();
            var data = manager.SelectAllVehicleBrands();

            if (data != null && data.Count() > 0)
            {
                return Json(new { status = "success", data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 根据选择的品牌获取该品牌的车型系列
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        public JsonResult GetVehicleSeries(string brand)
        {
            BaoYangManager manager = new BaoYangManager();
            IDictionary<string, string> data = null;
            if (!string.IsNullOrEmpty(brand))
            {
                data = manager.SelectVehicleSeries(Server.UrlDecode(brand));
            }
            if (data != null && data.Any())
            {
                return Json(new { status = "success", data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail", data = data }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SelectVehiclePaiLiang(string vid)
        {
            BaoYangManager manager = new BaoYangManager();
           IEnumerable<string> data = null;
            if (!string.IsNullOrEmpty(vid))
            {
                data = manager.SelectVehiclePaiLiang(Server.UrlDecode(vid));
            }
            if (data != null && data.Any())
            {
                return Json(new { status = "success", data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail", data = data }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 根据productCategory获取产品表对应的车型系列名称
        /// </summary>
        /// <param name="productCategory">partName对应的key</param>
        /// <returns></returns>
        public JsonResult GetVehicleSeriesByProductCategory(string productCategory)
        {
            BaoYangManager manager = new BaoYangManager();

            var data = manager.GetVehicleSeriesByProductCategory(productCategory);

            if (data != null && data.Count() > 0)
            {
                return Json(new { status = "success", data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 根据系列名称获取产品表机油对应的品牌
        /// </summary>
        /// <param name="priorityType">机油的key--'Oil'</param>
        /// <param name="series">系列的名称</param>
        /// <returns></returns>
        public JsonResult GetJYBrandByPriorityTypeAndSeries(string priorityType, string series)
        {
            BaoYangManager manager = new BaoYangManager();
            var data = manager.GetJYBrandByPriorityTypeAndSeries(priorityType, series);

            if (data != null && data.Any())
            {
                if (data.Count > 0)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        data[i] = data[i].Split('/')[0];
                    }
                }
                return Json(new { status = "success", data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 根据partName、PriorityType、vehicleID获取特殊车型配置表的配置信息
        /// </summary>
        /// <param name="partName">零件名称</param>
        /// <param name="priorityType">零件的Key</param>
        /// <param name="vehicleID">车型的ID</param>
        /// <returns></returns>
        public JsonResult GetPriorityVehicleRules(string partName, string priorityType, string vehicleID)
        {
            BaoYangManager manager = new BaoYangManager();
            List<BaoYangPriorityVehicleSettingModel> result = new List<BaoYangPriorityVehicleSettingModel>();

            if (!string.IsNullOrEmpty(vehicleID))
            {
                result = manager.GetPriorityVehicleRulesByPartNameAndType(partName, priorityType, vehicleID);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据VehicleID获取机油的特殊车型的配置信息
        /// </summary>
        /// <param name="vehicleID"></param>
        /// <returns></returns>
        public JsonResult GetJYPriorityVehicleSetting(string vehicleID)
        {
            BaoYangManager mananer = new BaoYangManager();

            BaoYangPriorityVehicleSettingModel result = new BaoYangPriorityVehicleSettingModel();

            if (!string.IsNullOrEmpty(vehicleID))
            {
                result = mananer.GetJYPriorityVehicleSetting(vehicleID);
            }

            if (result != null)
            {
                return Json(new { status = "success", data = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 保存保养的特殊车型的配置信息
        /// </summary>
        /// <param name="project"></param>
        /// <param name="vehicleIDs">一系列的vehicleID</param>
        /// <param name="partName"></param>
        /// <returns></returns>
        public ActionResult SaveVehicleSettingRecommendProducts(string project, string vehicleIDs, string partName)
        {
            bool result = false;
            var userName = HttpContext.User.Identity.Name;

            List<BaoYangPriorityVehicleSettingModel> projectList = null;
            if (!string.IsNullOrEmpty(project))
            {
                projectList = JsonConvert.DeserializeObject<List<BaoYangPriorityVehicleSettingModel>>(project);
                var vehicleIdList = vehicleIDs.Split(',').ToList();
                if (projectList != null && projectList.Any() && vehicleIdList.Count > 0)
                {
                    result = BaoYangManager.InsertOrUpdateVehicleSetting(projectList, vehicleIdList, partName, userName);
                }
            }

            return Json(result);
        }

        /// <summary>
        /// 保存机油的特殊车型的配置信息
        /// </summary>
        /// <param name="project"></param>
        /// <param name="vehicleIDs"></param>
        /// <param name="partName"></param>
        /// <returns></returns>
        public ActionResult SaveJYVehicleSeriesSetting(string project, string vehicleIDs, string partName)
        {
            bool result = false;
            var userName = HttpContext.User.Identity.Name;

            List<BaoYangPriorityVehicleSettingModel> projectList = null;
            if (!string.IsNullOrEmpty(project))
            {
                projectList = JsonConvert.DeserializeObject<List<BaoYangPriorityVehicleSettingModel>>(project);
                var vehicleIdList = vehicleIDs.Split(',').ToList();
                if (projectList != null && projectList.Any() && vehicleIdList.Count > 0)
                {
                    result = BaoYangManager.InsertBaoYangJYVehicleSeriesSetting(projectList, vehicleIdList, partName, userName);
                }
            }

            return Json(result);
        }

        /// <summary>
        /// 获取保养推荐的配置信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRecommendConfig()
        {
            var configManager = new BaoYangConfigManager();
            var result = configManager.GetRecommendConfig();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除保养车型推荐的信息
        /// </summary>
        /// <param name="vehicleID"></param>
        /// <param name="partName"></param>
        /// <returns></returns>
        public JsonResult DeletedBaoYangPriorityVehicleSetting(string vehicleID, string partName)
        {
            bool result = false;
            BaoYangManager manager = new BaoYangManager();
            var userName = HttpContext.User.Identity.Name;

            result = manager.DeletedBaoYangPriorityVehicleSettingByVehicleIDAndPartName(vehicleID, partName, userName);

            if (result)
            {
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult BatchDeletedBaoYangVehicleConfig(string vehicleIdStr, string partName)
        {
            bool result = false;
            BaoYangManager manager = new BaoYangManager();
            var userName = HttpContext.User.Identity.Name;

            result = manager.BatchDeletedBaoYangVehicleConfig(vehicleIdStr, partName, userName);

            if (result)
            {
                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Vehicle SettingNew

        public ActionResult GetBaoYangPriorityVehicleSettingNew(VehicleSettingNewSearchRequest request)
        {
            var manager = new BaoYangManager();
            request = request ?? new VehicleSettingNewSearchRequest();
            if (!string.IsNullOrEmpty(request.VehicleId))
            {
                request.Brand = null;
            }
            var result = manager.GetBaoYangPriorityVehicleSettingNew(request);
            return Json(new { status = result != null, data = result.Item2, total = result.Item1 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddOrEditPriorityVehicleSettingNew(List<PriorityVehicleSettingNew> settings)
        {
            Func<string> validFunc = () =>
            {
                settings = settings?.Where(s => !string.IsNullOrWhiteSpace(s.PartName) && !string.IsNullOrWhiteSpace(s.VehicleId)).ToList();
                if (settings == null || !settings.Any())
                {
                    return "配置不能为空.";
                }
                if (settings.Any(s => s.PartName.Equals("机油") && string.IsNullOrWhiteSpace(s.Series)))
                {
                    return "机油系列不能为空";
                }
                return string.Empty;
            };
            var validResult = validFunc();
            if (!string.IsNullOrEmpty(validResult))
            {
                return Json(new { status = false, msg = validResult });
            }
            var manager = new BaoYangManager();

            bool success = manager.AddOrEditPriorityVehicleSettingNew(settings, User.Identity.Name);

            return Json(new { status = success });
        }

        [HttpPost]
        public ActionResult DelPriorityVehicleSettingNew(List<string> vehicleIds, string partName)
        {
            Func<string> validFunc = () =>
            {
                if (vehicleIds == null || !vehicleIds.Any(v => !string.IsNullOrEmpty(v)))
                {
                    return "请至少选一个进行删除";
                }
                if (string.IsNullOrEmpty(partName))
                {
                    return "配件名称有误";
                }
                vehicleIds = vehicleIds.Where(x => !string.IsNullOrEmpty(x)).ToList();
                return string.Empty;
            };
            var validResult = validFunc();
            if (!string.IsNullOrEmpty(validResult))
            {
                return Json(new { status = false, msg = validFunc });
            }
            var manager = new BaoYangManager();
            bool success = manager.DelPriorityVehicleSettingNew(vehicleIds, partName, User.Identity.Name);
            return Json(new { status = success });
        }

        [HttpPost]
        public ActionResult EnableOrDisableVehicleSettingNew(List<string> vehicleIds, string partName, bool isEnabled)
        {
            Func<string> validFunc = () =>
            {
                if (vehicleIds == null || !vehicleIds.Any(v => !string.IsNullOrEmpty(v)))
                {
                    return "请至少选一个进行删除";
                }
                if (string.IsNullOrEmpty(partName))
                {
                    return "配件名称有误";
                }
                vehicleIds = vehicleIds.Where(x => !string.IsNullOrEmpty(x)).ToList();
                return string.Empty;
            };
            var validResult = validFunc();
            if (!string.IsNullOrEmpty(validResult))
            {
                return Json(new { status = false, msg = validFunc });
            }
            var manager = new BaoYangManager();
            bool success = manager.EnableOrDisableVehicleSettingNew(vehicleIds, partName, isEnabled, User.Identity.Name);
            return Json(new { status = success });
        }

        public ActionResult GetPriorityVehicleSettings(string vehicleId, string partName)
        {
            var manager = new BaoYangManager();
            var result = string.IsNullOrEmpty(vehicleId) || string.IsNullOrEmpty(partName) ? new List<PriorityVehicleSettingNew>() :
                manager.GetPriorityVehicleSettings(vehicleId, partName);
            return Json(new { status = result != null, data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOilBrandAndSeries()
        {
            var manager = new BaoYangManager();
            var result = manager.GetOilBrandAndSeries();
            return Json(new { status = result != null, data = result }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Product SettingNew

        public ActionResult GetProductPrioritySettingsNew(string partName)
        {
            var manager = new BaoYangManager();
            var list = manager.GetProductPrioritySettingsNew(partName);
            var result = list.GroupBy(x => new { x.PartName, x.PriorityType }).Select(g => new
            {
                g.Key.PartName,
                g.Key.PriorityType,
                Priorities = g.Select(x => new
                {
                    x.Brand,
                    x.Series,
                    x.PID,
                    //x.IsEnabled,
                    x.Priority
                }).ToList(),
            });
            return Json(new { status = result != null, data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddProductPrioritySettingsNew(List<ProductPrioritySettingNew> settings)
        {
            Func<string> vaildFun = () =>
            {
                if (settings == null)
                {
                    return "参数不能为空";
                }
                settings.ForEach(x =>
                {
                    x.PartName = x.PartName?.Trim();
                    x.PriorityType = x.PriorityType?.Trim();
                    x.Series = x.Series?.Trim();
                    x.Brand = x.Brand?.Trim();
                    if (x.PartName == "机油")
                    {
                        x.IsEnabled = true;
                        x.PID = string.Empty;
                    }
                });
                settings = settings.Where(x => !(string.IsNullOrEmpty(x.Brand) && string.IsNullOrEmpty(x.Series))).ToList();
                if (!settings.Any())
                {
                    return "配置不允许为空";
                }
                if (settings.Any(s => "机油" == s.PartName && !string.IsNullOrEmpty(s.Brand) && string.IsNullOrEmpty(s.Series)))
                {
                    return "系列不能为空";
                }
                if (settings.Any(s => string.IsNullOrEmpty(s.PartName) || string.IsNullOrEmpty(s.PriorityType)))
                {
                    return "配置有误！请刷新页面重试。";
                }
                return string.Empty;
            };
            var vaildResult = vaildFun();
            if (!string.IsNullOrEmpty(vaildResult))
            {
                return Json(new { status = false, msg = vaildResult });
            }
            var manager = new BaoYangManager();
            var success = manager.AddProductPrioritySettingsNew(settings, User.Identity.Name);
            return Json(new { status = success });
        }

        #endregion

        #region InstallType Config

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetInstallTypeConfigs()
        {
            var config = new InstallTypeConfigManager().GetInstallTypeConfigs();

            return Json(config, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateInstallType(InstallTypeConfig config)
        {
            bool result = new InstallTypeConfigManager().UpdateInstallTypeConfig(config, User.Identity.Name);

            return Json(new { success = result });
        }

        [HttpPost]
        public JsonResult GetInstallTypeVehicleConfigs(string packageType, string installType,
            string brand, string series, string vehicleId, string categories, int minPrice, int maxPrice, string brands,
            bool isConfig, int pageIndex, int pageSize)
        {
            var manager = new InstallTypeConfigManager();
            var count = manager.GetInstallTypeVehicleConfigCount(packageType, installType,
                 brand, series, vehicleId, categories, minPrice, maxPrice, brands, isConfig);
            var configs = manager.GetInstallTypeVehicleConfig(packageType, installType,
                 brand, series, vehicleId, categories, minPrice, maxPrice, brands, isConfig, pageIndex, pageSize);

            return Json(new { data = configs, totalCount = count });
        }

        [HttpPost]
        public JsonResult DeleteInstallTypeVehicleConfig(string packageType, string installType,
            string vehicleIds)
        {
            var manager = new InstallTypeConfigManager();
            var result = manager.DeleteInstallTypeVehicleConfig(packageType, installType, vehicleIds, User.Identity.Name);

            return Json(new { success = result });
        }

        [HttpPost]
        public JsonResult AddInstallTypeVehicleConfig(string packageType, string installType,
            string vehicleIds)
        {
            var manager = new InstallTypeConfigManager();
            var result = manager.AddInstallTypeVehicleConfig(packageType, installType, vehicleIds, User.Identity.Name);

            return Json(new { success = result });
        }

        #endregion
        
        #region 保养升级购
        /// <summary>
        /// 获取保养升级购图标
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetBaoYangLevelUpIconConfig()
        {
            var manager = new BaoYangManager();
            var result= manager.GetBaoYangLevelUpIcon();
            return Json(new { Status = !string.IsNullOrEmpty(result), Data = result });
        }

        /// <summary>
        /// 更新保养升级购图标配置
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpsertBaoYangLevelUpIcon(string icon)
        {
            var manager = new BaoYangManager();
            var result = manager.UpsertBaoYangLevelUpIcon(icon);
            return Json(new { Status = result, Msg = $"更新{(result ? "成功" : "失败")}" });
        }

        /// <summary>
        /// 清除保养升级购图标缓存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RemoveBaoYangLevelUpIconCache()
        {
            var manager = new BaoYangManager();
            var result = manager.RemoveBaoYangLevelUpIconCache();
            return Json(new { Status = result, Msg = $"清除缓存{(result ? "成功" : "失败")}" });
        }
        #endregion
        
        [HttpPost]
        public async Task<JsonResult> UpdateCache(string type, List<string> data)
        {
            bool result = false;
            using(var client = new CacheClient())
            {
                var serviceResult = await client.RemoveByTypeAsync(type, data);
                if (serviceResult.Success)
                {
                    result = serviceResult.Result;
                }
            }

            return Json(new { success = result });
        }
    }
}
