using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.Promotion;
using Tuhu.Provisioning.Business.Setting;
using Tuhu.Provisioning.DataAccess.Entity;
using System.Linq;
using Tuhu.Provisioning.DataAccess.Mapping;
using System.Linq.Expressions;

namespace Tuhu.Provisioning.Controllers
{
    public class PromotionActivityController : Controller
    {
        #region 老版本
        private readonly ISettingManager settingManager = new SettingManager();

        public ActionResult Index()
        {
            List<CouponActivityModel> couponSettingModelList = settingManager.SelectAllCouponActivity();
            ViewBag.CouponSettingModelList = couponSettingModelList;
            return View();
        }

        public ActionResult IndexV1()
        {
            List<CouponActivityModel> couponSettingModelList = settingManager.SelectAllCouponActivityV1();
            ViewBag.CouponSettingModelList = couponSettingModelList;
            return View();
        }

        [HttpGet]
        public ActionResult Edit(Guid? activityID)
        {
            ViewBag.CouponTypes = settingManager.SelectAllPromotionRules();

            if (activityID == null)
            {
                ViewBag.Title = "添加优惠券活动页";
                ViewBag.ActivityType = 0;
                return View();
            }
            else
            {
                ViewBag.Title = "编辑优惠券活动页";
                CouponActivityModel model = settingManager.FetchCouponActivity(activityID.Value);
                ViewBag.ActivityType = model.ActivityType;
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult EditV1(Guid? activityID)
        {
            ViewBag.CouponTypes = settingManager.SelectAllPromotionRules();

            if (activityID == null)
            {
                ViewBag.Title = "添加优惠券活动页";
                ViewBag.ActivityType = 0;
                return View();
            }
            else
            {
                ViewBag.Title = "编辑优惠券活动页";
                CouponActivityModel model = settingManager.FetchCouponActivity(activityID.Value);
                ViewBag.ActivityType = model.ActivityType;
                return View(model);
            }
        }

        /// <summary>
        /// 添加/修改(活动页)
        /// </summary>
        /// <param name="couponSettingStr"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Guid? activityID, string data)
        {
            var model = JsonConvert.DeserializeObject<CouponActivityModel>(data);
            if (model != null && settingManager.EditSetting(activityID, model))
            {
                return Content("1");
            }
            return Content("0");
        }
        
        [HttpPost]
        public ActionResult EditV1(Guid? activityID, string data)
        {
            var model = JsonConvert.DeserializeObject<CouponActivityModel>(data);
           
            if (model != null && settingManager.EditSettingV1(activityID, model))
            {
                return Content("1");
            }
            return Content("0");
        }
        /// <summary>
        /// 删除多表信息
        /// </summary>
        /// <param name="PKID"></param>
        /// <returns></returns>
        [HttpPost]
        public string Delete(Guid activityID)
        {
            try
            {
                if (settingManager.Delete(activityID))
                    return "1";
                else
                    return "0";
            }
            catch
            {
                return "0";
            }
        }

        public ActionResult GetDwz(string url)
        {
            using (var webClient = new WebClient())
            {
                // 采取POST方式必须加的Header
                webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                var json = webClient.UploadString("http://dwz.cn/create.php", "url=" + HttpUtility.UrlEncode(url));
                return Content(JsonConvert.DeserializeObject<JObject>(json).Value<string>("tinyurl"));
            }
        }
        #endregion


        [PowerManage]
        public ActionResult PromotionActivityList()
        {
            return View();
        }


        public ActionResult GetPromotionActivityListJson(Pagination pagination, string title,int status)
        {
            if (pagination.page == 0)
                pagination.page = 1;
            if (pagination.rows == 0)
                pagination.rows = 100;
            if (string.IsNullOrWhiteSpace(pagination.sidx))
                pagination.sidx = "CreateDateTime desc";
            pagination.sord = "asc";
            RepositoryManager repository = new RepositoryManager();
            List<SEGetPromotionActivityConfig> list = null;
            if (!string.IsNullOrWhiteSpace(title))
            {
                Expression<Func<SEGetPromotionActivityConfig, bool>> exp = _ => _.ActivityName.Contains(title);
                list = repository.GetEntityList<SEGetPromotionActivityConfig>(exp, pagination);
                list?.Where(_ => {
                    return status == 1 ? (_.StartDateTime <= DateTime.Now && _.EndDateTime > DateTime.Now) : (status == 2 ? (_.EndDateTime <= DateTime.Now) : (_.EndDateTime > DateTime.Now));
                });
            }
            else
            {
                list = repository.GetEntityList<SEGetPromotionActivityConfig>(pagination);
            }
            PromotionActivityManager manager = new PromotionActivityManager();
            if (list != null)
                list.ForEach(_ =>
                {
                    _.StatusText = _.Status ? "启用" : "禁用";
                    _.NewUserText = _.IsNewUser ? "新用户" : "全部";
                    _.GetCouponNumbers = manager.GetCouponHad(_.ID.Value);
                    var items = manager.GetEntity(_.ID.Value).CouponItems;
                    if (items != null && items.Count() > 0)
                    {
                        _.GetCouponTotal = items.FirstOrDefault().Quantity;
                    }
                    _.GetCouponTotal = _.GetCouponTotal != null ? (_.GetCouponTotal - _.GetCouponNumbers) : null;

                    _.Uri = HttpContext.Request.Headers["Host"].Contains(".cn") ? "https://wx.tuhu.cn/PromotionActivity/Coupon/" + _.ID + ".html" : "http://wx.tuhu.work/PromotionActivity/Coupon/" + _.ID + ".html";
                    _.Uri = string.Format("<a href=\"{0}\" target=\"_bank\" >{0}</a> ", _.Uri);
                });
            return Content(JsonConvert.SerializeObject(new
            {
                total = pagination.records,
                rows = list
            }));
        }


        public ActionResult GetPromotionActivityJson(Pagination pagination)
        {
            if (pagination.page == 0)
                pagination.page = 1;
            if (pagination.rows == 0)
                pagination.rows = 20;
            if (string.IsNullOrWhiteSpace(pagination.sidx))
                pagination.sidx = "CreateDateTime desc";
            pagination.sord = "asc";
            RepositoryManager repository = new RepositoryManager();
           var list =  repository.GetEntityList<SEGetPromotionActivityConfig>(pagination);
            PromotionActivityManager manager = new PromotionActivityManager();
            if (list != null)
                list.ForEach(_ =>
                {
                    _.StatusText = _.Status ? "启用" : "禁用";
                    _.NewUserText = _.IsNewUser ? "新用户" : "全部";
                    _.GetCouponNumbers = manager.GetCouponHad(_.ID.Value);
                    var items = manager.GetEntity(_.ID.Value).CouponItems;
                    if (items != null && items.Count() > 0)
                    {
                        _.GetCouponTotal = items.FirstOrDefault().Quantity;
                    }
                    _.GetCouponTotal = _.GetCouponTotal != null ? (_.GetCouponTotal - _.GetCouponNumbers) : null;

                    _.Uri = HttpContext.Request.Headers["Host"].Contains(".cn") ? "https://wx.tuhu.cn/PromotionActivity/Coupon/" + _.ID + ".html" : "http://wx.tuhu.work/PromotionActivity/Coupon/" + _.ID + ".html";
                    _.Uri = string.Format("<a href=\"{0}\" target=\"_bank\" >{0}</a> ", _.Uri);
                });
            return Content(JsonConvert.SerializeObject(new
            {
                total = pagination.records,
                rows = list
            }));
             
        }

       
        public ActionResult PromotionEdit(Guid? ID)
        {
            ViewBag.Count = 0;
            if (ID == null)
            {
                return View();
            }
            else
            {
                PromotionActivityManager manager = new PromotionActivityManager();
                SE_GetPromotionActivityConfig model = manager.GetEntity(ID.Value);
                ViewBag.Count = manager.GetPromotionActivityCountByID(ID.Value);
                return View(model);
            }
           
        }

       
        public ActionResult CouponInfo(SE_GetPromotionActivityCouponInfoConfig model)
        {
            return View(model);
        }


        public ActionResult GetCouponValidate(Guid? guid)
        {
            if (!guid.HasValue)
            {
                return Json(null);
            }
            else
            {
                PromotionActivityManager manager = new PromotionActivityManager();
                SE_GetPromotionActivityCouponInfoConfig model = manager.GetCouponValidate(guid.Value);
                if (model.SingleQuantity != 1) {
                    return Json(null);
                }
               return Json(JsonConvert.SerializeObject(model));
            }
        }

        [ValidateInput(false)]
        public ActionResult SubmitPromitionActivity(SE_GetPromotionActivityConfig model,string CouponItems) {
            if (!string.IsNullOrWhiteSpace(CouponItems))
            {
                model.CouponItems = JsonConvert.DeserializeObject<IEnumerable<SE_GetPromotionActivityCouponInfoConfig>>(CouponItems);
                PromotionActivityManager manager = new PromotionActivityManager();
                if (model.ID == null)
                    model.CreatorUser = User.Identity.Name;
                else
                    model.UpdateUser = User.Identity.Name;
                if (manager.Save(model))
                    return Json(1);
                else
                    return Json(0);
            }
            else
                return Json(0);
        }


        public ActionResult PromitionActivityDelete(Guid? ID)
        {
            if (ID.HasValue)
            {
                PromotionActivityManager manager = new PromotionActivityManager();
                if (manager.Delete(ID.Value))
                {
                    return Json(1);
                }
                else
                    return Json(0);
            }
            return Json(0);
        }

    }
}
