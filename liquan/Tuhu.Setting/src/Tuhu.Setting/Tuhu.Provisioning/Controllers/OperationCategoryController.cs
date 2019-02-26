using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business.OperationCategory;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business.Logger;
using Newtonsoft.Json;

namespace Tuhu.Provisioning.Controllers
{
    public class OperationCategoryController : Controller
    {
        private static string ObjectType = "OCC";
        private static Lazy<OperationCategoryManager> manager = new Lazy<OperationCategoryManager>();
        private static OperationCategoryManager GetManager => manager.Value;

        //
        // GET: /OperationCategory/
        [PowerManage]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 添加&编辑类目
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parentId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult AddOrEditCategory(int id = 0, int parentId = 0, int type = 0)
        {
            OperationCategoryModel ocModel = new OperationCategoryModel();
            if (id != 0)
            {
                ViewBag.Title = "修改";
                ocModel = GetManager.Select(id).FirstOrDefault();
                ocModel.Type = type;
                return View(ocModel);
            }
            else
            {
                ViewBag.Title = "新增";
                ocModel.Id = 0;
                ocModel.ParentId = parentId;
                ocModel.Type = type;
                return View(ocModel);
            }
        }

        /// <summary>
        /// 删除类目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteCategory(int id = 0)
        {
            if (id > 0)
            {
                return GetManager.Delete(id) ? 1 : 0;
            }
            return 0;
        }

        public ActionResult SaveCategory(OperationCategoryModel model, int id = 0)
        {
            bool result = false;
            if (id == 0)
            {
                model.CreateTime = DateTime.Now;
                model.Id = id;
                result = GetManager.Insert(model);
                if (result) { LoggerManager.InsertOplog(User.Identity.Name, ObjectType, model.Id, "添加车品类目" + model.DisplayName); }
            }
            else
            {
                model.CreateTime = DateTime.Now;
                model.Id = id;
                result = GetManager.Update(model);
                if (result) { LoggerManager.InsertOplog(User.Identity.Name, ObjectType, model.Id, "修改车品类目" + model.DisplayName); }
            }

            return RedirectToAction("Index");
        }

        public ActionResult TreeTableList(int type = 0)
        {
            ViewBag.TreeTableList = GetManager.Select(null, null, type);
            return View();
        }

        /// <summary>
        /// 后台类目产品列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult ProductTreeList(int id = 0, int type = 0)
        {
            ViewBag.id = id;
            ViewBag.type = type;

            var cateItems = GetManager.SelectProductCategories();
            var productList = GetManager.SelectOperationCategoryProducts(id);

            if (productList != null && productList.Count() > 0)
            {
                foreach (var pitem in productList)
                {
                    foreach (var citem in cateItems)
                    {
                        if (pitem.CorrelId.Equals(citem.id))
                        {
                            citem.open = true;
                            citem.isChecked = true;
                        }
                    }
                }
            }

            ViewBag.CategoryTagManager = JsonConvert.SerializeObject(cateItems).Replace("isChecked", "checked");
            return View();
        }

        public int SaveProductTreeList(int id, int type, string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                bool result = false;
                List<ZTreeTagModel> dicTreeItems = JsonConvert.DeserializeObject<List<ZTreeTagModel>>(data);
                if (dicTreeItems != null && dicTreeItems.Count > 0)
                {
                    List<OperationCategoryProductsModel> pmodel = new List<OperationCategoryProductsModel>();
                    for (int i = 0; i < dicTreeItems.Count; i++)
                    {
                        pmodel.Add(new OperationCategoryProductsModel()
                        {
                            OId = id,
                            CorrelId = dicTreeItems[i].key,
                            CategoryCode = "",
                            DefinitionCode = "",
                            Sort = i,
                            IsShow = 1,
                            Type = type,
                            CreateTime = DateTime.Now
                        });
                    }
                    result = GetManager.UpdateOperationCategoryProducts(id, type, pmodel);
                }
                else {
                    result = GetManager.UpdateOperationCategoryProducts(id, type, null);
                }
                return result ? 1 : 0;
            }
            return 0;
        }

        /// <summary>
        /// 搜索类目对应产品
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult SearchProductCategories(string data, int id = 0, int type = 0)
        {
            bool isSearch = false;                                                                                                      //判断是否搜索
            List<VW_ProductsModel> productCategoriesList = null;                                                                        //搜索匹配产品
            List<VW_ProductsModel> productCategoriesListForDefaultCheck = null;                                                         //默认选中保存产品
            IEnumerable<OperationCategoryProductsModel> productList = id > 0 ? GetManager.SelectOperationCategoryProducts(id) : null;   //类目关联外部产品列表
            if (!string.IsNullOrEmpty(data))
            {
                List<ZTreeTagModel> dicTreeItems = JsonConvert.DeserializeObject<List<ZTreeTagModel>>(data);
                if (dicTreeItems != null && dicTreeItems.Count > 0)
                {
                    isSearch = true;
                    productCategoriesList = GetManager.SelectProductCategories((from n in dicTreeItems select n.key.ToString()), false).ToList() ?? null;

                    if (productList != null && productList.Count() > 0)
                    {
                        productCategoriesListForDefaultCheck = GetManager.SelectProductCategories((from n in productList select n.CorrelId.ToString()), true).ToList() ?? null;
                        productCategoriesListForDefaultCheck.ForEach(x => x.isChecked = true); //默认为选中
                    }
                }
                else if (productList != null && productList.Count() > 0)
                {
                    isSearch = false;
                    productCategoriesList = GetManager.SelectProductCategories((from n in productList select n.CorrelId.ToString()), true).ToList() ?? null;
                    productCategoriesList.ForEach(x => x.isChecked = true); //默认为选中
                }

                //剔除搜索中存在的，以保存产品
                if (isSearch &&
                    productList != null && productList.Count() > 0 &&
                    productCategoriesList != null && productCategoriesList.Count() > 0)
                {
                    foreach (var plitem in productList)
                    {
                        productCategoriesList.Remove(productCategoriesList.FirstOrDefault(w => w.oid.Equals(plitem.CorrelId)));
                    }

                    if (productCategoriesListForDefaultCheck != null)
                    {
                        //将以保存产品，添加到搜索列表顶部
                        productCategoriesList.InsertRange(0, productCategoriesListForDefaultCheck);
                    }
                }
                ViewBag.JsonData = productCategoriesList;
            }
            return View();
        }
    }
}