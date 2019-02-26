using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Tuhu.Models;
using Tuhu.Nosql;
using Tuhu.Service.Activity.DataAccess;
using Tuhu.Service.Activity.DataAccess.Models;
using Tuhu.Service.Activity.DataAccess.Models.Activity;
using Tuhu.Service.Activity.Enum;
using Tuhu.Service.Activity.Models;
using Tuhu.Service.Activity.Models.Requests;
using Tuhu.Service.Activity.Models.Response;
using Tuhu.Service.Activity.Server.Config;
using Tuhu.Service.Activity.Server.Model;
using Tuhu.Service.Activity.Server.Utils;
using Tuhu.Service.BaoYang;
using Tuhu.Service.Config.Models;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Models;
using Tuhu.Service.Product;
using Tuhu.Service.Product.Enum;
using Tuhu.Service.Product.Models;
using Tuhu.Service.Product.Models.New;
using Tuhu.Service.Product.Models.ProductConfig;
using Tuhu.Service.Product.Request;
using Tuhu.Service.UserAccount;
using Tuhu.ZooKeeper;
using FlashSaleProductModel = Tuhu.Service.Activity.Models.FlashSaleProductModel;
using static Tuhu.Service.Order.Enum.OrderEnum;
using Tuhu.Service.Order.Enum;
using Tuhu.Service.Push;
using Tuhu.Service.Push.Models.Push;
using Tuhu.Service.Activity.DataAccess.Questionnaire;
using Tuhu.Service.Activity.Models.Questionnaire;
using Tuhu.Service.Member;
using Tuhu.Service.Member.Models;
using Tuhu.MessageQueue;
using Tuhu.Service.Activity.Models.Requests.Activity;
using Tuhu.Service.Activity.DataAccess.Models;
using Tuhu.Service.Activity.Server.Enum;
using Tuhu.Service.Activity.Server.ServiceProxy;
using Tuhu.Service.Pay;
using Tuhu.Service.Pay.Models;
using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace Tuhu.Service.Activity.Server.Manager
{
    public static class TuboAllianceManager
    {
        public static readonly string DefaultClientName = "TuboAlliance";
        public static readonly ILog Logger = LogManager.GetLogger(typeof(TuboAllianceManager));


        /// <summary>
        /// 佣金商品列表查询接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<List<CommissionProductModel>> GetCommissionProductListManager(GetCommissionProductListRequest request)
        {
            List<CommissionProductModel> resultList = null;

            try
            {
                //通用缓存Key
                var prefix = await CacheManager.CommonGetKeyPrefixAsync(DefaultClientName,
                    GlobalConstant.CommissionCacheName);

                using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
                {
                    var cacheResult = await client.GetOrSetAsync($"{prefix}/{request.pageIndex}/{request.pageSize}",
                        () => DalTuboAlliance.GetCommissionProductListDal(request), GlobalConstant.CommissionExpiration);

                    if (cacheResult.Success)
                    {
                        resultList = cacheResult.Value?.ToList();
                    }
                    else
                    {
                        Logger.Error($"GetCommissionProductListManager佣金商品列表查询Redis执行异常");
                        resultList = await DalTuboAlliance.GetCommissionProductListDal(request);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"GetCommissionProductListManager佣金商品列表查询接口异常:{ex.Message};堆栈信息:{ex.StackTrace}");
            }

            return resultList;
        }


        /// <summary>
        /// 佣金商品详情查询接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<CommissionProductModel> GetCommissionProductDetatilsManager(GetCommissionProductDetatilsRequest request)
        {
            CommissionProductModel resultModel = null;

            try
            {
                //通用缓存Key
                var prefix = await CacheManager.CommonGetKeyPrefixAsync(DefaultClientName,
                    GlobalConstant.CommissionCacheName);

                using (var client = CacheHelper.CreateCacheClient(DefaultClientName))
                {
                    var cacheResult = await client.GetOrSetAsync($"{prefix}/{request.CpsId}/{request.PID}",
                        () => DalTuboAlliance.GetCommissionProductDetatilsDal(request), GlobalConstant.CommissionExpiration);

                    if (cacheResult.Success)
                    {
                        resultModel = cacheResult.Value;
                    }
                    else
                    {
                        Logger.Error($"GetCommissionProductDetatilsManager佣金商品详情查询Redis执行异常");
                        resultModel = await DalTuboAlliance.GetCommissionProductDetatilsDal(request);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"GetCommissionProductDetatilsManager佣金商品详情查询接口异常:{ex.Message};堆栈信息:{ex.StackTrace}");
            }

            return resultModel;
        }

        /// <summary>
        /// 佣金订单商品记录创建接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<CreateOrderItemRecordResponse> CreateOrderItemRecordManager(CreateOrderItemRecordRequest request)
        {
            var resultModule = new CreateOrderItemRecordResponse();
            resultModule.Success = false;

            try
            {
                //查询所有PID在佣金产品表中的记录
                var pidList = request?.OrderItem?.Select(a => a.Pid).ToList();
                var cpsIDList = request?.OrderItem?.Select(a => "" + a.CpsId).ToList();
                var commissionProductList = await DalTuboAlliance.GetCommissionProductByIdsDal(pidList, cpsIDList);

                //过滤当前请求的PID未在佣金产品表中存在的
                var orderItemRecords = new List<OrderItemRecord>();
                request.OrderItem.ForEach(orderItem =>
                {
                    var commissionProductCount = commissionProductList.Where(a => a.PID ==
                                     orderItem.Pid && a.CpsId == orderItem.CpsId).Count();

                    if (commissionProductCount > 0)
                    {
                        orderItemRecords.Add(orderItem);
                    }

                });

                request.OrderItem = orderItemRecords;
                var resultRow = await DalTuboAlliance.CreateOrderItemRecordsManager(request);

                if (resultRow > 0)
                {
                    resultModule.Success = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"CreateOrderItemRecordManager佣金订单商品记录创建接口异常:{ex.Message};堆栈信息:{ex.StackTrace}");
                resultModule.ErrorMessage = ex.Message;
            }

            return resultModule;
        }

        #region 订单返佣

        /// <summary>
        /// 订单商品返佣接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<CommodityRebateResponse> CommodityRebateManager(CommodityRebateRequest request)
        {
            var resultModule = new CommodityRebateResponse();
            resultModule.Success = true;

            try
            {
                var orderId = string.IsNullOrWhiteSpace(request.OrderId) ? 0 : Convert.ToInt32(request.OrderId);

                //获取拆单订单号集合
                var relatedSplitOrderIds = await GetRelatedSplitOrderIDs(orderId);

                //拆单是需要获取主订单号ID，关联订单集合最小的一位是主订单ID
                var mainOrderId = relatedSplitOrderIds.OrderBy(splitOrderId
                                      => splitOrderId).FirstOrDefault() + "";

                var orderItemRecordList = await DalTuboAlliance.GetOrderItemRecordListDal(mainOrderId);



                //主订单ID在下单记录表中存在的，进行佣金计算
                if (orderItemRecordList?.Count > 0)
                {
                    var orderModel = FetchOrderInfoByID(orderId).Result;

                    if (IsOrderCompleted(orderModel))
                    {
                        await orderModel.OrderListModel?.ForEachAsync(async orderItem =>
                        {
                            //去除套餐的子产品与赠品
                            if (orderItem.PayPrice > 0 && (orderItem.ParentId == null || orderItem.ParentId == 0))
                            {
                                //当前订单的PID在主订单商品集合中查询
                                var orderItemRecord = orderItemRecordList.Where(p =>
                                    p.PID == orderItem.Pid).FirstOrDefault();

                                if (orderItemRecord != null)
                                {
                                    var pid = orderItem.Pid;
                                    var number = orderItem.Num;

                                    //计算佣金使用实体
                                    var calculateCommissionModel = new CalculateCommissionModel();
                                    calculateCommissionModel.OrderItem = orderItem;
                                    calculateCommissionModel.CpsId = orderItemRecord.CpsId;
                                    calculateCommissionModel.DarenId = orderItemRecord.DarenID;
                                    calculateCommissionModel.Pid = pid;
                                    calculateCommissionModel.Number = number;

                                    Logger.Info($"CommodityRebateManager：【OrderId：{orderId}】【MainOrderId:{mainOrderId}】");

                                    if (orderItem.OrderId + "" == mainOrderId) //主订单逻辑
                                    {
                                        await CalculateCommission(calculateCommissionModel);
                                    }
                                    else //拆单订单
                                    {
                                        var cpsSplitOrderItemRecordModel = new CpsSplitOrderItemRecordModel();
                                        cpsSplitOrderItemRecordModel.OrderId = "" + orderItem.OrderId;
                                        cpsSplitOrderItemRecordModel.CpsOrderItemRecordID = orderItemRecord.PkId;
                                        cpsSplitOrderItemRecordModel.PID = pid;
                                        cpsSplitOrderItemRecordModel.Number = number;

                                        var splitOrderItemRecordCount = await DalTuboAlliance.GetSplitOrderItemRecordCountManager(cpsSplitOrderItemRecordModel);

                                        if (splitOrderItemRecordCount <= 0)
                                        {
                                            await DalTuboAlliance.CreateSplitOrderItemRecordManager(cpsSplitOrderItemRecordModel);
                                        }

                                        await CalculateCommission(calculateCommissionModel);
                                    }

                                }
                                else
                                {
                                    Logger.Info($"CommodityRebateManager：【OrderId：{orderId}】【MainOrderId:{mainOrderId}】【PID：{orderItem.Pid}】该商品未在下单记录表中存在");
                                }
                            }
                            else
                            {
                                Logger.Info($"CommodityRebateManager：【orderId：{orderId}】" +
                                            $"【mainOrderId：{mainOrderId}】【PID：{orderItem.Pid}】【ParentId：{orderItem.ParentId}】为套装子产品或者是赠品");
                            }

                        });
                    }
                    else
                    {
                        Logger.Info($"CommodityRebateManager：【OrderId：{orderId}】【MainOrderId】:{mainOrderId}订单未完成");
                    }
                }
                else
                {
                    Logger.Info($"CommodityRebateManager：【OrderId：{orderId}】【MainOrderId】:{mainOrderId}未在下单记录表中存在");
                }


            }
            catch (Exception ex)
            {
                Logger.Error($"CommodityRebateManager订单商品返佣接口异常:{ex.Message};堆栈信息:{ex.StackTrace}");
                resultModule.ErrorMessage = ex.Message;
                resultModule.Success = false;
            }

            return resultModule;
        }

        /// <summary>
        /// 订单是否完成
        /// </summary>
        /// <returns></returns>
        private static bool IsOrderCompleted(OrderModel orderModel)
        {
            var result = false;

            if (orderModel.Status == "3Installed") //到店安装
            {
                if (orderModel.InstallStatus == "2Installed" && orderModel.InstallShopId > 0
                        && orderModel.InstallType == "1ShopInstall")
                {
                    result = true;
                }
            }
            else if (orderModel.Status == "2Shipped") //已签收
            {
                if ((orderModel.InstallShopId == 0 || orderModel.InstallShopId == null)
                    && orderModel.DeliveryStatus.Contains("3.5Signed"))
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// 返佣佣金计算
        /// </summary>
        /// <param name="model"></param>
        /// <param name="orderRecordProductNumber">下单记录的商品数量</param>
        private static async Task CalculateCommission(CalculateCommissionModel model)
        {
            var commissionFlowCount = await DalTuboAlliance.GetCommissionFlowRecordDal("" + model.OrderItem.OrderId,
                nameof(CpsEnum.CpsRunningType.VALUEADDED), model.OrderItem.OrderListId, "");

            //防止重复请求,每个订单下的商品只能存在一条流水记录
            if (commissionFlowCount <= 0)
            {
                //获取商品的佣金比例
                var cpsProductListModel = await DalTuboAlliance.GetCpsProductDal(model.CpsId);

                if (cpsProductListModel != null)
                {
                    var outBizNo = model.OrderItem.OrderId + "_"
                            + (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
                    var actutalAmount = (model.OrderItem.PayPrice * model.OrderItem.Num) *
                                        (cpsProductListModel.CommissionRatio / 100);
                    var minuteActutalAmount = actutalAmount * 100; //转换为单位分
                    //向下取整，总是舍去小数
                    minuteActutalAmount = Math.Floor(minuteActutalAmount);

                    var requestNo = Guid.NewGuid();
                    var cpsCommissionFlowRecordModel = new CpsCommissionFlowRecordModel();
                    cpsCommissionFlowRecordModel.CommissionFlowRecordNo = outBizNo;
                    cpsCommissionFlowRecordModel.OrderId = "" + model.OrderItem.OrderId;
                    cpsCommissionFlowRecordModel.OrderItemPKID = model.OrderItem.OrderListId;
                    cpsCommissionFlowRecordModel.CpsId = model.CpsId;
                    cpsCommissionFlowRecordModel.DarenID = model.DarenId;
                    cpsCommissionFlowRecordModel.Pid = model.Pid;
                    cpsCommissionFlowRecordModel.PayPrice = model.OrderItem.PayPrice;
                    cpsCommissionFlowRecordModel.Number = model.Number;
                    cpsCommissionFlowRecordModel.CommissionRatio = cpsProductListModel.CommissionRatio;
                    cpsCommissionFlowRecordModel.ActutalAmount = minuteActutalAmount;
                    cpsCommissionFlowRecordModel.Type = nameof(CpsEnum.CpsRunningType.VALUEADDED);
                    cpsCommissionFlowRecordModel.RequestNo = requestNo;
                    cpsCommissionFlowRecordModel.Status = nameof(CpsEnum.CpsRunningStatus.NOTSENT);

                    int cpsCreateRunningCount = await DalTuboAlliance.CpsCommissionFlowRecordDal(cpsCommissionFlowRecordModel);

                    if (cpsCreateRunningCount > 0)
                    {
                        var cpsSendPayRequest = new CpsSendPayRequest();
                        cpsSendPayRequest.OutBizNo = outBizNo;
                        cpsSendPayRequest.OrderDesc = $"订单返佣：{model.OrderItem.OrderId};" + model.OrderItem.Remark;
                        cpsSendPayRequest.Amount = minuteActutalAmount;
                        cpsSendPayRequest.ProductName = $"订单返佣：{model.OrderItem.OrderId};";
                        cpsSendPayRequest.RequestNo = "" + requestNo;
                        cpsSendPayRequest.UserId = "" + model.DarenId;

                        await CpsSendPayAsync(cpsSendPayRequest);
                    }
                    else
                    {
                        Logger.Info($"CommodityRebateManager -->CalculateCommission：【outBizNo：{outBizNo}】支付流水创建失败");
                    }
                }
                else
                {
                    Logger.Info($"CommodityRebateManager -->CalculateCommission：【CpsId：{model.CpsId}】佣金ID不存在");
                }
            }
            else
            {
                Logger.Info($"CommodityRebateManager -->CalculateCommission：【OrderId：{model.OrderItem.OrderId}】" +
                            $"【OrderItemPKID:{model.OrderItem.OrderListId}】支付流水已经存在");
            }
        }


        /// <summary>
        /// CPS返佣支付接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static async Task<bool> CpsSendPayAsync(CpsSendPayRequest request)
        {
            var result = false;

            try
            {
                using (var client = new PayClient())
                {
                    var response = await client.CpsSendPayAsync(request);

                    if (!response.Success)
                    {
                        Logger.Info($"{request.OutBizNo} CPS返佣支付请求Pay服务失败");
                    }
                    else
                    {
                        result = response.Result.Success;
                        Logger.Info($"{request.OutBizNo} CPS返佣支付请求Pay服务成功");

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{request.OutBizNo} CPS返佣支付请求Pay服务失败异常;异常信息：{ex.Message};堆栈信息：{ex.StackTrace}");
            }

            return result;
        }

        #endregion

        /// <summary>
        /// 订单商品扣佣接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<CommodityDeductionResponse> CommodityDeductionManager(CommodityDeductionRequest request)
        {
            var resultModule = new CommodityDeductionResponse();
            resultModule.Success = true;

            try
            {
                var orderId = string.IsNullOrWhiteSpace(request.OrderId) ? 0 : Convert.ToInt32(request.OrderId);
                //红冲订单关联信息
                var orderRelationShipOnly = await GetOrderRelationShipOnly(orderId);

                if (orderRelationShipOnly?.Count > 0)
                {
                    //红冲订单的父订单ID
                    var redRushParentOrderId = orderRelationShipOnly.Where(a => a.RelationshipType ==
                                 Convert.ToInt32(OrderRelationshipTypeEnum.HCOrder)).Select(a => a.ParentOrderId).FirstOrDefault();

                    //获取拆单订单号集合
                    var relatedSplitOrderIds = await GetRelatedSplitOrderIDs(redRushParentOrderId);

                    //拆单是需要获取主订单号ID，关联订单集合最小的一位是主订单ID
                    var mainOrderId = relatedSplitOrderIds.OrderBy(splitOrderId
                                          => splitOrderId).FirstOrDefault() + "";

                    //下单商品记录集合
                    var orderItemRecordList = await DalTuboAlliance.GetOrderItemRecordListDal(mainOrderId);

                    //红冲订单主订单再下单记录表存在
                    if (orderItemRecordList?.Count > 0)
                    {
                        var orderModel = FetchOrderInfoByID(orderId).Result;

                        if (orderModel.OrderType == "5红冲订单")
                        {
                            await orderModel.OrderListModel.ForEachAsync(async orderItem =>
                            {
                                //去除套餐的子产品与赠品
                                if (orderItem.PayPrice < 0 && (orderItem.ParentId == null || orderItem.ParentId == 0))
                                {
                                    //当前订单的PID在父订单商品集合中查询
                                    var orderItemRecord = orderItemRecordList?.Where(p =>
                                        p.PID == orderItem.Pid).FirstOrDefault();

                                    if (orderItemRecord != null)
                                    {

                                        var commissionFlowRecord =
                                            await DalTuboAlliance.GetCommissionFlowRecordDetalDal(redRushParentOrderId,
                                                orderItemRecord.CpsId, orderItemRecord.DarenID,
                                                orderItem.Pid, nameof(CpsEnum.CpsRunningType.VALUEADDED));

                                        if (commissionFlowRecord != null)
                                        {
                                            //计算退佣金使用实体
                                            var calculateCommissionModel = new CalculateCommissionModel();
                                            calculateCommissionModel.OrderItem = orderItem;
                                            calculateCommissionModel.CpsId = orderItemRecord.CpsId;
                                            calculateCommissionModel.DarenId = orderItemRecord.DarenID;
                                            calculateCommissionModel.Pid = orderItem.Pid;
                                            calculateCommissionModel.Number = orderItem.Num;
                                            calculateCommissionModel.CommissionRatio =
                                                commissionFlowRecord.CommissionRatio;
                                            calculateCommissionModel.OriOrderId = "" + redRushParentOrderId;
                                            calculateCommissionModel.OriPayInstructionId =
                                                commissionFlowRecord.TransactionNo;
                                            await DeductionCommission(calculateCommissionModel);
                                        }
                                        else
                                        {
                                            Logger.Info($"CommodityDeductionManager：【orderId：{orderId}】" +
                                                        $"【mainOrderId：{mainOrderId}】【PID：{orderItem.Pid}】" +
                                                        $"【CpsId：{orderItemRecord.CpsId}】【DarenID:{orderItemRecord.DarenID}】无增长流水记录");
                                        }
                                    }
                                    else
                                    {
                                        Logger.Info($"CommodityDeductionManager：【orderId：{orderId}】" +
                                                    $"【mainOrderId：{mainOrderId}】【PID：{orderItem.Pid}】订单商品未在下单主记录表中存在");
                                    }
                                }
                                else
                                {
                                    Logger.Info($"CommodityDeductionManager：【orderId：{orderId}】" +
                                                $"【mainOrderId：{mainOrderId}】【PID：{orderItem.Pid}】【ParentId：{orderItem.ParentId}】为套装子产品或者是赠品");
                                }
                            });
                        }
                        else
                        {
                            Logger.Info($"CommodityDeductionManager：【orderId：{orderId}】非红冲订单");
                        }
                    }
                    else
                    {
                        Logger.Info($"CommodityDeductionManager：【orderId：{orderId}】【mainOrderId：{mainOrderId}】红冲订单的主订单无下单商品记录");
                    }
                }
                else
                {
                    Logger.Info($"CommodityDeductionManager：【orderId：{orderId}】红冲关联订单为空");
                }

            }
            catch (Exception ex)
            {
                Logger.Error($"CommodityDeductionManager订单商品退佣佣接口异常:{ex.Message};堆栈信息:{ex.StackTrace}");
                resultModule.ErrorMessage = ex.Message;
                resultModule.Success = false;
            }

            return resultModule;
        }

        /// <summary>
        /// 退佣金计算
        /// </summary>
        /// <param name="model"></param>
        private static async Task DeductionCommission(CalculateCommissionModel model)
        {
            var commissionFlowCount = await DalTuboAlliance.GetCommissionFlowRecordDal("",
                nameof(CpsEnum.CpsRunningType.IMPAIRMENT), model.OrderItem.OrderListId, "" + model.OrderItem.OrderId);

            if (commissionFlowCount <= 0)
            {
                var outBizNo = model.OrderItem.OrderId + "_"
                        +(DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
                var actutalAmount = (model.OrderItem.PayPrice * model.OrderItem.Num) * (model.CommissionRatio / 100);
                ;
                var minuteActutalAmount = actutalAmount * 100; //转换为单位分
                var requestNo = Guid.NewGuid();
                minuteActutalAmount = Math.Ceiling(minuteActutalAmount);

                var commissionFlowRecordSumAmount = DalTuboAlliance.GetCommissionFlowRecordSumAmountDal(model.OriOrderId);

                var comissionSumAmount = commissionFlowRecordSumAmount + minuteActutalAmount;

                if (comissionSumAmount >= 0)
                {
                    var cpsCommissionFlowRecordModel = new CpsCommissionFlowRecordModel();
                    cpsCommissionFlowRecordModel.CommissionFlowRecordNo = outBizNo;
                    cpsCommissionFlowRecordModel.OrderId = model.OriOrderId;
                    cpsCommissionFlowRecordModel.OrderItemPKID = model.OrderItem.OrderListId;
                    cpsCommissionFlowRecordModel.RedRushOrderId = "" + model.OrderItem.OrderId;
                    cpsCommissionFlowRecordModel.CpsId = model.CpsId;
                    cpsCommissionFlowRecordModel.DarenID = model.DarenId;
                    cpsCommissionFlowRecordModel.Pid = model.Pid;
                    cpsCommissionFlowRecordModel.PayPrice = model.OrderItem.PayPrice;
                    cpsCommissionFlowRecordModel.Number = model.Number;
                    cpsCommissionFlowRecordModel.CommissionRatio = model.CommissionRatio;
                    cpsCommissionFlowRecordModel.ActutalAmount = minuteActutalAmount;
                    cpsCommissionFlowRecordModel.Type = nameof(CpsEnum.CpsRunningType.IMPAIRMENT);
                    cpsCommissionFlowRecordModel.RequestNo = requestNo;
                    cpsCommissionFlowRecordModel.TransactionNo = model.OriPayInstructionId;
                    cpsCommissionFlowRecordModel.Status = nameof(CpsEnum.CpsRunningStatus.NOTSENT);

                    int cpsCreateRunningCount = await DalTuboAlliance.CpsCommissionFlowRecordDal(cpsCommissionFlowRecordModel);

                    if (cpsCreateRunningCount > 0)
                    {
                        var cpsSendRefundRequest = new CpsSendRefundRequest();
                        cpsSendRefundRequest.OutBizNo = outBizNo;
                        cpsSendRefundRequest.OriPayInstructionId = model.OriPayInstructionId;
                        cpsSendRefundRequest.RefundType = nameof(CpsEnum.CpsRefundType.NORMAL);
                        cpsSendRefundRequest.RefundRoute = nameof(CpsEnum.CpsRefundRoute.ORIGINAL);
                        cpsSendRefundRequest.RequestNo = "" + requestNo;
                        cpsSendRefundRequest.RefundAmount = Math.Abs(minuteActutalAmount);

                        await CpsSendRefundAsync(cpsSendRefundRequest);
                    }
                    else
                    {
                        Logger.Info($"CommodityDeductionManager -->DeductionCommission：【orderId：{model.OriOrderId}】【RedRushOrderId:{ model.OrderItem.OrderId}】【DarenID：{model.DarenId}】【CPSID：{model.CpsId}】【PID：{model.Pid}】退款流水创建失败");
                    }
                }
                else
                {
                    Logger.Info($"CommodityDeductionManager -->DeductionCommission：【orderId：{model.OriOrderId}】【RedRushOrderId:{ model.OrderItem.OrderId}】【DarenID：{model.DarenId}】【CPSID：{model.CpsId}】【PID：{model.Pid}】退款金额大于支付金额");
                }
            }
            else
            {
                Logger.Info($"CommodityDeductionManager -->DeductionCommission：【orderId：{model.OriOrderId}】【RedRushOrderId:{ model.OrderItem.OrderId}】【DarenID：{model.DarenId}】【CPSID：{model.CpsId}】【PID：{model.Pid}】退款流水已存在");
            }
        }


        /// <summary>
        /// CPS退佣支付接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static async Task<bool> CpsSendRefundAsync(CpsSendRefundRequest request)
        {
            var result = false;

            try
            {
                using (var client = new PayClient())
                {
                    var response = await client.CpsSendRefundAsync(request);

                    if (!response.Success)
                    {
                        Logger.Info($"{request.OutBizNo} CPS退佣支付请求Pay服务失败");
                    }
                    else
                    {
                        result = response.Result.Success;
                        Logger.Info($"{request.OutBizNo} CPS退佣支付请求Pay服务成功");

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{request.OutBizNo} CPS退佣支付请求Pay服务失败异常;异常信息：{ex.Message};堆栈信息：{ex.StackTrace}");
            }

            return result;
        }


        /// <summary>
        /// CPS支付流水修改状态接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<CpsUpdateRunningResponse> CpsUpdateRunningManager(CpsUpdateRunningRequest request)
        {
            var resultModule = new CpsUpdateRunningResponse();
            resultModule.Success = false;

            try
            {
                var resultRow = await DalTuboAlliance.CpsUpdateRunningDal(request);

                if (resultRow > 0)
                {
                    resultModule.Success = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"CpsUpdateRunningManagerCPS佣金流水状态修改接口异常:{ex.Message};堆栈信息:{ex.StackTrace}");
                resultModule.ErrorMessage = ex.Message;
            }

            return resultModule;
        }


        /// <summary>
        /// 根据订单ID获取订单信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        private static async Task<OrderModel> FetchOrderInfoByID(int orderId)
        {
            OrderModel order = null;
            try
            {
                using (var orderClient = new OrderQueryClient())
                {
                    var fetchResult = await orderClient.FetchOrderInfoByIDAsync(orderId);
                    fetchResult.ThrowIfException(true);

                    if (fetchResult.Success)
                    {
                        order = fetchResult.Result;
                    }
                    else
                    {
                        Logger.Warn($"OrderQueryClient.FetchOrderInfoByIDAsync {orderId} 订单详情接口查询失败");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"FetchOrderInfoByID 接口异常;异常信息：{ex.Message}；堆栈信息：{ex.StackTrace}", ex);
            }
            return order;
        }

        /// <summary>
        /// 根据订单ID获取拆单订单的相关订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        private static async Task<List<int>> GetRelatedSplitOrderIDs(int orderId)
        {
            List<int> order = null;
            try
            {
                using (var orderClient = new OrderQueryClient())
                {
                    var fetchResult = await orderClient.GetRelatedSplitOrderIDsAsync(orderId, SplitQueryType.Full);
                    fetchResult.ThrowIfException(true);

                    if (fetchResult.Success)
                    {
                        order = fetchResult.Result?.ToList();
                    }
                    else
                    {
                        Logger.Warn($"OrderQueryClient.GetRelatedSplitOrderIDsAsync {orderId} 订单拆单关联接口查询失败");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"GetRelatedSplitOrderIDs 接口异常;异常信息：{ex.Message}；堆栈信息：{ex.StackTrace}", ex);
            }
            return order;
        }

        /// <summary>
        ///  获取订单关联信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        private static async Task<List<OrderRelationship>> GetOrderRelationShipOnly(int orderId)
        {
            List<OrderRelationship> order = null;
            try
            {
                using (var orderClient = new OrderQueryClient())
                {
                    var fetchResult = await orderClient.GetOrderRelationShipOnlyAsync(orderId, true);
                    fetchResult.ThrowIfException(true);

                    if (fetchResult.Success)
                    {
                        order = fetchResult.Result?.ToList();
                    }
                    else
                    {
                        Logger.Warn($"OrderQueryClient.GetOrderRelationShipOnly {orderId} 订单关联接口查询失败");
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Error($"GetOrderRelationShipOnly 接口异常;异常信息：{ex.Message}；堆栈信息：{ex.StackTrace}", ex);
            }
            return order;
        }
    }
}
