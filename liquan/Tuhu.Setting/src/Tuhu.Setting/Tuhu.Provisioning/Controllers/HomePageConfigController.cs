using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.HomePageConfig;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.Entity;
using TDAEntity = Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class HomePageConfigController : Controller
    {
        private static string ObjectType = "HPC";
        private static Lazy<HomePageConfigManager> manager = new Lazy<HomePageConfigManager>();
        private static HomePageConfigManager GetManager
        {
            get
            {
                return manager.Value;
            }
        }

        // GET: /HomePageConfig/
        public ActionResult Index()
        {
            //if (!HttpContext.Request.IsAuthenticated)
            //{
            //  return  Redirect("/Home/index");
            //}
            ViewBag.ParentList = GetParentAllData();
            return View();
        }

        #region 添加大区域操作
        public ActionResult BigModule(int id = 0)
        {
            if (id != 0)
            {
                ViewBag.Title = "修改";
                tal_newappsetdata_v2 appv2 = GetManager.GetTal_newappsetdata_v2ById(id).FirstOrDefault<tal_newappsetdata_v2>();
                return View(appv2);
            }
            else
            {
                ViewBag.Title = "新增";
                tal_newappsetdata_v2 appv2 = new tal_newappsetdata_v2()
                {
                    id = 0,
                    areatype = (int)AreaTypeEnum.大区域,
                    isparent = true,
                    createtime = DateTime.Now
                };
                return View(appv2);
            }
        }

        public ActionResult SaveBigModule(tal_newappsetdata_v2 model, int id = 0)
        {
            bool result = false;
            if (id == 0)
            {
                model.isparent = true;
                result = GetManager.AddTal_newappsetdata_v2(model);
                if (result)
                {

                    LoggerManager.InsertOplog(User.Identity.Name, ObjectType, model.id, "添加大渠道" + model.modelname);
                }

            }
            else
            {
                model.updatetime = DateTime.Now;
                model.isparent = true;
                result = GetManager.UpdateTal_newappsetdata_v2(model);
                if (result)
                {
                    LoggerManager.InsertOplog(User.Identity.Name, ObjectType, model.id, "修改大渠道" + model.modelname);
                }

            }
            return RedirectToAction("Index");
        }
        #endregion

        #region 添加子区域操作
        /// <summary>
        /// 添加子级
        /// </summary>
        /// <param name="pid">父ID</param>
        /// <param name="id">子ID</param>
        /// <param name="isothercity">是否添加城市</param>
        /// <returns></returns>
        public ActionResult BigModuleChild(int pid = 0, int id = 0, int areatype = 0)
        {
            #region 获取省市
            List<SelectListItem> list = ParentCityList();
            ViewBag.ParentCity = list;
            if (id == 0 && (list != null && list.Count > 0))
            {
                ViewBag.ChildCity = ParentCityList(Convert.ToInt32(list[0].Value));
            }
            #endregion

            if (id != 0)
            {
                ViewBag.Title = "修改";
                tal_newappsetdata_v2 appv2 = GetManager.GetTal_newappsetdata_v2ById(id).FirstOrDefault<tal_newappsetdata_v2>();
                ViewBag.ChildCity = ParentCityList(Convert.ToInt32(appv2.citycode)) ?? null;
                return View(appv2);
            }
            else
            {
                ViewBag.Title = "新增";
                tal_newappsetdata_v2 appv2 = new tal_newappsetdata_v2();
                appv2.id = 0;
                appv2.parentid = pid;

                if (areatype == 2)
                    appv2.areatype = (int)AreaTypeEnum.区域楼层;
                else if (areatype == 3)
                    appv2.areatype = (int)AreaTypeEnum.区域小模块;
                else if (areatype == 4)
                    appv2.areatype = (int)AreaTypeEnum.小模块城市;
                else
                    appv2.areatype = areatype;

                appv2.isparent = false;
                appv2.starttime = DateTime.Now;
                appv2.overtime = DateTime.Now;
                appv2.createtime = DateTime.Now;
                appv2.updatetime = DateTime.Now;
                return View(appv2);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveBigModuleChild(tal_newappsetdata_v2 model, int parentid = 0, int id = 0)
        {
            
            bool result = false;
            if (id == 0)
            {
                model.isparent = false;
                result = GetManager.AddChidAreaData(model);
                if (result)
                {
                    LoggerManager.InsertOplog(User.Identity.Name, ObjectType, model.id, "添加子区域" + model.modelname);
                }

            }
            else
            {
                model.isparent = false;
                result = GetManager.UpdateChidAreaData(model);
                if (result)
                {
                    LoggerManager.InsertOplog(User.Identity.Name, ObjectType, model.id, "修改子区域" + model.modelname);
                }
            }
            return Json(true);
        }

        /// <summary>
        /// 获取城市信息集合JSON
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
        [HttpPost]
        public string GetCityJson(int parentID)
        {
            Dictionary<int, object> resultItems = Municipalities(parentID);
            if (resultItems != null)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject((List<TDAEntity.CityModel>)resultItems[1]);
            }
            return GetCityInfo.CreateGetCityInfo.GetCityJson(parentID);
        }
        #endregion

        #region 列表区域操作
        public ActionResult ShowModuleChildList(int pid = 0)
        {
            ViewBag.ShowModuleChildList = GetManager.GetTal_newappsetdata_v2ByParentId(pid) ?? null;
            return View();
        }
        #endregion

        #region 添加产品
        public ActionResult ProdcutModule(int id = 0)
        {
            return RedirectToAction("Index");
        }
        public ActionResult SaveProdcutModule(int id = 0)
        {
            return RedirectToAction("Index");
        }
        #endregion

        #region 数据处理
        /// <summary>
        /// 获取全部配置信息
        /// </summary>
        /// <returns></returns>
        private List<tal_newappsetdata_v2> GetAllData()
        {
            return GetManager.GetALLTal_newappsetdata_v2() ?? null;
        }

        /// <summary>
        /// 获取全部顶级节点
        /// </summary>
        /// <returns></returns>
        private List<tal_newappsetdata_v2> GetParentAllData()
        {
            var list = GetAllData();
            if (list != null && list.Count > 0)
            {
                return list.Where(w => w.isparent == true).ToList() ?? null;
            }
            return null;
        }

        /// <summary>
        /// APP类型
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetAppTypeListItem()
        {
            var listItem = new List<SelectListItem>(){
                            new SelectListItem() { Value = "1", Text = "Android"},
                            new SelectListItem() { Value = "2", Text = "IOS" },
                            new SelectListItem() { Value = "3", Text = "移动端" },
                            new SelectListItem() { Value = "4", Text = "网站" },
                            new SelectListItem() { Value = "0", Text = "测试"}
            };
            return listItem;
        }

        public static string GetAppTypeListItemToCN(int value)
        {
            string valueCn = "";
            switch (value)
            {
                case 1:
                    valueCn = "Android";
                    break;
                case 2:
                    valueCn = "IOS";
                    break;
                case 3:
                    valueCn = "移动端";
                    break;
                case 4:
                    valueCn = "网站";
                    break;
                default:
                    valueCn = "测试";
                    break;
            }
            return valueCn;
        }

        /// <summary>
        /// 层级深度区域
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetAreaTypeListItem()
        {
            var listItem = new List<SelectListItem>(){
                            new SelectListItem() { Value = "1", Text = "大区域"},
                            new SelectListItem() { Value = "2", Text = "区域楼层" },
                            new SelectListItem() { Value = "3", Text = "区域小模块" },
                            new SelectListItem() { Value = "4", Text = "小模块城市" }
            };
            return listItem;
        }

        /// <summary>
        /// 层级深度区域枚举
        /// </summary>
        public enum AreaTypeEnum
        {
            大区域 = 1,
            区域楼层 = 2,
            区域小模块 = 3,
            小模块城市 = 4,
        }

        /// <summary>
        /// 获取城市信息
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public static List<TDAEntity.CityModel> CityItemsList(int parentId = 0)
        {
            List<TDAEntity.CityModel> citylist = null;
            if (parentId == 0)
                citylist = GetCityInfo.CreateGetCityInfo.GetCityList(0);
            else
                citylist = GetCityInfo.CreateGetCityInfo.GetCityList(parentId);
            return citylist;
        }

        /// <summary>
        /// 获取城市级节点
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> ParentCityList(int parentId = 0)
        {
            List<SelectListItem> listitem = new List<SelectListItem>();

            Dictionary<int, object> resultItems = Municipalities(parentId);
            if (resultItems != null)
            {
                listitem = (List<SelectListItem>)resultItems[0];
                return listitem;
            }

            var items = CityItemsList(parentId);
            if (items != null && items.Count > 0)
            {
                foreach (var item in items)
                {
                    listitem.Add(new SelectListItem() { Text = item.RegionName, Value = item.PKID.ToString() });
                }
            }
            return listitem;
        }

        /// <summary>
        /// 直辖市处理
        /// </summary>
        /// <param name="outCity"></param>
        /// <param name="outListItem"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private static Dictionary<int, object> Municipalities(int parentId = 0)
        {
            if (parentId == 1 || parentId == 2 || parentId == 19 || parentId == 20)
            {
                string regionName = string.Empty;
                Dictionary<int, object> dicItems = new Dictionary<int, object>();
                List<SelectListItem> listitem = new List<SelectListItem>();
                List<TDAEntity.CityModel> listCityItem = new List<TDAEntity.CityModel>();

                switch (parentId)
                {
                    case 1:
                        regionName = "上海市";
                        listitem.Add(new SelectListItem() { Text = regionName, Value = parentId.ToString() });
                        listCityItem.Add(new TDAEntity.CityModel() { PKID = parentId, RegionName = regionName, ParentID = 0 });
                        break;
                    case 2:
                        regionName = "北京市";
                        listitem.Add(new SelectListItem() { Text = regionName, Value = parentId.ToString() });
                        listCityItem.Add(new TDAEntity.CityModel() { PKID = parentId, RegionName = regionName, ParentID = 0 });
                        break;
                    case 19:
                        regionName = "天津市";
                        listitem.Add(new SelectListItem() { Text = regionName, Value = parentId.ToString() });
                        listCityItem.Add(new TDAEntity.CityModel() { PKID = parentId, RegionName = regionName, ParentID = 0 });
                        break;
                    case 20:
                        regionName = "重庆市";
                        listitem.Add(new SelectListItem() { Text = regionName, Value = parentId.ToString() });
                        listCityItem.Add(new TDAEntity.CityModel() { PKID = parentId, RegionName = regionName, ParentID = 0 });
                        break;
                    default:
                        break;
                }

                dicItems.Add(0, listitem);
                dicItems.Add(1, listCityItem);
                return dicItems;
            }
            return null;
        }
        #endregion

        public ActionResult SelTires()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CopyNewAppSetData(int id, int newid)
        {
            LoggerManager.InsertOplog(User.Identity.Name, ObjectType, newid, "复制数据" + id + "粘贴到" + newid);
            return Json(GetManager.CopyNewAppSetData(id, newid));
        }

        public ActionResult GetCity(int parentID)
        {
            return Json(HomePageConfigController.ParentCityList(parentID));
        }

        public ActionResult ReloadCache()
        {
            try
            {
                using (var client = new Tuhu.Service.Config.HomePageClient())
                {
                    client.RefreshSelectWxHomeConfigsByCityCache();
                    client.RefreshSelectWxHomeConfigsCache();
                    client.RefreshSelectWxHomeConfigsWithoutVersionCache();
                    client.RefreshSelectWxHomeConfigsByCityCache();
                    client.RefreshSelectWxWaterfallFlowConfigsCache();
                }
                return Content("1");
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }
       

    }
}
