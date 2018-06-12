using Common.Logging;
using Quartz;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Tuhu.Service.ThirdParty;
using Tuhu.C.Job.ThirdPart.Dal;
using Tuhu.Service;

namespace Tuhu.C.Job.ThirdPart.Job
{
    [DisallowConcurrentExecution]
    public class HandleNotifyAliPayFailedOrder : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<HandleNotifyAliPayFailedOrder>();
        public void Execute(IJobExecutionContext context)
        {

            string failedOrderID =string.Empty ;
            var failedLogs = AlipayDal.GetFailedlogs();
            if (failedLogs != null && failedLogs.Count() > 0)
            {
                Logger.Info(string.Format("获得通知车主平台失败订单,log表THOrderID:{0}", string.Join(",", failedLogs.Select(x => x.THOrderID))));


                try
                {
                    using (var client = new AliPayServiceClient())
                    {
                        foreach (var log in failedLogs)
                        {
                            failedOrderID = log.THOrderID;
                            if (!string.IsNullOrWhiteSpace(log.billNo))
                            {
                                if ((int)log.OrderStatus == 7)
                                {
                                    client.ModifyOrderStatus(Convert.ToInt32(log.THOrderID), "7");
                                }
                                else if ((int)log.OrderStatus == 8)
                                {
                                    client.ModifyOrderStatus(Convert.ToInt32(log.THOrderID), "8");
                                }
                                else if ((int)log.OrderStatus == 55)
                                {
                                    client.ModifyOrderStatus(Convert.ToInt32(log.THOrderID), "55");
                                }
                            }
                        }
                    }
                }


                catch (Exception ex)
                {
                    Logger.Error(string.Format("失败订单重新通知车主平台.THOrderID:{0},ex:{1}", failedOrderID, ex));
                }


                    
                }
                
            }
        



        private string HttpGet(string requrl)
        {
            using (var client = new WebClient())
            {
                string result = client.DownloadString(requrl);
                return result;
            }
        }
    }
}
