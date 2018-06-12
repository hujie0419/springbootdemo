using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tuhu.WebSite.Component.Activity.Business;
using Tuhu.WebSite.Component.Activity.BusinessData;
using System.Threading.Tasks;

namespace Tuhu.WebSite.Web.Activity.Controllers
{
    /// <summary>
    /// 会员中心Contoller
    /// </summary>
    public class UserPromotionController : Controller
    {
        /// <summary>
        /// 获取会员优惠券
        /// </summary>
        /// <param name="userRank"></param>
        /// <returns></returns>
        public ActionResult GetUserPromotionCode(string userRank)
        {
            JObject json = new JObject();
            if (string.IsNullOrEmpty(userRank))
            {
                json.Add("status", 0);
                json.Add("Content", "");
            }
            else
            {
                tbl_UserPromotionCodeBLL bll = new tbl_UserPromotionCodeBLL();
                List<tbl_UserPromotionCodeModel> list = bll.GetUserPromotionEntityList(userRank.ToUpper());
                json.Add("status", 1);
                json.Add("Content", JsonConvert.SerializeObject(list));
            }
            return JavaScript(json.ToString());
        }

        public ActionResult Coupon(string userRank)
        {
            tbl_UserPromotionCodeBLL bll = new tbl_UserPromotionCodeBLL();
            List<tbl_UserPromotionCodeModel> list = bll.GetUserPromotionEntityList(userRank.ToUpper());
            return View(list);
        }
       


    }
}