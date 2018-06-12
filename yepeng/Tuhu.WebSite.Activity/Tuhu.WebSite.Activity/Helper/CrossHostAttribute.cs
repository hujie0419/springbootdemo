using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Tuhu.WebSite.Web.Activity.Helpers
{
    public class CrossHostAttribute : ActionFilterAttribute
    {
        /// <summary>为空是则为允许所有(*)</summary>
        public string HostRegex { get; set; }

        /// <summary>HostRegex为空是则为允许所有(*)</summary>
        public CrossHostAttribute() { }
        /// <summary>HostRegex为空是则为允许所有(*)</summary>
        public CrossHostAttribute(string appSettingKey)
        {
            HostRegex = WebConfigurationManager.AppSettings[appSettingKey];
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var context = filterContext.RequestContext.HttpContext;
            var origin = context.Request.Headers["Origin"];

            if (!string.IsNullOrWhiteSpace(origin))
            {
                if (string.IsNullOrWhiteSpace(HostRegex))
                    context.Response.AddHeader("Access-Control-Allow-Origin", "*");
                else if (Regex.IsMatch(origin, HostRegex))
                    context.Response.AddHeader("Access-Control-Allow-Origin", origin);
                //跨域的坑
                context.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");
                context.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Access-Control-Allow-Headers, Authorization, X-Requested-With");
                context.Response.AddHeader("Access-Control-Allow-Credentials", "true");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}