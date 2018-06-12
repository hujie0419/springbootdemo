using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class CouponActivityConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("CouponActivityConfig");

        public List<CouponActivityConfig> GetCouponActivityConfigList(int type, string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DalCouponActivityConfig.GetCouponActivityConfig(type, sqlStr, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new CouponActivityConfigException(1, "GetCouponActivityConfigList", ex);
                Logger.Log(Level.Error, exception, "GetCouponActivityConfigList");
                throw ex;
            }
        }

        public CouponActivityConfig GetCouponActivityConfig(int id)
        {
            try
            {
                return DalCouponActivityConfig.GetCouponActivityConfigById(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new CouponActivityConfigException(1, "GetCouponActivityConfig", ex);
                Logger.Log(Level.Error, exception, "GetCouponActivityConfig");
                throw ex;
            }
        }

        public bool UpdateCouponActivityConfig(CouponActivityConfig model)
        {
            try
            {
                return DalCouponActivityConfig.UpdateCouponActivityConfig(model);

            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new CouponActivityConfigException(1, "UpdateCouponActivityConfig", ex);
                Logger.Log(Level.Error, exception, "UpdateCouponActivityConfig");
                throw ex;
            }

        }

        public bool InsertCouponActivityConfig(CouponActivityConfig model)
        {
            try
            {
                return DalCouponActivityConfig.InsertCouponActivityConfig(model);

            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new CouponActivityConfigException(1, "InsertCouponActivityConfig", ex);
                Logger.Log(Level.Error, exception, "InsertCouponActivityConfig");
                throw ex;
            }
        }

        public bool DeleteCouponActivityConfig(int id)
        {
            try
            {
                return DalCouponActivityConfig.DeleteCouponActivityConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new CouponActivityConfigException(1, "DeleteCouponActivityConfig", ex);
                Logger.Log(Level.Error, exception, "DeleteCouponActivityConfig");
                throw ex;
            }
        }
    }
}
