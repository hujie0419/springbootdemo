using Microsoft.ApplicationBlocks.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.WebActivity;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Models;

namespace Tuhu.Provisioning.Controllers
{
    public class WebSiteActivityController : Controller
    {
        [PowerManage]
        public ActionResult Index()
        {
            var model1 = WebActivityManager.SelectAllWebActivity();
            IEnumerable<ActivityModel> model2 = null;
            IConnectionManager cm = new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
            using (var conn = cm.OpenConnection())
            {
                var data = SqlHelper.ExecuteDataTable(conn, CommandType.Text, "SELECT* FROM Activity..tbl_FlashSale AS FS WHERE FS.WebBanner IS NOT NULL ");
                cm.CloseConnection();
                if (data.Rows.Count > 0)
                    model2 = data.Rows.Cast<DataRow>().Select(row => new ActivityModel(row));
            }
            return View(Tuple.Create(model1, model2));
        }

        public ActionResult FsActive(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            ViewBag.ActivityID = id;
            IConnectionManager cm = new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
            using (var conn = cm.OpenConnection())
            {
                var data = SqlHelper.ExecuteDataRow(conn, CommandType.Text, "SELECT* FROM Activity..tbl_FlashSale AS FS Where ActivityID=@ActivityID", new SqlParameter("@ActivityID", id));
                cm.CloseConnection();
                if (data != null)
                    return View(new ActivityModel(data));
                else
                    return null;
            }
        }
        public ActionResult WebActive(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                ViewBag.ActiveID = Convert.ToInt32(WebActivityManager.GetLastActiveID()) + 1;
                ViewBag.action = "新增";
                return View(new WebActive());
            }
            ViewBag.action = "更新";
            ViewBag.ActiveID = id;
            var data = WebActivityManager.WebActivityDetail(id);
            return View(data);
        }

