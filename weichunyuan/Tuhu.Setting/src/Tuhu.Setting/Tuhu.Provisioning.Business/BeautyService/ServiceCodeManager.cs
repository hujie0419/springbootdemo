using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.Business.BeautyCode;
using Tuhu.Provisioning.Business.KuaiXiuService;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.Models;
using Tuhu.Provisioning.DataAccess.DAO.BeautyServicePackageDal;
using Tuhu.Service.KuaiXiu.Enums;
using Tuhu.Service.KuaiXiu.Models;

namespace Tuhu.Provisioning.Business.BeautyService
{
    public class ServiceCodeManager
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ServiceCodeManager));
        public async Task<ResultModel<string>> RevertServiceCode(string batchCode, string channel, string source, string user)
        {
            var result = new ResultModel<string> { IsSuccess = false };

            try
            {
                var kuaiXiuService = new KuaiXiuService.KuaiXiuService();
                if (!string.IsNullOrEmpty(batchCode))
                {
                    var serviceCodes = BeautyServicePackageDal.SelectServiceCodesByBatchCode(batchCode);
                    if (serviceCodes != null && serviceCodes.Any())
                    {
                        var beautyCodeManager = new BeautyCodeManager();
                        var taskDetail = beautyCodeManager.GetBeautyCodeStatistics(new List<string>() { batchCode });
                        var codeTask = taskDetail.FirstOrDefault();
                        var buyOutOrderId = codeTask?.BuyoutOrderId;
                        var serviceCodeDetails = new List<ServiceCode>();
                        for (var index = 0; index < (serviceCodes.Count() + 127) / 128; index++)
                        {
                            var codeItems = serviceCodes.Skip(index * 128).Take(128);
                            var item = await SearchCodeManager.GetServiceCodeDetailsByCodes(codeItems);
                            serviceCodeDetails.AddRange(item);
                        }
                        var avaiableServiceCodes = serviceCodeDetails.Where(s => (s.Status == ServiceCodeStatusType.Created || s.Status == ServiceCodeStatusType.SmsSent) && !string.Equals(s.Source, "VOLRevert")).Select(t => t.Code);
                        var avaiableCount = avaiableServiceCodes.Count();
                        if (buyOutOrderId > 0 && avaiableCount < serviceCodeDetails.Count())//如果是买断，并且有部分核销，则不能作废服务码
                        {
                            result.IsSuccess = false;
                            result.Msg += "当前批次服务码中已经有部分核销，不能作废当前批次";
                        }
                        else
                        {
                            for (var index = 0; index < (avaiableCount + 127) / 128; index++)
                            {
                                var codeItems = avaiableServiceCodes.Skip(index * 128).Take(128);
                                var revertResult = await kuaiXiuService.RevertServiceCodes(codeItems, channel, source);
                                if (!revertResult)
                                {
                                    throw new Exception($"作废失败,部分服务码已使用或已作废");
                                }
                            }
                            beautyCodeManager.UpdateBeautyCodeTaskStatus(batchCode, "Reverted");
                            if (buyOutOrderId > 0)
                            {
                                result.IsSuccess = false;
                                result.Msg += "未核销服务码作废完成,订单作废请联系业务系统研发手动处理";
                                //var serviceCodeConfig = BeautyServicePackageManager.GetBeautyServicePackageDetail(codeTask.MappingId);
                                //var cooperateUser = new BankMRManager().FetchMrCooperateUserConfigByPKID(serviceCodeConfig?.CooperateId ?? -1);
                                //if (cooperateUser != null)
                                //{
                                //    var revertOrderResult = await OrderServiceProxy.OrderServiceProxy.CancelOrder(new Service.Order.Request.CancelOrderRequest()
                                //    {
                                //        OrderId = Convert.ToInt32(buyOutOrderId),
                                //        UserID = cooperateUser.VipUserId,
                                //        Remark = $"运营回滚服务码,服务码批次号:{batchCode}",
                                //        FirstMenu = "运营",
                                //        SecondMenu = "服务码取消"
                                //    });
                                //    if (!revertOrderResult.IsSuccess)
                                //    {
                                //        throw new Exception($"服务码作废成功,2B订单:{buyOutOrderId},取消订单失败,请联系业务系统研发手动作废");
                                //    }
                                //}
                                //else
                                //{
                                //    throw new Exception($"查不到合作用户,MappingId:{codeTask.MappingId}");
                                //}
                            }
                            else
                            {
                                result.IsSuccess = true;
                                result.Msg = "未核销服务码作废完成";
                            }
                            if (avaiableCount > 0)
                            {
                                var log = new DataAccess.Entity.BeautyOprLog
                                {
                                    LogType = "RevertServiceCode",
                                    IdentityID = $"{batchCode}",
                                    OldValue = null,
                                    NewValue = null,
                                    Remarks = $"根据批次号作废服务码",
                                    OperateUser = user,
                                };
                                LoggerManager.InsertLog("BeautyOprLog", log);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("当前批次没有服务码");
                    }     
                }            
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
            }       

            return result;
        }

        public async Task<ResultModel<string>> RevertServiceCode(List<string> codes, string channel, string source)
        {
            var result = new ResultModel<string> { IsSuccess = false };

            try
            {
                var kuaiXiuService = new KuaiXiuService.KuaiXiuService();
                if (codes != null && codes.Any())
                {
                    var revertResult = await kuaiXiuService.RevertServiceCodes(codes, channel, source);
                    if (!revertResult)
                    {
                        throw new Exception($"批量回滚服务码失败,codes:{JsonConvert.SerializeObject(codes)}");
                    }
                }
                result.IsSuccess = true;
                result.Msg = "成功";
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                result.Msg = ex.Message;
            }

            return result;
        }
    }
}
