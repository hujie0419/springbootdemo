using System;
using System.Collections.Generic;
using Common.Logging;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.ProductLimit
{
    public class ProductLimitManager
    {
        private static readonly ILog logger = LogManager.GetLogger<ProductLimitManager>();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public ProductLimitCountEntity GetModelByName(string category, int level)
        {
            try
            {
                return DALProductLimit.GetModelByName(category, level);
            }
            catch (Exception ex)
            {
                logger.Error("GetLimitCountByCategoryName", ex);
                return new ProductLimitCountEntity();
            }
        }

        /// <summary>
        /// 获取单个类目的限购信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ProductLimitCountEntity FetchCategoryLimitCount(ProductLimitCountEntity model)
        {
            try
            {
                return DALProductLimit.FetchCategoryLimitCount(model);
            }
            catch (Exception ex)
            {
                logger.Error("FetchCategoryLimitCount", ex);
                return new ProductLimitCountEntity();
            }
        }



        /// <summary>
        /// 获取单个类目的限购信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ProductLimitCountEntity GetCategoryLimitCount(ProductLimitCountEntity model)
        {
            try
            {
                return DALProductLimit.GetCategoryLimitCount(model);
            }
            catch (Exception ex)
            {
                logger.Error("FetchCategoryLimitCount", ex);
                return new ProductLimitCountEntity();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ProductLimitCountEntity FetchProductLimitCount(ProductLimitCountEntity model)
        {
            try
            {
                return DALProductLimit.FetchProductLimitCount(model);
            }
            catch (Exception ex)
            {
                logger.Error("FetchProductLimitCount", ex);
                return new ProductLimitCountEntity();
            }
        }

        /// <summary>
        /// 插入限购数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int InsertProductLimitCount(ProductLimitCountEntity model)
        {
            try
            {
                return DALProductLimit.InsertProductLimitCount(model);
            }
            catch (Exception ex)
            {
                logger.Error("InsertProductLimitCount", ex);
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateProductLimitCount(ProductLimitCountEntity model)
        {
            try
            {
                return DALProductLimit.UpdateProductLimitCount(model);
            }
            catch (Exception ex)
            {
                logger.Error("UpdateProductLimitCount", ex);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sqlWhere"></param>
        /// <returns></returns>
        public List<ProductLimitCountEntity> GetProductListByCategory(string category, int pageIndex, int pageSize, string sqlWhere, out int total)
        {
            try
            {
                return DALProductLimit.GetProductListByCategory(category, pageIndex, pageSize, sqlWhere, out total);
            }
            catch (Exception ex)
            {
                total = 0;
                logger.Error("GetProductListByCategory", ex);
                return new List<ProductLimitCountEntity>();
            }
        }
    }
}
