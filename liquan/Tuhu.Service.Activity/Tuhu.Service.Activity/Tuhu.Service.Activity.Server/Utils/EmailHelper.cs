using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.EmailProcess;
using Tuhu.Service.EmailProcess.Model;

namespace Tuhu.Service.Activity.Server.Utils
{
    public static class EmailHelper
    {
        public static readonly ILog Logger = LogManager.GetLogger(typeof(EmailHelper));

        public static void SendEmailNotification(string toEmail, string cc, string message, string body)
        {
            var email = new InsertSMSModel()
            {
                url = string.Empty,
                ToMail = toEmail,
                CC = cc,
                Status = "New",
                Type = "Email",
                Subject = message,
                Body = body,
                OrderNo = string.Empty
            };

            try
            {
                using (var client = new EmailProcessOperationClient())
                {
                    var result = client.InsertEmail(email);
                    if (!result.Success && result.Exception != null)
                        throw result.Exception;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("发送邮件接口异常", ex);
            }
        }
    }
}
