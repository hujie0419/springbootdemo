using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class FlashSalesTwoController : Controller
    {

        private readonly IFlashSalesTwoManager manager = new FlashSalesTwoManager();
        //  private readonly IAutoSuppliesManager ASmanager = new AutoSuppliesManager();
        //   ThBiz.Business.ProductInfomationManagement.IProductInfomationManager _PImanager = new ThBiz.Business.ProductInfomationManagement.ProductInfomationManager();

        #region 限时抢购活动管理
        public ActionResult FlashSalesTwo()
        {
            List<FlashSalesTwo> AllFlashSales = manager.GetAllFlashSales();
            var AreaArray = from s in AllFlashSales orderby s.Area select s.Area;
            ViewBag.Area = AreaArray.Distinct();
            ViewBag.AllFlashSales = AllFlashSales;
            return View();
        }

        public ActionResult FlashSalesV1(string ActivityID, string ActivityName, string StartAct, string EndAct, string StartUpdate, string EndUpdate, int pageIndex = 1)
        {
            string sqlStr = "";
            if (!string.IsNullOrWhiteSpace(ActivityID))
            {
                sqlStr += " AND ActivityID = '" + ActivityID + "'";
            }
            if (!string.IsNullOrWhiteSpace(ActivityName))
            {
                sqlStr += " AND ActivityName = N'" + ActivityName + "'";
            }
            if (!string.IsNullOrWhiteSpace(StartAct) && !string.IsNullOrWhiteSpace(EndAct))
            {
                sqlStr += " AND StartDateTime BETWEEN  '" + StartAct + "' AND '" + EndAct + "'";
            }
            if (!string.IsNullOrWhiteSpace(StartAct) && string.IsNullOrWhiteSpace(EndAct))
            {
                sqlStr += " AND StartDateTime >=  '" + StartAct + "'";

            }
            if (!string.IsNullOrWhiteSpace(EndAct) && string.IsNullOrWhiteSpace(StartAct))
            {
                sqlStr += " AND StartDateTime <=  '" + EndAct + "'";
            }

            if (!string.IsNullOrWhiteSpace(StartUpdate) && !string.IsNullOrWhiteSpace(EndUpdate))
            {
                sqlStr += " AND UpdateDateTime BETWEEN  '" + StartAct + "' AND '" + EndAct + "'";
            }
            if (!string.IsNullOrWhiteSpace(StartUpdate) && string.IsNullOrWhiteSpace(EndUpdate))
            {
                sqlStr += " AND UpdateDateTime >=  '" + StartUpdate + "'";
            }
            if (!string.IsNullOrWhiteSpace(EndUpdate) && string.IsNullOrWhiteSpace(StartUpdate))
            {
                sqlStr += " AND UpdateDateTime <=  '" + EndUpdate + "'";

            }
            int recordCount = 0;

            var model = manager.GetAllFlashSalesV1(sqlStr, 20, pageIndex, out recordCount);

            var list = new OutData<List<FlashSalesTwo>, int>(model, recordCount);
            var pager = new PagerModel(pageIndex, 20)
            {
                TotalItem = recordCount
            };
            return this.View(new ListModel<FlashSalesTwo>(list.ReturnValue, pager));
        }
        #endregion

        #region 限时抢购活动产品
        public ActionResult FlashSalesProduct(string activityid)
        {
            if (!string.IsNullOrEmpty(activityid))
            {
                // string _FlashSalesID = activityid.Value;
                FlashSalesTwo _FlashSales = manager.GetFlashSalesByID(activityid);
                if (_FlashSales != null)
                {
                    List<FlashSalesProductTwo> _ProList = manager.GetProListByFlashSalesID(activityid);
                    ViewBag.FlashSales = _FlashSales;
                    ViewBag.ProList = _ProList;
                    return View();
                }
                else
                {
                    return Content("<script>alert('该活动不存在，请确认该活动是否被删除');location='/FlashSales/FlashSales';</script>");
                }
            }
            else
            {
                return Content("<script>alert('该活动不存在，请从正常来源进入');location='/FlashSales/FlashSales'</script>");
            }
        }

        public string GetFristProduct(string activityid)
        {
            FlashSalesProductTwo model = manager.GetProListByFlashSalesID(activityid).FirstOrDefault();
            return model.PID;
        }
        #endregion

        #region 修改限时抢购
        public ActionResult AddFlashSalesTwo(string activityid)
        {
            ViewBag.Title = "修改抢购活动";
            return View(manager.GetFlashSalesByID(activityid));
        }

        public ActionResult AddFlashSalesV1(string activityid)
        {
            ViewBag.Title = "修改抢购活动";
            return View(manager.GetFlashSalesByID(activityid));
        }
        #endregion

        #region 修改的form提交

        [HttpPost]
        public ActionResult AddFlashSalesV1(FlashSalesTwo flashSales)
        {
            string kstr = "";
            bool IsSuccess = true;
            if (string.IsNullOrEmpty(flashSales.ActivityName))
            {
                kstr = "模块名称不能为空";
                IsSuccess = false;
            }
            if (IsSuccess)
            {
                int cart = flashSales.ShoppingCart ? 1 : 0;
                string pid = GetFristProduct(flashSales.ActivityID);
                manager.UpdateFlashSalesTwoV1(flashSales);
                return RedirectToAction("FlashSalesV1");
            }
            else
            {
                string js = "<script>alert(\"" + kstr + "\");location='/FlashSalesTwo/FlashSalesV1';</script>";
                return Content(js);
            }
        }

        [HttpPost]
        public ActionResult AddFlashSalesTwo(FlashSalesTwo flashSales)
        {
            string kstr = "";
            bool IsSuccess = true;
            if (string.IsNullOrEmpty(flashSales.ActivityName))
            {
                kstr = "模块名称不能为空";
                IsSuccess = false;
            }
            if (IsSuccess)
            {
                manager.UpdateFlashSalesTwo(flashSales);
                return RedirectToAction("FlashSalesTwo");
            }
            else
            {
                string js = "<script>alert(\"" + kstr + "\");location='FlashSales';</script>";
                return Content(js);
            }
        }
        #endregion

        #region 限时抢购活动产品合集
        public ActionResult FlashSalesProductTwo(string ActivityID)
        {
            if (!string.IsNullOrEmpty(ActivityID))
            {
                FlashSalesTwo _FlashSales = manager.GetFlashSalesByID(ActivityID);
                if (_FlashSales != null)
                {
                    List<FlashSalesProductTwo> _ProList = manager.GetProListByFlashSalesID(ActivityID);
                    ViewBag.FlashSales = _FlashSales;
                    ViewBag.ProList = _ProList;
                    return View();
                }
                else
                {
                    return Content("<script>alert('该活动不存在，请确认该活动是否被删除');location='/FlashSalesTwo/FlashSalesTwo';</script>");
                }
            }
            else
            {
                return Content("<script>alert('该活动不存在，请从正常来源进入');location='/FlashSalesTwo/FlashSalesTwo'</script>");
            }
        }

        public ActionResult FlashSalesProductV1(string ActivityID)
        {
            if (!string.IsNullOrEmpty(ActivityID))
            {
                FlashSalesTwo _FlashSales = manager.GetFlashSalesByID(ActivityID);
                if (_FlashSales != null)
                {
                    List<FlashSalesProductTwo> _ProList = manager.GetProListByFlashSalesID(ActivityID);
                    ViewBag.FlashSales = _FlashSales;
                    ViewBag.ProList = _ProList;
                    return View();
                }
                else
                {
                    return Content("<script>alert('该活动不存在，请确认该活动是否被删除');location='/FlashSalesTwo/FlashSalesTwo';</script>");
                }
            }
            else
            {
                return Content("<script>alert('该活动不存在，请从正常来源进入');location='/FlashSalesTwo/FlashSalesTwo'</script>");
            }
        }
        #endregion

        [HttpPost]
        #region 限时抢购的产品的修改
        public ActionResult SaveProductTwo(string dataList)
        {
            Dictionary<string, string> hashMap = new Dictionary<string, string>();

            List<FlashSalesProductTwo> flashsalesproducttwo = JsonConvert.DeserializeObject<List<FlashSalesProductTwo>>(dataList);
            try
            {
                int result = 0;
                foreach (var item in flashsalesproducttwo)
                {
                    result = manager.SaveProductTwo(item);

                }
                if (result > 0)
                {
                    hashMap.Add("data", "1");
                }
                else
                {
                    hashMap.Add("data", " 0");
                }

            }
            catch (Exception ex)
            {
                hashMap.Add("data", ex.Message);
            }
            return Json(hashMap);
        }
        #endregion

        //根据楼层去查，限时抢购的这个区对应的唯一一个开启的活动，1-A,2-B,3-C,,,,,,7-G
        public ActionResult GetActivityByModelfloor(int modelfloor)
        {
            FlashSalesTwo flashsalestwo = manager.GetActivityByModelfloor(modelfloor);
            string area = flashsalestwo.Area;
            string activityid = flashsalestwo.ActivityID;
            string returnJson = "{\"activityid\":\"" + activityid + "\",\"area\":\"" + area + "\"}";
            //string stringflashsalestwo = GetJSON<FlashSalesTwo>(flashsalestwo);
            return Json(returnJson, JsonRequestBehavior.AllowGet);
        }
    }
}
