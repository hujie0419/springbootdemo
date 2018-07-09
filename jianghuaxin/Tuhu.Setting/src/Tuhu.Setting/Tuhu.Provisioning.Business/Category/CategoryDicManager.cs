using System;
using System.Collections.Generic;
using System.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.Category
{
    public class CategoryDicManager
    {
        #region Private Fields

        private static readonly IConnectionManager ConnectionManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(ConnectionManager);

        private readonly CategoryDicHandler handler;

        private static readonly ILog Logger = LoggerFactory.GetLogger("CategoryDic");

        #endregion

        public CategoryDicManager()
        {
            handler = new CategoryDicHandler(DbScopeManager, ConnectionManager);
        }


        public bool InsertCategory(CategoryDic model)
        {
            try
            {
                return handler.InsertCategory(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new CategoryDicException(1, "InsertCategory", ex);
                Logger.Log(Level.Error, "InsertCategory", exception);
                throw;
            }

        }

        public List<CategoryDic> GetCategoryDic()
        {
            try
            {
                return handler.GetCategoryDic();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new CategoryDicException(1, "GetCategoryDic", ex);
                Logger.Log(Level.Error, "GetCategoryDic", exception);
                throw;
            }

        }

        public List<NewAppData> GetNewAppData()
        {
            try
            {
                return handler.GetNewAppData();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new CategoryDicException(1, "GetNewAppData", ex);
                Logger.Log(Level.Error, "GetNewAppData", exception);
                throw;
            }

        }
        public bool InsertNewAppData(NewAppData model)
        {
            try
            {
                return handler.InsertNewAppData(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new CategoryDicException(1, "InsertNewAppData", ex);
                Logger.Log(Level.Error, "InsertNewAppData", exception);
                throw;
            }

        }

        public int GetMaxModelFloor(NewAppData model)
        {
            try
            {
                return handler.GetMaxModelFloor(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new CategoryDicException(1, "GetMaxModelFloor", ex);
                Logger.Log(Level.Error, "GetMaxModelFloor", exception);
                throw;
            }

        }
        public List<CategoryDic> GetCategoryIdName()
        {
            try
            {
                return handler.GetCategoryIdName();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new CategoryDicException(1, "GetCategoryIdName", ex);
                Logger.Log(Level.Error, "GetCategoryIdName", exception);
                throw;
            }

        }

    }
}
