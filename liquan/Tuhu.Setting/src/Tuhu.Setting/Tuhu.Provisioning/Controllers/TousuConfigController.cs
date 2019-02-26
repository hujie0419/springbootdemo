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
using Tuhu.Service.ConfigLog;
using Newtonsoft.Json;
using System.Data;

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

        public JsonResult UpdateTopLevelTouSu(TousuModel model, string oldAppDisplayName)
        {
            var identity = ThreadIdentity.Operator;
            //var histroylog = new ConfigHistory()
            //{
            //    ObjectType = "TouSuTopLevelAppNameUpdate",
            //    Author = identity.Name,
            //    BeforeValue = oldAppDisplayName,
            //    AfterValue = model.AppDisplayName,
            //    IPAddress = identity.IPAddress,
            //    ChangeDatetime = DateTime.Now,
            //    Operation = "更新投诉大类在APP端的显示名称"
            //};
            //LoggerManager.InsertOplog(histroylog);
            var result = TousuBusiness.SetAppDisplayName(model, identity.Name);
            if (result)
            {
                var tousuitem = TousuBusiness.tousuList.FirstOrDefault(s => string.Equals(s.DicValue, model.DicValue, StringComparison.OrdinalIgnoreCase));
                if (tousuitem != null)
                {
                    tousuitem.AppDisplayName = model.AppDisplayName;
                }
                var aftervalue = "修改TopLevelTousuName:" + model.DicValue + ",OldAppDisplayName:" + oldAppDisplayName + ",AppDisplayName:" + model.AppDisplayName;
                using (var log = new ConfigLogClient())
                {
                    var issuccess = log.InsertDefaultLogQueue("TousuConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = model.DicValue,
                        ObjectType = "TopLevelTousuNameUpdate",
                        BeforeValue = "",
                        AfterValue = "修改TopLevelTousuName:" + model.DicValue + ",OldAppDisplayName:" + oldAppDisplayName + ",AppDisplayName:" + model.AppDisplayName,
                        Operate = "Update",
                        Author = identity.Name
                    }));
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateLastLevelTouSu(TousuModel model,string oldAppDisplayName)
        {
            var identity= ThreadIdentity.Operator;
            //var histroylog = new ConfigHistory()
            //{
            //    ObjectType = "TouSuTopLevelAppNameUpdate",
            //    Author = identity.Name,
            //    BeforeValue = oldAppDisplayName,
            //    AfterValue = model.AppDisplayName,
            //    IPAddress = identity.IPAddress,
            //    ChangeDatetime = DateTime.Now,
            //    Operation = "更新投诉大类在APP端的显示名称"
            //};
            //LoggerManager.InsertOplog(histroylog);
            var result=TousuBusiness.SetAppDisplayName(model, identity.Name);
            if (result)
            {
                var tousuitem = TousuBusiness.tousuList.FirstOrDefault(s => string.Equals(s.DicValue, model.DicValue, StringComparison.OrdinalIgnoreCase));
                if (tousuitem != null)
                {
                    tousuitem.AppDisplayName = model.AppDisplayName;
                }
               
                    var aftervalue = "修改LastLevelTousuName:" + model.DicValue + ",OldAppDisplayName:" + oldAppDisplayName + ",AppDisplayName:" + model.AppDisplayName;
                    using (var log = new ConfigLogClient())
                    {
                       var issuccess= log.InsertDefaultLogQueue("TousuConfigLog", JsonConvert.SerializeObject(new
                        {
                            ObjectId = model.DicValue,
                            ObjectType = "LastLevelTousuNameUpdate",
                            BeforeValue = "",
                            AfterValue = "修改LastLevelTousuName:" + model.DicValue + ",OldAppDisplayName:" + oldAppDisplayName + ",AppDisplayName:" + model.AppDisplayName,
                            Operate = "Update",
                            Author = identity.Name
                       }));
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
            var identity = ThreadIdentity.Operator;
            var result = TousuBusiness.UpdateOrInsertOrderTypeTousuConfig(model);
            if (result)
            {
                var aftervalue = "修改LastLevelTousu:" + model.LastLevelTousu + ",IsChecked:" + model.IsChecked + ",IsNeedPhoto:" + model.IsNeedPhoto + ",CautionText:" + model.CautionText;
                using (var log = new ConfigLogClient())
                {
                    var issuccess = log.InsertDefaultLogQueue("TousuConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = model.LastLevelTousu,
                        ObjectType = "LastLevelTousuUpdate",
                        BeforeValue = "",
                        AfterValue = "修改LastLevelTousu:" + model.LastLevelTousu+",IsChecked:"+model.IsChecked+ ",IsNeedPhoto:" + model.IsNeedPhoto + ",CautionText:" + model.CautionText,
                        Operate = "Update",
                        Author = identity.Name
                    }));
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult HistoryActions(string objectId, string objectType)
        {
            var content = "";
            var logs = TousuBusiness.GetTousuLog(objectId, objectType);
            System.Text.StringBuilder sbr = new System.Text.StringBuilder();
            sbr.AppendFormat(@"<table><tr><td>操作时间</td><td>操作人</td><td>操作内容</td></tr>");
            int k = logs.Rows.Count;
            for (var i = 0; i < logs.Rows.Count; i++)
            {
                DataRow h = logs.Rows[i];
              //  string beforeJson = "";
                if ((i + 1) < logs.Rows.Count)
                {
                    DataRow b = logs.Rows[i + 1];
                 //   beforeJson = JsonConvert.DeserializeObject(b.GetValue<string>("AfterValue"));
                }
                sbr.Append(
                        $@"<tr><td>{h.GetValue<DateTime>("CreateTime")}</td><td>{h.GetValue<string>("Author")}</td><td>{h.GetValue<string>("AfterValue")}</td>
                                       </tr>");
                k--;
            }
            sbr.AppendFormat("</table>");
            content = sbr.ToString();
            return Content(content);
        }
    }
}