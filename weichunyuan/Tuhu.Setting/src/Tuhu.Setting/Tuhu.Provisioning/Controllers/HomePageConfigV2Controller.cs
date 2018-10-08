using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business;
using Newtonsoft.Json;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.Mapping;
using System.Linq.Expressions;

namespace Tuhu.Provisioning.Controllers
{
    public class HomePageConfigV2Controller : Controller
    {
        //
        // GET: /HomePageConfigV2/

        /// <summary>
        /// 获取首页列表
        /// </summary>
        /// <returns></returns>
#if !DEBUG
        [PowerManage]
#endif
        public ActionResult Index()
        {
            return View(new SE_HomePageConfigManager().GetHomePageList());
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

            var homepagelist = manager.GetHomePageList();
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

        #region 首页基本操作
        /// <summary>
        /// 编辑首页信息
        /// </summary>
        /// <returns></returns>
        public ActionResult HomePageEdit(int id)
        {
            var obj = new SE_HomePageConfigManager().GetHomePageEntity(id);
            return View(obj == null ? new SE_HomePageConfig() : obj);
        }

        /// <summary>
        /// 删除首页配置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteHomePage(int id)
        {
            SE_HomePageConfigManager manger = new SE_HomePageConfigManager();
            var before = manger.GetHomePageEntity(id);
            if (manger.DeleteHomePage(id))
            {
                LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue = JsonConvert.SerializeObject(before), AfterValue = "", ChangeDatetime = DateTime.Now, ObjectID = id.ToString(), ObjectType = "HPLoger", Operation = "删除首页配置" + before.HomePageName });
                return Json(new { Status = 0 });
            }
            else
                return Json(new { Status = -1 });
        }


        /// <summary>
        /// 保存首页信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult HomePageSave(SE_HomePageConfig model)
        {
            var manager = new SE_HomePageConfigManager();

            bool result;
            if (model.ID <= 0)
            {
                result = manager.AddHomePage(model) > 0;
                LoggerManager.InsertOplog(new ConfigHistory { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = "", ObjectType = "HPLoger", ChangeDatetime = DateTime.Now, Operation = "新增首页" + model.HomePageName });
            }
            else
            {
                var before = manager.GetHomePageEntity(model.ID);
                result = manager.UpdateHomePage(model);
                LoggerManager.InsertOplog(new ConfigHistory { ObjectID = model.ID.ToString(), AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ObjectType = "HPLoger", ChangeDatetime = DateTime.Now, Operation = "编辑首页" + model.HomePageName });
            }

            return Json(result ? new { Status = 0 } : new { Status = -1 });
        }
        #endregion


