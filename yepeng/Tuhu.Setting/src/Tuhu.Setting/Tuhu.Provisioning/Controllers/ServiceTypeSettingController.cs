using System;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class ServiceTypeSettingController : Controller
    {
        //private static readonly Lazy<UpkeepActivitySettingManager> lazyUpkeepActivitySettingManager = new Lazy<UpkeepActivitySettingManager>();

        //private UpkeepActivitySettingManager UpkeepActivitySettingManager
        //{
        //    get { return lazyUpkeepActivitySettingManager.Value; }
        //}

        //public ActionResult Index()
        //{
        //    return View(UpkeepActivitySettingManager.GetServiceTypeSetting());
        //}

        //public ActionResult Edit(ServiceTypeSetting model)
        //{
        //    return View(model);
        //}

        //[HttpPost]
        //public ActionResult Edit(ServiceTypeSetting model, int? Id = 0)
        //{
        //    string js = "<script>alert(\"保存失败 \");location='/ServiceTypeSetting/Index';</script>";
        //    if (model.Id != 0)
        //    {
        //        if (UpkeepActivitySettingManager.UpdateServiceTypeSetting(model))
        //        {
        //            return RedirectToAction("Index");
        //        }
        //        else
        //        {
        //            return Content(js);
        //        }
        //    }
        //    else
        //    {
        //        if (UpkeepActivitySettingManager.InsertServiceTypeSetting(model))
        //        {
        //            return RedirectToAction("Index");
        //        }
        //        else
        //        {
        //            return Content(js);
        //        }
        //    }
        //}

        //[HttpPost]
        //public JsonResult Delete(int Id)
        //{
        //    return Json(UpkeepActivitySettingManager.DeleteServiceTypeSetting(Id));
        //}
    }
}
