using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class BusinessKeywordsConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("BusinessKeywordsConfig");

        public List<BusinessKeywordsConfig> GetBusinessKeywordsConfigList(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALBusinessKeywordsConfig.GetBusinessKeywordsConfigList(sqlStr, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BusinessKeywordsConfigException(1, "GetBusinessKeywordsConfigList", ex);
                Logger.Log(Level.Error, exception, "GetBusinessKeywordsConfigList");
                throw ex;
            }
        }

        public BusinessKeywordsConfig GetBusinessKeywordsConfig(int id)
        {
            try
            {
                return DALBusinessKeywordsConfig.GetBusinessKeywordsConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BusinessKeywordsConfigException(1, "GetBusinessKeywordsConfig", ex);
                Logger.Log(Level.Error, exception, "GetBusinessKeywordsConfig");
                throw ex;
            }
        }

        public bool UpdateBusinessKeywordsConfig(BusinessKeywordsConfig model)
        {
            try
            {
                return DALBusinessKeywordsConfig.UpdateBusinessKeywordsConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BusinessKeywordsConfigException(1, "UpdateBusinessKeywordsConfig", ex);
                Logger.Log(Level.Error, exception, "UpdateBusinessKeywordsConfig");
                throw ex;
            }

        }

        public bool InsertBusinessKeywordsConfig(BusinessKeywordsConfig model)
        {
            try
            {
                return DALBusinessKeywordsConfig.InsertBusinessKeywordsConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BusinessKeywordsConfigException(1, "InsertBusinessKeywordsConfig", ex);
                Logger.Log(Level.Error, exception, "InsertBusinessKeywordsConfig");
                throw ex;
            }
        }
        public bool DeleteBusinessKeywordsConfig(int id)
        {
            try
            {
                return DALBusinessKeywordsConfig.DeleteBusinessKeywordsConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BusinessKeywordsConfigException(1, "DeleteBusinessKeywordsConfig", ex);
                Logger.Log(Level.Error, exception, "DeleteBusinessKeywordsConfig");
                throw ex;
            }
        }
    }
}
