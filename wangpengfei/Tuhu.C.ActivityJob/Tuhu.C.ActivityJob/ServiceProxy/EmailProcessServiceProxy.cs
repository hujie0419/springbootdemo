using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Newtonsoft.Json;
using Tuhu.Service.EmailProcess;
using Tuhu.Service.EmailProcess.Model;

namespace Tuhu.C.ActivityJob.ServiceProxy
{
    public class EmailProcessServiceProxy
    {
        private static readonly ILog _logger = LogManager.GetLogger<EmailProcessServiceProxy>();

        public static void InsertEmail(InsertSMSModel request)
        {
            try
            {
                using (var emailClient = new EmailProcessOperationClient())
                {
                    var insertResult = emailClient.InsertEmail(request);
                    insertResult.ThrowIfException(true);

                    _logger.Info($"邮件发送状态：{insertResult.Success}，" +
                        $"内容：{JsonConvert.SerializeObject(request)}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"发送邮件通知失败：{JsonConvert.SerializeObject(request)}", ex);
            }
        }
    }
}
