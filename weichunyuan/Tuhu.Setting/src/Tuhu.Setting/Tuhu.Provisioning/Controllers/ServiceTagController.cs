using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.ServiceTag;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class ServiceTagController : Controller
    {

        public ActionResult ProductServiceTag()
        {
            var model = ServiceTagManager.SelectServiceTag();
            return View(model);
        }

        public ActionResult Index()
        {
            var model = ServiceTagManager.SelectServiceTag();
            return View(model);
        }

        public ActionResult Categorys(string id)
        {
            ViewBag.ID = id;
            return View();
        }

        public ActionResult DeleteTag(int PKID)
        {
            return Json(ServiceTagManager.DeleteTag(PKID));

        }


        public ActionResult InsertAndUpdateTag(string data)
        {
            IEnumerable<ServiceTagModel> list = JsonConvert.DeserializeObject<List<ServiceTagModel>>(data);
            int count = 0, allCount = 0;
            string b = "";
            var tag = false;
            foreach (var model in list)//判断是否重复
            {
                if (model.Type == 1)
                {
                    tag = true;
                    b += model.StrServiceIDs + ";";
                    var s = model.StrServiceIDs.Split(';');
                    string[] q = s.Distinct().ToArray();
                    foreach (var item in q)
                    {
                        if (item != "") allCount++;
                    }
                }
            }
            var tempArray = b.Split(';').Distinct().ToArray();
            foreach (var item in tempArray)
            {
                if (item != "") count++;
            }
            var falsePids = "";
            var result = ServiceTagManager.VaildatePID(b, ref falsePids);
            if (count != allCount && tag)
            {
                return Json(-5);
            }
            else if (result < 0 && tag)
            {
                if (result == -1) falsePids = "所有PID都不正确";
                return Json(falsePids);
            }
            else {
                return Json(ServiceTagManager.InsertAndUpdateTag(list));
            }
        }

    }
}
