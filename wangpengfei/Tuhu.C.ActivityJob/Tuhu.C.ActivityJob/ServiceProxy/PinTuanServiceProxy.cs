using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.PinTuan;
using Tuhu.Service.PinTuan.Models;

namespace Tuhu.C.ActivityJob.ServiceProxy
{
    public class PinTuanServiceProxy
    {
        private static readonly ILog _logger = LogManager.GetLogger<PinTuanServiceProxy>();

        public static bool NotifyEmployee(NotifyEmployeeRequest request)
        {
            try
            {
                using (var pinTuanClient = new PinTuanClient())
                {
                    var pinTuanResult = pinTuanClient.NotifyEmployee(request);
                    pinTuanResult.ThrowIfException(true);
                    return pinTuanResult.Result;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"发送企业微信通知失败：{request.ErrorMessage}", ex);
                return false;
            }
        }
    }
}
