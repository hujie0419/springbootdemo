using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThBiz.Business.Power;
using ThBiz.DataAccess.Entity;

namespace Tuhu.MMS.Web.Filters
{
    /// <summary>
    ///　权限拦截
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class PowerManageAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// ActionKey
        /// </summary>
        public string ActionKey { get; set; }

        /// <summary>
        /// 是否是视图的ACTION
        /// </summary>
        public bool IsViewAction { get; set; }

        /// <summary>
        /// 所属系统
        /// </summary>
        private string _iwSystem = "BSys";
        public string IwSystem
        {
            get { return _iwSystem; }
            set { _iwSystem = value; }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
                filterContext.HttpContext.Response.Redirect("/Home/ErrorPage?error=权限不足，请联系部门管理员配置访问权限");
            List<ActionPower> listPower = new List<ActionPower>();
            string userNo = filterContext.HttpContext.User.Identity.Name;
            if (!string.IsNullOrEmpty(userNo))
            {
                byte issupper = 0;
                if (System.Configuration.ConfigurationManager.AppSettings["SupperUsers"].Contains(userNo))
                {
                    issupper = 1;
                }
                //listPower = new PowerManage().GetBusPower(userNo, issupper, IwSystem); 
                listPower = new PowerManage().GetBusPower(userNo, issupper, filterContext, IwSystem);
            }
            else
            {
                filterContext.HttpContext.Response.Redirect("/Home/ErrorPage?error=请先登录");
                filterContext.HttpContext.Response.End();
                filterContext.Result = new EmptyResult();
            }
            string controllerName = filterContext.RouteData.Values["controller"].ToString();
            string actionName = filterContext.RouteData.Values["action"].ToString();
            string msg = "";

            string query = filterContext.HttpContext.Request.Url.Query;
            if (string.IsNullOrEmpty(ActionKey))
                IsViewAction = true;
            bool bol = PowerHandle.PowerValidServer(listPower, userNo, controllerName, actionName, query, ActionKey, IsViewAction, out msg);
            string type = msg.Split('|')[1];
            msg = msg.Split('|')[0];
            if (!bol)
            {
                if (type.ToLower() == "function")
                    filterContext.HttpContext.Response.Write("{Status:false,Msg:\"" + msg + "\",IsPower:1}");
                else
                    filterContext.HttpContext.Response.Redirect("/Home/ErrorPage?error=" + msg);

                filterContext.HttpContext.Response.End();
                filterContext.Result = new EmptyResult();
            }
            base.OnActionExecuting(filterContext);
        }
    }
}