using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.FlashSalesManagement
{
    public class FlashSalesHandler
    {
        #region Private Fields
        private readonly IDBScopeManager dbManager;
        #endregion

        #region Ctor
        public FlashSalesHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }
        #endregion


        #region FlashSales
        public List<FlashSales> GetAllFlashSales()
        {
            Func<SqlConnection, List<FlashSales>> action = (connection) => DalFlashSales.GetAllFlashSales(connection);
            return dbManager.Execute(action);
        }
        public void DeleteFlashSales(int id)
        {
            Action<SqlConnection> action = (connection) => DalFlashSales.DeleteFlashSales(connection, id);
            dbManager.Execute(action);
        }
        public void AddFlashSales(FlashSales flashSales)
        {
            Action<SqlConnection> action = (connection) => DalFlashSales.AddFlashSales(connection, flashSales);
            dbManager.Execute(action);
        }
        public void UpdateFlashSales(FlashSales flashSales)
        {
            Action<SqlConnection> action = (connection) => DalFlashSales.UpdateFlashSales(connection, flashSales);
            dbManager.Execute(action);
        }
        public FlashSales GetFlashSalesByID(int id)
        {
            Func<SqlConnection, FlashSales> action = (connection) => DalFlashSales.GetFlashSalesByID(connection, id);
            return dbManager.Execute(action);
        }
        public void ResetFlashSales(int id)
        {
            Action<SqlConnection> action = (connection) => DalFlashSales.ResetFlashSales(connection, id);
            dbManager.Execute(action);
        }
        #endregion


        #region FlashSalesProduct
        public List<FlashSalesProduct> GetProListByFlashSalesID(int FlashSalesID)
        {
            Func<SqlConnection, List<FlashSalesProduct>> action = (connection) => DalFlashSales.GetProListByFlashSalesID(connection, FlashSalesID);
            return dbManager.Execute(action);
        }

        public string GetCountByFlashSalesID(int FlashSalesID)
        {
            Func<SqlConnection, string> action = (connection) => DalFlashSales.GetCountByFlashSalesID(connection, FlashSalesID);
            return dbManager.Execute(action);
        }
        public void DeleteFlashSalesProduct(int PKID)
        {
            Action<SqlConnection> action = (connection) => DalFlashSales.DeleteFlashSalesProduct(connection, PKID);
            dbManager.Execute(action);
        }
        public void ChangeStatus(int PKID, byte Status)
        {
            Action<SqlConnection> action = (connection) => DalFlashSales.ChangeStatus(connection, PKID, Status);
            dbManager.Execute(action);
        }

        public void ChangeIsHotSale(int PKID, bool IsHotSale)
        {
            Action<SqlConnection> action = (connection) => DalFlashSales.ChangeIsHotSale(connection, PKID, IsHotSale);
            dbManager.Execute(action);
        }
        public void UpdateFlashSalesProduct(FlashSalesProduct flashSalesProduct)
        {
            Action<SqlConnection> action = (connection) => DalFlashSales.UpdateFlashSalesProduct(connection, flashSalesProduct);
            dbManager.Execute(action);
        }
        public void AddFlashSalesProduct(FlashSalesProduct flashSalesProduct)
        {
            Action<SqlConnection> action = (connection) => DalFlashSales.AddFlashSalesProduct(connection, flashSalesProduct);
            dbManager.Execute(action);
        }
        #endregion
    }
}
