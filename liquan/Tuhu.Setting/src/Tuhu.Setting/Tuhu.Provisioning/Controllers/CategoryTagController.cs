using System;
using System.Linq;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.CategoryTag;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Controllers
{
    public class CategoryTagController : Controller
    {
        private static Lazy<CategoryTagManager> manager = new Lazy<CategoryTagManager>();
        private static CategoryTagManager GetManager
        {
            get
            {
                return manager.Value;
            }
        }

        //
        // GET: /CategoryTag/

        public ActionResult Index()
        {
            ViewBag.ParentList = GetManager.SelectById().Where(w => w.isParent.Equals(true)).ToList();
            return View();
        }

        #region 添加标签
        public ActionResult BigModule(int id = 0, int parentId = 0, int isAddParent = 0, int isAddChild = 0)
        {
            if (id != 0 && isAddChild == 0 && isAddParent == 0)
            {
                ViewBag.Title = "修改";

                if (GetManager.IsVehicle(id))
                {
                    ViewBag.VehicleList = GetManager.SelectByPID(2, 0, "name");
                }

                CategoryTagModel ctag = GetManager.SelectById(id).FirstOrDefault();
                ctag.updateTime = DateTime.Now;
                ctag.parentId = parentId;
                return View(ctag);
            }
            else if (id == 0 && parentId == 0 && isAddParent == 1 && isAddChild == 0)
            {
                ViewBag.Title = "新增父标签";
                CategoryTagModel ctag = new CategoryTagModel()
                {
                    isParent = true,
                    parentId = 0,
                    createTime = DateTime.Now,
                    updateTime = DateTime.Now
                };
                return View(ctag);
            }
            else
            {
                ViewBag.Title = "新增子标签";
                CategoryTagModel ctag = new CategoryTagModel()
                {
                    isParent = false,
                    parentId = parentId,
                    createTime = DateTime.Now,
                    updateTime = DateTime.Now
                };
                return View(ctag);
            }
        }

        public ActionResult SaveBigModule(CategoryTagModel model, int id = 0, int parentId = 0)
        {
            bool result = false;
            if (id == 0)
            {
                result = GetManager.Insert(model);
            }
            else
            {
                model.updateTime = DateTime.Now;
                model.parentId = (parentId == 0 ? 0 : parentId);
                result = GetManager.UpdateById(model);
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region 列表区域操作
        public ActionResult ShowModuleChildList(int id = 0)
        {
            ViewBag.ShowModuleChildList = GetManager.SelectById(null, id) ?? null;
            return View();
        }
        #endregion
    }
}