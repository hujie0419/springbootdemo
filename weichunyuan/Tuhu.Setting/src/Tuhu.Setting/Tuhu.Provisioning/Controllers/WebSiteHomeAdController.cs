using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.BaoYang;
using Tuhu.Provisioning.Business.WebSiteHomeAd;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.Config;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Request;

namespace Tuhu.Provisioning.Controllers
{
    public class WebSiteHomeAdController : Controller
    {
        //
        // GET: /WebSiteHomeAd/
        [HttpPost]
        public ActionResult UpdateHomePageAdvertise()
        {
            var conn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            if (SecurityHelp.IsBase64Formatted(conn))
            {
                conn = SecurityHelp.DecryptAES(conn);
            }
            using (var dbhelper = new SqlDbHelper(conn))
            {
                var HomePageAdvertise = WebSiteHomeAdManager.SelectAllAdDetail("Home_");
            }
            using (var client = new CacheClient())
            {
                var result = client.RefreshHomePageAdvertiseCache();
                return Json(result.Result);
            }

        }
#if !DEBUG
        [PowerManage]
#endif
        public ActionResult WebSiteHomeIndex()
        {
            var r = WebSiteHomeAdManager.SelectAllAdDetail("Home_");
            return View(Tuple.Create(r, BaoYangManager.GetBaoyangIndexConfigItemList()));
        }
        public ActionResult WSHomeAdConfig(string id, string name, string remark, string description = "img")
        {
            if (String.IsNullOrWhiteSpace(id))
                return HttpNotFound();
            var result = WebSiteHomeAdManager.SelectAdDetailByID(id);
            ViewBag.haveflag = true;
            if (result == null)
            {
                result = new AdColumnModel();
                result.ID = id;
                result.ADCName = name;
                result.Remark = remark;
                ViewBag.haveflag = false;
            }
            ViewBag.description = description;
            result.Products = WebSiteHomeAdManager.SelectAdProductByID(id);
            return View(result);
        }

        public ActionResult SubmitWSHomeAdConfig(string type, string model, string products, string act)
        {
            var result = -1;
            IEnumerable<AdProductModel> Products = JsonConvert.DeserializeObject<IEnumerable<AdProductModel>>(products);
            if (String.Compare("adcolunm", type, true) == 0)
            {
                var data = JsonConvert.DeserializeObject<AdColumnModel>(model);
                data.Products = Products;
                if (String.Compare("add", act, true) == 0)
                    result = WebSiteHomeAdManager.InsertAdDetail(data);
                else
                    result = WebSiteHomeAdManager.UpdateAdColumn(data);
            }
            else if (String.Compare("advertise", type, true) == 0)
            {
                var data = JsonConvert.DeserializeObject<AdvertiseModel>(model);
                if (String.Compare("add", act, true) == 0)
                    result = WebSiteHomeAdManager.InsertAdvertiseDetail(data, Products);
                else
                {
                    WebSiteHomeAdManager.UpdateAdvertise(data, Products, out result);
                }
            }
            return Json(result);
        }
        public PartialViewResult WSHomeAdConfigPop(string remark, string type, string model, string act, string description = "img")
        {
            ViewBag.act = act;
            var data = new AdColumnModel();
            if (!string.IsNullOrWhiteSpace(model))
                data = JsonConvert.DeserializeObject<AdColumnModel>(HttpUtility.HtmlDecode(HttpUtility.UrlDecode(model)));
            ViewBag.type = type;
            ViewBag.description = description;
            ViewBag.remark = remark;
            if (String.Compare(remark, "product", true) == 0)
            {
                var products = WebSiteHomeAdManager.SelectAdProductByID(data.ID);
                if (data.Advertises == null || data.Advertises.Count() <= 0)
                    data.Products = products == null ? null : products.Where(p => p.AdvertiseID == 0);
                else
                    data.Products = products == null ? null : products.Where(p => p.AdvertiseID == data.Advertises.FirstOrDefault().PKID);
            }

            return PartialView(data);
        }

        public ActionResult ImageUploadToAli(string filepath, int maxwidth = 0, int maxheight = 0)
        {
            if (Request.Files.Count > 0 && !String.IsNullOrWhiteSpace(filepath))
            {
                var Imgfile = Request.Files[0];
                var buffer = new byte[Imgfile.ContentLength];
                Imgfile.InputStream.Read(buffer, 0, buffer.Length);

                //filepath = filepath + BitConverter.ToString(HashAlgorithm.Create("MD5").ComputeHash(buffer)).Replace("-", "").ToString() + Path.GetExtension(Imgfile.FileName);

                using (var client = new FileUploadClient())
                {
                    var result = client.UploadImage(new ImageUploadRequest(filepath, buffer));
                    result.ThrowIfException(true);
                    if (result.Success)
                    {
                        return Content(result.Result);
                    }
                }
            }
            return Content(null);
        }

        public ActionResult DeleteAdvertise(int PKID)
        {
            return Json(WebSiteHomeAdManager.DeleteAdvertise(PKID));
        }

        public ActionResult DeleteProducts(string adcolumnID, string advertiseID)
        {
            if (String.IsNullOrWhiteSpace(adcolumnID))
                return Json(false);
            return Json(WebSiteHomeAdManager.DeleteProducts(adcolumnID, advertiseID) > 0);
        }
    }
}
