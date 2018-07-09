using System.Collections.Generic;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public interface IFlashSalesTwoManager
    {
        #region FlashSales
        List<FlashSalesTwo> GetAllFlashSales();

        List<FlashSalesTwo> GetAllFlashSalesV1(string sqlStr, int pageSize, int pageIndex, out int recordCount);
        #endregion

        FlashSalesTwo GetFlashSalesByID(string activityid);

        #region FlashSalesProduct
        List<FlashSalesProductTwo> GetProListByFlashSalesID(string activityid);
        #endregion

        string GetCountByFlashSalesID(string activityid);

        void UpdateFlashSalesTwo(FlashSalesTwo flashSales);
        void UpdateFlashSalesTwoV1(FlashSalesTwo flashSales);

        int SaveProductTwo(FlashSalesProductTwo flashsalesproducttwo);

        FlashSalesTwo GetActivityByModelfloor(int modelfloor);
    }
}
