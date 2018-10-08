using System;
using System.Collections.Generic;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.GroupBuyingV2
{
    public class GroupBuyCategoryConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("GroupBuyCategoryConfigManager");
        private static readonly string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);
        private GroupBuyCategoryConfigHandler handler = null;

        public GroupBuyCategoryConfigManager()
        {
            handler = new GroupBuyCategoryConfigHandler(DbScopeManager);
        }

        public IEnumerable<OperationCategoryModel> Select(int? id = null, int? parentId = null, int? type = null)
        {
            try
            {
                return handler.Select(id, parentId, type);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, string.Format("方法:{0},错误:{1}", "GroupBuyCategoryConfigManager.Select", ex.Message), id, parentId, type);
                return null;
            }
        }

        public bool Insert(OperationCategoryModel model)
        {
            try
            {
                return handler.Insert(model);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, string.Format("方法:{0},错误:{1}", "GroupBuyCategoryConfigManager.Insert", ex.Message), model);
                return false;
            }
        }

        public bool Update(OperationCategoryModel model)
        {
            try
            {
                return handler.Update(model);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, string.Format("方法:{0},错误:{1}", "GroupBuyCategoryConfigManager.Update", ex.Message), model);
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                return handler.Delete(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, string.Format("方法:{0},错误:{1}", "GroupBuyCategoryConfigManager.Delete", ex.Message), id);
                return false;
            }
        }

        /// <summary>
        /// 获取关联产品
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        public IEnumerable<OperationCategoryProductsModel> SelectOperationCategoryProducts(int oid)
        {
            try
            {
                return handler.SelectOperationCategoryProducts(oid);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, string.Format("方法:{0},错误:{1}", "GroupBuyCategoryConfigManager.SelectOperationCategoryProducts", ex.Message), oid);
                return null;
            }
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
            try
            {
                return handler.UpdateOperationCategoryProducts(oid, type, lists);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, string.Format("方法:{0},错误:{1}", "GroupBuyCategoryConfigManager.UpdateOperationCategoryProducts", ex.Message), oid, type, lists);
                return false;
            }
        }

        /// <summary>
        /// 获取后台类目集合
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ZTreeModel> SelectProductCategories()
        {
            try
            {
                return handler.SelectProductCategories();
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, string.Format("方法:{0},错误:{1}", "GroupBuyCategoryConfigManager.SelectProductCategories", ex.Message), "");
                return null;
            }
        }

        /// <summary>
        /// 通过oid查询后台类目
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        public IEnumerable<VW_ProductCategoriesModel> SelectProductCategoriesForOid(IEnumerable<int> oids)
        {
            try
            {
                return handler.SelectProductCategoriesForOid(oids);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, string.Format("方法:{0},错误:{1}", "GroupBuyCategoryConfigManager.SelectProductCategoriesForOid", ex.Message), oids);
                return null;
            }
        }

        /// <summary>
        /// 查询产品信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<VW_ProductsModel> SelectProductCategories(IEnumerable<string> tags, bool isOid = false)
        {
            try
            {
                return handler.SelectProductCategories(tags, isOid);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, string.Format("方法:{0},错误:{1}", "GroupBuyCategoryConfigManager.SelectProductCategories", ex.Message), tags, isOid);
                return null;
            }
        }
    }
}