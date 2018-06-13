using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using Tuhu.WebSite.Component.SystemFramework.Extensions;
using Tuhu.WebSite.Component.SystemFramework.Log;

namespace Tuhu.WebSite.Web.Activity
{
	public partial class Error : Page
	{
		protected int StatusCode { get; private set; }

		protected override void OnPreInit(EventArgs e)
		{
			base.OnPreInit(e);
			
			var exception = Server.GetLastError();
			if (exception != null)
			{
				var httpException = exception is HttpException ? exception as HttpException : new HttpException(null, exception.InnerException ?? exception);

				StatusCode = httpException.GetHttpCode();
				exception = exception.InnerException ?? exception;

                if (Request.IsAjaxRequest() && Context.IsCustomErrorEnabled)
                {
                    if (StatusCode == 500)
                    {
                        var exceptionID = WebLog.LogException(exception);
                        Context.Items["_ExceptionID_"] = exceptionID;

                        Response.Write("{ExceptionID:" + exceptionID + "}");
                    }
                    Response.End();
                }
                else if (StatusCode != 404)
                    Context.Items["_ExceptionID_"] = WebLog.LogException(exception);
            }
			else
			{
				int statusCode;
				if (int.TryParse(Request.QueryString["StatusCode"], out statusCode) && statusCode >= 400)
					StatusCode = statusCode;
				else
					StatusCode = 500;
			}

			Response.TrySkipIisCustomErrors = true;
			Response.StatusCode = StatusCode;
		}
	}
}