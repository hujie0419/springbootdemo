using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.WebSite.Component.Activity.BusinessData;
using Tuhu.WebSite.Component.Activity.Business;
using Newtonsoft.Json;

namespace Tuhu.WebSite.Web.Activity.Controllers
{
    public class FAQShareController : Controller
    {
        private Lazy<FAQShareBLL> _FAQShareBLL = new Lazy<FAQShareBLL>();
        private FAQShareBLL InvokeFAQShareBLL => _FAQShareBLL.Value;

        /// <summary>
        /// 问题列表
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="QueryType">QueryType : 1 (专题问题), 2 （全局问题）</param>
        /// <returns></returns>
        [OutputCache(Duration = 600)]
        public JsonResult FAQList(int PKID = 0, int QueryType = 2)
        {
            FAQListModel data = (QueryType == 1 ? InvokeFAQShareBLL.GetFAQListForTopics(PKID) : InvokeFAQShareBLL.GetFAQList(PKID));
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  问题详情页面
        /// </summary>
        /// <param name="PKID"></param>
        /// <param name="ID"></param>
        /// <param name="QueryType">QueryType : 1 (专题问题), 2 （全局问题）</param>
        /// <returns></returns>
        [OutputCache(Duration = 600)]
        public JsonResult FAQDetail(int PKID = 0, int ID = 0, int QueryType = 2)
        {
            FAQDetailModel data = (QueryType == 1 ? InvokeFAQShareBLL.GetFAQDetailForTopics(PKID, ID) : InvokeFAQShareBLL.GetFAQDetail(PKID, ID));
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
