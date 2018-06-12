using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Component.Common.Models;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity.ProductSearch;
using Tuhu.Service.Product;

namespace Tuhu.Provisioning.Controllers
{
    public class SearchWordBrandsConfigController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List(string keyword, int pageIndex = 1, int pageSize = 20)
        {
            var count = 0;

            List<SearchWordBrandsConfig> lists = new List<SearchWordBrandsConfig>();
            lists = DALDefaultSearchConfig.GetSearchWordBrandsConfigList(keyword, pageSize, pageIndex, out count);
            var list = new OutData<List<SearchWordBrandsConfig>, int>(lists, count);
            var pager = new PagerModel(pageIndex, pageSize)
            {
                TotalItem = count
            };
            return View(new ListModel<SearchWordBrandsConfig>(list.ReturnValue, pager));
        }

        public ActionResult Edit(int id = 0)
        {
            if (id == 0)
            {
                SearchWordBrandsConfig model = new SearchWordBrandsConfig();
                return View(model);
            }
            else
            {
                return View(DALDefaultSearchConfig.GetSearchWordBrandsConfig(id));
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(SearchWordBrandsConfig model)
        {
            string js = "<script>alert(\"保存失败 {0}\");location='/SearchWordBrandsConfig/Index';</script>";
            
            if (model.BrandsList == null || !model.BrandsList.Any() || model.BrandsList.Count > 3)
                return Content(string.Format(js, "品牌词不少于1个不多于3个"));

            if (model.Pkid != 0)
            {
                if (DALDefaultSearchConfig.UpdateSearchWordBrandsConfig(model))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return Content(string.Format(js, ""));
                }
            }
            else
            {
                if (DALDefaultSearchConfig.InsertSearchWordBrandsConfig(model))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return Content(string.Format(js, ""));
                }
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            return Json(DALDefaultSearchConfig.DeleteSearchWordBrandsConfig(id));
        }

        [HttpPost]
        public JsonResult Refresh()
        {
            using (var product = new ProductSearchClient())
            {
                var refresh = product.RemoveRedisCacheKey("Product", "KeywordBrandCache");
                return Json(refresh.Result);
            }
        }

        public ActionResult Search()
        {
            return View();
        }

        public JsonResult GetSuggest(string keyWord)
        {
            using(var client=new ProductSearchClient())
            {
               var result= client.SearchSuggest(keyWord);
                if (result.Success)
                {
                    return Json( result.Result.Keys.ToList(), JsonRequestBehavior.AllowGet);
                }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}