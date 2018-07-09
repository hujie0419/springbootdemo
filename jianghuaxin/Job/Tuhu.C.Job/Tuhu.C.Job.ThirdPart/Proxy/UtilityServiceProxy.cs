using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Request;

namespace Tuhu.C.Job.ThirdPart.Proxy
{
    public class UtilityServiceProxy
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(UtilityServiceProxy));
        /// <summary>
        /// 批量发短信接口
        /// </summary>
        /// <param name="cellphones"></param>
        /// <param name="TemplateId"></param>
        /// <param name="TemplateArguments"></param>
        /// <param name="Host"></param>
        /// <param name="UserIp"></param>
        /// <param name="UserData"></param>
        /// <param name="NeedSentTime"></param>
        /// <returns></returns>
        public static async Task<bool> SendSmsAsync(IEnumerable<string> cellphones, int templateId,
            string[] templateArguments, string host, string userIp, string userData, DateTime? needSentTime)
        {
            var code = -1;
            var msg = string.Empty;
            try
            {

                using (var client = new SmsClient())
                {
                    var sendResult = await client.SendBatchSmsAsync(new SendBatchCellphoneSmsRequest()
                    {
                        Cellphones = cellphones,
                        TemplateId = templateId,
                        TemplateArguments = templateArguments,
                        Host = host,
                        UserIp = userIp,
                        UserData = userData,
                        NeedSentTime = needSentTime
                    });
                    sendResult.ThrowIfException(true);
                    code = sendResult.Result;
                    if (code < 0)
                    {
                        msg = GetSmsMsgByCode(code);
                        Logger.Error($"发送短信失败，返回结果：code:{code},msg:{msg}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"发送短信失败，返回结果：code:{code},msg:{msg}", ex);
            }

            return code > 0;
        }
        /// <summary>
        ///   -503：请求数量太多，一次最多1000个手机号，-120：请求参数异常，-110：短信有非营销短信，-100模板参数异常，-97：请求中有短信模板不存在，-96：短信内容超过512，>0 操作成功受影响的行数
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static string GetSmsMsgByCode(int code)
        {
            var result = string.Empty;
            switch (code)
            {
                case 0:
                    result = "发送全部失败";
                    break;
                case -1:
                    result = "服务异常";
                    break;
                case -503:
                    result = "请求数量太多";
                    break;
                case -120:
                    result = "请求参数异常";
                    break;
                case -110:
                    result = "短信有非营销短信";
                    break;
                case -100:
                    result = "模板参数异常";
                    break;
                case -97:
                    result = "请求中有短信模板不存在";
                    break;
                case -96:
                    result = "短信内容超过512";
                    break;
                default:
                    result = "未知错误";
                    break;
            }

            return result;
        }
    }
}
