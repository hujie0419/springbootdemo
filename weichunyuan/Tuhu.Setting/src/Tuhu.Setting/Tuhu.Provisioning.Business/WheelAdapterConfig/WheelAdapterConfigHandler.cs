using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.WheelAdapterConfig
{
    public class WheelAdapterConfigHandler
    {
        #region
        private readonly IDBScopeManager dbManager;
        public WheelAdapterConfigHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }
        #endregion
        public List<Str> SelectBrands()
        {
            Func<SqlConnection, List<Str>> action = (connection) => DalWheelAdapterConfig.SelectBrands(connection);
            return dbManager.Execute(action);
        }
        public List<Str> SelectVehiclesAndId(string brand)
        {
            Func<SqlConnection, List<Str>> action = (connection) => DalWheelAdapterConfig.SelectVehiclesAndId(connection, brand);
            return dbManager.Execute(action);
        }
        public List<Str> SelectPaiLiang(string vehicleid)
        {
            Func<SqlConnection, List<Str>> action = (connection) => DalWheelAdapterConfig.SelectPaiLiang(connection, vehicleid);
            return dbManager.Execute(action);
        }
        public List<Str> SelectYear(string vehicleid, string pailiang)
        {
            Func<SqlConnection, List<Str>> action = (connection) => DalWheelAdapterConfig.SelectYear(connection, vehicleid, pailiang);
            return dbManager.Execute(action);
        }
        public List<Str> SelectNianAndSalesName(string vehicleid, string pailiang, string year)
        {
            Func<SqlConnection, List<Str>> action = (connection) => DalWheelAdapterConfig.SelectNianAndSalesName(connection, vehicleid, pailiang, year);
            return dbManager.Execute(action);
        }
        public List<VehicleTypeInfo> QueryVehicleTypeInfo(WheelAdapterConfigQuery query)
        {
            Func<SqlConnection, List<VehicleTypeInfo>> action = (connection) => DalWheelAdapterConfig.QueryVehicleTypeInfo(connection, query);
            return dbManager.Execute(action);
        }
        public List<VehicleTypeInfo> QueryVehicleTypeInfoByTID(WheelAdapterConfigQuery query)
        {
            Func<SqlConnection, List<VehicleTypeInfo>> action = (connection) => DalWheelAdapterConfig.QueryVehicleTypeInfoByTID(connection, query);
            return dbManager.Execute(action);
        }
        public List<WheelAdapterConfigWithTid> SelectWheelAdapterConfigWithTid(string tid)
        {
            Func<SqlConnection, List<WheelAdapterConfigWithTid>> action = (connection) => DalWheelAdapterConfig.SelectWheelAdapterConfigWithTid(connection, tid);
            return dbManager.Execute(action);
        }
        public bool InsertWheelAdapterConfigWithTid(WheelAdapterConfigWithTid wac)
        {
            Func<SqlConnection, bool> action = (connection) => DalWheelAdapterConfig.InsertWheelAdapterConfigWithTid(connection, wac);
            return dbManager.Execute(action);
        }
        public bool InsertWheelAdapterConfigWithTid(WheelAdapterConfigWithTid wac,IEnumerable<string> tids)
        {
            Func<SqlConnection, bool> action = (connection) => DalWheelAdapterConfig.InsertWheelAdapterConfigWithTid(connection, wac,tids);
            return dbManager.Execute(action);
        }
        public bool UpdateWheelAdapterConfigWithTid(WheelAdapterConfigWithTid wac)
        {
            Func<SqlConnection, bool> action = (connection) => DalWheelAdapterConfig.UpdateWheelAdapterConfigWithTid(connection, wac);
            return dbManager.Execute(action);
        }
        public bool InsertWheelAdapterConfigLog(WheelAdapterConfigLog wacl)
        {
            Func<SqlConnection, bool> action = (connection) => DalWheelAdapterConfig.InsertWheelAdapterConfigLog(connection, wacl);
            return dbManager.Execute(action);
        }
        public List<WheelAdapterConfigLog> SelectWheelAdapterConfigLog(string tid)
        {
            Func<SqlConnection, List<WheelAdapterConfigLog>> action = (connection) => DalWheelAdapterConfig.SelectWheelAdapterConfigLog(connection, tid);
            return dbManager.Execute(action);
        }
    }
}
