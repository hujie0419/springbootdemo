using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.DAO.ThirdReplaceOrder;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Provisioning.DataAccess.Entity.CheXingYiOrderResultInfo;
using Tuhu.Service.Order;
using Tuhu.Service.Order.Models;
using Tuhu.Service.ThirdParty;

namespace Tuhu.Provisioning.Business.ThirdPartyReplaceOrder
{
    public class ThirdPartyReplaceOrderManager
    {
        private static readonly IConnectionManager ConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IConnectionManager ConnectionReadManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);
        private static readonly IConnectionManager TuhuLogConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Tuhu_log"].ConnectionString);

        private readonly IDBScopeManager dbScopeManager = null;
        private readonly IDBScopeManager dbScopeReadManager = null;
        private readonly IDBScopeManager dbTuhuLogScopeReadManager = null;

        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(ThirdPartyReplaceOrderManager));
        private static string LogType = "ThirdOrderSubmitLog";
        //private static String VIOLATION_URL = $"{ConfigurationManager.AppSettings["CheXinYiHost"]}gateway.aspx";

        public ThirdPartyReplaceOrderManager()
        {
            this.dbScopeManager = new DBScopeManager(ConnectionManager);
            this.dbScopeReadManager = new DBScopeManager(ConnectionReadManager);
            this.dbTuhuLogScopeReadManager = new DBScopeManager(TuhuLogConnectionManager);
        }

        /// <summary>
        /// 获取需要发送支付通知的订单
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="orderType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<OrderLists> SelectNeedSendOrder(DateTime startTime, DateTime endTime, string orderType)
        {
            List<OrderLists> result = new List<OrderLists>();
            try
            {
                var data = dbScopeReadManager.Execute(conn => DALThirdReplaceOrder.SelectNeedSendOrder(conn, startTime, endTime, orderType));
                var sendOrderList = dbTuhuLogScopeReadManager.Execute(conn => DALThirdReplaceOrder.SelectSendOrderPayNoticeOrderIds(conn));
                if (data != null && data.Any())
                {
                    if (sendOrderList == null || !sendOrderList.Any())
                    {
                        result = data;
                    }
                    else
                    {
                        data.ForEach(x =>
                        {
                            if (sendOrderList.Where(y => y == x.PKID).Count() <= 0)
                                result.Add(x);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public Tuple<bool, string> SubmitThirdReplaceOrderByOrderId(int tuhuOrderId, string user)
        {
            var result = false;
            var msg = "提交失败";
            try
            {
                var order = FetchOrderByOrderId(tuhuOrderId);
                var serialNum = SelectSerialNumByTuhuOrderId(tuhuOrderId);
                if (order != null && order.Status != "7Canceled")
                {
                    switch (order.OrderType)
                    {
                        case "11违章代缴":
                            result = SendCheXingYiPeccancyNoti(tuhuOrderId, serialNum ?? string.Empty);
                            break;
                        case "12加油卡":
                            result = SendFuleCardRechargeNoti(tuhuOrderId, serialNum ?? string.Empty);
                            break;
                        case "26道路救援":
                            result = SendRoadRescueNoti(tuhuOrderId);
                            break;
                        default:
                            msg = $"订单类型必须为12加油卡或11违章代缴或26道路救援 OrderType:{order.OrderType}";
                            break;
                    }
                }
                else
                {
                    msg = "订单不存在或已取消";
                }
                InsertLog(tuhuOrderId.ToString(), order?.OrderType ?? string.Empty, msg, result, user);
            }
            catch (Exception ex)
            {
                msg = "系统异常";
                logger.Error(ex);
            }
            return Tuple.Create(result, msg);
        }

        /// <summary>
        /// 第三方补单
        /// </summary>
        /// <param name="tuhuOrderId"></param>
        /// <param name="orderType"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Tuple<bool, string> SubmitThirdReplaceOrder(int tuhuOrderId, string orderType, string user)
        {
            var result = false;
            var msg = "提交失败";
            try
            {
                var isSend = IsSendOrderPayNotice(tuhuOrderId);
                if (!isSend)
                {
                    var order = FetchOrderByOrderId(tuhuOrderId);
                    var serialNum = SelectSerialNumByTuhuOrderId(tuhuOrderId);
                    if (order != null && order.Status != "7Canceled")
                    {
                        switch (order.OrderType)
                        {
                            case "11违章代缴":
                                result = SendCheXingYiPeccancyNoti(tuhuOrderId, serialNum ?? string.Empty);
                                //var cheXingYiId = SelectCheXingYiIdByTuhuOrderId(tuhuOrderId);
                                //if (!string.IsNullOrEmpty(cheXingYiId))
                                //{
                                //    var cheXingYiInfo = GetCheXingYiOrderInfo(cheXingYiId);
                                //    if (cheXingYiInfo != null && cheXingYiInfo.Code == "0")
                                //    {
                                //        if (cheXingYiInfo.Data.OrderStatus == "NeedPay" || cheXingYiInfo.Data.OrderStatus == "Proceccing")
                                //        {
                                //            result = SendCheXingYiPeccancyNoti(tuhuOrderId, serialNum);
                                //        }
                                //        else
                                //        {
                                //            msg = $"车行易订单状态 OrderStatus{cheXingYiInfo.Data.OrderStatus}";
                                //        }
                                //    }
                                //    else
                                //    {
                                //        msg = $"车行易订单查询异常 ErrorMsg:{cheXingYiInfo.Errormsg}";
                                //    }
                                //}
                                //else
                                //{
                                //    msg = "没有对应的车行易订单号";
                                //}
                                break;
                            case "12加油卡":
                                result = SendFuleCardRechargeNoti(tuhuOrderId, serialNum ?? string.Empty);
                                break;
                            default:
                                msg = $"订单类型必须为12加油卡或11违章代缴 OrderType:{order.OrderType}";
                                break;
                        }
                    }
                    else
                    {
                        msg = "订单已取消";
                    }
                }
                else
                {
                    msg = "已提交补单操作，请勿重复提交";
                }
            }
            catch (Exception ex)
            {
                msg = "系统异常";
                logger.Error(ex);
            }
            InsertLog(tuhuOrderId.ToString(), orderType, msg, result, user);
            return Tuple.Create(result, msg);
        }

        /// <summary>
        /// 获取途虎订单的订单流水号
        /// </summary>
        /// <param name="tuhuOrderId"></param>
        /// <returns></returns>
        public string SelectSerialNumByTuhuOrderId(long tuhuOrderId)
        {
            var result = string.Empty;
            try
            {
                result = dbScopeReadManager.Execute(conn => DALThirdReplaceOrder.SelectSerialNumByTuhuOrderId(conn, tuhuOrderId));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// 是否已经发送支付通知
        /// </summary>
        /// <param name="tuhuOrderId"></param>
        /// <returns></returns>
        public bool IsSendOrderPayNotice(long tuhuOrderId)
        {
            var result = false;

            try
            {
                result = dbTuhuLogScopeReadManager.Execute(conn => DALThirdReplaceOrder.IsSendOrderPayNotice(conn, tuhuOrderId)) > 0;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }

        public static void InsertLog(string tuhuOrderId, string orderType, string msg, bool result, string opera)
        {
            try
            {
                var info = new
                {
                    TuhuOrderId = tuhuOrderId,
                    OrderType = orderType,
                    Msg = msg,
                    Result = result,
                    Operator = opera,
                    CreatedUser = opera,
                    CreatedTime = DateTime.Now,
                    UpdatedTime = DateTime.Now
                };
                LoggerManager.InsertLog(LogType, info);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }


        ///// <summary>
        ///// 获取途虎订单号对应的车行易订单号
        ///// </summary>
        ///// <param name="tuhuOrderId"></param>
        ///// <returns></returns>
        //public string SelectCheXingYiIdByTuhuOrderId(long tuhuOrderId)
        //{
        //    var result = string.Empty;
        //    try
        //    {
        //        result = dbScopeReadManager.Execute(conn => DALThirdReplaceOrder.SelectCheXingYiIdByTuhuOrderId(conn, tuhuOrderId));
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex);
        //    }
        //    return result;
        //}

        ///// <summary>
        ///// 根据车行易Id获取车行易订单信息
        ///// </summary>
        ///// <param name="cheXingYiId"></param>
        ///// <returns></returns>
        //public CheXingYiOrderResultInfo GetCheXingYiOrderInfo(string cheXingYiId)
        //{
        //    CheXingYiOrderResultInfo result = null;
        //    try
        //    {
        //        List<Tuple<String, String>> orderRsultDetail = new List<Tuple<string, string>>()
        //        {
        //            Tuple.Create("method_type", "detail"),
        //            Tuple.Create("orderId", cheXingYiId),
        //        };
        //        var info = ViolationHttpUtils.SendAndReceive(orderRsultDetail, VIOLATION_URL);
        //        result = JsonConvert.DeserializeObject<CheXingYiOrderResultInfo>(info);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex);
        //    }
        //    return result;
        //}

        #region 服务
        /// <summary>
        /// 发送违章代缴支付通知
        /// </summary>
        /// <param name="tuhuOrderId"></param>
        /// <param name="serialNumbers"></param>
        /// <returns></returns>
        public bool SendCheXingYiPeccancyNoti(int tuhuOrderId, string serialNumbers)
        {
            var result = false;
            try
            {
                using (var client = new FuleOrPeccancyServiceClient())
                {
                    var getResult = client.SendCheXingYiPeccancyNoti(tuhuOrderId, serialNumbers);
                    getResult.ThrowIfException(true);
                    result = getResult.Success ? getResult.Result : false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// 发送加油卡支付通知
        /// </summary>
        /// <param name="tuhuOrderId"></param>
        /// <param name="serialNumbers"></param>
        /// <returns></returns>
        public bool SendFuleCardRechargeNoti(int tuhuOrderId, string serialNumbers)
        {
            var result = false;
            try
            {
                using (var client = new FuleOrPeccancyServiceClient())
                {
                    var getResult = client.SendFuleCardRechargeNoti(tuhuOrderId, serialNumbers);
                    getResult.ThrowIfException(true);
                    result = getResult.Success ? getResult.Result : false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// 道路救援补单通知
        /// </summary>
        /// <param name="tuhuOrderId"></param>
        /// <returns></returns>
        public bool SendRoadRescueNoti(int tuhuOrderId)
        {
            var result = false;
            try
            {
                using (var client = new FuleOrPeccancyServiceClient())
                {
                    var getResult = client.SendRoadRescueNoti(tuhuOrderId);
                    getResult.ThrowIfException(true);
                    result = getResult.Success ? getResult.Result : false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// 查询订单信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public OrderModel FetchOrderByOrderId(int orderId)
        {
            OrderModel order = null;
            try
            {
                using (var orderClient = new OrderQueryClient())
                {
                    var fetchResult = orderClient.FetchOrderByOrderId(orderId);
                    fetchResult.ThrowIfException(true);
                    order = fetchResult.Success ? fetchResult.Result : null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return order;
        }
        #endregion
    }
}
