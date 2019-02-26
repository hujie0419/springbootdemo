using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tuhu.Provisioning.Areas.Activity
{
    public class ActivityAreaRegistration:System.Web.Mvc.AreaRegistration
    {
        public override string AreaName
        {
            get { return "Activity"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                 this.AreaName + "_Default",
                 this.AreaName + "/{controller}/{action}/{id}",
                 new { area = this.AreaName, controller = "Home", action = "Index", id = UrlParameter.Optional },
                 new string[] { "Tuhu.Provisioning.Areas." + this.AreaName + ".Controllers" }
           );
        }

    }
}