using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.WebSite.Component.DataAccess;
using Tuhu.WebSite.Component.SystemFramework.Log;

namespace Tuhu.WebSite.Web.Activity.Controllers
{
    public class HwxController : Controller
    {
        //// GET: Hwx
        //public ActionResult AddUser()
        //{
        //    var uijson = Request.Form[WxProxy.USERINFO_WX];
        //    var px = WxProxy.Create(this);
        //    px.IsReady();
        //    if (!string.IsNullOrWhiteSpace(uijson))
        //    {
        //        WxUserInfo ui = uijson.FromJson<WxUserInfo>();
        //        if (string.IsNullOrEmpty(ui.headimgurl))
        //        {
        //            ui.headimgurl = "http://resource.tuhu.cn/Image/Product/jinlaohu.png";
        //        }
        //        if (string.IsNullOrEmpty(ui.nickname))
        //        {
        //            ui.nickname = "亲";
        //        }
        //        dynamic md = SqlAdapter.Create("hdAddWxUser", System.Data.CommandType.StoredProcedure)
        //            .Par("@oid", ui.openid, SqlDbType.UniqueIdentifier)
        //            .Par("@nick", ui.nickname, SqlDbType.NVarChar, 128)
        //            .Par("@img", ui.headimgurl, SqlDbType.NVarChar, 4000)
        //            .ExecuteModel();
        //        if (md.IsEmpty || !md.IsNotNull("wuid"))
        //        {
        //            WebLog.LogInfo("hdAddWxUser failed {0}", ui.ToJson());
        //        }
        //        else
        //        {
        //            return Content(md.wuid.ToString());
        //        }
        //    }
        //    return Content(string.Empty);
        //}
        //public ActionResult Entry(string cburl)
        //{
        //    var proxy = WxProxy.Create(this);
        //    if (!proxy.IsReady())
        //    {
        //        return proxy.GetInfo(cburl);
        //    }
        //    else
        //    {
        //        return Redirect(cburl);
        //    }
        //}
        //public ActionResult WxDetails(string key)
        //{
        //    var px = WxProxy.Create(this);
        //    if (string.IsNullOrEmpty(key) ? px.IsReady() : px.IsReady(key))
        //    {
        //        return Content(px.CachedUserInfo.ToJson());
        //    }
        //    return Content("WxInfo empty");
        //}
    }
}
