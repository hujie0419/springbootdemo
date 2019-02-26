using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.Business.BaoYang;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Service.GroupBuying;
using Tuhu.Service.GroupBuying.Models.RedemptionCode;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Enum;
using Tuhu.Service.Order.Models;
using Tuhu.Service.Order.Request;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Enum;
using Tuhu.Service.Product.Models;
using Tuhu.Service.UserAccount;
using Tuhu.Service.UserAccount.Enums;
using Tuhu.Service.UserAccount.Models;

namespace Tuhu.Provisioning.Business.VipBaoYangPackage
{
    public class VipBaoYangPackageService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(VipBaoYangPackageService));

        /// <summary>
        /// 根据手机号获取用户
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static User GetUserByMobile(string phone)
        {
            User result = null;
            try
            {
                result = BaoYangExternalService.GetUserByMobile(phone);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// 创建套餐对应产品
        /// </summary>
        /// <param name="variantId"></param>
        /// <param name="displayName"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string CreateProduct(string variantId, string displayName, string user)
        {
            var result = string.Empty;
            try
            {
                WholeProductInfo info = new WholeProductInfo()
                {
                    ProductID = "BY-TUHU-BXGSDCBY",
                    VariantID = variantId,
                    CategoryName = "CarPAR",
                    DefinitionName = "Service",
                    PrimaryParentCategory = "BXGSDCBY",
                    Image_filename = "/Images/Products/bf46/d111/fbda108daa189c1f90cd0324_w800_h800.jpg",
                    CP_Tire_ROF = "非防爆",
                    invoice = "Yes",
                    CP_ShuXing3 = "商品安装服务",
                    stockout = false,
                    IsDaiFa = false,
                    DisplayName = displayName,
                    Description = "<p> < br /></ p > ",
                    CP_Brand = "途虎/Tuhu",
                    Name = "保险公司单次保养"
                };
                result = BaoYangExternalService.CreateProductV2(info, user, ChannelType.Tuhu);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return result;
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
        public static GenerateRedemptionCodeResult GenerateRedemptionCode(
            int quantity, DateTime startTime, DateTime endTime, string user)
        {
            var result = BaoYangExternalService.GenerateRedemptionCode(new GenerateRedemptionCodeRequest
            {
                CreateUser = user,
                EndTime = endTime,
                StartTime = startTime,
                Quantity = quantity,
                RedemptionCodeType = RedemptionCodeEnum.Type.SingleBaoYangPackage,
                Remark = "单次保养兑换码生成",
            });
            return result;
        }

        public static CreateOrderResult CreateOrder(decimal price, string pid, string name, Guid vipUserId,
            int num, string remark)
        {
            var vipUser = BaoYangExternalService.GetCompanyUserInfo(vipUserId);
            var companyId = vipUser.CompanyId == 0 ? (int?)null : vipUser.CompanyId;
            var companyName = string.Empty;
            if (companyId != null && vipUser.CompanyInfo != null)
            {
                if (vipUser.CompanyInfo.Id == companyId.Value)
                {
                    companyName = vipUser.CompanyInfo.Name;
                }
                else if (vipUser.CompanyInfo.ChildCompany != null)
                {
                    companyName = vipUser.CompanyInfo.ChildCompany.FirstOrDefault(c => c.Id == companyId.Value)?.Name;
                }
            }
            var createOrderRequest = new CreateOrderRequest
            {
                OrderChannel = "f大客户保养套餐",
                OrderType = "44大客户保养2B保批",
                Status = OrderEnum.OrderStatus.New,
                Customer = new OrderCustomer
                {
                    UserId = vipUser.UserId,
                    UserName = vipUser.UserName,
                    UserTel = vipUser.UserMobile,
                },
                Delivery = new OrderDelivery
                {
                    DeliveryStatus = OrderEnum.DeliveryStatus.Signed,
                    DeliveryType = OrderEnum.DeliveryType.NoDelivery,
                    InstallType = OrderEnum.InstallType.ShopInstall,
                },
                Payment = new OrderPayment
                {
                    PayStatus = OrderEnum.PayStatus.Waiting,
                    PayMothed = OrderEnum.PayMethod.MonthPay,
                    PaymentType = "5Special",
                },
                Money = new OrderMoney
                {
                    SumMoney = price * num,
                    SumMarkedMoney = price * num,
                },
                Items = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Price = price,
                            Pid = pid,
                            Num =  num,
                            Name = name,
                            Category = "BXGSDCBY"
                        }
                    },
                SumNum = num,
                Remark = remark,
                BigCustomerCompanyId = companyId,
                BigCustomerCompanyName = companyName,
            };
            CreateOrderResult result = null;
            try
            {
                result = BaoYangExternalService.CreateOrder(createOrderRequest);
            }
            catch (Exception ex)
            {
                Logger.Error($"创建订单失败, request:{{{JsonConvert.SerializeObject(createOrderRequest)}}}", ex);
            }
            return result;
        }

        public static bool ExecuteOrderProcess(ExecuteOrderProcessRequest request)
        {
            var result = false;
            try
            {
                var executeResult = BaoYangExternalService.ExecuteOrderProcess(request);
                result = string.IsNullOrEmpty(executeResult);
                if (!result)
                {
                    Logger.Warn($"更新订单{request.OrderId}失败, request=>{JsonConvert.SerializeObject(request)}result=>{executeResult}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"更新订单信息失败！request=>{JsonConvert.SerializeObject(request)}", ex);
            }
            return result;
        }

        public static bool InsertBaoYangOprLog(object data)
        {
            try
            {
                return LoggerManager.InsertLog("BYOprLog", data);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return false;
            }
        }
    }
}
