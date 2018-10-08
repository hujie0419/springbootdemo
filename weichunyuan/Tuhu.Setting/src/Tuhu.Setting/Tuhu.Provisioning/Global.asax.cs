using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Filters;

namespace Tuhu.Provisioning
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ThreadIdentityActionFilterAttribute());
            filters.Add(new HandleErrorAttribute());
            filters.Add(new NoCacheFilter());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
            routes.MapRoute(
                "ZeroActivity", // Route name
                "{controller}/{action}" // URL with parameters
            );
        }

        protected void Application_Start()
        {        
            Qiniu.Conf.Config.Init();
            MVCControlsToolkit.Core.Extensions.Register();//this is the line of code to add
            AreaRegistration.RegisterAllAreas();
            //MVCControlsToolkit.Core.ClientDataTypeModelValidatorProviderExt.ErrorMessageResources = typeof(MVCNestedModels.Models.Resource1);
            MVCControlsToolkit.Core.ClientDataTypeModelValidatorProviderExt.NumericErrorKey = "NumericError";
            MVCControlsToolkit.Core.ClientDataTypeModelValidatorProviderExt.DateTimeErrorKey = "DateError";
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            LoggerFactory.ConfigureAndWatch(Server.MapPath("Log4net.config"));
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            string redirectUrl = this.Response.RedirectLocation;
            if (Response.StatusCode == 302 && !string.IsNullOrWhiteSpace(redirectUrl))
            {
                this.Response.RedirectLocation = Regex.Replace(redirectUrl, @"(?'1'\?|&)ReturnUrl=(?'2'.+?)(?'3'&|$)", m =>
                {
                    return string.Format("{0}ReturnUrl={1}{2}", m.Groups["1"].Value, HttpUtility.UrlEncode(new Uri(Request.Url, HttpUtility.UrlDecode(m.Groups["2"].Value)).ToString()), m.Groups["3"].Value);
                }, RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
           var logger = LogManager.GetLogger(typeof(MvcApplication));
            Exception ex = Server.GetLastError().GetBaseException();
            if (new HttpException(null, ex).GetHttpCode() != 404)
            {
                logger.Error(ex);
            }
        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            Response.Headers.Remove("Server");

            //将当前用户邮箱或UserId输出到Response
            if (Context.User.Identity.IsAuthenticated)
            {
                Response.Headers["UserId"] = Context.User.Identity.Name;
            }
        }
    }
}