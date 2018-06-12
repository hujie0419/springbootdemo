using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Provisioning.Business.CommonServices;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.DataAccess.DAO.RebateConfig;
using Tuhu.Provisioning.DataAccess.Entity.RebateConfig;
using Tuhu.Service.Pay;
using Tuhu.Service.Pay.Models;
using Tuhu.Service.Push.Models.Push;

namespace Tuhu.Provisioning.Business.RebateConfig
{
    public class RebateConfigManager
    {
        private static readonly IConnectionManager ConnectionManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir"].ConnectionString);
        private static readonly IConnectionManager ConnectionReadManager =
            new ConnectionManager(ConfigurationManager.ConnectionStrings["Gungnir_AlwaysOnRead"].ConnectionString);

        private readonly IDBScopeManager dbScopeManager = null;
        private readonly IDBScopeManager dbScopeReadManager = null;

        private static readonly Common.Logging.ILog logger = LogManager.GetLogger(typeof(RebateConfigManager));

        public RebateConfigManager()
        {
            this.dbScopeManager = new DBScopeManager(ConnectionManager);
            this.dbScopeReadManager = new DBScopeManager(ConnectionReadManager);
        }

        public List<RebateConfigModel> SelectRebateConfig(Status status, string orderId, string phone,
            string wxId, string remarks, string timeType, DateTime? startTime, DateTime? endTime,
            string wxName, string principalPerson, string rebateMoney, int pageIndex, int pageSize)
        {
            var result = new List<RebateConfigModel>();
            try
            {
                dbScopeReadManager.Execute(conn =>
                {
                    result = DALRebateConfig.GetRebateConfig(conn, status, orderId ?? string.Empty, phone, wxId, remarks,
                        timeType, startTime, endTime, wxName, principalPerson, rebateMoney, pageIndex, pageSize);
                    if (result != null && result.Any())
                    {
                        foreach (var item in result)
                        {
                            item.ImgList = DALRebateConfig.GetRebateApplyImageConfig(conn, item.PKID, ImgSource.UserImg);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public RebateConfigModel SelectRebateConfigByPKID(int pkid)
        {
            RebateConfigModel result = null;
            try
            {
                dbScopeReadManager.Execute(conn =>
                {
                    result = DALRebateConfig.SelectRebateConfigByPKID(conn, pkid);
                    result.ImgList = DALRebateConfig.GetRebateApplyImageConfig(conn, pkid, ImgSource.UserImg);
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public bool UpdateRebateApplyRemarks(RebateConfigModel data, string user)
        {
            var result = false;
            try
            {
                result = dbScopeManager.Execute(conn => DALRebateConfig.UpdateRebateApplyConfig(conn, data)) > 0;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            InsertLog(data.PKID.ToString(), "UpdateRemarks", result ? "更新成功" : "更新失败", $"PKID:{data.PKID.ToString()},Remarks:{data.Remarks}", string.Empty, string.Empty, user);
            return result;
        }

        public bool UpdateStatusForComplete(int pkid, string user)
        {
            var result = false;
            try
            {
                dbScopeManager.CreateTransaction(conn =>
                {
                    var data = DALRebateConfig.SelectRebateConfigByPKID(conn, pkid);
                    if (data.Status == Status.Approved)
                    {
                        result = DALRebateConfig.UpdateStatusForComplete(conn, pkid) > 0;
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            InsertLog(pkid.ToString(), "UpdateStatusForComplete", result ? "成功" : "失败", $"PKID:{pkid.ToString()},Status:Complete", string.Empty, string.Empty, user);
            return result;
        }

        public Tuple<bool, string> UpdateStatus(int pkid, Status status, string refusalReason, int pushBatchId, string user)
        {
            var result = false;
            var msg = string.Empty;
            try
            {
                if (status == Status.Approved || status == Status.Unapprove)
                {
                    dbScopeManager.CreateTransaction(conn =>
                    {
                        var data = DALRebateConfig.SelectRebateConfigByPKID(conn, pkid);
                        if (status == Status.Approved)
                        {
                            var existedData = DALRebateConfig.SelectRebateApplyConfigByParam(conn, data);
                            if (existedData?.Where(x => String.Equals(x.UserPhone, data.UserPhone)
                            && String.Equals(x.OrderId, data.OrderId)).Count() > 0)
                            {
                                msg = "该手机号、订单号已有返现记录，不能重复参加";
                            }
                            else if (existedData?.Where(x => String.Equals(x.UserPhone, data.UserPhone)).Count() > 0)
                            {
                                msg = "该手机号已有返现记录，不能重复参加";
                            }
                            else if (existedData?.Where(x => String.Equals(x.OrderId, data.OrderId)).Count() > 0)
                            {
                                msg = "该订单号已有返现记录，不能重复参加";
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(data.OpenId))
                                {
                                    if (data.Status == Status.Applying)
                                    {
                                        var getResult = SendRedBag(new WxSendRedBagRequest()
                                        {
                                            OpenId = data.OpenId,
                                            PayWay = "WX_QIYEFUKUAN",
                                            Money = 2500M, //返现25元 单位：分
                                            Channel = "途虎朋友圈点赞返现",
                                            Remark = "途虎25元集赞已返到零钱；参加58元发帖返现请添加微信号：tuhu24咨询。"
                                        });
                                        if (getResult.Item1)
                                        {
                                            var pushResult = PushService.PushWechatInfoByBatchId(3383, new PushTemplateLog() { Target = data.OpenId });
                                            InsertLog(pkid.ToString(), "PushWechatInfo", pushResult ? "消息推送成功" : "消息推送失败", $"BatchId：{3383}", string.Empty, string.Empty, user);
                                            result = DALRebateConfig.UpdateStatusForCompleteV2(conn, pkid) > 0;
                                        }

                                        msg = getResult.Item2 ?? "返现失败";
                                        InsertLog(pkid.ToString(), "SendRedBag", getResult.Item1 ? "返现成功" : msg, $"OpenId：{data.OpenId},Money：{decimal.Parse("25.00")}", string.Empty, string.Empty, user);
                                    }
                                    else
                                    {
                                        msg = $"审核状态异常，BeforeValue:{data.Status.ToString()},AfterValue:{status.ToString()}";
                                    }
                                }
                                else
                                {
                                    result = DALRebateConfig.UpdateStatus(conn, pkid, Status.Approved) > 0;
                                }
                            }
                        }
                        else
                        {
                            result = DALRebateConfig.UpdateStatus(conn, pkid, Status.Unapprove, refusalReason) > 0;
                            if (!string.IsNullOrEmpty(data.OpenId) && result && pushBatchId > 0 && data.Status == Status.Applying)
                            {
                                var pushResult = PushService.PushWechatInfoByBatchId(pushBatchId, new PushTemplateLog() { Target = data.OpenId });
                                InsertLog(pkid.ToString(), "PushWechatInfo", pushResult ? "消息推送成功" : "消息推送失败", $"BatchId：{pushBatchId}", string.Empty, string.Empty, user);
                            }
                            else
                            {
                                msg = $"审核状态异常，BeforeValue:{data.Status.ToString()},AfterValue:{status.ToString()}";
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                msg = "服务异常";
                logger.Error(ex);
            }
            InsertLog(pkid.ToString(), "UpdateStatus", result ? "审核成功" : msg, $"PKID:{pkid.ToString()},Status:{status.ToString()}", string.Empty, string.Empty, user);
            return Tuple.Create(result, msg);
        }

        public static Tuple<bool, string> SendRedBag(WxSendRedBagRequest request)
        {
            var result = false;
            var errorMsg = string.Empty;
            try
            {
                using (var client = new PayClient())
                {
                    var getResult = client.Wx_SendRedBag(request);
                    //getResult.ThrowIfException(true);
                    result = getResult.Result;
                    errorMsg = getResult.ErrorMessage;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return Tuple.Create(result, errorMsg);
        }

        public bool WxPayManualRetry(string outbizNo, string user)
        {
            var result = false;
            try
            {
                using (var client = new PayClient())
                {
                    var getResult = client.WxPayManualRetry(new[] { outbizNo });
                    result = getResult.Result > 0 && getResult.Success;
                    InsertLog(outbizNo, "WxPayManualRetry", result ? "重试成功" : "重试失败", JsonConvert.SerializeObject(getResult), string.Empty, string.Empty, user);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public bool DeleteRebateApplyConfig(List<string> pkids, bool isDelete, string user)
        {
            var result = false;
            try
            {
                if (pkids != null && pkids.Any())
                {
                    result = dbScopeManager.Execute(conn => DALRebateConfig.DeleteRebateApplyConfig(conn, pkids, isDelete)) > 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            #region 日志
            if (result)
            {
                ThreadPool.QueueUserWorkItem(o =>
                {
                    foreach (var item in pkids)
                    {
                        InsertLog(item, "DeleteRebateApplyConfig", result ? "操作成功" : "操作失败", $"PKID:{item}，IsDelete:{isDelete}", string.Empty, string.Empty, user);
                    }
                });
            }
            #endregion
            return result;
        }

        public List<RebateConfigLog> SearchRebateConfigLog(string pkid)
        {
            List<RebateConfigLog> result = new List<RebateConfigLog>();
            try
            {
                result = DALRebateConfig.SearchRebateConfigLog(pkid);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;

        }

        public static void InsertLog(string identityId, string type, string msg, string remark,
            string ipAddress, string hostName, string user)
        {
            try
            {
                var data = new
                {
                    IdentityID = identityId,
                    Type = type,
                    Msg = msg,
                    Remark = remark,
                    OperateUser = user,
                    IPAddress = ipAddress,
                    HostName = hostName
                };
                LoggerManager.InsertLog("RebateConfigLog", data);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public List<RebateApplyPageConfig> SelectRebateApplyPageConfig(int pageIndex, int pageSize)
        {
            var result = new List<RebateApplyPageConfig>();
            try
            {
                dbScopeReadManager.Execute(conn =>
                {
                    result = DALRebateConfig.GetRebateApplyPageConfig(conn, pageIndex, pageSize);
                    if (result != null && result.Any())
                    {
                        result.ForEach(item => item.ImgList = DALRebateConfig.GetRebateApplyImageConfig(conn, item.PKID, ImgSource.PageImg));
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }

        public bool InsertRebateApplyPageConfig(RebateApplyPageConfig data, string user)
        {
            var result = false;
            try
            {
                dbScopeManager.CreateTransaction(conn =>
                {
                    var pkid = DALRebateConfig.InsertRebateApplyPageConfig(conn, data.BackgroundImg, data.ActivityRules, user);
                    if (data.ImgList != null && data.ImgList.Any())
                    {
                        foreach (var item in data.ImgList)
                        {
                            DALRebateConfig.InsertRebateImgConfig(conn, pkid, item.ImgUrl, ImgSource.PageImg, item.Remarks);
                        }
                    }
                    result = true;
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return result;
        }
    }
}
