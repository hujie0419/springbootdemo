using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class VIPAuthorizationRuleConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("VIPAuthorizationRuleConfig");

        public List<VIPAuthorizationRuleConfig> GetVIPAuthorizationRuleConfigList(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALVIPAuthorizationRuleConfig.GetVIPAuthorizationRuleConfigList(sqlStr, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new VIPAuthorizationRuleConfigException(1, "GetVIPAuthorizationRuleConfigList", ex);
                Logger.Log(Level.Error, exception, "GetVIPAuthorizationRuleConfigList");
                throw ex;
            }
        }
        public List<VIPAuthorizationRuleConfig> GetVIPAuthorizationRuleAndId()
        {
            try
            {
                return DALVIPAuthorizationRuleConfig.GetVIPAuthorizationRuleAndId();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new VIPAuthorizationRuleConfigException(1, "GetVIPAuthorizationRuleAndId", ex);
                Logger.Log(Level.Error, exception, "GetVIPAuthorizationRuleAndId");
                throw ex;
            }
        }
        public VIPAuthorizationRuleConfig GetVIPAuthorizationRuleConfig(int id)
        {
            try
            {
                return DALVIPAuthorizationRuleConfig.GetVIPAuthorizationRuleConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new VIPAuthorizationRuleConfigException(1, "GetVIPAuthorizationRuleConfig", ex);
                Logger.Log(Level.Error, exception, "GetVIPAuthorizationRuleConfig");
                throw ex;
            }
        }

        public bool UpdateVIPAuthorizationRuleConfig(VIPAuthorizationRuleConfig model)
        {
            try
            {
                return DALVIPAuthorizationRuleConfig.UpdateVIPAuthorizationRuleConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new VIPAuthorizationRuleConfigException(1, "UpdateVIPAuthorizationRuleConfig", ex);
                Logger.Log(Level.Error, exception, "UpdateVIPAuthorizationRuleConfig");
                throw ex;
            }

        }

        public bool InsertVIPAuthorizationRuleConfig(VIPAuthorizationRuleConfig model)
        {
            try
            {
                return DALVIPAuthorizationRuleConfig.InsertVIPAuthorizationRuleConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new VIPAuthorizationRuleConfigException(1, "InsertVIPAuthorizationRuleConfig", ex);
                Logger.Log(Level.Error, exception, "InsertVIPAuthorizationRuleConfig");
                throw ex;
            }
        }

        public bool DeleteVIPAuthorizationRuleConfig(int id)
        {
            try
            {
                return DALVIPAuthorizationRuleConfig.DeleteVIPAuthorizationRuleConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new VIPAuthorizationRuleConfigException(1, "DeleteVIPAuthorizationRuleConfig", ex);
                Logger.Log(Level.Error, exception, "DeleteVIPAuthorizationRuleConfig");
                throw ex;
            }
        }
    }
}
