using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.Models
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
            result.Data = new
            {
                Success = false,
                Message = filterContext.Exception.Message,
                InnerExcception= filterContext.Exception.InnerException,
                StackTrace = filterContext.Exception.StackTrace,
                Data= filterContext.Exception.Data
            };
            filterContext.Result = result;
            filterContext.ExceptionHandled = true;
        }
    }
}