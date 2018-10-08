using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    /// <summary>
    /// 2017年双11分品类品牌销量排名 CategoryBrand Rank 2017 11 11
    /// </summary>
    public class CBR20171111Controller : Controller
    {
        // GET: CBR20171111
        public ActionResult Index(long id = 0, DateTime? date = null)
        {
            var manager = new CategoryBrandRankManager();
            ViewBag.Categorys = manager.SelectCategoryBrand(0, null);
            ViewBag.PKID = id;
            ViewBag.Date = date == null
                ? DateTime.Now.ToString("yyyy-MM-dd")
                : date.Value.ToString("yyyy-MM-dd");
            if (id > 0)
            {
                ViewBag.Category = manager.FetchCategoryBrand(id);
                ViewBag.Brands = manager.SelectCategoryBrand(id, date.Value);
            }
            else
            {
                ViewBag.Category = new CategoryBrandRankModel();
                ViewBag.Brands = new List<CategoryBrandRankModel>();
            }

            return View("Index");
        }

        [HttpPost]
        public JsonResult AddCategory()
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            var javaScriptSerializer = new JavaScriptSerializer();
            var model = javaScriptSerializer.Deserialize<CategoryBrandRankModel>(stream);

            var manager = new CategoryBrandRankManager();
            model.PKID = manager.InsertCategoryBrand(model);
            return Json(new
            {
                code=1
            });
        }
        [HttpPost]
        public JsonResult SaveCategory() {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            var javaScriptSerializer = new JavaScriptSerializer();
            var model = javaScriptSerializer.Deserialize<CategoryBrandRankModel>(stream);

            var manager = new CategoryBrandRankManager();
            model.PKID = manager.UpdateCategoryBrand(model);
            return Json(new
            {
                code = 1
            });
        }

        [HttpPost]
        public JsonResult DeleteCategory(long id)
        {
            var manager = new CategoryBrandRankManager();
            manager.DeleteCategoryBrand(id);
            return Json(new
            {
                code = 1
            });
        }

        [HttpPost]
        public JsonResult SaveCategoryBrand()
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            var javaScriptSerializer = new JavaScriptSerializer();
            var models = javaScriptSerializer.Deserialize<IEnumerable<CategoryBrandRankModel>>(stream);


            var manager = new CategoryBrandRankManager();
            manager.UpdateCategoryBrands(models);
            return Json(new
            {
                code = 1
            });
        }
    }
}