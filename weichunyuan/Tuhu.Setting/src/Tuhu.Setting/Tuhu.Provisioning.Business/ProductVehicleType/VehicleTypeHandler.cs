using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO.ProductVehicleInfoDao;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.ProductVehicleType
{
    class VehicleTypeHandler
    {
        private readonly IDBScopeManager dbManager;

        public VehicleTypeHandler(IDBScopeManager dbMgr)
        {
            this.dbManager = dbMgr;
        }

        public List<VehicleTypeInfoDb> GetVehicleTypeInfoByCharacter(string alpha)
        {
            Func<SqlConnection, List<VehicleTypeInfoDb>> action = (connection) => DalProductVehicleInfo.GetVehicleTypeInfoByCharacter(connection, alpha);
            return dbManager.Execute(action);
        }

        public List<VehicleTypeInfoDb> GetVehicleTypeInfoByBrandName(string brandNames)
        {
            Func<SqlConnection, List<VehicleTypeInfoDb>> action = (connection) => DalProductVehicleInfo.GetVehicleTypeInfoByBrandName(connection, brandNames);
            return dbManager.Execute(action);
        }

        public VehicleInfoExDb GetVehicleInfoExByTid(string tid)
        {
            Func<SqlConnection, VehicleInfoExDb> action = (connection) => DalProductVehicleInfo.GetVehicleInfoExByTid(connection, tid);
            return dbManager.Execute(action);
        }

        public List<VehicleInfoExDb> GetVehicleInfoExByVehicleId(string vehicleId)
        {
            Func<SqlConnection, List<VehicleInfoExDb>> action = (connection) => DalProductVehicleInfo.GetVehicleInfoExByVehicleId(connection, vehicleId);
            return dbManager.Execute(action);
        }

        public List<VehicleInfoExDb> GetVehicleInfoExByPid(string pid, string level)
        {
            Func<SqlConnection, List<VehicleInfoExDb>> action = (connection) => DalProductVehicleInfo.GetVehicleInfoExByPid(connection, pid, level);
            return dbManager.Execute(action);
        }

        public List<VehicleInfoExDb> GetVehicleInfoEx()
        {
            Func<SqlConnection, List<VehicleInfoExDb>> action = (connection) => DalProductVehicleInfo.GetVehicleInfoEx(connection);
            return dbManager.Execute(action);
        }

        public List<string> GetVehicleBrandCategory()
        {
            Func<SqlConnection, List<string>> action =
                (connection) => DalProductVehicleInfo.GetVehicleBrandCategory(connection);
            return dbManager.Execute(action);
        }

        public List<string> GetVehicleBodyType()
        {
            Func<SqlConnection, List<string>> action =
                (connection) => DalProductVehicleInfo.GetVehicleBodyType(connection);
            return dbManager.Execute(action);
        }
    }
}
