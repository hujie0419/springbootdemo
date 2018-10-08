using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;




namespace Tuhu.Provisioning.Controllers
{
    public class CarTagController : Controller
    {
        //
        // GET: /CarTag/

        public ActionResult Index()
        {

            return View(new CarTagManager().GetList());
        }


        public ActionResult GetCouponValidate(Guid? guid,string startDateTime,string endDateTime)
        {
            if (!guid.HasValue)
            {
                return Json(null);
            }
            else
            {
                PromotionActivityManager manager = new PromotionActivityManager();
                SE_GetPromotionActivityCouponInfoConfig model = manager.GetCouponValidate(guid.Value);
                if (model != null)
                {
                    SE_CarTagCouponConfig config = new SE_CarTagCouponConfig()
                    {
                         CouponGuid = guid.ToString(),
                         CreateDate = DateTime.Now,
                         Description = model.Description,
                         Discount = model.Discount,
                        // EndDateTime =Convert.ToDateTime(endDateTime??DateTime.Now.ToString()),
                         MinMoney= model.MinMoney,
                        // StartDateTime= Convert.ToDateTime(startDateTime??DateTime.Now.AddMonths(1).ToString()),
                         Status = true
                    };
                    if (new CarTagManager().Add(config))
                        return Json(config);
                    else
                        return Json(null);
                }
                return Json(null);
            }
        }

        public ActionResult UpdateStatus(int id, int status)
        {
            return Json( new CarTagManager().UpdateStatus(id, status)?1:0);
        }

        public ActionResult UpdateDateTime(DateTime startDateTime, DateTime endDateTime,string name)
        {
            return Json(new CarTagManager().UpdateDateTime(startDateTime,endDateTime,name)?1:0);
        }


        public ActionResult UpdateImageUrl(int id, string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return Json(0);
            return Json(new CarTagManager().UpdateImageUrl(id, url)?1:0);
        }


    }
}
