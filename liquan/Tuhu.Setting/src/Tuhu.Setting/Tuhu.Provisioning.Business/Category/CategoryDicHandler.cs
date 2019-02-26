using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.Category
{
    public class CategoryDicHandler
    {
        private readonly IConnectionManager connectionManager;
        private readonly IDBScopeManager dbManager;

        public CategoryDicHandler(IDBScopeManager dbManager, IConnectionManager connectionManager)
        {
            this.connectionManager = connectionManager;
            this.dbManager = dbManager;
        }


        public bool InsertCategory(CategoryDic model)
        {
            return dbManager.Execute(conn => DALCategory.InsertCategory(conn, model));
        }

        public List<CategoryDic> GetCategoryDic()
        {
            return dbManager.Execute(conn => DALCategory.GetCategoryDic(conn));
        }

        public List<NewAppData> GetNewAppData()
        {
            return dbManager.Execute(conn => DALCategory.GetNewAppData(conn));
        }
        public bool InsertNewAppData(NewAppData model)
        {
            return dbManager.Execute(conn => DALCategory.InsertNewAppData(conn, model));
        }

        public int GetMaxModelFloor(NewAppData model)
        {

            return dbManager.Execute(conn => DALCategory.GetMaxModelFloor(conn, model));
        }

        public List<CategoryDic> GetCategoryIdName()
        {
            return dbManager.Execute(conn => DALCategory.GetCategoryIdName(conn));

        }
    }
}
