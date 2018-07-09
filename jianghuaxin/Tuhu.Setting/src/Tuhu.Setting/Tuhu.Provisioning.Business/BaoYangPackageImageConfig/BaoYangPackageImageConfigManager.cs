using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class BaoYangPackageImageConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("BaoYangPackageImageConfig");

        private static readonly Common.Logging.ILog _Logger = Common.Logging.LogManager.GetLogger(typeof(BaoYangPackageImageConfigManager));

        public List<BaoYangPackageImageConfig> GetBaoYangPackageImageConfigList(string pid, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DalBaoYangPackageImageConfig.GetBaoYangPackageImageConfig(pid, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BaoYangPackageImageConfigException(1, "GetBaoYangPackageImageConfigList", ex);
                Logger.Log(Level.Error, exception, "GetBaoYangPackageImageConfigList", exception);
                throw ex;
            }
        }


        public List<BaoYangPackageImageConfig> GetBaoYangPackagePruduct()
        {
            try
            {
                return DalBaoYangPackageImageConfig.GetBaoYangPackagePruduct();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BaoYangPackageImageConfigException(1, "GetBaoYangPackageImageConfigList", ex);
                Logger.Log(Level.Error, exception, "GetBaoYangPackageImageConfigList");
                throw ex;
            }
        }
        public BaoYangPackageImageConfig GetBaoYangPackageImageConfig(int id)
        {
            try
            {
                return DalBaoYangPackageImageConfig.GetBaoYangPackageImageConfigById(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BaoYangPackageImageConfigException(1, "GetBaoYangPackageImageConfig", ex);
                Logger.Log(Level.Error, exception, "GetBaoYangPackageImageConfig");
                throw ex;
            }
        }

        public bool UpdateBaoYangPackageImageConfigNew(BaoYangPackageImageConfig model)
        {
            try
            {
                return DalBaoYangPackageImageConfig.UpdateBaoYangPackageImageConfigNew(model);
            }
            catch (Exception ex)
            {
                _Logger.Error("UpdateBaoYangPackageImageConfigNew", ex);
                return false;
            }
        }

        public bool DeleteBaoYangPackageImageConfig(int id)
        {
            try
            {
                return DalBaoYangPackageImageConfig.DeleteBaoYangPackageImageConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BaoYangPackageImageConfigException(1, "DeleteBaoYangPackageImageConfig", ex);
                Logger.Log(Level.Error, exception, "DeleteBaoYangPackageImageConfig");
                throw ex;
            }
        }
    }
}
