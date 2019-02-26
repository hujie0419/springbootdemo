using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class PointsRulesConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("PointsRulesConfig");

        public List<PointsRulesConfig> GetPointsRulesConfigList(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return  DALPointsRulesConfig.GetPointsRulesConfigList(sqlStr, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new PointsRulesConfigException(1, "GetPointsRulesConfigList", ex);
                Logger.Log(Level.Error, exception, "GetPointsRulesConfigList");
                throw ex;
            }
        }

        public PointsRulesConfig GetPointsRulesConfig(int id)
        {
            try
            {
                return DALPointsRulesConfig.GetPointsRulesConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new PointsRulesConfigException(1, "GetPointsRulesConfig", ex);
                Logger.Log(Level.Error, exception, "GetPointsRulesConfig");
                throw ex;
            }
        }

        public bool UpdatePointsRulesConfig(PointsRulesConfig model)
        {
            try
            {
                return DALPointsRulesConfig.UpdatePointsRulesConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new PointsRulesConfigException(1, "UpdatePointsRulesConfig", ex);
                Logger.Log(Level.Error, exception, "UpdatePointsRulesConfig");
                throw ex;
            }

        }

        public bool InsertPointsRulesConfig(PointsRulesConfig model)
        {
            try
            {
                return DALPointsRulesConfig.InsertPointsRulesConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new PointsRulesConfigException(1, "InsertPointsRulesConfig", ex);
                Logger.Log(Level.Error, exception, "InsertPointsRulesConfig");
                throw ex;
            }
        }
        public bool DeletePointsRulesConfig(int id)
        {
            try
            {     
                return DALPointsRulesConfig.DeletePointsRulesConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new PointsRulesConfigException(1, "DeletePointsRulesConfig", ex);
                Logger.Log(Level.Error, exception, "DeletePointsRulesConfig");
                throw ex;
            }
        }
    }
}
