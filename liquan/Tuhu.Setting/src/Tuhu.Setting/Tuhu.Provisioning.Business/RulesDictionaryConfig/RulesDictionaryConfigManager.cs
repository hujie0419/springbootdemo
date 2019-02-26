using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class RulesDictionaryConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("RulesDictionaryConfig");
        
        public RulesDictionaryConfig GetRulesDictionaryConfig(int type)
        {
            try
            {
                return DALRulesDictionaryConfig.GetRulesDictionaryConfig(type);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new RulesDictionaryConfigException(1, "GetRulesDictionaryConfig", ex);
                Logger.Log(Level.Error, "GetRulesDictionaryConfig", exception);
                throw ex;
            }
        }
        public RulesDictionaryConfig GetRulesDictionaryConfigById(int id)
        {
            try
            {
                return DALRulesDictionaryConfig.GetRulesDictionaryConfigById(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new RulesDictionaryConfigException(1, "GetRulesDictionaryConfigById", ex);
                Logger.Log(Level.Error, "GetRulesDictionaryConfigById", exception);
                throw ex;
            }
        }
        public bool UpdateRulesDictionaryConfig(RulesDictionaryConfig model)
        {
            try
            {
                return DALRulesDictionaryConfig.UpdateRulesDictionaryConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new RulesDictionaryConfigException(1, "UpdateRulesDictionaryConfig", ex);
                Logger.Log(Level.Error, "UpdateRulesDictionaryConfig", exception);
                throw ex;
            }

        }

        public bool InsertRulesDictionaryConfig(RulesDictionaryConfig model)
        {
            try
            {
                return DALRulesDictionaryConfig.InsertRulesDictionaryConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new RulesDictionaryConfigException(1, "InsertRulesDictionaryConfig", ex);
                Logger.Log(Level.Error, "InsertRulesDictionaryConfig", exception);
                throw ex;
            }
        }

        public bool DeleteRulesDictionaryConfig(int id)
        {
            try
            {
                return DALRulesDictionaryConfig.DeleteRulesDictionaryConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new RulesDictionaryConfigException(1, "DeleteRulesDictionaryConfig", ex);
                Logger.Log(Level.Error, "DeleteRulesDictionaryConfig", exception);
                throw ex;
            }
        }
    }
}
