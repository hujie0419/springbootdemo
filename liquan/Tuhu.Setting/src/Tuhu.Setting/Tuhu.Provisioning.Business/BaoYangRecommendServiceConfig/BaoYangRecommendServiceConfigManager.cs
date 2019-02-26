using Common.Logging;
using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.BaoYang;

namespace Tuhu.Provisioning.Business
{
    public class BaoYangRecommendServiceConfigManager
    {
        private static readonly Common.Logging.ILog Logger = LogManager.GetLogger(typeof(BaoYangRecommendServiceConfigManager));

        public Tuple<List<BaoYangRecommendServiceConfig>, int> GetBaoYangRecommendServiceConfigList(string type, int pageSize, int pageIndex)
        {
            var result = null as List<BaoYangRecommendServiceConfig>;
            var recordCount = 0;
            try
            {
                result = DALBaoYangRecommendServiceConfig.GetBaoYangRecommendServiceConfigList(type, pageSize, pageIndex, out recordCount);
            }
            catch (Exception ex)
            {
                Logger.Error("GetBaoYangRecommendServiceConfigList", ex);
            }
            return Tuple.Create(result ?? new List<BaoYangRecommendServiceConfig>(), recordCount);
        }

        public BaoYangRecommendServiceConfig GetBaoYangRecommendServiceConfig(int id)
        {
            var result = null as BaoYangRecommendServiceConfig;
            try
            {
                result = DALBaoYangRecommendServiceConfig.GetBaoYangRecommendServiceConfig(id);
            }
            catch (Exception ex)
            {
                Logger.Error("GetBaoYangRecommendServiceConfig", ex);
            }
            return result;
        }

        public bool UpdateBaoYangRecommendServiceConfig(BaoYangRecommendServiceConfig model)
        {
            var result = false;
            try
            {
                if (DALBaoYangRecommendServiceConfig.UpdateBaoYangRecommendServiceConfig(model))
                {
                    using (var client = new CacheClient())
                    {
                        client.UpdateTuhuRecommendConfigAsync();
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UpdateBaoYangRecommendServiceConfig", ex);
            }
            return result;
        }

        public bool InsertBaoYangRecommendServiceConfig(BaoYangRecommendServiceConfig model)
        {
            var result = false;
            try
            {
                if (DALBaoYangRecommendServiceConfig.InsertBaoYangRecommendServiceConfig(model))
                {
                    using (var client = new CacheClient())
                    {
                        client.UpdateTuhuRecommendConfigAsync();
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("InsertBaoYangRecommendServiceConfig", ex);
            }
            return result;
        }

        public bool CheckService(BaoYangRecommendServiceConfig model)
        {
            var result = false;
            try
            {
                result = DALBaoYangRecommendServiceConfig.CheckService(model);
            }
            catch (Exception ex)
            {
                Logger.Error("CheckService", ex);
            }
            return result;
        }
        public bool DeleteBaoYangRecommendServiceConfig(int id)
        {
            var result = false;
            try
            {
                if (DALBaoYangRecommendServiceConfig.DeleteBaoYangRecommendServiceConfig(id))
                {
                    using (var client = new CacheClient())
                    {
                        client.UpdateTuhuRecommendConfigAsync();
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("DeleteBaoYangRecommendServiceConfig", ex);
            }
            return result;
        }
    }
}
