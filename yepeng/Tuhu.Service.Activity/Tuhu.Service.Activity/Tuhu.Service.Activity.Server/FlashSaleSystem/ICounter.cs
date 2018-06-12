using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Activity.Server.Manager.FlashSaleSystem;

namespace Tuhu.Service.Activity.Server.FlashSaleSystem
{
    public interface ICounter
    {
        Task<ResultModel<string>> CanPurchaseAndIncreaseCount(Guid userId, string deviceId, string userTel);

        Task<bool> DecreasePurchaseCount(Guid userId, string deviceId, string userTel);

        Task<bool> AddOrderRecord(int orderId, Guid userId, string deviceId, string userTel);
    }
}
