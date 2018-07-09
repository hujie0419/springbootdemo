using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class ShareActivityRulesConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("ShareActivityRulesConfig");

        public List<ShareActivityRulesConfig> GetShareActivityRulesList(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALShareActivityRules.GetShareActivityRulesList(sqlStr, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ShareActivityRulesConfigException(1, "GetShareActivityRulesList", ex);
                Logger.Log(Level.Error, exception, "GetShareActivityRulesList");
                throw ex;
            }
        }

        public ShareActivityRulesConfig GetShareActivityRules(int id)
        {
            try
            {
                return DALShareActivityRules.GetShareActivityRules(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ShareActivityRulesConfigException(1, "GetShareActivityRules", ex);
                Logger.Log(Level.Error, exception, "GetShareActivityRules");
                throw ex;
            }
        }

        public bool UpdateShareActivityRules(ShareActivityRulesConfig model)
        {
            try
            {
                return DALShareActivityRules.UpdateShareActivityRules(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ShareActivityRulesConfigException(1, "UpdateShareActivityRules", ex);
                Logger.Log(Level.Error, exception, "UpdateShareActivityRules");
                throw ex;
            }

        }

        public bool InsertShareActivityRules(ShareActivityRulesConfig model)
        {
            try
            {
                return DALShareActivityRules.InsertShareActivityRules(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ShareActivityRulesConfigException(1, "InsertShareActivityRules", ex);
                Logger.Log(Level.Error, exception, "InsertShareActivityRules");
                throw ex;
            }
        }
        public bool DeleteShareActivityRules(int id)
        {
            try
            {
                return DALShareActivityRules.DeleteShareActivityRules(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ShareActivityRulesConfigException(1, "DeleteShareActivityRules", ex);
                Logger.Log(Level.Error, exception, "DeleteShareActivityRules");
                throw ex;
            }
        }
    }
}
