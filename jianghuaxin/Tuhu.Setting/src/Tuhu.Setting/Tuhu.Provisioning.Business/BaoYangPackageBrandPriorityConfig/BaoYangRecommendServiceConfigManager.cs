using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class BaoYangPackageBrandPriorityConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("BaoYangPackageBrandPriorityConfig");

        public List<BaoYangPackageBrandPriorityConfig> GetBaoYangPackageBrandPriorityConfigList(string brand, string vehicle, string data, int pageSize, int pageIndex, out int recordCount, int startPrice = 0, int endPrice = 0)
        {
            try
            {
                return DALBaoYangPackageBrandPriorityConfig.GetBaoYangPackageBrandPriorityConfigList(brand, vehicle, data, pageSize, pageIndex, out recordCount, startPrice, endPrice);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BaoYangPackageBrandPriorityConfigException(1, "GetBaoYangPackageBrandPriorityConfigList", ex);
                Logger.Log(Level.Error, exception, "GetBaoYangPackageBrandPriorityConfigList");
                throw ex;
            }
        }

        public BaoYangPackageBrandPriorityConfig GetBaoYangPackageBrandPriorityConfig(int id)
        {
            try
            {
                return DALBaoYangPackageBrandPriorityConfig.GetBaoYangPackageBrandPriorityConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BaoYangPackageBrandPriorityConfigException(1, "GetBaoYangPackageBrandPriorityConfig", ex);
                Logger.Log(Level.Error, exception, "GetBaoYangPackageBrandPriorityConfig");
                throw ex;
            }
        }

        public bool UpdateBaoYangPackageBrandPriorityConfig(BaoYangPackageBrandPriorityConfig model)
        {
            try
            {
                return DALBaoYangPackageBrandPriorityConfig.UpdateBaoYangPackageBrandPriorityConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BaoYangPackageBrandPriorityConfigException(1, "UpdateBaoYangPackageBrandPriorityConfig", ex);
                Logger.Log(Level.Error, exception, "UpdateBaoYangPackageBrandPriorityConfig");
                throw ex;
            }

        }

        public bool InsertOrUpdate(List<BaoYangPackageBrandPriorityConfig> list)
        {

            try
            {
                return DALBaoYangPackageBrandPriorityConfig.InsertOrUpdate(list);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BaoYangPackageBrandPriorityConfigException(1, "InsertOrUpdate", ex);
                Logger.Log(Level.Error, exception, "InsertOrUpdate");
                throw ex;
            }
        }

        public bool InsertBaoYangPackageBrandPriorityConfig(BaoYangPackageBrandPriorityConfig model)
        {
            try
            {
                return DALBaoYangPackageBrandPriorityConfig.InsertBaoYangPackageBrandPriorityConfig(model);

            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BaoYangPackageBrandPriorityConfigException(1, "InsertBaoYangPackageBrandPriorityConfig", ex);
                Logger.Log(Level.Error, exception, "InsertBaoYangPackageBrandPriorityConfig");
                throw ex;
            }
        }

        public bool DeleteBaoYangPackageBrandPriorityConfig(int id)
        {
            try
            {
                return DALBaoYangPackageBrandPriorityConfig.DeleteBaoYangPackageBrandPriorityConfig(id);

            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BaoYangPackageBrandPriorityConfigException(1, "DeleteBaoYangPackageBrandPriorityConfig", ex);
                Logger.Log(Level.Error, exception, "DeleteBaoYangPackageBrandPriorityConfig");
                throw ex;
            }
        }

        public static List<string> GetAllVehicleBrands()
        {
            try
            {
                return DALBaoYangPackageBrandPriorityConfig.GetAllVehicleBrands();

            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BaoYangPackageBrandPriorityConfigException(1, "GetAllVehicleBrands", ex);
                Logger.Log(Level.Error, exception, "GetAllVehicleBrands");
                throw ex;
            }

        }

        /// <summary>
        /// 根据选择的品牌该品牌的系列
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="brand"></param>
        /// <returns></returns>
        public static IDictionary<string, string> GetVehicleSeries(string brand)
        {
            try
            {
                return DALBaoYangPackageBrandPriorityConfig.GetVehicleSeries(brand);

            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new BaoYangPackageBrandPriorityConfigException(1, "GetVehicleSeries", ex);
                Logger.Log(Level.Error, exception, "GetVehicleSeries");
                throw ex;
            }

        }
    }
}
