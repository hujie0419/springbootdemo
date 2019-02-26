using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tuhu.Service.Product;
using Tuhu.Service.ConfigLog;
using Tuhu.Service.Product.Models;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.Business.ConfigLog;
using Tuhu.Provisioning.Business.ProductLimit;

namespace Tuhu.Provisioning.Controllers
{
    public class ProductLimitController : Controller
    {
        public static readonly JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings
        {
            DateFormatString = "yyyy-MM-dd HH:mm:ss"
        };

        readonly ProductLimitManager _manager = new ProductLimitManager();

        /// <summary>
        /// 获取类目的所有下级类目
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<ForIviewTreeModel> GetCatrgoryList(string category)
        {
            var list = new List<ForIviewTreeModel>();
            using (var client = new ProductClient())
            {
                var clientResult = client.GetCategoryDetailLevelsByCategory(category);
                if (clientResult.Success && clientResult.Result != null)
                {
                    var cateModel = clientResult.Result;
                    list.Add(new ForIviewTreeModel
                    {
                        expand = true,
                        level = 1,
                        title = cateModel.DisplayName,
                        value = cateModel.CategoryName,
                        levelName = cateModel.DisplayName,
                        limitCount = _manager.FetchCategoryLimitCount(new ProductLimitCountEntity { CategoryCode = cateModel.CategoryName, CategoryName = cateModel.DisplayName, CategoryLevel = 1 }).LimitCount,
                        children = (from item in cateModel?.ChildCategorys
                                    where item.ParentId == clientResult.Result.Id
                                    select new ForIviewTreeModel
                                    {
                                        level = item.Level,
                                        title = item.DisplayName,
                                        value = item.CategoryName,
                                        levelName = cateModel.DisplayName + " > " + item.DisplayName,
                                        limitCount = _manager.FetchCategoryLimitCount(new ProductLimitCountEntity { CategoryCode = item.CategoryName, CategoryName = item.DisplayName, CategoryLevel = item.Level }).LimitCount,
                                        children = GetChildCategory(cateModel?.ChildCategorys.ToList(), item.Id, cateModel.DisplayName + " > " + item.DisplayName)
                                    }).ToList()
                    });
                }
            }
            return list;
        }

        /// <summary>
        /// 递归类目集合
        /// </summary>
        /// <param name="cateList"></param>
        /// <param name="parentId"></param>
        /// <param name="parentName"></param>
        /// <returns></returns>
        public List<ForIviewTreeModel> GetChildCategory(List<ChildCategoryModel> cateList, int parentId, string parentName)
        {
            return (from item in cateList
                    where item.ParentId == parentId
                    select new ForIviewTreeModel
                    {
                        level = item.Level,
                        title = item.DisplayName,
                        value = item.CategoryName,
                        levelName = parentName + " > " + item.DisplayName,
                        limitCount = _manager.FetchCategoryLimitCount(new ProductLimitCountEntity { CategoryCode = item.CategoryName, CategoryName = item.DisplayName, CategoryLevel = item.Level }).LimitCount,
                        children = GetChildCategory(cateList, item.Id, parentName + " > " + item.DisplayName)
                    }).ToList();
        }

