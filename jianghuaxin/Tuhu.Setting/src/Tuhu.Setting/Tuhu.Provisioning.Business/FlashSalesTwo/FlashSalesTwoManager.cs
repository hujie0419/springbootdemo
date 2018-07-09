using System.Collections.Generic;
using System.Configuration;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Component.Framework;

namespace Tuhu.Provisioning.Business
{
    public class FlashSalesTwoManager : IFlashSalesTwoManager
    {
        #region Private Fields & Ctor
        static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;
        private static readonly IConnectionManager connectionManager = new ConnectionManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);

        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(connectionManager);
        private static readonly ILog logger = LoggerFactory.GetLogger("FlashSales");

        private FlashSalesTwoHandler handler = null;
        public FlashSalesTwoManager()
        {
            handler = new FlashSalesTwoHandler(DbScopeManager);
        }
        #endregion


        public List<FlashSalesTwo> GetAllFlashSales()
        {
            return handler.GetAllFlashSales();
        }

        public List<FlashSalesTwo> GetAllFlashSalesV1(string sqlStr, int pageSize, int pageIndex, out int recordCount)
        {
            return handler.GetAllFlashSalesV1(sqlStr, pageSize, pageIndex, out recordCount);
        }
        public List<FlashSalesProductTwo> GetProListByFlashSalesID(string activityid)
        {
            return handler.GetProListByFlashSalesID(activityid);
        }

        public FlashSalesTwo GetFlashSalesByID(string activityid)
        {
            return handler.GetFlashSalesByID(activityid);
        }

        public string GetCountByFlashSalesID(string activityid)
        {
            return handler.GetCountByFlashSalesID(activityid);
        }

        public void UpdateFlashSalesTwo(FlashSalesTwo flashSales)
        {
            handler.UpdateFlashSalesTwo(flashSales);
        }
        public void UpdateFlashSalesTwoV1(FlashSalesTwo flashSales)
        {
            handler.UpdateFlashSalesTwoV1(flashSales);
        }

        public int SaveProductTwo(FlashSalesProductTwo flashsalesproducttwo)
        {
            return handler.SaveProductTwo(flashsalesproducttwo);
        }

        public FlashSalesTwo GetActivityByModelfloor(int modelfloor)
        {
            return handler.GetActivityByModelfloor(modelfloor);
        }
    }
}
