using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tuhu.WebSite.Component.SystemFramework;
using Tuhu.WebSite.Component.SystemFramework.Log;

namespace Tuhu.WebSite.Web.Activity.Controllers
{
    public class UserController:Controller
    {
        /// <summary>
        /// 查询发现个人中心，用户的信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ActionResult> SelectUserInfoForDiscovery(string userId)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("Code", "0");
            return Json(dic, JsonRequestBehavior.AllowGet);
            //try
            //{
            //    Guid uid;
            //    if (string.IsNullOrWhiteSpace(userId)||!Guid.TryParse(userId,out uid))
            //    {
            //        dic.Add("Code", "0");
            //        return Json(dic, JsonRequestBehavior.AllowGet);
            //    }
            //    var result = await UserObjectSystem.SelectUserInfoForDiscovery(userId);
            //    if (result == null)
            //    {
            //        dic.Add("Code", "0");
            //        return Json(dic, JsonRequestBehavior.AllowGet);
            //    }

            //    dic.Add("Code", "1");


            //    var vehicleLogo = ImageHelper.GetLogoUrlByName(result.Item5);
            //    var imageUrl = string.Concat(DomainConfig.ImageSite, vehicleLogo);


            //    dic.Add("Data", new { UserName = result.Item1, TargetUserName =ArticleController.GetUserName(result.Item1), UserHeader = result.Item2, UserGrade = result.Item3, UserSign = result.Item4, Vehicle = result.Item5, VehicleImageUrl = imageUrl });
            //    return Json(dic, JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception ex)
            //{
            //    WebLog.LogException(ex);
            //    dic.Clear();
            //    dic.Add("Code", "0");
            //    return Json(dic, JsonRequestBehavior.AllowGet);
            //}
        }
    }
} 