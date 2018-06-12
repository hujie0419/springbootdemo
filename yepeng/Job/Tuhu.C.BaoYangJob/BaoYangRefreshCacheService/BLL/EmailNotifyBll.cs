using BaoYangRefreshCacheService.DAL;
using BaoYangRefreshCacheService.Model;
using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.ConfigLog;

namespace BaoYangRefreshCacheService.BLL
{
    public class EmailNotifyBll
    {
        private static readonly string host = ConfigurationManager.AppSettings["Parts_Host"];
        private ILog logger;
        public EmailNotifyBll(ILog logger)
        {
            this.logger = logger;
        }

        private void SendEmail(string subject, string body, List<string> tos)
        {
            var email = new MailMessage
            {
                Body = body,
                Subject = subject,
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                Priority = MailPriority.High,
            };
            tos.ForEach(to => email.To.Add(to));

            using (var smtpClient = new SmtpClient())
            {
                try
                {
                    smtpClient.Send(email);
                }
                catch (Exception ex)
                {
                    throw new Exception($"邮件({subject})发送失败, {ex.Message}", ex);
                }
            }
        }

        private void SendEmailByConfig(PartsNotifyReportConfig config, DateTime startTime, DateTime endTime)
        {
            var subject = $"【适配站点自动化报表】-{DateTime.Now.ToString("yyyy-MM-dd")}-{config.ReportName}";
            var url = $"{host}/OpenReport/{config.ReportType}";
            var html = string.Empty;
            using (var client = new HttpClient())
            {
                var response = client.PostAsJsonAsync(url, new
                {
                    authSecret = config.AuthSecret,
                    startTime,
                    endTime = endTime.AddSeconds(-1)
                }).Result;
                var responseStr = response.ReadAsAsync<string>().Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    html = responseStr;
                }
                else
                {
                    throw new Exception($"请求响应Code:{(int)response.StatusCode}, Error:{response.StatusCode.ToString()}");
                }
            }
            if (!string.IsNullOrEmpty(html))
            {
                var tos = config.NotifyUsers.Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                SendEmail(subject, html, tos);
                InsertSendEmailLog(config.ReportType, config.NotifyUsers, "", endTime);
                Thread.Sleep(2000);
            }
        }

        private void SendEmailByConfig(PartsNotifyReportConfig config)
        {
            var result = EmailNotifyDal.GetRencentlyExecuteTime(config.ReportType);
            var now = DateTime.Now.Date;
            var startTime = now;
            var endTime = now;
            if (result.HasValue)
            {
                startTime = result.Value;
                switch (config.Frequency)
                {
                    case Frequency.Day:
                        var ts = endTime - startTime;
                        if (ts.TotalDays >= 1)
                        {
                            startTime = endTime.AddDays(-1);
                        }
                        else
                        {
                            endTime = startTime.AddDays(1);
                        }
                        break;
                    case Frequency.Week:
                        endTime = startTime.AddDays(7);
                        break;
                    case Frequency.Month:
                        endTime = startTime.AddMonths(1);
                        break;
                }
            }
            else
            {
                switch (config.Frequency)
                {
                    case Frequency.Day:
                        startTime = endTime.AddDays(-1);
                        break;
                    case Frequency.Week:
                        var days = now.DayOfWeek - DayOfWeek.Sunday;
                        endTime = endTime.AddDays(-days);
                        startTime = endTime.AddDays(-7);
                        break;
                    case Frequency.Month:
                        endTime = new DateTime(startTime.Year, startTime.Month, 1);
                        startTime = endTime.AddMonths(-1);
                        break;
                }
            }

            if (DateTime.Now >= endTime)
            {
                SendEmailByConfig(config, startTime, endTime);
            }
        }

        public void SendEmails()
        {
            var configs = SelectPartsNotifyReportConfig();
            configs.ForEach(config =>
            {
                try
                {
                    SendEmailByConfig(config);
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message, ex);
                }
            });
        }

        private List<PartsNotifyReportConfig> SelectPartsNotifyReportConfig()
        {
            var result = EmailNotifyDal.SelectPartsNotifyReportConfig();
            return result;
        }

        private void InsertSendEmailLog(string reportType, string notifyUsers, string reportContent, DateTime reportTime)
        {
            using (var client = new ConfigLogClient())
            {
                var data = JsonConvert.SerializeObject(new
                {
                    ReportType = reportType,
                    NotifyUsers = notifyUsers,
                    ReportContent = reportContent,
                    ReportTime = reportTime
                });
                var type = "PartsNotifyRptHist";

                var status = client.InsertDefaultLogQueue(type, data);
                if (!status.Success)
                {
                    status.ThrowIfException(true);
                }
            }
        }
    }
}
