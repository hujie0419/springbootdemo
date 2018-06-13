using System;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Tuhu.WebSite.Component.Activity.Common;
using Tuhu.WebSite.Component.SystemFramework.Log;

namespace Tuhu.WebSite.Web.Activity
{
    public class Global : HttpApplication
	{
        protected void Application_Start()
		{
			MvcHandler.DisableMvcResponseHeader = true;

			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);

			RegisterRoutes(RouteTable.Routes);

            //RegisterLog4Net();

            RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            Response.Cache.SetOmitVaryStar(true);
            //Context.Response.StatusCode = 302;
            //Context.Response.Redirect("http://faxian.tuhu.test/Article/Detail?id=4460");

        }

        //protected void RegisterLog4Net()
        //{
        //    //log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(HostingEnvironment.MapPath("~/Site.config")));
           
        //    if ((System.Configuration.ConfigurationManager.AppSettings["Log4NetMode"] ?? "FileAppender").Equals("AdoNetAppender"))
        //        log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(HostingEnvironment.MapPath("~/Log4NetForMSSQL.config")));
        //    else
        //        log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(HostingEnvironment.MapPath("~/Log4NetForFile.config")));
        //}

        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        var exception = Server.GetLastError();
        //        if (new HttpException(null, exception).GetHttpCode() != 404)
        //        {
        //            WebLog.LogException(exception.Message, exception);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.LogException(ex.Message, ex);
        //    }
        //}

		public void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
            filters.Add(new FaxianHandleErrorAttribute());
            filters.Add(new Component.SystemFramework.Security.AppVersionAuthorizeAttribute());
		}

        public void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                        name: "toutiao_list",
                        url: "firstline",
                        defaults: new { controller = "Article", action = "Firstline" }
                    );
            routes.MapRoute(
                        name: "toutiao_listnew",
                        url: "find_new",
                        defaults: new { controller = "Article", action = "FindNew" }
                    );
            routes.MapRoute(
                "ID", // Route name
                "{controller}/{action}/{id}.html" // URL with parameters
            );

            routes.MapRoute(
                "Action", // Route name
                "{controller}/{action}.html" // URL with parameters
            );

            routes.MapRoute(
                "Controller", // Route name
                "{controller}.html", // URL with parameters
                new { action = "Index" }
            );

            routes.MapRoute(
                null, // Route name
                "", // URL with parameters
                new { controller = "Home", action = "Index" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        public static void RegisterBundles(BundleCollection bundles)
        {
            //文章详情的样式
            bundles.Add(new StyleBundle("~/Content/ArticleCss").Include(
                   "~/Content/detailCss/common.min.css",
                   "~/Content/detailCss/list.min.css",
                   "~/Content/detailCss/detail.min.css"));
        }

    }
}