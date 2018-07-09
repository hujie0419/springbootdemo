using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Configuration;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.DataAccess.Entity;
using Tuhu.Service.OprLog;

namespace Tuhu.Provisioning.Business.OprLogManagement
{
    public class OprLogManager
    {
        #region Private Fields
        static string strConn = ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString;

        private static readonly IDBScopeManager DbScopeManager = new DBScopeManager(SecurityHelp.IsBase64Formatted(strConn) ? SecurityHelp.DecryptAES(strConn) : strConn);

        private static readonly ILog logger = LoggerFactory.GetLogger("OprLog");

        private readonly OprLogHandler handler = null;

        #endregion

        #region Ctor
        public OprLogManager()
        {
            handler = new OprLogHandler(DbScopeManager);
        }
        #endregion

        #region Public Methods

        public void AddOprLog(OprLog oprLog)
        {
            try
            {
                this.handler.Add(oprLog);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new OprLogException(BizErrorCode.SystemError, "添加操作日志出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in adding opr log.");

                throw exception;
            }
        }

        public void AddOprLogAsync(OprLog oprLog)
        {
            try
            {
                this.handler.AddAsync(oprLog);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new OprLogException(BizErrorCode.SystemError, "添加操作日志出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in adding opr log.");

                throw exception;
            }
        }

