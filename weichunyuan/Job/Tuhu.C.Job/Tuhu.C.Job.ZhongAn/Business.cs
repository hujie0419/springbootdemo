using Common.Logging;
using K.DLL.DAL;
using K.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using Tuhu.Service.EmailProcess;
using Tuhu.Service.EmailProcess.Model;
using Tuhu.Service.OprLog;
using Tuhu.Service.OprLog.Models;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Request;

namespace K.BLL
{
    public static class Business
    {
        public static string SendDate { get; set; }
        #region 返回码说明
        //SUCCESS("00", "成功"),
        //NOT_NULL("01", "值不能为空!"),
        //VALUE_MISTAKE("02", "值不正确!"),
        //UNDERWRITE_ERROR("03", "核保失败!"),
        //CALL_FAIL("04", "调用失败!"),
        //PROCESS_ERROR("05", "处理错误!"),
        //UNKNOW_ERROR("06", "未知错误!");
        #endregion
        /// <summary>
        /// 第一次发送：录入产品数据
        /// </summary>
        //public static void FirstSend(ILog logger)
        //{
        //    List<DInsuranceTyre> _TyreList = DALInsuranceTyre.GetFirstTypeListFromOrder().OrderBy(p => p.orderNo).ThenBy(p => p.PID).ToList();
        //    string _ExitOrderID = string.Empty;
        //    string _ExitPID = string.Empty;
        //    foreach (var item in _TyreList)
        //    {
        //        try
        //        {
        //            if (string.Equals(item.orderNo, _ExitOrderID) && string.Equals(item.PID, _ExitPID))
        //            {
        //                continue;
        //            }
        //            logger.Info("处理开始，OrderList：" + JsonConvert.SerializeObject(item));
        //            _ExitOrderID = item.orderNo;
        //            _ExitPID = item.PID;
        //            item.type = "1";
        //            item.state = 0;
        //            item.SentTime = DateTime.Now;
        //            var _TyreIDList = DALInsuranceTyre.GetTyreID(int.Parse(item.orderNo), item.PID);
        //            if (_TyreIDList.Tables[0].Rows.Count > 0 && _TyreIDList != null)
        //            {
        //                foreach (DataRow dr in _TyreIDList.Tables[0].Rows)
        //                {
        //                    item.tyreId = dr["InsuranceCode"].ToString();
        //                    item.tyreBatchNo = dr["WeekYear"].ToString();
        //                    bool IsRight = true;
        //                    if (string.IsNullOrEmpty(item.tyreId))
        //                    {
        //                        logger.Error("轮胎条形码为空,OrderListPkid：" + item.OrderListPkid);
        //                        DalMonitor.AddMonitorOperation(new MonitorOperation()
        //                        {
        //                            SubjectType = "Order",
        //                            SubjectId = item.orderNo,
        //                            ErrorMessage = "轮胎条形码为空,OrderListPkid：" + item.OrderListPkid,
        //                            OperationUser = "ZhongAnJob",
        //                            OperationName = "Insurance",
        //                            MonitorLevel = 2,
        //                            MonitorModule = "JobSchedulerService"
        //                        });
        //                        IsRight = false;
        //                    }
        //                    if (string.IsNullOrEmpty(item.tyreBatchNo))
        //                    {
        //                        logger.Error("轮胎批号为空,OrderListPkid：" + item.OrderListPkid);
        //                        DalMonitor.AddMonitorOperation(new MonitorOperation()
        //                        {
        //                            SubjectType = "Order",
        //                            SubjectId = item.orderNo,
        //                            ErrorMessage = "轮胎批号为空,OrderListPkid：" + item.OrderListPkid,
        //                            OperationUser = "ZhongAnJob",
        //                            OperationName = "Insurance",
        //                            MonitorLevel = 2,
        //                            MonitorModule = "JobSchedulerService"
        //                        });
        //                        IsRight = false;
        //                    }
        //                    if (IsRight)
        //                    {
        //                        var insuranceTyreSuc = DALInsuranceTyre.GetInsuranceType(item.tyreId, item.type, 1, item.orderNo);
        //                        if (insuranceTyreSuc != null)
        //                        {
        //                            continue;
        //                        }
        //                        string _PKID = string.Empty;
        //                        var insuranceTyre = DALInsuranceTyre.GetInsuranceType(item.tyreId, item.type, 0, item.orderNo);
        //                        if (insuranceTyre != null)
        //                        {
        //                            _PKID = insuranceTyre.PKID.ToString();
        //                        }
        //                        else
        //                        {
        //                            _PKID = DALInsuranceTyre.AddInsuranceTyre(item);
        //                        }
        //                        item.PKID = int.Parse(_PKID);
        //                        IDictionary<String, String> _Result = DataInteraction.ZAOP.SendData(item);
        //                        logger.Info("发送数据结束,OrderListPkid：" + item.OrderListPkid + ",返回" + JsonConvert.SerializeObject(_Result));

