using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.UserPermission;
using Tuhu.Provisioning.DataAccess.Entity;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Tuhu.Service.Member.Request;
using Tuhu.Service.UserAccount.Models;
using Tuhu.Service.UserAccount.Enums;
using NPOI.SS.UserModel;
using System.Web;
using System.IO;
using System.Data;

namespace Tuhu.Provisioning.Controllers
{
    public class UserPermissionController : Controller
    {
        //
        // GET: /UserPermission/

        #region 会员特权

        public ActionResult Index(string page, string pageSize = "5")
        {
            int result = 1;
            if (!int.TryParse(page, out result))
                result = 1;

            var data = UserPermissionManager.SelectUserPermissionByPage(result, Convert.ToInt32(pageSize));
            int rows = UserPermissionManager.GetRowCount();
            ViewBag.PageCount = Math.Ceiling((double)rows / Convert.ToInt32(pageSize) * 1.0);
            ViewBag.Page = result;
            if (data.Count() <= 0 || !data.Any())
                return View();

            return View(data);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ViewBag.Title = "添加用户等级配置";
                return View(new UserPermissionModel());
            }
            else
            {
                ViewBag.Title = "编辑用户等级配置";
                return View(UserPermissionManager.GetUserPermission(id));
            }

        }

        [HttpPost]
        public ActionResult Edit(string id, string data)
        {
            var model = JsonConvert.DeserializeObject<UserPermissionModel>(data);
            if (model != null)
            {
                if (string.IsNullOrEmpty(id))
                {
                    if (UserPermissionManager.Add(model) > 0)
                        return Content("1");
                    else
                        return Content("0");
                }
                else
                {
                    model.Id = Convert.ToInt32(id);
                    if (UserPermissionManager.Update(model) > 0)
                        return Content("1");
                    else
                        return Content("0");
                }
            }
            return Content("0");
        }



        [HttpPost]
        public string Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return "0";

