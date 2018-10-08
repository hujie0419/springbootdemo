using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.DataAccess.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Models;
using Tuhu.Service.Product.Enum;
using Tuhu.Service.Product.Request;
using System.Threading;
using System.Web.Caching;
using Tuhu.Provisioning.Models;
using System.Threading.Tasks;
using Tuhu.Service.Shop;
using Common.Logging;

namespace Tuhu.Provisioning.Controllers
{
    public class SE_MDBeautyCategoryProductConfigController : Controller
    {
        private static readonly object SyncLock = new object();
        private static readonly object SyncLock_Save = new object();
        private static ILog logger = LogManager.GetLogger<SE_MDBeautyCategoryProductConfigController>();
        //
        // GET: /SE_MDBeautyCategoryProductConfig/

        public ActionResult Index(int pageIndex = 1, int pageSize = 10, string categoryIds = "")
        {
            var data = SE_MDBeautyCategoryProductConfigBLL.Select(pageIndex, pageSize, categoryIds);

            int totalRecords = (data != null && data.Any())
                ? data.FirstOrDefault().TotalCount
                : 0;
            ViewBag.totalRecords = totalRecords;
            ViewBag.totalPage = totalRecords % pageSize == 0
                ? totalRecords / pageSize
                : (totalRecords / pageSize) + 1;

            return View(data);
        }

        //
        // GET: /SE_MDBeautyCategoryProductConfig/Edit/5

        public ActionResult Edit(int id = 0)
        {
            SE_MDBeautyCategoryProductConfigModel model = new SE_MDBeautyCategoryProductConfigModel();
            model.RecommendCar = 4; //默认选项

            if (id > 0)
            {
                model = SE_MDBeautyCategoryProductConfigBLL.Select(id);
            }

            ViewBag.ZTreeJsonForCategory = SE_MDBeautyCategoryConfigController.SE_MDBeautyCategoryTreeJson(model.CategoryIds, true);
            //ViewBag.ZTreeJsonForBrand = SE_MDBeautyBrandConfigController.SE_MDBeautyBrandTreeJson(model.Brands, true);
            ViewBag.ZTreeJsonForBrand = id > 0 ? SE_MDBeautyBrandTreeForCategoryIdJson(model.CategoryIds, model.Brands, true) : "[]";

            return View(model);
        }

