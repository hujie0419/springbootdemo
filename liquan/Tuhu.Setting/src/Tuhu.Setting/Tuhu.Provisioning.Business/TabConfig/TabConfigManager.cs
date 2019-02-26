using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.CommonServices;
using Tuhu.Provisioning.DataAccess.DAO.TagConfig;
using Tuhu.Provisioning.DataAccess.Entity.TagConfig;

namespace Tuhu.Provisioning.Business.TagConfig
{
    public class TagConfigManager
    {
        private static readonly IConnectionManager ConnectionManager = new
            ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ToString());
        private static readonly IConnectionManager ConnectionReadManager = new
            ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ToString());

        private readonly IDBScopeManager dbScopeManager = null;
        private readonly IDBScopeManager dbScopeReadManager = null;

        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(TagConfigManager));
        public TagConfigManager()
        {
            this.dbScopeManager = new DBScopeManager(ConnectionManager);
            this.dbScopeReadManager = new DBScopeManager(ConnectionReadManager);
        }
        /// <summary>
        /// 获取标签配置
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<ArticleTabConfig> SelectArticleTabConfig(int pageIndex, int pageSize)
        {
            IEnumerable<ArticleTabConfig> result = null;
            try
            {
                result = dbScopeReadManager.Execute(conn => DalTagConfig.GetArticleTabConfig(conn, pageIndex, pageSize));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }
        /// <summary>
        /// 添加标签配置
        /// </summary>
        /// <param name="config"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool InsertArticleTabConfig(ArticleTabConfig config, string user)
        {
            var result = false;
            try
            {
                if (config != null)
                {
                    config.Source = TagSource.TuhuWangPai;
                    config.CreateUser = user;
                    result = dbScopeManager.Execute(conn => DalTagConfig.InsertArticleTabConfig(conn, config)) > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }
        /// <summary>
        /// 刷新缓存
        /// </summary>
        /// <returns></returns>
        public bool RefreshTArticleTabConfigCache()
        {
            var result = false;
            try
            {
                result = ConfigService.RefreshArticleTabConfigCache();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }
    }
}
