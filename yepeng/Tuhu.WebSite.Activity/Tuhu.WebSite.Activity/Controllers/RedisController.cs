using HuoDong.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Tuhu.WebSite.Component.DataAccess;
using Tuhu.WebSite.Component.SystemFramework.Log;

namespace Tuhu.WebSite.Web.Activity.Controllers
{
    public class RedisController : Controller
    {
        protected RedisAdapter ra;
        protected Exception lasterror;
        public RedisController()
        {
            try
            {
                ra = RedisAdapter.Create(this.GetType().Name);
            }
            catch (Exception ex)
            {
                lasterror = ex;
                WebLog.LogException(ex);
            }
        }
        public ActionResult LastError()
        {
            return Content(lasterror != null ? lasterror.ToString() : "No error");
        }
    }
}
