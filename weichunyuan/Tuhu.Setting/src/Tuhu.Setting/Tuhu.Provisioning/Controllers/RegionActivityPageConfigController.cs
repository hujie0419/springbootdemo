using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class RegionActivityPageConfigController : Controller
    {
        private static readonly Lazy<RegionActivityPageConfigManager> lazy = new Lazy<RegionActivityPageConfigManager>();

        private static readonly Lazy<OprLogManager> lazyOprLog = new Lazy<OprLogManager>();

        private RegionActivityPageConfigManager RegionActivityPageConfigManager
        {
            get
            {
                return lazy.Value;
            }
        }

        private static readonly Lazy<MeiRongAcitivityConfigManager> lazy1 = new Lazy<MeiRongAcitivityConfigManager>();

        private MeiRongAcitivityConfigManager MeiRongAcitivityConfigManager
        {
            get
            {
                return lazy1.Value;
            }
        }
        private OprLogManager OprLogManager
        {
            get
            {
                return lazyOprLog.Value;
            }
        }

        public ActionResult Index(ActivityPageConfig model, int pageIndex = 1, int pageSize = 20)
        {

            int count = 0;
            string strSql = string.Empty;
            var lists = RegionActivityPageConfigManager.GetActivityPageConfig(model, pageSize, pageIndex, out count);

            var list = new OutData<List<ActivityPageConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            return View(new ListModel<ActivityPageConfig>(list.ReturnValue, pager));
        }

        public ActionResult Region(int num, int id = 0)
        {
            ViewBag.ProvinceList = MeiRongAcitivityConfigManager.GetRegion(0);
            ViewBag.Num = num;
            ViewBag.UrlId = id;
            if (id == 0)
            {
                return View(new List<ActivityPageRegionConfig>());
            }
            else
            {
                return View(RegionActivityPageConfigManager.GetRegionRelationGroup(id));
            }
        }

        public ActionResult Edit(int id = 0)
        {
            ViewBag.Id = id;
            if (id == 0)
            {
                var model = new ActivityPageConfig();
                model.StartTime = DateTime.Now;
                model.EndTime = DateTime.Now.AddDays(30);
                ViewBag.ActivityPageUrlConfig = new List<ActivityPageUrlConfig>();
                return View(model);
            }
            else
            {
                ActivityPageConfig model = RegionActivityPageConfigManager.GetActivityPageConfigById(id);
                ViewBag.ActivityPageUrlConfig = RegionActivityPageConfigManager.GetActivityPageUrlConfigById(id);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(string data1, string data2)
        {
            try
            {
                ActivityPageConfig model = JsonConvert.DeserializeObject<ActivityPageConfig>(data1);
                string name = string.IsNullOrWhiteSpace(User.Identity.Name) ? "途虎系统" : User.Identity.Name;
                if (model.Id != 0)
                {
                    model.UpdateName = name;
                    model.UpdateName = name;
                    if (RegionActivityPageConfigManager.UpdateActivityPageConfig(model))
                    {
                        EditModel(data2, model.Id, model);
                        AddOprLog(model, "修改");
                        return Json(true);
                    }
                    else
                    {
                        return Json(false);
                    }
                }
                else
                {
                    string domain = Request.Url.Host.Contains("tuhu.cn") ? "http://res.tuhu.org" : "http://resource.tuhu.test";
                    int newid = 0;
                    model.CreateName = name;
                    model.UpdateName = name;
                    if (RegionActivityPageConfigManager.InsertActivityPageConfig(model, ref newid))
                    {
                        //默认添加分享链接
                        ShareParams share = JsonConvert.DeserializeObject<ShareParams>(model.ShareParameters);
                        if (string.IsNullOrWhiteSpace(share.URL))
                        {
                            share.URL = domain + "/StaticPage/pictures/adaptImgs.html?id=" + EncodeBase64(newid.ToString());
                        }
                        if (share.shareUrl == "&type=2&utm_source=1")
                        {
                            share.shareUrl = domain + "/StaticPage/pictures/adaptImgs.html?id=" + EncodeBase64(newid.ToString()) + "&type=2&utm_source=1";
                        }
                        model.ShareParameters = JsonConvert.SerializeObject(share);
                        model.Id = newid;
                        RegionActivityPageConfigManager.UpdateActivityPageConfig(model);

                        model.Id = newid;
                        EditModel(data2, model.Id, model);
                        AddOprLog(model, "添加");
                        return Json(true);
                    }
                    else
                    {
                        return Json(false);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(false);
                throw ex;
            }

        }

        public static string EncodeBase64(string source)
        {
            byte[] bytes = Encoding.Default.GetBytes(source);
            return Convert.ToBase64String(bytes);
        }

        public bool EditModel(string data, int id, ActivityPageConfig model)
        {
            if (string.IsNullOrWhiteSpace(data) || id <= 0 || model == null)
            {
                return false;
            }
            List<ActivityPageUrlConfig> list = JsonConvert.DeserializeObject<List<ActivityPageUrlConfig>>(data);
            foreach (var item in list)
            {
                if (item.Id == 0)
                {
                    int outid = 0;
                    item.ActivityPageId = id;
                    RegionActivityPageConfigManager.InsertActivityPageUrlConfig(item, ref outid);
                    if (item.IsDefault)
                    {
                        model.DefaultUrlId = outid;
                        RegionActivityPageConfigManager.UpdateActivityPageConfig(model);
                    }
                    EditRegion(item.RegionString, outid);
                }
                else
                {
                    RegionActivityPageConfigManager.UpdateActivityPageUrlConfig(item);
                    if (item.IsDefault)
                    {
                        model.DefaultUrlId = item.Id;
                        RegionActivityPageConfigManager.UpdateActivityPageConfig(model);
                    }
                    EditRegion(item.RegionString, item.Id);
                }
            }
            return true;
        }

        public bool EditRegion(object data, int id)
        {
            if (data == null || id <= 0)
            {
                return false;
            }
            List<ActivityPageRegionConfig> list = JsonConvert.DeserializeObject<List<ActivityPageRegionConfig>>(data.ToString());
            RegionActivityPageConfigManager.DeleteActivityPageRegionConfig(id);
            foreach (var item in list)
            {
                item.UrlId = id;
                RegionActivityPageConfigManager.InsertActivityPageRegionConfig(item);
            }
            return true;
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            bool result = false;
            ActivityPageConfig model = new ActivityPageConfig();
            model.Id = id;

            if (RegionActivityPageConfigManager.DeleteRegionActivityPageConfig(id))
            {
                AddOprLog(model, "删除");
                result = true;
            }
            return Json(result);
        }

        [HttpPost]
        public ActionResult DeleteActivityPageUrlConfig(int id)
        {
            return Json(RegionActivityPageConfigManager.DeleteActivityPageUrlConfig(id));
        }
        public void AddOprLog(ActivityPageConfig model, string opr)
        {
            OprLog oprModel = new OprLog();
            oprModel.Author = User.Identity.Name;
            oprModel.ChangeDatetime = DateTime.Now;
            oprModel.HostName = Request.UserHostName;
            oprModel.ObjectID = model.Id;
            oprModel.ObjectType = "RAPC";
            oprModel.Operation = opr;
            OprLogManager.AddOprLog(oprModel);
        }

        public ActionResult RefreshRegionVehicleIdActivityUrlCache(string activityId)
        {
            Guid ActivityId;
            if (Guid.TryParse(activityId, out ActivityId))
            {
                using (var client = new Service.Activity.ActivityClient())
                {
                    var serviceResult = client.RefreshRegionVehicleIdActivityUrlCache(ActivityId);
                    return Json(new { Status = serviceResult.Success, Msg = serviceResult.ErrorMessage }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Status = false, Msg = "请输入有效的活动Id" }, JsonRequestBehavior.AllowGet);
        }

    }
}
