using System.Web.Mvc;

namespace Tuhu.Provisioning.Areas.ActivityPage
{
    public class ActivityPageAreaRegistration : AreaRegistration 
    {
        public override string AreaName
        {
            get
            {
                return "ActivitySetting";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                this.AreaName + "_default",
                this.AreaName + "/Activity/{action}/{id}",
                new { Controller = "ActivityPage", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}