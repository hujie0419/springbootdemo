using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tuhu.Provisioning.Filters
{
    public class NoCacheFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var response = filterContext.HttpContext.Response;
            response.Cache.SetCacheability(HttpCacheability.NoCache);
        }
    }
}