using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business;
using Newtonsoft.Json;

namespace Tuhu.Provisioning.Controllers
{
    public class EveryDaySeckillController : Controller
    {
        
        //
        // GET: /EveryDaySeckill/

        public ActionResult Index(string activityGuid = "", DateTime? startDate = null, DateTime? endDate = null)
        {
            EveryDaySeckillHandler handler = new EveryDaySeckillHandler();
            var list = handler.GetListByWhere(activityGuid, startDate, endDate);
            //var list = handler.GetList();
            return View(list);
        }


        public ActionResult Submit(string everyDaySeckill, string items)
        {
            if (string.IsNullOrWhiteSpace(everyDaySeckill))
                return Json(0);

            SE_EveryDaySeckill model = JsonConvert.DeserializeObject<SE_EveryDaySeckill>(everyDaySeckill);
            SE_EveryDaySeckillInfo info = JsonConvert.DeserializeObject<SE_EveryDaySeckillInfo>(items);
            model.EveryDaySeckillInfo = info;
            EveryDaySeckillHandler handler = new EveryDaySeckillHandler();

            if (handler.Submit(model, User))
                return Json(1);
            else
                return Json(0);
        }

        public ActionResult Edit(string activity = null)
        {
            ViewBag.ActivityGuid = activity;
            return View();
        }

        public ActionResult EditSearch(string activityGuid = "", DateTime? startDate = null, DateTime? endDate = null)
        {
            EveryDaySeckillHandler handler = new EveryDaySeckillHandler();
            var list = handler.GetListByWhere(activityGuid, startDate, endDate);
            var model = list.FirstOrDefault();
            ViewBag.ActivityGuid = model != null ? model.ActivityGuid.ToString() : "";
            return View("Edit");
        }



        public ActionResult GetEntity(Guid? activiity)
        {
            if (activiity.HasValue && activiity != null)
            {
                EveryDaySeckillHandler handler = new EveryDaySeckillHandler();
                var model = handler.GetEntity(activiity.Value);
                return Json(JsonConvert.SerializeObject(model));
            }
            else
                return Json(null);
        }

        public ActionResult SeckillProductList(string aid)
        {
            QiangGouModel model = null;
            Guid activityID;
            if (Guid.TryParse(aid, out activityID))
                model = QiangGouManager.FetchQiangGouAndProducts(activityID);
            return View(model);
        }

        public ActionResult GetSeckillEntity(Guid? activity)
        {
            if (activity.HasValue)
            {
                QiangGouModel model = QiangGouManager.FetchQiangGouAndProducts(activity.Value);
                if (model == null)
                    return null;

                return Json(new
                {
                    ActivityName = model.ActivityName,
                    StartDateTime = model.StartDateTime.ToString("yyyy-MM-dd HH:mm:ss")
                    ,
                    EndDateTime = model.EndDateTime.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            else
                return null;
        }


        public ActionResult ActivityOne()
        {
            return View();
        }

        public ActionResult ActivityThread()
        {
            return View();
        }

        public ActionResult CouponOne()
        {
            return View();
        }


        public ActionResult CouponThread()
        {
            return View();
        }

        public ActionResult Delete(Guid? activity)
        {
            if (activity.HasValue && activity != null)
            {
                EveryDaySeckillHandler handler = new EveryDaySeckillHandler();
                if (handler.Delete(activity.Value))
                    return Json(1);
                else
                    return Json(0);
            }
            return Json(0);
        }




    }
}