        [HttpPost]
        public ActionResult WebActiveConfig(string webActivity, string commodifyfloor, string otherpart, string Action)
        {
            WebActive webact = JsonConvert.DeserializeObject<WebActive>(webActivity);
            webact.CommodifyFloor = JsonConvert.DeserializeObject<List<CommodityFloor>>(commodifyfloor);
            webact.OtherPart = JsonConvert.DeserializeObject<List<OtherPart>>(otherpart);
            var result = WebActivityManager.WebActiveConfig(webact, Action);
            return Json(result);
        }
        [HttpPost]
        public ActionResult FlashSalesConfig(string ActivityModel)
        {
            ActivityModel activeModel = null;
            try
            {
                activeModel = JsonConvert.DeserializeObject<ActivityModel>(ActivityModel); ;
            }
            catch (InvalidCastException ex)
            {
                return Json(ex.Message);
            }

            IConnectionManager cm = new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
            using (var conn = cm.OpenConnection())
            {
                if (SqlHelper.ExecuteDataTable(conn, CommandType.Text, "SELECT* FROM Activity..tbl_FlashSale AS FS WHERE ActivityID=@ActivityID", new SqlParameter("@ActivityID", activeModel.ActivityID)).Rows.Count > 0)
                {
                    SqlParameter ActivityID = new SqlParameter("@ActivityID", activeModel.ActivityID);
                    SqlParameter WebBackground = new SqlParameter("@WebBackground", activeModel.WebBackground);
                    SqlParameter WebBanner = new SqlParameter("@WebBanner", activeModel.WebBanner);
                    SqlParameter WebCornerMark = new SqlParameter("@WebCornerMark", activeModel.WebCornerMark);
                    SqlParameter WebOtherPart = new SqlParameter("@WebOtherPart", activeModel.WebOtherPart);
                    var sql = "UPDATE Activity..tbl_FlashSale SET WebBackground=@WebBackground, WebBanner=@WebBanner,WebCornerMark=@WebCornerMark,WebOtherPart=@WebOtherPart WHERE ActivityID=@ActivityID";
                    var result = SqlHelper.ExecuteNonQuery(conn, CommandType.Text, sql, ActivityID, WebBackground, WebBanner, WebCornerMark, WebOtherPart);
                    cm.CloseConnection();
                    if (result > 0)
                        return Json(result);
                    else
                        return Json("活动配置失败！");
                }
                else
                {
                    cm.CloseConnection();
                    return Json("该活动产品还未配置！");
                }
            }
        }
        public ActionResult SelectFlashSalesProducts(Guid? ActivityID)
        {
            if (ActivityID != null)
            {
                IConnectionManager cm = new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
                using (var conn = cm.OpenConnection())
                {
                    SqlParameter ActID = new SqlParameter("@ActivityID", ActivityID);
                    var sql = "SELECT FSP.*,FS.ActivityName,FS.StartDateTime,FS.EndDateTime,VP.price AS OriginalPrice,VP.DisplayName FROM Activity..tbl_FlashSaleProducts AS FSP LEFT JOIN Activity..tbl_FlashSale AS FS ON FS.ActivityID=FSP.ActivityID LEFT JOIN Gungnir..vw_Products AS VP ON Fsp.PID=Vp.PID  WHERE Fsp.ActivityID=@ActivityID";
                    var result = SqlHelper.ExecuteDataTable(conn, CommandType.Text, sql, ActID);
                    cm.CloseConnection();
                    if (result == null || result.Rows.Count <= 0)
                        return Json(null, JsonRequestBehavior.AllowGet);
                    return Json(result.Rows.Cast<DataRow>()
                        .Select(row => new ActivityProduct(row))
                        .Select(a => new
                        {
                            a.PID,
                            a.ActivityName,
                            a.DisplayName,
                            a.OriginalPrice,
                            a.Price,
                            a.MaxQuantity,
                            a.TotalQuantity,
                            StartDateTime = a.StartDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            EndDateTime = a.EndDateTime.ToString("yyyy-MM-dd HH:mm:ss")
                        }), JsonRequestBehavior.AllowGet);
                }
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteActivity(string actID, int Type)
        {
            return Json(WebActivityManager.DeleteWebActivity(actID, Type), JsonRequestBehavior.AllowGet);
        }
        public ActionResult HistoryActions(int activeID, string type)
        {
            var content = "";
            var result = WebActivityManager.HistoryActions(activeID, type).OrderBy(h => h.ChangeDatetime);
            if (result != null && result.Count() > 0)
            {
                if (type == "AddGetRule")
                {
                    System.Text.StringBuilder sbr = new System.Text.StringBuilder();
                    sbr.AppendFormat(@"<table><tr><td>序号</td><td>操作</td><td>操作人账号</td><td>操作时间</td></tr>");
                    int k = result.Count();
                    foreach (var h in result.Reverse())
                    {
                        sbr.AppendFormat($"<tr><td>{k}</td><td>{GetAddGetRuleOper(h.Operation)}</td><td>{h.Author}</td><td>{h.ChangeDatetime}</td></tr>");
                        k--;
                    }
                    sbr.AppendFormat("</table>");
                    content = sbr.ToString();
                }
                else
                {
                    foreach (var h in result)
                    {
                        content += "<span style='display:block;margin-bottom:5px;'>" + (String.IsNullOrWhiteSpace(h.Author) ? (String.IsNullOrWhiteSpace(h.HostName) ? (String.IsNullOrWhiteSpace(h.IpAddress) ? "无用户名" : h.IpAddress) : h.HostName) : h.Author) + h.ChangeDatetime + h.Operation + "</span>";
                    }
                }
            }
            else
                content = "暂无记录";
            return Content(content);
        }

        private string GetAddGetRuleOper(string operation)
        {
            return operation == "ADD" ? "创建" : "更新";
        }

        public ActionResult InsertHistoryAction(int ActiveID, string ObjectType, string AfterValue, string Operation)
        {
            return Json(WebActivityManager.InsertHistoryActions(ActiveID, ObjectType, AfterValue, Operation, User == null ? "" : User.Identity.Name, Request.UserHostName, ""), JsonRequestBehavior.AllowGet);
        }
    }
}
