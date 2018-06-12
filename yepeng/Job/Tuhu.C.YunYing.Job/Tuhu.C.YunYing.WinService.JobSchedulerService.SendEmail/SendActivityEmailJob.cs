using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Linq;
using Common.Logging;
using Quartz;
using Tuhu.C.YunYing.WinService.JobSchedulerService.SendEmail.Comm;
using Tuhu.WebSite.Component.SystemFramework;
using Tuhu.C.YunYing.WinService.JobSchedulerService.SendEmail.DAL;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.SendEmail
{
    [DisallowConcurrentExecution]
    public class SendActivityEmailJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<SendActivityEmailJob>();
        //private const string SendEmail = "diaojingwen@tuhu.cn;lingyuxiao@tuhu.cn;feifei@tuhu.cn;liuyangyang@tuhu.cn";
        private static readonly string SendEmail =
            ConfigurationManager.AppSettings["Email:Activity"];
        //"lixin@cheche365.com;feifei@tuhu.cn".;
        private const string FileEx = "途虎车车车险活动用户手机号_{0}";

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Logger.Info("SendActivityEmailJob：开始执行");
                if (string.IsNullOrEmpty(SendEmail))
                {
                    Logger.Info("SendActivityEmailJob：没有需要发送的邮箱");
                    return;
                }
                var dt = SendActivityEmailJobDal.GetSendUserPhone();
                if (dt == null || dt.Rows.Count == 0)
                {
                    Logger.Info("SendActivityEmailJob：没有需要发送的数据");
                    return;
                }
                if (dt.Rows.Count == ExportExcelHelper.MaxRows)
                {
                    //单次5W  ExportExcelHelper.MaxRows=5W 记录日志，可能下一个批次发送
                    Logger.Info($"SendActivityEmailJob：当前数据为{dt.Rows.Count}超出单次最大发送数{ExportExcelHelper.MaxRows},可能分批次发送");
                }

                var data = ExportExcelHelper.ExportExcel(dt);

                var idList = new List<int>(dt.Rows.Count);
                idList.AddRange(from DataRow dr in dt.Rows select int.Parse(dr["id"].ToString()));
                var startTime = (from DataRow dr in dt.Rows select DateTime.Parse(dr["CreateDateTime"].ToString())).Min();
                var endTime = (from DataRow dr in dt.Rows select DateTime.Parse(dr["CreateDateTime"].ToString())).Max();

                var fileName = startTime.ToString("yyyyMMdd-HHmm") + "_" + endTime.ToString("yyyyMMdd-HHmm");

                using (var ms = new MemoryStream(data))
                {
                    TuhuEmail.SendImmediatelyEmail(string.Format(FileEx, fileName), SendEmail, "", new Attachment(ms, fileName + ".xlsx"));
                }
                var result = SendActivityEmailJobDal.UpdateSendStatus(idList);

                Logger.Info(result == 1 ?
                    $"SendActivityEmailJob：发送成功" :
                    $"SendActivityEmailJob：发送失败=》{result}");
            }
            catch (Exception ex)
            {
                Logger.Info($"SendActivityEmailJob：发送异常=》{ex}");
            }
        }
    }
}
