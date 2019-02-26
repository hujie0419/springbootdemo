using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class BaoYangRecommendServiceConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("BaoYangRecommendServiceConfig");

        public List<BaoYangRecommendServiceConfig> GetBaoYangRecommendServiceConfigList(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALBaoYangRecommendServiceConfig.GetBaoYangRecommendServiceConfigList(sqlStr, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BaoYangRecommendServiceConfigException(1, "GetBaoYangRecommendServiceConfigList", ex);
                Logger.Log(Level.Error, "GetBaoYangRecommendServiceConfigList", exception);
                throw ex;
            }
        }

        public BaoYangRecommendServiceConfig GetBaoYangRecommendServiceConfig(int id)
        {
            try
            {
                return DALBaoYangRecommendServiceConfig.GetBaoYangRecommendServiceConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BaoYangRecommendServiceConfigException(1, "GetBaoYangRecommendServiceConfig", ex);
                Logger.Log(Level.Error, "GetBaoYangRecommendServiceConfig", exception);
                throw ex;
            }
        }

        public bool UpdateBaoYangRecommendServiceConfig(BaoYangRecommendServiceConfig model)
        {
            try
            {
                return DALBaoYangRecommendServiceConfig.UpdateBaoYangRecommendServiceConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BaoYangRecommendServiceConfigException(1, "UpdateBaoYangRecommendServiceConfig", ex);
                Logger.Log(Level.Error, "UpdateBaoYangRecommendServiceConfig", exception);
                throw ex;
            }

        }

        public bool InsertBaoYangRecommendServiceConfig(BaoYangRecommendServiceConfig model)
        {
            try
            {
                return DALBaoYangRecommendServiceConfig.InsertBaoYangRecommendServiceConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BaoYangRecommendServiceConfigException(1, "InsertBaoYangRecommendServiceConfig", ex);
                Logger.Log(Level.Error, "InsertBaoYangRecommendServiceConfig", exception);
                throw ex;
            }
        }

        public bool CheckService(BaoYangRecommendServiceConfig model)
        {
            try
            {
                return DALBaoYangRecommendServiceConfig.CheckService(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BaoYangRecommendServiceConfigException(1, "CheckService", ex);
                Logger.Log(Level.Error, "CheckService", exception);
                throw ex;
            }
        }
        public bool DeleteBaoYangRecommendServiceConfig(int id)
        {
            try
            {
                return DALBaoYangRecommendServiceConfig.DeleteBaoYangRecommendServiceConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BaoYangRecommendServiceConfigException(1, "DeleteBaoYangRecommendServiceConfig", ex);
                Logger.Log(Level.Error, "DeleteBaoYangRecommendServiceConfig", exception);
                throw ex;
            }
        }
    }
}
