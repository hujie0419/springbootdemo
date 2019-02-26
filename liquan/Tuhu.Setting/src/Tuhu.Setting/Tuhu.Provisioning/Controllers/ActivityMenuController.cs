using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Controllers
{
    public class ActivityMenuController : Controller
    {    
        public ActionResult List(string activityID)
        {
            if (activityID != "0")
            {
                IEnumerable<ActivityMenu> list = ActivityMenuBll.GetList(activityID).Rows.Cast<DataRow>().Select(row => new ActivityMenu(row));
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