        public ActionResult Save(SE_MDBeautyCategoryProductConfigModel model)
        {
            bool result = false;
            if (model != null)
            {
                try
                {
                    model.Brands = string.IsNullOrWhiteSpace(model.Brands) ? null : model.Brands;
                    if (model.PId == 0)
                    {
                        var userName = HttpContext.User?.Identity?.Name;
                        var data = SyncProdcutLibrary(model, userName);
                        logger.Info(data);
                        result = SE_MDBeautyCategoryProductConfigBLL.BatchInsertOrUpdateSyncProdcutLibrary(data, model);
                    }
                    else
                    {
                        result = SE_MDBeautyCategoryProductConfigBLL.Update(model);
                        using (var client = new Tuhu.Service.Shop.CacheClient())
                        {
                            client.UpdateBeautyProductDetailByPid(model.PId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message,ex);
                }

            }
            return RedirectToAction("Index", "SE_MDBeautyCategoryConfig");
        }

        /// <summary>
        /// 获取门店指定类目美容品牌树
        /// </summary>
        public string SE_MDBeautyBrandTreeForCategoryIdJson(string categoryId = "", string opens = "", bool isDisable = false)
        {
            opens = opens ?? "";
            var opensArr = opens.Split(',');
            var ZTreeModel = SE_MDBeautyBrandConfigBLL.SelectListForCategoryId(categoryId)?.Select(m => new
            {
                id = m.Id,
                pId = m.ParentId,
                name = m.BrandName,
                open = opensArr.Contains(m.Id.ToString().Trim()),
                @checked = opensArr.Contains(m.Id.ToString().Trim()),
                chkDisabled = false,
                disabled = m.IsDisable,
                isParent = m.ParentId == 0
            });

            if (isDisable)
                ZTreeModel = ZTreeModel.Where(m => m.disabled == false);

            return JsonConvert.SerializeObject(ZTreeModel);
        }

        /// <summary>
        /// 同步产品库
        /// </summary>
        private Dictionary<string, string> SyncProdcutLibrary(SE_MDBeautyCategoryProductConfigModel model, string userName)
        {
            lock (SyncLock_Save)
            {
                CachingLogsHelp.CacheItemRemove();

                List<string> SyncProdcutLibraryLog = new List<string>();
                Dictionary<string, string> dicSQL = new Dictionary<string, string>();
                List<BatchTreeModel> dataTreeItems = JsonConvert.DeserializeObject<List<BatchTreeModel>>(model?.Brands);

                if (dataTreeItems != null && dataTreeItems.Any())
                {
                    #region 检测产品
                    var _AdaptiveCarCheckBox = model.AdaptiveCarCheckBox?.Split(',');
                    List<string> sqlWhere = new List<string>();
                    if (_AdaptiveCarCheckBox != null && _AdaptiveCarCheckBox.Any())
                    {
                        if (dataTreeItems != null && dataTreeItems.Any())
                        {
                            foreach (var a in dataTreeItems)
                            {
                                if (!string.IsNullOrWhiteSpace(a.CategorysName))
                                {
                                    if (a.Childs != null && a.Childs.Any())
                                    {
                                        foreach (var b in a.Childs)
                                        {
                                            foreach (var c in _AdaptiveCarCheckBox?.ToList())
                                            {
                                                sqlWhere.Add(string.Format("{0}|{1}|{2}", model.CategoryIds, a.ParentId + "," + b.Id, c));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (var c in _AdaptiveCarCheckBox?.ToList())
                                        {
                                            sqlWhere.Add(string.Format("{0}|{1}|{2}", model.CategoryIds, a.ParentId, c));
                                        }
                                    }
                                }
                                else
                                {
                                    SyncProdcutLibraryLog.Add($"检测产品: {model.Brands} 数据异常，请打开重试！,操作时间:{DateTime.Now.ToString()}");
                                }
                            }
                        }
                    }
                    string checkSQL = @"SELECT * FROM 
                               (
	                                SELECT ISNULL(CategoryIds,'') + '|' + ISNULL(Brands,'') + '|'+ ISNULL(CONVERT(nvarchar,AdaptiveCar),'') as 'TreeItems',* FROM SE_MDBeautyCategoryProductConfig WITH(NOLOCK)
                               ) AS tab1
                               WHERE tab1.TreeItems IN(" + "'" + string.Join("','", sqlWhere) + "'" + ")";

                    IEnumerable<SE_MDBeautyCategoryProductConfigModel> dataList = SE_MDBeautyCategoryProductConfigBLL.CustomQuery<SE_MDBeautyCategoryProductConfigModel>(checkSQL);
                    #endregion

                    #region 公共变量
                    string sqlInsert = @" INSERT [SE_MDBeautyCategoryProductConfig] (
                                             [ProdcutId],
                                             [ProdcutName],
                                             [CategoryIds],
                                             [Describe],
                                             [Commission],
                                             [BeginPrice],
                                             [EndPrice],
                                             [BeginPromotionPrice],
                                             [EndPromotionPrice],
                                             [EveryDayNum],
                                             [Brands],
                                             [RecommendCar],
                                             [AdaptiveCar],
                                             [IsDisable],
                                             [CreateTime],
                                             [IsNotShow]) VALUES ";

                    string sqlUpdate = @" UPDATE [SE_MDBeautyCategoryProductConfig]
                                           SET [ProdcutName] = {1}
                                              ,[Describe] = {2}
                                              ,[Commission] = @Commission
                                              ,[BeginPrice] = @BeginPrice
                                              ,[EndPrice] = @EndPrice
                                              ,[BeginPromotionPrice] = @BeginPromotionPrice
                                              ,[EndPromotionPrice] = @EndPromotionPrice
                                              ,[EveryDayNum] = @EveryDayNum
                                              ,[IsDisable] = @IsDisable
                                              ,[CreateTime] = GETDATE()
                                              ,[IsNotShow]=@IsNotShow 
                                          WHERE PId IN({0}) ";

                    List<string> sqlInsertWhere = new List<string>();
                    List<SE_MDBeautyCategoryProductConfigModel> sqlUpdateWhere = new List<SE_MDBeautyCategoryProductConfigModel>();
                    #endregion

                    #region 遍历品牌
                    foreach (var item in dataTreeItems)
                    {
                        if (!string.IsNullOrWhiteSpace(item.CategorysName))
                        {
                            if (item.Childs != null && item.Childs.Any()) //判断是否遍历子系列
                            {
                                #region 遍历系列
                                foreach (var itemChilds in item.Childs)
                                {
                                    #region 遍历车型
                                    using (IProductClient client = new ProductClient())
                                    {
                                        foreach (var itemCar in _AdaptiveCarCheckBox?.ToList())
                                        {
                                            Thread.Sleep(2000); //延时操作

                                            string itemCarName = (itemCar == "1" ? "五座轿车"
                                                                : itemCar == "2" ? "SUV/MPV"
                                                                : itemCar == "3" ? "SUV"
                                                                : itemCar == "4" ? "MPV"
                                                                : "");

                                            string _TreeItems = string.Format("{0}|{1}|{2}", model.CategoryIds, item.ParentId + "," + itemChilds.Id, itemCar);
                                            var _compareData = dataList?.Where(_ => _.TreeItems == _TreeItems)?.FirstOrDefault();
                                            if (_compareData == null)
                                            {
                                                #region 同步添加到产品库
                                                Service.OperationResult<string> createResult = client.CreateProductV2(
                                                new WholeProductInfo
                                                {
                                                    Name = item.Name + itemChilds.Name + itemCarName + item.CategorysName,
                                                    DisplayName = item.Name + itemChilds.Name + itemCarName + item.CategorysName,
                                                    Description = model.Describe?.Replace("$1", item.CategorysName)?.Replace("$2", itemChilds.Name),
                                                    PrimaryParentCategory = model.PrimaryParentCategory,
                                                    ProductID = model.ProdcutId,  //Common.PinYinConverter.ConvertToFirstSpell(item.Name + itemChilds.Name + itemCarName + item.CategorysName),
                                                    VariantID = null,
                                                    Image_filename = model.Image_filename,
                                                    CatalogName = "CarPAR",
                                                    DefinitionName = model.DefinitionName,
                                                    CreateDatetime = DateTime.Now
                                                },
                                                userName,
                                                ChannelType.MenDian);

                                                if (createResult.Success && !string.IsNullOrWhiteSpace(createResult?.Result))
                                                {
                                                    sqlInsertWhere.Add(string.Format(" ( N'{14}',N'{0}',N'{1}',N'{2}',{3},{4},{5},{6},{7},{8},N'{9}',{10},{11},{12},N'{13}',{15}) \r\n",
                                                        item.Name + itemChilds.Name + itemCarName + item.CategorysName,
                                                        model.CategoryIds,
                                                        model.Describe?.Replace("$1", item.CategorysName)?.Replace("$2", itemChilds.Name),
                                                        model.Commission,
                                                        model.BeginPrice,
                                                        model.EndPrice,
                                                        model.BeginPromotionPrice,
                                                        model.EndPromotionPrice,
                                                        model.EveryDayNum,
                                                        item.ParentId + "," + itemChilds.Id,
                                                        model.RecommendCar,
                                                        itemCar,
                                                        model.IsDisable.GetHashCode(),
                                                        model.CreateTime,
                                                        createResult.Result,  //产品ID
                                                        model.IsNotShow.GetHashCode()
                                                     ));
                                                }
                                                SyncProdcutLibraryLog.Add($"子集：添加,操作人:{userName},ErrorMessage:{createResult.ErrorMessage},Exception:{createResult.Exception},ErrorCode:{createResult.ErrorCode},Result:{createResult.Result},Success:{createResult.Success},操作时间:{DateTime.Now.ToString()}");
                                                #endregion
                                            }
                                            else
                                            {
                                                #region 同步修改产品库
                                                Service.OperationResult<bool> updateResult = client.UpdateProduct(
                                                new WholeProductInfo
                                                {
                                                    Name = item.Name + itemChilds.Name + itemCarName + item.CategorysName,
                                                    DisplayName = item.Name + itemChilds.Name + itemCarName + item.CategorysName,
                                                    Description = model.Describe?.Replace("$1", item.CategorysName)?.Replace("$2", ""),
                                                    PrimaryParentCategory = model.PrimaryParentCategory,
                                                    ProductID = _compareData.ProdcutId.Split('|')[0],
                                                    VariantID = _compareData.ProdcutId.Split('|')[1],
                                                    Image_filename = model.Image_filename,
                                                    CatalogName = "CarPAR",
                                                    DefinitionName = model.DefinitionName
                                                },
                                                userName,
                                               ChannelType.MenDian);

                                                if (updateResult.Success && updateResult.Result)
                                                {
                                                    sqlUpdateWhere.Add(new SE_MDBeautyCategoryProductConfigModel()
                                                    {
                                                        ProdcutId = _compareData.ProdcutId,
                                                        PId = _compareData.PId,
                                                        ProdcutName = item.Name + itemChilds.Name + itemCarName + item.CategorysName,
                                                        Describe = model.Describe?.Replace("$1", item.CategorysName)?.Replace("$2", itemChilds.Name),
                                                    });
                                                }
                                                SyncProdcutLibraryLog.Add($"子集：修改,操作人:{userName},ErrorMessage:{updateResult.ErrorMessage},Exception:{updateResult.Exception},ErrorCode:{updateResult.ErrorCode},ProdcutId:{_compareData.ProdcutId},Result:{updateResult.Result},Success:{updateResult.Success},操作时间:{DateTime.Now.ToString()}");
                                                #endregion
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            else
                            {
                                #region 遍历车型
                                using (IProductClient client = new ProductClient())
                                {
                                    foreach (var itemCar in _AdaptiveCarCheckBox?.ToList())
                                    {
                                        Thread.Sleep(2000); //延时操作

                                        string itemCarName = (itemCar == "1" ? "五座轿车"
                                                            : itemCar == "2" ? "SUV/MPV"
                                                            : itemCar == "3" ? "SUV"
                                                            : itemCar == "4" ? "MPV"
                                                            : "");

                                        string _TreeItems = string.Format("{0}|{1}|{2}", model.CategoryIds, item.ParentId, itemCar);
                                        var _compareData = dataList?.Where(_ => _.TreeItems == _TreeItems)?.FirstOrDefault();
                                        if (_compareData == null)
                                        {
                                            #region 同步添加到产品库
                                            Service.OperationResult<string> createResult = client.CreateProductV2(
                                            new WholeProductInfo
                                            {
                                                Name = item.Name + itemCarName + item.CategorysName,
                                                DisplayName = item.Name + itemCarName + item.CategorysName,
                                                Description = model.Describe?.Replace("$1", item.CategorysName)?.Replace("$2", ""),
                                                PrimaryParentCategory = model.PrimaryParentCategory,
                                                ProductID = model.ProdcutId, //Common.PinYinConverter.ConvertToFirstSpell(item.Name + itemCarName + item.CategorysName),
                                                VariantID = null,
                                                Image_filename = model.Image_filename,
                                                CatalogName = "CarPAR",
                                                DefinitionName = model.DefinitionName
                                            },
                                            userName,
                                            ChannelType.MenDian);

                                            if (createResult.Success && !string.IsNullOrWhiteSpace(createResult?.Result))
                                            {
                                                sqlInsertWhere.Add(string.Format(" ( N'{14}',N'{0}',N'{1}',N'{2}',{3},{4},{5},{6},{7},{8},N'{9}',{10},{11},{12},N'{13}',{15}) \r\n",
                                                    item.Name + itemCarName + item.CategorysName,
                                                    model.CategoryIds,
                                                    model.Describe?.Replace("$1", item.CategorysName)?.Replace("$2", ""),
                                                    model.Commission,
                                                    model.BeginPrice,
                                                    model.EndPrice,
                                                    model.BeginPromotionPrice,
                                                    model.EndPromotionPrice,
                                                    model.EveryDayNum,
                                                    item.ParentId,
                                                    model.RecommendCar,
                                                    itemCar,
                                                    model.IsDisable.GetHashCode(),
                                                    model.CreateTime,
                                                    createResult.Result,  //产品ID
                                                    model.IsNotShow.GetHashCode()
                                                ));
                                            }
                                            SyncProdcutLibraryLog.Add($"添加,操作人:{userName},ErrorMessage:{createResult.ErrorMessage},Exception:{createResult.Exception},ErrorCode:{createResult.ErrorCode},Result:{createResult.Result},Success:{createResult.Success},操作时间:{DateTime.Now.ToString()}");
                                            #endregion
                                        }
                                        else
                                        {
                                            #region 同步修改产品库
                                            Service.OperationResult<bool> updateResult = client.UpdateProduct(
                                            new WholeProductInfo
                                            {
                                                Name = item.Name + itemCarName + item.CategorysName,
                                                DisplayName = item.Name + itemCarName + item.CategorysName,
                                                Description = model.Describe?.Replace("$1", item.CategorysName)?.Replace("$2", ""),
                                                PrimaryParentCategory = model.PrimaryParentCategory,
                                                ProductID = _compareData.ProdcutId.Split('|')[0],
                                                VariantID = _compareData.ProdcutId.Split('|')[1],
                                                Image_filename = model.Image_filename,
                                                CatalogName = "CarPAR",
                                                DefinitionName = model.DefinitionName
                                            },
                                            userName,
                                           ChannelType.MenDian);

                                            if (updateResult.Success && updateResult.Result)
                                            {
                                                sqlUpdateWhere.Add(new SE_MDBeautyCategoryProductConfigModel()
                                                {
                                                    ProdcutId = _compareData.ProdcutId,
                                                    PId = _compareData.PId,
                                                    ProdcutName = item.Name + itemCarName + item.CategorysName,
                                                    Describe = model.Describe?.Replace("$1", item.CategorysName)?.Replace("$2", ""),
                                                });
                                            }
                                            SyncProdcutLibraryLog.Add($"修改,操作人:{userName},ErrorMessage:{updateResult.ErrorMessage},Exception:{updateResult.Exception},ErrorCode:{updateResult.ErrorCode},ProdcutId:{_compareData.ProdcutId},Result:{updateResult.Result},Success:{updateResult.Success},操作时间:{DateTime.Now.ToString()}");
                                            #endregion
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            SyncProdcutLibraryLog.Add($"遍历品牌: {model.Brands} 数据异常，请打开重试！,操作时间:{DateTime.Now.ToString()}");
                        }
                    }
                    #endregion

                    #region 拼接条件
                    if (sqlInsertWhere != null && sqlInsertWhere.Any())
                        dicSQL.Add("INSERT", sqlInsert + string.Join(",", sqlInsertWhere));

                    if (sqlUpdateWhere != null && sqlUpdateWhere.Any())
                    {
                        string _ProdcutName = " CASE PId ", _Describe = " CASE PId ";

                        foreach (var item in sqlUpdateWhere)
                        {
                            _ProdcutName += string.Format("WHEN {0} THEN N'{1}'", item.PId, item.ProdcutName);
                            _Describe += string.Format("WHEN {0} THEN N'{1}'", item.PId, item.Describe);
                        }
                        _ProdcutName += " END ";
                        _Describe += " END ";

                        dicSQL.Add("UPDATE", string.Format(sqlUpdate, string.Join(",", sqlUpdateWhere.Select(s => s.PId)), _ProdcutName, _Describe));
                    }
                    #endregion
                }
                else
                {
                    SyncProdcutLibraryLog.Add($"入口: dataTreeItems = null , Brands = {model.Brands} ！,操作时间:{DateTime.Now.ToString()}");
                }

                CachingLogsHelp.AddOrGetExisting(SyncProdcutLibraryLog);
                return dicSQL;
            }
        }

        #region 门店产品列表
        public ActionResult ShopSaleItemForGrouponPage(int pageIndex = 1, int pageSize = 10)
        {
            var data = ShopSaleItemForGrouponBLL.SelectPages(pageIndex, pageSize);
            ViewBag.totalRecords = data?.FirstOrDefault().TotalCount ?? 0;
            ViewBag.totalPage = data?.FirstOrDefault().TotalPage(pageSize) ?? 0;
            ViewBag.pageIndex = pageIndex;
            return View(data);
        }
        public ActionResult ShopSaleItemForGrouponPageNew()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetShopSaleItemForGrouponPageData(GetShopSaleItemForGrouponPageData request)
        {
            int totalCount = 0;
            var result = ShopSaleItemForGrouponBLL.SelectPagesNew(
                request.PageIndex,
                request.PageSize,
                request.ShopName,
                request.Region?.ProvinceName,
                request.Region?.CityName,
                request.Region?.AreaName,
                request.ShopType,
                request.Category,
                request.ProName,
                request.Sales,
                request.IsActive,
                out totalCount
                );

            return Json(new { resultData = result, total = totalCount });
        }
        [HttpPost]
        public async Task<JsonResult> GetAllCitys(string provinceName)
        {
            try
            {
                using (var client = new RegionClient())
                {
                    var regions = await client.GetRegionByRegionNameAsync(provinceName);
                    regions.ThrowIfException(true);
                    if (regions.Result.ChildRegions.FirstOrDefault().IsBelongMunicipality)
                        return Json(regions.Result.ChildRegions.Select(s => new { name = s.DistrictName }).ToArray());
                    else
                    {
                        return Json(regions.Result.ChildRegions.Select(s => new { name = s.CityName }).ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(null);
        }
        [HttpPost]
        public async Task<JsonResult> GetAllAreas(string cityName)
        {
            try
            {
                using (var client = new RegionClient())
                {
                    var regions = await client.GetRegionByRegionNameAsync(cityName);
                    regions.ThrowIfException(true);
                    if (regions.Result.ChildRegions.FirstOrDefault().IsBelongMunicipality)
                        return Json(regions.Result.ChildRegions.Select(s => new { name = s.DistrictName }).ToArray());
                    else
                    {
                        return Json(regions.Result.ChildRegions.Select(s => new { name = s.DistrictName }).ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(null);
        }
        #endregion

        public static class CachingLogsHelp
        {
            private static Cache CacheClient = HttpRuntime.Cache;
            public static TValue AddOrGetExisting<TValue>(TValue value, string key = "SyncProdcutLibraryLog")
            {
                lock (SyncLock)
                {
                    CacheClient.Insert(key, value);
                    return (TValue)CacheClient.Get(key);
                }
            }

            public static TValue Get<TValue>(string key = "SyncProdcutLibraryLog")
            {
                lock (SyncLock)
                {
                    return (TValue)CacheClient.Get(key);
                }
            }

            public static void CacheItemRemove(string key = "SyncProdcutLibraryLog")
            {
                lock (SyncLock)
                {
                    CacheClient.Remove(key);
                }
            }
        }

        private class BatchTreeModel
        {
            /// <summary>
            /// 类目名
            /// </summary>
            public string CategorysName { get; set; }
            /// <summary>
            /// 品牌ID
            /// </summary>
            public int? ParentId { get; set; }
            /// <summary>
            /// 品牌名
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 品牌子系列
            /// </summary>
            public IEnumerable<BatchTreeChildsModel> Childs { get; set; }
        }

        private class BatchTreeChildsModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}