using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class ShareOrderConfigController : Controller
    {
        private static readonly Lazy<ShareOrderConfigManager> lazy = new Lazy<ShareOrderConfigManager>();

        private static readonly Lazy<OprLogManager> lazyOprLog = new Lazy<OprLogManager>();

        private ShareOrderConfigManager ShareOrderConfigManager
        {
            get
            {
                return lazy.Value;
            }
        }

        private OprLogManager OprLogManager
        {
            get
            {
                return lazyOprLog.Value;
            }
        }

        public ActionResult Index(ShareOrderConfig model, int pageIndex = 1, int pageSize = 20)
        {
            int count = 0;
            string strSql = string.Empty;
            var lists = ShareOrderConfigManager.GetShareOrderConfigList(model, pageSize, pageIndex, out count);

            var list = new OutData<List<ShareOrderConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            return View(new ListModel<ShareOrderConfig>(list.ReturnValue, pager));
        }

        public ActionResult Edit(int id = 0)
        {
            if (id == 0)
            {
                return View(new ShareOrderConfig());
            }
            else
            {
                return View(ShareOrderConfigManager.GetShareOrderConfigById(id));
            }

        }

        public ActionResult LogList(int id)
        {
            var result = LoggerManager.SelectOprLogByParams("SESOC", id.ToString());
            return View(result);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ShareOrderConfig model)
        {
            string js = "<script>alert(\"保存失败 \");location='/ShareOrderConfig/Index';</script>";
            if (model.Id != 0)
            {
                AddOprLog(model, "修改");
                if (ShareOrderConfigManager.UpdateShareOrderConfig(model))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return Content(js);
                }
            }
            else
            {
                int id = 0;

                if (ShareOrderConfigManager.InsertShareOrderConfig(model, ref id))
                {
                    model.Id = id;
                    AddOprLog(model, "添加");
                    return RedirectToAction("Index");
                }
                else
                {
                    return Content(js);
                }
            }

        }




        public void AddOprLog(ShareOrderConfig model, string opr)
        {
            OprLog oprModel = new OprLog();
            if (model != null)
            {
                oprModel.AfterValue = JsonConvert.SerializeObject(model);
            }
            var result = ShareOrderConfigManager.GetShareOrderConfigById(model.Id);
            if (result != null)
            {
                oprModel.BeforeValue = JsonConvert.SerializeObject(result);
            }

            oprModel.Author = User.Identity.Name;
            oprModel.ChangeDatetime = DateTime.Now;
            oprModel.HostName = Request.UserHostName;
            oprModel.ObjectID = model.Id;
            oprModel.ObjectType = "SESOC";
            oprModel.Operation = opr;
            OprLogManager.AddOprLog(oprModel);
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            bool result = false;
            ShareOrderConfig model = new ShareOrderConfig();
            model.Id = id;

            if (ShareOrderConfigManager.DeleteShareOrderConfig(id))
            {
                AddOprLog(model, "删除");
                result = true;
            }
            return Json(result);
        }

        public ActionResult LogDetail(string before, string after)
        {
            if (!string.IsNullOrWhiteSpace(before))
            {
                ViewBag.BeforeValue = JsonConvert.DeserializeObject<ShareOrderConfig>(before);
            }
            else
            {
                ViewBag.BeforeValue = new ShareOrderConfig();
            }

            if (!string.IsNullOrWhiteSpace(after))
            {
                ViewBag.AfterValue = JsonConvert.DeserializeObject<ShareOrderConfig>(after);
            }
            else
            {
                ViewBag.AfterValue = new ShareOrderConfig();
            }

            return View();
        }



        public ActionResult PushMessage(OrderSharedPushMessageConfig model, int pageIndex = 1, int pageSize = 20)
        {
            int count = 0;
            string strSql = string.Empty;
            var lists = ShareOrderConfigManager.GetOrderSharedPushMessageConfig(model, pageSize, pageIndex, out count);

            var list = new OutData<List<OrderSharedPushMessageConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            return View(new ListModel<OrderSharedPushMessageConfig>(list.ReturnValue, pager));
        }



        public ActionResult EditMessage(int id = 0)
        {
            if (id == 0)
            {
                OrderSharedPushMessageConfig model = new OrderSharedPushMessageConfig();
                model.IOSModel = new IOSModel();
                model.AndriodModel = new AndriodModel();
                return View(model);
            }
            else
            {
                OrderSharedPushMessageConfig model = ShareOrderConfigManager.GetOrderSharedPushMessageConfig(id);
                model.AndriodModel = JsonConvert.DeserializeObject<AndriodModel>(model.AndroidCommunicationValue);
                model.IOSModel = JsonConvert.DeserializeObject<IOSModel>(model.IOSCommunicationValue);
                return View(model);
            }

        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveMessage(string data, string andriod, string ios)
        {
            OrderSharedPushMessageConfig model = JsonConvert.DeserializeObject<OrderSharedPushMessageConfig>(data);

            model.AndroidCommunicationValue = andriod;
            model.IOSCommunicationValue = ios;
            string js = "<script>alert(\"保存失败 \");location='/ShareOrderConfig/PushMessage';</script>";
            if (model.Id != 0)
            {

                if (ShareOrderConfigManager.UpdateOrderSharedPushMessageConfig(model))
                {
                    AddPustMessageOprLog(model, "修改");
                    return RedirectToAction("PushMessage");
                }
                else
                {
                    return Content(js);
                }
            }
            else
            {
                int id = 0;

                if (ShareOrderConfigManager.InsertOrderSharedPushMessageConfig(model, ref id))
                {
                    model.Id = id;
                    AddPustMessageOprLog(model, "添加");
                    return RedirectToAction("PushMessage");
                }
                else
                {
                    return Content(js);
                }
            }

        }

        [HttpPost]
        public JsonResult DeleteMessage(int id)
        {
            bool result = false;
            OrderSharedPushMessageConfig model = new OrderSharedPushMessageConfig();
            model.Id = id;

            if (ShareOrderConfigManager.DeleteOrderSharedPushMessageConfig(id))
            {
                AddPustMessageOprLog(model, "删除");
                result = true;
            }
            return Json(result);
        }

        public void AddPustMessageOprLog(OrderSharedPushMessageConfig model, string opr)
        {
            OprLog oprModel = new OprLog();
            oprModel.Author = User.Identity.Name;
            oprModel.ChangeDatetime = DateTime.Now;
            oprModel.HostName = Request.UserHostName;
            oprModel.ObjectID = model.Id;
            oprModel.ObjectType = "SEOSPM";
            oprModel.Operation = opr;
            OprLogManager.AddOprLog(oprModel);
        }
        public ActionResult LogPushMessage(int id)
        {
            var result = LoggerManager.SelectOprLogByParams("SEOSPM", id.ToString());
            return View(result);
        }

        public ActionResult GetOrderSharedPushMessageConfig()
        {
            return Json(ShareOrderConfigManager.GetOrderSharedPushMessageConfig(), JsonRequestBehavior.AllowGet);
        }

    }
}
