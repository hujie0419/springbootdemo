using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO.GroupBuyingV2Dao;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.GroupBuyingV2
{
    internal class GroupBuyCategoryConfigHandler
    {
        private readonly IDBScopeManager dbManager;

        public GroupBuyCategoryConfigHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }

        public IEnumerable<OperationCategoryModel> Select(int? id, int? parentId, int? type)
        {
            Func<SqlConnection, IEnumerable<OperationCategoryModel>> action = (connection) => DalGroupBuyCategoryConfig.Select(connection, id, parentId, type);
            return dbManager.Execute(action);
        }

        public bool Insert(OperationCategoryModel model)
        {
            Func<SqlConnection, bool> action = (connection) => DalGroupBuyCategoryConfig.Insert(connection, model);
            return dbManager.Execute(action);
        }

        public bool Update(OperationCategoryModel model)
        {
            Func<SqlConnection, bool> action = (connection) => DalGroupBuyCategoryConfig.Update(connection, model);
            return dbManager.Execute(action);
        }

        public bool Delete(int id)
        {
            Func<SqlConnection, bool> action = (connection) => DalGroupBuyCategoryConfig.Delete(connection, id);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 获取关联产品
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public IEnumerable<OperationCategoryProductsModel> SelectOperationCategoryProducts(int oid)
        {
            Func<SqlConnection, IEnumerable<OperationCategoryProductsModel>> action = (connection) => DalGroupBuyCategoryConfig.SelectOperationCategoryProducts(connection, oid);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 更新关联产品关系
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="type"></param>
        /// <param name="lists"></param>
        /// <returns></returns>
        public bool UpdateOperationCategoryProducts(int oid, int type, List<OperationCategoryProductsModel> lists)
        {
            Func<SqlConnection, bool> action = (connection) => DalGroupBuyCategoryConfig.UpdateOperationCategoryProducts(connection, oid, type, lists);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 获取后台类目集合
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ZTreeModel> SelectProductCategories()
        {
            Func<SqlConnection, IEnumerable<ZTreeModel>> action = (connection) => DalGroupBuyCategoryConfig.SelectProductCategories(connection);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 通过oid查询后台类目
        /// </summary>
        /// <param name="oids"></param>
        /// <returns></returns>
        public IEnumerable<VW_ProductCategoriesModel> SelectProductCategoriesForOid(IEnumerable<int> oids)
        {
            Func<SqlConnection, IEnumerable<VW_ProductCategoriesModel>> action = (connection) => DalGroupBuyCategoryConfig.SelectProductCategoriesForOid(connection, oids);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 查询产品信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<VW_ProductsModel> SelectProductCategories(IEnumerable<string> tags, bool isOid = false)
        {
            Func<SqlConnection, IEnumerable<VW_ProductsModel>> action = (connection) => DalGroupBuyCategoryConfig.SelectProductCategories(connection, tags, isOid);
            return dbManager.Execute(action);
        }
    }
}