using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.Identity;
using Tuhu.Nosql;
using Tuhu.Provisioning.Business;
using Tuhu.Provisioning.Business.Monitor;
using Tuhu.Provisioning.DataAccess;
using MonitorLevel = Tuhu.Service.Order.Monitor.MonitorLevel;
using MonitorModule = Tuhu.Service.Order.Monitor.MonitorModule;

namespace Tuhu.Provisioning.Business
{
    public class RegionManager 
    {
        #region  Private Fields
        private static readonly IConnectionManager connectionManager =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString.Decrypt());

        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);

        static string readOnlyStrConn = ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString;
        private static readonly IConnectionManager readOnlyConnectionManager =  new ConnectionManager(readOnlyStrConn.Decrypt());
        private static readonly IDBScopeManager readOnlyDbScopeManager = new DBScopeManager(readOnlyConnectionManager);
        #endregion

        #region Ctor

        public RegionManager()
        {
        }

        #endregion
        /// <summary>
        /// 获得所有省
        /// </summary>
        /// <returns></returns>
        public List<Region> SelectAllProvince(int parentId)
        {
            return readOnlyDbScopeManager.Execute(connection => DalRegion.SelectAllProvince(connection, parentId));
        }
        /// <summary>
        /// 获得region表里面所有的数据
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public List<Region> SelectAllRegions()
        {
            var key = EnOrDeHelper.GetMd5("Yewu/Shop/YeWuAllRegions", Encoding.UTF8);
            List<Region> resultList;
            using (CacheClient client = CacheHelper.CreateCacheClient("Cache_GetRegions"))
            {
                resultList = client.GetOrSet<List<Region>>(key, () => {
                    return GetRegionAllData();
                }).Value;
            }
            return resultList;
        }
        private List<Region> GetRegionAllData()
        {
            var result = readOnlyDbScopeManager.Execute(connection => DalRegion.SelectAllRegions(connection));
            return result;
        }
        
    }
}
