using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.CategoryTag
{
    public class CategoryTagManager
    {
        #region Private Fields
        static string strConn = ConfigurationManager.ConnectionStrings["Aliyun"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);
        private static readonly ILog logger = LoggerFactory.GetLogger("CategoryTagManager");
        private CategoryTagHandler handler = null;
        #endregion

        public CategoryTagManager()
        {
            handler = new CategoryTagHandler(DbScopeManager);
        }

        public IEnumerable<CategoryTagModel> SelectById(int? id = null, int? parentId = null)
        {
            return handler.SelectById(id, parentId);
        }

        public IEnumerable<ZTreeModel> SelectByTree()
        {
            return handler.SelectByTree();
        }

        public bool Insert(CategoryTagModel model)
        {
            return handler.Insert(model);
        }

        public bool UpdateById(CategoryTagModel model)
        {
            return handler.UpdateById(model);
        }

        public bool InsertBatch(List<ArticleCategoryTagModel> models)
        {
            return handler.InsertBatch(models);
        }

        public bool DeleteBatch(int articleId)
        {
            return handler.DeleteBatch(articleId);
        }

        public bool UpdateBatch(int articleId, List<ArticleCategoryTagModel> models)
        {
            return handler.UpdateBatch(articleId, models);
        }


        public IEnumerable<CategoryTagModel> SelectByPID(int parentId, int isParent, string orderBy)
        {
            try
            {
                return handler.SelectByPID(parentId, isParent, orderBy);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 检测顶级节点是否车型
        /// </summary>
        public bool IsVehicle(int id)
        {
            try
            {
                return handler.IsVehicle(id);
            }
            catch (Exception ex)
            {
                logger.Log(Level.Error, ex.ToString());
                return false;
            }
        }
    }
}