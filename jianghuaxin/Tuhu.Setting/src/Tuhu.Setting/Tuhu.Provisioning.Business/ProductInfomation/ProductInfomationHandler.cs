using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.ProductInfomationManagement
{
    public class ProductInfomationHandler
	{
		#region Private Fields
		private readonly IDBScopeManager dbManager;
		#endregion

		#region Ctor
		public ProductInfomationHandler(IDBScopeManager dbManager)
		{
			this.dbManager = dbManager;
		}
		#endregion
		public List<ProductInformation> GetAllProductInfomation(Dictionary<string, string>.KeyCollection pricenNameList, Dictionary<string, string>.KeyCollection stockNameList)
		{
			Func<SqlConnection, List<ProductInformation>> action = (connection) => DalProductInfomation.GetAllProductInfomation(connection, pricenNameList, stockNameList);
			return dbManager.Execute(action);
		}
		public List<ProductInfo_Order> GetProductInfo_Order(string PIDS, string OrderChannel)
		{
			Func<SqlConnection, List<ProductInfo_Order>> action = (connection) => DalProductInfomation.GetProductInfo_Order(connection, PIDS, OrderChannel);
			return dbManager.Execute(action);
		}
        public List<ProductInfo_Order> GetProductInfo_Order(string PIDS, string OrderChannel,string fromRegion)
        {
            Func<SqlConnection, List<ProductInfo_Order>> action = (connection) => DalProductInfomation.GetProductInfo_Order(connection, PIDS, OrderChannel, fromRegion);
            return dbManager.Execute(action);
        }
		public void UpdateCommission(string PID, float Commission)
		{
			Action<SqlConnection> action = (connection) => DalProductInfomation.UpdateCommission(connection, PID, Commission);
			dbManager.Execute(action);
		}
		public void UpdatePrice(string PID, float Price, string ShopCode)
		{
			Action<SqlConnection> action = (connection) => DalProductInfomation.UpdatePrice(connection, PID, Price, ShopCode);
			dbManager.Execute(action);
		}
		public List<ProductUser> GetProductUser(string UserPhone, string UserTel, string TaoBaoID, string CarNO)
		{
			Func<SqlConnection, List<ProductUser>> action = (connection) => DalProductInfomation.GetProductUser(connection, UserPhone, UserTel, TaoBaoID, CarNO);
			return dbManager.Execute(action);
		}
		public List<BizShopSimple> GetShopSimple()
		{
			Func<SqlConnection, List<BizShopSimple>> action = (connection) => DalProductInfomation.GetShopSimple(connection);
			return dbManager.Execute(action);
		}
		public int SaveOrder(BizOrder bizOrder)
		{
			Func<SqlConnection, int> action = (connection) => DalProductInfomation.SaveOrder(connection, bizOrder);
			return dbManager.Execute(action);
		}
		public void SaveOrderNo(int PKID, string OrderNo)
		{
			Action<SqlConnection> action = (connection) => DalProductInfomation.SaveOrderNo(connection, PKID, OrderNo);
			dbManager.Execute(action);
		}
		public void SaveOrderItem(OrderListProduct orderListProduct)
		{
			Action<SqlConnection> action = (connection) => DalProductInfomation.SaveOrderItem(connection, orderListProduct);
			dbManager.Execute(action);
		}

		public long PlatFormItemID(string PID, string OrderChannel)
		{
			Func<SqlConnection, long> action = (connection) => DalProductInfomation.PlatFormItemID(connection, PID, OrderChannel);
			return dbManager.Execute(action);
		}
		public void SaveOrderSumPrice(int PKID, decimal SumMarkedMoney, int SumNum, decimal SumDisMoney, decimal SumMoney)
		{
			Action<SqlConnection> action = (connection) => DalProductInfomation.SaveOrderSumPrice(connection, PKID, SumMarkedMoney, SumNum, SumDisMoney, SumMoney);
			dbManager.Execute(action);
		}

        public FlashSalesProductPara GetFlashSalesProductParaByPID(string PID)
        {
            Func<SqlConnection, FlashSalesProductPara> action = (connection) => DalFlashSales.GetFlashSalesProductParaByPID(connection, PID);
            return dbManager.Execute(action);
        }

        public List<string> SelectBaoYangPIDs()
        {
            return dbManager.Execute(conection => DalProductInfomation.SelectBaoyangPIDs(conection));
        }

        public List<ProductInformation> SelectBYPID()
        {
            return dbManager.Execute(conection => DalProductInfomation.SelectBYPID(conection));
        }

        public List<ProductSalesPrice> SelectProductSalesPrice()
        {
            return dbManager.Execute(conn => DalProductInfomation.SelectProductSalesPrice(conn));
        }

        public void UpdateOrderProductInfoCache(DataTable cacheOrderProductInfo)
        {
            dbManager.Execute(conn => DalProductInfomation.UpdateOrderProductInfoCache(conn, cacheOrderProductInfo));
        }

	    public string SelectBrandByPID(string PID)
	    {
	        return dbManager.Execute(conn => DalProductInfomation.SelectBrandByPID(conn, PID));
	    }

	    public List<SKUProductCategory> GetAllProductCategories(SqlConnection conn)
	    {
	        return DalProductCategory.SelectAllProductCategories(conn);
	    }

        /// <summary>
        /// 获取所有蓄电池的PID
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
	    public List<SKUPBatteryPID> GetAllBatteryPID(SqlConnection conn) 
	    {
            return DalProductCategory.SelectAllBatteryPID(conn);
        }     
        public List<Tuple<string, string, int>> GetSkuProductsAndStockQuantity(SqlConnection conn, string productPID,
	        string brand, string category)
	    {
	        return DalProductInfomation.SelectSkuProductsAndStockQuantity(conn, productPID, brand, category);
	    }

	    
	    public String GetNameById(SqlConnection conn, string pId)
	    {
	        return DalProductInfomation.GetNameById(conn, pId);

        }
	}
}
