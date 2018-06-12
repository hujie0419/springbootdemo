using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.Tousu;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Controllers
{
    public class TousuConfigController : Controller
    {
        // GET: TousuConfig
        public ActionResult Index()
        {
            IEnumerable<TousuModel> tousuModelList = TousuBusiness.GetTopLevelTousuModelList();
            return View(tousuModelList);
        }
        public ActionResult LastLevelTousuNameEdit()
        {
            return View();
        }

        public ActionResult LastLevelTousuEdit()
        {
            //  IEnumerable<string> orderTypeList = TousuBusiness.OrderTypeList();
            IEnumerable<TousuModel> tousuModelList = TousuBusiness.GetTopLevelTousuModelList();
            return View(tousuModelList);
        }

        public PartialViewResult LastLevelTousuItemNameList(string dicValue, int showLevel = 2,int pageIndex = 1, int pageSize = 10)
        {
            var pager = new PagerModel(pageIndex, pageSize);
            var data = TousuBusiness.SelectList(dicValue, showLevel, pager);
            ViewBag.Select = new TousuModel() { DicValue= dicValue };
            return PartialView("LastLevelTousuNameList", new ListModel<TousuModel>() { Pager = pager, Source = data });
        }

        public PartialViewResult LastLevelTousuItemList(TousuModel model,int showLevel=2,int pageIndex = 1, int pageSize = 10)
        {
            var pager = new PagerModel(pageIndex, pageSize);
            var data = TousuBusiness.SelectList(model.DicValue, showLevel, pager,model.OrderType);
            ViewBag.Select = model;
            return PartialView("LastLevelTousuList", new ListModel<TousuModel>() { Pager = pager, Source = data });
        }

        public JsonResult GetTousuModelListByDicValue(string dicValue)
        {
            
            IEnumerable<TousuModel> tousuModelList = TousuBusiness.GetTouModelListByDicValue(dicValue);
            return Json(tousuModelList);
        }

        public ActionResult TopLevelTousuEdit()
        {
            IEnumerable<TousuModel> tousuModelList = TousuBusiness.GetTopLevelTousuModelList();
            return View(tousuModelList);
        }

        public ActionResult GetTopLevelTousuList()
        {
            var toplevelTousuList = TousuBusiness.GetTopLevelTousuModelList();
            return Json(toplevelTousuList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateTopLevelTouSu(TousuModel model,string oldAppDisplayName)
        {
            var identity= ThreadIdentity.Operator;
            var histroylog = new ConfigHistory()
            {
                ObjectType = "TouSuTopLevelAppNameUpdate",
                Author = identity.Name,
                BeforeValue = oldAppDisplayName,
                AfterValue = model.AppDisplayName,
                IPAddress = identity.IPAddress,
                ChangeDatetime = DateTime.Now,
                Operation = "更新投诉大类在APP端的显示名称"
            };
            LoggerManager.InsertOplog(histroylog);
            var result=TousuBusiness.SetAppDisplayName(model, identity.Name);
            if (result)
            {
                var tousuitem = TousuBusiness.tousuList.FirstOrDefault(s => string.Equals(s.DicValue, model.DicValue, StringComparison.OrdinalIgnoreCase));
                if (tousuitem != null)
                {
                    tousuitem.AppDisplayName = model.AppDisplayName;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTopTousuNameList(string orderType)
        {
            var topTousuNameList=DalTousuConfig.GetTopTousuNameList(orderType);
            return Json(topTousuNameList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrderTypeList()
        {
            var orderTypeList = TousuBusiness.GetOrderTypeList();
            return Json(orderTypeList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateOrInsertOrderTypeTousuConfig(OrderTypeTousuConfig model)
        {
            var result = TousuBusiness.UpdateOrInsertOrderTypeTousuConfig(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}