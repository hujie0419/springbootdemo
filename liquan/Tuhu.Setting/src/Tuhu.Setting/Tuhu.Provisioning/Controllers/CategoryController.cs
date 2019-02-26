using System;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.Category;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class CategoryController : Controller
    {
        private readonly Lazy<CategoryDicManager> lazy = new Lazy<CategoryDicManager>();

        private CategoryDicManager CategoryDicManager
        {
            get { return this.lazy.Value; }
        }

        public ActionResult AddCategory()
        {
            return View();
        }

        public ActionResult AddModelFloor()
        {
            ViewData["CategoryIdName"] = CategoryDicManager.GetCategoryIdName();
            return View();
        }

        public ActionResult CategoryList()
        {
            ViewData["CategoryDic"] = CategoryDicManager.GetCategoryDic();
            return View();
        }

        public ActionResult ModelFloorList()
        {
            ViewData["NewAppData"] = CategoryDicManager.GetNewAppData();
            return View();
        }

        public ActionResult AddModelFloorModel(NewAppData model)
        {
            int i = CategoryDicManager.GetMaxModelFloor(model);
            i = i + 1;
            model.modelfloor = i;
            return Json(CategoryDicManager.InsertNewAppData(model));
        }

        public ActionResult AddCategoryModel(CategoryDic model)
        {
            model.Type = "app";
            model.ParentId = 0;
            return Json(CategoryDicManager.InsertCategory(model));
        }
    }
}
