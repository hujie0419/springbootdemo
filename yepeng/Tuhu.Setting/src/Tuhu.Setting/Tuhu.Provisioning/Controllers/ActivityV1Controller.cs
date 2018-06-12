using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qiniu.IO;
using Qiniu.RS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.Activity;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business;
using System.IO;
using System.Web.Configuration;
using System.Text;
using Tuhu.Service.Member.Models;

namespace Tuhu.Provisioning.Controllers
{
    public class ActivityV1Controller : Controller
    {
        #region 活动配置
        private static string htmlActivity = string.Empty;

        private readonly Lazy<ActivityManager> lazyActivityManager = new Lazy<ActivityManager>();

        private ActivityManager ActivityManager
        {
            get { return this.lazyActivityManager.Value; }
        }

        public ActionResult Index(int id)
        {
            ActivityBuild activityBuild = ActivityManager.GetActivityBuildById(id);
            htmlActivity = GetArticleContent(activityBuild);

            return this.Content(htmlActivity);
        }

        public ActionResult Update(int id = 0)
        {
            ActivityBuild model = ActivityManager.GetActivityBuildById(id);
            ViewBag.ActivityBuild = model;
            List<ActivityContent> ContentList = JsonConvert.DeserializeObject<List<ActivityContent>>(model.Content);
            ViewBag.ContentList = ContentList;
            return View();
        }

        public ActionResult UpdateActivity(ActivityBuild activityBuild, int id)
        {
            return Json(ActivityManager.UpdateActivityBuild(activityBuild, id));

        }

        public ActionResult List(int pageIndex = 1, int id = 0, string title = "")
        {
            int count = 0;
            string strSql = string.Empty;

            if (id != 0)
            {
                strSql += " AND id = " + id;
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                strSql += " AND Title like N'%" + title + "%'";
            }
            var model = ActivityManager.GetActivityBuild(strSql, 25, pageIndex, out count);

            var list = new OutData<List<ActivityBuild>, int>(model, count);
            var pager = new PagerModel(pageIndex, 25)
            {
                TotalItem = count
            };
            return this.View(new ListModel<ActivityBuild>(list.ReturnValue, pager));

        }

        public ActionResult Add()
        {
            return View();
        }

        public ActionResult AddActivity(ActivityBuild activityBuild)
        {
            if (!string.IsNullOrEmpty(activityBuild.Content))
            {
                //string pureHtml = GetArticleContent(activityBuild);
                //byte[] array = System.Text.Encoding.UTF8.GetBytes(pureHtml);
                //Stream stream = new MemoryStream(array);
                //activityBuild.ActivityUrl = UploadFile(stream, "Activity_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".html");

                return Json(ActivityManager.InsertActivityBuild(activityBuild));
            }
            return Json(false);
        }

        #region 优惠券活动
        [PowerManage]

        public ActionResult CouponActivityConfig(string id = "-1")
        {
            CouponActivity model = ActivityManager.GetCurrentCouponActivity(id);

            return View(model);
        }

        public JsonResult SaveCouponActivityConfig(string configStr)
        {
            CouponActivity configObject = JsonConvert.DeserializeObject<CouponActivity>(configStr);

            string userName = HttpContext.User.Identity.Name;

            bool result = ActivityManager.SaveCouponActivityConfig(configObject, userName);
            return Json(result);
        }

        [PowerManage]
        public ActionResult CouponActivityWebConfig(string id = "-1")
        {
            WebCouponActivity model = ActivityManager.GetCurrentWebCouponActivity(id);
            WebCouponActivityRuleModel coupon = ActivityManager.GetCouponRule(model.PromotionRuleGUID.GetValueOrDefault()) ?? new WebCouponActivityRuleModel();
            ViewData["CouponRuleModel"] = coupon;
            return View(model);
        }

