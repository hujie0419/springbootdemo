using System;
using System.Collections.Generic;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class VehicleMountedCouponConfigManager
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("VehicleMountedCouponConfig");

        public List<tbl_OrderModel> JiayouCard(string startTime, string endTime, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALVehicleMountedCouponConfig.JiayouCard(startTime, endTime, pageSize, pageIndex, out recordCount);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "JiayouCard");
                throw ex;
            }
        }

        public List<VehicleMountedCouponConfig> GetVehicleMountedCouponConfigList(VehicleMountedCouponConfig model, int pageSize, int pageIndex, out int recordCount)
        {
            try
            {
                return DALVehicleMountedCouponConfig.GetVehicleMountedCouponConfig(model, pageSize, pageIndex, out recordCount);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetVehicleMountedCouponConfigList");
                throw ex;
            }
        }

        public VehicleMountedCouponConfig GetVehicleMountedCouponConfigById(int id)
        {
            try
            {
                return DALVehicleMountedCouponConfig.GetVehicleMountedCouponConfigById(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "GetVehicleMountedCouponConfigById");
                throw ex;
            }
        }

        public bool UpdateVehicleMountedCouponConfig(VehicleMountedCouponConfig model)
        {
            try
            {
                return DALVehicleMountedCouponConfig.UpdateVehicleMountedCouponConfig(model);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "UpdateVehicleMountedCouponConfig");
                throw ex;
            }

        }

        public bool InsertVehicleMountedCouponConfig(VehicleMountedCouponConfig model, ref int id)
        {
            try
            {
                return DALVehicleMountedCouponConfig.InsertVehicleMountedCouponConfig(model, ref id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "InsertVehicleMountedCouponConfig");
                throw ex;
            }
        }
        public bool DeleteVehicleMountedCouponConfig(int id)
        {
            try
            {
                return DALVehicleMountedCouponConfig.DeleteVehicleMountedCouponConfig(id);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "DeleteVehicleMountedCouponConfig");
                throw ex;
            }
        }
    }
}
