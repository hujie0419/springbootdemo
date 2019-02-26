using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Tuhu.Component.Common;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.Shipping;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.ConfigCategory
{
    public class ConfigCategoryManager
    {
        private static readonly IConnectionManager connectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IConnectionManager grAlwaysOnReadConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);

        private readonly IDBScopeManager dbScopeManagerBY = null;
        private readonly IDBScopeManager GRAlwaysOnReadDbScopeManager = null;

        public ConfigCategoryManager()
        {
            dbScopeManagerBY = new DBScopeManager(connectionManager);
            GRAlwaysOnReadDbScopeManager = new DBScopeManager(grAlwaysOnReadConnectionManager);

            //using (var client = new CacheClient())
            //{
            //    client.UpdateBaoYangActivityAsync("activtiyId");
            //    client.UpdateTuhuRecommendConfigAsync();
            //}
        }

        /// <summary>
        /// 获取类目配置列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<GaiZhuangCategoryModel> SelectChangeCategoryList()
        {
            DataTable resulTable = DALChangeCategory.SelectChangeCategoryList();
            if (resulTable != null && resulTable.Rows.Count > 0)
            {
                return resulTable.Rows.Cast<DataRow>().Select(dr => new GaiZhuangCategoryModel(dr));
            }
            else
            {
                return new GaiZhuangCategoryModel[0];
            }
        }

        public static GaiZhuangCategoryModel GetChangeCategoryModelByPKID(int pkid)
        {
            var dr = DALChangeCategory.GetChangeCategoryModelByPKID(pkid);
            return new GaiZhuangCategoryModel(dr);
        }

        public static bool UpdateChangeCategoryModel(GaiZhuangCategoryModel model)
        {
            return DALChangeCategory.UpdateChangeCategoryModel(model);
        }


        /// <summary>
        /// 添加类目
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool InsertChangeCategoryModel(GaiZhuangCategoryModel model)
        {
            var result = DALChangeCategory.InsertChangeCategoryModel(model);
            return result;
        }


        /// <summary>
        /// 根据PKID删除类目
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeleteChangeCategoryModelByPkid(int pkid)
        {
            return DALChangeCategory.DeleteChangeCategoryModelByPkid(pkid);
        }

        /// <summary>
        /// 根据PKID删除已关联商品
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeleteRelateProductByPkid(int pkid)
        {
            return DALChangeCategory.DeleteRelateProductByPkid(pkid);
        }

        /// <summary>
        /// 根据PKID删除已关联商品
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeleteRelateProductByPkids(string pkids)
        {
            return DALChangeCategory.DeleteRelateProductByPkids(pkids);
        }

        /// <summary>
        /// 获取广告类目配置列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<AdvertCategoryModel> SelectAdvertCategoryList(string categoryid)
        {
            DataTable resulTable = DALChangeCategory.SelectAdvertCategoryList(categoryid);
            if (resulTable != null && resulTable.Rows.Count > 0)
            {
                return resulTable.Rows.Cast<DataRow>().Select(dr => new AdvertCategoryModel(dr));
            }
            else
            {
                return new AdvertCategoryModel[0];
            }
        }

        public static bool UpdateAdvertCategoryModel(AdvertCategoryModel model)
        {
            return DALChangeCategory.UpdateAdvertCategoryModel(model);
        }


        /// <summary>
        /// 添加广告类目
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool InsertAdvertCategoryModel(AdvertCategoryModel model)
        {
            var result = DALChangeCategory.InsertAdvertCategoryModel(model);
            return result;
        }

        /// <summary>
        /// 根据PKID获取广告类目配置
        /// </summary>
        /// <returns></returns>
        public static AdvertCategoryModel GetAdvertCategoryModelByPKID(int pkid)
        {
            var dr = DALChangeCategory.GetAdvertCategoryModelByPKID(pkid);

            return new AdvertCategoryModel(dr);
        }

        /// <summary>
        /// 根据PKID删除广告
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeleteAdvertCategoryModelByPkid(int pkid)
        {
            return DALChangeCategory.DeleteAdvertCategoryModelByPkid(pkid);
        }

        /// <summary>
        /// 根据PKID获取文章
        /// </summary>
        /// <returns></returns>
        public static GaiZhuangRelateArticleModel GetRelateArticleByPKID(int pkid)
        {
            var dr = DALChangeCategory.GetRelateArticleByPKID(pkid);

            return new GaiZhuangRelateArticleModel(dr);
        }

        /// <summary>
        /// 获取文章类目配置列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<GaiZhuangRelateArticleModel> SelectArticleCategoryList(string categoryid)
        {
            DataTable resulTable = DALChangeCategory.SelectArticleCategoryList(categoryid);
            if (resulTable != null && resulTable.Rows.Count > 0)
            {
                return resulTable.Rows.Cast<DataRow>().Select(dr => new GaiZhuangRelateArticleModel(dr));
            }
            else
            {
                return new GaiZhuangRelateArticleModel[0];
            }
        }

        /// <summary>
        /// 根据文章ID获取子类目文章具体信息列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<GaiZhuangRelateArticleModel> SelectArticleCategoryByArticleId(int articleId)
        {
            DataTable resulTable = DALChangeCategory.SelectArticleCategoryByArticleId(articleId);
            if (resulTable != null && resulTable.Rows.Count > 0)
            {
                return resulTable.Rows.Cast<DataRow>().Select(dr => new GaiZhuangRelateArticleModel(dr));
            }
            else
            {
                return new GaiZhuangRelateArticleModel[0];
            }
        }

        /// <summary>
        /// 根据PKID删除文章
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeleteArticleCategoryModelByPkid(int pkid)
        {
            return DALChangeCategory.DeleteArticleCategoryModelByPkid(pkid);
        }

        /// <summary>
        /// 添加文章
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool InsertArticleCategoryModel(GaiZhuangRelateArticleModel model)
        {
            var result = DALChangeCategory.InsertArticleCategoryModel(model);
            return result;
        }

        /// <summary>
        /// 添加文章返回主键
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int InsertArticleCategory(GaiZhuangRelateArticleModel model)
        {
            var result = DALChangeCategory.InsertArticleCategory(model);
            return result;
        }

        /// <summary>
        /// 批量添加文章匹配车型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool BulkSaveAriticleVehicle(DataTable dt)
        {
            var result = DALChangeCategory.BulkSaveAriticleVehicle(dt);
            return result;
        }

        public static bool UpdateArticleCategoryModel(GaiZhuangRelateArticleModel model)
        {
            return DALChangeCategory.UpdateArticleCategoryModel(model);
        }

        /// <summary>
        /// 获取文章匹配车型列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ArticleAdaptVehicleModel> SelectArticleVehicleCategoryList(int articleId)
        {
            DataTable resulTable = DALChangeCategory.SelectArticleVehicleCategoryList(articleId);
            if (resulTable != null && resulTable.Rows.Count > 0)
            {
                return resulTable.Rows.Cast<DataRow>().Select(dr => new ArticleAdaptVehicleModel(dr));
            }
            else
            {
                return new ArticleAdaptVehicleModel[0];
            }
        }

        /// <summary>
        /// 根据PKID删除文章
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static bool DeleteArticleVehicleCategoryModel(List<ArticleAdaptVehicleModel> list)
        {
            return DALChangeCategory.DeleteArticleVehicleCategoryModel(list);
        }

        public static GetPCodeModel GetPromotionModelByRuleID(string getGuid)
        {
            var dr = DALChangeCategory.GetPromotionModelByRuleID(getGuid);
            return new GetPCodeModel(dr);
        }

        /// <summary>
        /// 获取产品列表
        /// </summary>
        /// <returns></returns>
        public static List<RelateProductModel> GetRelateProductList(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALChangeCategory.GetRelateProductList(sqlStr, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据商品名称关键词获取关联商品
        /// </summary>
        /// <returns></returns>
        public static List<RelateProductModel> GetRelateProductListByProductKeyName(string productNameKey, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALChangeCategory.GetRelateProductListByProductKeyName(productNameKey, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据商品PID获取关联商品
        /// </summary>
        /// <returns></returns>
        public static List<RelateProductModel> GetRelateProductListByProductPID(string pid, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALChangeCategory.GetRelateProductListByProductPID(pid, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据选中的商品类目获取关联商品
        /// </summary>
        /// <returns></returns>
        public static List<RelateProductModel> GetRelateProductListByProductItems(string productItems, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALChangeCategory.GetRelateProductListByProductItems(productItems, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool UpdateArticleVehicleCategoryModel(GaiZhuangRelateArticleModel model)
        {
            return DALChangeCategory.UpdateArticleCategoryModel(model);
        }

        /// <summary>
        /// 根据子类目ID获取已关联的商品
        /// </summary>
        /// <returns></returns>
        public static List<GaiZhuangRelateProductModel> GetSelectedRelateProductListByCategoryID(int categoryid)
        {
            try
            {
                return DALChangeCategory.GetSelectedRelateProductListByCategoryID(categoryid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 批量添加子类目关联商品
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool BulkSaveRelateProduct(DataTable dt)
        {
            var result = DALChangeCategory.BulkSaveRelateProduct(dt);
            return result;
        }

        public static List<CategoryModel> GetProductCategoryList()
        {
            try
            {
                return DALChangeCategory.GetProductCategoryList();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取已选产品列表
        /// </summary>
        /// <returns></returns>
        public static List<SelectedRelateProductModel> GetSelectedRelateProductList(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALChangeCategory.GetSelectedRelateProductList(sqlStr, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据商品名称关键词获取已选关联商品
        /// </summary>
        /// <returns></returns>
        public static List<SelectedRelateProductModel> GetSelectedRelateProductListByProductKeyName(string productNameKey, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALChangeCategory.GetSelectedRelateProductListByProductKeyName(productNameKey, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据商品PID获取已选关联商品
        /// </summary>
        /// <returns></returns>
        public static List<SelectedRelateProductModel> GetSelectedRelateProductListByProductPID(string pid, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALChangeCategory.GetSelectedRelateProductListByProductPID(pid, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据选中的商品类目获取已选商品
        /// </summary>
        /// <returns></returns>
        public static List<SelectedRelateProductModel> GetSelectedRelateProductListByProductItems(string productItems, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALChangeCategory.GetSelectedRelateProductListByProductItems(productItems, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据选中的类目获取已选商品
        /// </summary>
        /// <returns></returns>
        public static List<SelectedRelateProductModel> GetSelectedRelateProductListByCategoryId(int categoryId, string[] brands)
        {
            try
            {
                var list = DALChangeCategory.GetSelectedRelateProductListByCategoryId(categoryId, brands);
                var dic = list.GroupBy(r => r.PKID).ToDictionary(r => r.Key, r => r.ToList());
                var list2 = new List<SelectedRelateProductModel>();
                foreach (var value in dic.Values)
                {
                    var model = new SelectedRelateProductModel()
                    {
                        PKID = value.FirstOrDefault().PKID,
                        CategoryId = value.FirstOrDefault().CategoryId,
                        PID = value.FirstOrDefault().PID,
                        ProductName = value.FirstOrDefault().ProductName,
                        ProductType = value.FirstOrDefault().ProductType,
                        OnSale = value.FirstOrDefault().OnSale,
                        CityType = value.FirstOrDefault().CityType,
                        Brand = value.FirstOrDefault().Brand,
                        Sort = value.FirstOrDefault().Sort,
                        RegionIds = new List<int>()
                    };
                    foreach (var item in value)
                    {
                        if (item.RegionId != null)
                        {
                            model.RegionIds.Add(item.RegionId.GetValueOrDefault());
                        }

                    }
                    if (model.RegionIds.Any())
                    {
                        model.StrCityIDs = string.Join("|", model.RegionIds);
                        model.StrCityNames = ShippingManager.SelectCityNameByCityIDs(model.RegionIds);
                    }

                    list2.Add(model);
                }
                return list2;
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据商品名称关键词获取关联商品
        /// </summary>
        /// <returns></returns>
        public static List<RelateProductModel> GetRelateProductListByProductKeyNameNoPage(string productNameKey)
        {
            try
            {
                return DALChangeCategory.GetRelateProductListByProductKeyNameNoPage(productNameKey);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据商品PID获取关联商品
        /// </summary>
        /// <returns></returns>
        public static List<RelateProductModel> GetRelateProductListByProductPIDNoPage(string pid)
        {
            try
            {
                return DALChangeCategory.GetRelateProductListByProductPIDNoPage(pid);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据选中的商品类目获取关联商品
        /// </summary>
        /// <returns></returns>
        public static List<RelateProductModel> GetRelateProductListByProductItemsNoPage(string productItems)
        {
            try
            {
                return DALChangeCategory.GetRelateProductListByProductItemsNoPage(productItems);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 更新城市
        /// </summary>
        /// <returns></returns>
        public static int UpdateRegion(string fkid, List<int> regionids, int type)
        {
            try
            {
                using (var dbhelper = new SqlDbHelper(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString))
                {
                    var delresult = DALChangeCategory.DelRegionid(dbhelper, fkid);
                    foreach (var regionId in regionids)
                    {
                        foreach (var item in fkid.Split(','))
                        {
                            var insertResult = DALChangeCategory.InsertCityId(dbhelper, item, regionId, type);
                        }

                    }

                }
                return 1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据PKID查询单个关联商品ID
        /// </summary>
        /// <param name="pkid"></param>
        /// <returns></returns>
        public static SelectedRelateProductModel EditRelateProduct(int pkid)
        {
            var dr = DALChangeCategory.EditRelateProduct(pkid);
            return new SelectedRelateProductModel(dr);
        }

        /// <summary>
        /// 编辑关联商品排序字段
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public static bool UpdateRelateProduct(int pkid, string sort)
        {
            return DALChangeCategory.UpdateRelateProduct(pkid,sort);
        }

    }
}