        [HttpGet]
        public ActionResult GetCouponRule(Guid? ruleGUID)
        {
            var coupon = ActivityManager.GetCouponRule(ruleGUID.GetValueOrDefault());
            var result = coupon == null ? null : new
            {
                coupon.Channel,
                coupon.Creater,
                DeadLineDate = coupon.DeadLineDate?.ToString("yyyy-MM-dd") ?? "",
                coupon.DepartmentName,
                coupon.Description,
                coupon.Discount,
                coupon.GetRuleGUID,
                coupon.IntentionName,
                coupon.MinMoney,
                coupon.PromotionName,
                coupon.RuleID,
                coupon.Term,
                ValiEndDate = coupon.ValiEndDate?.ToString("yyyy-MM-dd") ?? "",
                ValiStartDate = coupon.ValiStartDate?.ToString("yyyy-MM-dd") ?? "",
            };
            return Json(result, behavior: JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveCouponActivityWebConfig(string configStr)
        {
            WebCouponActivity configObject = JsonConvert.DeserializeObject<WebCouponActivity>(configStr);

            string userName = HttpContext.User.Identity.Name;

            bool result = ActivityManager.SaveCouponActivityWebConfig(configObject, userName);
            return Json(result);
        }

        #endregion

        protected string UploadFile(Stream stream, string fileName)
        {
            string _ReturnStr = string.Empty;
            stream.Seek(0, SeekOrigin.Begin);
            IOClient _IOClient = new IOClient();
            var _PutPolicy = new PutPolicy(WebConfigurationManager.AppSettings["Qiniu:comment_scope"], 3600).Token();
            var _Result = _IOClient.Put(_PutPolicy, fileName, stream, new PutExtra());
            if (_Result.OK)
                _ReturnStr = WebConfigurationManager.AppSettings["Qiniu:comment_url"] + fileName;
            return _ReturnStr;
        }

        public ActionResult ViewActivity(ActivityBuild activityBuild)
        {
            htmlActivity = GetArticleContent(activityBuild);

            return Json(true);

        }
        public ActionResult Preview()
        {
            return this.Content(htmlActivity);

        }

        public ActionResult DeleteActivity(int id)
        {
            return Json(ActivityManager.DeleteActivityBuild(id));

        }

        public ActionResult ActivityList(int pageIndex = 1, int pageSize = 50)
        {
            int count = 0;
            string strSql = string.Empty;

            var model = ActivityManager.GetActivityBuild(strSql, pageSize, pageIndex, out count);
            var list = new OutData<List<ActivityBuild>, int>(model, count);
            ViewBag.Total = count;
            ViewBag.pageSize = pageSize;
            ViewBag.pageNumber = pageIndex;
            return this.View(new ListModel<ActivityBuild>(list.ReturnValue));
            // return View();

        }


        /// <summary>
        /// 编辑显示整体内容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ActivityEdit(int id = 0)
        {
            ActivityBuild model = new ActivityBuild();
            if (id != 0)
            {
                model = ActivityManager.GetActivityBuildById(id);
            }
            else
            {
                model.Content = "{total:0,rows:[]}";
            }
            return View(model);
        }


        public string GetActivityBuild(ActivityBuild model)
        {
            if (!string.IsNullOrEmpty(model.id.ToString()) && model.id != 0)
            {
                model = ActivityManager.GetActivityBuildById(model.id);
                return JsonConvert.SerializeObject(model);
            }
            return "";
        }

        /// <summary>
        /// 新增或者更新活动配置信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ActivityEdit(ActivityBuild model)
        {
            if (model.id == 0)
            {
                if (!string.IsNullOrEmpty(model.Content))
                {
                    bool result = ActivityManager.InsertActivityBuild(model);
                    return Json(result);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(model.Content))
                {
                    string activityMenu = JsonConvert.SerializeObject(ActivityMenuBll.GetList(model.id.ToString()));
                    string activityHome = JsonConvert.SerializeObject(ActivityHomeBll.GetList(model.id.ToString()));
                    model.ActivityHome = activityHome;
                    model.ActivityMenu = activityMenu;

                    bool result = ActivityManager.UpdateActivityBuild(model, model.id);

                    return Json(result);
                }
            }
            return Json(false);
        }

        [HttpPost]
        public ActionResult GetProductImage(string pid)
        {
            Dictionary<string, string> url = ActivityManager.GetProductImageUrl(pid);
            if (url == null)
                return Json("{\"BImage\":\"\",\"SImage\":\"\",\"DisplayName\":\"\"}");

            StringBuilder json = new StringBuilder("{");
            string bImage = Tuhu.WebSite.Component.SystemFramework.ImageHelper.GetProductImage(url["image"], 300, 300);
            json.Append("\"BImage\":\"" + bImage + "\",");
            string sImage = Tuhu.WebSite.Component.SystemFramework.ImageHelper.GetProductImage(url["image"], 20, 20);
            json.Append("\"SImage\":\"" + sImage + "\",");
            json.Append("\"DisplayName\":\"" + url["name"] + "\"");
            json.Append("}");
            return Json(json.ToString());
        }

        #region 列表单显示
        public ActionResult One()
        {
            ActivityBuildDetail detail = null;
            return View(detail);
        }

        public ActionResult Two()
        {
            return View();
        }

        public ActionResult Edit(string rows = "")
        {
            try
            {
                // string rows = "";
                ActivityBuildDetail detail = new ActivityBuildDetail();
                if (rows != "")
                {
                    JObject json = JObject.Parse(rows);
                    detail.Group = json["Group"].ToString();
                    detail.HandlerAndroid = json["HandlerAndroid"].ToString();
                    detail.HandlerIOS = json["HandlerIOS"].ToString();
                    detail.Image = json["Image"].ToString();
                    detail.LinkUrl = json["LinkUrl"].ToString();
                    detail.OrderBy = json["OrderBy"].ToString();
                    detail.PID = json["PID"].ToString();
                    JToken value;
                    if (json.TryGetValue("SmallImage", out value))
                        detail.SmallImage = json["SmallImage"].ToString();

                    detail.SOAPAndroid = json["SOAPAndroid"].ToString();
                    detail.SOAPIOS = json["SOAPIOS"].ToString();
                    detail.Type = Convert.ToInt32(json["Type"].ToString());
                    detail.VID = json["VID"].ToString();
                    detail.BigImg = Convert.ToInt32(json["BigImg"].ToString());
                    detail.CID = json["CID"].ToString();
                    detail.IsUploading = Convert.ToInt32(json["IsUploading"]);
                    if (json.TryGetValue("Description", out value))
                        detail.Description = json["Description"].ToString();
                    else
                        detail.Description = "";

                    if (json.TryGetValue("DisplayWay", out value))
                        detail.DisplayWay = Convert.ToInt32(json["DisplayWay"]);
                    else
                    {
                        detail.DisplayWay = -1;
                    }

                    if (json.TryGetValue("TwoSImage", out value))
                    {
                        detail.TwoSImage = json["TwoSImage"].ToString();
                    }
                    else
                    {
                        detail.TwoSImage = "";
                    }

                    if (json.TryGetValue("TwoBImage", out value))
                    {
                        detail.TwoBImage = json["TwoBImage"].ToString();
                    }
                    else
                    {
                        detail.TwoBImage = "";
                    }

                    if (json.TryGetValue("WXUrl", out value))
                    {
                        detail.WXUrl = json["WXUrl"].ToString();
                    }
                    else
                    {
                        detail.WXUrl = "";
                    }


                    value = null;
                    if (json.TryGetValue("ProductName", out value))
                    {
                        detail.ProductName = json["ProductName"].ToString();
                    }
                    else
                    {
                        detail.ProductName = "";
                    }

                    #region 旧优惠券 作废
                    //if (detail.VID != "")
                    //{
                    //    StringBuilder sb = new StringBuilder("<select name=\"CID\" id=\"CID\"> ");
                    //    Dictionary<int,string> dictionary = ActivityManager.GetActivity_Coupon(detail.VID);
                    //    foreach (KeyValuePair<int,string> pair in dictionary)
                    //    {
                    //        if (pair.Key.ToString() == detail.CID)
                    //        {
                    //            sb.Append("<option value=\"" + detail.CID + "\" selected=\"selected\" >" + pair.Value + " </option>");
                    //        }
                    //        else
                    //        {
                    //            sb.Append("<option value=\"" + pair.Key + "\"  >" + pair.Value + " </option>");
                    //        }
                    //    }
                    //    sb.Append("</select >");
                    //    ViewBag.CIDHtml = sb.ToString();
                    //}
                    //else
                    //{
                    //    ViewBag.CIDHtml = "<select name=\"CID\" id=\"CID\" ></select>";
                    //}
                    #endregion

                    if (json.TryGetValue("PCUrl", out value))
                    {
                        detail.PCUrl = json["PCUrl"].ToString();
                    }
                    else
                    {
                        detail.PCUrl = "";
                    }

                }
                else
                {
                    detail = null;
                }
                return View(detail);
            }
            catch (Exception em)
            {
                throw new Exception(em.Message);
            }
        }

        public ActionResult Three()
        {
            return View();
        }

        public ActionResult Four()
        {
            return View();
        }

        public ActionResult OnePC()
        {
            return View();
        }

        #endregion

        /// <summary>
        /// 校验优惠券 作废
        /// </summary>
        /// <param name="activityID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCoupon(string activityID)
        {
            if (!string.IsNullOrEmpty(activityID))
            {
                StringBuilder json = new StringBuilder("[");
                Dictionary<int, string> dictionary = ActivityManager.GetActivity_Coupon(activityID);
                foreach (KeyValuePair<int, string> pair in dictionary)
                {
                    json.Append("{\"Value\":\"" + pair.Key + "\",\"Name\":\"" + pair.Value + "\"},");
                }

                return Json(json.ToString().TrimEnd(',') + "]");
            }
            else
                return Json("");

        }

        [HttpPost]
        public ActionResult CouponVlidate(string couponRulePKID)
        {
            return JavaScript(ActivityManager.CouponVlidate(couponRulePKID));
        }

        /// <summary>
        /// 通过PKID 校验 优惠券 (勿动)
        /// </summary>
        /// <param name="couponRulePKID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CouponVlidateForPKID(int couponRulePKID)
        {
            return JavaScript(ActivityManager.CouponVlidateForPKID(couponRulePKID));
        }


        private static string GetArticleContent(ActivityBuild model)
        {
            string pureHtml = GetPureHtml("/Content/CommonFile/ActivityTemplate.html");

            pureHtml = pureHtml.Replace("$title$", model.Title);
            var activityContent = JsonConvert.DeserializeObject<List<ActivityContent>>(model.Content);

            string content = string.Empty;
            foreach (var item in activityContent.OrderBy(x => x.OrderBy).ToList())
            {
                if (!string.IsNullOrWhiteSpace(item.PID))
                {
                    if (item.Type == 0 && item.BigImg == 1) //轮胎,1图
                    {
                        content += " <a href=\"javascript:void(0);\" class=\"img\" iostype=\"#testapp#customSegue#TNTireInfoVC#\" androidtype=\"cn.TuHu.Activity.TireInfoUI\" pid=\"" + item.PID + "\"   vid=\"" + item.VID + "\"  activityid=\"" + item.ActivityId + "\" ><img src=\"" + item.Image + "\" alt=\"\" /></a>";
                    }
                    else if (item.Type == 1 && item.BigImg == 1) //车品,1图
                    {
                        content += " <a href=\"javascript:void(0);\" class=\"img\" iostype=\"#testapp#customSegue#TNGoodsListDetailViewController#\" androidtype=\"cn.TuHu.Activity.AutomotiveProducts.AutomotiveProductsDetialUI\" pid=\"" + item.PID + "\"   vid=\"" + item.VID + "\" activityid=\"" + item.ActivityId + "\"><img src=\"" + item.Image + "\" alt=\"\" /></a>";
                    }
                    else if (item.Type == 0 && item.BigImg == 0) //轮胎,2图
                    {
                        content += " <a href=\"javascript:void(0);\" class=\"content\" iostype=\"#testapp#customSegue#TNTireInfoVC#\" androidtype=\"cn.TuHu.Activity.TireInfoUI\" pid=\"" + item.PID + "\"   vid=\"" + item.VID + "\" activityid=\"" + item.ActivityId + "\"><img src=\"" + item.Image + "\" alt=\"\" /></a>";
                    }
                    else if (item.Type == 1 && item.BigImg == 0)//车品,2图
                    {
                        content += " <a href=\"javascript:void(0);\" class=\"content\" iostype=\"#testapp#customSegue#TNGoodsListDetailViewController#\" androidtype=\"cn.TuHu.Activity.AutomotiveProducts.AutomotiveProductsDetialUI\" pid=\"" + item.PID + "\"   vid=\"" + item.VID + "\"  activityid=\"" + item.ActivityId + "\"><img src=\"" + item.Image + "\" alt=\"\" /></a>";
                    }
                    else if (item.Type == 0 && item.BigImg == 2)//轮胎,3图
                    {
                        content += " <a href=\"javascript:void(0);\" class=\"content\" iostype=\"#testapp#customSegue#TNTireInfoVC#\" androidtype=\"cn.TuHu.Activity.TireInfoUI\" pid=\"" + item.PID + "\"   vid=\"" + item.VID + "\" activityid=\"" + item.ActivityId + "\"><img class=\"img3\" src=\"" + item.Image + "\" alt=\"\" /></a>";
                    }
                    else if (item.Type == 1 && item.BigImg == 2)//轮胎,3图
                    {
                        content += " <a href=\"javascript:void(0);\" class=\"content\" iostype=\"#testapp#customSegue#TNGoodsListDetailViewController#\" androidtype=\"cn.TuHu.Activity.AutomotiveProducts.AutomotiveProductsDetialUI\" pid=\"" + item.PID + "\"   vid=\"" + item.VID + "\" activityid=\"" + item.ActivityId + "\"><img class=\"img3\" src=\"" + item.Image + "\" alt=\"\" /></a>";
                    }
                }
                else
                {
                    if (item.Type == 2 && item.BigImg == 2)//链接，3图
                    {
                        content += " <a href=\"" + item.LinkUrl + "\" class=\"content\"><img class=\"img3\" src=\"" + item.Image + "\" alt=\"\" /></a>";
                    }
                    else if (item.Type == 2 && item.BigImg == 0)//链接，2图
                    {
                        content += " <a href=\"" + item.LinkUrl + "\" class=\"content\"><img  src=\"" + item.Image + "\" alt=\"\" /></a>";
                    }
                    else if (item.Type == 2 && item.BigImg == 1)//链接，1图
                    {
                        content += " <a href=\"" + item.LinkUrl + "\" class=\"img\"><img src=\"" + item.Image + "\" alt=\"\" /></a>";
                    }
                    else if (item.Type == 3 && item.BigImg == 1)//无，1图
                    {
                        content += " <a href=\"javascript:void(0);\" class=\"img\"><img src=\"" + item.Image + "\" alt=\"\" /></a>";
                    }
                    else if (item.Type == 3 && item.BigImg == 0)//无，2图
                    {
                        content += " <a href=\"javascript:void(0);\" class=\"content\"><img  src=\"" + item.Image + "\" alt=\"\" /></a>";
                    }
                    else if (item.Type == 3 && item.BigImg == 2)//无，3图
                    {
                        content += " <a href=\"javascript:void(0);\" class=\"content\"><img class=\"img3\" src=\"" + item.Image + "\" alt=\"\" /></a>";
                    }
                }

            }

            pureHtml = pureHtml.Replace("$content$", content);
            return pureHtml;
        }

        public static string GetPureHtml(string strfile)
        {
            string strout = "";
            string fpath = System.Web.HttpContext.Current.Server.MapPath(strfile);
            if (System.IO.File.Exists(fpath))
            {
                StreamReader sr = new StreamReader(fpath, System.Text.Encoding.UTF8);
                string input = sr.ReadToEnd();
                sr.Close();
                if (isLuan(input))
                {
                    StreamReader sr2 = new StreamReader(fpath, System.Text.Encoding.Default);
                    input = sr2.ReadToEnd();
                    sr2.Close();
                }
                strout = input;
            }
            return strout;
        }

        public static bool isLuan(string txt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(txt);
            for (int i = 0; i < bytes.Length; i++)
            {
                if (i < bytes.Length - 3)
                    if (bytes[i] == 239 && bytes[i + 1] == 191 && bytes[i + 2] == 189)
                    {
                        return true;
                    }
            }
            return false;
        }

        public class ActivityContent
        {
            public string PID { get; set; }
            public string VID { get; set; }
            public string ActivityId { get; set; }
            public string Image { get; set; }
            public string SmallImage { get; set; }
            public int Type { get; set; }
            public int OrderBy { get; set; }
            public int BigImg { get; set; }
            public string LinkUrl { get; set; }
        }
        #region 活动配置列表
        [HttpGet]
        [PowerManage]
        public ActionResult ActivityConfigList(int pageIndex = 1, string type = "APP")
        {
            ViewBag.BaoYangDomain = WebConfigurationManager.AppSettings["BaoYangDomain"];
            ViewBag.WeiXinDomain = WebConfigurationManager.AppSettings["WeiXinDomain"];

            var result = ActivityManager.GetActivityList(type, 8, pageIndex);
            var model = result.Item2;
            int count = result.Item1;
            var list = new OutData<List<CouponActivity>, int>(model, count);
            var pager = new PagerModel(pageIndex, 8)
            {
                TotalItem = count
            };
            ViewBag.type = type;

            return this.View(new ListModel<CouponActivity>(list.ReturnValue, pager));
        }

        [HttpPost]
        [PowerManage]
        public JsonResult DeleteActivityConfig(string type, string id)
        {
            string userName = HttpContext.User.Identity.Name;

            return Json(ActivityManager.DeleteActivityConfig(type, id, userName));
        }
        #endregion



        #endregion

    }
}
