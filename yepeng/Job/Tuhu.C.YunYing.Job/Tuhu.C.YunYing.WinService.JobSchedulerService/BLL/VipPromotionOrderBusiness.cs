using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Member;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Enum;
using Tuhu.Service.Order.Models;
using Tuhu.Service.Order.Request;
using Tuhu.Service.Order.Response;
using Tuhu.Service.UserAccount.Models;
using Tuhu.C.YunYing.WinService.JobSchedulerService.DAL;
using Tuhu.C.YunYing.WinService.JobSchedulerService.Model;
using Tuhu.C.YunYing.WinService.JobSchedulerService.Model.Enum;
using static Tuhu.C.YunYing.WinService.JobSchedulerService.Model.VipPromotionOrder;
using Tuhu.C.YunYing.WinService.JobSchedulerService.DLL;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BLL
{
    public class VipPromotionOrderBusiness
    {
        private static ILog Logger = LogManager.GetLogger(typeof(VipPromotionOrderBusiness));

        private const string OrderTypeBaoYang2C = "44大客户保养2C保";//大客户保养2C订单类型
        private const string OrderTypeBaoYang2BByPeriod = "44大客户保养2B保";//大客户保养2B据实订单类型
        private const string OrderTypeBaoYang2BPreSettled = "44大客户保养2B保批";//大客户保养2B买断订单类型
        private const string OrderTypePaint2C = "45大客户钣喷2C";//大客户喷漆ToC订单类型
        private const string OrderTypePaint2BPreSettled = "55大客户钣喷2B买断";//大客户喷漆ToB买断订单类型
        private const string OrderTypePaint2BByPeriod = "49大客户钣喷2B据实";//大客户喷漆ToB据实订单类型
        private const string OrderStatus3Installed = "3Installed";
        private const string OrderStatus7Canceled = "7Canceled";

        #region 据实已安装ToC订单关联2B

        /// <summary>
        /// 处理订单
        /// </summary>
        /// <param name="splitOrders"></param>
        public static void ProcessBatchOrders(string orderType, IEnumerable<OrderModel> splitOrders)
        {
            try
            {
                if (splitOrders == null || !splitOrders.Any() || string.IsNullOrEmpty(orderType))
                {
                    return;
                }
                var splitOrderIds = splitOrders.Select(x => x.OrderId).Distinct().ToList();
                var relationOrderIds = new List<int>();
                var relationToBOrderIds = new List<int>();
                //获取关联的ToB订单
                switch (orderType)
                {
                    case OrderTypeBaoYang2C:
                        {
                            relationOrderIds = GetRelationOrderIds(OrderRelationshipTypeEnum.DaKeHuBaoYang, splitOrderIds);//获取关联订单
                            relationToBOrderIds = GetOrderIdsForB(relationOrderIds, OrderTypeBaoYang2BByPeriod);//从关联订单中获取据实ToB订单
                            break;
                        }
                    case OrderTypePaint2C:
                        {
                            relationOrderIds = GetRelationOrderIds(OrderRelationshipTypeEnum.DaKeHuPenQi, splitOrderIds);//获取关联订单
                            relationToBOrderIds = GetOrderIdsForB(relationOrderIds, OrderTypePaint2BByPeriod);//从关联订单中获取据实ToB订单
                            break;
                        }
                }
                var installedOrder = splitOrders.FirstOrDefault(x => string.Equals(x.Status, OrderStatus3Installed, StringComparison.OrdinalIgnoreCase));//已安装订单
                if (installedOrder != null)
                {
                    if (!relationOrderIds.Any())
                    {
                        Create2B2COrderRelation(orderType, splitOrderIds);
                    }
                }
                else
                {
                    if (relationToBOrderIds.Any())
                    {
                        foreach (var orderId in relationToBOrderIds)
                        {
                            Logger.Info("2c订单未安装，将取消订单");
                            var cancelStatus = CancelOrder(orderId, "2c订单没有安装");
                            Logger.Info($"取消订单{orderId}{(cancelStatus ? "成功" : "失败")}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("ProcessBatchOrders", ex);
            }
        }

        /// <summary>
        /// 获取待处理订单
        /// </summary>
        /// <returns></returns>
        public static List<Tuple<string, CSplitOrderResponse>> GetSplitOrders()
        {
            var result = new List<Tuple<string, CSplitOrderResponse>>();
            try
            {
                var orders = GetOrderIdsForC();
                var orderIds = orders.Select(s => s.OrderId);
                var csplitOrderResList = InvokeServiceManager.GetSplitOrders(orderIds);
                foreach (var item in csplitOrderResList)
                {
                    if (!result.Any(x => x.Item2.SplitOrders.Any(o => o.OrderId == item.InputOrderID)))
                    {
                        var order = orders.FirstOrDefault(s => s.OrderId == item.InputOrderID);
                        if (order != null)
                        {
                            result.Add(Tuple.Create(order.OrderType, item));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("GetSplitOrders", ex);
            }
            return result;
        }

        #endregion

        #region 买断制生成ToB订单

        /// <summary>
        /// 创建保养买断制订单
        /// </summary>
        /// <param name="batchCode"></param>
        /// <returns></returns>
        public static bool CreateBaoYangBuyoutOrder(string batchCode)
        {
            bool success = false;
            var package = DalSingleBaoYang.SelectSingleBaoYangPackage(batchCode);
            if (package != null && package.SettlementMethod == SettlementMethod.PreSettled.ToString())
            {
                var total = DalSingleBaoYang.SelectPromotionDetailsTotal(batchCode);
                var count = DalSingleBaoYang.SelectPromotionDetailsCount(batchCode);
                if (total > 0 && total == count)
                {
                    var result = CreateBaoYangOrder(package.Price, package.PID, package.PackageName, package.VipUserId, OrderTypeBaoYang2BPreSettled, count, batchCode);
                    if (result != null && result.OrderId > 0)
                    {
                        success = DALVipBaoYangPackage.UpdatePromotionDetailToBOrder(batchCode, result.OrderId);
                        if (success)
                        {
                            Logger.Info($"创建买断制2B订单{result.OrderId}成功, 一共{count}个产品数量, 关联批次号:{batchCode}");
                        }
                        else
                        {
                            var cancel = CancelOrder(result.OrderId, "关联买断订单到大客户保养塞券记录失败");
                            Logger.Info($"更新塞券批次{batchCode}对应买断制2B订单失败," +
                                $"取消订单{result.OrderId}{(cancel ? "成功" : "失败")}");
                        }
                    }
                    else
                    {
                        Logger.Info($"创建买断制2B订单失败, 一共{count}个产品数量, 批次号:{batchCode}");
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// 创建喷漆买断ToB订单
        /// </summary>
        /// <param name="model"></param>
        /// <param name="num"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public static CreateOrderResult CreatePaintBuyoutOrder(VipPaintPackageConfig model, int num = 1, string remark = null)
        {
            var vipUser = InvokeServiceManager.GetCompanyUserInfo(model.VipUserId);
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
                OrderChannel = "d大客户喷漆套餐",
                OrderType = OrderTypePaint2BPreSettled,
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
                    SumMoney = model.PackagePrice * num,
                    SumMarkedMoney = model.PackagePrice * num,
                },
                Items = new List<OrderItem>()
                    {
                        new OrderItem()
                        {
                            Price = model.PackagePrice,
                            Pid = model.PackagePid,
                            Num =  num,
                            Name = model.PackageName,
                            Category = "DKHPQTC"
                        }
                    },
                SumNum = num,
                Remark = remark,
                BigCustomerCompanyId = companyId,
                BigCustomerCompanyName = companyName,
            };
            try
            {
                var result = InvokeServiceManager.CreateOrder(createOrderRequest);
                if (result != null && result.OrderId > 0)
                {
                    ExecuteOrderProcess(new ExecuteOrderProcessRequest()
                    {
                        OrderId = result.OrderId,
                        OrderProcessEnum = OrderProcessEnum.GeneralCompleteToHome,
                        CreateBy = vipUser.UserId.ToString()
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"创建订单失败, request:{{{JsonConvert.SerializeObject(createOrderRequest)}}}", ex);
                return null;
            }
        }

        #endregion

        #region **********Private Method**********

        /// <summary>
        /// 创建2B2C订单关联关系
        /// </summary>
        /// <param name="orderIds"></param>
        private static bool Create2B2COrderRelation(string orderType, IEnumerable<int> orderIds)
        {
            bool success = false;
            var result = null as CreateOrderResult;
            switch (orderType)
            {
                case OrderTypeBaoYang2C:
                    {
                        var packageWithPromotion = GetSingleBaoYangPackage(orderIds);
                        var promotionId = packageWithPromotion?.Item1 ?? 0;
                        var package = packageWithPromotion?.Item2;
                        if (package != null && package.SettlementMethod.Equals(SettlementMethod.ByPeriod.ToString()))
                        {
                            #region 据实2C订单创建对应的2B订单,并关联
                            result = CreateBaoYangOrder(package.Price, package.PID, package.PackageName, package.VipUserId, OrderTypeBaoYang2BByPeriod);
                            if (result != null && result.OrderId > 0)
                            {
                                success = InsertOrderRelationship(result.OrderId, orderIds, OrderRelationshipTypeEnum.DaKeHuBaoYang);
                                if (success)
                                {
                                    if (!string.IsNullOrWhiteSpace(package.RedemptionCode))
                                    {
                                        DalSingleBaoYang.UpdateBaoYangRedemptionCodeOrderId(package.RedemptionCode, result.OrderId);
                                    }
                                }
                            }
                            #endregion
                        }
                        else if (package != null && package.SettlementMethod.Equals(SettlementMethod.PreSettled.ToString())
                            && promotionId > 0)
                        {
                            #region 买断2C订单关联塞券时创建的2B订单
                            var preSettled2BOrderId = DalSingleBaoYang.GetPreSettled2BOrderIdByPromotionId(promotionId);
                            if (preSettled2BOrderId > 0)
                            {
                                success = InsertOrderRelationship(preSettled2BOrderId, orderIds, OrderRelationshipTypeEnum.DaKeHuBaoYang);
                                Logger.Info($"2B买断订单{preSettled2BOrderId}关联2C订单{string.Join(",", orderIds.OrderBy(x => x))}{(success ? "成功" : "失败")}");
                            }
                            #endregion
                        }
                        break;
                    }
                case OrderTypePaint2C:
                    {
                        var packageWithPromotion= GetVipPaintPackage(orderIds);
                        var promotionId = packageWithPromotion?.Item1 ?? 0;
                        var package = packageWithPromotion?.Item2;
                        if (package != null && package.SettlementMethod.Equals(SettlementMethod.ByPeriod.ToString()))
                        {
                            result = CreatePaintOrder(package.PackagePrice, package.PackagePid, package.PackageName, package.VipUserId);
                            if (result != null && result.OrderId > 0)
                            {
                                success = InsertOrderRelationship(result.OrderId, orderIds, OrderRelationshipTypeEnum.DaKeHuPenQi);
                            }
                        }
                        else if (package != null && package.SettlementMethod.Equals(SettlementMethod.PreSettled.ToString())
                            && promotionId > 0)
                        {
                            var preSettled2BOrderId = DALVipPaintPackage.GetPreSettled2BOrderIdByPromotionId(promotionId);
                            if (preSettled2BOrderId > 0)
                            {
                                success = InsertOrderRelationship(preSettled2BOrderId, orderIds, OrderRelationshipTypeEnum.DaKeHuPenQi);
                                Logger.Info($"2B买断订单{preSettled2BOrderId}关联2C订单{string.Join(",", orderIds.OrderBy(x => x))}{(success ? "成功" : "失败")}");
                            }
                        }
                        break;
                    }
                default: break;
            }
            if (result != null && result.OrderId > 0)
            {
                if (!success)
                {
                    Logger.Info($"创建2B据实订单{result.OrderId}成功,由于订单关联关系插入失败,订单将被取消");
                    var cancelStatus = CancelOrder(result.OrderId, "插入关联关系失败");
                    Logger.Info($"取消2B据实订单{result.OrderId}{(cancelStatus ? "成功" : "失败")}");
                }
                else
                {
                    Logger.Info($"创建2B据实订单{result.OrderId}成功, 关联2C订单{string.Join(",", orderIds.OrderBy(x => x))}成功");
                }
            }
            return success;
        }

        /// <summary>
        /// 创建保养2b订单
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        private static CreateOrderResult CreateBaoYangOrder(decimal price, string pid, string name, Guid vipUserId,
          string orderType, int num = 1, string remark = null)
        {
            var vipUser = InvokeServiceManager.GetCompanyUserInfo(vipUserId);
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
                OrderType = orderType,
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

            try
            {
                var result = InvokeServiceManager.CreateOrder(createOrderRequest);
                #region 更新订单信息为已安装
                if (result != null && result.OrderId > 0)
                {
                    ExecuteOrderProcess(new ExecuteOrderProcessRequest()
                    {
                        OrderId = result.OrderId,
                        OrderProcessEnum = OrderProcessEnum.GeneralCompleteToHome,
                        CreateBy = vipUser.UserId.ToString()
                    });
                }
                #endregion
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"创建订单失败, request:{{{JsonConvert.SerializeObject(createOrderRequest)}}}", ex);
                return null;
            }
        }

        /// <summary>
        /// 创建喷漆2B订单
        /// 关联已安装据实ToC订单
        /// </summary>
        /// <param name="price"></param>
        /// <param name="pid"></param>
        /// <param name="name"></param>
        /// <param name="vipUserId"></param>
        /// <param name="num"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        private static CreateOrderResult CreatePaintOrder(decimal price, string pid, string name, Guid vipUserId,
            int num = 1, string remark = null)
        {
            var vipUser = InvokeServiceManager.GetCompanyUserInfo(vipUserId);
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
                OrderChannel = " d大客户喷漆套餐",
                OrderType = OrderTypePaint2BByPeriod,
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
                            Category = "DKHPQTC"
                        }
                    },
                SumNum = num,
                Remark = remark,
                BigCustomerCompanyId = companyId,
                BigCustomerCompanyName = companyName,
            };
            try
            {
                var result = InvokeServiceManager.CreateOrder(createOrderRequest);
                if (result != null && result.OrderId > 0)
                {
                    ExecuteOrderProcess(new ExecuteOrderProcessRequest()
                    {
                        OrderId = result.OrderId,
                        OrderProcessEnum = OrderProcessEnum.GeneralCompleteToHome,
                        CreateBy = vipUser.UserId.ToString()
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"创建喷漆大客户据实2B订单失败, request:{{{JsonConvert.SerializeObject(createOrderRequest)}}}", ex);
                return null;
            }
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static bool CancelOrder(int orderId, string cancelReson)
        {
            bool success = false;
            int count = 3;
            var order = InvokeServiceManager.GetOrderById(orderId);
            if (order != null && order.Status != OrderStatus7Canceled)
            {
                while (count-- > 0 && !success)
                {
                    try
                    {
                        var rsp = InvokeServiceManager.CancelOrder(new CancelOrderRequest
                        {
                            OrderId = orderId,
                            UserID = order.UserId,
                            Remark = "用户自己取消",
                            FirstMenu = "用户",
                            SecondMenu = cancelReson,
                            OtherMenu = "",

                        });
                        success = rsp.IsSuccess;
                        if (!success)
                        {
                            Logger.Info($"取消订单{orderId}失败, {rsp.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message, ex);
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// 根据拆分订单获取关联订单
        /// </summary>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        private static List<int> GetRelationOrderIds(OrderRelationshipTypeEnum orderRelationshipType, IEnumerable<int> orderIds)
        {
            var result = new List<int>();
            foreach (var orderId in orderIds)
            {
                var relationshipIds = InvokeServiceManager.GetOrderRelationshipIds(orderRelationshipType, orderId)
                    .Where(x => x > 0 && !result.Contains(x)).ToList();
                result.AddRange(relationshipIds);
            }
            return result;
        }

        /// <summary>
        /// 大客户订单创建订单插入2b,2c订单关联关系
        /// </summary>
        /// <param name="orderIdNew"></param>
        /// <param name="orderIdOld"></param>
        /// <returns></returns>
        private static bool InsertOrderRelationship(int orderId2b, IEnumerable<int> orderIds, OrderRelationshipTypeEnum orderRelationshipType)
        {
            var success = false;
            foreach (var orderId in orderIds)
            {
                try
                {
                    var request = new OrderRelationshipRequest
                    {
                        ParentOrderId = orderId2b,
                        ChildOrderId = orderId,
                        RelationshipType = orderRelationshipType
                    };
                    var result = InvokeServiceManager.InsertOrderRelationship(request);
                    if (result > 0) success = true;
                    else Logger.Info($"关联订单失败, 2B订单{orderId2b}, 2C订单{orderId}");
                }
                catch (Exception ex)
                {
                    Logger.Info($"关联订单失败, 2B订单{orderId2b}, 2C订单{orderId},{ex.Message}, Exception:{{{JsonConvert.SerializeObject(ex)}}}");
                }
            }
            return success;
        }

        /// <summary>
        /// 获取2C订单
        /// </summary>
        /// <returns></returns>
        private static List<VipPromotionOrderModel> GetOrderIdsForC()
        {
            var endTime = DateTime.Now;
            var startTime = endTime.Date.AddDays(-7);
            var orderType = new List<string>()
            {
                OrderTypeBaoYang2C,
                OrderTypePaint2C
            };
            var orders = DalVipPromotionOrder.GetOrderIds(orderType, startTime, endTime).ToList();
            Logger.Info($"从{startTime.ToString("yyyy-MM-dd HH:mm:ss")}到{endTime.ToString("yyyy-MM-dd HH:mm:ss")}一共有{orders.Count}个订单状态更改");
            return orders;
        }

        /// <summary>
        /// 获取传入订单中的2B订单
        /// </summary>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        private static List<int> GetOrderIdsForB(IEnumerable<int> orderIds, string orderType)
        {
            List<int> result = null;
            orderIds = (orderIds ?? new List<int>()).Distinct();
            if (orderIds.Any())
            {
                result = DalVipPromotionOrder.GetOrderIds(orderIds, orderType).ToList();
            }
            return result ?? new List<int>();
        }

        /// <summary>
        /// 根据订单号获取单次保养套餐
        /// </summary>
        /// <param name="promotionCode"></param>
        /// <returns></returns>
        private static Tuple<long,SingleBaoYangPackage> GetSingleBaoYangPackage(IEnumerable<int> orderIds)
        {
            foreach (var orderId in orderIds)
            {
                var promotionId = InvokeServiceManager.GetPromotionIdByOrderId(orderId);
                var package = DalSingleBaoYang.SelectSingleBaoYangPackage(promotionId);
                if (package != null && !string.IsNullOrEmpty(package.SettlementMethod))
                {
                    return Tuple.Create(promotionId, package);
                }
            }
            return null;
        }

        /// <summary>
        /// 根据订单号获取喷漆大客户配置
        /// </summary>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        private static Tuple<long, VipPaintPackageConfig> GetVipPaintPackage(IEnumerable<int> orderIds)
        {
            foreach (var orderId in orderIds)
            {
                var promotionId = InvokeServiceManager.GetPromotionIdByOrderId(orderId);
                var package = DALVipPaintPackage.GetVipPaintPackageConfig(promotionId);
                if (package != null && !string.IsNullOrEmpty(package.SettlementMethod))
                {
                    return Tuple.Create(promotionId, package);
                }
            }
            return null;
        }

        /// <summary>
        /// 更新订单状态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static bool ExecuteOrderProcess(ExecuteOrderProcessRequest request)
        {
            var result = false;
            try
            {
                var executeResult = InvokeServiceManager.ExecuteOrderProcess(request);
                result = string.IsNullOrEmpty(executeResult);
                if (!result)
                {
                    Logger.Warn($"更新订单{request.OrderId}失败," +
                        $"request=>{JsonConvert.SerializeObject(request)}result=>{executeResult}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"更新订单信息失败！request=>{JsonConvert.SerializeObject(request)}", ex);
            }
            return result;
        }
        #endregion

    }
}
