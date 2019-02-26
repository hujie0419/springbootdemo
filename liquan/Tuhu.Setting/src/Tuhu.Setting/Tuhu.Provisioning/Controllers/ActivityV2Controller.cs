using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qiniu.IO;
using Qiniu.RS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.Activity;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business;
using System.Threading.Tasks;
using Tuhu.Provisioning.Business.Logger;

namespace Tuhu.Provisioning.Controllers
{
    public class ActivityV2Controller : Controller
    {
        #region 活动配置
        private static string htmlActivity = string.Empty;

        private readonly Lazy<ActivityManager> lazyActivityManager = new Lazy<ActivityManager>();

        private ActivityManager ActivityManager
        {
            get { return this.lazyActivityManager.Value; }
        }

        public ActionResult DeleteActivity(int id)
        {
            bool result = ActivityManager.DeleteActivityBuild(id);
            if (result)
            {
                try
                {
                    LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = null, Author = User.Identity.Name, Operation = "删除活动:" +id.ToString(), ObjectType = "ACFV2", ObjectID = id.ToString() });
                    Business.CommonServices.CallCRMService.NewDeleteActivityBySourceId(id.ToString(), DataAccess.Entity.CommonServices.CRMSourceType.ActivityV2, User.Identity.Name);

                }
                catch (Exception em)
                {
                }
                finally
                {

                }
            }
            return Json(result);

        }

        /// <summary>
        /// 获取列表 status 1 进行中 2已结束 3即将开始
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
#if !DEBUG
        [PowerManage]