        //                        string _BizContentString = _Result["bizContent"];
        //                        if (!string.IsNullOrEmpty(_BizContentString))
        //                        {
        //                            IDictionary<String, String> _BizContent = JsonConvert.DeserializeObject<Dictionary<String, String>>(_BizContentString);
        //                            string _ReturnCode = _BizContent["returnCode"];
        //                            if (!string.IsNullOrEmpty(_ReturnCode) && _ReturnCode == "00")
        //                            {
        //                                logger.Info("更新发送表,OrderListPkid：" + item.OrderListPkid + ",返回" + _ReturnCode);
        //                                DALInsuranceTyre.UpdateState(1, item.PKID);
        //                            }
        //                            else
        //                            {
        //                                DalMonitor.AddMonitorOperation(new MonitorOperation()
        //                                {
        //                                    SubjectType = "Order",
        //                                    SubjectId = item.orderNo,
        //                                    ErrorMessage = "更新发送表错误：" + _BizContentString,
        //                                    OperationUser = "ZhongAnJob",
        //                                    OperationName = "Insurance",
        //                                    MonitorLevel = 2,
        //                                    MonitorModule = "JobSchedulerService"
        //                                });
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            logger.Error("第一次发送错误：", ex);
        //            DalMonitor.AddMonitorOperation(new MonitorOperation()
        //            {
        //                SubjectType = "Order",
        //                SubjectId = item.orderNo,
        //                ErrorMessage = "第一次发送错误：" + ex.ToString(),
        //                OperationUser = "ZhongAnJob",
        //                OperationName = "Insurance",
        //                MonitorLevel = 2,
        //                MonitorModule = "JobSchedulerService"
        //            });
        //        }
        //    }
        //}

