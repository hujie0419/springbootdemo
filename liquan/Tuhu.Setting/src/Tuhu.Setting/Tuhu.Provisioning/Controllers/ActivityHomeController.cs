using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business;
using System.Data;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Controllers
{
    public class ActivityHomeController : Controller
    {
        //
        // GET: /ActivityHome/

        public ActionResult Index(string activityID)
        {
            if (activityID != "0")
            {
                IEnumerable<ActivityHome> list = ActivityHomeBll.GetList(activityID).Rows.Cast<DataRow>().Select(row => new ActivityHome(row));
                if (list.Count() > 0)
                    return View(list);
                else
                    return View();
            }
            else
                return View();
        }
    }
}
