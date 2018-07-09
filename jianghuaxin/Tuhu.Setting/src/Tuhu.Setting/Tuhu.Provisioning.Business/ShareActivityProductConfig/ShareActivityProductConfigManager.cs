using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class ShareActivityProductConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("ShareActivityProductConfig");

        public List<ShareActivityProductConfig> GetShareActivityProductConfigList(ShareActivityProductConfig model, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALShareActivityProductConfig.GetShareActivityProductConfigList(model, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ShareActivityProductConfigException(1, "GetShareActivityProductConfigList", ex);
                Logger.Log(Level.Error, exception, "GetShareActivityProductConfigList");
                throw ex;
            }
        }

        public List<ShareActivityProductConfig> GetShareActivityProduct()
        {
            try
            {
                return DALShareActivityProductConfig.GetShareActivityProduct();
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ShareActivityProductConfigException(1, "GetShareActivityProduct", ex);
                Logger.Log(Level.Error, exception, "GetShareActivityProduct");
                throw ex;
            }
        }

        public ShareActivityProductConfig GetShareActivityProductConfig(int id)
        {
            try
            {
                return DALShareActivityProductConfig.GetShareActivityProductConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ShareActivityProductConfigException(1, "GetShareActivityProductConfig", ex);
                Logger.Log(Level.Error, exception, "GetShareActivityProductConfig");
                throw ex;
            }
        }

        public bool UpdateShareActivityProductConfig(ShareActivityProductConfig model)
        {
            try
            {
                return DALShareActivityProductConfig.UpdateShareActivityProductConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ShareActivityProductConfigException(1, "UpdateShareActivityProductConfig", ex);
                Logger.Log(Level.Error, exception, "UpdateShareActivityProductConfig");
                throw ex;
            }

        }

        public bool InsertShareActivityProductConfig(ShareActivityProductConfig model)
        {
            try
            {
                return DALShareActivityProductConfig.InsertShareActivityProductConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ShareActivityProductConfigException(1, "InsertShareActivityProductConfig", ex);
                Logger.Log(Level.Error, exception, "InsertShareActivityProductConfig");
                throw ex;
            }
        }
        public bool DeleteShareActivityProductConfig(int id)
        {
            try
            {
                return DALShareActivityProductConfig.DeleteShareActivityProductConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new ShareActivityProductConfigException(1, "DeleteShareActivityProductConfig", ex);
                Logger.Log(Level.Error, exception, "DeleteShareActivityProductConfig");
                throw ex;
            }
        }
    }
}
