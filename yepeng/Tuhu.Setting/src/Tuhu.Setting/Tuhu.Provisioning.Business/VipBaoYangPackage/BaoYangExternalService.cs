using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.GroupBuying;
using Tuhu.Service.GroupBuying.Models.RedemptionCode;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Models;
using Tuhu.Service.Order.Request;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Enum;
using Tuhu.Service.Product.Models;
using Tuhu.Service.UserAccount;
using Tuhu.Service.UserAccount.Models;

namespace Tuhu.Provisioning.Business.BaoYang
{
    internal class BaoYangExternalService
    {

        /// <summary>
        /// 根据手机号查询用户信息
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static User GetUserByMobile(string phone)
        {
            using (var client = new UserAccountClient())
            {
                var serviceResult = client.GetUserByMobile(phone);
                serviceResult.ThrowIfException(true);
                return serviceResult.Result;
            }
        }

        /// <summary>
        /// 修改产品
        /// </summary>
        /// <param name="info"></param>
        /// <param name="operatorName"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static string CreateProductV2(WholeProductInfo info, string operatorName, ChannelType channel)
        {
            using (var client = new ProductClient())
            {
                var serviceResult = client.CreateProductV2(info, operatorName, channel);
                serviceResult.ThrowIfException(true);
                return serviceResult.Result;
            }
        }

        /// <summary>
        /// 调用生成兑换码服务
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="quantity"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static GenerateRedemptionCodeResult GenerateRedemptionCode(GenerateRedemptionCodeRequest request)
        {
            using (var client = new RedemptionCodeClient())
            {
                var serviceResult = client.GenerateRedemptionCode(request);
                serviceResult.ThrowIfException(true);
                return serviceResult.Result;
            }
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static CreateOrderResult CreateOrder(CreateOrderRequest request)
        {
            if (request != null)
            {
                using (var client = new CreateOrderClient())
                {
                    var serviceResult = client.CreateOrder(request);
                    serviceResult.ThrowIfException(true);
                    return serviceResult.Result;
                }
            }
            return null;
        }

        public static string ExecuteOrderProcess(ExecuteOrderProcessRequest request)
        {
            if (request != null)
            {
                using (var client = new OrderOperationClient())
                {
                    var clientResult = client.ExecuteOrderProcess(request);
                    clientResult.ThrowIfException(true);
                    return clientResult.Result;
                }
            }
            return null;
        }

        /// <summary>
        /// 根据大客户Id获取大客户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static SYS_CompanyUser GetCompanyUserInfo(Guid userId)
        {
            using (var client = new Service.UserAccount.UserAccountClient())
            {
                var serviceResult = client.SelectCompanyUserInfo(userId);
                serviceResult.ThrowIfException(true);
                return serviceResult.Result;
            }
        }

    }
}
