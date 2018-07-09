using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Identity;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.ConfigLog;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.WechatHome;
using Tuhu.Provisioning.Common;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.WeChat;
using Tuhu.Provisioning.DataAccess.Mapping;
using Tuhu.Provisioning.Models;
using Tuhu.Provisioning.Models.Wechat;
using Tuhu.Service.Config;
using Tuhu.Service.ConfigLog;
using Tuhu.Service.Push;
using Tuhu.Service.ThirdParty;

namespace Tuhu.Provisioning.Controllers
{
    public class WechatHomeController : Controller
    {
        // GET: WechatHome
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadModuleType()
        {
            WechatHomeManager manager = new WechatHomeManager();
            var rows = manager.GeHomePageModuleTypeList();
            List<string> moduleTypeList = rows.OrderBy(p => p.ID).Select(p => p.Name).ToList();
            return Json(moduleTypeList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult HomePageConfig()
        {
            return View();
        }

        public ActionResult Edit(int homepageconfigId, int id = 0)
        {
            if (id != 0)
            {
                WechatHomeManager manager = new WechatHomeManager();
                return View(manager.GetWechatHomeEntity(id));
            }
            else
            {
                return View(new WechatHomeList() { HomePageConfigID = homepageconfigId });
            }
        }

        public ActionResult HomePageConfigEdit(int id = 0)
        {
            if (id != 0)
            {
                WechatHomeManager manager = new WechatHomeManager();
                return View(manager.GetHomePageConfigEntity(id));
            }
            else
            {
                return View(new HomePageConfiguation());
            }
        }

        public ActionResult SaveWechatHome(WechatHomeList model)
        {
            string state = "success";
            string message = "操作成功";
            try
            {
                WechatHomeManager manager = new WechatHomeManager();
                // int result = 0;

                //var rows = manager.GetWechatHomeList(model.HomePageConfigID);
                //bool isexisted = rows.Select(p => p.Title).ToList().Contains(model.Title) ? true : false;
                //if (isexisted)
                //{
                //    state = "error";
                //    message = "名称为" + model.Title + "的模块已存在,故不能新增或修改";
                //}
                //else
                //{
                if (model.ID == 0)
                {
                    if (manager.AddWechatHomeList(model))
                    {
                        LoggerManager.InsertOplog(new ConfigHistory() { Author = User.Identity.Name, BeforeValue = "", AfterValue = JsonConvert.SerializeObject(model), ChangeDatetime = DateTime.Now, ObjectType = "WHPLoger", Operation = "新增小程序首页配置" + model.Title });
                        //  result = 1;
                    }
                }
                else
                {
                    var before = manager.GetWechatHomeEntity(model.ID);
                    if (manager.UpdateWechatHomeList(model))
                    {
                        LoggerManager.InsertOplog(new ConfigHistory() { Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), AfterValue = JsonConvert.SerializeObject(model), ChangeDatetime = DateTime.Now, ObjectID = model.ID.ToString(), ObjectType = "WHPLoger", Operation = "修改小程序首页配置" + model.Title });
                        //  result = 1;
                    }
                }
                //}
                return Content(JsonConvert.SerializeObject(new
                {
                    state = state,
                    message = message,
                    data = ""
                }));
            }
            catch (Exception em)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = state,
                    message = em.Message
                }));
            }
        }

        public ActionResult SaveHomePageConfig(HomePageConfiguation model)
        {
            try
            {
                WechatHomeManager manager = new WechatHomeManager();
                int result = 0;

                if (model.ID == 0)
                {
                    model.KeyValue = manager.GetRandomString();
                    if (manager.AddHomePageConfig(model))
                    {
                        LoggerManager.InsertOplog(new ConfigHistory() { Author = User.Identity.Name, BeforeValue = "", AfterValue = JsonConvert.SerializeObject(model), ChangeDatetime = DateTime.Now, ObjectType = "WHPLoger", Operation = "新增首页配置" + model.Name });
                        result = 1;
                    }
                }
                else
                {
                    var before = manager.GetHomePageConfigEntity(model.ID);
                    if (manager.UpdateHomePageConfig(model))
                    {
                        LoggerManager.InsertOplog(new ConfigHistory() { Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), AfterValue = JsonConvert.SerializeObject(model), ChangeDatetime = DateTime.Now, ObjectID = model.ID.ToString(), ObjectType = "WHPLoger", Operation = "修改首页配置" + model.Name });
                        result = 1;
                    }
                }
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功",
                    data = ""
                }));
            }
            catch (Exception em)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = em.Message
                }));
            }
        }

        public ActionResult DeleteWechatHome(int? id)
        {
            int result = 0;
            if (id.HasValue)
            {
                WechatHomeManager manager = new WechatHomeManager();
                var model = manager.GetWechatHomeEntity(id.Value);
                if (manager.DeleteWechatHomeList(id.Value))
                {
                    result = 1;
                    LoggerManager.InsertOplog(new ConfigHistory() { Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(model), AfterValue = "", ChangeDatetime = DateTime.Now, ObjectType = "WHPLoger", Operation = "删除小程序首页配置" + model.Title });
                }
            }

            return Content(JsonConvert.SerializeObject(new
            {
                state = "success",
                message = "删除成功",
                data = ""
            }));
        }

        public ActionResult DeleteHomePageConfig(int? id)
        {
            int result = 0;
            if (id.HasValue)
            {
                WechatHomeManager manager = new WechatHomeManager();
                var model = manager.GetHomePageConfigEntity(id.Value);
                if (manager.DeleteHomePageConfig(id.Value))
                {
                    result = 1;
                    LoggerManager.InsertOplog(new ConfigHistory() { Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(model), AfterValue = "", ChangeDatetime = DateTime.Now, ObjectType = "WHPLoger", Operation = "删除首页配置" + model.Name });
                }
            }

            return Content(JsonConvert.SerializeObject(new
            {
                state = "success",
                message = "删除成功",
                data = ""
            }));
        }

        public ActionResult GetWechatHomeList(int homepageconfigId)
        {
            WechatHomeManager manager = new WechatHomeManager();
            var rows = manager.GetWechatHomeList(homepageconfigId);
            return Content(JsonConvert.SerializeObject(rows.OrderBy(_ => _.OrderBy)));
        }

        public ActionResult GetHomePageConfigList()
        {
            WechatHomeManager manager = new WechatHomeManager();
            var rows = manager.GetHomePageConfigList();
            return Content(JsonConvert.SerializeObject(rows.OrderBy(_ => _.ID)));
        }

        public ActionResult ContentEdit(int fkid, int id = 0, bool copy = false)
        {
            if (id == 0)
            {
                return View(new WechatHomeContent() { FKID = fkid });
            }
            else
            {
                WechatHomeManager manager = new WechatHomeManager();
                var model = manager.GetContentEntity(id);
                if (copy)
                    model.ID = 0;
                return View(model);
            }
        }

        public ActionResult Regions(string id)
        {
            ViewBag.ID = id;
            return View();
        }

        public ActionResult AreaContentEdit(int fkid, int id = 0, bool copy = false)
        {
            if (id == 0)
            {
                return View(new WechatHomeAreaContent() { FKID = fkid });
            }
            else
            {
                WechatHomeManager manager = new WechatHomeManager();
                var model = manager.GetAreaContentEntity(id);
                if (copy)
                    model.ID = 0;
                return View(model);
            }
        }

        public ActionResult ProductContentEdit(int fkid, int id = 0, bool copy = false)
        {
            if (id == 0)
            {
                return View(new WechatHomeProductContent() { FKID = fkid });
            }
            else
            {
                WechatHomeManager manager = new WechatHomeManager();
                var model = manager.GetProductContentEntity(id);
                if (copy)
                    model.ID = 0;
                return View(model);
            }
        }

        public ActionResult ContentList(int fkid = 0)
        {
            return View();
        }

        public ActionResult ProductContentList(int fkid = 0)
        {
            return View();
        }

        public ActionResult AreaContentList(int fkid = 0)
        {
            return View();
        }

        public ActionResult GetContentList(int fkid = 0)
        {
            WechatHomeManager manager = new WechatHomeManager();

            return Content(JsonConvert.SerializeObject(manager.GetContentList(fkid).OrderBy(o => o.OrderBy)));
        }

        public ActionResult GetAreaContentList(int fkid = 0)
        {
            WechatHomeManager manager = new WechatHomeManager();

            return Content(JsonConvert.SerializeObject(manager.GetAreaContentList(fkid)));
        }

        public ActionResult GetProductContentList(int fkid = 0)
        {
            WechatHomeManager manager = new WechatHomeManager();

            return Content(JsonConvert.SerializeObject(manager.GetProductContentList(fkid)));
        }

        public ActionResult SaveContent(WechatHomeContent model)
        {
            try
            {
                WechatHomeManager manager = new WechatHomeManager();
                int result = 0;
                if (model.ID == 0)
                {
                    if (manager.AddContent(model))
                    {
                        result = 1;
                        LoggerManager.InsertOplog(new ConfigHistory() { Author = User.Identity.Name, BeforeValue = "", AfterValue = JsonConvert.SerializeObject(model), ChangeDatetime = DateTime.Now, ObjectType = "WHPLoger", Operation = "新增小程序首页配置模块内容" + model.Title });
                    }
                }
                else
                {
                    var before = manager.GetContentEntity(model.ID);
                    if (manager.UpdateContent(model))
                    {
                        result = 1;
                        LoggerManager.InsertOplog(new ConfigHistory() { Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), AfterValue = JsonConvert.SerializeObject(model), ChangeDatetime = DateTime.Now, ObjectType = "WHPLoger", Operation = "修改小程序首页配置模块内容" + model.Title });
                    }
                }
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功",
                    data = ""
                }));
            }
            catch (Exception em)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = em.Message
                }));
            }
        }

        public ActionResult SaveAreaContent(WechatHomeAreaContent model)
        {
            try
            {
                WechatHomeManager manager = new WechatHomeManager();
                int result = 0;
                if (model.ID == 0)
                {
                    if (manager.AddAreaContent(model))
                    {
                        result = 1;
                        LoggerManager.InsertOplog(new ConfigHistory() { Author = User.Identity.Name, BeforeValue = "", AfterValue = JsonConvert.SerializeObject(model), ChangeDatetime = DateTime.Now, ObjectType = "WHPAreaLoger", Operation = "新增小程序首页配置模块区域内容" + model.Headings });
                    }
                }
                else
                {
                    var before = manager.GetAreaContentEntity(model.ID);
                    if (manager.UpdateAreaContent(model))
                    {
                        result = 1;
                        LoggerManager.InsertOplog(new ConfigHistory() { Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), AfterValue = JsonConvert.SerializeObject(model), ChangeDatetime = DateTime.Now, ObjectType = "WHPAreaLoger", Operation = "修改小程序首页配置模块区域内容" + model.Headings });
                    }
                }
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功",
                    data = ""
                }));
            }
            catch (Exception em)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = em.Message
                }));
            }
        }

        public ActionResult SaveProductContent(WechatHomeProductContent model)
        {
            try
            {
                WechatHomeManager manager = new WechatHomeManager();
                int result = 0;
                if (model.ID == 0)
                {
                    if (manager.AddProductContent(model))
                    {
                        result = 1;
                        LoggerManager.InsertOplog(new ConfigHistory() { Author = User.Identity.Name, BeforeValue = "", AfterValue = JsonConvert.SerializeObject(model), ChangeDatetime = DateTime.Now, ObjectType = "WHPLoger", Operation = "新增小程序首页配置模块内容" + model.ProductName });
                    }
                }
                else
                {
                    var before = manager.GetProductContentEntity(model.ID);
                    if (manager.UpdateProductContent(model))
                    {
                        result = 1;
                        LoggerManager.InsertOplog(new ConfigHistory() { Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(before), AfterValue = JsonConvert.SerializeObject(model), ChangeDatetime = DateTime.Now, ObjectType = "WHPLoger", Operation = "修改小程序首页配置模块内容" + model.ProductName });
                    }
                }
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功",
                    data = ""
                }));
            }
            catch (Exception em)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = em.Message
                }));
            }
        }

        public ActionResult DeleteContent(int? id)
        {
            int result = 0;
            if (id.HasValue)
            {
                WechatHomeManager manager = new WechatHomeManager();
                var model = manager.GetContentEntity(id.Value);
                if (manager.DeleteContent(id.Value))
                {
                    result = 1;
                    LoggerManager.InsertOplog(new ConfigHistory() { Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(model), AfterValue = "", ChangeDatetime = DateTime.Now, ObjectType = "WHPLoger", Operation = "新增小程序首页配置模块内容" + model.Title });
                }
            }
            if (result == 1)
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "删除成功",
                    data = ""
                }));
            else
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = "删除成功",
                    data = ""
                }));
        }

        public ActionResult DeleteAreaContent(int? id)
        {
            int result = 0;
            if (id.HasValue)
            {
                WechatHomeManager manager = new WechatHomeManager();
                var model = manager.GetAreaContentEntity(id.Value);
                if (manager.DeleteAreaContent(id.Value))
                {
                    result = 1;
                    LoggerManager.InsertOplog(new ConfigHistory() { Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(model), AfterValue = "", ChangeDatetime = DateTime.Now, ObjectType = "WHPLoger", Operation = "新增小程序首页配置模块内容" + model.Headings });
                }
            }
            if (result == 1)
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "删除成功",
                    data = ""
                }));
            else
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = "删除成功",
                    data = ""
                }));
        }

        public ActionResult DeleteProductContent(int? id)
        {
            int result = 0;
            if (id.HasValue)
            {
                WechatHomeManager manager = new WechatHomeManager();
                var model = manager.GetProductContentEntity(id.Value);
                if (manager.DeleteProductContent(id.Value))
                {
                    result = 1;
                    LoggerManager.InsertOplog(new ConfigHistory() { Author = User.Identity.Name, BeforeValue = JsonConvert.SerializeObject(model), AfterValue = "", ChangeDatetime = DateTime.Now, ObjectType = "WHPLoger", Operation = "新增小程序首页配置模块内容" + model.ProductName });
                }
            }
            if (result == 1)
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "删除成功",
                    data = ""
                }));
            else
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = "删除成功",
                    data = ""
                }));
        }

        public async Task<JsonResult> GetProductGroupInfoByPIdAsync(string pid)
        {
            JsonResult jr = new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            var datetime = DateTime.Now;
            using (var client = new Tuhu.Service.PinTuan.PinTuanClient())
            {
                var result = await client.GetProductGroupInfoByPIdAsync(pid);
                result.ThrowIfException(true);
                jr.Data = new
                {
                    code = 1,
                    data = result.Result.ProductGroupList.Where(p => p.IsShow == true).Where(p => p.IsStockout == false).Where(p => p.StartTime <= datetime && p.EndTime >= datetime).ToList()
                };
            }
            return jr;
        }

        public ActionResult SaveWechatHomeCopy(int id)
        {
            try
            {
                WechatHomeManager manager = new WechatHomeManager();
                var model = manager.GetWechatHomeEntity(id);
                model.ID = 0;
                var keyValue = manager.AddWechatHomeListToInt(model);
                var contetnlist = manager.GetContentList(id)?.ToList(); ;
                for (int i = 0; i < contetnlist.Count; i++)
                {
                    var item = contetnlist[i];
                    item.ID = 0;
                    item.FKID = keyValue;
                    manager.AddContent(item);
                }
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功",
                    data = ""
                }));
            }
            catch (Exception em)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = "操作失败",
                    data = em.Message
                }));
            }
        }

        #region 公众号菜单配置

        public ActionResult SubNumberList()
        {
            return View();
        }

        /// <summary>
        /// 获取公众号的菜单列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSubNumberGrid(string originalId)
        {
            RepositoryManager manager = new Business.RepositoryManager();
            System.Linq.Expressions.Expression<Func<WechatSubNumberEntity, bool>> exp = _ => _.OriginalID == originalId;
            var list = manager.GetEntityList<WechatSubNumberEntity>(exp);
            if (list != null)
            {
                var treeList = new List<TreeGridModel>();
                foreach (WechatSubNumberEntity item in list?.OrderBy(_ => _?.OrderBy))
                {
                    TreeGridModel treeModel = new TreeGridModel();
                    treeModel.id = item.PKID.ToString();
                    bool hasChildren = list.Count(t => t.ParentPKID == item.PKID) == 0 ? false : true;
                    treeModel.text = item.Name;
                    item.Name = hasChildren == false ? "<span style=\"color:blue;\">" + item.Name + "</span>" : item.Name;
                    treeModel.isLeaf = hasChildren;
                    treeModel.parentId = string.IsNullOrWhiteSpace(item.ParentPKID.ToString()) ? "0" : item.ParentPKID.ToString();
                    treeModel.expanded = false;
                    treeModel.entityJson = JsonConvert.SerializeObject(item);

                    treeList.Add(treeModel);
                }
                return Content(treeList.TreeGridJson());
            }
            else
                return Content("null");
        }

        /// <summary>
        /// 获取父菜单
        /// </summary>
        /// <param name="originalId"></param>
        /// <returns></returns>
        public ActionResult GetSubNumberSelectBind(string originalId)
        {
            RepositoryManager manager = new Business.RepositoryManager();
            System.Linq.Expressions.Expression<Func<WechatSubNumberEntity, bool>> exp = _ => _.OriginalID == originalId;
            var list = manager.GetEntityList<WechatSubNumberEntity>(exp);
            return Content(JsonConvert.SerializeObject(list?.Where(_ => _.ParentPKID == null)));
        }

        /// <summary>
        /// 跳转到新增页
        /// </summary>
        /// <returns></returns>
        public ActionResult SubNumberForm()
        {
            return View();
        }

        /// <summary>
        /// 获取微信菜单详情
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public ActionResult GetSubNumberEntity(int pkid)
        {
            RepositoryManager manager = new RepositoryManager();
            var mappingManager = new WXMenuMaterialMappingManager();
            var materialTextManager = new WXMaterialTextManager();

            WechatSubNumberModel model = new WechatSubNumberModel();

            //微信菜单信息
            var entity = manager.GetEntity<WechatSubNumberEntity>(pkid);
            //对象赋值
            model = ObjectMapper.ConvertTo<WechatSubNumberEntity, WechatSubNumberModel>(entity);

            #region 菜单关联的素材信息

            model.MaterialList = new List<MaterialModel>();
            model.DelayMaterialList = new List<MaterialModel>();
            MaterialModel materialModel = null;

            model.MaterialTextList = new List<WXMaterialTextModel>();
            model.DelayMaterialTextList = new List<WXMaterialTextModel>();
            WXMaterialTextModel materialTextModel = null;

            var menuMaterialMappingList = mappingManager.GetWXMenuMaterialMappingList(entity.PKID, "click");
            var materialTextList = materialTextManager.GetWXMaterialTextList(entity.PKID, "click");

            foreach (var item in menuMaterialMappingList)
            {
                materialModel = new MaterialModel();
                if (item.IsDelaySend)//延迟
                {
                    if (string.IsNullOrEmpty(model.IntervalTime))
                        model.IntervalTime = item.IntervalHours.ToString() + ":" + item.IntervalMinutes;

                    if (model.DelayReplyWay == 0)
                        model.DelayReplyWay = item.ReplyWay;

                    if (!model.IsDelaySend)
                        model.IsDelaySend = true;
                }
                else
                    model.ReplyWay = item.ReplyWay;

                if (item.MediaType == "text")//文字素材
                {
                    materialTextModel = new WXMaterialTextModel();
                    var materialTextInfo = materialTextList.Where(t => t.PKID == item.MaterialID).FirstOrDefault();
                    if (materialTextInfo != null)
                        materialTextModel.Content = materialTextInfo.Content;
                    if (item.IsDelaySend)
                        model.DelayMaterialTextList.Add(materialTextModel);
                    else
                        model.MaterialTextList.Add(materialTextModel);
                }
                else//其他素材
                {
                    materialModel.Title = item.Title;
                    materialModel.MediaID = item.MediaID;
                    materialModel.MediaType = item.MediaType;

                    if (item.IsDelaySend)
                        model.DelayMaterialList.Add(materialModel);
                    else
                        model.MaterialList.Add(materialModel);
                }
            }

            #endregion 菜单关联的素材信息

            return Content(JsonConvert.SerializeObject(model));
        }

        /// <summary>
        /// 保存菜单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ActionResult SubmitSubNumber(WechatSubNumberModel model)
        {
            WechatSubNumberEntity entity = ObjectMapper.ConvertTo<WechatSubNumberModel, WechatSubNumberEntity>(model);
            var mappingManager = new WXMenuMaterialMappingManager();
            var materialTextManager = new WXMaterialTextManager();
            RepositoryManager manager = new RepositoryManager();
            try
            {
                if (entity.PKID == 0)
                {
                    entity.IsEnabled = true;
                    manager.Add<WechatSubNumberEntity>(entity);
                    entity.ButtonKey = "rselfmenu_" + entity.PKID;
                    manager.Update<WechatSubNumberEntity>(entity);
                }
                else
                {
                    entity.IsEnabled = true;
                    entity.ButtonKey = "rselfmenu_" + entity.PKID;
                    manager.Update<WechatSubNumberEntity>(entity);

                    //删除菜单和素材关联数据
                    mappingManager.DeleteWXMenuMaterialMapping(entity.PKID, "click");
                    //删除文本素材数据
                    materialTextManager.DeleteWXMaterialText(entity.PKID, "click");
                }

                #region 微信菜单关联素材

                WXMenuMaterialMappingModel mappingModel = null;
                WXMaterialTextModel textModel = null;
                //保存菜单和素材Mapping
                if (model.MaterialList != null)
                {
                    foreach (var item in model.MaterialList)
                    {
                        mappingModel = new WXMenuMaterialMappingModel();
                        mappingModel.ObjectID = entity.PKID;
                        mappingModel.ObjectType = "click";
                        mappingModel.OriginalID = entity.OriginalID;
                        mappingModel.ButtonKey = entity.ButtonKey;
                        //mappingModel.MaterialID = item.MediaID;
                        mappingModel.MediaType = item.MediaType;
                        mappingModel.MediaID = item.MediaID;
                        mappingModel.Title = item.Title;
                        mappingModel.ImageUrl = item.URL;
                        mappingModel.ReplyWay = model.ReplyWay;
                        mappingModel.IsDelaySend = false;

                        mappingManager.AddWXMenuMaterialMapping(mappingModel);
                    }
                }
                //保存延迟回复的菜单和素材Mapping
                if (model.DelayMaterialList != null)
                {
                    foreach (var item in model.DelayMaterialList)
                    {
                        mappingModel = new WXMenuMaterialMappingModel();
                        mappingModel.ObjectID = entity.PKID;
                        mappingModel.ObjectType = "click";
                        mappingModel.OriginalID = entity.OriginalID;
                        mappingModel.ButtonKey = entity.ButtonKey;
                        //mappingModel.MaterialID = item.MediaID;
                        mappingModel.MediaType = item.MediaType;
                        mappingModel.MediaID = item.MediaID;
                        mappingModel.Title = item.Title;
                        mappingModel.ImageUrl = item.URL;
                        mappingModel.ReplyWay = model.DelayReplyWay;
                        mappingModel.IsDelaySend = model.IsDelaySend;

                        var arrTime = model.IntervalTime.Split(':');

                        mappingModel.IntervalHours = int.Parse(arrTime[0]);
                        mappingModel.IntervalMinutes = int.Parse(arrTime[1]);

                        mappingManager.AddWXMenuMaterialMapping(mappingModel);
                    }
                }
                //保存菜单和文字素材Mapping
                if (model.MaterialTextList != null)
                {
                    foreach (var item in model.MaterialTextList)
                    {
                        textModel = new WXMaterialTextModel();
                        textModel.ObjectID = entity.PKID;
                        textModel.ObjectType = "click";
                        textModel.Content = item.Content;
                        textModel.OriginalID = entity.OriginalID;
                        materialTextManager.AddWXMaterialText(textModel);

                        mappingModel = new WXMenuMaterialMappingModel();
                        mappingModel.ObjectID = entity.PKID;
                        mappingModel.ObjectType = "click";
                        mappingModel.OriginalID = entity.OriginalID;
                        mappingModel.ButtonKey = entity.ButtonKey;
                        mappingModel.MaterialID = textModel.PKID;
                        mappingModel.MediaType = "text";
                        mappingModel.ReplyWay = model.ReplyWay;
                        mappingModel.IsDelaySend = false;

                        mappingManager.AddWXMenuMaterialMapping(mappingModel);
                    }
                }
                //保存延迟回复的菜单和文字素材Mapping
                if (model.DelayMaterialTextList != null)
                {
                    foreach (var item in model.DelayMaterialTextList)
                    {
                        textModel = new WXMaterialTextModel();
                        textModel.ObjectID = entity.PKID;
                        textModel.ObjectType = "click";
                        textModel.Content = item.Content;
                        textModel.OriginalID = entity.OriginalID;
                        materialTextManager.AddWXMaterialText(textModel);

                        mappingModel = new WXMenuMaterialMappingModel();
                        mappingModel.ObjectID = entity.PKID;
                        mappingModel.ObjectType = "click";
                        mappingModel.OriginalID = entity.OriginalID;
                        mappingModel.ButtonKey = entity.ButtonKey;
                        mappingModel.MaterialID = textModel.PKID;
                        mappingModel.MediaType = "text";
                        mappingModel.ReplyWay = model.DelayReplyWay;
                        mappingModel.IsDelaySend = model.IsDelaySend;

                        var arrTime = model.IntervalTime.Split(':');

                        mappingModel.IntervalHours = int.Parse(arrTime[0]);
                        mappingModel.IntervalMinutes = int.Parse(arrTime[1]);

                        mappingManager.AddWXMenuMaterialMapping(mappingModel);
                    }
                }

                #endregion 微信菜单关联素材

                //更新缓存
                using (var client = new WxConfigClient())
                {
                    client.RefreshWXClickMsgCache(entity.OriginalID, entity.ButtonKey);
                }
                //等待1秒，写库同步到读库
                Thread.Sleep(1000);
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功",
                    data = ""
                }));
            }
            catch (Exception em)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = "操作失败",
                    data = em.Message
                }));
            }
        }

        /// <summary>
        /// 启用或禁用菜单
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public ActionResult DeleteSubNumber(int pkid)
        {
            if (pkid <= 0)
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "warning",
                    message = "参数无效",
                    data = ""
                }));

            RepositoryManager manager = new RepositoryManager();
            var entity = manager.GetEntity<WechatSubNumberEntity>(pkid);
            entity.IsEnabled = !entity.IsEnabled;
            manager.Update<WechatSubNumberEntity>(entity);
            //等待1秒，写库同步到读库
            Thread.Sleep(1000);
            return Content(JsonConvert.SerializeObject(new
            {
                state = "success",
                message = "设置禁用成功",
                data = ""
            }));
        }

        /// <summary>
        /// 同步公众号菜单到微信平台
        /// </summary>
        /// <returns></returns>
        public ActionResult ReloadWechatMenu(string originalId)
        {
            RepositoryManager manager = new Business.RepositoryManager();
            WechatHomeManager wechatManager = new WechatHomeManager();
            System.Linq.Expressions.Expression<Func<WechatSubNumberEntity, bool>> exp = _ => _.IsEnabled == true && _.OriginalID == originalId;
            var list = manager.GetEntityList<WechatSubNumberEntity>(exp);
            List<WechatMenuListEntity> menuList = new List<WechatMenuListEntity>();
            WechatMenuListEntity menus = null;
            if (list != null)
            {
                foreach (var item in list?.Where(t => t.ParentPKID == null).OrderBy(_ => _?.OrderBy))
                {
                    var children = list?.Where(_ => _.ParentPKID == item.PKID).OrderBy(_ => _.OrderBy);
                    switch (item.Type)
                    {
                        case "click":
                            if (children?.Count() <= 0)
                                menus = new WechatMenuListEntity() { name = item.Name, type = item.Type, key = item.ButtonKey };
                            break;

                        case "view":
                            if (children?.Count() <= 0)
                                menus = new WechatMenuListEntity() { name = item.Name, type = item.Type, url = item.Url };
                            break;

                        case "miniprogram":
                            if (children?.Count() <= 0)
                                menus = new WechatMenuListEntity() { name = item.Name, type = item.Type, url = item.Url, appid = item.Appid, pagepath = item.Pagepath };
                            break;
                    }
                    if (children?.Count() > 0)
                    {
                        menus = new WechatMenuListEntity() { name = item.Name, sub_button = new List<WechatMenuEntity>() };
                        children?.ForEach(_ =>
                        {
                            WechatMenuEntity entity = new WechatMenuEntity()
                            {
                                name = _.Name,
                                type = _.Type,
                                key = _.ButtonKey,
                                url = _.Url,
                                appid = _.Appid,
                                pagepath = _.Pagepath
                            };
                            menus.sub_button.Add(entity);
                        });
                    }
                    menuList.Add(menus);
                }
            }
            //return Content(JsonConvert.SerializeObject(new
            //{
            //    button = menuList
            //}));

            if (Request.Headers["host"] != "setting.tuhu.cn" && Request.Headers["host"] != "settingut.tuhu.cn")
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "warning",
                    message = "该功能只能在线上使用",
                    data = ""
                }));
            }
            string accessToken = string.Empty;
            using (var client = new WeiXinPushClient())
            {
                var weChatInfo = client.SelectWxConfigs().Result.Where(t => t.OriginalId == originalId).FirstOrDefault();
                if (weChatInfo != null)
                    accessToken = client.SelectWxAccessToken(weChatInfo.channel).Result;
            }
            if (!menuList.Any())
            {
                //删除自定义菜单
                var flag = wechatManager.DeleteWxMenu(accessToken, User.Identity.Name);
                if (flag)
                    return Content(JsonConvert.SerializeObject(new { state = "success", message = "菜单更新成功", data = "" }));
            }
            string requestUrl = string.Format("https://api.weixin.qq.com/cgi-bin/menu/create?access_token={0}", accessToken);
            var request = System.Net.WebRequest.Create(requestUrl);
            request.Method = "POST";
            var stream = request.GetRequestStream();
            var bytes = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
            {
                button = menuList
            }));
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();
            var response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string result = reader.ReadToEnd();
            response.Close();
            response.Dispose();

            JObject json = JObject.Parse(result);
            if (json["errcode"].ToString() == "0")
            {
                LoggerManager.InsertOplog(new ConfigHistory()
                {
                    AfterValue = JsonConvert.SerializeObject(new
                    {
                        button = menuList
                    }),
                    Author = User.Identity.Name,
                    ObjectType = "WxMenu",
                    Operation = "更新公众号菜单"
                });
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "菜单更新成功",
                    data = ""
                }));
            }
            else
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "warning",
                    message = "菜单更新失败:" + json["errmsg"].ToString(),
                    data = ""
                }));
            }
        }

        #endregion 公众号菜单配置

        #region 关键字回复配置

        public ActionResult WechatKeywords()
        {
            return View();
        }

        public ActionResult GetKeywordsGrid(string originalId)
        {
            RepositoryManager manager = new Business.RepositoryManager();
            System.Linq.Expressions.Expression<Func<WechatKeywordReplyEntity, bool>> exp =
                _ => _.OriginalID == originalId && !_.IsDeleted;
            var list = manager.GetEntityList<WechatKeywordReplyEntity>(exp);
            if (list != null)
            {
                var result = list.Where(x => x.RuleGroup == null).ToList();
                var dic = list.Where(x => x.RuleGroup != null).GroupBy(x => x.RuleGroup).ToDictionary(k => k.Key, v => v.ToList());
                foreach (var key in dic.Keys)
                {
                    var temp = dic[key]?.FirstOrDefault();
                    if (temp != null)
                    {
                        temp.Keyword = string.Join(",", dic[key].Select(x => x.Keyword));
                        result.Add(temp);
                    }
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
                return Content("null");
        }

        public ActionResult SubmitKeywordReply(WechatKeywordReplyModel model)
        {
            try
            {
                if (model.RuleGroup == Guid.Empty)
                {
                    model.RuleGroup = Guid.NewGuid();
                }

                List<WechatKeywordReplyEntity> entitys = new List<WechatKeywordReplyEntity>();
                RepositoryManager manager = new RepositoryManager();

                var mappingManager = new WXMenuMaterialMappingManager();
                WXMenuMaterialMappingModel mappingModel = null;

                var materialTextManager = new WXMaterialTextManager();
                WXMaterialTextModel textModel = null;
                foreach (var keywordReply in model.KeywordReplyList)
                {
                    WechatKeywordReplyEntity entity =
                        ObjectMapper.ConvertTo<WechatKeywordReplyModel, WechatKeywordReplyEntity>(model);

                    if (keywordReply.PKID == 0)
                    {
                        entity.IsDeleted = false;
                        entity.IsEnabled = true;
                        entity.MatchedPattern = keywordReply.MatchedPattern;
                        entity.PKID = keywordReply.PKID;
                        entity.Keyword = keywordReply.Keyword;
                        manager.Add<WechatKeywordReplyEntity>(entity);
                    }
                    else
                    {
                        entity.IsDeleted = false;
                        entity.IsEnabled = true;
                        entity.MatchedPattern = keywordReply.MatchedPattern;
                        entity.PKID = keywordReply.PKID;
                        entity.Keyword = keywordReply.Keyword;
                        manager.Update<WechatKeywordReplyEntity>(entity);

                        //删除菜单和素材关联数据
                        mappingManager.DeleteWXMenuMaterialMapping(entity.PKID, "keyword");
                        //删除文本素材数据
                        materialTextManager.DeleteWXMaterialText(entity.PKID, "keyword");
                    }
                    entitys.Add(entity);

                    #region 微信菜单关联素材

                    //保存菜单和素材Mapping
                    if (model.MaterialList != null)
                    {
                        foreach (var item in model.MaterialList)
                        {
                            mappingModel = new WXMenuMaterialMappingModel();
                            mappingModel.ObjectID = entity.PKID;
                            mappingModel.ObjectType = "keyword";
                            mappingModel.OriginalID = entity.OriginalID;
                            // mappingModel.MaterialID = item.MediaID;
                            mappingModel.ButtonKey = string.Empty;
                            mappingModel.MediaType = item.MediaType;
                            mappingModel.MediaID = item.MediaID;
                            mappingModel.Title = item.Title;
                            mappingModel.ImageUrl = item.URL;
                            mappingModel.ReplyWay = model.ReplyWay;
                            mappingModel.IsDelaySend = false;

                            mappingManager.AddWXMenuMaterialMapping(mappingModel);
                        }
                    }
                    //保存延迟回复的菜单和素材Mapping
                    if (model.DelayMaterialList != null)
                    {
                        foreach (var item in model.DelayMaterialList)
                        {
                            mappingModel = new WXMenuMaterialMappingModel();
                            mappingModel.ObjectID = entity.PKID;
                            mappingModel.ObjectType = "keyword";
                            mappingModel.OriginalID = entity.OriginalID;
                            mappingModel.ButtonKey = string.Empty;
                            //mappingModel.MaterialID = item.MediaID;
                            mappingModel.MediaType = item.MediaType;
                            mappingModel.MediaID = item.MediaID;
                            mappingModel.Title = item.Title;
                            mappingModel.ImageUrl = item.URL;
                            mappingModel.ReplyWay = model.DelayReplyWay;
                            mappingModel.IsDelaySend = model.IsDelaySend;

                            var arrTime = model.IntervalTime.Split(':');

                            mappingModel.IntervalHours = int.Parse(arrTime[0]);
                            mappingModel.IntervalMinutes = int.Parse(arrTime[1]);

                            mappingManager.AddWXMenuMaterialMapping(mappingModel);
                        }
                    }
                }

                //保存菜单和文字素材Mapping
                if (model.MaterialTextList != null)
                {
                    foreach (var item in model.MaterialTextList)
                    {
                        textModel = new WXMaterialTextModel();
                        textModel.ObjectID = 0;
                        textModel.ObjectType = "keyword";
                        textModel.Content = item.Content;
                        textModel.OriginalID = model.OriginalID;
                        materialTextManager.AddWXMaterialText(textModel);
                        foreach (var entity in entitys)
                        {
                            mappingModel = new WXMenuMaterialMappingModel();
                            mappingModel.ObjectID = entity.PKID;
                            mappingModel.ObjectType = "keyword";
                            mappingModel.OriginalID = entity.OriginalID;
                            mappingModel.ButtonKey = string.Empty;
                            mappingModel.MaterialID = textModel.PKID;
                            mappingModel.MediaType = "text";
                            mappingModel.ReplyWay = model.ReplyWay;
                            mappingModel.IsDelaySend = false;

                            mappingManager.AddWXMenuMaterialMapping(mappingModel);
                        }
                    }
                }
                //保存延迟回复的菜单和文字素材Mapping
                if (model.DelayMaterialTextList != null)
                {
                    foreach (var item in model.DelayMaterialTextList)
                    {
                        textModel = new WXMaterialTextModel();
                        textModel.ObjectID = 0;
                        textModel.ObjectType = "keyword";
                        textModel.Content = item.Content;
                        textModel.OriginalID = model.OriginalID;
                        materialTextManager.AddWXMaterialText(textModel);
                        foreach (var entity in entitys)
                        {
                            mappingModel = new WXMenuMaterialMappingModel();
                            mappingModel.ObjectID = entity.PKID;
                            mappingModel.ObjectType = "keyword";
                            mappingModel.OriginalID = entity.OriginalID;
                            mappingModel.ButtonKey = string.Empty;
                            mappingModel.MaterialID = textModel.PKID;
                            mappingModel.MediaType = "text";
                            mappingModel.ReplyWay = model.DelayReplyWay;
                            mappingModel.IsDelaySend = model.IsDelaySend;

                            var arrTime = model.IntervalTime.Split(':');

                            mappingModel.IntervalHours = int.Parse(arrTime[0]);
                            mappingModel.IntervalMinutes = int.Parse(arrTime[1]);

                            mappingManager.AddWXMenuMaterialMapping(mappingModel);
                        }
                    }
                }

                #endregion 微信菜单关联素材

                //等待1秒，写库同步到读库
                Thread.Sleep(1000);
                //更新缓存
                using (var client = new WxConfigClient())
                {
                    client.RefreshWXKeywordsMsgCache(model.OriginalID);
                }
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "success",
                    message = "操作成功",
                    data = ""
                }));
            }
            catch (Exception em)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = "操作失败",
                    data = em.Message
                }));
            }
        }

        public ActionResult GetKeywordReplyEntity(int pkid)
        {
            RepositoryManager manager = new RepositoryManager();
            var mappingManager = new WXMenuMaterialMappingManager();
            var materialTextManager = new WXMaterialTextManager();

            WechatKeywordReplyModel model = new WechatKeywordReplyModel();

            //微信菜单信息
            var entity = manager.GetEntity<WechatKeywordReplyEntity>(pkid);

            //对象赋值
            model = ObjectMapper.ConvertTo<WechatKeywordReplyEntity, WechatKeywordReplyModel>(entity);
            model.KeywordReplyList = new List<KeywordReplyListModel>()
            {
                new KeywordReplyListModel()
                {
                     Keyword=entity.Keyword,
                      MatchedPattern=entity.MatchedPattern??0,
                       PKID=entity.PKID
                }
            };
            if (entity.RuleGroup != null)
            {
                var entityList = manager.GetEntityList<WechatKeywordReplyEntity>(
                    "SELECT * FROM Configuration..WXKeywordReply WITH(NOLOCK) WHERE RuleGroup=@RuleGroup",
                    new SqlParameter[]
                    {
                        new SqlParameter("@RuleGroup", entity.RuleGroup)
                    });

                model.KeywordReplyList = new List<KeywordReplyListModel>();
                foreach (var entityItem in entityList)
                {
                    model.KeywordReplyList.Add(new KeywordReplyListModel()
                    {
                        Keyword = entityItem.Keyword,
                        MatchedPattern = entityItem.MatchedPattern ?? 0,
                        PKID = entityItem.PKID
                    });
                }
            }

            #region 菜单关联的素材信息

            model.MaterialList = new List<MaterialModel>();
            model.DelayMaterialList = new List<MaterialModel>();
            MaterialModel materialModel = null;

            model.MaterialTextList = new List<WXMaterialTextModel>();
            model.DelayMaterialTextList = new List<WXMaterialTextModel>();
            WXMaterialTextModel materialTextModel = null;

            var menuMaterialMappingList = mappingManager.GetWXMenuMaterialMappingList(entity.PKID, "keyword");
            var materialTextList = materialTextManager.GetWXMaterialTextList(entity.PKID, "keyword");

            foreach (var item in menuMaterialMappingList)
            {
                materialModel = new MaterialModel();
                if (item.IsDelaySend)//延迟
                {
                    if (string.IsNullOrEmpty(model.IntervalTime))
                        model.IntervalTime = item.IntervalHours.ToString() + ":" + item.IntervalMinutes;

                    if (model.DelayReplyWay == 0)
                        model.DelayReplyWay = item.ReplyWay;

                    if (!model.IsDelaySend)
                        model.IsDelaySend = true;
                }
                else
                    model.ReplyWay = item.ReplyWay;

                if (item.MediaType == "text")//文字素材
                {
                    materialTextModel = new WXMaterialTextModel();
                    var materialTextInfo = materialTextList.Where(t => t.PKID == item.MaterialID).FirstOrDefault();
                    if (materialTextInfo != null)
                        materialTextModel.Content = materialTextInfo.Content;
                    if (item.IsDelaySend)
                        model.DelayMaterialTextList.Add(materialTextModel);
                    else
                        model.MaterialTextList.Add(materialTextModel);
                }
                else//其他素材
                {
                    materialModel.Title = item.Title;
                    materialModel.MediaID = item.MediaID;
                    materialModel.MediaType = item.MediaType;

                    if (item.IsDelaySend)
                        model.DelayMaterialList.Add(materialModel);
                    else
                        model.MaterialList.Add(materialModel);
                }
            }

            #endregion 菜单关联的素材信息

            return Content(JsonConvert.SerializeObject(model));
        }

        public ActionResult DeleteKeywordReply(int pkid)
        {
            if (pkid <= 0)
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "warning",
                    message = "参数无效",
                    data = ""
                }));

            RepositoryManager manager = new RepositoryManager();
            var entity = manager.GetEntity<WechatKeywordReplyEntity>(pkid);
            if (entity.RuleGroup != null)
            {
                var entityList = manager.GetEntityList<WechatKeywordReplyEntity>(
                    "SELECT * FROM Configuration..WXKeywordReply WITH(NOLOCK) WHERE RuleGroup=@RuleGroup",
                    new SqlParameter[]
                    {
                        new SqlParameter("@RuleGroup", entity.RuleGroup)
                    });

                foreach (var entityItem in entityList)
                {
                    entityItem.IsDeleted = true;
                    manager.Update<WechatKeywordReplyEntity>(entityItem);
                }
            }
            else
            {
                entity.IsDeleted = !entity.IsDeleted;
                manager.Update<WechatKeywordReplyEntity>(entity);
            }
            //等待1秒，写库同步到读库
            Thread.Sleep(1000);
            //更新缓存
            using (var client = new WxConfigClient())
            {
                client.RefreshWXKeywordsMsgCache(entity.OriginalID);
            }
            return Content(JsonConvert.SerializeObject(new
            {
                state = "success",
                message = "设置禁用成功",
                data = ""
            }));
        }

        public ActionResult EnabledKeywordReply(int pkid)
        {
            if (pkid <= 0)
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "warning",
                    message = "参数无效",
                    data = ""
                }));

            RepositoryManager manager = new RepositoryManager();
            var entity = manager.GetEntity<WechatKeywordReplyEntity>(pkid);
            if (entity.RuleGroup != null)
            {
                var entityList = manager.GetEntityList<WechatKeywordReplyEntity>(
                    "SELECT * FROM Configuration..WXKeywordReply WITH(NOLOCK) WHERE RuleGroup=@RuleGroup",
                    new SqlParameter[]
                    {
                        new SqlParameter("@RuleGroup", entity.RuleGroup)
                    });

                foreach (var entityItem in entityList)
                {
                    entityItem.IsEnabled = !entity.IsEnabled;
                    manager.Update<WechatKeywordReplyEntity>(entityItem);
                }
            }
            else
            {
                entity.IsEnabled = !entity.IsEnabled;
                manager.Update<WechatKeywordReplyEntity>(entity);
            }
            //等待1秒，写库同步到读库
            Thread.Sleep(1000);
            //更新缓存
            using (var client = new WxConfigClient())
            {
                client.RefreshWXKeywordsMsgCache(entity.OriginalID);
            }
            return Content(JsonConvert.SerializeObject(new
            {
                state = "success",
                message = "设置禁用成功",
                data = ""
            }));
        }

        public ActionResult WxKeywordsForm()
        {
            return View();
        }

        #endregion 关键字回复配置

        #region 公众号关注回复配置

        public ActionResult WechatSubscribeList()
        {
            return View();
        }

        /// <summary>
        /// 获取关注回复列表信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetWechatSubscribeGrid(string originalId)
        {
            RepositoryManager manager = new RepositoryManager();
            System.Linq.Expressions.Expression<Func<WXEventMsgConfigEntity, bool>> exp = _ => _.OriginalID == originalId;
            var list = manager.GetEntityList<WXEventMsgConfigEntity>(exp);
            return Content(JsonConvert.SerializeObject(list?.OrderBy(_ => _.OrderBy)));
        }

        /// <summary>
        /// 获取关注回复详情信息
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public ActionResult GetWechatSubscEntity(int pkid)
        {
            RepositoryManager manager = new RepositoryManager();
            var entity = manager.GetEntity<WXEventMsgConfigEntity>(pkid);
            return Content(JsonConvert.SerializeObject(entity));
        }

        public ActionResult WechatSubscribeForm()
        {
            return View();
        }

        /// <summary>
        /// 保存关注回复信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ActionResult SaveWechatSubsc(WXEventMsgConfigEntity entity)
        {
            RepositoryManager manager = new RepositoryManager();
            if (entity?.PKID <= 0)
            {
                entity.CreateDateTime = DateTime.Now;
                entity.LastUpdateDateTime = DateTime.Now;
                entity.IsEnabled = true;
                manager.Add<WXEventMsgConfigEntity>(entity);
            }
            else
            {
                entity.LastUpdateDateTime = DateTime.Now;
                manager.Update<WXEventMsgConfigEntity>(entity);
            }

            //更新缓存
            using (var client = new WxConfigClient())
            {
                client.RefreshWXSubscribeMsgCache(entity.OriginalID);
            }
            //等待1秒，写库同步到读库
            Thread.Sleep(1000);
            return Content(JsonConvert.SerializeObject(new
            {
                state = "success",
                message = "操作成功",
                data = ""
            }));
        }

        /// <summary>
        /// 启用/禁用关注回复
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public ActionResult DeleteWechatSubsc(int pkid)
        {
            RepositoryManager manager = new RepositoryManager();
            var entity = manager.GetEntity<WXEventMsgConfigEntity>(pkid);
            entity.IsEnabled = !entity.IsEnabled;
            entity.LastUpdateDateTime = DateTime.Now;
            manager.Update<WXEventMsgConfigEntity>(entity);
            //更新缓存
            using (var client = new WxConfigClient())
            {
                client.RefreshWXSubscribeMsgCache(entity.OriginalID);
            }
            //等待1秒，写库同步到读库
            Thread.Sleep(1000);
            return Content(JsonConvert.SerializeObject(new
            {
                state = "success",
                message = "操作成功",
                data = ""
            }));
        }

        public ActionResult ReloadWechatSubsc()
        {
            using (var client = new Tuhu.Service.Config.HomePageClient())
            {
                var result = client.RefreshWXEventMsgCache();
                if (result.Result)
                {
                    return Content(JsonConvert.SerializeObject(new
                    {
                        state = "success",
                        message = "操作成功",
                        data = ""
                    }));
                }
                else
                {
                    return Content(JsonConvert.SerializeObject(new
                    {
                        state = "error",
                        message = result.Exception.Message,
                        data = ""
                    }));
                }
            }
        }

        /// <summary>
        /// 获取公众号列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetWeChatOfficialAccounts(int type = 0)
        {
            using (var client = new WeiXinPushClient())
            {
                var result = client.SelectWxConfigs();
                result.ThrowIfException(true);
                var accounts = result.Result.ToList();
                return Content(JsonConvert.SerializeObject(accounts.Where(t => type == 0 ? t.Type == "WX_Open" : t.Type == "WX_APP").Select(t => new { t.OriginalId, t.name })));
            }
        }

        #endregion 公众号关注回复配置

        /// <summary>
        /// 进入选择素材页
        /// </summary>
        /// <param name="originalId"></param>
        /// <param name="materialType"></param>
        /// <param name="isDelay">0=不延迟；1=延迟</param>
        /// <returns></returns>
        public ActionResult MaterialList(string originalId, string materialType, int isDelay)
        {
            ViewBag.OriginalId = originalId;
            ViewBag.MaterialType = materialType;
            ViewBag.IsDelay = isDelay;
            return View();
        }

        /// <summary>
        /// 获取素材列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="originalId"></param>
        /// <param name="materialType"></param>
        /// <returns></returns>
        public ActionResult GetMaterialList(Pagination pagination, string originalId, string materialType)
        {
            string accessToken = string.Empty;
            using (var client = new WeiXinPushClient())
            {
                var weChatInfo = client.SelectWxConfigs().Result.Where(t => t.OriginalId == originalId).FirstOrDefault();
                if (weChatInfo != null)
                    accessToken = client.SelectWxAccessToken(weChatInfo.channel).Result;
            }
            string requestUrl = string.Format("https://api.weixin.qq.com/cgi-bin/material/batchget_material?access_token={0}", accessToken);
            var request = System.Net.WebRequest.Create(requestUrl);
            request.Method = "POST";
            var stream = request.GetRequestStream();
            int offset = (pagination.page - 1) * pagination.rows;
            var bytes = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
            {
                type = materialType,
                offset = offset,
                count = pagination.rows
            }));
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();
            var response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string result = reader.ReadToEnd();
            response.Close();
            response.Dispose();

            #region 组装数据

            JObject json = JObject.Parse(result);

            JArray jar = JArray.Parse(json["item"].ToString());
            int totalCount = int.Parse(json["total_count"].ToString());
            int itemCount = int.Parse(json["item_count"].ToString());

            List<MaterialModel> MaterialList = new List<MaterialModel>();
            MaterialModel materialModel = null;
            for (int i = 0; i < jar.Count; i++)
            {
                materialModel = new MaterialModel();

                materialModel.MediaID = jar[i]["media_id"].ToString();
                materialModel.MediaType = materialType;

                if (materialType == "news")
                {
                    materialModel.UpdateTime = GetDateTimeWithTimeStamp(long.Parse(jar[i]["content"]["update_time"].ToString()));
                    JArray jarContent = JArray.Parse(jar[i]["content"]["news_item"].ToString());
                    for (int j = 0; j < jarContent.Count; j++)
                    {
                        materialModel.Title += jarContent[j]["title"].ToString() + "、";
                    }
                    materialModel.Title = materialModel.Title.Substring(0, materialModel.Title.Length - 1);
                }
                else if (materialType == "image")
                {
                    materialModel.Name = jar[i]["name"].ToString();
                    materialModel.URL = jar[i]["url"].ToString();
                    materialModel.UpdateTime = GetDateTimeWithTimeStamp(long.Parse(jar[i]["update_time"].ToString()));
                }
                else
                {
                    materialModel.Name = jar[i]["name"].ToString();
                    materialModel.UpdateTime = GetDateTimeWithTimeStamp(long.Parse(jar[i]["update_time"].ToString()));
                }
                MaterialList.Add(materialModel);
            }

            #endregion 组装数据

            decimal page = Convert.ToDecimal(totalCount) / pagination.rows;
            int totalPage = Convert.ToInt32(Math.Ceiling(page));

            return Content(JsonConvert.SerializeObject(new
            {
                rows = MaterialList,
                total = totalPage,
                page = pagination.page,
                records = totalCount
            }));
        }

        /// <summary>
        /// 时间戳转换为datetime
        /// </summary>
        /// <param name="timeStamp">微信接口返回时间戳</param>
        /// <returns></returns>
        private DateTime GetDateTimeWithTimeStamp(long timeStamp)
        {
            timeStamp = timeStamp * 1000;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime dt = startTime.AddMilliseconds(timeStamp);
            return dt;
        }

        #region 微信小程序客服消息配置

        public ActionResult WxAppConfig()
        {
            return View();
        }

        public ContentResult WxAppConfigList(int pageIndex = 1)
        {
            return Content(JsonConvert.SerializeObject(new WechatHomeManager().SelectAppUserEventConfig(pageIndex, 50), new JsonSerializerSettings { DateFormatString = "yyyy-MM-dd HH:mm:ss" }));
        }

        public JsonResult WxAppConfigDetails(int id)
        {
            return Json(new WechatHomeManager().FetchWxAppUserEventConfig(id), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveWxAppConfig(string configJson)
        {
            var config = JsonConvert.DeserializeObject<WxAppUserEventConfigModel>(configJson);
            //验证请求参数...
            if (string.IsNullOrWhiteSpace(config?.OriginId))
            {
                return Json("请求参数异常");
            }

            if (string.IsNullOrEmpty(config.EventType))
            {
                config.EventType = "user_enter_tempsession";
            }

            var manager = new WechatHomeManager();
            var bef = manager.FetchWxAppUserEventConfig(config.OriginId, config.EventType,
                config.UserData);
            if (bef.PKID > 0 && bef.PKID != config.PKID && "user_enter_tempsession".Equals(bef.EventType))
            {
                return Json("该小程序【用户通过客服消息按钮进入会话】消息已配置");
            }
            var result = manager.SaveWxAppUserEventConfig(config);
            if (result > 0)
            {
                LoggerManager.InsertOplog(new ConfigHistory
                {
                    AfterValue = configJson,
                    Author = ThreadIdentity.Operator.UserName,
                    ObjectType = "WxAppConf",
                    ObjectID = config.PKID.ToString(),
                    Operation = $"{(config.PKID > 0 ? "更新" : "新增")}小程序配置"
                });
                return Json("");
            }
            return Json("保存失败");
        }

        [HttpPost]
        public JsonResult DeleteWxAppConfig(int pkid)
        {
            if (pkid <= 0)
            {
                return Json("请求参数异常");
            }

            var result = new WechatHomeManager().DeleteWxAppConfig(new WxAppUserEventConfigModel
            {
                PKID = pkid
            });
            LoggerManager.InsertOplog(new ConfigHistory
            {
                BeforeValue = $"ConfigId:{pkid}",
                AfterValue = result.ToString(),
                Operation = "删除小程序配置",
                Author = ThreadIdentity.Operator.UserName,
                ObjectType = "WxAppConf"
            });
            return Json(result > 0 ? "" : "操作失败");
        }

        #endregion 微信小程序客服消息配置

        #region 微信社交立减金配置

        /// <summary>
        /// 微信社交立减金卡券配置
        /// </summary>
        /// <returns></returns>
        public ActionResult WechatSocialCardConfig()
        {
            return View();
        }

        public ActionResult WechatSocialCardConfigList(int pageIndex, int pageSize)
        {
            var list = new WechatHomeManager().SelectWechatSocialCardConfig(pageIndex, pageSize);
            return Content(JsonConvert.SerializeObject(list, new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            }), "application/json");
        }

        [HttpPost]
        public JsonResult SaveWechatSocialCardConfig(WechatSocialCardConfigModel model)
        {
            model.CardId = string.Empty;

            //0.验证请求参数
            if (!ModelState.IsValid)
            {
                return Json("请求参数异常");
            }

            if (model.BeginTime <= DateTime.Now)
            {
                return Json("开始时间必须大于当前时间");
            }
            if (model.CardDateInfoType.Equals("DATE_TYPE_FIX_TIME_RANGE"))
            {
                if (!model.EndTime.HasValue)
                {
                    return Json("卡券有效期结束时间不能为空");
                }
                if (model.EndTime <= model.BeginTime)
                {
                    return Json("卡券有效期结束时间必须大于当前时间");
                }
            }
            else
            {
                if (model.FixedTerm <= 0)
                {
                    return Json("卡券有效期信息有误");
                }
            }
            if (model.CardCondition < model.CardAmount)
            {
                return Json("使用条件须大于卡券面额");
            }

            var manager = new WechatHomeManager();
            var accessToken = GetSocialWechatAccessToken();
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return Json("获取accessToken失败");
            }
            //1. 根据model生成微信请求参数
            try
            {
                var requestEntity = new WechatSocialCardRequest
                {
                    card = new Card
                    {
                        cash = new Cash
                        {
                            base_info = new Base_Info
                            {
                                brand_name = model.MerchantName,
                                logo_url = model.LogoUrl,
                                pay_info = new Pay_Info
                                {
                                    swipe_card = new Swipe_Card
                                    {
                                        create_mid = MerchantNo,
                                        use_mid_list = new[] { MerchantNo },
                                        is_swipe_card = true
                                    }
                                },
                                code_type = "CODE_TYPE_NONE",
                                title = model.CardName,
                                service_phone = model.CutomerServiceTelphone,
                                description = model.CardDescription,
                                can_give_friend = model.IsCanGiveToFriend,
                                can_share = model.IsCanShare,
                                center_title = model.CardButtonText,
                                center_app_brand_user_name = model.CardButtonToWxApp,
                                center_app_brand_pass = model.CardButtonToPath,
                                color = model.CardColor,
                                sku = new Sku
                                {
                                    quantity = model.CardStockQuantity
                                },
                                get_limit = model.CardGetLimit,
                                date_info = new Date_Info
                                {
                                    type = model.CardDateInfoType,
                                    begin_timestamp = GetTimestamp(model.BeginTime, true),
                                    end_timestamp = GetTimestamp(model.FixedTerm > 0 ? model.EndDateTime : model.EndTime, true),
                                    fixed_term = model.FixedTerm == 0 ? (int?)null : model.FixedTerm,
                                    fixed_begin_term = model.FixedTerm == 0 ? (int?)null : model.FixedBeginTerm
                                }
                            },
                            advanced_info = new Advanced_Info
                            {
                                use_condition = new Use_Condition
                                {
                                    can_use_with_other_discount = model.IsCanUseWithOtherDiscount,
                                    least_cost = model.CardCondition
                                }
                            },
                            reduce_cost = model.CardAmount
                        }
                    }
                };
                var client = new RestClient("https://api.weixin.qq.com");
                var request =
                    new RestRequest($"/card/create?access_token={accessToken}", Method.POST)
                    {
                        JsonSerializer = NewtonsoftJsonSerializer.Default
                    };
                request.AddJsonBody(requestEntity);
                var response = client.Execute(request);
                var jObject = JsonConvert.DeserializeObject<JObject>(response.Content);
                if (jObject["errcode"].Value<int>() == 0)
                {
                    model.CardId = jObject["card_id"].Value<string>();
                }
                else
                {
                    return Json($"创建微信代金券失败：{response.Content}");
                }
            }
            catch (Exception ex)
            {
                // 调用微信创建代金券失败
                return Json($"创建微信代金券失败：{ex.Message}");
            }
            //2. 调用微信接口创建微信社交立减金代金券
            if (string.IsNullOrWhiteSpace(model.CardId))
            {
                return Json("创建微信卡券失败");
            }
            //3. 保存配置到数据库并记录日志
            model.MerchantNo = MerchantNo;
            var result = manager.AddWechatSocialCardConfig(model);
            if (result > 0)
            {
                // 保存成功
                using (var client = new ConfigLogClient())
                {
                    client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = model.CardId,
                        ObjectType = "WechatSocialCardConfig",
                        BeforeValue = "",
                        AfterValue = JsonConvert.SerializeObject(model),
                        Remark = $"新增微信社交立减金卡券配置--{model.CardId}",
                        Creator = ThreadIdentity.Operator.UserName
                    }));
                }

                return Json("");
            }
            return Json("操作失败");
        }

        /// <summary>
        /// 微信社交立减金活动配置
        /// </summary>
        /// <returns></returns>
        public ActionResult WechatSocialActivityConfig()
        {
            return View();
        }

        public ActionResult WechatSocialActivityConfigList(int pageIndex, int pageSize)
        {
            var list = new WechatHomeManager().SelectWechatSocialActivityConfig(pageIndex, pageSize);
            return Content(JsonConvert.SerializeObject(list, new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            }), "application/json");
        }

        [HttpPost]
        public JsonResult SaveWechatSocialActivityConfig(WechatSocialActivityConfigModel model)
        {
            model.ActivityId = string.Empty;
            //0.验证请求参数
            if (!ModelState.IsValid)
            {
                return Json("请求参数异常");
            }
            //小程序校验
            if (!SocialWxApp.Any(_ => _.AppId.Equals(model.ActivityWxAppId)))
            {
                return Json("微信小程序异常，请重新选择小程序");
            }
            if (model.BeginTime <= DateTime.Now)
            {
                return Json("活动开始时间必须大于当前时间");
            }
            if (model.EndTime <= model.BeginTime)
            {
                return Json("活动结束时间必须大于开始时间");
            }
            //
            var manager = new WechatHomeManager();
            var cardInfo = manager.FetchWxAppByCardId(model.CardId);
            if (null == cardInfo)
            {
                return Json("卡券不存在，请检查卡券是否填写有误");
            }

            var cardEndTime = cardInfo.FixedTerm > 0 && !cardInfo.EndTime.HasValue
                ? cardInfo.BeginTime.AddDays(cardInfo.FixedTerm) : cardInfo.EndTime.GetValueOrDefault();
            if ((cardEndTime - model.EndTime).Hours < 2)
            {
                return Json("活动结束时间必须早于卡券结束时间至少两小时，卡券没有结束时间按开始时间加领取后几天内有效天数计算结束时间");
            }
            var accessToken = GetSocialWechatAccessToken();
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return Json("获取accessToken失败");
            }
            //1. 根据model生成微信请求参数
            //2. 调用微信接口创建微信社交立减金领取代金券活动
            try
            {
                var requestEntity = new WechatSocialActivityRequest
                {
                    info = new Info
                    {
                        basic_info = new Basic_Info
                        {
                            activity_bg_color = model.ActivityBgColor,
                            activity_tinyappid = model.ActivityWxAppId,
                            begin_time = GetTimestamp(model.BeginTime, true),
                            end_time = GetTimestamp(model.EndTime, true),
                            gift_num = model.GiftNum,
                            max_partic_times_act = model.MaxParticTimesAct,
                            max_partic_times_one_day = model.MaxParticTimesOneDay,
                            mch_code = MerchantNo
                        },
                        card_info_list = new[]
                        {
                            new Card_Info_List
                            {
                                card_id = model.CardId,
                                min_amt = model.MinAmount,
                                total_user = model.UserScope>0?(bool?)null:true,
                                new_tinyapp_user = model.UserScope>0?true:(bool?)null
                            }
                        }
                    }
                };
                var client = new RestClient("https://api.weixin.qq.com");
                var request = new RestRequest($"/card/mkt/activity/create?access_token={accessToken}", Method.POST);
                request.AddJsonBody(requestEntity);
                var response = client.Execute(request);
                var jObject = JsonConvert.DeserializeObject<JObject>(response.Content);
                if (jObject["errcode"].Value<int>() == 0)
                {
                    model.ActivityId = jObject["activity_id"].Value<string>();
                }
                else
                {
                    return Json($"创建社交立减金活动失败：{response.Content}");
                }
            }
            catch (Exception ex)
            {
                return Json($"创建社交立减金活动失败,{ex.Message}");
            }
            if (string.IsNullOrEmpty(model.ActivityId))
            {
                return Json("创建社交立减金活动失败");
            }
            //3. 保存配置到数据库并记录日志
            model.MerchantNo = MerchantNo;
            var result = manager.AddWechatSocialActivityConfig(model);
            if (result > 0)
            {
                using (var client = new ConfigLogClient())
                {
                    client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = model.ActivityId,
                        ObjectType = "WechatSocialActivityConfig",
                        BeforeValue = "",
                        AfterValue = JsonConvert.SerializeObject(model),
                        Remark = $"新增社交立减金活动配置--{model.ActivityId}",
                        Creator = ThreadIdentity.Operator.UserName
                    }));
                }
                return Json("");
            }
            return Json("操作失败");
        }

        /// <summary>
        /// 获取社交立减金的小程序
        /// </summary>
        /// <returns></returns>
        public JsonResult GetSocialWxAppList() => Json(SocialWxApp, JsonRequestBehavior.AllowGet);

        /// <summary>
        /// 社交立减金商户号
        /// </summary>
        private const string MerchantNo = "1426920202";

        /// <summary>
        /// 社交立减金小程序
        /// </summary>
        private static readonly IReadOnlyCollection<WxAppEntity> SocialWxApp =
            new[]
            {
                new WxAppEntity("途虎养车", "gh_513038890d99", "wx27d20205249c56a3"),
                new WxAppEntity("途虎拼团购", "gh_45b5ae08e697","wx25f9f129712845af"),
                new WxAppEntity("途虎洗车", "gh_9d3cedff1f68","wxfa83eefa43f2c0e9")
            };

        private static string GetSocialWechatAccessToken()
        {
            using (var client = new WeiXinServiceClient())
            {
                var response = client.GetAccessTokenByPlatform(11);
                return response.Result?.access_token;
            }
        }

        private static uint GetTimestamp(DateTime dateTime, bool isSecond)
        {
            var span = dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToUInt32(isSecond ? span.TotalSeconds : span.TotalMilliseconds);
        }

        private static uint? GetTimestamp(DateTime? dateTime, bool isSecond)
        {
            if (!dateTime.HasValue)
            {
                return null;
            }
            var span = dateTime.Value.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToUInt32(isSecond ? span.TotalSeconds : span.TotalMilliseconds);
        }

        internal class WxAppEntity
        {
            /// <summary>
            /// 显示名称
            /// </summary>
            public string DisplayName { get; }

            /// <summary>
            /// 原始id
            /// </summary>
            public string OriginalId { get; }

            public string AppId { get; }

            public WxAppEntity(string displayName, string originalId, string appId)
            {
                DisplayName = displayName;
                OriginalId = originalId;
                AppId = appId;
            }
        }

        internal class NewtonsoftJsonSerializer : ISerializer
        {
            private readonly JsonSerializerSettings _serializerSetting;

            private NewtonsoftJsonSerializer(JsonSerializerSettings serializerSettings) => _serializerSetting = serializerSettings;

            public string ContentType
            {
                get => "application/json";
                set
                {
                }
            }

            public string DateFormat { get; set; }

            public string Namespace { get; set; }

            public string RootElement { get; set; }

            public string Serialize(object obj) => JsonConvert.SerializeObject(obj, Formatting.None, _serializerSetting);

            public static NewtonsoftJsonSerializer Default => new NewtonsoftJsonSerializer
            (new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        #endregion 微信社交立减金配置

        #region 微信 服务号 二维码

        public ActionResult WX_QRCodeConfigList()
        {
            return View();
        }

        public ActionResult GetWX_QRCodeConfigList(string originalId)
        {
            RepositoryManager manager = new Business.RepositoryManager();
            System.Linq.Expressions.Expression<Func<WX_QRCodeConfigEntity, bool>> exp = _ => _.OriginalID == originalId;
            var list = manager.GetEntityList<WX_QRCodeConfigEntity>(exp).OrderByDescending(p => p.CreateDateTime);
            if (list != null)
            {
                var treeList = new List<TreeGridModel>();
                foreach (WX_QRCodeConfigEntity item in list)
                {
                    var model = ObjectMapper.ConvertTo<WX_QRCodeConfigEntity, WX_QRCodeConfigModel>(item);
                    model.Expire_Date = model.LastUpdateDateTime.AddSeconds(model.expire_seconds);
                    TreeGridModel treeModel = new TreeGridModel();
                    treeModel.id = item.PKID.ToString();
                    bool hasChildren = false;
                    treeModel.isLeaf = hasChildren;
                    treeModel.expanded = false;
                    treeModel.parentId = "0";
                    treeModel.entityJson = JsonConvert.SerializeObject(model);

                    treeList.Add(treeModel);
                }
                return Content(treeList.TreeGridJson());
            }
            else
                return Content("null");
        }

        /// <summary>
        /// 跳转到  微信二维码 详情页
        /// </summary>
        /// <returns></returns>
        public ActionResult WX_QRCodeConfigForm()
        {
            return View();
        }

        /// <summary>
        /// 获取微信菜单详情
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public ActionResult GetWX_QRCodeConfigEntity(int pkid, string originalId)
        {
            RepositoryManager manager = new RepositoryManager();
            var mappingManager = new WXMenuMaterialMappingManager();
            var materialTextManager = new WXMaterialTextManager();

            WX_QRCodeConfigModel model = new WX_QRCodeConfigModel();

            //微信菜单信息
            var entity = manager.GetEntity<WX_QRCodeConfigEntity>(pkid) ?? new WX_QRCodeConfigEntity();
            if (pkid == 0)//初始化 对象
            {
                entity.Type = "scene_id";
                var maxEntity = manager.GetEntityList<WX_QRCodeConfigEntity>(p => p.OriginalID == originalId && p.Type == "scene_id").OrderByDescending(p => p.PKID).FirstOrDefault();
                entity.Scene = (Convert.ToInt32(maxEntity == null ? "0" : maxEntity.Scene) + 1).ToString();//取该 服务号的 secen 最大值 +1
                entity.action_name = "QR_STR_SCENE";
                entity.expire_seconds = 30;
                entity.OriginalID = originalId;
                entity.CreateDateTime = DateTime.Now;
            }

            //对象赋值
            model = ObjectMapper.ConvertTo<WX_QRCodeConfigEntity, WX_QRCodeConfigModel>(entity);
            model.Expire_Date = model.LastUpdateDateTime.AddSeconds(model.expire_seconds);

            #region 菜单关联的素材信息

            model.MaterialList = new List<MaterialModel>();
            MaterialModel materialModel = null;

            model.MaterialTextList = new List<WXMaterialTextModel>();
            WXMaterialTextModel materialTextModel = null;

            var menuMaterialMappingList = mappingManager.GetWXMenuMaterialMappingList(entity.PKID, "scan");
            var materialTextList = materialTextManager.GetWXMaterialTextList(entity.PKID, "scan");

            foreach (var item in menuMaterialMappingList)
            {
                materialModel = new MaterialModel();

                if (item.MediaType == "text")//文字素材
                {
                    materialTextModel = new WXMaterialTextModel();
                    var materialTextInfo = materialTextList.Where(t => t.PKID == item.MaterialID).FirstOrDefault();
                    if (materialTextInfo != null)
                    {
                        materialTextModel.Content = materialTextInfo.Content;
                        model.MaterialTextList.Add(materialTextModel);
                    }
                }
                else//其他素材
                {
                    materialModel.PKID = Convert.ToInt32(item.PKID);
                    materialModel.Title = item.Title;
                    materialModel.MediaID = item.MediaID;
                    materialModel.MediaType = item.MediaType;

                    model.MaterialList.Add(materialModel);
                }
            }

            #endregion 菜单关联的素材信息

            return Content(JsonConvert.SerializeObject(model));
        }

        /// <summary>
        /// 保存 微信二维码
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ActionResult SubmitWX_QRCodeConfig(WX_QRCodeConfigModel model)
        {
            var mappingManager = new WXMenuMaterialMappingManager();
            var materialTextManager = new WXMaterialTextManager();
            RepositoryManager manager = new RepositoryManager();
            string BeforeValue = "";
            string message = "";
            try
            {
                #region 提交数据 验证 只针对新增

                //类型 和 过期 时间
                model.expire_seconds = Convert.ToInt64((model.Expire_Date - DateTime.Now).TotalSeconds);
                model.expire_seconds = model.expire_seconds < 30 ? 30 : model.expire_seconds;
                if (model.action_name == "QR_STR_SCENE" && model.expire_seconds > 2592000)//临时
                {
                    return Content(JsonConvert.SerializeObject(new { state = "error", message = "临时二维码有效期最大值30天" }));
                }
                WX_QRCodeConfigEntity entity = ObjectMapper.ConvertTo<WX_QRCodeConfigModel, WX_QRCodeConfigEntity>(model);

                #endregion 提交数据 验证 只针对新增

                //调用微信接口 生成二维码 返回二维码的 地址
                string qrCodeURL = WX_API_Create_QRcode_Forever(model.action_name, Convert.ToInt32(model.expire_seconds), model.Type, model.Scene, model.OriginalID, ref message);
                if (string.IsNullOrWhiteSpace(qrCodeURL))
                {
                    return Content(JsonConvert.SerializeObject(new { state = "error", message = message }));
                }
                entity.URL = qrCodeURL;
                entity.Userid = ThreadIdentity.Operator.UserName;
                if (entity.PKID == 0)
                {
                    entity.CreateDateTime = DateTime.Now;
                    entity.LastUpdateDateTime = DateTime.Now;
                    manager.Add<WX_QRCodeConfigEntity>(entity);
                }
                else
                {
                    var ori = manager.GetEntity<WX_QRCodeConfigEntity>(model.PKID);
                    BeforeValue = JsonConvert.SerializeObject(ori);
                    entity.CreateDateTime = ori.CreateDateTime;
                    entity.LastUpdateDateTime = DateTime.Now;
                    manager.Update<WX_QRCodeConfigEntity>(entity);

                    //删除菜单和素材关联数据
                    mappingManager.DeleteWXMenuMaterialMapping(entity.PKID, "scan");
                    //删除文本素材数据
                    materialTextManager.DeleteWXMaterialText(entity.PKID, "scan");
                }

                #region 微信二维码 关联的素材自动回复

                WXMenuMaterialMappingModel mappingModel = null;
                WXMaterialTextModel textModel = null;
                //保存菜单和素材Mapping
                if (model.MaterialList != null)
                {
                    foreach (var item in model.MaterialList)
                    {
                        mappingModel = new WXMenuMaterialMappingModel();
                        mappingModel.PKID = item.PKID;
                        mappingModel.ObjectID = entity.PKID;
                        mappingModel.ObjectType = "Scan";
                        mappingModel.OriginalID = entity.OriginalID;
                        //mappingModel.MaterialID = item.MediaID;
                        mappingModel.MediaType = item.MediaType;
                        mappingModel.MediaID = item.MediaID;
                        mappingModel.Title = item.Title;
                        mappingModel.ImageUrl = item.URL;
                        //mappingModel.ReplyWay = model.ReplyWay;
                        mappingModel.ButtonKey = "";
                        mappingModel.IsDelaySend = false;

                        mappingManager.AddWXMenuMaterialMapping(mappingModel);
                    }
                }

                //保存菜单和文字素材Mapping
                if (model.MaterialTextList != null)
                {
                    foreach (var item in model.MaterialTextList)
                    {
                        textModel = new WXMaterialTextModel();
                        textModel.ObjectID = entity.PKID;
                        textModel.ObjectType = "Scan";
                        textModel.Content = HttpUtility.UrlDecode(item.Content);
                        textModel.OriginalID = entity.OriginalID;
                        materialTextManager.AddWXMaterialText(textModel);

                        mappingModel = new WXMenuMaterialMappingModel();
                        mappingModel.ObjectID = entity.PKID;
                        mappingModel.ObjectType = "Scan";
                        mappingModel.OriginalID = entity.OriginalID;
                        mappingModel.MaterialID = textModel.PKID;
                        mappingModel.MediaType = "text";
                        //mappingModel.ReplyWay = model.ReplyWay;
                        mappingModel.ButtonKey = "";
                        mappingModel.IsDelaySend = false;

                        mappingManager.AddWXMenuMaterialMapping(mappingModel);
                    }
                }

                #endregion 微信二维码 关联的素材自动回复

                var logManager = new CommonConfigLogManager();

                logManager.AddCommonConfigLogInfo(new CommonConfigLogModel
                {
                    BeforeValue = BeforeValue,
                    AfterValue = JsonConvert.SerializeObject(entity),
                    Creator = ThreadIdentity.Operator.UserName,
                    ObjectType = "WX_QRCodeConfig",
                    ObjectId = entity.PKID.ToString(),
                    CreateDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    LastUpdateDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Remark = $"{(model.PKID > 0 ? "更新" : "新增")}服务号二维码 scene = {entity.Scene}"
                });

                //更新缓存
                //using (var client = new WxConfigClient())
                //{
                //    client.RefreshWXClickMsgCache(entity.OriginalID, entity.ButtonKey);
                //}
                ////等待1秒，写库同步到读库
                //Thread.Sleep(1000);
                //byte[] fs = GetBytesFromUrl(entity.URL);
                //return File(GetBytesFromUrl(entity.URL), "image/jpeg", Guid.NewGuid().ToString() + ".jpg");
                //return File(fs, "application/octet-stream", "qrCode.jpg");
                return Content(JsonConvert.SerializeObject(new
                {
                    state = string.IsNullOrWhiteSpace(entity.URL) ? "false" : "success",
                    message = string.IsNullOrWhiteSpace(entity.URL) ? "创建二维码失败" : "操作成功",
                    data = entity.URL
                }));
            }
            catch (Exception em)
            {
                return Content(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    message = "操作失败",
                    data = em.Message
                }));
            }
        }

        #region 生成带参数的二维码

        //永久二维码请求[post]
        public static string WX_API_Create_QRcode_Forever(string action_name, int expire_seconds, string type, string scene, string originalId, ref string Message)
        {
            try
            {
                string Access_Token = string.Empty;
                using (var client = new WeiXinPushClient())
                {
                    var weChatInfo = client.SelectWxConfigs().Result.Where(t => t.OriginalId == originalId).FirstOrDefault();
                    if (weChatInfo != null)
                        Access_Token = client.SelectWxAccessToken(weChatInfo.channel).Result;
                }

                if (string.IsNullOrEmpty(Access_Token))
                    return null;
                string request_URL = "https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token={0}";
                request_URL = string.Format(request_URL, Access_Token);
                string postData = "";
                //文档 ：https://mp.weixin.qq.com/wiki?t=resource/res_main&id=mp1443433542
                if (action_name == "QR_STR_SCENE")//临时
                {
                    if (type == "scene_id")//整形
                    {
                        postData = "{\"expire_seconds\": " + expire_seconds + ",\"action_name\": \"" + action_name.Replace("STR_", "") + "\", \"action_info\": {\"scene\": {\"" + type + "\": " + scene + "}}}";
                    }
                    else
                    {
                        postData = "{\"expire_seconds\": " + expire_seconds + ",\"action_name\": \"" + action_name + "\", \"action_info\": {\"scene\": {\"" + type + "\": \"" + scene + "\"}}}";
                    }
                }
                else//永久
                {
                    if (type == "scene_id")//整形
                    {
                        postData = "{\"action_name\": \"" + action_name.Replace("STR_", "") + "\", \"action_info\": {\"scene\": {\"" + type + "\": " + scene + "}}}";
                    }
                    else
                    {
                        postData = "{\"action_name\": \"" + action_name + "\", \"action_info\": {\"scene\": {\"" + type + "\": \"" + scene + "\"}}}";
                    }
                }
                string rtnStr = HttpHelper.HttpPost(request_URL, postData);
                string ticket = JsonHelper.GetJsonValue_JObject(rtnStr, "ticket");
                if (string.IsNullOrWhiteSpace(ticket))
                {
                    Message = JsonHelper.GetJsonValue_JObject(rtnStr, "errmsg");
                    return null;
                }
                string qrCodeURL = WX_API_Show_QRcode(ticket, scene);
                return qrCodeURL;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return null;
            }
        }

        //通过ticket换取二维码[get]【此处的图片请求地址不要用微信的登录验证 所以不用下载到本地 可以直接使用】
        private static string WX_API_Show_QRcode(string ticket, string scene_id)
        {
            if (string.IsNullOrEmpty(ticket))
                return null;
            //记得进行UrlEncode
            ticket = HttpUtility.UrlEncode(ticket, System.Text.Encoding.UTF8);

            string request_URL = "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket={0}";
            request_URL = string.Format(request_URL, ticket);
            //DownLoadPicture(request_URL, "", scene_id);
            return request_URL;
        }

        static public byte[] GetBytesFromUrl(string url)
        {
            byte[] b;
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
            WebResponse myResp = myReq.GetResponse();

            Stream stream = myResp.GetResponseStream();
            //int i;
            using (BinaryReader br = new BinaryReader(stream))
            {
                //i = (int)(stream.Length);
                b = br.ReadBytes(500000);
                br.Close();
            }
            myResp.Close();
            return b;
        }

        #endregion 生成带参数的二维码

        #endregion 微信 服务号 二维码
    }
}