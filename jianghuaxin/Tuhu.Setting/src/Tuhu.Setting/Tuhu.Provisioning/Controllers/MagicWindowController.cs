using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Tuhu.Provisioning.Controllers
{
    public class MagicWindowController : Controller
    {
        // GET: MagicWindow
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult fetchMagicWindowList(int pageIndex = 1, int pageSize = 10)
        {
            var dic = new Dictionary<string, object>();
            var result = Business.MagicWindow.MagicWindowManager.SelectMagicWindowList(pageIndex, pageSize).ToList();
            if (result != null && result.Count > 0)
            {
                dic.Add("code", 1);
                dic.Add("totalPage", (result[0].Total - 1) / pageSize + 1);
                dic.Add("data", result);
            }
            else
            {
                dic.Add("code", 0);
                dic.Add("totalPage", 0);
            }

            return Json(dic, JsonRequestBehavior.AllowGet);
        }
        public ActionResult insertMagicWindow(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return Json(-1);
            return Json(Business.MagicWindow.MagicWindowManager.InsertMagicWindow(url));
        }

        public ActionResult UpdateMagicWindow(string url, int pkid)
        {
            if (string.IsNullOrWhiteSpace(url) || pkid <= 0) return Json(-1);
            return Json(Business.MagicWindow.MagicWindowManager.UpdateMagicWindow(url, pkid));
        }

        public ActionResult DeleteMagicWindow(int pkid)
        {
            if (pkid <= 0) return Json(-1);
            return Json(Business.MagicWindow.MagicWindowManager.DeleteMagicWindow(pkid));
        }
    }
}