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
            string wxName, string principalPerson, string rebateMoney,string source, int pageIndex, int pageSize)
        {
            var result = new List<RebateConfigModel>();
            try
            {
                dbScopeReadManager.Execute(conn =>
                {
                    result = DALRebateConfig.GetRebateConfig(conn, status, orderId ?? string.Empty, phone, wxId, remarks,
                        timeType, startTime, endTime, wxName, principalPerson, rebateMoney,source, pageIndex, pageSize);
                    if (result != null && result.Any() && pageSize != 9999999)
                    {
                        var pay = PayService.QueryWxPayStatus("途虎朋友圈点赞返现", "WX_QIYEFUKUAN", result.Select(x => x.OrderId.ToString()).ToList());
                        foreach (var item in result)
                        {
                            var payInfo= pay?.Where(x => String.Equals(x.OrderNo, item.OrderId.ToString()))?.FirstOrDefault();
                            item.PayStatus = payInfo?.PayStatus ?? 0;
                            item.RedOutbizNo = payInfo?.WxPayOrderNo ?? string.Empty;
                            item.Reason = payInfo?.Reason ?? string.Empty;
                            item.UserPhone = item.UserPhone.Substring(0, 3) + "****" + item.UserPhone.Substring(item.UserPhone.Length - 4, 4);
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

        public Tuple<bool, string> InsertRebateConfig(RebateConfigModel data, string user)
        {
            var result = 0;
            var msg = string.Empty;
            try
            {
                dbScopeManager.CreateTransaction(conn =>
                {
                    var existedData = DALRebateConfig.SelectRebateApplyConfigByParam(conn, data);

                    if (String.Equals(data.Source, "Rebate25"))
                    {
                        existedData = existedData.Where(_ => String.Equals(_.Source, "Rebate25")).ToList();
                        if (existedData?.Where(x => String.Equals(x.OrderId, data.OrderId)).Count() > 0
                           || existedData?.Where(x => String.Equals(x.UserPhone, data.UserPhone)).Count() > 0)
                        {
                            data.Remarks += "  重复返现";
                        }
                        data.RebateMoney = 25M;
                        data.Status = Status.Complete;
                        data.RebateTime = DateTime.Now;
                        data.PKID = DALRebateConfig.InsertRebateConfig(conn, data);
                        if (data.PKID > 0 && data.ImgList != null && data.ImgList.Any())
                        {
                            foreach (var item in data.ImgList)
                            {
                                DALRebateConfig.InsertRebateImgConfig(conn, data.PKID, item.ImgUrl, ImgSource.UserImg, string.Empty);
                            }
                        }
                        result = data.PKID;
                    }
                    else
                    {
                    existedData = existedData.Where(_ => String.Equals(_.Source, "爱卡") || String.Equals(_.Source, "汽车之家")).ToList();
                    if (existedData?.Where(x => String.Equals(x.OrderId, data.OrderId)).Count() > 0
                        || existedData?.Where(x => String.Equals(x.UserPhone, data.UserPhone)).Count() > 0)
                    {
                        msg = "每个客户只能参与一次（包含手机号、订单号、微信号）均视为同一客户";
                    }
                    else
                    {
                        data.RebateMoney = 58M;
                        data.Status = Status.Applying;
                        result = DALRebateConfig.InsertRebateConfig(conn, data);
                    }
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            InsertLog(result.ToString(), "InsertRebateConfig", result > 0 ? "添加成功" : "添加失败", $"PKID:{result.ToString()},Status:Applying", string.Empty, string.Empty, user);
            return Tuple.Create(result > 0, msg);
        }

        public bool UpdateRemarks(int pkid, string remarks, string user)
        {
            var result = false;
            try
            {
                result = dbScopeManager.Execute(conn => DALRebateConfig.UpdateRemarks(conn, pkid, remarks)) > 0;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            InsertLog(pkid.ToString(), "UpdateRemarks", result ? "更新成功" : "更新失败", $"PKID:{pkid.ToString()},Remarks:{remarks}", string.Empty, string.Empty, user);
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
                    if (data.Status == Status.Approved || ((String.Equals(data.Source, "爱卡") || String.Equals(data.Source, "汽车之家")) && data.Status == Status.Applying))
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
                            if (String.Equals(data.Source, "Rebate25"))
                            {
                                existedData = existedData.Where(_ => String.Equals(_.Source, "Rebate25")).ToList();
                            }
                            else
                            {
                                existedData = existedData.Where(_ => String.Equals(_.Source, "爱卡") || String.Equals(_.Source, "汽车之家")).ToList();
                            }
                            if (existedData?.Where(x => String.Equals(x.OrderId, data.OrderId)).Count() > 0
                            || existedData?.Where(x => String.Equals(x.UserPhone, data.UserPhone)).Count() > 0)
                            {
                                msg = "每个客户只能参与一次（包含手机号、订单号、微信号）均视为同一客户";
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(data.OpenId))
                                {
                                    if (data.Status == Status.Applying)
                                    {
                                        if (String.Equals(data.Source, "爱卡") || String.Equals(data.Source, "汽车之家"))
                                        {
                                            result = DALRebateConfig.UpdateStatusForCompleteV2(conn, pkid) > 0;
                                        }
                                        else
                                        {
                                            var getResult = SendRedBag(new WxSendRedBagRequest()
                                            {
                                                OrderNo = data.OrderId.ToString(),
                                                OpenId = data.OpenId,
                                                PayWay = "WX_QIYEFUKUAN",
                                                Money = 2500M, //返现25元 单位：分
                                                Channel = "途虎朋友圈点赞返现",
                                                Remark = "已返现到微信零钱中，请查收。加入车主微信群请添加微信：tuhu25。"
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

        public bool WxPayManualRetry(int pkid, string user)
        {
            var result = false;
            var remarks = string.Empty;
            try
            {
                var info = SelectRebateConfigByPKID(pkid);
                if (info != null)
                {
                    var pay = PayService.QueryWxPayStatus("途虎朋友圈点赞返现", "WX_QIYEFUKUAN", new List<string> { info.OrderId.ToString() });
                    if (pay != null && pay.Any())
                    {
                        using (var client = new PayClient())
                        {
                            var getResult = client.WxPayManualRetry(new[] { pay.FirstOrDefault().WxPayOrderNo });
                            remarks = $"WxPayOrderNo:{pay.FirstOrDefault().WxPayOrderNo},RetryResult:{JsonConvert.SerializeObject(getResult)}";
                            if (getResult.Success)
                            {
                                result = dbScopeManager.Execute(conn => DALRebateConfig.UpdateStatusForCompleteV2(conn, pkid)) > 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            InsertLog(pkid.ToString(), "WxPayManualRetry", result ? "重试成功" : "重试失败", remarks, string.Empty, string.Empty, user);
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

        public List<RebateConfigModel> UpsertOldRebateApply(List<RebateConfigModel> data, string user)
        {
            var result = new List<RebateConfigModel>();
            try
            {
                dbScopeManager.CreateTransaction(conn =>
                {
                    foreach (var item in data)
                    {
                        item.RebateTime = item.CreateTime;
                        item.RebateMoney = 58;
                        item.Status = Status.Complete;
                        var pkid = DALRebateConfig.InsertRebateConfig(conn, item);
                        result.Add(new RebateConfigModel() { OrderId = item.OrderId, Remarks = "插入成功" });
                        InsertLog(pkid.ToString(), "UploadFile", pkid > 0 ? "导入成功" : "导入失败", JsonConvert.SerializeObject(item), string.Empty, string.Empty, user);
                    }
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
