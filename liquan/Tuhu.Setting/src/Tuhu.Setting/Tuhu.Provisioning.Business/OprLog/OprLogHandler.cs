using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Component.Framework.Identity;
using Tuhu.Provisioning.DataAccess.DAO;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.EmailProcess;
using Tuhu.Service.EmailProcess.Model;
using Tuhu.Service.OprLog;
using Tuhu.Service.OprLog.Models;

namespace Tuhu.Provisioning.Business.OprLogManagement
{
    internal class OprLogHandler
    {
        private static readonly ILog Logger = LoggerFactory.GetLogger("OprLogClient.AddOprLog()");
        #region Private Fields

        private readonly IDBScopeManager dbManager;

        #endregion

        internal OprLogHandler(IDBScopeManager dbManager)
        {
            this.dbManager = dbManager;
        }

        public void AddAsync(OprLog oprLog)
        {
            ThreadPoolAdapter.Instance.QueueAction(_ => Add(oprLog), oprLog);
        }

        public void Add(OprLog oprLog)
        {
            ParameterChecker.CheckNull(oprLog, "oprLog");

            if (string.IsNullOrEmpty(oprLog.Author))
            {
                oprLog.Author = ThreadIdentity.Operator.Name;
            }
            if (string.IsNullOrEmpty(oprLog.IPAddress))
            {
                oprLog.IPAddress = ThreadIdentity.Operator.IPAddress;
            }

            OprLogModel opl = new OprLogModel();
            opl.ObjectId = oprLog.ObjectID;
            opl.ObjectType = oprLog.ObjectType;
            opl.Operation = oprLog.Operation;
            opl.Author = oprLog.Author;
            opl.BeforeValue = oprLog.BeforeValue;
            opl.AfterValue = oprLog.AfterValue;
            opl.HostName = oprLog.HostName;
            opl.IpAddress = oprLog.IPAddress;
            opl.ChangeDatetime = DateTime.Now;

            try
            {
                using (var client = new OprLogClient())
                {
                    var result2 = client.AddOprLog(opl);

                    bool EnablePower = false;
                    if (bool.TryParse(WebConfigurationManager.AppSettings["EnablePower"], out EnablePower) && EnablePower)
                    {
                        result2.ThrowIfException(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, "AddOprLog");
                throw ex;
            }
        }

        public void AddEmailProcess(int orderId, BizEmailProcess emailProcess)
        {
            dbManager.Execute(connection => DalOprLog.AddEmailProcess(connection, orderId, emailProcess));
        }
        public bool IsExistsEmailSendingMark(int OrderId, string EmailType = "到店短信")
        {
            return dbManager.Execute(connection => DalOprLog.IsExistsEmailSendingMark(connection, OrderId, EmailType));
        }

        /// <summary>
        /// 往EmailProcess表中添加数据
        /// </summary>
        /// <param name="emailProcess"></param>
        public void AddEmailProcess(BizEmailProcess emailProcess)
        {
            InsertSMSModel smsModel = new InsertSMSModel
            {
                OrderId = emailProcess.OrderID,
                OrderNo = emailProcess.OrderNo,
                Type = emailProcess.Type,
                Subject = emailProcess.Subject,//主题
                ToMail = emailProcess.ToMail,//发给谁
                FromMail = (string.IsNullOrEmpty(emailProcess.FromMail) ? "tuhusystem@tuhu.cn" : emailProcess.FromMail),//谁发出
                Body = emailProcess.Body,//内容
                Status = emailProcess.Status,//状态
                url = emailProcess.url,
                CC = emailProcess.CC//短信标识
            };
            using (var client = new EmailProcessOperationClient())
            {
                var result = client.InsertEmail(smsModel);
            }
        }

        public bool AddEmailProcessList(List<BizEmailProcess> emailProcess)
        {
            return dbManager.Execute(connection => DalOprLog.AddEmailProcessList(connection, emailProcess));
        }

        /// <summary>
        /// 查询订单修改历史(默认取订单的Log)
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        //public List<OprLog> SelectOrderOprLog(int orderId, string objectType = "Order")
        //{
        //    var list = dbManager.Execute(connection => DalOprLog.SelectOrderOprLog(connection, orderId, objectType));
        //    if (string.Equals(objectType, "Order"))
        //    {
        //        var wmsList = new OperationLogManager().SelectOrderOprLog(orderId, objectType);
        //        foreach (var item in wmsList)
        //        {
        //            var oprLog = new OprLog
        //            {
        //                PKID = item.PKID,
        //                Author = item.Operator,
        //                Operation = item.Operation,
        //                AfterValue = item.Detail,
        //                BeforeValue = item.Detail,
        //                ChangeDatetime = item.LastUpdateTime,
        //                LogType = "WMS"
        //            };
        //            list.Add(oprLog);
        //        }
        //    }
        //    return list.OrderByDescending(x => x.ChangeDatetime).ToList();
        //}

        //public OprLog SelectLog(int logId, string logType = "OprLog")
        //{
        //    OprLog oprLog = null;
        //    if (string.Equals(logType, "WMS"))
        //    {
        //        var WMSLog = new OperationLogManager().GetOperationLog(logId);
        //        if (WMSLog != null)
        //        {
        //            oprLog = new OprLog
        //            {
        //                PKID = WMSLog.PKID,
        //                ChangeDatetime = WMSLog.LastUpdateTime,
        //                Author = WMSLog.CreatedBy,
        //                Operation = WMSLog.Operation,
        //                BeforeValue = "",
        //                AfterValue = WMSLog.Detail
        //            };
        //        }
        //        else
        //        {
        //            throw new OprLogException(logId, "在WMS.OperationLog中没有找到该日志编号对应的详细信息");
        //        }
        //    }
        //    else
        //    {
        //        oprLog = dbManager.Execute(connection => DalOprLog.SelectLog(connection, logId, logType));
        //    }
        //    return oprLog;
        //}

        public List<BizEmailProcess> SelectEmailProcessesByOrderNo(DateTime startDateTime, DateTime endDateTime,
            string orderNo, string userTel)
        {
            ParameterChecker.CheckNull(startDateTime, "startDateTime");
            ParameterChecker.CheckNull(endDateTime, "endDateTime");
            ParameterChecker.CheckNull(orderNo, "orderNo");
            ParameterChecker.CheckNull(userTel, "userTel");

            return DalOprLog.SelectEmailProcessesByOrderNo(startDateTime, endDateTime, orderNo, userTel);
        }
        public bool CheckEmailIsSent(string body)
        {
            ParameterChecker.CheckNullOrEmpty(body, "body");
            return DalOprLog.CheckEmailIsSent(body);
        }

        public void AddOprLog<T>(string objType, int objId, string operation, T beforeValue, T afterValue)
            where T : class
        {
            ParameterChecker.CheckNullOrEmpty(objType, "objType");
            ParameterChecker.CheckNullOrEmpty(operation, "operation");

            var oprLog = new OprLog();
            oprLog.ObjectType = objType;
            oprLog.ObjectID = objId;
            oprLog.ChangeDatetime = DateTime.Now;
            oprLog.Operation = operation;
            oprLog.Author = ThreadIdentity.Operator.Name;
            oprLog.IPAddress = ThreadIdentity.Operator.IPAddress;

            if (typeof(T) == typeof(string))
            {
                oprLog.BeforeValue = (string)(object)beforeValue;
                oprLog.AfterValue = (string)(object)afterValue;
            }
            else
            {
                if (beforeValue != null)
                {
                    var valueBuilder = new StringBuilder();
                    foreach (var property in typeof(T).GetProperties())
                    {
                        var value = property.GetValue(beforeValue, null);
                        valueBuilder.AppendFormat("{0} = {1}", property.Name, value ?? "NULL").AppendLine();
                    }
                    oprLog.BeforeValue = valueBuilder.ToString();
                }
                if (afterValue != null)
                {
                    var valueBuilder = new StringBuilder();
                    foreach (var property in typeof(T).GetProperties())
                    {
                        var value = property.GetValue(afterValue, null);
                        valueBuilder.AppendFormat("{0} = {1}", property.Name, value ?? "NULL").AppendLine();
                    }
                    oprLog.AfterValue = valueBuilder.ToString();
                }
            }

            //dbManager.Execute(connection => DalOprLog.AddOprLog(connection, oprLog));

            Add(oprLog);
        }

        public void Add(string objType, int objId, string beforeValue, string afterValue, string operation)
        {
            var oprLog = new OprLog();
            oprLog.ObjectType = objType;
            oprLog.ObjectID = objId;
            oprLog.BeforeValue = beforeValue;
            oprLog.AfterValue = afterValue;
            oprLog.Operation = operation;
            oprLog.Author = ThreadIdentity.Operator.Name;
            oprLog.IPAddress = ThreadIdentity.Operator.IPAddress;

            Add(oprLog);
        }

        public void InsertOrderInstalledEmailProcess(int orderId)
        {

            dbManager.Execute(
                connection => DalOprLog.InsertOrderInstalledEmailProcess(connection, orderId));
        }
        public void AddSystemOperationTimeLog(int orderId, string description)
        {

            dbManager.Execute(
                connection => DalOprLog.AddSystemOperationTimeLog(connection, orderId, description));
        }
        public void AddDALMy_Center_News(My_Center_News news)
        {
            dbManager.Execute(connection => DALMy_Center_News.Add(connection, news));
        }
        public List<BizEmailProcess> SelectSMSHistoryBySearchVal(string searchType, int pageIndex, int pageSize, string toMail, string orderNo)
        {
            return DalOprLog.SelectSMSHistoryBySearchVal(searchType, pageIndex, pageSize, toMail, orderNo);
        }
        /// <summary>
        /// 添加营销短信
        /// </summary>
        /// <param name="emailProcess"></param>
        public void AddMarketingSms(BizEmailProcess emailProcess)
        {
            ParameterChecker.CheckNull(emailProcess.ToMail, "emailProcess.ToMail");
            ParameterChecker.CheckNullOrWhiteSpace(emailProcess.Subject, "emailProcess.Subject");
            ParameterChecker.CheckNullOrWhiteSpace(emailProcess.Body, "emailProcess.Body");
            ParameterChecker.CheckNullOrEmpty(emailProcess.RelatedUser, "emailProcess.RelatedUser");
            ParameterChecker.CheckNull(emailProcess.OrderID, "emailProcess.OrderID");

            dbManager.Execute(connection => DalOprLog.AddMarketingSms(connection, emailProcess));
        }

        /// <summary>
        /// 拼接保养短信
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        //public string GetBaoYangEmailProcess(int orderId)
        //{
        //    var bizOrder = dbManager.Execute(connection => DalOrder.SelectOrderByOrderId(connection, orderId));
        //    var orderHash =
        //              WebSecurity.Hash(string.Concat(bizOrder.Refno, bizOrder.UserId,
        //                  bizOrder.OrderDatetime.ToString("yyyy-MM-dd HH:mm")));
        //    var shortUrl =
        //        TuhuUtil.GetShortUrl(string.Format(ConfigurationManager.AppSettings["LongUrl"], orderId, orderHash));
        //    if (bizOrder.OrderChannel.Contains("天猫"))
        //    {
        //        bizOrder.OrderChannel = "天猫";
        //    }
        //    else
        //        if (bizOrder.OrderChannel.Contains("淘宝"))
        //        {
        //            bizOrder.OrderChannel = "淘宝";
        //        }
        //    string message = string.Format(ConfigurationManager.AppSettings["MaintainPackageMessage"], bizOrder.UserName, bizOrder.OrderChannel, bizOrder.Refno, shortUrl);
        //    //new JobServerManager().SendMaintainPackageMessage(item.OrderId, item.Refno, item.UserId.ToString(),
        //    //                        item.OrderDatetime, item.UserName, item.UserTel, item.OrderChannel);
        //    return message;
        //}

        ///// <summary>
        ///// 直接发短信
        ///// </summary>
        //public void SendMaintainPackageMessage(int orderId, BizEmailProcess bizEmailProcess)
        //{
        //    var item = dbManager.Execute(connection => DalOrder.SelectOrderByOrderId(connection, orderId));
        //    new JobServerManager().SendMaintainPackageMessage(item.OrderId, item.OrderNo, item.Refno, item.UserId.ToString(),
        //                            item.OrderDatetime, item.UserName, bizEmailProcess.ToMail, item.OrderChannel);
        //}



        #region 门店日志

        public void AddShopOprLog(ShopEditOprLog oprLog)
        {
            ParameterChecker.CheckNull(oprLog, "oprLog");

            if (string.IsNullOrEmpty(oprLog.Author))
            {
                oprLog.Author = ThreadIdentity.Operator.Name;
            }
            if (string.IsNullOrEmpty(oprLog.IPAddress))
            {
                oprLog.IPAddress = ThreadIdentity.Operator.IPAddress;
            }

            dbManager.Execute(connection => DalOprLog.AddShopEditOprLog(connection, oprLog));
        }

        public void AddShopOprLog<T, D>(int shopId, string objType, int objId, string operation, T beforeValue, D afterValue, string Remark)
            where T : class
            where D : class
        {
            ParameterChecker.CheckNullOrEmpty(objType, "objType");
            ParameterChecker.CheckNullOrEmpty(operation, "operation");

            var oprLog = new ShopEditOprLog();
            oprLog.ShopID = shopId;
            oprLog.ObjectType = objType;
            oprLog.ObjectID = objId;
            oprLog.ChangeDatetime = DateTime.Now;
            oprLog.Operation = operation;
            oprLog.Author = ThreadIdentity.Operator.Name;
            oprLog.IPAddress = ThreadIdentity.Operator.IPAddress;
            oprLog.Remark = Remark;

            if (typeof(T) == typeof(string))
            {
                oprLog.BeforeValue = (string)(object)beforeValue;
            }
            else
            {
                if (beforeValue != null)
                {
                    var valueBuilder = new StringBuilder();
                    foreach (var property in typeof(T).GetProperties())
                    {
                        var value = property.GetValue(beforeValue, null);
                        valueBuilder.AppendFormat("{0} = {1}", property.Name, value ?? "NULL").AppendLine();
                    }
                    oprLog.BeforeValue = valueBuilder.ToString();
                }
            }
            if (typeof(D) == typeof(string))
            {
                oprLog.AfterValue = (string)(object)afterValue;
            }
            else
            {
                if (afterValue != null)
                {
                    var valueBuilder = new StringBuilder();
                    foreach (var property in typeof(D).GetProperties())
                    {
                        var value = property.GetValue(afterValue, null);
                        valueBuilder.AppendFormat("{0} = {1}", property.Name, value ?? "NULL").AppendLine();
                    }
                    oprLog.AfterValue = valueBuilder.ToString();
                }
            }

            dbManager.Execute(connection => DalOprLog.AddShopEditOprLog(connection, oprLog));
        }

        public void AddShopOprLog(int shopId, string objType, int objId, string beforeValue, string afterValue, string operation, string Remark)
        {
            var oprLog = new ShopEditOprLog();
            oprLog.ShopID = shopId;
            oprLog.ObjectType = objType;
            oprLog.ObjectID = objId;
            oprLog.BeforeValue = beforeValue;
            oprLog.AfterValue = afterValue;
            oprLog.Operation = operation;
            oprLog.Author = ThreadIdentity.Operator.Name;
            oprLog.IPAddress = ThreadIdentity.Operator.IPAddress;
            oprLog.Remark = Remark;
            if (!string.IsNullOrEmpty(afterValue))
            {
                string[] strs = afterValue.Split(',');
                var valueBuilder = new StringBuilder();
                for (int i = 0; i < strs.Length; i++)
                {
                    valueBuilder.AppendFormat(strs[i]).AppendLine();
                }
                oprLog.AfterValue = valueBuilder.ToString();
            }
            if (!string.IsNullOrEmpty(beforeValue))
            {
                string[] strs = beforeValue.Split(',');
                var valueBuilder = new StringBuilder();
                for (int i = 0; i < strs.Length; i++)
                {
                    valueBuilder.AppendFormat(strs[i]).AppendLine();
                }
                oprLog.BeforeValue = valueBuilder.ToString();
            }


            AddShopOprLog(oprLog);
        }

        #endregion
    }
}
