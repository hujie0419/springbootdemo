using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using System.Data.SqlClient;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.DAO;

namespace Tuhu.Provisioning.Business.OperationCategory
{
    public class OperationCategoryHandler
    {
        private readonly IDBScopeManager dbManager;
        public OperationCategoryHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }

        public IEnumerable<OperationCategoryModel> Select(int? id, int? parentId, int? type)
        {
            Func<SqlConnection, IEnumerable<OperationCategoryModel>> action = (connection) => OperationCategoryDAL.Select(connection, id, parentId, type);
            return dbManager.Execute(action);
        }

        public bool Insert(OperationCategoryModel model)
        {
            Func<SqlConnection, bool> action = (connection) => OperationCategoryDAL.Insert(connection, model);
            return dbManager.Execute(action);
        }

        public bool Update(OperationCategoryModel model)
        {
            Func<SqlConnection, bool> action = (connection) => OperationCategoryDAL.Update(connection, model);
            return dbManager.Execute(action);
        }

        public bool Delete(int id)
        {
            Func<SqlConnection, bool> action = (connection) => OperationCategoryDAL.Delete(connection, id);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 获取关联产品
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        public IEnumerable<OperationCategoryProductsModel> SelectOperationCategoryProducts(int oid)
        {
            Func<SqlConnection, IEnumerable<OperationCategoryProductsModel>> action = (connection) => OperationCategoryDAL.SelectOperationCategoryProducts(connection, oid);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 更新关联产品关系
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="oid"></param>
        /// <param name="type"></param>
        /// <param name="lists"></param>
        /// <returns></returns>
        public bool UpdateOperationCategoryProducts(int oid, int type, List<OperationCategoryProductsModel> lists)
        {
            Func<SqlConnection, bool> action = (connection) => OperationCategoryDAL.UpdateOperationCategoryProducts(connection, oid, type, lists);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 获取后台类目集合
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ZTreeModel> SelectProductCategories()
        {
            Func<SqlConnection, IEnumerable<ZTreeModel>> action = (connection) => OperationCategoryDAL.SelectProductCategories(connection);
            return dbManager.Execute(action);
        }


        /// <summary>
        /// 通过oid查询后台类目
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        public IEnumerable<VW_ProductCategoriesModel> SelectProductCategoriesForOid(IEnumerable<int> oids)
        {
            Func<SqlConnection, IEnumerable<VW_ProductCategoriesModel>> action = (connection) => OperationCategoryDAL.SelectProductCategoriesForOid(connection, oids);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 查询产品信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<VW_ProductsModel> SelectProductCategories(IEnumerable<string> tags, bool isOid = false)
        {
            Func<SqlConnection, IEnumerable<VW_ProductsModel>> action = (connection) => OperationCategoryDAL.SelectProductCategories(connection, tags, isOid);
            return dbManager.Execute(action);
        }
    }
}