        public static void FirstSend(ILog logger)
        {
            var orders = DALOrder.SelectFirstSendOrders();
            foreach (var order in orders)
            {
                var orderToinsuranceTyreId = -1;
                var isNeedUpdateInsuranceStatus = false;
                var insuranceTyres = DALInsuranceTyre.GetFirstTypeListFromOrder(order.PKID);
                var orderToinsuranceTyre = DALOrderToInsuranceTyre.GetOrderToInsuranceTyre(order.PKID, 1, 1);
                if (orderToinsuranceTyre != null)
                {
                    continue;
                }
                orderToinsuranceTyre = DALOrderToInsuranceTyre.GetOrderToInsuranceTyre(order.PKID, 1, 0);
                if (orderToinsuranceTyre == null)
                {
                    orderToinsuranceTyreId = DALOrderToInsuranceTyre.InsertOrderToInsuranceTyre(new DOrderToInsuranceTyre()
                    {
                        OrderId = order.PKID,
                        InsuranceType = 1,
                        InsuranceStatus = 0,
                        CreatedBy = "ZhongAnJob"
                    });
                }
                else
                {
                    orderToinsuranceTyreId = orderToinsuranceTyre.PKID;
                }

                string _ExitOrderID = string.Empty;
                string _ExitPID = string.Empty;

                foreach (var item in insuranceTyres)
                {
                    item.orderNo = order.PKID.ToString();
                    item.orderDate = string.Format("{0:yyyy-MM-dd HH:mm:ss}", order.OrderDate);
                    item.customerName = order.UserName;
                    // item.customerPhoneNo = order.UserTel;
                    item.plateNumber = order.CarPlate;
                    item.storeAddress = order.StoreAddress;
                    item.storeName = order.StoreName;

                    try
                    {
                        if (string.Equals(item.orderNo, _ExitOrderID) && string.Equals(item.PID, _ExitPID))
                        {
                            continue;
                        }
                        logger.Info("处理开始，OrderList：" + JsonConvert.SerializeObject(item));
                        _ExitOrderID = item.orderNo;
                        _ExitPID = item.PID;
                        item.type = "1";
                        item.state = 0;
                        item.SentTime = DateTime.Now;
                        var _TyreIDList = DALInsuranceTyre.GetTyreID(int.Parse(item.orderNo), item.PID);
                        if (_TyreIDList.Tables[0].Rows.Count > 0 && _TyreIDList != null)
                        {
                            foreach (DataRow dr in _TyreIDList.Tables[0].Rows)
                            {
                                item.tyreId = dr["InsuranceCode"].ToString();
                                item.tyreBatchNo = dr["WeekYear"].ToString();
                                bool IsRight = true;
                                if (string.IsNullOrEmpty(item.tyreId))
                                {
                                    logger.Error("轮胎条形码为空,OrderListPkid：" + item.OrderListPkid);
                                    DalMonitor.AddMonitorOperation(new MonitorOperation()
                                    {
                                        SubjectType = "Order",
                                        SubjectId = item.orderNo,
                                        ErrorMessage = "轮胎条形码为空,OrderListPkid：" + item.OrderListPkid,
                                        OperationUser = "ZhongAnJob",
                                        OperationName = "Insurance",
                                        MonitorLevel = 2,
                                        MonitorModule = "JobSchedulerService"
                                    });
                                    IsRight = false;
                                }
                                if (string.IsNullOrEmpty(item.tyreBatchNo))
                                {
                                    logger.Error("轮胎批号为空,OrderListPkid：" + item.OrderListPkid);
                                    DalMonitor.AddMonitorOperation(new MonitorOperation()
                                    {
                                        SubjectType = "Order",
                                        SubjectId = item.orderNo,
                                        ErrorMessage = "轮胎批号为空,OrderListPkid：" + item.OrderListPkid,
                                        OperationUser = "ZhongAnJob",
                                        OperationName = "Insurance",
                                        MonitorLevel = 2,
                                        MonitorModule = "JobSchedulerService"
                                    });
                                    IsRight = false;
                                }
                                if (IsRight)
                                {
                                    var insuranceTyreSuc = DALInsuranceTyre.GetInsuranceType(item.tyreId, item.type, 1, item.orderNo);
                                    if (insuranceTyreSuc != null)
                                    {
                                        continue;
                                    }
                                    string _PKID = string.Empty;
                                    var insuranceTyre = DALInsuranceTyre.GetInsuranceType(item.tyreId, item.type, 0, item.orderNo);
                                    if (insuranceTyre != null)
                                    {
                                        _PKID = insuranceTyre.PKID.ToString();
                                    }
                                    else
                                    {
                                        _PKID = DALInsuranceTyre.AddInsuranceTyre(item);
                                    }
                                    item.PKID = int.Parse(_PKID);
                                    IDictionary<String, String> _Result = DataInteraction.ZAOP.SendData(item);
                                    logger.Info("发送数据结束,OrderListPkid：" + item.OrderListPkid + ",返回" + JsonConvert.SerializeObject(_Result));

                                    string _BizContentString = _Result["bizContent"];
                                    if (!string.IsNullOrEmpty(_BizContentString))
                                    {
                                        IDictionary<String, String> _BizContent = JsonConvert.DeserializeObject<Dictionary<String, String>>(_BizContentString);
                                        string _ReturnCode = _BizContent["returnCode"];
                                        if (!string.IsNullOrEmpty(_ReturnCode) && _ReturnCode == "00")
                                        {
                                            logger.Info("更新发送表,OrderListPkid：" + item.OrderListPkid + ",返回" + _ReturnCode);
                                            DALInsuranceTyre.UpdateState(1, item.PKID);
                                            isNeedUpdateInsuranceStatus = true;
                                        }
                                        else
                                        {
                                            DalMonitor.AddMonitorOperation(new MonitorOperation()
                                            {
                                                SubjectType = "Order",
                                                SubjectId = item.orderNo,
                                                ErrorMessage = "更新发送表错误：" + _BizContentString,
                                                OperationUser = "ZhongAnJob",
                                                OperationName = "Insurance",
                                                MonitorLevel = 2,
                                                MonitorModule = "JobSchedulerService"
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error("第一次发送错误：", ex);
                        DalMonitor.AddMonitorOperation(new MonitorOperation()
                        {
                            SubjectType = "Order",
                            SubjectId = item.orderNo,
                            ErrorMessage = "第一次发送错误：" + ex.ToString(),
                            OperationUser = "ZhongAnJob",
                            OperationName = "Insurance",
                            MonitorLevel = 2,
                            MonitorModule = "JobSchedulerService"
                        });
                    }
                }

                #region 更新订单基础数据的投保状态
                if (isNeedUpdateInsuranceStatus)
                {
                    var needInsureNum = insuranceTyres.Sum(_ => _.Num);
                    var insuredNum = DALInsuranceTyre.SelectInsuranceTypes(1, 1, order.PKID).Count;
                    if(needInsureNum == insuredNum)
                    {
                        DALOrderToInsuranceTyre.UpdateOrderToInsuranceTyreStatus(orderToinsuranceTyreId, 1);
                    }
                }
                #endregion
            }
        }

        /// <summary>
        /// 第二次发送：发送承保数据
        /// </summary>
        /// <param name="logger"></param>
        //public static void SecondSend(ILog logger)
        //{
        //    List<DInsuranceTyre> _TyreList = DALInsuranceTyre.GetSecondTypeListFromOrder().OrderBy(p => p.orderNo).ThenBy(p => p.PID).ToList();
        //    string _ExitOrderID = string.Empty;
        //    string _ExitPID = string.Empty;
        //    foreach (var item in _TyreList)
        //    {
        //        try
        //        {
        //            if (string.Equals(item.orderNo, _ExitOrderID) && string.Equals(item.PID, _ExitPID))
        //            {
        //                continue;
        //            }
        //            item.type = "2";
        //            item.state = 0;
        //            item.SentTime = DateTime.Now;
        //            var _TyreIDList = DALInsuranceTyre.GetTyreID(int.Parse(item.orderNo), item.PID);
        //            if (_TyreIDList != null && _TyreIDList.Tables[0].Rows.Count > 0)
        //            {
        //                logger.Info("处理开始，OrderList：" + JsonConvert.SerializeObject(item));
        //                foreach (DataRow dr in _TyreIDList.Tables[0].Rows)
        //                {
        //                    item.tyreId = dr["InsuranceCode"].ToString();
        //                    item.tyreBatchNo = dr["WeekYear"].ToString();
        //                    bool IsRight = true;
        //                    if (string.IsNullOrEmpty(item.tyreId))
        //                    {
        //                        logger.Error("轮胎条形码为空,OrderListPkid：" + item.OrderListPkid);
        //                        DalMonitor.AddMonitorOperation(new MonitorOperation()
        //                        {
        //                            SubjectType = "Order",
        //                            SubjectId = item.orderNo,
        //                            ErrorMessage = "轮胎条形码为空,OrderListPkid：" + item.OrderListPkid,
        //                            OperationUser = "ZhongAnJob",
        //                            OperationName = "Insurance",
        //                            MonitorLevel = 2,
        //                            MonitorModule = "JobSchedulerService"
        //                        });
        //                        IsRight = false;
        //                    }
        //                    if (string.IsNullOrEmpty(item.tyreBatchNo))
        //                    {
        //                        logger.Error("轮胎批号为空,OrderListPkid：" + item.OrderListPkid);
        //                        DalMonitor.AddMonitorOperation(new MonitorOperation()
        //                        {
        //                            SubjectType = "Order",
        //                            SubjectId = item.orderNo,
        //                            ErrorMessage = "轮胎批号为空,OrderListPkid：" + item.OrderListPkid,
        //                            OperationUser = "ZhongAnJob",
        //                            OperationName = "Insurance",
        //                            MonitorLevel = 2,
        //                            MonitorModule = "JobSchedulerService"
        //                        });
        //                        IsRight = false;
        //                    }
        //                    if (IsRight)
        //                    {
        //                        var insuranceTyreSuc = DALInsuranceTyre.GetInsuranceType(item.tyreId, item.type, 1, item.orderNo);
        //                        if (insuranceTyreSuc != null)
        //                        {
        //                            continue;
        //                        }
        //                        string _PKID = string.Empty;
        //                        var insuranceTyre = DALInsuranceTyre.GetInsuranceType(item.tyreId, item.type, 0, item.orderNo);
        //                        if (insuranceTyre != null)
        //                        {
        //                            _PKID = insuranceTyre.PKID.ToString();
        //                        }
        //                        else
        //                        {
        //                            _PKID = DALInsuranceTyre.AddInsuranceTyre(item);
        //                        }
        //                        item.PKID = int.Parse(_PKID);
        //                        IDictionary<String, String> _Result = DataInteraction.ZAOP.SendData(item);
        //                        logger.Info("发送数据结束,OrderListPkid：" + item.OrderListPkid + ",返回" + JsonConvert.SerializeObject(_Result));
        //                        string _BizContentString = _Result["bizContent"];
        //                        if (!string.IsNullOrEmpty(_BizContentString))
        //                        {
        //                            IDictionary<String, String> _BizContent = JsonConvert.DeserializeObject<Dictionary<String, String>>(_BizContentString);
        //                            string _ReturnCode = _BizContent["returnCode"];
        //                            if (!string.IsNullOrEmpty(_ReturnCode) && _ReturnCode == "00")
        //                            {
        //                                DInsuranceNO _DInsuranceNO = new DInsuranceNO()
        //                                {
        //                                    tyreId = _BizContent["tyreId"],
        //                                    policyNo = _BizContent["policyNo"],
        //                                    effectiveDate = _BizContent["effectiveDate"],
        //                                    endDate = _BizContent["endDate"],
        //                                    issueDate = _BizContent["issueDate"],
        //                                };
        //                                DALInsuranceNO.AddInsuranceNO(_DInsuranceNO);
        //                                logger.Info("更新发送表,OrderListPkid：" + item.OrderListPkid + ",返回" + _ReturnCode);
        //                                DALInsuranceTyre.UpdateState(1, item.PKID);
        //                                var oprLog = new DOprLog()
        //                                {
        //                                    Author = "ZhongAnJob",
        //                                    ObjectType = "Order",
        //                                    ObjectID = int.Parse(item.orderNo.Trim()),
        //                                    BeforeValue = string.Format("轮胎保险号：{0}", item.tyreId),
        //                                    AfterValue = string.Format("众安保单号：{0}", _DInsuranceNO.policyNo),
        //                                    ChangeDatetime = DateTime.Now,
        //                                    Operation = "众安保险投保",
        //                                };
        //                                ThreadPool.QueueUserWorkItem(_ => 
        //                                {
        //                                    OprLogModel opl = new OprLogModel()
        //                                    {
        //                                        ObjectId = int.Parse(item.orderNo.Trim()),
        //                                        ObjectType = "Order",
        //                                        Operation = "众安保险投保",
        //                                        Author = "ZhongAnJob",
        //                                        BeforeValue = string.Format("轮胎保险号：{0}", item.tyreId),
        //                                        AfterValue  = string.Format("众安保单号：{0}", _DInsuranceNO.policyNo),
        //                                        ChangeDatetime = DateTime.Now
        //                                    };
        //                                    try
        //                                    {
        //                                        using (var client = new OprLogClient())
        //                                        {
        //                                            var result = client.Invoke(channel => channel.AddOprLog(opl));
        //                                            if (!result.Success)
        //                                            {
        //                                                oprLog.HostName += "Error";
        //                                                DALOprLog.InsertOprLog(oprLog);
        //                                            }
        //                                        }
        //                                    }
        //                                    catch (Exception ex)
        //                                    {
        //                                        oprLog.HostName += "Error";
        //                                        DALOprLog.InsertOprLog(oprLog);
        //                                    }
        //                                }, oprLog);
        //                            }
        //                            else
        //                            {
        //                                DalMonitor.AddMonitorOperation(new MonitorOperation()
        //                                {
        //                                    SubjectType = "Order",
        //                                    SubjectId = item.orderNo,
        //                                    ErrorMessage = "更新发送表错误：" + _BizContentString,
        //                                    OperationUser = "ZhongAnJob",
        //                                    OperationName = "Insurance",
        //                                    MonitorLevel = 2,
        //                                    MonitorModule = "JobSchedulerService"
        //                                });
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            logger.Error("第二次发送错误：", ex);
        //            DalMonitor.AddMonitorOperation(new MonitorOperation()
        //            {
        //                SubjectType = "Order",
        //                SubjectId = item.orderNo,
        //                ErrorMessage = "第二次发送错误：" + ex.ToString(),
        //                OperationUser = "ZhongAnJob",
        //                OperationName = "Insurance",
        //                MonitorLevel = 2,
        //                MonitorModule = "JobSchedulerService"
        //            });
        //        }
        //    }
        //}

        public static void SecondSend(ILog logger)
        {
            var orders = DALOrder.SelectSecondSendOrders();
            foreach (var order in orders)
            {
                #region 投保

                var orderToinsuranceTyreId = -1;
                var isNeedUpdateInsuranceStatus = false;
                var insuranceTyres = DALInsuranceTyre.GetFirstTypeListFromOrder(order.PKID);
                var orderToinsuranceTyre = DALOrderToInsuranceTyre.GetOrderToInsuranceTyre(order.PKID, 2, 1);
                if (orderToinsuranceTyre != null)
                {
                    continue;
                }
                orderToinsuranceTyre = DALOrderToInsuranceTyre.GetOrderToInsuranceTyre(order.PKID, 2, 0);
                if (orderToinsuranceTyre == null)
                {
                    orderToinsuranceTyreId = DALOrderToInsuranceTyre.InsertOrderToInsuranceTyre(new DOrderToInsuranceTyre()
                    {
                        OrderId = order.PKID,
                        InsuranceType = 2,
                        InsuranceStatus = 0,
                        CreatedBy = "ZhongAnJob"
                    });
                }
                else
                {
                    orderToinsuranceTyreId = orderToinsuranceTyre.PKID;
                }

                string _ExitOrderID = string.Empty;
                string _ExitPID = string.Empty;
                var insuranceNos = new List<string>();
                foreach (var item in insuranceTyres)
                {
                    item.orderNo = order.PKID.ToString();
                    item.orderDate = string.Format("{0:yyyy-MM-dd HH:mm:ss}", order.OrderDate);
                    item.customerName = order.UserName;
                    // item.customerPhoneNo = order.UserTel;
                    item.plateNumber = order.CarPlate;
                    item.storeAddress = order.StoreAddress;
                    item.storeName = order.StoreName;

                    try
                    {
                        if (string.Equals(item.orderNo, _ExitOrderID) && string.Equals(item.PID, _ExitPID))
                        {
                            continue;
                        }
                        item.type = "2";
                        item.state = 0;
                        item.SentTime = DateTime.Now;
                        var _TyreIDList = DALInsuranceTyre.GetTyreID(int.Parse(item.orderNo), item.PID);
                        if (_TyreIDList != null && _TyreIDList.Tables[0].Rows.Count > 0)
                        {
                            var olCount = 0;
                            logger.Info("处理开始，OrderList：" + JsonConvert.SerializeObject(item));
                            foreach (DataRow dr in _TyreIDList.Tables[0].Rows)
                            {
                                item.tyreId = dr["InsuranceCode"].ToString();
                                item.tyreBatchNo = dr["WeekYear"].ToString();
                                bool IsRight = true;
                                if (string.IsNullOrEmpty(item.tyreId))
                                {
                                    logger.Error("轮胎条形码为空,OrderListPkid：" + item.OrderListPkid);
                                    DalMonitor.AddMonitorOperation(new MonitorOperation()
                                    {
                                        SubjectType = "Order",
                                        SubjectId = item.orderNo,
                                        ErrorMessage = "轮胎条形码为空,OrderListPkid：" + item.OrderListPkid,
                                        OperationUser = "ZhongAnJob",
                                        OperationName = "Insurance",
                                        MonitorLevel = 2,
                                        MonitorModule = "JobSchedulerService"
                                    });
                                    IsRight = false;
                                }
                                if (string.IsNullOrEmpty(item.tyreBatchNo))
                                {
                                    logger.Error("轮胎批号为空,OrderListPkid：" + item.OrderListPkid);
                                    DalMonitor.AddMonitorOperation(new MonitorOperation()
                                    {
                                        SubjectType = "Order",
                                        SubjectId = item.orderNo,
                                        ErrorMessage = "轮胎批号为空,OrderListPkid：" + item.OrderListPkid,
                                        OperationUser = "ZhongAnJob",
                                        OperationName = "Insurance",
                                        MonitorLevel = 2,
                                        MonitorModule = "JobSchedulerService"
                                    });
                                    IsRight = false;
                                }

                                olCount++;
                                if(olCount > item.Num)
                                {
                                    // 部分取消的单子只需要随即选条码导入
                                    IsRight = false;
                                }

                                if (IsRight)
                                {
                                    var insuranceTyreSuc = DALInsuranceTyre.GetInsuranceType(item.tyreId, item.type, 1, item.orderNo);
                                    if (insuranceTyreSuc != null)
                                    {
                                        continue;
                                    }
                                    string _PKID = string.Empty;
                                    var insuranceTyre = DALInsuranceTyre.GetInsuranceType(item.tyreId, item.type, 0, item.orderNo);
                                    if (insuranceTyre != null)
                                    {
                                        _PKID = insuranceTyre.PKID.ToString();
                                    }
                                    else
                                    {
                                        _PKID = DALInsuranceTyre.AddInsuranceTyre(item);
                                    }
                                    item.PKID = int.Parse(_PKID);
                                    IDictionary<String, String> _Result = DataInteraction.ZAOP.SendData(item);
                                    logger.Info("发送数据结束,OrderListPkid：" + item.OrderListPkid + ",返回" + JsonConvert.SerializeObject(_Result));
                                    string _BizContentString = _Result["bizContent"];
                                    if (!string.IsNullOrEmpty(_BizContentString))
                                    {
                                        IDictionary<String, String> _BizContent = JsonConvert.DeserializeObject<Dictionary<String, String>>(_BizContentString);
                                        string _ReturnCode = _BizContent["returnCode"];
                                        if (!string.IsNullOrEmpty(_ReturnCode) && _ReturnCode == "00")
                                        {
                                            DInsuranceNO _DInsuranceNO = new DInsuranceNO()
                                            {
                                                tyreId = _BizContent["tyreId"],
                                                policyNo = _BizContent["policyNo"],
                                                effectiveDate = _BizContent["effectiveDate"],
                                                endDate = _BizContent["endDate"],
                                                issueDate = _BizContent["issueDate"],
                                            };
                                            DALInsuranceNO.AddInsuranceNO(_DInsuranceNO);
                                            logger.Info("更新发送表,OrderListPkid：" + item.OrderListPkid + ",返回" + _ReturnCode);
                                            DALInsuranceTyre.UpdateState(1, item.PKID);
                                            isNeedUpdateInsuranceStatus = true;
                                            if (!insuranceNos.Contains(_DInsuranceNO.policyNo))
                                            {
                                                insuranceNos.Add(_DInsuranceNO.policyNo);
                                            }
                                            var oprLog = new DOprLog()
                                            {
                                                Author = "ZhongAnJob",
                                                ObjectType = "Order",
                                                ObjectID = int.Parse(item.orderNo.Trim()),
                                                BeforeValue = string.Format("轮胎保险号：{0}", item.tyreId),
                                                AfterValue = string.Format("众安保单号：{0}", _DInsuranceNO.policyNo),
                                                ChangeDatetime = DateTime.Now,
                                                Operation = "众安保险投保",
                                            };
                                            ThreadPool.QueueUserWorkItem(_ =>
                                            {
                                                OprLogModel opl = new OprLogModel()
                                                {
                                                    ObjectId = int.Parse(item.orderNo.Trim()),
                                                    ObjectType = "Order",
                                                    Operation = "众安保险投保",
                                                    Author = "ZhongAnJob",
                                                    BeforeValue = string.Format("轮胎保险号：{0}", item.tyreId),
                                                    AfterValue = string.Format("众安保单号：{0}", _DInsuranceNO.policyNo),
                                                    ChangeDatetime = DateTime.Now
                                                };
                                                try
                                                {
                                                    using (var client = new OprLogClient())
                                                    {
                                                        var result = client.Invoke(channel => channel.AddOprLog(opl));
                                                        if (!result.Success)
                                                        {
                                                            oprLog.HostName += "Error";
                                                            // DALOprLog.InsertOprLog(oprLog);
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    oprLog.HostName += "Error";
                                                    // DALOprLog.InsertOprLog(oprLog);
                                                }
                                            }, oprLog);
                                        }
                                        else
                                        {
                                            DalMonitor.AddMonitorOperation(new MonitorOperation()
                                            {
                                                SubjectType = "Order",
                                                SubjectId = item.orderNo,
                                                ErrorMessage = "更新发送表错误：" + _BizContentString,
                                                OperationUser = "ZhongAnJob",
                                                OperationName = "Insurance",
                                                MonitorLevel = 2,
                                                MonitorModule = "JobSchedulerService"
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error("第二次发送错误：", ex);
                        DalMonitor.AddMonitorOperation(new MonitorOperation()
                        {
                            SubjectType = "Order",
                            SubjectId = item.orderNo,
                            ErrorMessage = "第二次发送错误：" + ex.ToString(),
                            OperationUser = "ZhongAnJob",
                            OperationName = "Insurance",
                            MonitorLevel = 2,
                            MonitorModule = "JobSchedulerService"
                        });
                    }
                }

                #endregion

                #region 更新订单承保数据的投保状态

                if (isNeedUpdateInsuranceStatus)
                {
                    var needInsureNum = insuranceTyres.Sum(_ => _.Num);
                    var insuredNum = DALInsuranceTyre.SelectInsuranceTypes(2, 1, order.PKID).Count;
                    if (needInsureNum == insuredNum)
                    {
                        DALOrderToInsuranceTyre.UpdateOrderToInsuranceTyreStatus(orderToinsuranceTyreId, 1);
                    }

                    if (insuranceNos.Any())
                    {
                        try
                        {
                            var insuranceMessage = string.Format(ConfigurationManager.AppSettings["Sms_ZhongAnInsurance"], order.OrderNo, string.Join(",", insuranceNos));
                            // 模板：【133】尊敬的途虎用户，您的轮胎保（途虎轮胎订单号{0}）已由众安保险成功承保，保险凭证号{1}，如有任何疑问请致电400-111-8868。
                            using (var smsClient = new SmsClient())
                            {
                                var result = smsClient.SendSms(new SendTemplateSmsRequest
                                {
                                    Cellphone = order.UserTel,
                                    UserData = order.OrderNo,
                                    TemplateId = 133,
                                    TemplateArguments = new []{ order.OrderNo, string.Join(",", insuranceNos) }
                                });
                                logger.Info($"Tuhu.Yewu.Job.ZhongAn 途虎养车轮胎险投保发送短信，请求信息：{JsonConvert.SerializeObject(result)},响应信息：{JsonConvert.SerializeObject(result)}");
                                result.ThrowIfException(true);
                                if (result.Result<=0)
                                {
                                    logger.Error("Tuhu.Yewu.Job.ZhongAn 途虎养车轮胎险投保发送短信发送短信失败，请求信息：{JsonConvert.SerializeObject(result)}，响应信息：{JsonConvert.SerializeObject(result)}", new Exception($"ErrorCode:{result.ErrorCode??result.Result.ToString()},ErrorMsg:{result.ErrorMessage}"));
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            DalMonitor.AddMonitorOperation(new MonitorOperation()
                            {
                                SubjectType = "Order",
                                SubjectId = order.PKID.ToString(),
                                ErrorMessage = "第二次发送错误：" + ex.ToString(),
                                OperationUser = "ZhongAnJob",
                                OperationName = "Insurance",
                                MonitorLevel = 2,
                                MonitorModule = "JobSchedulerService"
                            });
                        }
                    }
                }

                #endregion
            }
        }

        public static void ThirdSend(ILog logger)
        {
            List<DInsuranceTyre> _TyreList = DALInsuranceTyre.GetThirdTypeList();
            foreach (var item in _TyreList)
            {
                try
                {
                    if (DALInsuranceNO.IsTyreExisted(item.tyreId) && item.type == "2")
                    {
                        DALInsuranceTyre.UpdateState(1, item.PKID);
                    }
                    else
                    {
                        IDictionary<String, String> _Result = DataInteraction.ZAOP.SendData(item);
                        string _BizContentString = _Result["bizContent"];
                        logger.Info(string.Format("发送弥补数据, PKID: {0}, bizContent: {1}", item.PKID, _BizContentString));
                        if (!string.IsNullOrEmpty(_BizContentString))
                        {
                            IDictionary<String, String> _BizContent = JsonConvert.DeserializeObject<Dictionary<String, String>>(_BizContentString);
                            string _ReturnCode = _BizContent["returnCode"];
                            if (!string.IsNullOrEmpty(_ReturnCode) && _ReturnCode == "00")
                            {
                                if (item.type == "2")
                                {
                                    DInsuranceNO _DInsuranceNO = new DInsuranceNO()
                                    {
                                        tyreId = _BizContent["tyreId"],
                                        policyNo = _BizContent["policyNo"],
                                        effectiveDate = _BizContent["effectiveDate"],
                                        endDate = _BizContent["endDate"],
                                        issueDate = _BizContent["issueDate"],
                                    };
                                    DALInsuranceNO.AddInsuranceNO(_DInsuranceNO);
                                }
                                DALInsuranceTyre.UpdateState(1, item.PKID);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("第三次发送错误：", ex);
                }
            }
        }

        public static void CancelInsurance(ILog logger)
        {
            var insuranceTypes = DALInsuranceTyre.SelectCancelingInsuranceTypes();
            foreach (var insuranceType in insuranceTypes)
            {
                try
                {
                    logger.Info(string.Format("OrderNo:{0}, OrderListId:{1}开始取消投保", insuranceType.orderNo, insuranceType.OrderListPkid));

                    insuranceType.type = "3";
                    insuranceType.SentTime = DateTime.Now;
                    var result = DataInteraction.ZAOP.SendData(insuranceType);
                    var content = result["bizContent"];
                    if (!string.IsNullOrEmpty(content))
                    {
                        logger.Info(string.Format("OrderNo:{0}, OrderListId:{1}取消投保返回bizContent: {2}", insuranceType.orderNo, insuranceType.OrderListPkid, content));

                        var bizContent = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                        var returnCode = bizContent["returnCode"];
                        if (string.Equals(returnCode, "00", StringComparison.CurrentCultureIgnoreCase))
                        {
                            DALInsuranceTyre.UpdateState(1, insuranceType.PKID);
                        }
                        else
                        {
                            logger.Info(string.Format("OrderNo:{0}, OrderListId:{1}取消投保失败, ReturnCode:{2}", insuranceType.orderNo, insuranceType.OrderListPkid, returnCode));
                            DalMonitor.AddMonitorOperation(new MonitorOperation()
                            {
                                SubjectType = "Order",
                                SubjectId = insuranceType.orderNo,
                                ErrorMessage = string.Format("OrderNo:{0}, OrderListId:{1}取消投保失败, ReturnCode:{2}", insuranceType.orderNo, insuranceType.OrderListPkid, returnCode),
                                OperationUser = "ZhongAnJob",
                                OperationName = "Insurance",
                                MonitorLevel = 2,
                                MonitorModule = "JobSchedulerService"
                            });
                        }
                    }
                    else
                    {
                        logger.Info(string.Format("OrderNo:{0}, OrderListId:{1}取消投保返回bizContent为空", insuranceType.orderNo, insuranceType.OrderListPkid));
                        DalMonitor.AddMonitorOperation(new MonitorOperation()
                        {
                            SubjectType = "Order",
                            SubjectId = insuranceType.orderNo,
                            ErrorMessage = string.Format("OrderNo:{0}, OrderListId:{1}取消投保返回bizContent为空", insuranceType.orderNo, insuranceType.OrderListPkid),
                            OperationUser = "ZhongAnJob",
                            OperationName = "Insurance",
                            MonitorLevel = 2,
                            MonitorModule = "JobSchedulerService"
                        });
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(string.Format("OrderNo:{0}取消投保失败:", insuranceType.orderNo), ex);
                    DalMonitor.AddMonitorOperation(new MonitorOperation()
                    {
                        SubjectType = "Order",
                        SubjectId = insuranceType.orderNo,
                        ErrorMessage = string.Format("OrderNo:{0}取消投保失败:{1}", insuranceType.orderNo, ex.ToString()),
                        OperationUser = "ZhongAnJob",
                        OperationName = "Insurance",
                        MonitorLevel = 2,
                        MonitorModule = "JobSchedulerService"
                    });
                }
            }
        }
    }
}
