using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.CategoryTag
{
    public class CategoryTagHandler
    {
        private readonly IDBScopeManager dbManager;
        public CategoryTagHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }

        public IEnumerable<CategoryTagModel> SelectById(int? id, int? parentId)
        {
            Func<SqlConnection, IEnumerable<CategoryTagModel>> action = (connection) => DALCategoryTag.SelectById(connection, id, parentId);
            return dbManager.Execute(action);
        }

        public IEnumerable<ZTreeModel> SelectByTree()
        {
            Func<SqlConnection, IEnumerable<ZTreeModel>> action = (connection) => DALCategoryTag.SelectByTree(connection);
            return dbManager.Execute(action);
        }

        public bool Insert(CategoryTagModel model)
        {
            Func<SqlConnection, bool> action = (connection) => DALCategoryTag.Insert(connection, model);
            return dbManager.Execute(action);
        }

        public bool UpdateById(CategoryTagModel model)
        {
            Func<SqlConnection, bool> action = (connection) => DALCategoryTag.UpdateById(connection, model);
            return dbManager.Execute(action);
        }

        public bool InsertBatch(List<ArticleCategoryTagModel> models)
        {
            Func<SqlConnection, bool> action = (connection) => DALArticleCategoryTag.InsertBatch(connection, models);
            return dbManager.Execute(action);
        }

        public bool DeleteBatch(int articleId)
        {
            Func<SqlConnection, bool> action = (connection) => DALArticleCategoryTag.DeleteBatch(connection, articleId);
            return dbManager.Execute(action);
        }

        public bool UpdateBatch(int articleId, List<ArticleCategoryTagModel> models)
        {
            Func<SqlConnection, bool> action = (connection) => DALArticleCategoryTag.UpdateBatch(connection, articleId, models);
            return dbManager.Execute(action);
        }

        public IEnumerable<CategoryTagModel> SelectByPID(int parentId, int isParent, string orderBy)
        {
            Func<SqlConnection, IEnumerable<CategoryTagModel>> action = (connection) => DALCategoryTag.SelectByPID(connection, parentId, isParent, orderBy);
            return dbManager.Execute(action);
        }

        /// <summary>
        /// 检测顶级节点是否车型
        /// </summary>
        public bool IsVehicle(int id)
        {
            Func<SqlConnection, bool> action = (connection) => DALCategoryTag.IsVehicle(connection, id);
            return dbManager.Execute(action);
        }
    }
}