using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.WebSite.Component.SystemFramework.Log;

namespace Tuhu.WebSite.Web.Activity.Filters
{
    /// <summary>
    /// 异常记录日志
    /// </summary>
    public class ExceptionLogAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            WebLog.LogException(filterContext.Exception);
            var result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            result.Data = new { success = false, msg = filterContext.Exception.Message + ":" + filterContext.Exception.StackTrace, InnerException = filterContext.Exception.InnerException != null ? filterContext.Exception.InnerException.Message : string.Empty };
            filterContext.Result = result;
            filterContext.ExceptionHandled = true;
        }
    }
}