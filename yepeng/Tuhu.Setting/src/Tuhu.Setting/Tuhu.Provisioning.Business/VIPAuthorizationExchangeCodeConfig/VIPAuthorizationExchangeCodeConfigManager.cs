using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class VIPAuthorizationExchangeCodeConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("VIPAuthorizationExchangeCodeConfig");

        public List<VIPAuthorizationExchangeCodeConfig> GetVIPAuthorizationExchangeCodeConfigList(string pranm, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALVIPAuthorizationExchangeCodeConfig.GetVIPAuthorizationExchangeCodeConfigList(pranm, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new VIPAuthorizationExchangeCodeConfigException(1, "GetVIPAuthorizationExchangeCodeConfigList", ex);
                Logger.Log(Level.Error, exception, "GetVIPAuthorizationExchangeCodeConfigList");
                throw ex;
            }
        }

        public VIPAuthorizationExchangeCodeConfig GetVIPAuthorizationExchangeCodeConfig(int id)
        {
            try
            {
                return DALVIPAuthorizationExchangeCodeConfig.GetVIPAuthorizationExchangeCodeConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new VIPAuthorizationExchangeCodeConfigException(1, "GetVIPAuthorizationExchangeCodeConfig", ex);
                Logger.Log(Level.Error, exception, "GetVIPAuthorizationExchangeCodeConfig");
                throw ex;
            }
        }

        public List<VIPAuthorizationExchangeCodeConfig> GetCodeBatch()
        {
            try
            {
                return DALVIPAuthorizationExchangeCodeConfig.GetCodeBatch();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new VIPAuthorizationExchangeCodeConfigException(1, "GetVIPAuthorizationExchangeCodeConfig", ex);
                Logger.Log(Level.Error, exception, "GetVIPAuthorizationExchangeCodeConfig");
                throw ex;
            }
        }
        public bool UpdateVIPAuthorizationExchangeCodeConfig(VIPAuthorizationExchangeCodeConfig model)
        {
            try
            {
                return DALVIPAuthorizationExchangeCodeConfig.UpdateVIPAuthorizationExchangeCodeConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new VIPAuthorizationExchangeCodeConfigException(1, "UpdateVIPAuthorizationExchangeCodeConfig", ex);
                Logger.Log(Level.Error, exception, "UpdateVIPAuthorizationExchangeCodeConfig");
                throw ex;
            }

        }

        public bool InsertVIPAuthorizationExchangeCodeConfig(VIPAuthorizationExchangeCodeConfig model)
        {
            try
            {
                return DALVIPAuthorizationExchangeCodeConfig.InsertVIPAuthorizationExchangeCodeConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new VIPAuthorizationExchangeCodeConfigException(1, "InsertVIPAuthorizationExchangeCodeConfig", ex);
                Logger.Log(Level.Error, exception, "InsertVIPAuthorizationExchangeCodeConfig");
                throw ex;
            }
        }

        public bool DeleteVIPAuthorizationExchangeCodeConfig(string id)
        {
            try
            {
                return DALVIPAuthorizationExchangeCodeConfig.DeleteVIPAuthorizationExchangeCodeConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new VIPAuthorizationExchangeCodeConfigException(1, "DeleteVIPAuthorizationExchangeCodeConfig", ex);
                Logger.Log(Level.Error, exception, "DeleteVIPAuthorizationExchangeCodeConfig");
                throw ex;
            }
        }
    }
}
