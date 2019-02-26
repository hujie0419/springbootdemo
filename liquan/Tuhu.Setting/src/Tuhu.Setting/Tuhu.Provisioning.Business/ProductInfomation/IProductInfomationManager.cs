using System.Collections.Generic;
using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.ProductInfomationManagement
{
    public interface IProductInfomationManager
    {
        List<ProductInformation> GetAllProductInfomation(Dictionary<string, string>.KeyCollection pricenNameList, Dictionary<string, string>.KeyCollection stockNameList);
        List<ProductInfo_Order> GetProductInfo_Order(string PIDS, string OrderChannel);
        List<ProductInfo_Order> GetProductInfo_Order(string PIDS, string OrderChannel, string fromRegion);
        void UpdateCommission(string PID, float Commission);
        void UpdatePrice(string PID, float Price, string ShopCode);
        List<ProductUser> GetProductUser(string UserPhone, string UserTel, string TaoBaoID, string CarNO);
        List<BizShopSimple> GetShopSimple();
        int SaveOrder(BizOrder bizOrder);
        void SaveOrderNo(int PKID, string OrderNo);
        void SaveOrderItem(OrderListProduct orderListProduct);
        long PlatFormItemID(string PID, string OrderChannel);
        void SaveOrderSumPrice(int PKID, decimal SumMarkedMoney, int SumNum, decimal SumDisMoney, decimal SumMoney);
        FlashSalesProductPara GetFlashSalesProductParaByPID(string PID);
        List<TireProductModel> GetTireProductByID(string ProductID, string VariantID);

    }
}
