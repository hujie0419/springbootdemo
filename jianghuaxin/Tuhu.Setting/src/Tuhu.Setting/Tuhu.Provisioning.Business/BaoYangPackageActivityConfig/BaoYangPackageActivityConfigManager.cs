using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class BaoYangPackageActivityConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("BaoYangPackageActivityConfig");

        public List<BaoYangPackageActivityConfig> GetBaoYangPackageActivityConfigList(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DalBaoYangPackageActivityConfig.GetBaoYangPackageActivityConfig(sqlStr, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BaoYangPackageActivityConfigException(1, "GetBaoYangPackageActivityConfigList", ex);
                Logger.Log(Level.Error, exception, "GetBaoYangPackageActivityConfigList");
                throw ex;
            }
        }

        public BaoYangPackageActivityConfig GetBaoYangPackageActivityConfig(int id)
        {
            try
            {
                return DalBaoYangPackageActivityConfig.GetBaoYangPackageActivityConfigById(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BaoYangPackageActivityConfigException(1, "GetBaoYangPackageActivityConfig", ex);
                Logger.Log(Level.Error, exception, "GetBaoYangPackageActivityConfig");
                throw ex;
            }
        }

        public bool UpdateBaoYangPackageActivityConfig(BaoYangPackageActivityConfig model)
        {
            try
            {
                return DalBaoYangPackageActivityConfig.UpdateBaoYangPackageActivityConfig(model);

            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BaoYangPackageActivityConfigException(1, "UpdateBaoYangPackageActivityConfig", ex);
                Logger.Log(Level.Error, exception, "UpdateBaoYangPackageActivityConfig");
                throw ex;
            }

        }

        public bool InsertBaoYangPackageActivityConfig(BaoYangPackageActivityConfig model)
        {
            try
            {
                return DalBaoYangPackageActivityConfig.InsertBaoYangPackageActivityConfig(model);

            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BaoYangPackageActivityConfigException(1, "InsertBaoYangPackageActivityConfig", ex);
                Logger.Log(Level.Error, exception, "InsertBaoYangPackageActivityConfig");
                throw ex;
            }
        }

        public bool DeleteBaoYangPackageActivityConfig(int id)
        {
            try
            {
                return DalBaoYangPackageActivityConfig.DeleteBaoYangPackageActivityConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BaoYangPackageActivityConfigException(1, "DeleteBaoYangPackageActivityConfig", ex);
                Logger.Log(Level.Error, exception, "DeleteBaoYangPackageActivityConfig");
                throw ex;
            }
        }
    }
}
