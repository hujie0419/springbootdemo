using Tuhu.Provisioning.DataAccess.Entity;

namespace Tuhu.Provisioning.Business.OrderTrackingLog
{
    public interface IOrderTrackingLogManager
    {
        void AddOrderTrackingLog(int orderid, string orderStatus, string deliveryStatus, string installStatus, string logisticTaskStatus, DescriptionType descriptionType);

    }
}
