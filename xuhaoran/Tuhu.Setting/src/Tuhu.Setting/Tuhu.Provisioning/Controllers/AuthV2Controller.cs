using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Filters;

namespace Tuhu.Provisioning.Controllers
{
    public class AuthV2Controller : Controller
    {
        // GET: AuthV2
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult test()
        {
            string name = User.Identity.Name;
            return Json(new { success = true, result = false, messge = $"hello World:{name}" }, JsonRequestBehavior.AllowGet);
        }
    }
}