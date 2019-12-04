using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Utility;

namespace Tuhu.C.ActivityJob.ServiceProxy
{

    /// <summary>
    /// 短信通知代理
    /// </summary>
    public class SmsServiceProxy
    {
        private static readonly ILog _logger = LogManager.GetLogger<SmsServiceProxy>();
        private static readonly string jobName = "SmsServiceProxy";


        /// <summary>
        /// 发送短信通知
        /// </summary>
        /// <param name="cellphone">手机号</param>
        /// <param name="templateId">模板Id</param>
        /// <param name="args">模板参数</param>
        /// <returns></returns>
        public static bool SendSms(string cellphone, int templateId, params string[] args)
        {
            try
            {
                using (var smsClient = new SmsClient())
                {
                    var sendResult = smsClient.SendSms(
                        new Service.Utility.Request.SendTemplateSmsRequest
                        {
                            Cellphone = cellphone,
                            TemplateId = templateId,
                            TemplateArguments = args
                        });
                    if (!sendResult.Success || sendResult.Result < 1)
                    {
                        _logger.Warn(
                            $"{jobName} SmsClient => SendSmsAsync => {cellphone} {templateId} 失败。{sendResult.ErrorCode} {sendResult.ErrorMessage} {sendResult.Exception?.Message}");
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(
                    $" {jobName}异常 {cellphone} {templateId},{ex.Message + ex.StackTrace} ", ex);
            }
            return false;
        }
    }
}