        /// <summary>
        /// 保存限购信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult SaveCategoryLimitCount(ProductLimitCountEntity model)
        {
            var dbResult = _manager.FetchCategoryLimitCount(model);
            if (dbResult == null || dbResult.PKID <= 0)
            {
                var result = _manager.InsertProductLimitCount(model);
                using (var client = new ConfigLogClient())
                {
                    client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = result,
                        ObjectType = "CategoryLimitCount",
                        BeforeValue = 0,
                        AfterValue = model.LimitCount,
                        Remark = "初次设置类目限购",
                        Creator = User.Identity.Name
                    }));
                }
                return Json(new { success = result > 0 });
            }
            else
            {
                var oldVal = dbResult.LimitCount;
                dbResult.LimitCount = model.LimitCount;
                var result = _manager.UpdateProductLimitCount(dbResult);
                using (var client = new ConfigLogClient())
                {
                    client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = dbResult.PKID,
                        ObjectType = "CategoryLimitCount",
                        BeforeValue = oldVal,
                        AfterValue = model.LimitCount,
                        Remark = "编辑类目限购数量",
                        Creator = User.Identity.Name
                    }));
                }
                return Json(new { success = result });
            }
        }

        /// <summary>
        /// 保存限购信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult SaveProductLimitCount(ProductLimitCountEntity model)
        {
            var dbResult = _manager.FetchProductLimitCount(model);
            if (dbResult == null || dbResult.PKID <= 0)
            {
                var result = _manager.InsertProductLimitCount(model);
                using (var client = new ConfigLogClient())
                {
                    client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = result,
                        ObjectType = "ProductLimitCountLog",
                        BeforeValue = "0",
                        AfterValue = model.LimitCount,
                        Remark = "初次设置产品限购",
                        Creator = User.Identity.Name
                    }));
                }
                return Json(new { success = result > 0 });
            }
            else
            {
                var oldVal = dbResult.LimitCount;
                dbResult.LimitCount = model.LimitCount;
                var result = _manager.UpdateProductLimitCount(dbResult);
                using (var client = new ConfigLogClient())
                {
                    client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                    {
                        ObjectId = dbResult.PKID,
                        ObjectType = "ProductLimitCountLog",
                        BeforeValue = oldVal,
                        AfterValue = model.LimitCount,
                        Remark = "编辑产品限购数量",
                        Creator = User.Identity.Name
                    }));
                }
                return Json(new { success = result });
            }
        }

        /// <summary>
        /// 批量设置产品限购数量
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult BatchSaveProductLimitCount(string model)
        {
            var errList = new List<string>();
            if (string.IsNullOrEmpty(model))
            {
                return Json(new { success = false });
            }
            var list = JsonConvert.DeserializeObject<List<ProductLimitCountEntity>>(model);
            if (list.Any())
            {
                foreach (var item in list)
                {
                    var dbResult = _manager.FetchProductLimitCount(item);
                    if (dbResult == null || dbResult.PKID <= 0)
                    {
                        var result = _manager.InsertProductLimitCount(item);
                        using (var client = new ConfigLogClient())
                        {
                            client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                            {
                                ObjectId = result,
                                ObjectType = "ProductLimitCountLog",
                                BeforeValue = "0",
                                AfterValue = item.LimitCount,
                                Remark = "初次设置产品限购",
                                Creator = User.Identity.Name
                            }));
                        }
                        if (result <= 0)
                            errList.Add(item.Pid);
                    }
                    else
                    {
                        var oldVal = dbResult.LimitCount;
                        dbResult.LimitCount = item.LimitCount;
                        var result = _manager.UpdateProductLimitCount(dbResult);
                        using (var client = new ConfigLogClient())
                        {
                            client.InsertDefaultLogQueue("CommonConfigLog", JsonConvert.SerializeObject(new
                            {
                                ObjectId = dbResult.PKID,
                                ObjectType = "ProductLimitCountLog",
                                BeforeValue = oldVal,
                                AfterValue = item.LimitCount,
                                Remark = "编辑产品限购数量",
                                Creator = User.Identity.Name
                            }));
                        }
                        if (!result)
                            errList.Add(item.Pid);
                    }
                }
                if (errList.Any())
                {
                    return Json(new { success = false, pidList = errList });
                }
            }
            else
            {
                return Json(new { success = false });
            }

            return Json(new { success = true });
        }

        /// <summary>
        ///根据类别获取品牌信息
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public ActionResult GetBrandsByCategory(string category)
        {
            var brandList = new List<string>();
            using (var client = new ProductClient())
            {
                var clientResult = client.GetBrandsByCategoryName(category);
                if (!clientResult.Success)
                    return Json(new { Success = false, Msg = "获取品牌列表失败", Data = brandList });

                brandList = clientResult.Result;
                return Json(new { Success = true, Msg = "", Data = brandList });
            }
        }

        /// <summary>
        /// 筛选产品列表
        /// </summary>
        /// <param name="category"></param>
        /// <param name="onSale"></param>
        /// <param name="brandName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>SaveProductLimitCount
        public ActionResult GetProductListByCagegoryCode(string category, string brandName, string onSale, string pid, string productName, int pageIndex, int pageSize)
        {
            var where = "";
            int total;
            where += !string.IsNullOrWhiteSpace(pid) ? $" AND CP.PID = '{pid}'" : "";
            where += !string.IsNullOrWhiteSpace(onSale) ? $" AND CP.OnSale = {onSale}" : "";
            where += !string.IsNullOrWhiteSpace(brandName) ? $" AND CP.CP_Brand = N'{brandName}'" : "";
            where += !string.IsNullOrWhiteSpace(productName) ? $" AND CP.DisplayName LIKE N'%{productName}%' " : "";
            var result = _manager.GetProductListByCategory(category, pageIndex, pageSize, where, out total);
            return Json(new { Success = true, Data = result, totalCount = total });
        }

        /// <summary>
        /// 获取通用日志列表
        /// </summary>
        /// <param name="category"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public JsonResult GetCommonConfigLogs(string category, int level)
        {
            var model = _manager.GetCategoryLimitCount(
                new ProductLimitCountEntity { CategoryCode = category, CategoryLevel = level });
            if (model != null && model.PKID > 0)
            {
                var manager = new CommonConfigLogManager();
                var pagination = new Pagination
                {
                    page = 1,
                    rows = 10000
                };
                var commonConfigLogList = manager.GetCommonConfigLogList(pagination, model.PKID.ToString(), "CategoryLimitCount");
                return Json(commonConfigLogList, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> RefreshCache()
        {
            using (var client = new CacheClient())
            {
                var result = await client.RefreshProductLimitCountWithPidListAsync(new List<string>());
                return Json(new { success = result.Success });
            }
        }
    }

    public class ForIviewTreeModel
    {
        public string title { get; set; }

        public string value { get; set; }

        public bool expand { get; set; }

        public int level { get; set; }

        public string levelName { get; set; }

        public int limitCount { get; set; }

        public List<ForIviewTreeModel> children { get; set; }
    }
}