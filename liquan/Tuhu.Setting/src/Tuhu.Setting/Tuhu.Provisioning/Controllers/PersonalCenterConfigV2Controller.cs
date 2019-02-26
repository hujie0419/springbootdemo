using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business;
using Newtonsoft.Json;
using Tuhu.Provisioning.Business.Logger;

namespace Tuhu.Provisioning.Controllers
{
    public class PersonalCenterConfigV2Controller : Controller
    {
        private static readonly Lazy<ExchangeCenterConfigManager> lazy = new Lazy<ExchangeCenterConfigManager>();

        private ExchangeCenterConfigManager ExchangeCenterConfigManager
        {
            get
            {
                return lazy.Value;
            }
        }
        /// <summary>
        /// 获取个人中心列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View(new SE_HomePageConfigManager().GetPersonlCenterConfig());
        }

        public PartialViewResult PreViewHome()
        {
            SE_HomePageConfigManager manger = new SE_HomePageConfigManager();

            var list = manger.GetPreViewModuleList();

            return PartialView(list);
        }

        /// <summary>
        /// 小工具
        /// </summary>
        /// <returns></returns>
        public ActionResult SmallToolbar()
        {
            return View();
        }

        /// <summary>
        /// 左导航来
        /// </summary>
        /// <returns></returns>
        public PartialViewResult NavigationList()
        {

            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();

            var homepagelist = manager.GetPersonalCenterConfig();
            foreach (var item in homepagelist)
            {
                var items = manager.GetHomePageModuleList(item.ID);
                item.ModuleItems = items;
                if (items != null)
                {
                    foreach (var helper in items)
                    {
                        helper.ModuleHelper = manager.GetHomePageModuleHelperList(helper.ID);
                    }
                }
            }

            return PartialView(homepagelist);
        }


        #region 个人中心基本操作
        /// <summary>
        /// 编辑个人中心信息
        /// </summary>
        /// <returns></returns>
        public ActionResult HomePageEdit(int id)
        {
            var obj = new SE_HomePageConfigManager().GetHomePageEntity(id);
            return View(obj == null ? new SE_HomePageConfig() : obj);
        }

