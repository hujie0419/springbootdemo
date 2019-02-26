using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO.ThirdParty;
using Tuhu.Provisioning.DataAccess.Entity.ThirdParty;

namespace Tuhu.Provisioning.Business.ThirdParty
{
    public class ThirdPartyManager
    {
        private static readonly IConnectionManager ConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["ThirdParty"].ConnectionString);
        private static readonly IConnectionManager ConnectionReadManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["ThirdPartyReadOnly"].ConnectionString);

        private readonly IDBScopeManager dbScopeManager = null;
        private readonly IDBScopeManager dbScopeReadManager = null;

        private static readonly Common.Logging.ILog Logger = LogManager.GetLogger(typeof(ThirdPartyManager));

        public ThirdPartyManager()
        {
            this.dbScopeManager = new DBScopeManager(ConnectionManager);
            this.dbScopeReadManager = new DBScopeManager(ConnectionReadManager);
        }
        /// <summary>
        /// 获取三方码配置信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<ServiceCodeSourceConfig> GetServiceCodeSourceConfig(int pageIndex, int pageSize)
        {
            List<ServiceCodeSourceConfig> result = null;
            try
            {
                result = dbScopeReadManager.Execute(conn => DALThirdParty.GetServiceCodeSourceConfig(conn, pageIndex, pageSize));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// 编辑三方码信息
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public Tuple<bool, string> UpsertCodeSourceConfig(ServiceCodeSourceConfig config)
        {
            var result = false;
            var msg = string.Empty;
            try
            {
                dbScopeManager.CreateTransaction(conn =>
                {
                    if (config.PKID > 0)
                    {
                        result = DALThirdParty.UpdateServiceCodeSourceConfig(conn, config);
                    }
                    else
                    {
                        var allSourceConfigs = DALThirdParty.GetServiceCodeSourceConfigBySource(conn, config.Source);
                        if (allSourceConfigs != null && allSourceConfigs.Any())
                        {
                            msg = "服务码来源重复";
                        }
                        else
                        {
                            result = DALThirdParty.InsertServiceCodeSourceConfig(conn, config);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                msg = "服务异常";
                Logger.Error(ex);
            }
            return Tuple.Create(result, msg);
        }
    }
}
