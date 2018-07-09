using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.Component.Framework;
using Tuhu.Service.Utility;
using Tuhu.Service.Utility.Request;

namespace Tuhu.Provisioning.Business.Sms
{
    public class SmsManager
    {
        /// <summary>
        /// logger
        /// </summary>
        private static readonly ILog Logger = LoggerFactory.GetLogger("SmsManager");
        private const int _batchSize = 800;
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
        public static async Task<Tuple<int, string>> SendBatchSmsAsync(IEnumerable<string> cellphones, int templateId,
            string[] templateArguments, string host, string userIp, string userData, DateTime? needSentTime)
        {
            var code = -1;
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
                        Logger.Log(Level.Error, $"发送短信失败，返回结果：{code}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex, $"批量发短信异常");
            }
            var msg = string.Empty;
            if (code <= 0)
            {
                msg = GetSmsMsgByCode(code);
            }
            else
            {
                msg = "成功";
            }

            return new Tuple<int, string>(code, msg);
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
        /// <summary>
        /// 发送模板短信
        /// </summary>
        /// <param name="cellphone">手机号</param>
        /// <param name="templateId">模板id</param>
        /// <param name="parameters">模板参数</param>
        /// <returns></returns>
        public static async Task<bool> SendTemplateSmsMessageAsync(string cellphone, int templateId, params object[] parameters)
        {
            if (String.IsNullOrWhiteSpace(cellphone) || templateId <= 0)//请求参数异常，手机号为空或模板id异常
            {
                return false;
            }
            try
            {
                using (var client = new SmsClient())
                {
                    var result = await client.SendSmsAsync(cellphone, templateId, parameters.Select(s => s.ToString()).ToArray());
                    // 记录 elk 日志
                    Logger.Log(Level.Info, $"SendTemplateSmsMessageAsync 发送模板短信：手机号：{cellphone},模板id：{templateId}，参数：{String.Join(",", parameters)}，响应信息：{JsonConvert.SerializeObject(result)}");
                    if (result.Success)
                    {
                        return result.Result > 0;
                    }
                    else
                    {
                        Logger.Log(Level.Error, $"SendTemplateSmsMessageAsync 发送模板短信发生异常，手机号：{cellphone}，异常信息：{result.Exception?.ToFriendlyString() ?? result.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, $"SendTemplateSmsMessageAsync 调用发送模板短信服务异常，手机号：{cellphone}，异常信息：{ex.ToFriendlyString()}");
                return false;
            }
            return false;
        }

        /// <summary>
        /// 发送验证码短信
        /// </summary>
        /// <param name="cellphone">手机号</param>
        /// <param name="verifyCode">验证码</param>
        /// <param name="ip">获取验证码客户端Ip地址</param>
        /// <param name="expireMinutes">有效时间（分钟），默认5分钟内有效</param>
        /// <param name="host">发送短信host</param>
        /// <returns></returns>
        public static async Task<bool> SendVerificationCodeSmsMessageAsync(string cellphone, string verifyCode, string ip, int expireMinutes = 5, string host = "setting.tuhu.cn")
        {
            if (String.IsNullOrWhiteSpace(cellphone) || String.IsNullOrWhiteSpace(verifyCode))
            {
                return false;
            }
            var model = new SendVerificationCodeRequest
            {
                Cellphone = cellphone,
                ChannelType = VerificationCodeChannelTypes.Default,
                VerificationCode = verifyCode,
                Expires = expireMinutes,
                Host = host,
                UserIp = ip
            };
            try
            {
                using (var client = new SmsClient())
                {
                    var result = await client.SendVerificationCodeAsync(model);
                    // 记录 elk 日志
                    Logger.Log(Level.Info, $"SendVerificationCodeSmsMessageAsync 发送验证码短信：手机号：{cellphone},验证码：{verifyCode}，过期时间：{expireMinutes.ToString()}，响应信息：{JsonConvert.SerializeObject(result)}");
                    if (result.Success)
                    {
                        return result.Result > 0;
                    }
                    else
                    {
                        Logger.Log(Level.Error, "SendVerificationCodeSmsMessageAsync 发送验证码短信失败，手机号：{0}，异常信息：{1}", cellphone, result.Exception?.ToFriendlyString() ?? result.ErrorMessage);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, "SendVerificationCodeSmsMessageAsync 发送验证码短信异常，手机号：{0}，异常信息：{1}", cellphone, ex.ToFriendlyString());
                return false;
            }
        }

        public static async Task<bool> SubmitVerficationCodeAsync(string cellphone, string verifyCode)
        {
            if (String.IsNullOrWhiteSpace(cellphone) || String.IsNullOrWhiteSpace(verifyCode))
            {
                return false;
            }
            try
            {
                using (var client = new SmsClient())
                {
                    var result = await client.SubmitVerificationCodeAsync(cellphone, verifyCode);
                    // 记录 elk 日志
                    Logger.Log(Level.Info, $"SubmitVerificationCodeAsync 提交短信验证码：手机号：{cellphone},验证码：{verifyCode}，响应信息：{JsonConvert.SerializeObject(result)}");
                    if (result.Success && result.Result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        Logger.Log(Level.Error, "SubmitVerificationCodeAsync 提交短信验证码失败，手机号：{0}，异常信息：{1}", cellphone, result.Exception?.ToFriendlyString() ?? result.ErrorMessage);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, "SendVerificationCodeSmsMessageAsync 发送验证码短信异常，手机号：{0}，异常信息：{1}", cellphone, ex.ToFriendlyString());
                return false;
            }
        }
    }
}