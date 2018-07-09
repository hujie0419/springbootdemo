using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.C.Job.Models;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class OrderGroupBuyJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OrderGroupBuyJob));

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("启动任务");
            AsyncHelper.RunSync(TaskMethod);
            Logger.Info("结束任务");
        }

        public async Task TaskMethod()
        {
            try
            {
                string sql = @"SELECT * FROM Activity.dbo.tbl_OrderGroupBuy WITH (NOLOCK) WHERE OrderStatus=1 AND (IsPush =1 OR IsPush IS NULL)";

                var list = DbHelper.ExecuteSelect<OrderGroupBuyModel>(true, sql);
                if (list != null)
                {
                    foreach (var item in list)
                        using (var productClient = new Tuhu.Service.Product.ProductClient())
                        using (var orderClient = new Tuhu.Service.Order.OrderQueryClient())
                        {
                            var orderResult = orderClient.FetchOrderByOrderIdAsync(Convert.ToInt32(item.OrderID.Replace("TH", "")));
                            var productResult = productClient.SelectProductDetailAsync(new List<string>() { item.PID });
                            await Task.WhenAll(productResult, orderResult);
                            if (orderResult.Result.Success && orderResult?.Result?.Result?.SubmitDate.ToString("yyyyMMdd") != "00010101")
                            {
                                UpdateOrderGroupIsPush(item.ID);
                                await PushOrderFinish(item.UserId, productResult?.Result?.Result?.FirstOrDefault()?.DisplayName, orderResult.Result.Result.SumMoney, item.OrderID);
                            }
                        }
                }
            }
            catch (Exception em)
            {
                Logger.Error(em.Message, em);
            }
        }


        public static async Task PushOrderFinish(Guid userId, string productName, decimal money, string orderId)
        {
            List<string> target = new List<string> { userId.ToString() };


            using (var client = new Tuhu.Service.Push.TemplatePushClient())
            {
                var result = await client.PushByUserIDAndBatchIDAsync(target, 798, new Service.Push.Models.Push.PushTemplateLog()
                {
                    Replacement = Newtonsoft.Json.JsonConvert.SerializeObject(new Dictionary<string, string>()
                    {
                        { "{{keyword1.DATA}}",productName},
                        { "{{keyword2.DATA}}",string.Format("{0:f}",money)},
                        { "{{keyword3.DATA}}",orderId},
                        { "{{keyword4.DATA}}","\"恭喜您拼团成功，我们正在火速为您安排发货~\""},
                        { "{{orderid}}",orderId}
                    }),
                    DeviceType = Service.Push.Models.Push.DeviceType.WeChat
                });
                result.ThrowIfException(true);
            }
        }

        public static void UpdateOrderGroupIsPush(int ID)
        {
            string sql = "UPDATE Activity.dbo.tbl_OrderGroupBuy SET IsPush=2 WHERE ID="+ID;
            DbHelper.ExecuteNonQuery(sql);
        }

    }
}
