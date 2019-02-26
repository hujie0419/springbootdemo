using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class ExchangeCenterConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("ExchangeCenterConfig");


        public List<ExchangeCenterConfig> GetExchangeCenterConfigList(int id)
        {
            try
            {
                return DALExchangeCenterConfig.GetExchangeCenterConfigList(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ExchangeCenterConfigException(1, "GetExchangeCenterConfigList", ex);
                Logger.Log(Level.Error, exception, "GetExchangeCenterConfigList");
                throw ex;
            }
        }

        public List<ExchangeCenterConfig> GetExchangeCenterConfigList()
        {
            try
            {
                return DALExchangeCenterConfig.GetExchangeCenterConfigList();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ExchangeCenterConfigException(1, "GetExchangeCenterConfigList", ex);
                Logger.Log(Level.Error, exception, "GetExchangeCenterConfigList");
                throw ex;
            }
        }
        public ExchangeCenterConfig GetExchangeCenterConfig(int id)
        {
            try
            {
                return DALExchangeCenterConfig.GetExchangeCenterConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ExchangeCenterConfigException(1, "GetExchangeCenterConfig", ex);
                Logger.Log(Level.Error, exception, "GetExchangeCenterConfig");
                throw ex;
            }
        }

        /// <summary>
        /// 更新积分兑换
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateExchangeCenterConfig(ExchangeCenterConfig model)
        {
            try
            {
                return DALExchangeCenterConfig.UpdateExchangeCenterConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ExchangeCenterConfigException(1, "UpdateExchangeCenterConfig", ex);
                Logger.Log(Level.Error, exception, "UpdateExchangeCenterConfig");
                throw ex;
            }

        }

        /// <summary>
        /// 添加积分兑换
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int InsertExchangeCenterConfig(ExchangeCenterConfig model)
        {
            try
            {
                return DALExchangeCenterConfig.InsertExchangeCenterConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ExchangeCenterConfigException(1, "InsertExchangeCenterConfig", ex);
                Logger.Log(Level.Error, exception, "InsertExchangeCenterConfig");
                throw ex;
            }
        }
        public bool DeleteExchangeCenterConfig(int id)
        {
            try
            {
                return DALExchangeCenterConfig.DeleteExchangeCenterConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ExchangeCenterConfigException(1, "DeleteExchangeCenterConfig", ex);
                Logger.Log(Level.Error, exception, "DeleteExchangeCenterConfig");
                throw ex;
            }
        }

        public bool DeletePersonalCenterCouponConfig(int id)
        {
            try
            {
                return DALExchangeCenterConfig.DeletePersonalCenterCouponConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ExchangeCenterConfigException(1, "DeletePersonalCenterCouponConfig", ex);
                Logger.Log(Level.Error, exception, "DeletePersonalCenterCouponConfig");
                throw ex;
            }
        }
        
        public int InsertPersonalCenterCouponConfig(PersonalCenterCouponConfig id)
        {
            try
            {
                return DALExchangeCenterConfig.InsertPersonalCenterCouponConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ExchangeCenterConfigException(1, "InsertPersonalCenterCouponConfig", ex);
                Logger.Log(Level.Error, exception, "InsertPersonalCenterCouponConfig");
                throw ex;
            }
        }

        public bool UpdatePersonalCenterCouponConfig(PersonalCenterCouponConfig model)
        {
            try
            {
                return DALExchangeCenterConfig.UpdatePersonalCenterCouponConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ExchangeCenterConfigException(1, "InsertPersonalCenterCouponConfig", ex);
                Logger.Log(Level.Error, exception, "InsertPersonalCenterCouponConfig");
                throw ex;
            }
        }
    }
}
