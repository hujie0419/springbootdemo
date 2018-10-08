using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;
using Newtonsoft.Json;

namespace Tuhu.Provisioning.Controllers
{
    public class GroupBuyingController :Controller
    {
        public ActionResult List()
        {
            return View(SE_GroupBuyingManager.GetList().ToList());
        }


        public ActionResult Edit(int id = 0)
        {
            if (id == 0)
                return View(new SE_GroupBuyingConfig());
            return View(SE_GroupBuyingManager.GetEntity(id));
        }

        public ActionResult GetFalshSaleProduct(string pid, Guid guid)
        {
            using (var client = new  Tuhu.Service.Activity.FlashSaleClient())
            {
               var result =  client.GetFlashSaleList(new Guid[] { guid });
                result.ThrowIfException(true);
                if (result.Success)
                {
                    var item = result.Result.FirstOrDefault().Products.Where(o => o.PID == pid).FirstOrDefault();
                    return Content(JsonConvert.SerializeObject(new
                    {
                        item.PID,
                        item.ActivityID,
                        item.FalseOriginalPrice,
                        item.Price,
                        item.SaleOutQuantity,
                        item.MaxQuantity,
                        item.ProductName
                    }));
                }
                else
                {
                    return Content(JsonConvert.SerializeObject(null));
                }
            }
        }


        public ActionResult Save(SE_GroupBuyingConfig model)
        {
            if (model.ID == 0)
            {
                if (SE_GroupBuyingManager.Add(model))
                    return Json(1);
                else
                    return Json(0);
            }
            else
                if (SE_GroupBuyingManager.Update(model))
                return Json(1);
            else
                return Json(0);
        }


        public ActionResult Del(int id)
        {
            if (SE_GroupBuyingManager.Delete(id))
            {
                return Json(1);
            }
            else
                return Json(0);

        }


    }
}