        public void AddEmailProcess(int orderId, BizEmailProcess emailProcess)
        {
            try
            {
                this.handler.AddEmailProcess(orderId, emailProcess);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new OprLogException(BizErrorCode.SystemError, "添加短信错误", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in adding email process.");

                throw exception;
            }
        }
        /// <summary>
        /// 查询是否发送短信
        /// </summary>
        /// <param name="OrderId">订单号：64137</param>
        /// <param name="EmailType">类型：到店短信/发货短信/确认短信 </param>
        /// <returns>false=不存在 true=存在</returns>
        public bool IsExistsEmailSendingMark(int OrderId, string EmailType = "到店短信")
        {
            try
            {
                return this.handler.IsExistsEmailSendingMark(OrderId, EmailType);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new OprLogException(BizErrorCode.SystemError, "查询是否发送短信错误", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in Exists email process.");

                throw exception;
            }
        }
        /// <summary>
        /// 往EmailProcess表中添加数据
        /// </summary>
        /// <param name="emailProcess"></param>
        public void AddEmailProcess(BizEmailProcess emailProcess)
        {
            try
            {
                handler.AddEmailProcess(emailProcess);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var exception = new OprLogException(BizErrorCode.SystemError, "添加邮件出错错误", ex);
                logger.Log(Level.Error, exception, "Error occurred in adding email process.");

                throw exception;
            }
        }

        public bool AddEmailProcessList(List<BizEmailProcess> emailProcess)
        {
            try
            {
                return this.handler.AddEmailProcessList(emailProcess);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new OprLogException(BizErrorCode.SystemError, "添加短信错误", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in adding email process.");

                throw exception;
            }
        }

        public List<BizEmailProcess> SelectEmailProcessesByOrderNo(DateTime startDateTime, DateTime endDateTime,
             string orderNo, string userTel)
        {
            try
            {
                return handler.SelectEmailProcessesByOrderNo(startDateTime, endDateTime, orderNo, userTel);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new OprLogException(BizErrorCode.SystemError, "", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in getting tbl_OprLog by logId.");

                throw exception;
            }
        }

        public bool CheckEmailIsSent(string body)
        {
            try
            {
                return handler.CheckEmailIsSent(body);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new OprLogException(BizErrorCode.SystemError, "", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in SelectEmailProcessByType.");

                throw exception;
            }
        }

        public void AddOprLog<T>(string objType, int objId, string operation, T beforeValue, T afterValue)
            where T : class
        {
            try
            {
                handler.AddOprLog(objType, objId, operation, beforeValue, afterValue);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new OprLogException(BizErrorCode.SystemError, "添加操作日志出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in adding opr log.");

                throw exception;
            }
        }

        public void AddOprLog(string objType, int objId, string beforeValue, string afterValue, string operation)
        {
            try
            {
                this.handler.Add(objType, objId, beforeValue, afterValue, operation);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new OprLogException(BizErrorCode.SystemError, "添加操作日志出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in adding opr log.");

                throw exception;
            }
        }

        public void InsertOrderInstalledEmailProcess(int orderId)
        {
            try
            {
                handler.InsertOrderInstalledEmailProcess(orderId);
            }
            catch (Exception ex)
            {
                var exception = new OprLogException(BizErrorCode.SystemError, "安装短信发送失败！", ex);
                logger.Log(Level.Error, exception, "Error in InsertOrderInstalledEmailProcess.");
                throw exception;
            }
        }

        public void AddSystemOperationTimeLog(int orderId, string description)
        {
            try
            {
                handler.AddSystemOperationTimeLog(orderId, description);
            }
            catch (Exception ex)
            {
                var exception = new OprLogException(BizErrorCode.SystemError, "记录正式提价每个步骤耗时时间！", ex);
                logger.Log(Level.Error, exception, "Error in AddSystemOperationTimeLog.");
                throw exception;
            }

        }

        public void AddDALMy_Center_News(My_Center_News news)
        {
            try
            {
                this.handler.AddDALMy_Center_News(news);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new OprLogException(BizErrorCode.SystemError, "AddDALMy_Center_News出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in AddDALMy_Center_News.");

                throw exception;
            }

        }

        #endregion


        public List<BizEmailProcess> SelectSMSHistoryBySearchVal(string searchType, int pageIndex, int pageSize, string toMail, string orderNo)
        {
            try
            {
                return handler.SelectSMSHistoryBySearchVal(searchType, pageIndex, pageSize, toMail, orderNo);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new OprLogException(BizErrorCode.SystemError, "", innerEx);
                logger.Log(Level.Error, exception, "SelectSMSHistoryBySearchVal");

                throw exception;
            }
        }

        public void AddMarketingSms(BizEmailProcess emailProcess)
        {
            try
            {
                handler.AddMarketingSms(emailProcess);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new OprLogException(BizErrorCode.SystemError, "", innerEx);
                logger.Log(Level.Error, exception, "AddMarketingSms");

                throw exception;
            }
        }

        /// <summary>
        /// 拼接短信
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        //public string GetBaoYangEmailProcess(int orderId)
        //{
        //    return handler.GetBaoYangEmailProcess(orderId);
        //}

        ///// <summary>
        ///// 发短信
        ///// </summary>
        ///// <param name="orderId"></param>
        //public void SendMaintainPackageMessage(int orderId, BizEmailProcess bizEmailProcess)
        //{
        //    handler.SendMaintainPackageMessage(orderId, bizEmailProcess);
        //}

        public void AddShopOprLog(ShopEditOprLog oprLog)
        {
            try
            {
                this.handler.AddShopOprLog(oprLog);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new OprLogException(BizErrorCode.SystemError, "添加门店操作日志出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in adding  shop opr log.");

                throw exception;
            }
        }
        public void AddShopOprLog<T, D>(int shopId, string objType, int objId, string operation, T beforeValue = null, D afterValue = null, string remark = "")
            where T : class
            where D : class
        {
            try
            {
                handler.AddShopOprLog(shopId, objType, objId, operation, beforeValue, afterValue, remark);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new OprLogException(BizErrorCode.SystemError, "添加门店操作日志出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in adding  shop opr log.");

                throw exception;
            }
        }

        public void AddShopOprLog(int shopId, string objType, int objId, string operation, string beforeValue, string afterValue, string remark = "")
        {
            try
            {
                this.handler.AddShopOprLog(shopId, objType, objId, beforeValue, afterValue, operation, remark);
            }
            catch (TuhuBizException)
            {
                throw;
            }
            catch (Exception innerEx)
            {
                var exception = new OprLogException(BizErrorCode.SystemError, "添加门店操作日志出错", innerEx);
                logger.Log(Level.Error, exception, "Error occurred in adding  shop opr log.");

                throw exception;
            }
        }
    }
}
