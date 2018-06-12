using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThBiz.Common.Configurations;
using Tuhu.Component.Framework;

using Tuhu.Nosql;
using Tuhu.Provisioning.Business.Power;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using EnOrDe = Tuhu.Component.Framework.EnOrDeHelper;

namespace Tuhu.Provisioning.Business.Setting
{
    public class IndexConfigManager
    {
        private  static IConnectionManager connectionManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString);
        private  static IDBScopeManager dbScopeManager = new DBScopeManager(connectionManager);
        private  static IConnectionManager readConnectionManager = new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration_AlwaysOnRead"].ConnectionString);
        private  static IDBScopeManager readDbScopeManager = new DBScopeManager(readConnectionManager);
        private static CacheClient client = CacheHelper.CreateCacheClient("Index");
        private const string keyStr = "search12031136";

        
 
        public static bool SetIndexModulesCache()
        {
            var cacheResult = client.Set("IndexConfig",GetAllIndexModules(), TimeSpan.FromDays(1));
            if (cacheResult.Success)
                return true;
            return false;
        }

        public static List<IndexModuleConfig> GetAllIndexModulesFormCache()
        {
            List<IndexModuleConfig> result = new List<IndexModuleConfig>();
            var cacheResult = client.GetOrSet("IndexConfig", GetAllIndexModules, TimeSpan.FromDays(1));
            if (cacheResult.Success)
            {
                result = cacheResult.Value;
            }
            return result;
        }

        public static List<IndexModuleConfig> GetAllIndexModules()
        {
            List<IndexModuleConfig> result = new List<IndexModuleConfig>();

            result = readDbScopeManager.Execute(conn => DalIndexModule.SelectAllIndexModuleConfigs(conn));

            if(result!= null && result.Any())
            {
                foreach(var module in result)
                {
                    var items = readDbScopeManager.Execute(conn => DalIndexModule.SelectAllIndexModuleItems(conn, module.PKID));
                    module.Items = items;
                }
            }
            return result;
        }

        public static List<IndexModuleConfig> GetIndexModules()
        {
            List<IndexModuleConfig> result = new List<IndexModuleConfig>();
            result = readDbScopeManager.Execute(conn => DalIndexModule.SelectAllIndexModuleConfigs(conn));
            return result;
        }

        public static List<IndexModuleItem> GetIndexItems(int moduleId)
        {
            List<IndexModuleItem> items = new List<IndexModuleItem>();
            items = readDbScopeManager.Execute(conn => DalIndexModule.SelectAllIndexModuleItems(conn, moduleId));
            return items;
        }

        public static bool UpdateIndexModuleName(int moduleId, string name)
        {
            var result = dbScopeManager.Execute(conn => DalIndexModule.UpdateModuleName(conn, moduleId, name));
            return result;
        }

        public static bool UpdateIndexModuleIndex(List<string> moduleIds)
        {
            var index = 1;
            foreach(var moduleId in moduleIds)
            {
                var result = dbScopeManager.Execute(conn => DalIndexModule.UpdateModuleIndex(conn, int.Parse(moduleId), index++));
                if (!result)
                    return false;
            }
            return true;
        }

        public static bool DeleteIndexModule(int moduleId)
        {
            var result = dbScopeManager.Execute(conn => DalIndexModule.DeleteModule(conn, moduleId));
            if(result)
                result = dbScopeManager.Execute(conn => DalIndexModule.DeleteModuleItemsByModuleId(conn, moduleId));
            return result;
        }

        public static bool CreateIndexModule(string moduleName)
        {
            if(readDbScopeManager.Execute(conn => DalIndexModule.IsExistModule(conn, moduleName)))
            {
                return false;
            }
            var maxIndex = readDbScopeManager.Execute(conn => DalIndexModule.GetMaxModuleIndex(conn));
            var result = dbScopeManager.Execute(conn => DalIndexModule.CreateModule(conn, moduleName, maxIndex + 1));
            return result;
        }

        public static bool UpdateIndexModuleItemIndex(List<string> items)
        {
            var displayIndex = 1;
            foreach(var item in items)
            {
                if(!dbScopeManager.Execute(conn => DalIndexModule.UpdateModuleItemIndex(conn, int.Parse(item), displayIndex++)))
                    return false;
            }
            return true;
        }

        public static bool UpdateIndexModuleItem(int moduleItenId, string entryName, string controller, string action)
        {
            var result = dbScopeManager.Execute(conn => DalIndexModule.UpdateModuleEntry(conn, moduleItenId, entryName, controller, action));
            return result;
        }

        public static bool DeleteIndexModuleItem(int moduleItemId)
        {
            var result =  dbScopeManager.Execute(conn => DalIndexModule.DeleteModuleItem(conn, moduleItemId));
            return result;
        }

        public static bool CreateIndexModuleItem(int moduleId, string entryName, string controller, string action)
        {
            if(readDbScopeManager.Execute(conn => DalIndexModule.IsExistModuleItem(conn, moduleId, entryName, controller, action)))
            {
                return false;
            }
            var maxIndex = readDbScopeManager.Execute(conn => DalIndexModule.GetMaxModuleItemIndex(conn, moduleId));
            var result = dbScopeManager.Execute(conn => DalIndexModule.CreateModuleItem(conn, moduleId, entryName, controller, action, maxIndex + 1));
            return result;
        }
    }
}
