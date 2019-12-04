using Common.Logging;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Tuhu.C.ActivityJob.Dal.OrderCashBack;
using Tuhu.C.ActivityJob.Dal.ZeroActivity;
using Tuhu.C.ActivityJob.ServiceProxy;

namespace Tuhu.C.ActivityJob.Job.OrderCashBack
{
    /// <summary>
    /// 项目：订单返现助力
    /// 内容：发起助力失败推送
    /// Job执行时间:每小时一次
    /// 创建人：黎先攀 
    /// </summary>
    public class OrderCashBackFailPushJob : IJob
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(OrderCashBackFailPushJob));
        protected static readonly string jobName = "OrderCashBackFailPushJob";

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info($"{jobName}开始执行");

            Stopwatch timer = new Stopwatch();
            timer.Start();
            AsyncHelper.RunSync(() => DoJob());

            timer.Stop();

            Logger.Info($"{jobName} 执行结束执行,执行时间:{timer.ElapsedMilliseconds}");
        }

        private static async Task DoJob()
        {
            try
            {
                //1.获取助力失败的发起记录
                var failOwnerList = OrderCashBackDal.GetFailPushOwnerList();
                if (failOwnerList?.Count > 0)
                {
                    foreach (var itemList in failOwnerList.Split(20))
                    {
                        var setList = new List<int>();
                        foreach (var item in itemList)
                        {
                            //2.推送
                            var userList = new List<string> { item.UserId.ToString() };
                            var pushResult = await TemplatePushServiceProxy.PushByUserIDAndBatchIDAsync(userList, 7449,
                                     new Service.Push.Models.Push.PushTemplateLog()
                                     {
                                         Replacement = JsonConvert.SerializeObject(new Dictionary<string, string>()
                                         {
                                             ["{{ordernumber}}"] = item.OrderId.ToString(),
                                             ["{{shareId}}"] = item.ShareId.ToString()
                                         })
                                     });
                            if (pushResult)
                            {
                                setList.Add(item.PKID);
                            }
                            else
                            {
                                Logger.Info($"{jobName} 用户订单返现助力失败推送 ->失败{item.PKID}");
                            }
                        }

                        if (setList?.Any() ?? false)
                        {
                            //3.更推送状态 
                            var setDbCount = OrderCashBackDal.SetIsFailPush(setList);
                            if (setDbCount <= 0)
                            {
                                Logger.Warn($"{jobName} 推送后更新状态失败{string.Join(",", setList)}");
                            }
                        }
                    }

                    Logger.Info($"{jobName} 用户订单返现助力失败 数量:{failOwnerList.Count}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{jobName} 异常:{ex.Message + ex.StackTrace}", ex);
            }
        }

    }
}