        /// <summary>
        /// 删除个人中心配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteHomePage(int id)
        {
            SE_HomePageConfigManager manger = new SE_HomePageConfigManager();
            var before = manger.GetHomePageEntity(id);
            if (manger.DeleteHomePage(id))
            {
                AutoReloadCache();
                LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue = JsonConvert.SerializeObject(before), AfterValue = "", ChangeDatetime = DateTime.Now, ObjectID = id.ToString(), ObjectType = "PCLoger", Operation = "删除个人中心配置" + before.HomePageName });
                return Json(new { Status = 0 });
            }
            else
                return Json(new { Status = -1 });
        }



        #endregion


        #region 模块列表的基本操作
        /// <summary>
        /// 个人中心模块列表
        /// </summary>
        /// <param name="id">个人中心ID</param>
        /// <returns></returns>
        public ActionResult HomePageModuleList(int id)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            ViewBag.FKHomePage = id;
            return View(manager.GetHomePageModuleList(id).OrderBy(o => o.PriorityLevel));
        }


        /// <summary>
        /// 添加模块
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult HomePageModuleEdit(int FKHomePage, int ModeulType, int Module)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            ViewBag.ModuleTypeName = manager.GetPersonalModuleTypeName(ModeulType);
            if (Module == 0)
            {
                SE_HomePageModuleConfig model = new SE_HomePageModuleConfig();
                model.FKHomePage = FKHomePage;
                model.ModuleType = ModeulType;
                model.SpliteLine = "";
                model.PriorityLevel = manager.SelectHomePageModulePriorityLevel(FKHomePage) + 1;
                return View(model);
            }
            else
            {
                var model = manager.GetHomePageModuleEntity(Module);
                return View(model);
            }
        }


        /// <summary>
        /// 保存模块
        /// </summary>
        /// <param name="moduleString"></param>
        /// <param name="moduleProperty"></param>
        /// <returns></returns>
        public ActionResult HomePageModuleSave(SE_HomePageModuleConfig model)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();

            if (model.ID > 0)
            {
                //更新
                model.UpdateDateTime = DateTime.Now;
                var before = manager.GetHomePageModuleEntity(model.ID);
                manager.UpdatePageModule(model);
                AutoReloadCache();
                LoggerManager.InsertOplog(new ConfigHistory() { ObjectID = model.ID.ToString(), AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ObjectType = "PCMLoger", ChangeDatetime = DateTime.Now, Operation = "编辑个人中心模块" + model.ModuleName });

                return Json(2);
            }
            else
            {

                manager.AddHomePageModule(model);
                AutoReloadCache();
                LoggerManager.InsertOplog(new ConfigHistory() { ObjectID = model.ID.ToString(), AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = "", ObjectType = "PCMLoger", ChangeDatetime = DateTime.Now, Operation = "编辑个人中心模块" + model.ModuleName });

                return Json(1);
            }
        }

        public ActionResult HomePageModuleDelete(int id)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            var model = manager.GetHomePageModuleEntity(id);
            if (manager.DeleteModule(id, (int)model.FKHomePage))
            {
                AutoReloadCache();
                LoggerManager.InsertOplog(new ConfigHistory() { ObjectID = model.ID.ToString(), AfterValue = "", Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(model), ObjectType = "PCMLoger", ChangeDatetime = DateTime.Now, Operation = "删除个人中心模块" + model.ModuleName });
                return Json(1);
            }
            else
                return Json(0);
        }


        /// <summary>
        /// 更新模块的排序
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public ActionResult UpdateHomePageModulePriorityLevel(string content)
        {
            try
            {
                SE_HomePageConfigManager manger = new SE_HomePageConfigManager();
                IEnumerable<SE_HomePageModuleConfig> modules = JsonConvert.DeserializeObject<IEnumerable<SE_HomePageModuleConfig>>(content);
                //  var model = manger.GetHomePageModuleEntity(modules.FirstOrDefault().ID);
                //  var before = manger.GetHomePageModuleList(model.FKHomePage.Value);

                if (manger.UpdateHomePageModulePriorityLevel(modules))
                {
                    AutoReloadCache();
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(modules), ObjectID = "", BeforeValue = "", Author = User.Identity.Name, ObjectType = "PCMLoger", ChangeDatetime = DateTime.Now, Operation = "排序个人中心模块" });
                    return Json(1);
                }
                else
                    return Json(0);
            }
            catch (Exception em)
            {
                throw new Exception(em.Message);
            }
        }
        #endregion



        /// <summary>
        /// 自定义模块列表明细
        /// </summary>
        /// <param name="moduleID"></param>
        /// <param name="moduleHelperID"></param>
        /// <returns></returns>
        public ActionResult HomePageModuleDeatil(int? moduleID, int? moduleHelperID)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            ViewBag.ModuleID = moduleID;
            ViewBag.ModuleHelperID = moduleHelperID;
            if (moduleID != null)
            {
                var module = manager.GetHomePageModuleEntity(moduleID.Value);
                return View(module);
            }
            else
            {
                var module = manager.GetHomePageModuleHelperEntity(moduleHelperID.Value);

                return View(new SE_HomePageModuleConfig() { BgImageUrl = module.BgImageUrl, ModuleType = module.ModuleType });
            }




        }

        /// <summary>
        /// 自定义模块
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult HomePageModuleDeatilEdit(int id, string postion, int? moduleID, int? moduleHelperID)
        {
            ViewBag.Postion = postion;
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            int priortylevel = 1;
            var list = manager.GetHomePageContentList(moduleID, moduleHelperID);
            if (list != null && list.Count() > 0)
            {
                priortylevel += list.Count();
            }
            if (moduleID != null)
            {
                var module = manager.GetHomePageModuleEntity(moduleID.Value);
                ViewBag.ParentPriortylevel = module.PriorityLevel;

            }
            else
            {
                var module = manager.GetHomePageModuleHelperEntity(moduleHelperID.Value);
                ViewBag.ParentPriortylevel = module.PriorityLevel;
            }
            if (id == 0)
            {

                return View(new SE_HomePageModuleContentConfig() { PriorityLevel = priortylevel });
            }
            else
            {
                return View(manager.GetHomePageContentEntity(id));
            }

        }

        public PartialViewResult HomePageContentTable(int? moduleID, int? moduleHelperID)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();

            return PartialView(manager.GetHomePageContentList(moduleID, moduleHelperID));

        }

        /// <summary>
        /// 修改背景图片
        /// </summary>
        /// <param name="moduleID"></param>
        /// <param name="moduleHelperID"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public ActionResult UpdateBgImage(int? moduleID, int? moduleHelperID, string url)
        {
            SE_HomePageConfigManager mangaer = new SE_HomePageConfigManager();
            if (moduleID != null)
            {
                var model = mangaer.GetHomePageModuleEntity(moduleID.Value);
                if (model == null || model.ID == 0)
                    throw new Exception("模块不存在");
                var before = model.BgImageUrl;
                model.BgImageUrl = url;
                if (mangaer.UpdatePageModule(model))
                {
                    AutoReloadCache();
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = url, Author = User.Identity.Name, BeforeValue = before, ChangeDatetime = DateTime.Now, ObjectID = model.ID.ToString(), ObjectType = "PCMLoger", Operation = "更改模块背景图片" + model.ModuleName });
                    return Json(1);
                }
                else
                    return Json(0);

            }
            else
            {
                var model = mangaer.GetHomePageModuleHelperEntity(moduleHelperID.Value);
                if (model == null || model.ID == 0)
                    throw new Exception("模块不存在");
                var before = model.BgImageUrl;
                model.BgImageUrl = url;
                if (mangaer.UpdateHomePageModuleHelperBgImage(model))
                {
                    AutoReloadCache();
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = url, Author = User.Identity.Name, BeforeValue = before, ChangeDatetime = DateTime.Now, ObjectID = model.ID.ToString(), ObjectType = "PCMHLoger", Operation = "更改子模块背景图片" + model.ModuleName });
                    return Json(1);
                }
                else
                    return Json(0);
            }

        }

        /// <summary>
        /// 模块内容的
        /// </summary>
        /// <returns></returns>
        public ActionResult HomePageModuleContent(int id, int? moduleID, int? moduleHelperID)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            VIPAuthorizationRuleConfigManager vip = new VIPAuthorizationRuleConfigManager();
            int count = 0;
            ViewBag.VIPList = vip.GetVIPAuthorizationRuleConfigList("", int.MaxValue, 1, out count);
            int priortylevel = 1;
            var list = manager.GetHomePageContentList(moduleID, moduleHelperID);
            if (list != null && list.Count() > 0)
            {
                priortylevel += list.Count();
            }
            if (moduleID != null)
            {
                var module = manager.GetHomePageModuleEntity(moduleID.Value);
                ViewBag.ParentPriortylevel = module.PriorityLevel;

            }
            else
            {
                var module = manager.GetHomePageModuleHelperEntity(moduleHelperID.Value);
                ViewBag.ParentPriortylevel = module.PriorityLevel;
            }

            if (id == 0)
            {
                return View(new SE_HomePageModuleContentConfig() { PriorityLevel = priortylevel });
            }
            else
            {

                return View(manager.GetHomePageContentEntity(id));
            }
        }

        /// <summary>
        /// 模块内容列表
        /// </summary>
        /// <returns></returns>
        public ActionResult HomePageModuleContentList(int? moduleID, int? moduleHelperID)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            if (moduleID != null)
            {
                var module = manager.GetHomePageModuleEntity(moduleID.Value);
                ViewBag.ModuleName = module.ModuleName;
            }
            else
            {
                var module = manager.GetHomePageModuleHelperEntity(moduleHelperID.Value);
                ViewBag.ModuleName = module.ModuleName;
            }
            ViewBag.moduleID = moduleID;
            ViewBag.moduleHelperID = moduleHelperID;
            var list = manager.GetHomePageContentList(moduleID, moduleHelperID);
            return View(list);
        }


        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult HomePageModuleContentSave(SE_HomePageModuleContentConfig model)
        {
            int? pkid = null;
            var manager = new SE_HomePageConfigManager();

            if (model.FKHomePageModuleID != null)
            {
                var moduleModel = manager.GetHomePageModuleEntity(model.FKHomePageModuleID.Value);
                if (moduleModel != null)
                {
                    model.StartVersion = moduleModel.StartVersion;
                    model.EndVersion = moduleModel.EndVersion;
                    pkid = moduleModel.FKHomePage;
                }
            }

            if (model.ID == 0)
            {
                if (model.FKHomePageModuleHelperID != null)
                {
                    var moduleHelperModel = manager.GetHomePageModuleHelperEntity(model.FKHomePageModuleHelperID.Value);
                    if (moduleHelperModel != null)
                    {
                        model.StartVersion = moduleHelperModel.StartVersion;
                        model.EndVersion = moduleHelperModel.EndVersion;
                    }
                }

                model.ID = manager.AddHomePageContent(model, pkid.Value);

                if (model.ID <= 0)
                    return Json(0);

                AutoReloadCache();
                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = "", ChangeDatetime = DateTime.Now, ObjectID = model.ID.ToString(), ObjectType = "PCCLoger", Operation = "新增内容" + model.Title });
                return Json(model.ID);

            }

            var before = manager.GetHomePageContentEntity(model.ID);

            if (!manager.UpdateHomePageContent(model, pkid.Value))
                return Json(0);

            AutoReloadCache();
            LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ChangeDatetime = DateTime.Now, ObjectID = model.ID.ToString(), ObjectType = "PCCLoger", Operation = "编辑内容" + model.Title });
            return Json(1);

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteHomePageModuleContent(int id)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            var model = manager.GetHomePageContentEntity(id);

            var pageModel = manager.GetHomePageModuleEntity((int)model.FKHomePageModuleID);

            if (!manager.DeleteHomePageContent(model, pageModel))
                return Json(0);

            AutoReloadCache();
            LoggerManager.InsertOplog(new ConfigHistory() { Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(model), ChangeDatetime = DateTime.Now, ObjectID = model.ID.ToString(), ObjectType = "PCCLoger", Operation = "删除内容" + model.Title });
            return Json(1);

        }


        #region 参数关系对应
        /// <summary>
        /// 参数关系列表
        /// </summary>
        /// <returns></returns>
        public ActionResult WapParameterList()
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();

            return View(manager.GetWapParameterList());
        }

        public ActionResult WapParameterEdit()
        {
            return View();
        }

        public ActionResult WapParameterSave(SE_WapParameterConfig model)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            if (manager.AddWapParameter(model))
                return Json(1);
            else
                return Json(0);
        }


        public ActionResult DeleteWapParameter(int id)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            if (manager.DeleteWapParameter(id))
                return Json(1);
            else
                return Json(0);
        }
        #endregion



        /// <summary>
        /// 获取所属城市的父级
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult GetCityParent(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return Json(null);

            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            return Json(manager.GetCityGroupProvince(ids.TrimEnd(',')));
        }

        public ActionResult ReloadCache()
        {
            using (var client = new Tuhu.Service.Config.MemberMallClient())
            {
                var result = client.RefreshMemberMallConfigs();
                result.ThrowIfException(true);
                if (result.Success)
                {
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public static bool AutoReloadCache()
        {
            using (var client = new Tuhu.Service.Config.MemberMallClient())
            {
                var result = client.RefreshMemberMallConfigs();
                return result.Success;
            }
        }

        public ActionResult UpdateNextDateTime(string date)
        {
            DateTime datetime = Convert.ToDateTime(date);
            if (new SE_HomePageConfigManager().UpdateNextDateTime(datetime))
                return Json(1);
            else
                return Json(0);
        }


        public ActionResult HomePageLoger(string startDateTime = "", string endDateTime = "", string type = "")
        {
            if (string.IsNullOrWhiteSpace(type))
                return View();
            else
            {
                if (string.IsNullOrWhiteSpace(startDateTime))
                {
                    startDateTime = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                    endDateTime = DateTime.Now.ToString("yyyy-MM-dd");
                }
                LoggerManager manager = new LoggerManager();
                return View(manager.GetList(type, startDateTime, endDateTime));
            }
        }

        public ActionResult HomePageLogerDeatil(int id)
        {
            return View(LoggerManager.GetConfigHistory(id.ToString()));
        }





        /// <summary>
        /// 保存个人中心信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult HomePageSave(SE_HomePageConfig model)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();

            bool result = false;
            if (model.ID <= 0)
            {
                result = manager.AddPersonalCenter(model) > 0 ? true : false;
                AutoReloadCache();
                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = "", ObjectType = "PCLoger", ChangeDatetime = DateTime.Now, Operation = "新增个人中心" + model.HomePageName });
            }
            else
            {
                var before = manager.GetHomePageEntity(model.ID);
                result = manager.UpdateHomePage(model);
                AutoReloadCache();
                LoggerManager.InsertOplog(new ConfigHistory() { ObjectID = model.ID.ToString(), AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ObjectType = "PCLoger", ChangeDatetime = DateTime.Now, Operation = "编辑个人中心" + model.HomePageName });
            }

            if (result)
                return Json(new { Status = 0 });
            else
                return Json(new { Status = -1 });

        }
        /// <summary>
        /// 优惠券
        /// </summary>
        /// <returns></returns>
        public ActionResult CouponList(int? moduleID, int? moduleHelperID)
        {
            return View(ExchangeCenterConfigManager.GetExchangeCenterConfigList(moduleID.Value));
        }

        public ActionResult CouponEdit(int sort)
        {
            ViewBag.Sort = sort;
            return View(ExchangeCenterConfigManager.GetExchangeCenterConfigList());
        }

        public ActionResult DeletePersonalCenterCouponConfig(int id)
        {
            if (ExchangeCenterConfigManager.DeletePersonalCenterCouponConfig(id))
            {
                AutoReloadCache();
                LoggerManager.InsertOplog(new ConfigHistory() { ObjectID = id.ToString(), AfterValue = id.ToString(), Author = User.Identity.Name, BeforeValue = id.ToString(), ObjectType = "PCCPLoger", ChangeDatetime = DateTime.Now, Operation = "删除优惠券" + id });
                return Json(true);
            }
            else
            {
                return Json(false);
            }

        }

        public ActionResult InsertPersonalCenterCouponConfig(string data)
        {
            try
            {
                List<PersonalCenterCouponConfig> list = JsonConvert.DeserializeObject<List<PersonalCenterCouponConfig>>(data);
                foreach (var item in list)
                {
                    int i = ExchangeCenterConfigManager.InsertPersonalCenterCouponConfig(item);
                    AutoReloadCache();
                    LoggerManager.InsertOplog(new ConfigHistory() { ObjectID = i.ToString(), AfterValue = JsonConvert.SerializeObject(item), Author = User.Identity.Name, BeforeValue = "", ObjectType = "PCCPLoger", ChangeDatetime = DateTime.Now, Operation = "添加优惠券" + i });
                }
                return Json(true);
            }
            catch (Exception)
            {
                return Json(false);
                throw;
            }

        }

        public ActionResult EidtSort(PersonalCenterCouponConfig model)
        {
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateSort(PersonalCenterCouponConfig model)
        {
            if (ExchangeCenterConfigManager.UpdatePersonalCenterCouponConfig(model))
            {
                AutoReloadCache();
                LoggerManager.InsertOplog(new ConfigHistory() { ObjectID = model.Id.ToString(), AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = "", ObjectType = "PCCPLoger", ChangeDatetime = DateTime.Now, Operation = "编辑优惠券顺序" + model.Id });
                return Json(true);
            }
            else
            {
                return Json(false);
            }

        }
    }
}