        #region 模块列表的基本操作
        /// <summary>
        /// 首页模块列表
        /// </summary>
        /// <param name="id">首页ID</param>
        /// <returns></returns>
        public ActionResult HomePageModuleList(int id)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();

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
            ViewBag.ModuleTypeName = manager.GetModuleTypeName(ModeulType);
            if (Module == 0)
            {
                SE_HomePageModuleConfig model = new SE_HomePageModuleConfig();
                model.FKHomePage = FKHomePage;
                model.ModuleType = ModeulType;
                model.SpliteLine = "";
                model.PriorityLevel = manager.SelectHomePageModulePriorityLevel(FKHomePage) + 1;
                model.UriCount = model.PriorityLevel + "_" + "更多";
                return View(model);
            }
            else
            {
                var model = manager.GetHomePageModuleEntity(Module);
                return View(model);
            }
        }

        /// <summary>
        /// 添加子模块
        /// </summary>
        /// <param name="FKHomePage"></param>
        /// <param name="ModeulType"></param>
        /// <param name="Module"></param>
        /// <param name="ParentID"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public ActionResult HomePageModuleChildEdit(int ModeulType, int id = 0, int parentModuleID = 0)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            ViewBag.ModuleTypeName = manager.GetModuleTypeName(ModeulType);
            if (id == 0)
            {
                var model = new SE_HomePageModuleHelperConfig();
                if (parentModuleID != 0)
                {
                    var parentModel = manager.GetHomePageModuleEntity(parentModuleID);
                    model.BgColor = parentModel.BgColor;
                    model.BgImageUrl = parentModel.BgImageUrl;
                    model.EndVersion = parentModel.EndVersion;
                    model.FKHomePageModuleID = parentModel.ID;
                    model.IsEnabled = parentModel.IsEnabled;
                    model.IsMore = parentModel.IsMore;
                    model.IsTag = parentModel.IsTag;
                    model.ModuleName = parentModel.ModuleName;
                    model.ModuleType = parentModel.ModuleType;
                    model.MoreUri = parentModel.MoreUri;
                    model.PriorityLevel = parentModel.PriorityLevel;
                    model.SpliteLine = parentModel.SpliteLine;
                    model.StartVersion = parentModel.StartVersion;
                    model.TagContent = parentModel.TagContent;
                    model.Title = parentModel.Title;
                    model.TitleColor = parentModel.TitleColor;
                    model.TitleImageUrl = parentModel.TitleImageUrl;
                    model.TitleBgColor = parentModel.TitleBgColor;
                    model.UriCount = parentModel.UriCount;
                    model.Pattern = parentModel.Pattern;
                    ViewBag.IsChannel = parentModel.IsMoreChannel;

                }
                return View(model);
            }
            else
            {
                var parentModel = manager.GetHomePageModuleEntity(parentModuleID);
                ViewBag.IsChannel = parentModel.IsMoreChannel;
                var model = manager.GetHomePageModuleHelperEntity(id);
                var list = manager.GetModuleHelperCityList(id);
                if (list != null)
                {
                    model.City = JsonConvert.SerializeObject(list);
                }
                return View(model);
            }

        }

        /// <summary>
        /// 保存附属模块
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult HomePageModuleChildSave(SE_HomePageModuleHelperConfig model)
        {
            var manager = new SE_HomePageConfigManager();
            var module = manager.GetHomePageModuleEntity(model.FKHomePageModuleID.Value);
            model.PriorityLevel = module.PriorityLevel;
            switch (model.ID)
            {
                case 0 when manager.AddHomePageModuleHelper(model, module.FKHomePage.Value):
                    LoggerManager.InsertOplog(new ConfigHistory { ObjectID = model.ID.ToString(), AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = "", ObjectType = "HPMHLoger", ChangeDatetime = DateTime.Now, Operation = "新增首页子模块" + model.ModuleName });
                    return Json(1);
                case 0:
                    return Json(0);
                default:
                    var before = manager.GetHomePageModuleHelperEntity(model.ID);
                    if (manager.UpdateHomePageModuleHelper(model, module.FKHomePage.Value))
                    {
                        LoggerManager.InsertOplog(new ConfigHistory { ObjectID = model.ID.ToString(), AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ObjectType = "HPMHLoger", ChangeDatetime = DateTime.Now, Operation = "编辑首页子模块" + model.ModuleName });
                        return Json(1);
                    }
                    else
                        return Json(0);
            }

        }


        /// <summary>
        /// 保存模块
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult HomePageModuleSave(SE_HomePageModuleConfig model)
        {
            var manager = new SE_HomePageConfigManager();

            if (model.ID > 0)
            {
                //更新
                model.UpdateDateTime = DateTime.Now;
                var before = manager.GetHomePageModuleEntity(model.ID);
                manager.UpdatePageModule(model);
                //同时更新子模块的所有拼图类型
                if (model.ModuleType == 30)
                {
                    var helpModules = manager.GetSE_HomePageModuleHelperConfigsByFkHomePageId(model.ID);
                    helpModules.ForEach(helpModule =>
                    {
                        helpModule.Pattern = model.Pattern;
                        manager.UpdateHomePageModuleHelperPattern(helpModule);
                    });
                }
                LoggerManager.InsertOplog(new ConfigHistory() { ObjectID = model.ID.ToString(), AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ObjectType = "HPMLoger", ChangeDatetime = DateTime.Now, Operation = "编辑首页模块" + model.ModuleName });
                return Json(2);
            }

            manager.AddHomePageModule(model);
            LoggerManager.InsertOplog(new ConfigHistory() { ObjectID = model.ID.ToString(), AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = "", ObjectType = "HPMLoger", ChangeDatetime = DateTime.Now, Operation = "编辑首页模块" + model.ModuleName });
            return Json(1);
        }

        public ActionResult HomePageModuleDelete(int id)
        {
            var manager = new SE_HomePageConfigManager();
            var model = manager.GetHomePageModuleEntity(id);
            if (!manager.DeleteModule(id, (int)model.FKHomePage))
                return Json(0);
            LoggerManager.InsertOplog(new ConfigHistory() { ObjectID = model.ID.ToString(), AfterValue = "", Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(model), ObjectType = "HPMLoger", ChangeDatetime = DateTime.Now, Operation = "删除首页模块" + model.ModuleName });
            return Json(1);

        }

        /// <summary>
        /// 附属模块列表
        /// </summary>
        /// <param name="moduleID"></param>
        /// <returns></returns>
        public ActionResult HomePageModuleChildList(int moduleID)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            var module = manager.GetHomePageModuleEntity(moduleID);
            ViewBag.ModuleID = moduleID;
            ViewBag.ModuleType = module.ModuleType;
            var list = manager.GetHomePageModuleHelperList(moduleID);
            if (list != null && list.Count() > 0)
                ViewBag.ModuleTypeName = manager.GetModuleTypeName(list.FirstOrDefault().ModuleType.Value);
            else
                ViewBag.ModuleTypeName = "";
            return View(list);
        }


        public ActionResult DeleteHomePageModuleHelper(int moduleHelperID)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            var model = manager.GetHomePageModuleHelperEntity(moduleHelperID);
            if (manager.DeleteHomePageModuleHelper(moduleHelperID))
            {
                LoggerManager.InsertOplog(new ConfigHistory() { ObjectID = moduleHelperID.ToString(), BeforeValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, ObjectType = "HPMHLoger", ChangeDatetime = DateTime.Now, Operation = "删除首页子模块" + model.ModuleName });
                return Json(1);
            }
            else
                return Json(0);
        }

        ///// <summary>
        ///// 获取子模块列表
        ///// </summary>
        ///// <param name="parentModuleID"></param>
        ///// <returns></returns>
        //public ActionResult GetHomePageModuleList(int parentModuleID)
        //{
        //    SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
        //    var list = manager.GetHomePageModuleChildList(parentModuleID);

        //    return Json(list.Select(o => new
        //    {
        //        o.ModuleName,
        //        o.ID

        //    }));
        //}


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
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = url, Author = User.Identity.Name, BeforeValue = before, ChangeDatetime = DateTime.Now, ObjectID = model.ID.ToString(), ObjectType = "HPMLoger", Operation = "更改模块背景图片" + model.ModuleName });
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
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = url, Author = User.Identity.Name, BeforeValue = before, ChangeDatetime = DateTime.Now, ObjectID = model.ID.ToString(), ObjectType = "HPMHLoger", Operation = "更改子模块背景图片" + model.ModuleName });
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
            ViewBag.isReadonly = true;
            ViewBag.IsHideDate = false;
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
                ViewBag.ModuleType = module.ModuleType;
                ViewBag.Pattern = module.Pattern;
                if (module.ModuleType == 30)
                {
                    ViewBag.isReadonly = false;
                    if (list != null)
                        priortylevel = list.GroupBy(o => o.PriorityLevel).Count() + 1;
                }


            }
            else
            {
                var module = manager.GetHomePageModuleHelperEntity(moduleHelperID.Value);
                ViewBag.ParentPriortylevel = module.PriorityLevel;
                ViewBag.ModuleType = module.ModuleType;
                if (module.ModuleType == 30)
                {
                    ViewBag.isReadonly = false;
                    ViewBag.IsHideDate = true;
                    if (list != null)
                        priortylevel = list.GroupBy(o => o.PriorityLevel).Count() + 1;
                }
            }
            var model = new SE_HomePageModuleContentConfig();
            if (id == 0)
            {
                model = new SE_HomePageModuleContentConfig() { PriorityLevel = priortylevel };
            }
            else
            {
                model = manager.GetHomePageContentEntity(id);
            }
            var deviceBrands = manager.SelectDeviceBrands();
            List<SelectListItem> sdeviceBrands = new List<SelectListItem>()
            {
              new SelectListItem()
                {
                    Text = "请选择",
                    Value = ""
                }
        };
            foreach (var item in deviceBrands)
            {
                sdeviceBrands.Add(new SelectListItem()
                {
                    Text = item.DeviceBrand,
                    Value = item.Pkid.ToString()
                });
            }

            foreach (var item in sdeviceBrands)
            {
                if (item.Value == model.DeviceBrand)
                {
                    item.Selected = true;
                }
            }
            ViewBag.ChannelDictionary = new Business.DownloadApp.DownloadAppManager().QueryNoticeChannel();
            ViewBag.SelectItems = sdeviceBrands;
            return View(model);

        }

        public JsonResult SelectMobileModels(int marketId)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            return Json(manager.SelectMobileModels(marketId));
        }
        /// <summary>
        /// 模块内容列表
        /// </summary>
        /// <returns></returns>
        public ActionResult HomePageModuleContentList(int? moduleID, int? moduleHelperID)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            ViewBag.isHide = true;
            ViewBag.isHideData = false;
            if (moduleID != null)
            {
                var module = manager.GetHomePageModuleEntity(moduleID.Value);
                ViewBag.ModuleName = module.ModuleName;
                if (module.ModuleType == 30)
                {
                    ViewBag.isHide = false;
                }

            }
            else
            {
                var module = manager.GetHomePageModuleHelperEntity(moduleHelperID.Value);
                ViewBag.ModuleName = module.ModuleName;
                if (module.ModuleType == 30)
                    ViewBag.isHide = false;
                ViewBag.isHideData = true;
            }
            ViewBag.moduleID = moduleID;
            ViewBag.moduleHelperID = moduleHelperID;
            var list = manager.GetHomePageContentList(moduleID, moduleHelperID);
            if (ViewBag.isHide == false)
                list = list.OrderBy(o => o.PriorityLevel).ThenBy(o => o.StartDateTime);

            return View(list);
        }

        /// <summary>
        /// 保存 模块下的内容信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult HomePageModuleContentSave(SE_HomePageModuleContentConfig model)
        {
            int? pkid = null;
            int? moduleType = null;
            var manager = new SE_HomePageConfigManager();
            if (model.ID == 0)
            {
                if (model.FKHomePageModuleID != null)
                {
                    var moduleModel = manager.GetHomePageModuleEntity(model.FKHomePageModuleID.Value);
                    if (moduleModel != null)
                    {
                        model.StartVersion = moduleModel.StartVersion;
                        model.EndVersion = moduleModel.EndVersion;
                        moduleType = moduleModel.ModuleType;
                        pkid = moduleModel.FKHomePage;
                    }
                }

                if (model.FKHomePageModuleHelperID != null)
                {
                    var moduleHelperModel = manager.GetHomePageModuleHelperEntity(model.FKHomePageModuleHelperID.Value);
                    if (moduleHelperModel != null)
                    {
                        model.StartVersion = moduleHelperModel.StartVersion;
                        model.EndVersion = moduleHelperModel.EndVersion;
                        moduleType = moduleHelperModel.ModuleType;

                        if (moduleHelperModel.FKHomePageModuleID != null)
                        {
                            var moduleModel = manager.GetHomePageModuleEntity(moduleHelperModel.FKHomePageModuleID.Value);
                            if (moduleModel != null)
                            {
                                pkid = moduleModel.FKHomePage;
                            }
                        }
                    }
                }


                if (moduleType != null && moduleType == 30)
                {
                    if (!manager.ContentDateTimeValidate(model))
                        return Json(-1);
                }

                model.ID = manager.AddHomePageContent(model, pkid.Value);

                if (model.ID <= 0)
                    return Json(0);

                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = "", ChangeDatetime = DateTime.Now, ObjectID = model.ID.ToString(), ObjectType = "HPCLoger", Operation = "新增内容" + model.Title });
                return Json(model.ID);

            }

            var before = manager.GetHomePageContentEntity(model.ID);

            if (model.FKHomePageModuleID != null)
            {
                var moduleModel = manager.GetHomePageModuleEntity(model.FKHomePageModuleID.Value);
                if (moduleModel != null)
                {
                    moduleType = moduleModel.ModuleType;
                    pkid = moduleModel.FKHomePage;
                }
            }

            if (model.FKHomePageModuleHelperID != null)
            {
                var moduleHelperModel = manager.GetHomePageModuleHelperEntity(model.FKHomePageModuleHelperID.Value);
                if (moduleHelperModel != null)
                {
                    moduleType = moduleHelperModel.ModuleType;
                    if (moduleHelperModel.FKHomePageModuleID != null)
                    {
                        var moduleModel = manager.GetHomePageModuleEntity(moduleHelperModel.FKHomePageModuleID.Value);
                        if (moduleModel != null)
                        {
                            pkid = moduleModel.FKHomePage;
                        }
                    }
                }
            }

            if (moduleType != null && moduleType == 30)
            {
                if (!manager.ContentDateTimeValidate(model))
                    return Json(-1);
            }

            model.UpdateDateTime = DateTime.Now;

            if (!manager.UpdateHomePageContent(model, pkid.Value))
                return Json(0);

            LoggerManager.InsertOplog(new ConfigHistory { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ChangeDatetime = DateTime.Now, ObjectID = model.ID.ToString(), ObjectType = "HPCLoger", Operation = "编辑内容" + model.Title });
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

            if (manager.DeleteHomePageContent(id, (int)pageModel.FKHomePage))
            {
                LoggerManager.InsertOplog(new ConfigHistory() { Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(model), ChangeDatetime = DateTime.Now, ObjectID = model.ID.ToString(), ObjectType = "HPCLoger", Operation = "删除内容" + model.Title });
                return Json(1);
            }
            else
                return Json(0);
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

        #region 闪购配置
        public ActionResult FlashScreenList()
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            var list = manager.GetFlashScreenList(1);
            return View(list);
        }

        public ActionResult ChannelFlashScreenList()
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            var list = manager.GetFlashScreenList(2);
            return View(list);
        }


        public ActionResult FlashScreenEdit(int id = 0, int type = 1)
        {
            ViewBag.ChannelDictionary = null;

            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            if (id == 0)
            {
                if (type == 2)
                {
                    ViewBag.ChannelDictionary = new Business.DownloadApp.DownloadAppManager().QueryNoticeChannel();
                }
                return View(new SE_FlashScreenConfig() { Type = type });
            }
            else
            {
                var entity = manager.GetFlashScreenEntity(id);
                if (entity.Type == 2)
                {
                    ViewBag.ChannelDictionary = new Business.DownloadApp.DownloadAppManager().QueryNoticeChannel();
                }
                if (entity.Type == 0)
                    entity.Type = 1;
                return View(entity);
            }
        }

        public ActionResult FlashScreenSave(SE_FlashScreenConfig model)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            if (model.ID == 0)
            {
                model.CreateDateTime = DateTime.Now;
                model.UpdateDateTime = DateTime.Now;
                if (manager.AddFlashScreenEntity(model))
                {
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = "", ChangeDatetime = DateTime.Now, ObjectID = model.ID.ToString(), ObjectType = "HPFSLoger", Operation = "新增闪屏配置" + model.Name });
                    return Json(1);
                }
                else
                    return Json(0);
            }
            else
            {
                model.UpdateDateTime = DateTime.Now;
                var before = manager.GetFlashScreenEntity(model.ID);
                if (manager.UpdateFlashScreenEntity(model))
                {
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ChangeDatetime = DateTime.Now, ObjectID = model.ID.ToString(), ObjectType = "HPFSLoger", Operation = "编辑闪屏配置" + model.Name });

                    return Json(1);
                }
                else
                    return Json(0);
            }
        }

        public ActionResult FlashScreenDelete(int id)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            var before = manager.GetFlashScreenEntity(id);
            if (manager.DeleteFlashScreen(id))
            {
                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = "", Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ChangeDatetime = DateTime.Now, ObjectID = before.ID.ToString(), ObjectType = "HPFSLoger", Operation = "删除闪屏配置" + before.Name });
                return Json(1);
            }
            else
                return Json(0);
        }

        #endregion


        #region 瀑布流
        public ActionResult FlowList()
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            return View(manager.GetFlowList("1"));
        }
        public ActionResult FlowEdit(int id, string type)
        {

            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            if (id == 0)
            {
                if (string.IsNullOrWhiteSpace(type))
                    type = "1";
                return View(new SE_HomePageFlowConfig() { Type = type.ToString() });
            }
            else
                return View(manager.GetFlowEntity(id));
        }

        public ActionResult FlowSave(SE_HomePageFlowConfig model)
        {
            SE_HomePageConfigManager manger = new SE_HomePageConfigManager();
            if (model.ID == 0)
            {
                if (string.IsNullOrWhiteSpace(model.UriCount))
                {
                    model.UriCount = "waterfall_" + model.PriorityLevel + "_" + model.Title;
                }
                if (manger.AddFlow(model))
                {
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = "", ChangeDatetime = DateTime.Now, ObjectID = model.ID.ToString(), ObjectType = "HPFLoger", Operation = "新增瀑布流配置" + model.Title });
                    return Json(1);
                }
                else
                    return Json(0);
            }
            else
            {
                var before = manger.GetFlowEntity(model.ID);
                if (manger.UpdateFlow(model))
                {
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ChangeDatetime = DateTime.Now, ObjectID = model.ID.ToString(), ObjectType = "HPFLoger", Operation = "编辑瀑布流配置" + model.Title });
                    return Json(1);
                }
                else
                    return Json(0);
            }
        }

        public ActionResult DeleteFlow(int id)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            var before = manager.GetFlowEntity(id);
            if (manager.DeleteFlow(id))
            {
                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = "", Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ChangeDatetime = DateTime.Now, ObjectID = id.ToString(), ObjectType = "HPFLoger", Operation = "删除瀑布流配置" + before.Title });
                return Json(1);
            }
            else
                return Json(0);
        }
        public ActionResult FlowProduct(int flowID)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            if (flowID <= 0)
                return View();
            ViewBag.FlowID = flowID;
            return View(manager.GetFlowProductList(flowID));
        }


        public ActionResult DeleteFlowProduct(int id)
        {
            SE_HomePageConfigManager manger = new SE_HomePageConfigManager();

            if (manger.DeleteFlowProduct(id))
            {
                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = "", Author = User.Identity.Name, BeforeValue = id.ToString(), ChangeDatetime = DateTime.Now, ObjectID = id.ToString(), ObjectType = "HPPLoger", Operation = "删除瀑布流产品配置" });

                return Json(1);
            }
            else
                return Json(0);
        }


        public ActionResult FlowProductSave(SE_HomePageFlowProductConfig model)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            if (manager.AddFlowProduct(model))
            {
                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = "", ChangeDatetime = DateTime.Now, ObjectID = model.ID.ToString(), ObjectType = "HPPLoger", Operation = "新增瀑布流产品配置" });
                return Json(1);
            }
            else
                return Json(0);
        }

        #endregion

        #region 爱车档案
        public ActionResult LoveCar()
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            return View(manager.GetFlowList("2"));
        }
        #endregion

        public ActionResult LoveCarBanner()
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            return View(manager.GetFlowList("5"));
        }


        public ActionResult CarTypeApprove()
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            return View(manager.GetFlowList("3"));
        }


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


        public ActionResult HomePageMenuList(int id)
        {
            SE_HomePageConfigManager manger = new SE_HomePageConfigManager();
            ViewBag.FKMenuListID = id;
            return View(manger.GetHomePageMenuList(id));
        }


        public ActionResult HomePageMenuEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new SE_HomePageMenuConfig() { MenuType = "" });
            }
            else
            {
                SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
                return View(manager.GetHomePageMenuEntity(id));
            }

        }


        public ActionResult HomePageMenuSave(SE_HomePageMenuConfig model)
        {
            SE_HomePageConfigManager manger = new SE_HomePageConfigManager();
            if (model.ID == 0)
            {
                if (manger.AddHomePageMenu(model))
                {
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = "", ChangeDatetime = DateTime.Now, ObjectID = model.ID.ToString(), ObjectType = "HPMULoger", Operation = "新增菜单配置" + model.MenuName });

                    return Json(1);
                }
                else
                    return Json(0);
            }
            else
            {
                var before = manger.GetHomePageMenuEntity(model.ID);
                if (manger.UpdateHomePageMenu(model))
                {
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ChangeDatetime = DateTime.Now, ObjectID = model.ID.ToString(), ObjectType = "HPMULoger", Operation = "编辑菜单配置" + model.MenuName });

                    return Json(1);
                }
                else
                    return Json(0);
            }
        }

        public ActionResult HomePageMenuDelete(int id)
        {
            SE_HomePageConfigManager manger = new SE_HomePageConfigManager();
            var before = manger.GetHomePageMenuEntity(id);
            if (manger.DeleteHomePageMenu(id))
            {
                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = "", Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ChangeDatetime = DateTime.Now, ObjectID = before.ID.ToString(), ObjectType = "HPMULoger", Operation = "删除菜单配置" + before.MenuName });

                return Json(1);
            }
            else
                return Json(0);
        }

        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <returns></returns>
        public ActionResult ReloadCache()
        {
            using (var client = new Service.Config.HomePageClient())
            {
                var result = client.RefreshHomeConfigs();
                result.ThrowIfException(true);
                return Json(result.Success ? 1 : 0, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult HomePageMenuGroup()
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            ViewBag.UpdateDateTime = manager.GetNextDateTime();
            return View(manager.GetHomePageMenuListList());
        }

        public ActionResult HomePageMenuGroupEdit(int id = 0)
        {
            if (id == 0)
                return View(new SE_HomePageMenuListConfig());
            else
                return View(new SE_HomePageConfigManager().GetHomePageMenuListEntity(id));
        }

        public ActionResult HomePageMenuGroupSave(SE_HomePageMenuListConfig model)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            if (model.ID == 0)
            {
                //if (manager.ExistsHomePageMenuList(model.StartDateTime.Value, model.EndDateTime.Value))
                //    return Json(2);

                if (manager.AddHomePageMenuList(model))
                {
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = "", ChangeDatetime = DateTime.Now, ObjectID = model.ID.ToString(), ObjectType = "HPMUGLoger", Operation = "新增菜单组配置" + model.Name });
                    return Json(1);
                }
                else
                    return Json(0);
            }
            else
            {
                var before = manager.GetHomePageMenuListEntity(model.ID);
                if (manager.UpdateHomePageMenuList(model))
                {
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ChangeDatetime = DateTime.Now, ObjectID = model.ID.ToString(), ObjectType = "HPMUGLoger", Operation = "编辑菜单组配置" + model.Name });

                    return Json(1);
                }
                else
                    return Json(0);
            }
        }

        public ActionResult HomePageMenuGroupDelete(int id)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            var before = manager.GetHomePageMenuListEntity(id);
            if (manager.DeleteHomePageMenuList(id))
            {
                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = "", Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ChangeDatetime = DateTime.Now, ObjectID = before.ID.ToString(), ObjectType = "HPMUGLoger", Operation = "删除菜单组配置" + before.Name });

                return Json(1);
            }
            else
                return Json(0);

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


        public ActionResult AnimationContentList(int? moduleID, int? moduleHelperID)
        {
            ViewBag.moduleID = moduleID;
            ViewBag.moduleHelperID = moduleHelperID;
            return View(new SE_HomePageConfigManager().GetHomePageAnimationContentList(moduleID, moduleHelperID));
        }

        public ActionResult AnimationContentSave(SE_HomePageAnimationContent model)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            if (model.ID > 0)
            {
                var before = manager.GetHomePageAnimationContentEntity(model.ID);
                model.UpdateDateTime = DateTime.Now;
                if (manager.UpdateHomePageAnimationContent(model))
                {
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), ChangeDatetime = DateTime.Now, ObjectType = "HPAC", Operation = "编辑动画内容配置" + model.Name });
                    return Json(1);
                }
                else
                    return Json(0);
            }
            else
            {
                if (manager.AddHomePageAnimationContent(model))
                {
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = JsonConvert.SerializeObject(model), Author = User.Identity.Name, BeforeValue = null, ChangeDatetime = DateTime.Now, ObjectType = "HPAC", Operation = "新增动画内容配置" + model.Name });
                    return Json(1);
                }
                else
                    return Json(0);
            }
        }

        public ActionResult AnimationContent(int id = 0, int? moduleID = null, int? moduleHelperID = null)
        {
            if (moduleID != null)
            {
                ViewBag.ParentPriortylevel = moduleID.Value;
            }
            if (moduleHelperID != null)
            {
                ViewBag.ParentPriortylevel = moduleHelperID.Value;
            }
            var manager = new SE_HomePageConfigManager();
            var list = manager.GetHomePageAnimationContentList(moduleID, moduleHelperID);

            if (id == 0)
            {
                var entity = new SE_HomePageAnimationContent();
                if (list != null && list.Count() > 0)
                    entity.PriorityLevel = list.Count() + 1;
                else
                    entity.PriorityLevel = 1;
                return View(entity);
            }
            else
            {
                var model = manager.GetHomePageAnimationContentEntity(id);
                return View(model);
            }
        }


        public ActionResult DeleteAnimationContent(int id)
        {
            if (new SE_HomePageConfigManager().DeleteHomePageAnimationContent(id))
                return Json(1);
            else
                return Json(0);
        }


        public ActionResult SelectPuzzle(int? moduleID, int? moduleHelperID)
        {
            return View();
        }

        public ActionResult GetPuzzleJson(int? moduleID, int? moduleHelperID)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            //1 四平分 2 挑两头 3 六大块 4 两大两小 5 三大块
            int puzzleType = 0;
            var list = new List<SE_HomePageModuleContentConfig>();
            if (!moduleID.HasValue)
            {
                if (moduleHelperID != null)
                {
                    var modulehelper = manager.GetHomePageModuleHelperEntity(moduleHelperID.Value);
                    var module = manager.GetHomePageModuleEntity(modulehelper.FKHomePageModuleID.Value);
                    if (modulehelper.Pattern != module.Pattern)
                    {
                        return Content(JsonConvert.SerializeObject(new
                        {
                            Code = 0,
                            Msg = "不允许与父模块配置的拼图类型不一致，请重新选择"
                        }));
                    }
                    puzzleType = modulehelper.Pattern.Value;
                    var moduleContents = manager.GetHomePageContentList(module.ID, null).Where(o => o.IsEnabled);
                    var moduleHelperContents = manager.GetHomePageContentList(null, moduleHelperID).Where(o => o.IsEnabled);
                    var query = (from m1 in moduleContents
                                 join m2 in moduleHelperContents on m1.PriorityLevel equals m2.PriorityLevel into temp
                                 from m2 in temp.DefaultIfEmpty()
                                 select new
                                 {
                                     m1,
                                     m2
                                 }).ToList();
                    foreach (var item in query)
                    {
                        if (item.m2 == null)
                        {
                            list.Add(item.m1);
                        }
                        else
                        {
                            item.m2.StartDateTime = item.m1.StartDateTime;
                            item.m2.EndDateTime = item.m1.EndDateTime;
                            list.Add(item.m2);
                        }
                    }

                }
            }
            else
            {
                if (moduleID != null)
                    puzzleType = manager.GetHomePageModuleEntity(moduleID.Value).Pattern.Value;
                list = manager.GetHomePageContentList(moduleID, moduleHelperID).Where(o => o.IsEnabled).ToList();
            }
            if (!list.Any())
                return Content(JsonConvert.SerializeObject(new
                {
                    Code = 0,
                    Msg = "没有配置内容"
                }));

            int count = list.GroupBy(o => o.PriorityLevel).Count();

            #region 判断配置内容是否完整
            string message = "";
            bool isFull = true;
            if (puzzleType == 1)
                if (count != 4)
                {
                    isFull = false;
                    message = count > 4 ? "配置内容超过4个，请删除或者禁用多余的" : "配置内容少于4个,请增加内容";
                }

            if (puzzleType == 2)
                if (count != 3)
                {
                    isFull = false;
                    message = count > 3 ? "配置内容超过3个，请删除或者禁用多余的" : "配置内容少于3个,请增加内容";
                }

            if (puzzleType == 3)
                if (count != 3)
                {
                    isFull = false;
                    message = count > 3 ? "配置内容超过3个，请删除或者禁用多余的" : "配置内容少于3个,请增加内容";
                }

            if (puzzleType == 4)
                if (count != 4)
                {
                    isFull = false;
                    message = count > 4 ? "配置内容超过4个，请删除或者禁用多余的" : "配置内容少于4个,请增加内容";
                }

            if (puzzleType == 5)
                if (count != 3)
                {
                    isFull = false;
                    message = count > 3 ? "配置内容超过3个，请删除或者禁用多余的" : "配置内容少于3个,请增加内容";
                }
            #endregion

            if (!isFull)
                return Content(JsonConvert.SerializeObject(new
                {
                    Code = 0,
                    Msg = "配置内容不符合要求"
                }));

            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (puzzleType == 1 || puzzleType == 4)
                dic = manager.GetPuzzle4(puzzleType, list);
            if (puzzleType == 2 || puzzleType == 5 || puzzleType == 3)
                dic = manager.GetPuzzle3(puzzleType, list);
            //if (puzzleType == 3)
            //    dic = manager.GetPuzzle6(puzzleType, list);
            if (dic.Count <= 0)
                return Content(JsonConvert.SerializeObject(new
                {
                    Code = 0,
                    Msg = "当前配置的各个时间段没有共同的，时间段，请重新配置！"
                }));

            return Content(JsonConvert.SerializeObject(dic));
        }

        public ActionResult SavePuzzle(IEnumerable<SE_HomePageContentPuzzle> entities, int type)
        {
            try
            {
                //不再支持多种组合的拼图了，简单处理
                var manager = new SE_HomePageConfigManager();
                var moduleId = entities.ToList()?.FirstOrDefault()?.FKModuleID;
                var helperResult = true;
                if (moduleId.HasValue)
                {
                    helperResult = GenerateChildHomePageContentPuzzleList(moduleId.Value);
                }
                //if (entities.Any())
                //{
                //    var dic = manager.GetPuzzleList(entities.ToList()?.FirstOrDefault()?.FKModuleID, entities.ToList()?.FirstOrDefault()?.FKModuleHelperID);
                //    if (dic != null)
                //    {
                //        foreach (var item in entities)
                //        {
                //            var entity =Newtonsoft.Json.Linq.JObject.Parse(JsonConvert.SerializeObject(dic[item.GroupID]));
                //            item.StartDateTime = Convert.ToDateTime(entity["StartDateTime"]);
                //            item.EndDateTime = Convert.ToDateTime(entity["EndDateTime"]);
                //        }
                //    }
                //}
                if (helperResult)
                {
                    if (manager.AddPuzzleConfig(entities, type))
                        return Content(JsonConvert.SerializeObject(new
                        {
                            Code = 1
                        }));

                    return Content(JsonConvert.SerializeObject(new
                    {
                        Code = 0
                    }));
                }
                else
                {
                    return Content(JsonConvert.SerializeObject(new
                    {
                        Code = 0
                    }));
                }
            }
            catch (Exception em)
            {
                throw em;
            }
        }

        private bool GenerateChildHomePageContentPuzzleList(int moduleId)
        {
            var manager = new SE_HomePageConfigManager();
            var result = true;
            //1,根据父模块id,找到配置的所有拼图子模块
            var helps = manager.GetSE_HomePageModuleHelperConfigsByFkHomePageId(moduleId);
            //2,根据配置的父子模块找到已经配置的所有内容模块
            var module = manager.GetHomePageModuleEntity(moduleId);
            foreach (var help in helps)
            {
                var list = new List<SE_HomePageModuleContentConfig>();
                var moduleContents = manager.GetHomePageContentList(module.ID, null).Where(o => o.IsEnabled);
                var moduleHelperContents = manager.GetHomePageContentList(null, help.ID).Where(o => o.IsEnabled);
                if (!moduleHelperContents.Any())
                    return true;
                var query = (from m1 in moduleContents
                             join m2 in moduleHelperContents on m1.PriorityLevel equals m2.PriorityLevel into temp
                             from m2 in temp.DefaultIfEmpty()
                             select new
                             {
                                 m1,
                                 m2
                             }).ToList();
                foreach (var item in query)
                {
                    if (item.m2 == null)
                    {
                        list.Add(item.m1);
                    }
                    else
                    {
                        item.m2.StartDateTime = item.m1.StartDateTime;
                        item.m2.EndDateTime = item.m1.EndDateTime;
                        list.Add(item.m2);
                    }
                }
                var puzzleType = module.Pattern.Value;
                var dics = new Dictionary<string, object>();
                var listPuzzles = new List<SE_HomePageContentPuzzle>();
                if (puzzleType == 1 || puzzleType == 4)
                {
                    dics = manager.GetPuzzle4(puzzleType, list);
                }

                if (puzzleType == 2 || puzzleType == 5 || puzzleType == 3)
                {
                    dics = manager.GetPuzzle3(puzzleType, list);
                }

                //3,生成大图，保存
                var group = dics.Keys.FirstOrDefault();
                var strDics = JsonConvert.SerializeObject(dics.Values);
                var dicmodel = JsonConvert.DeserializeObject<List<DicPuzzleSerializeModel>>(strDics).FirstOrDefault();
                listPuzzles.Add(new SE_HomePageContentPuzzle
                {
                    StartDateTime = dicmodel.StartDateTime,
                    EndDateTime = dicmodel.EndDateTime,
                    GroupID = group,
                    FKModuleHelperID = help.ID,
                    FKID = dicmodel.First.Id,
                    PriorityLevel = dicmodel.First.PriorityLevel,
                    ImageUrl = dicmodel.First.ImgUrl
                });
                listPuzzles.Add(new SE_HomePageContentPuzzle
                {
                    StartDateTime = dicmodel.StartDateTime,
                    EndDateTime = dicmodel.EndDateTime,
                    GroupID = group,
                    FKModuleHelperID = help.ID,
                    FKID = dicmodel.Second.Id,
                    PriorityLevel = dicmodel.Second.PriorityLevel,
                    ImageUrl = dicmodel.Second.ImgUrl
                });
                listPuzzles.Add(new SE_HomePageContentPuzzle
                {
                    StartDateTime = dicmodel.StartDateTime,
                    EndDateTime = dicmodel.EndDateTime,
                    GroupID = group,
                    FKModuleHelperID = help.ID,
                    FKID = dicmodel.Third.Id,
                    PriorityLevel = dicmodel.Third.PriorityLevel,
                    ImageUrl = dicmodel.Third.ImgUrl
                });
                if (puzzleType == 1 || puzzleType == 4)
                    listPuzzles.Add(new SE_HomePageContentPuzzle
                    {
                        StartDateTime = dicmodel.StartDateTime,
                        EndDateTime = dicmodel.EndDateTime,
                        GroupID = group,
                        FKModuleHelperID = help.ID,
                        FKID = dicmodel.Fourth.Id,
                        PriorityLevel = dicmodel.Fourth.PriorityLevel,
                        ImageUrl = dicmodel.Fourth.ImgUrl
                    });
                //dicmodel.Details.ForEach(r =>
                //{
                //    var puzzle = new SE_HomePageContentPuzzle();
                //    puzzle.FKID = r.FkId;
                //    puzzle.PriorityLevel = r.PriorityLevel;
                //    puzzle.ImageUrl = r.ImgUrl;
                //    puzzle.GroupID = group;
                //    puzzle.FKModuleHelperID = help.ID;
                //    puzzle.StartDateTime = dicmodel.StartDateTime;
                //    puzzle.EndDateTime = dicmodel.EndDateTime;
                //    listPuzzles.Add(puzzle);
                //});
                if (manager.AddPuzzleConfig(listPuzzles, puzzleType))
                    result = result && true;

                else
                {
                    result = false;

                }
            }
            return result;

        }
        public ActionResult GetHomePageContentPuzzleList(int? moduleID, int? moduleHelperID)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            var list = manager.GetHomePageContentPuzzleList(moduleID, moduleHelperID);
            if (list != null && list.Count() > 0)
                //var list=new List<SE_HomePageContentPuzzle>();
                //if (!moduleID.HasValue)
                //{
                //    if (moduleHelperID.HasValue)
                //    {
                //        var modulePhelps = manager.GetHomePageContentPuzzleList(null, moduleHelperID);
                //        var modulehelper = manager.GetHomePageModuleHelperEntity(moduleHelperID.Value);
                //        var module = manager.GetHomePageModuleEntity(modulehelper.FKHomePageModuleID.Value);
                //        var modulePs = manager.GetHomePageContentPuzzleList(module.ID, null);
                //        if (modulePhelps != null && modulePhelps.Any() && modulePs != null && modulePs.Any())
                //        {
                //            var group1s = modulePhelps.Select(r => r.GroupID).ToList();
                //            var group = "";
                //            foreach (var item in group1s)
                //            {
                //                var moduleP = modulePs.Where(r => r.GroupID == item);
                //                if (moduleP.Any())
                //                {
                //                    group = item;
                //                    break;
                //                }
                //            }
                //            list = modulePhelps.Where(r => r.GroupID == group)
                //                .ToList()
                //                .Union(modulePs.Where(r => r.GroupID == group).ToList())
                //                .ToList();
                //        }
                //    }
                //}
                //else
                //{
                //    list = manager.GetHomePageContentPuzzleList(moduleID, moduleHelperID).ToList();
                //}
                //if (list.Any())
                return Content(JsonConvert.SerializeObject(new
                {
                    Code = 1,
                    list
                }));
            return Content(JsonConvert.SerializeObject(new
            {
                Code = 0
            }));
        }


        public ActionResult GetHomePopUserJosn()
        {
            RepositoryManager manager = new RepositoryManager();
            Expression<Func<HomePopUserGroupEntity, bool>> exp = _ => _.TargetKey != "";
            var list = manager.GetEntityList<HomePopUserGroupEntity>(exp);
            return Content(JsonConvert.SerializeObject(list.Select(_ => new
            {
                id = _.TargetKey,
                text = _.TargetGroups
            })));
        }

        /// <summary>
        /// 复制新增
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public ActionResult CopyResult(int pkid)
        {
            SE_HomePageConfigManager manager = new SE_HomePageConfigManager();
            return manager.CopyHomePage(pkid) ? Content("1") : Content("0");
        }

        private void ModelConvert(SE_HomePageModuleContentConfig inModel, SE_HomePageModuleContentConfig outModel)
        {
            outModel.ID = inModel.ID;
            outModel.FKHomePageModuleID = inModel.FKHomePageModuleID;
            outModel.FKHomePageModuleHelperID = inModel.FKHomePageModuleHelperID;
            outModel.StartVersion = inModel.StartVersion;
            outModel.EndVersion = inModel.EndVersion;
            outModel.Title = inModel.Title;
            outModel.DeviceType = inModel.DeviceType;
            outModel.PriorityLevel = inModel.PriorityLevel;
            outModel.ButtonImageUrl = inModel.ButtonImageUrl;
            outModel.BannerImageUrl = inModel.BannerImageUrl;
            outModel.LinkUrl = inModel.LinkUrl;
            outModel.UriCount = inModel.UriCount;
            outModel.IsEnabled = inModel.IsEnabled;
            outModel.StartDateTime = inModel.StartDateTime;
            outModel.EndDateTime = inModel.EndDateTime;
            outModel.CreateDateTime = inModel.CreateDateTime;
            outModel.UpdateDateTime = inModel.UpdateDateTime;
            outModel.Width = inModel.Width;
            outModel.Height = inModel.Height;
            outModel.UpperLeftX = inModel.UpperLeftX;
            outModel.UpperLeftY = inModel.UpperLeftY;
            outModel.LowerRightX = inModel.LowerRightX;
            outModel.LowerRightY = inModel.LowerRightY;
            outModel.BigTitle = inModel.BigTitle;
            outModel.BigTilteColor = inModel.BigTilteColor;
            outModel.SmallTitle = inModel.SmallTitle;
            outModel.SmallTitleColor = inModel.SmallTitleColor;
            outModel.PromoteTitle = inModel.PromoteTitle;
            outModel.PromoteTitleColor = inModel.PromoteTitleColor;
            outModel.PromoteTitleBgColor = inModel.PromoteTitleBgColor;
            outModel.AnimationStyle = inModel.AnimationStyle;
            outModel.UserRank = inModel.UserRank;
            outModel.VIPRank = inModel.VIPRank;
            outModel.PeopleTip = inModel.PeopleTip;
            outModel.NoticeChannel = inModel.NoticeChannel;
            outModel.NewNoticeChannel = inModel.NewNoticeChannel;
        }


    }
}
