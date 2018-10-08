using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class MeiRongAcitivityConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("MeiRongAcitivityConfig");

        public List<MeiRongAcitivityConfig> GetMeiRongAcitivityConfigList(MeiRongAcitivityConfig model, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALMeiRongAcitivityConfig.GetMeiRongAcitivityConfig(model, pageSize, pageIndex, out recordCount);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetMeiRongAcitivityConfigList");
                throw ex;
            }
        }
        public List<MeiRongAcitivityConfig> GetMeiRongAcitivityConfigList()
        {
            try
            {
                return DALMeiRongAcitivityConfig.GetMeiRongAcitivityConfig();
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetMeiRongAcitivityConfigList");
                throw ex;
            }
        }
        public MeiRongAcitivityConfig GetMeiRongAcitivityConfigById(int id)
        {
            try
            {
                return DALMeiRongAcitivityConfig.GetMeiRongAcitivityConfigById(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetMeiRongAcitivityConfigById");
                throw ex;
            }
        }

        public bool UpdateMeiRongAcitivityConfig(MeiRongAcitivityConfig model)
        {
            try
            {
                return DALMeiRongAcitivityConfig.UpdateMeiRongAcitivityConfig(model);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "UpdateMeiRongAcitivityConfig");
                throw ex;
            }

        }

        public bool InsertMeiRongAcitivityConfig(MeiRongAcitivityConfig model, ref int id)
        {
            try
            {
                return DALMeiRongAcitivityConfig.InsertMeiRongAcitivityConfig(model, ref id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "InsertMeiRongAcitivityConfig");
                throw ex;
            }
        }
        public bool DeleteMeiRongAcitivityConfig(int id)
        {
            try
            {
                return DALMeiRongAcitivityConfig.DeleteMeiRongAcitivityConfig(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "DeleteMeiRongAcitivityConfig");
                throw ex;
            }
        }

        public bool CompelStart(int id)
        {
            try
            {
                return DALMeiRongAcitivityConfig.CompelStart(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "CompelStart");
                throw ex;
            }
        }
        public List<Region> GetRegion(int id)
        {
            try
            {
                return DALVehicleMountedCouponConfig.GetRegion(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetRegion");
                throw ex;
            }

        }

        public List<Region> GetRegion(string  name)
        {
            try
            {
                return DALVehicleMountedCouponConfig.GetRegion(name);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetRegion");
                throw ex;
            }

        }

        public List<ShopCosmetologyServers> GetShopCosmetologyServers(int id = 0)
        {
            try
            {
                return DALVehicleMountedCouponConfig.GetShopCosmetologyServers(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetShopCosmetologyServers");
                throw ex;
            }

        }
        public List<RegionRelation> GetRegion(int id, int type)
        {
            try
            {
                return DALMeiRongAcitivityConfig.GetRegion(id, type);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetRegionRelation");
                throw ex;
            }

        }
        public List<RegionRelation> GetRegionRelation(int id, int type)
        {
            try
            {
                return DALMeiRongAcitivityConfig.GetRegionRelation(id, type);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetRegionRelation");
                throw ex;
            }

        }

        public List<RegionRelation> GetRegionRelation(int pid, int id, int type)
        {
            try
            {
                return DALMeiRongAcitivityConfig.GetRegionRelation(pid, id, type);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetRegionRelation");
                throw ex;
            }

        }

        public List<ShopServiceRelation> GetShopServiceRelation(int id, int type, int catogryId)
        {
            try
            {
                return DALMeiRongAcitivityConfig.GetShopServiceRelation(id, type, catogryId);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetShopServiceRelation");
                throw ex;
            }

        }

        public bool DeleteRegionRelation(int id)
        {
            try
            {
                return true;// DALMeiRongAcitivityConfig.DeleteRegionRelation(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "DeleteRegionRelation");
                throw ex;
            }

        }

        public bool DeleteShopServiceRelation(int id)
        {
            try
            {
                return true;// DALMeiRongAcitivityConfig.DeleteShopServiceRelation(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "DeleteShopServiceRelation");
                throw ex;
            }

        }

        public bool InsertRegionRelation(RegionRelation model, ref int id)
        {
            try
            {
                return true;// DALMeiRongAcitivityConfig.InsertRegionRelation(model, ref id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "InsertRegionRelation");
                throw ex;
            }

        }
        public bool InsertShopServiceRelation(ShopServiceRelation model, ref int id)
        {
            try
            {
                return true;// DALMeiRongAcitivityConfig.InsertShopServiceRelation(model, ref id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "InsertShopServiceRelation");
                throw ex;
            }
        }

        public bool InsertShopNotificationRecord(ShopNotificationRecord model)
        {
            try
            {
                return DALMeiRongAcitivityConfig.InsertShopNotificationRecord(model);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "InsertShopNotificationRecord");
                throw ex;
            }
        }
    }
}
