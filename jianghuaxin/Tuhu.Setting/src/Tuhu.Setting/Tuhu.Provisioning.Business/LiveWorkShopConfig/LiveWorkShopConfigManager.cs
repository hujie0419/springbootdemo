using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.LiveWorkShopConfig
{
    public class LiveWorkShopConfigManager
    {
        private static readonly Common.Logging.ILog Logger;
        private static readonly IConnectionManager configConnRo =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration"].ConnectionString);
        private static readonly IConnectionManager configReadConnRo =
      new ConnectionManager(ConfigurationManager.ConnectionStrings["Configuration_AlwaysOnRead"].ConnectionString);
        private static readonly IDBScopeManager dbScopeManagerConfigRead;
        private static readonly IDBScopeManager dbScopeManagerConfig;

        static LiveWorkShopConfigManager()
        {
            Logger = Common.Logging.LogManager.GetLogger(typeof(LiveWorkShopConfigManager));
            dbScopeManagerConfig = new DBScopeManager(configConnRo);
            dbScopeManagerConfigRead = new DBScopeManager(configReadConnRo);
        }

        /// <summary>
        /// 插入透明工场配置记录
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public bool ImportLiveWorkShopConfig(List<LiveWorkShopConfigModel> models)
        {
            var result = false;
            try
            {
                var existsConfig = dbScopeManagerConfigRead.Execute(conn => DalLiveWorkShopConfig.GetExistLiveWorkShopConfig(conn));
                var delConfig = existsConfig.Where(s => !models.Any(m => string.Equals(m.TypeName, s.TypeName) && m.SortNumber == s.SortNumber)).Select(v => v.PKID).ToList();
                dbScopeManagerConfig.CreateTransaction(conn =>
                {
                    foreach (var model in models)
                    {
                        DalLiveWorkShopConfig.ImportLiveWorkShopConfig(conn, model);
                    }
                    foreach (var pkid in delConfig)
                    {
                        DalLiveWorkShopConfig.DeleteLiveWorkShopConfig(conn, pkid);
                    }
                });
                result = true;
            }
            catch (Exception ex)
            {
                Logger.Error("ImportLiveWorkShopConfig", ex);
            }
            return result;
        }

        /// <summary>
        /// 透明工场配置展示
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Tuple<List<LiveWorkShopConfigModel>, int> SelectLiveWorkShopConfig(string typeName, int pageIndex, int pageSize)
        {
            int totalCount = 0;
            List<LiveWorkShopConfigModel> result = null;
            try
            {
                result = dbScopeManagerConfigRead.Execute(conn =>
                     DalLiveWorkShopConfig.SelectLiveWorkShopConfig(conn, typeName, pageIndex, pageSize, out totalCount));
            }
            catch (Exception ex)
            {
                Logger.Error("SelectLiveWorkShopConfigModel", ex);
            }
            return new Tuple<List<LiveWorkShopConfigModel>, int>(result, totalCount);
        }

        /// <summary>
        /// 获取透明工场配置类型
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllLiveWorkShopConfigType()
        {
            List<string> result = null;
            try
            {
                result = dbScopeManagerConfigRead.Execute(conn => DalLiveWorkShopConfig.GetAllLiveWorkShopConfigType(conn));
            }
            catch (Exception ex)
            {
                Logger.Error("GetAllLiveWorkShopConfigType", ex);
            }
            return result ?? new List<string>();
        }

        #region 清除服务缓存
        /// <summary>
        /// 刷新透明工场配置服务缓存
        /// </summary>
        /// <returns></returns>
        public bool RefreshLiveWorkShopConfigCache()
        {
            var result = false;
            try
            {
                using (var client = new Tuhu.Service.Config.CacheClient())
                {
                    var cacheResult = client.RefreshLiveWorkShopConfigCache();
                    cacheResult.ThrowIfException(true);
                    result = cacheResult.Result;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("RefreshLiveWorkShopConfigCache", ex);
            }
            return result;
        }
        #endregion
    }
}
