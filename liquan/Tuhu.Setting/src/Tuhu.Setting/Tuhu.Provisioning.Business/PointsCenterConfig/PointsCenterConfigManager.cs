using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class PointsCenterConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("PointsCenterConfig");

        public List<PointsCenterConfig> GetPointsCenterConfigList(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DAlPointsCenterConfig.GetPointsCenterConfigList(sqlStr, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new PointsCenterConfigException(1, "GetPointsCenterConfigList", ex);
                Logger.Log(Level.Error, exception, "GetPointsCenterConfigList");
                throw ex;
            }
        }

        public PointsCenterConfig GetPointsCenterConfig(int id)
        {
            try
            {
                return DAlPointsCenterConfig.GetPointsCenterConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new PointsCenterConfigException(1, "GetPointsCenterConfig", ex);
                Logger.Log(Level.Error, exception, "GetPointsCenterConfig");
                throw ex;
            }
        }

        public bool UpdatePointsCenterConfig(PointsCenterConfig model)
        {
            try
            {
                return DAlPointsCenterConfig.UpdatePointsCenterConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new PointsCenterConfigException(1, "UpdatePointsCenterConfig", ex);
                Logger.Log(Level.Error, exception, "UpdatePointsCenterConfig");
                throw ex;
            }

        }

        public bool InsertPointsCenterConfig(PointsCenterConfig model)
        {
            try
            {
                return DAlPointsCenterConfig.InsertPointsCenterConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new PointsCenterConfigException(1, "InsertPointsCenterConfig", ex);
                Logger.Log(Level.Error, exception, "InsertPointsCenterConfig");
                throw ex;
            }
        }
        public bool DeletePointsCenterConfig(int id)
        {
            try
            {
                return DAlPointsCenterConfig.DeletePointsCenterConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new PointsCenterConfigException(1, "DeletePointsCenterConfig", ex);
                Logger.Log(Level.Error, exception, "DeletePointsCenterConfig");
                throw ex;
            }
        }
    }
}
