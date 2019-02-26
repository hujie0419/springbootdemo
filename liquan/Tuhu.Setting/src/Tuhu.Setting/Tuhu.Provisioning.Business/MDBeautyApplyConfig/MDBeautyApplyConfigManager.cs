using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class MDBeautyApplyConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("MDBeautyApplyConfig");

        public List<MDBeautyApplyConfig> GetMDBeautyApplyConfigList(MDBeautyApplyConfig model, int pageSize, int pageIndex, string type, out int recordCount)
        {
            try
            {
                return DALMDBeautyApplyConfig.GetMDBeautyApplyConfig(model, pageSize, pageIndex, type, out recordCount);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetMDBeautyApplyConfigList");
                throw ex;
            }
        }

        public MDBeautyApplyConfig GetMDBeautyApplyConfigById(int id)
        {
            try
            {
                return DALMDBeautyApplyConfig.GetMDBeautyApplyConfigById(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetMDBeautyApplyConfigById");
                throw ex;
            }
        }

        public bool UpdateMDBeautyApplyConfig(MDBeautyApplyConfig model, string type)
        {
            try
            {
                return DALMDBeautyApplyConfig.UpdateMDBeautyApplyConfig(model, type);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "UpdateMDBeautyApplyConfig");
                throw ex;
            }

        }


        public bool AuditMDBeautyApplyConfig(MDBeautyApplyConfig model, string type)
        {
            try
            {
                return DALMDBeautyApplyConfig.AuditMDBeautyApplyConfig(model, type);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "AuditMDBeautyApplyConfig");
                throw ex;
            }

        }

        public bool DeleteMDBeautyApplyConfig(int id)
        {
            try
            {
                return DALMDBeautyApplyConfig.DeleteMDBeautyApplyConfig(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "DeleteMDBeautyApplyConfig");
                throw ex;
            }
        }
    }
}
