using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tuhu.MMS.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILog logger;

        public HomeController(ILog logger)
        {
            this.logger = logger;
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}