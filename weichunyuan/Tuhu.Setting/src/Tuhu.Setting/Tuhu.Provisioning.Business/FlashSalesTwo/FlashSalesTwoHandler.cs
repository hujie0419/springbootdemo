using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public class FlashSalesTwoHandler
    {
        #region Private Fields
        private readonly IDBScopeManager dbManager;
        #endregion

        #region Ctor
        public FlashSalesTwoHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }
        #endregion

        #region FlashSales
        public List<FlashSalesTwo> GetAllFlashSales()
        {
            Func<SqlConnection, List<FlashSalesTwo>> action = (connection) => DalFlashSalesTwo.GetAllFlashSales(connection);
            return dbManager.Execute(action);
        }

        public List<FlashSalesTwo> GetAllFlashSalesV1(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
            strConn = SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn;
            SqlConnection conn = new SqlConnection(strConn);
            return DalFlashSalesTwo.GetAllFlashSalesV1(conn, sqlStr, pageSize, pageIndex, out recordCount);
        }
        #endregion

        //根据pkid查询所对于的产品
        public List<FlashSalesProductTwo> GetProListByFlashSalesID(string activityid)
        {
            Func<SqlConnection, List<FlashSalesProductTwo>> action = (connection) => DalFlashSalesTwo.GetProListByFlashSalesID(connection, activityid);
            return dbManager.Execute(action);
        }

        public FlashSalesTwo GetFlashSalesByID(string activityid)
        {
            Func<SqlConnection, FlashSalesTwo> action = (connection) => DalFlashSalesTwo.GetFlashSalesByID(connection, activityid);
            return dbManager.Execute(action);
        }

        public string GetCountByFlashSalesID(string activityid)
        {
            Func<SqlConnection, string> action = (connection) => DalFlashSalesTwo.GetCountByFlashSalesID(connection, activityid);
            return dbManager.Execute(action);
        }

        public void UpdateFlashSalesTwo(FlashSalesTwo flashSales)
        {
            Action<SqlConnection> action = (connection) => DalFlashSalesTwo.UpdateFlashSalesTwo(connection, flashSales);
            dbManager.Execute(action);
        }

        public void UpdateFlashSalesTwoV1(FlashSalesTwo flashSales)
        {
            Action<SqlConnection> action = (connection) => DalFlashSalesTwo.UpdateFlashSalesTwoV1(connection, flashSales);
            dbManager.Execute(action);
        }
        //更改模块下面的产品
        public int SaveProductTwo(FlashSalesProductTwo flashsalesproducttwo)
        {
            Func<SqlConnection, int> action = (connection) => DalFlashSalesTwo.SaveProductTwo(connection, flashsalesproducttwo);
            return dbManager.Execute(action);
        }

        public FlashSalesTwo GetActivityByModelfloor(int modelfloor)
        {
            Func<SqlConnection, FlashSalesTwo> action = (connection) => DalFlashSalesTwo.GetActivityByModelfloor(connection, modelfloor);
            return dbManager.Execute(action);
        }
    }
}
