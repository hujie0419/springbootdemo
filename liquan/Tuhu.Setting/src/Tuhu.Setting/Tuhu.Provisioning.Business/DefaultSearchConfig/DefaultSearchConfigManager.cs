using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class DefaultSearchConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("DefaultSearchConfig");

        public List<DefaultSearchConfig> GetDefaultSearchConfigList(string type, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALDefaultSearchConfig.GetDefaultSearchConfigList(type, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DefaultSearchConfigException(1, "GetDefaultSearchConfigList", ex);
                Logger.Log(Level.Error, exception, "GetDefaultSearchConfigList");
                throw ex;
            }
        }

        public DefaultSearchConfig GetDefaultSearchConfig(int id)
        {
            try
            {
                return DALDefaultSearchConfig.GetDefaultSearchConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DefaultSearchConfigException(1, "GetDefaultSearchConfig", ex);
                Logger.Log(Level.Error, exception, "GetDefaultSearchConfig");
                throw ex;
            }
        }

        public bool UpdateDefaultSearchConfig(DefaultSearchConfig model)
        {
            try
            {
                return DALDefaultSearchConfig.UpdateDefaultSearchConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DefaultSearchConfigException(1, "UpdateDefaultSearchConfig", ex);
                Logger.Log(Level.Error, exception, "UpdateDefaultSearchConfig");
                throw ex;
            }

        }

        public bool InsertDefaultSearchConfig(DefaultSearchConfig model)
        {
            try
            {
                return DALDefaultSearchConfig.InsertDefaultSearchConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DefaultSearchConfigException(1, "InsertDefaultSearchConfig", ex);
                Logger.Log(Level.Error, exception, "InsertDefaultSearchConfig");
                throw ex;
            }
        }
        public bool DeleteDefaultSearchConfig(int id)
        {
            try
            {
                return DALDefaultSearchConfig.DeleteDefaultSearchConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new DefaultSearchConfigException(1, "DeleteDefaultSearchConfig", ex);
                Logger.Log(Level.Error, exception, "DeleteDefaultSearchConfig");
                throw ex;
            }
        }
    }
}
