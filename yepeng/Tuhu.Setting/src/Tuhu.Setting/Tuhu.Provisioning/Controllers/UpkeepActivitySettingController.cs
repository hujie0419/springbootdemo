using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class UpkeepActivitySettingController : Controller
    {
        //    private static readonly Lazy<UpkeepActivitySettingManager> lazyUpkeepActivitySettingManager = new Lazy<UpkeepActivitySettingManager>();

        //    private UpkeepActivitySettingManager UpkeepActivitySettingManager
        //    {
        //        get { return lazyUpkeepActivitySettingManager.Value; }
        //    }

        //    public ActionResult Index(int pageIndex = 1, int pageSize = 15)
        //    {
        //        int count = 0;
        //        string strSql = string.Empty;
        //        var modelList = UpkeepActivitySettingManager.GetUpkeepActivitySetting(strSql, pageSize, pageIndex, out count);
        //        var list =  new OutData<List<UpkeepActivitySetting>, int>(new List<UpkeepActivitySetting>(), count);
        //        var pager = new PagerModel(pageIndex, pageSize)
        //        {
        //            TotalItem = count
        //        };
        //        return View(new ListModel<UpkeepActivitySetting>(list.ReturnValue, pager));
        //    }

        //    public ActionResult Edit(int id = 0)
        //    {
        //        if (id == 0)
        //        {
        //            return View(new UpkeepActivitySetting());
        //        }
        //        else
        //        {
        //            return View(UpkeepActivitySettingManager.GetUpkeepActivitySettingById(id));
        //        }

        //    }

        //    [HttpPost]
        //    public ActionResult Edit(UpkeepActivitySetting model)
        //    {
        //        string js = "<script>alert(\"保存失败 \");location='/ServiceTypeSetting/Index';</script>";
        //        if (model.Id != 0)
        //        {
        //            if (UpkeepActivitySettingManager.UpdateUpkeepActivitySetting(model))
        //            {
        //                return RedirectToAction("Index");
        //            }
        //            else
        //            {
        //                return Content(js);
        //            }
        //        }
        //        else
        //        {
        //            if (UpkeepActivitySettingManager.InsertUpkeepActivitySetting(model))
        //            {
        //                return RedirectToAction("Index");
        //            }
        //            else
        //            {
        //                return Content(js);
        //            }
        //        }
        //    }

        //    public ActionResult Delete(int id)
        //    {
        //        return Json(UpkeepActivitySettingManager.DeleteUpkeepActivitySetting(id));
        //    }
        //    public ActionResult ServiceType()
        //    {
        //        return View(UpkeepActivitySettingManager.GetServiceTypeSetting());
        //    }

        //    public ActionResult Brand(string catalogNames)
        //    {
        //        return View(UpkeepActivitySettingManager.GetRelevanceBrand(catalogNames));
        //    }

        //    public ActionResult Serie(string brands)
        //    {
        //        return View(UpkeepActivitySettingManager.GetRelevanceSeries(brands));
        //    }

        //    public ActionResult StoreAuthentication()
        //    {
        //        //UpkeepActivitySettingManager.GetStoreAuthentication()
        //        return View();
        //    }
    }
}
