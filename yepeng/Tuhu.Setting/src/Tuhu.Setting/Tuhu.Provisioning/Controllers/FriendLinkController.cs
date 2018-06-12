using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.DownloadApp;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.OprLogManagement;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class FriendLinkController : Controller
    {

        private readonly Lazy<DownloadAppManager> lazy = new Lazy<DownloadAppManager>();

        private DownloadAppManager DownloadAppManager
        {
            get { return lazy.Value; }
        }

        public ActionResult Index()
        {
            List<FriendLink> list = DownloadAppManager.GetFriendLink();
            return View(list);
        }

        public ActionResult Add(FriendLink model)
        {
            return View(model);
        }

        [PowerManage]
        [HttpPost]
        public JsonResult Insert(FriendLink model)
        {
            int result = DownloadAppManager.InsertFriendLink(model);
            if (result > 0)
            {
                var oprLog = new OprLog
                {
                    ObjectID = result,
                    ObjectType = "Friendlink",
                    AfterValue = model.FriendLinkName + ", " + model.Link,
                    Author = HttpContext.User.Identity.Name,
                    Operation = "插入友情链接"
                };
                new OprLogManager().AddOprLog(oprLog);
            }
            return Json(result);
        }

        [PowerManage]
        public JsonResult Delete(int id)
        {
            bool result = DownloadAppManager.DeleteFriendLink(id);
            if (result)
            {
                var oprLog = new OprLog
                {
                    ObjectID = id,
                    ObjectType = "Friendlink",
                    AfterValue = "id是"+id+"的友链被删除",
                    Author = HttpContext.User.Identity.Name,
                    Operation = "删除友情链接"
                };
                new OprLogManager().AddOprLog(oprLog);
            }
            return Json(result);
        }

        [PowerManage]
        [HttpPost]
        public JsonResult Update(FriendLink model)
        {
            bool result = DownloadAppManager.UpdateFriendLink(model);
            if (result)
            {
                var oprLog = new OprLog
                {
                    ObjectID = model.Fid,
                    ObjectType = "Friendlink",
                    AfterValue = model.FriendLinkName + ", " + model.Link,
                    Author = HttpContext.User.Identity.Name,
                    Operation = "更改友情链接"
                };
                new OprLogManager().AddOprLog(oprLog);
            }
            return Json(result);
        }

        public ActionResult SelectOprLogByFid(int Fid)
        {
            var result = LoggerManager.SelectOprLogByParams("Friendlink", Fid.ToString());
            return result != null && result.Any()
                ? Json(new { status = "success", data = result }, JsonRequestBehavior.AllowGet)
                : Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
        }
    }
}
