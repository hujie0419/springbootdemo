using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Provisioning.Business.ItemRelease;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class ItemReleaseController : Controller
    {
        // GET: ItemRelease
        public ActionResult Index()
        {
            List<string> releaseItems = ItemReleaseManager.SelectTuhuReleaseModelByCache().Select(q => q.Name).OrderBy(p => p).ToList();
            return View(releaseItems);
        }

        public PartialViewResult ReleaseItemList(ReleaseItemModel model, int pageIndex = 1, int pageSize = 10)
        {
            var pager = new PagerModel(pageIndex, pageSize);
            var data = ItemReleaseManager.SelectList(model, pager);
            ViewBag.Select = model;
            return PartialView("ItemReleaseList", new ListModel<ReleaseItemModel>() { Pager = pager, Source = data });
        }


        public ActionResult SaveAddReleaseItemModel(ReleaseItemModel model)
        {
            var result = ItemReleaseManager.AddReleaseItem(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteReleaseItemModel(int pkid)
        {
            var result = ItemReleaseManager.DeleteReleaseItem(pkid);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateReleaseItemModel(ReleaseItemModel model)
        {
            var result = ItemReleaseManager.UpdateReleaseItem(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ItemReleaseAudit(long pkid, short status, string reason = default(string))
        {
            //发布需处理的状态
            if (status == 1 || status == 2)
            {
                var detail = ItemReleaseManager.FetchReleaseItem(pkid);
                if (detail == null)
                {
                    return Json(new
                    {
                        Code = 0
                    });
                }
                detail.Status = status;
                detail.Reason = reason;
                if (ItemReleaseManager.UpdateReleaseItem(detail))
                {

                    return Json(new
                    {
                        Code = 1
                    });
                }
            }

            return Json(new
            {
                Code = 0
            });
        }
    }
}