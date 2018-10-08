using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;


namespace Tuhu.Provisioning.Business.ProductLibraryConfing
{
    public class ProductLibraryConfigHandler
    {
        private readonly IDBScopeManager dbManager;
        public ProductLibraryConfigHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(ProductLibraryConfigModel model)
        {
            Func<SqlConnection, bool> action = (connection) => ProductLibraryConfigDAL.Add(connection, model);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(ProductLibraryConfigModel model)
        {
            Func<SqlConnection, bool> action = (connection) => ProductLibraryConfigDAL.Update(connection, model);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool BatchDelete(string Idlist)
        {
            Func<SqlConnection, bool> action = (connection) => ProductLibraryConfigDAL.BatchDelete(connection, Idlist);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public IEnumerable<ProductLibraryConfigModel> GetList(string strWhere)
        {
            Func<SqlConnection, IEnumerable<ProductLibraryConfigModel>> action = (connection) => ProductLibraryConfigDAL.GetList(connection, strWhere);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 根据oids获取产品优惠券配置
        /// oids ，号隔开
        /// </summary>
        public IEnumerable<ProductLibraryConfigModel> GetProductCouponConfigByOids(List<int> oids)
        {
            Func<SqlConnection, IEnumerable<ProductLibraryConfigModel>> action = 
                (connection) => ProductLibraryConfigDAL.GetProductCouponConfigByOids(connection, oids);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 根据查询条件获取产品库优惠券数据列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IEnumerable<QueryProductsModel> QueryProductsForProductLibrary(SeachProducts model)
        {
            Func<SqlConnection, IEnumerable<QueryProductsModel>> action = (connection)
                => ProductLibraryConfigDAL.QueryProductsForProductLibrary(connection, model);
            return dbManager.Execute(action);
        }
        /// <summary>
        /// 根据查询条件获取产品总行数
        /// </summary>
        /// <param name="sqlcon"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public  int QueryProductsForProductLibraryCount(SeachProducts model)
        {
            Func<SqlConnection, int> action = (connection)
              => ProductLibraryConfigDAL.QueryProductsForProductLibraryCount(connection, model);
            return dbManager.Execute(action);
        }

        public ProductSalesPredic QueryProductSalesInfoByPID(string pid)
        {
            Func<SqlConnection, ProductSalesPredic> action = (connection)
               => ProductLibraryConfigDAL.QueryProductSalesInfoByPID(connection, pid);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 获取产品信息
        /// </summary>
        public QueryProductsModel QueryProduct(int oid) {
            Func<SqlConnection, QueryProductsModel> action = (connection) => ProductLibraryConfigDAL.QueryProduct(connection, oid);
            return dbManager.Execute(action);
        }

        public IEnumerable<QueryProductsModel> QueryProducts(List<int> oids)
        {
            Func<SqlConnection, IEnumerable<QueryProductsModel>> action = (connection) =>
                ProductLibraryConfigDAL.QueryProducts(connection, oids);
            return dbManager.Execute(action);
        }
        public List<string> GetPattern()
        {
            Func<SqlConnection, List<string>> action = (connection) => ProductLibraryConfigDAL.GetPattern(connection);
            return dbManager.Execute(action);
        }
        /// <summary>
        /// 批量修改或添加产品
        /// </summary>
        public MsgResultModel BatchAddCoupon(string oids, string coupons, int isBatch)
        {
            Func<SqlConnection, MsgResultModel> action = (connection) => ProductLibraryConfigDAL.BatchAddCoupon(connection, oids, coupons, isBatch);
            return dbManager.Execute(action);
        }

        public bool UpdateProductCouponConfig(int oid, List<int> couponPkIds)
        {
            Func<SqlConnection, bool> action = (connection) => 
            ProductLibraryConfigDAL.UpdateProductCouponConfig(connection, oid, couponPkIds);
            return dbManager.Execute(action);
        }
        /// <summary>
        /// 批量修改或添加产品
        /// </summary>
        public MsgResultModel BatchDeleteCoupon(string oids, string coupon)
        {
            Func<SqlConnection, MsgResultModel> action = (connection) => ProductLibraryConfigDAL.BatchDeleteCoupon(connection, oids, coupon);
            return dbManager.Execute(action);
        }
        ///// <summary>
        ///// 获取 品牌，标签，尺寸 
        ///// </summary>
        //public System.Data.DataSet GetFilterCondition(string category)
        //{
        //    Func<SqlConnection, System.Data.DataSet> action = (connection) => ProductLibraryConfigDAL.GetFilterCondition(connection, category);
        //    return dbManager.Execute(action);
        //}
    }
}
