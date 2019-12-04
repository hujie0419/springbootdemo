using Common.Logging;
using Quartz;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Tuhu.C.ActivityJob.Dal.ZeroActivity;
using Tuhu.C.ActivityJob.ServiceProxy;

namespace Tuhu.C.ActivityJob.Job.ZeroActivity
{
    /// <summary>
    /// 描述：众测活动 用户中奖后23天没提交评测报告的 发送短信通知
    /// Job执行时间:每天下午3点
    /// 创建人：黎先攀
    /// 创建时间：2019-05-18
    /// </summary>
    public class ZeroActivityMessageJob : IJob
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(ZeroActivityMessageJob));
        protected static readonly string jobName = "ZeroActivityMessageJob";

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info($"{jobName}开始执行");

            Stopwatch timer = new Stopwatch();
            timer.Start();

            DoJob();

            timer.Stop();
            Logger.Info($"{jobName}=> 执行时间:{timer.ElapsedMilliseconds}");

            Logger.Info($"{jobName} 执行结束执行");
        }

        private static void DoJob()
        {
            //1.获取中奖23天后未提交评测报告、未发送短信的用户
            var applyList = ZeroActivityDal.GetNeedMessageApplyList();
            var sentList = new List<int>();
            if (applyList?.Count > 0)
            {
                foreach (var itemList in applyList.Split(20))
                {
                    foreach (var item in itemList)
                    {
                        //2.发送短信
                        var sendResult = SmsServiceProxy.SendSms(item.UserMobileNumber, 1410);//1410
                        if (sendResult)
                        {
                            sentList.Add(item.PKID);
                        }
                        else
                        {
                            Logger.Warn($"{jobName}发送短信失败,pkid:{item.PKID},{item.UserMobileNumber}");
                        }
                    }

                    if (sentList?.Any() ?? false)
                    {
                        Logger.Info($"{jobName} 已发送短信{string.Join(",", sentList)}");
                        //3.更改短信发送状态 
                        var setDbCount = ZeroActivityDal.SetMessageSendStatus(sentList);
                        if (setDbCount <= 0)
                        {
                            Logger.Warn($"{jobName} 发送短信后更新状态失败{string.Join(",", sentList)}");
                        }
                    }
                }

            }
        }

    }
}
