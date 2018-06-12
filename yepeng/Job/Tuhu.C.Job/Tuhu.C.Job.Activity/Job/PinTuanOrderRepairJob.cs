using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.Job.Activity.Dal;
using Tuhu.C.Job.ActivityMonitor.BLL;
using Tuhu.Service.PinTuan;

namespace Tuhu.C.Job.Activity.Job
{
    [DisallowConcurrentExecution]
    public class PinTuanOrderRepairJob : IJob
    {
        private static ILog Logger = LogManager.GetLogger<PinTuanOrderRepairJob>();
        public void Execute(IJobExecutionContext context)
        {
            var watcher = new Stopwatch();
            watcher.Start();
            Logger.Info($"拼团异常订单处理Job");
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Error($"拼团异常订单处理Job异常：{ex}");
            }
            watcher.Stop();

            Logger.Info($"拼团异常订单处理Job完成-->{watcher.ElapsedMilliseconds}毫秒");
        }
        public void DoJob()
        {
            var orderList = (DalGroupBuying.GetPinTuanOrderList()).Distinct()?.ToList() ?? new List<int>();
            if (orderList.Any())
            {
                Logger.Warn($"查出{orderList.Count}个异常订单待补偿-->{string.Join("/", orderList)}");
                using (var client = new PinTuanClient())
                {
                    var result = client.RepairPinTuanOrderStatus(orderList);
                    if (result.Success && !string.IsNullOrWhiteSpace(result.Result.Info))
                    {
                        Logger.Info(result.Result.Info);
                        Email(result.Result.Info);
                    }
                    else
                    {
                        Logger.Error($"拼团异常订单处理失败-->{string.Join("/", orderList)}");
                    }
                }
            }
        }

        private void Email(string info)
        {
            var data = info.Split(';')?.ToList();
            StringBuilder sbr = new StringBuilder();
            sbr.Append($@"<label> {data.Count}个异常拼团订单处理 </label> ");
            sbr.Append(@"<table border=1" + " align=center" + " width =1000 " + "><tr bgcolor='#4da6ff'><td align=center><b>OrderId</b></td><td align=center><b>PID</b></td> </tr>");
            foreach (var item in data)
            {
                var dat = new List<int>();
                var childItem = item.Split(':').ToList();
                if (childItem.Any())
                {
                    if (childItem.Count == 1) childItem.Add("");
                    sbr.Append("<tr>\n");
                    sbr.Append($"<td style='text-align:center'>{childItem[0]}</td>");
                    sbr.Append($"<td style='text-align:center'>{childItem[1]}</td>");
                    sbr.Append("</tr>");
                }
            }
            sbr.Append("</table>");
            sbr.Append(info);
            var data2 = sbr.ToString();
            TuhuMessage.SendEmail($"拼团异常订单补偿--{DateTime.Now.ToString("s")}", "fuwenbo@tuhu.cn", sbr.ToString());
        }
    }
}

