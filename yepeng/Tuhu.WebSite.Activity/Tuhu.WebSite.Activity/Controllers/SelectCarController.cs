using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tuhu.WebSite.Web.Activity.Controllers
{
    public class SelectCarController : Controller
    {
        // GET: SelectCar
        public ActionResult Index(TempCarShow TempCar)
        {
            //FetchDefaultCar
            return View(TempCar);
        }
        public class TempCarShow
        {
            public string Brand { get; set; }
            public string VehicleID { get; set; }
            public string VehicleName { get; set; }
            public string PaiLiang { get; set; }
            public string Nian { get; set; }
            public string liYangID { get; set; }
            public string liYangName { get; set; }
            public string UserId { get; set; }
            public int Step { get; set; }//1-小保养2-一元秒杀
        }
    }
}