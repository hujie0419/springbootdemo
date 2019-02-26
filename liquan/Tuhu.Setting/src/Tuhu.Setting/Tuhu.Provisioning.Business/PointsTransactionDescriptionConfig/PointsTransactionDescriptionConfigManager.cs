using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class PointsTransactionDescriptionConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("PointsTransactionDescriptionConfig");

        public List<PointsTransactionDescriptionConfig> GetPointsTransactionDescriptionConfigList(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALPointsTransactionDescriptionConfig.GetPointsTransactionDescriptionConfigList(sqlStr, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new PointsTransactionDescriptionConfigException(1, "GetPointsTransactionDescriptionConfigList", ex);
                Logger.Log(Level.Error, exception, "GetPointsTransactionDescriptionConfigList");
                throw ex;
            }
        }

        public PointsTransactionDescriptionConfig GetPointsTransactionDescriptionConfig(string id)
        {
            try
            {
                return DALPointsTransactionDescriptionConfig.GetPointsTransactionDescriptionConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new PointsTransactionDescriptionConfigException(1, "GetPointsTransactionDescriptionConfig", ex);
                Logger.Log(Level.Error, exception, "GetPointsTransactionDescriptionConfig");
                throw ex;
            }
        }

        public bool UpdatePointsTransactionDescriptionConfig(PointsTransactionDescriptionConfig model)
        {
            try
            {
                return DALPointsTransactionDescriptionConfig.UpdatePointsTransactionDescriptionConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new PointsTransactionDescriptionConfigException(1, "UpdatePointsTransactionDescriptionConfig", ex);
                Logger.Log(Level.Error, exception, "UpdatePointsTransactionDescriptionConfig");
                throw ex;
            }

        }

        public bool InsertPointsTransactionDescriptionConfig(PointsTransactionDescriptionConfig model)
        {
            try
            {
                return DALPointsTransactionDescriptionConfig.InsertPointsTransactionDescriptionConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new PointsTransactionDescriptionConfigException(1, "InsertPointsTransactionDescriptionConfig", ex);
                Logger.Log(Level.Error, exception, "InsertPointsTransactionDescriptionConfig");
                throw ex;
            }
        }
        public bool DeletePointsTransactionDescriptionConfig(string  id)
        {
            try
            {
                return DALPointsTransactionDescriptionConfig.DeletePointsTransactionDescriptionConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new PointsTransactionDescriptionConfigException(1, "DeletePointsTransactionDescriptionConfig", ex);
                Logger.Log(Level.Error, exception, "DeletePointsTransactionDescriptionConfig");
                throw ex;
            }
        }
    }
}
