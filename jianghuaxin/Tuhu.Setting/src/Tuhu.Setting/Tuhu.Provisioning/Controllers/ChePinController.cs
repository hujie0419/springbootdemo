using Newtonsoft.Json;
using System;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.WebSiteHomeAd;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Config;

namespace Tuhu.Provisioning.Controllers
{
    public class ChePinController : Controller
    {
        //
        // GET: /ChePin/

        //配置首页
        public ActionResult Index()
        {
            return View(WebSiteHomeAdManager.SelectAllAdDetail("AutoProduct_"));//AutoProduct
        }
        //配置操作
        public ActionResult Config(string id, string remark)
        {
            if (String.IsNullOrWhiteSpace(id))
                return HttpNotFound();
            var result = WebSiteHomeAdManager.SelectAdDetailByID(id);
            ViewBag.haveflag = true;
            if (result == null)
            {
                result = new AdColumnModel();
                result.ID = id;
                result.Remark = remark;
                ViewBag.haveflag = false;
            }
            return View(result);
        }
        //配置操作弹窗
        public PartialViewResult ConfigBox(string remark, string type, string model, string act)
        {
            ViewBag.act = act;
            var data = new AdColumnModel();
            if (String.IsNullOrWhiteSpace(model))
                return null;
            else
                data = JsonConvert.DeserializeObject<AdColumnModel>(HttpUtility.UrlDecode(model));
            ViewBag.type = type;
            ViewBag.remark = remark;
            return PartialView(data);
        }
        //保存
        public ActionResult Save(string type, string model, string act)
        {
            var result = -1;
            if (String.Compare("adcolunm", type, true) == 0)//默认广告
            {
                var data = JsonConvert.DeserializeObject<AdColumnModel>(model);
                if (String.Compare("add", act, true) == 0)
                    result = WebSiteHomeAdManager.InsertAdDetail(data);
                else
                    result = WebSiteHomeAdManager.UpdateAdColumn(data);
            }
            else if (String.Compare("advertise", type, true) == 0)//非默认广告
            {
                var data = JsonConvert.DeserializeObject<AdvertiseModel>(model);
                if (String.Compare("add", act, true) == 0)
                    result = WebSiteHomeAdManager.InsertAdvertiseDetail(data, null);
                else
                    WebSiteHomeAdManager.UpdateAdvertise(data, null, out result);
            }
            return Json(result);
        }
        //更新到CouchBase
        public ActionResult UpdateToCouchBase()
        {
            using (var client = new CacheClient())
            {
                var result2 = client.RefreshHomeChePinAdvertiseCache();
                result2.ThrowIfException(true);
                return Json(result2.Result);
            }
        }

    }
}
