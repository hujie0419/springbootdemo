using System;
using System.Web;
using System.Web.Mvc;
using Tuhu.WebSite.Component.SystemFramework.Log;

namespace Tuhu.WebSite.Web.Activity
{

    public class FaxianHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            if (!filterContext.IsChildAction && (!filterContext.ExceptionHandled && filterContext.HttpContext.IsCustomErrorEnabled))
            {
                var innerException = filterContext.Exception;

                if ((new HttpException(null, innerException).GetHttpCode() == 500) && this.ExceptionType.IsInstanceOfType(innerException))
                {
                    if (innerException.Message == "Additional text encountered after finished reading JSON content: ,. Path '', line 1, position 452.")
                    {
                    }
                    else
                    {
                        try {
                            WebLog.LogException(innerException);
                        } catch(Exception ex)
                        {

                        }
                    }


                    filterContext.Result = new JavaScriptResult { Script = "{\"Code\":\"0\",\"Message\":\"服务器忙\",\"message\":\"" + innerException.Message + "\"}" };
                    filterContext.ExceptionHandled = true;
                    filterContext.HttpContext.Response.Clear();
                    filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                    try
                    {
                        filterContext.HttpContext.Response.StatusCode = 500;
                    }
                    catch { }
                }
            }
        }
    }
}