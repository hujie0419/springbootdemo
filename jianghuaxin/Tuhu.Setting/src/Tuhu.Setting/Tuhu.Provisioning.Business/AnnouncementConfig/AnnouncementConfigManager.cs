using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.BaoYang;

namespace Tuhu.Provisioning.Business
{
    public class AnnouncementConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("AnnouncementConfig");

        public List<AnnouncementConfig> GetAnnouncementConfigList(AnnouncementConfig model, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALAnnouncementConfig.GetAnnouncementConfigList(model, pageSize, pageIndex, out recordCount);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new AnnouncementConfigException(1, "GetAnnouncementConfigList", ex);
                Logger.Log(Level.Error, exception, "GetAnnouncementConfigList1");
                throw ex;
            }
        }

        public AnnouncementConfig GetAnnouncementConfig(int id)
        {
            try
            {
                return DALAnnouncementConfig.GetAnnouncementConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new AnnouncementConfigException(1, "GetAnnouncementConfig", ex);
                Logger.Log(Level.Error, exception, "GetAnnouncementConfig");
                throw ex;
            }
        }

        public bool UpdateAnnouncementConfig(AnnouncementConfig model)
        {
            try
            {
                using (var client = new CacheClient())
                {
                    var result = client.UpdateBaoYangNoticeSetting();

                }
                return DALAnnouncementConfig.UpdateAnnouncementConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new AnnouncementConfigException(1, "UpdateAnnouncementConfig", ex);
                Logger.Log(Level.Error, exception, "UpdateAnnouncementConfig");
                throw ex;
            }

        }

        public bool InsertAnnouncementConfig(AnnouncementConfig model)
        {
            try
            {
                using (var client = new CacheClient())
                {
                    var result = client.UpdateBaoYangNoticeSetting();

                }
                return DALAnnouncementConfig.InsertAnnouncementConfig(model);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new AnnouncementConfigException(1, "InsertAnnouncementConfig", ex);
                Logger.Log(Level.Error, exception, "InsertAnnouncementConfig");
                throw ex;
            }
        }
        public bool DeleteAnnouncementConfig(int id)
        {
            try
            {
                return DALAnnouncementConfig.DeleteAnnouncementConfig(id);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new AnnouncementConfigException(1, "DeleteAnnouncementConfig", ex);
                Logger.Log(Level.Error, exception, "DeleteAnnouncementConfig");
                throw ex;
            }
        }
    }
}
