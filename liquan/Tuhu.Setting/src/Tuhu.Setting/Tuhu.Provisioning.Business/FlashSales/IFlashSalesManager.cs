using System.Collections.Generic;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business
{
    public interface IFlashSalesManager
    {
        #region FlashSales
        List<FlashSales> GetAllFlashSales();
        void DeleteFlashSales(int id);
        void AddFlashSales(FlashSales flashSales);
        void UpdateFlashSales(FlashSales flashSales);
        FlashSales GetFlashSalesByID(int id);
        void ResetFlashSales(int id);
        #endregion

        #region FlashSalesProduct
        List<FlashSalesProduct> GetProListByFlashSalesID(int FlashSalesID);
        string GetCountByFlashSalesID(int FlashSalesID);
        void DeleteFlashSalesProduct(int PKID);
        void ChangeStatus(int PKID, byte Status);
        void ChangeIsHotSale(int PKID, bool IsHotSale);
        void UpdateFlashSalesProduct(FlashSalesProduct flashSalesProduct);
        void AddFlashSalesProduct(FlashSalesProduct flashSalesProduct);
        #endregion
    }
}