                if (UserPermissionManager.Delete(id) > 0)
                    return "1";
                else
                    return "0";
            }
            catch
            {
                return "0";
            }
        }
        #endregion


        #region 特价商品
        public ActionResult SaleProductList(string activityID = "8312FE9D-6DCE-4CD5-87D5-99CE064336A3")
        {
            if (activityID != "")
            {
                IEnumerable<UserPermissionActivityProduct> list = UserPermissionManager.GetActivityProductList(activityID);
                return View(list);
            }
            else
            {
                return View();
            }

        }

        public ActionResult SaleProductEdit(string id = "")
        {
            if (id == "")
            {
                return View(new UserPermissionActivityProduct());
            }
            else
            {
                UserPermissionActivityProduct model = UserPermissionManager.GetActivityProduct(id);
                if (model == null)
                    return View(new UserPermissionActivityProduct());
                return View(model);
            }

        }

        public ActionResult AddActivityProduct(UserPermissionActivityProduct model)
        {
            if (model.PID == "")
            {
                return JavaScript("{\"status\":0}");
            }
            else
            {
                if (UserPermissionManager.AddByActivityProduct(model))
                {
                    return JavaScript("{\"status\":1}");
                }
                else
                {
                    return JavaScript("{\"status\":0}");
                }
            }
        }

        public ActionResult DeleteActivityProduct(string activityID, string pkid)
        {
            if (activityID == "" || pkid == "")
                return JavaScript("{\"status\":0}");
            else
            {
                if (UserPermissionManager.DeleteByActivityProduct(activityID, pkid))
                    return JavaScript("{\"status\":1}");
                else
                    return JavaScript("{\"status\":0}");
            }
        }


        #endregion


        #region 运费折扣
        public ActionResult Freight()
        {
            List<tbl_UserTransportation> list = UserPermissionManager.GetUseTransMoney();

            return View(list);
        }

        public ActionResult SaveTrans(string list)
        {
            if (string.IsNullOrEmpty(list))
            {
                return JavaScript("{\"status\":0}");
            }
            List<tbl_UserTransportation> models = JsonConvert.DeserializeObject<List<tbl_UserTransportation>>(list);
            if (UserPermissionManager.SaveTransMoney(models))
            {
                return JavaScript("{\"status\":1}");
            }
            else
            {
                return JavaScript("{\"status\":0}");
            }
        }


        #endregion

        #region 升级任务
        public ActionResult TaskList(string appType = "1")
        {

            return View(UserPermissionManager.GetTaskList(appType));
        }

        public ActionResult TaskEdit(string appType = "1", string id = "0")
        {
            if (id == "0")
            {
                return View(new tbl_UserTask() { APPType = Convert.ToInt32(appType) });
            }
            else
            {
                tbl_UserTask model = UserPermissionManager.GetTask(id);
                return View(model);
            }
        }


        public ActionResult SaveTask(tbl_UserTask model)
        {
            if (UserPermissionManager.EditTask(model))
            {
                return JavaScript("{\"status\":1}");
            }
            else
                return JavaScript("{\"status\":0}");
        }

        public ActionResult DeleteTask(string id)
        {
            if (UserPermissionManager.DeleteTask(id))
            {
                return JavaScript("{\"status\":1}");
            }
            else
                return JavaScript("{\"status\":0}");
        }

        #endregion


        #region  会员优惠券
        public ActionResult PromotionList(string userRank = "LV1")
        {

            return View(UserPermissionManager.GetPromotionList(userRank));
        }

        public ActionResult PromotionEdit(string id = "0", string userRank = "LV1")
        {
            if (id == "0")
                return View(new tbl_UserPromotionCode() { UserRank = userRank });
            else
            {
                tbl_UserPromotionCode model = UserPermissionManager.GetPromotion(id);
                return View(model);
            }
        }


        public ActionResult SavePromotion(tbl_UserPromotionCode model)
        {
            if (UserPermissionManager.EditPromotion(model))
            {
                return JavaScript("{\"status\":1}");
            }
            else
                return JavaScript("{\"status\":0}");
        }

        public ActionResult DeletePromotion(string id)
        {
            if (UserPermissionManager.DeletePromotion(id))
            {
                return JavaScript("{\"status\":1}");
            }
            else
                return JavaScript("{\"status\":0}");
        }
        #endregion


        public ActionResult Grade()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ImportGrade()
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];

                    if (!file.FileName.Contains(".xlsx") || !file.FileName.Contains(".xls"))
                        return Json(new { Status = -1, Error = "请上传.xlsx文件或者.xls文件！" }, "text/html");

                    var excel = new Controls.ExcelHelper(file.InputStream, file.FileName);
                    var dt = excel.ExcelToDataTable("会员等级导入", true);
                    List<VipMobileRequest> list = new List<VipMobileRequest>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        list.Add(new VipMobileRequest()
                        {
                            Mobile = dr["Mobile"]?.ToString(),
                            Pkid = dr["Pkid"] != DBNull.Value ? Convert.ToInt32(dr["Pkid"]) : 0,
                        });
                    }

                    IEnumerable<VipMobileRequest> vipList = list as IEnumerable<VipMobileRequest>;

                    if (vipList.Any())
                    {
                        if (vipList.Count() >= 500)
                        {
                            return Json(new { Status = -6, Error = "单次不能超出500条数据操作！" }, "text/html");
                        }
                        using (var client = new Tuhu.Service.Member.UserClient())
                        {
                            var result = client.BatchCreateVipAuthorization(vipList, "Tuhu", "Import");
                            if (result.Success)
                            {
                                return Json(new { Status = 1, Result = result.Result[1] }, "text/html");
                            }
                            else
                            {
                                return Json(new { Status = -5, Error = "服务器错误！" }, "text/html");
                            }
                        }
                    }
                    else
                    {
                        return Json(new { Status = -2, Error = "导入数据为空！" }, "text/html");
                    }
                }
                return Json(new { Status = -3, Error = "未知错误！" }, "text/html");
            }
            catch (Exception em)
            {
                return Json(new { Status = -4, Error = em }, "text/html");
                throw em;
            }
        }
    }
}
