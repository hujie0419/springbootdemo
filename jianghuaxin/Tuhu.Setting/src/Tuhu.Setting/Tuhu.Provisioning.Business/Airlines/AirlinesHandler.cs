using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class AirlinesHandler
    {
        private readonly IDBScopeManager dbManager;

        public AirlinesHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }

        //获取客服信息
        public List<Airlines> GetAllAirlines()
        {
            Func<SqlConnection, List<Airlines>> action = (connection) => DalAirlines.GetAllAirlines(connection);
            return dbManager.Execute(action);
        }
        //添加客服信息
        public void AddAirlines(Airlines airlines)
        {
            Action<SqlConnection> action = (connection) => DalAirlines.AddAirlines(connection, airlines);
            dbManager.Execute(action);
        }
        //修改客服信息
        public void UpdateAirlines(Airlines airlines)
        {
            Action<SqlConnection> action = (connection) => DalAirlines.UpdateAirlines(connection, airlines);
            dbManager.Execute(action);
        }
        //删除客服信息
        public void DeleteAirlines(string Id)
        {
            Action<SqlConnection> action = (connection) => DalAirlines.DeleteAirlines(connection, Id);
            dbManager.Execute(action);
        }
        //根据ID获取客服信息
        public Airlines GetAirlinesByID(string Id)
        {
            Func<SqlConnection, Airlines> action = (connection) => DalAirlines.GetAirlinesByID(connection, Id);
            return dbManager.Execute(action);
        }



    }
}
