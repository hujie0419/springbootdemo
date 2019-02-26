using System.Web.Mvc;

namespace Tuhu.Provisioning.Areas.FeedBack
{
    public class FeedBackAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "FeedBack";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "FeedBack_default",
                this.AreaName + "/{action}/{id}",
                new {Controller= "FeedBack", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}