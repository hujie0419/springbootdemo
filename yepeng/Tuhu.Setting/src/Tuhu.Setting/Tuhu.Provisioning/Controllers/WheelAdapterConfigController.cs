using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.Business.WheelAdapterConfig;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Config;

namespace Tuhu.Provisioning.Controllers
{
    public class WheelAdapterConfigController : Controller
    {
        #region
        private readonly Lazy<WheelAdapterConfigManager> lazy = new Lazy<WheelAdapterConfigManager>();
        private WheelAdapterConfigManager WheelAdapterConfigManager
        {
            get { return lazy.Value; }
        }
        #endregion
        public ActionResult Index()
        {
            WheelAdapterConfigQuery query = new WheelAdapterConfigQuery();
            query.PageIndex = 0;
            query.PageDataQuantity = 10;
            query.TotalCount = 0;
            ViewBag.query = query;
            return View();
        }
        public PartialViewResult IndexList(WheelAdapterConfigQuery query)
        {
            List<VehicleTypeInfo> vehicletypeinfolist = new List<VehicleTypeInfo>();
            if (query.QueryWay == 0)
            {
                vehicletypeinfolist = WheelAdapterConfigManager.QueryVehicleTypeInfo(query);
            }
            else
            {
                vehicletypeinfolist = WheelAdapterConfigManager.QueryVehicleTypeInfoByTID(query);
            }
            var pager = new PagerModel(query.PageIndex, query.PageDataQuantity);
            ViewBag.query = query;
            pager.TotalItem = query.TotalCount;
            return PartialView(new ListModel<VehicleTypeInfo>() { Pager = pager, Source = vehicletypeinfolist });
        }
        public JsonResult GetBrands()
        {
            List<Str> brandlist = WheelAdapterConfigManager.SelectBrands();
            return Json(brandlist);
        }
        public JsonResult GetVehiclesAndId(string brand)
        {
            List<Str> vehicleandidlist = WheelAdapterConfigManager.SelectVehiclesAndId(brand);
            return Json(vehicleandidlist);
        }
        public JsonResult GetPaiLiang(string vehicleid)
        {
            List<Str> pailianglist = WheelAdapterConfigManager.SelectPaiLiang(vehicleid);
            return Json(pailianglist);
        }
        public JsonResult GetYear(string vehicleid, string pailiang)
        {
            List<Str> yearlist = WheelAdapterConfigManager.SelectYear(vehicleid, pailiang);
            for(int i = 0; i < yearlist.Count; i++)
            {
                if (yearlist[i].str1 == null)
                {
                    yearlist[i].str1 = DateTime.Now.Year.ToString();
                }
            }
            return Json(yearlist);
        }
        public JsonResult GetNianAndSalesName(string vehicleid, string pailiang, string year)
        {
            List<Str> nianandsalesnamelist = WheelAdapterConfigManager.SelectNianAndSalesName(vehicleid, pailiang, year);
            return Json(nianandsalesnamelist);
        }
        public JsonResult GetWheelAdapterConfigWithTid(string tid)
        {
            List<WheelAdapterConfigWithTid> waclist = WheelAdapterConfigManager.SelectWheelAdapterConfigWithTid(tid);
            return Json(waclist);
        }
        public ActionResult WheelAdapterEditModule(string tid)
        {
            List<WheelAdapterConfigWithTid> waclist = WheelAdapterConfigManager.SelectWheelAdapterConfigWithTid(tid);
            WheelAdapterConfigWithTid wac = new WheelAdapterConfigWithTid();
            if (waclist.Count > 0)
            {
                wac = waclist[0];
            }
            else
            {
                wac.PKId = 0;
                wac.TID = tid;
            }
            return View(wac);
        }

        public JsonResult BatchSaveWheelAdapterConfig(WheelAdapterConfigWithTid wac, IEnumerable<string> tids)
        {
            var props = typeof(WheelAdapterConfigWithTid).GetProperties();
            wac.CreateDateTime = DateTime.Now;
            wac.LastUpdateDateTime = DateTime.Now;
            wac.Creator = ThreadIdentity.Operator.Name;
            var result= WheelAdapterConfigManager.InsertWheelAdapterConfigWithTid(wac,tids);
            return Json(result);
        }

        public JsonResult SaveWheelAdapterConfig(WheelAdapterConfigWithTid wac)
        {
            bool flag = false;
            if (wac.PKId == 0)
            {
                wac.CreateDateTime = DateTime.Now;
                wac.LastUpdateDateTime = DateTime.Now;
                wac.Creator = ThreadIdentity.Operator.Name;
                flag = WheelAdapterConfigManager.InsertWheelAdapterConfigWithTid(wac);
                WheelAdapterConfigLog wacl = new WheelAdapterConfigLog()
                {
                    TID = wac.TID,
                    OperateType = 0,
                    CreateDateTime = wac.CreateDateTime,
                    LastUpdateDateTime = wac.LastUpdateDateTime,
                    Operator = wac.Creator,
                };
                bool flag1 = WheelAdapterConfigManager.InsertWheelAdapterConfigLog(wacl);
            }
            else
            {
                wac.LastUpdateDateTime = DateTime.Now;
                flag = WheelAdapterConfigManager.UpdateWheelAdapterConfigWithTid(wac);
                WheelAdapterConfigLog wacl = new WheelAdapterConfigLog()
                {
                    TID = wac.TID,
                    OperateType = 1,
                    CreateDateTime = DateTime.Now,
                    LastUpdateDateTime = DateTime.Now,
                    Operator = ThreadIdentity.Operator.Name,
                };
                bool flag1 = WheelAdapterConfigManager.InsertWheelAdapterConfigLog(wacl);
            }
            return Json(flag);
        }
        public ActionResult HistoryModule(string tid)
        {
            List<WheelAdapterConfigLog> wacllist = WheelAdapterConfigManager.SelectWheelAdapterConfigLog(tid);
            return View(wacllist);
        }
    }
}