#endif
        public ActionResult ActivityList(int pageIndex = 1,int pageSize=50,string ActivityName="",int? SelActivityID=null,int status=-1)
        {
            int count = 0;
            string strSql = string.Empty;
           
            if (ActivityName.Length>0)
            {
                strSql = " and  Title LIKE N'%" + ActivityName+"%'";
            }

            if (SelActivityID.HasValue)
            {
                strSql += " and ID like '%"+SelActivityID.Value+"%'";
            }

            if (status == 1)
            {
                strSql += " AND  EndDate IS NULL OR EndDate >=GETDATE()";
            }
            else if (status == 2)
            {
                strSql += " AND  EndDate < GETDATE() ";
            }
            else 
            {
                
            }



            var model = ActivityManager.GetActivityBuild(strSql, pageSize, pageIndex, out count);
            var list = new OutData<List<ActivityBuild>, int>(model, count);
            ViewBag.Total = count;
            ViewBag.pageSize = pageSize;
            ViewBag.pageNumber = pageIndex;
             return this.View(new ListModel<ActivityBuild>(list.ReturnValue));
            // return View();
           
        }

        public ActionResult GetActivityList(int pageIndex = 1, int pageSize = 50, string ActivityName = "", int? SelActivityID = null, int status = -1)
        {
            int count = 0;
            string strSql = string.Empty;

            if (ActivityName.Length > 0)
            {
                strSql = " and  Title LIKE N'%" + ActivityName + "%'";
            }

            if (SelActivityID.HasValue)
            {
                strSql += " and ID like '%" + SelActivityID.Value + "%'";
            }

            if (status == 1)
            {
                strSql += " AND  EndDate IS NULL OR EndDate >=GETDATE()";
            }
            else if (status == 2)
            {
                strSql += " AND  EndDate < GETDATE() ";
            }
            else
            {

            }



            var model = ActivityManager.GetActivityBuild(strSql, pageSize, pageIndex, out count);
            var list = new OutData<List<ActivityBuild>, int>(model, count);
            ViewBag.Total = count;
            ViewBag.pageSize = pageSize;
            ViewBag.pageNumber = pageIndex;
            var result = new ListModel<ActivityBuild>(list.ReturnValue);
            return Content(JsonConvert.SerializeObject(new
            {
                total = count,
                rows = result.Select(o => new
                {
                    o.ActivityConfigType,
                    o.CreatetorUser,
                    o.UpdateUser,
                    o.ActivityType,
                    o.id,
                    o.Title,
                    MUrl = o.ActivityType == 0 ? System.Configuration.ConfigurationManager.AppSettings["WeiXinDomain"].ToString() + "/staticpage/activity/list.html?id=" + o.id : (o.ActivityType == 3 ? System.Configuration.ConfigurationManager.AppSettings["WeiXinDomain"].ToString() + "/Activity/tiresList?id=" + o.id  : System.Configuration.ConfigurationManager.AppSettings["WeiXinDomain"].ToString() + "/Tires/ResultLg?actid=" + o.id),
                    PCUrl = o.ActivityType == 0 ? System.Configuration.ConfigurationManager.AppSettings["ItemDomain"].ToString() + "/Activity/act/" + o.id + ".html" : (o.ActivityType == 3 ? System.Configuration.ConfigurationManager.AppSettings["WeiXinDomain"].ToString() + "/Tires?act=tire&type=autoselect&activityid=" + o.id : System.Configuration.ConfigurationManager.AppSettings["WeiXinDomain"].ToString() + "/Tires/ResultLg?actid=" + o.id),
                    EndDate = o.EndDate == null ? "" : Convert.ToDateTime(o.EndDate).ToString("yyyy-MM-dd HH:mm:ss"),
                    CreateTime = o.CreateTime == null ? "" : Convert.ToDateTime(o.CreateTime).ToString("yyyy-MM-dd HH:mm:ss")
                }).Select(p => new
                {
                    p.id,
                    p.Title,
                    MUrl =   "<a href=\"" + p.MUrl + "\" >" + p.MUrl + " </a>"+( p.ActivityType  == 0?"":(p.ActivityType == 3? "  <br/><span>H5轮胎列表页</span>" : "  <br/> <span>H5轮毂列表页</span>")),
                    PCUrl = "<a href=\"" + p.PCUrl + "\" >" + p.PCUrl + " </a>" + (p.ActivityType == 0 ? "" : (p.ActivityType == 3 ? "  <br/><span>微信轮胎列表页</span>" : "  <br/> <span>微信轮毂列表页</span>")),
                    p.EndDate,
                    p.CreateTime,
                    p.ActivityConfigType,
                    p.CreatetorUser,
                    p.UpdateUser
                })
            }));
        }
       

        /// <summary>
        /// 编辑显示整体内容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ActivityEdit(int id=0,int copy=0)
        {
            ActivityBuild model = new ActivityBuild ();
            if (id != 0)
            {
                 model = ActivityManager.GetActivityBuildById(id);
                if (string.IsNullOrWhiteSpace(model.ActivityMenu))
                {
                    model.MenuType = -1;
                }
                if (copy == 1)
                {
                    model.id = 0;
                }
            }
            else
            {
                model.MenuType = -1;
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
        [ValidateInput(false)]
        public ActionResult ActivityEdit(ActivityBuild model)
        {
            if (model.id == 0)
            {
                if (!string.IsNullOrEmpty(model.Content))
                {
                    model.CreatetorUser = User.Identity.Name;
                    bool result = ActivityManager.InsertActivityBuild(model);
                    if (result)
                    {
                        int id = ActivityManager.GetMaxID();
                        if (id > 0)
                        {
                            try
                            {
                                using (var client = new Tuhu.Service.SEO.CouchbaseRemovalClient())
                                {
                                    var resultClient = client.GetOrSetWXActivityPush(id, true);
                                    resultClient.ThrowIfException(true);

                                }
                                ActivityManager.ReloadActivity(HttpContext.Request.Headers["Host"].Contains(".tuhu.cn") ? "wx.tuhu.cn" : "wx.tuhu.work", model.id);
                                LoggerManager.InsertOplog(new ConfigHistory() { AfterValue = model.Content, Author = User.Identity.Name, Operation = "新增活动:" + model.Title, ObjectType = "ACFV2", ObjectID = id.ToString() });
                                Business.CommonServices.CallCRMService.NewAddActivity(model.Title, model.StartDT.Value, model.EndDate.Value, "https://wx.tuhu.cn/staticpage/activity/list.html?id=" + id, null, id.ToString(), DataAccess.Entity.CommonServices.CRMSourceType.ActivityV2, User.Identity.Name);
                            }
                            catch ( Exception em)
                            {
                            }
                            finally
                            {

                            }
                        }
                    }
                    return Json(result);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(model.Content))
                {
                    // string activityMenu = JsonConvert.SerializeObject( ActivityMenuBll.GetList(model.id.ToString()));
                    //  string activityHome = JsonConvert.SerializeObject(ActivityHomeBll.GetList(model.id.ToString()));
                    //   model.ActivityHome = activityHome;
                    //   model.ActivityMenu = activityMenu;
                    var temp = ActivityManager.GetActivityBuildById(model.id);
                    model.UpdateUser = User.Identity.Name;
                    bool result = ActivityManager.UpdateActivityBuild(model, model.id);
                   
                    if (result)
                    {
                        try
                        {
                            using (var client = new Tuhu.Service.SEO.CouchbaseRemovalClient())
                            {
                                var resultClient = client.GetOrSetWXActivityPush(model.id, true);
                                resultClient.ThrowIfException(true);

                            }
                            ActivityManager.ReloadActivity(HttpContext.Request.Headers["Host"].Contains(".tuhu.cn") ? "wx.tuhu.cn" : "wx.tuhu.work", model.id);
                            LoggerManager.InsertOplog(new ConfigHistory() { BeforeValue = temp.Content, AfterValue = model.Content, Author = User.Identity.Name, Operation = "修改活动:" + model.Title, ObjectType = "ACFV2", ObjectID =model.id.ToString() });
                            Business.CommonServices.CallCRMService.NewUpdateActivity(model.Title, model.StartDT.Value, model.EndDate.Value, "https://wx.tuhu.cn/staticpage/activity/list.html?id=" + model.id, null, model.id.ToString(), DataAccess.Entity.CommonServices.CRMSourceType.ActivityV2, User.Identity.Name);
                        }
                        catch (Exception em)
                        {
                        }
                        finally
                        {

                        }
                    }

                    return Json(result);
                }
            }
            return Json(false);
        }

        [HttpPost]
        public ActionResult GetProductImage(string pid)
        {
            Dictionary<string,string> url = ActivityManager.GetProductImageUrl(pid);
            if (url == null)
                return Json("{\"BImage\":\"\",\"SImage\":\"\",\"DisplayName\":\"\"}");

            StringBuilder json = new StringBuilder("{");
            string bImage = Tuhu.WebSite.Component.SystemFramework.ImageHelper.GetProductImage(url["image"] ,300,300);
            json.Append("\"BImage\":\""+bImage+"\",");
            string sImage = Tuhu.WebSite.Component.SystemFramework.ImageHelper.GetProductImage(url["image"], 20, 20);
            json.Append("\"SImage\":\""+sImage+"\",");
            json.Append("\"CP_ShuXing5\":\"" + url["CP_ShuXing5"] + "\",");
            json.Append("\"DisplayName\":\""+url["name"]+"\",");
            json.Append("\"ActivityPrice\":" + (string.IsNullOrEmpty(url["ActivityPrice"]) == true?"0":url["ActivityPrice"]) + ",");
            if (pid.ToLower().Contains("tr"))
            {
                if (!string.IsNullOrWhiteSpace(url["CP_Tire_Width"]) && !string.IsNullOrWhiteSpace(url["CP_Tire_AspectRatio"]) && !string.IsNullOrWhiteSpace(url["CP_Tire_Rim"]))
                    json.Append("\"TireSize\":\"" + url["CP_Tire_Width"] + "/" + url["CP_Tire_AspectRatio"] + "R" + url["CP_Tire_Rim"] + "\"");
                else
                    json.Append("\"TireSize\":null");
            }
            else
            {
                json.Append("\"TireSize\":null");
            }
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

        [ValidateInput(false)]
        public ActionResult Edit(string rows="")
        {
            try
            {
                // string rows = "";
                ActivityBuildDetail detail = new ActivityBuildDetail();
                if (!string.IsNullOrEmpty(rows))
                {
                        detail = JsonConvert.DeserializeObject<ActivityBuildDetail>(rows);
                        return View(detail);
                }
                if (rows != "")
                {
                    JObject json = JObject.Parse(rows);
                    if (json["BigImg"].ToString() == "6")
                    {
                        detail = JsonConvert.DeserializeObject<ActivityBuildDetail>(rows);
                        return View(detail);
                    }
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


        public ActionResult BigActivityHome(string id)
        {
            // WX_Activity_Push
            if (!string.IsNullOrEmpty(id))
            {
                // var result = DistributedCache.Get<ActivityBuild>("WX_Activity_Push" + id);
               
                var result  = ActivityManager.GetActivityBuildById(int.Parse(id));
                if (result != null)
                { 
                    if (!string.IsNullOrEmpty(result.BigActivityHome))
                    {
                        BigActivityHome model = JsonConvert.DeserializeObject<BigActivityHome>(result.BigActivityHome);
                        return View(model);
                    }
                }
            }
            return View(new BigActivityHome());
        }


        public ActionResult BigFHomeEdit(string rows = "")
        {
            BigFHomeDeatil model = JsonConvert.DeserializeObject<BigFHomeDeatil>(rows);
            return View(model);
        }

        public ActionResult Menu(string rows,string group="")
        {
            if (!string.IsNullOrEmpty(rows))
            {
                if (group != "")
                {
                    ViewBag.Group = group;
                }
                return View( JsonConvert.DeserializeObject<List<ActivityMenu>>(rows));
            }
            else
            {
                return   View();
            }
        }


        /// <summary>
        /// 列表活动会场
        /// </summary>
        /// <returns></returns>
        public ActionResult GeneralHome(string rows)
        {
            if (!string.IsNullOrEmpty(rows))
            {
                return View(JsonConvert.DeserializeObject<IEnumerable<ActivityHome>>(rows));
            }
            else
            {
                return View();
            }
           
        }

#endregion


        /// <summary>
        /// 优惠券校验 GUID
        /// </summary>
        /// <param name="coupon"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CouponVlidate(string couponRulePKID)
        {
            return JavaScript(ActivityManager.CouponVlidate(couponRulePKID));
        }

        [HttpPost]
        public ActionResult CouponVlidate1(string couponRulePKID)
        {
            return JavaScript(ActivityManager.CouponVlidate1(couponRulePKID));
        }

        /// <summary>
        /// 校验活动下的PID是否存在
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public async Task<ActionResult> ActivityValidate(Guid activity,string pid)
        {
            JObject json = new JObject();

            try
            {
                using (var client = new Tuhu.Service.Activity.FlashSaleClient())
                {
                    var result = await client.GetFlashSaleListAsync(new Guid[] { activity });
                    result.ThrowIfException(true);
                    if (result.Success)
                    {
                        json.Add("status", result.Result.FirstOrDefault().Products.FirstOrDefault(o => o.PID == pid) != null);
                    }
                    else
                        json.Add("status", false);
                }
            }
            catch (Exception em)
            {
                json.Add("status",false);
                json.Add("error", em.Message);
            }
            return Json(json.ToString());
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
        public JsonResult DeleteActivityConfig(string type,string id)
        {
            string userName = HttpContext.User.Identity.Name;

            return Json(ActivityManager.DeleteActivityConfig(type, id, userName));
        }
#endregion



#endregion

        public ActionResult CreateActivity(string title)
        {

            return Content(ActivityManager.CreateActivity(title).ToString());
        }

        public ActionResult GetLogger(string id)
        {
           ConfigHistory model =  LoggerManager.GetConfigHistory(id);
            if (model?.ObjectType.Trim().ToLower() == "ACFV2".ToLower())
            {
                return View(model);
            }
            else
            {
                return HttpNotFound();
            }
           // return Json(model,JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetValidateLuckyWheel(string id)
        {
            var result = LuckyWheelManager.GetTableList("");
            if (result?.Where(_=>_.ID.ToLower() == id.ToLower().Trim()).Count() >0)
            {
                return Json("{\"status\":1}");
            }
            else
            {
                return Json("{\"status\":0}");
            }
        }

        public ActionResult GetTireSizeConfig(string config)
        {
            if (string.IsNullOrWhiteSpace(config))
                return View(new TireSizeConfig() { });
            else
                return View(JsonConvert.DeserializeObject<TireSizeConfig>(config));
        }


        public ActionResult ActivityRefresh(int id)
        {
            try
            {
                using (var client = new Tuhu.Service.SEO.CouchbaseRemovalClient())
                {
                    var resultClient = client.GetOrSetWXActivityPush(id, true);
                    resultClient.ThrowIfException(true);
                    return Json(new {status = 1, msg = "成功"});
                }
            }
            catch (Exception e)
            {
                return Json(new { status = 0, msg = e });
            }

        }

    }
}
