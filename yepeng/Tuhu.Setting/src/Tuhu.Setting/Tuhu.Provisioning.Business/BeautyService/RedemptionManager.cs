using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Provisioning.Business.GroupBuyingService;
using Tuhu.Provisioning.Business.Logger;
using Tuhu.Provisioning.Business.Models;

namespace Tuhu.Provisioning.Business.BeautyService
{
    public class RedemptionManager
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RedemptionManager));
        public async Task<ResultModel<string>> InvalidateRedemptionCode(string batchCode, string remark, string user)
        {
            var result = new ResultModel<string>() { IsSuccess = false };
            try
            {
                var groupBuyingService = new GroupBuyingService.GroupBuyingService();
                var codes = await groupBuyingService.GetBatchRedemptionCodes(batchCode);
                if (codes != null && codes.Any())
                {

                    for (var index = 0; index < (codes.Count() + 127) / 128; index++)
                    {
                        var codeItems = codes.Skip(index * 128).Take(128);
                        var serviceResult = await groupBuyingService.InvalidateRedemptionCode(codes.Select(s => s.Code).ToList(), "settting站点作废兑换码");
                        if (!serviceResult.Success)
                        {
                            throw new Exception($"作废兑换码失败,Msg:{serviceResult.ErrorMessage}");
                        }
                    }
                    ///暂时配置不支持大买断，取消订单todo
                    result.IsSuccess = true;
                    result.Msg = "成功";
                    var log = new DataAccess.Entity.BeautyOprLog
                    {
                        LogType = "InvalidateRedemptionCode",
                        IdentityID = $"{batchCode}",
                        OldValue = null,
                        NewValue = null,
                        Remarks = $"根据批次号作废通用兑换码",
                        OperateUser = user,
                    };
                    LoggerManager.InsertLog("BeautyOprLog", log);
                }
                else
                {
                    throw new Exception("当前批次没有兑换码");
                }
            }
            catch (Exception ex)
            {
                result.Msg = ex.Message;
            }
          
            return result;
        }
    }
}
