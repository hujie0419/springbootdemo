using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.ActivityJob.Dal.Rebate;
using Tuhu.C.ActivityJob.Models.Rebate;
using Tuhu.C.ActivityJob.ServiceProxy;
using Tuhu.Service.Pay.Models;

namespace Tuhu.C.ActivityJob.Job.Rebate
{
    /// <summary>
    /// 描述：朋友圈返现 - 支付状态为1或者2的返现数据的审核状态更新为已支付 JOB
    /// 创建人：姜华鑫
    /// Job执行时间：每天执行一次
    /// 创建时间：2019-08-06
    /// </summary>
    public class RebateUpdateStatusJob : IJob
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(RebateUpdateStatusJob));

        public void Execute(IJobExecutionContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            Logger.Info("朋友圈返现 - 支付状态为1或者2的返现数据的审核状态更新为已支付 JOB 开始执行");
            Run();
            stopwatch.Stop();
            Logger.Info($"朋友圈返现 - 支付状态为1或者2的返现数据的审核状态更新为已支付 JOB 结束执行,用时{stopwatch.Elapsed}");
        }

        private void Run()
        {
            try
            {
                var needUpdateCount = 0;//需要更新审核状态的数据条数
                var updateCount = 0;//实际更新审核状态的条数

                var rebateApllyingList = RebateDal.GetApplyingRebateList();//获取所有待审核的数据
                var rebateActivityList = RebateDal.GetRebateApplyPageConfigList(rebateApllyingList?.Select(p => p.ActivityId)?.Distinct()?.ToList());

                //筛选出线上返现活动下的返现数据
                var rebateApplyingNewList = rebateApllyingList.Where(p => rebateActivityList.Exists(q => q.ActivityId == p.ActivityId))?.ToList();

                if (rebateApplyingNewList != null && rebateApplyingNewList.Any())
                {
                    var payList = new List<WxPayStatusItem>();
                    var batchRebateApplyingList = rebateApplyingNewList.Split(499).ToList();
                    Parallel.For(0, batchRebateApplyingList.Count, new ParallelOptions() { MaxDegreeOfParallelism = 4 }, (i) =>
                    {
                        var payInfo = PayServiceProxy.QueryWxPayStatus("途虎朋友圈点赞返现", "WX_QIYEFUKUAN", batchRebateApplyingList[i]?.Select(p => p.OrderId.ToString())?.ToList());

                        lock (payList)
                        {
                            payList.AddRange(payInfo);
                        }
                    });

                    var paySuccessRebate = rebateApplyingNewList.Where(p => payList.Exists(m => m.OrderNo == p.OrderId.ToString() && (m.PayStatus == 1 || m.PayStatus == 2)))?.ToList();
                    needUpdateCount = paySuccessRebate == null ? 0 : paySuccessRebate.Count;
                    Logger.Info($"朋友圈返现 - 支付状态为1或者2的返现数据的审核状态更新为已支付 JOB，需要更新审核状态的返现数据有{needUpdateCount}条");

                    if (paySuccessRebate != null && paySuccessRebate.Any())
                    {
                        updateCount = RebateDal.UpdateRebateStatus(paySuccessRebate.Select(p => p.PKID).Split(50));
                    }
                    Logger.Info($"朋友圈返现 - 支付状态为1或者2的返现数据的审核状态更新为已支付 JOB，实际更新审核状态的返现数据有{updateCount}条");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"RebateUpdateStatusJob -> Run -> error ,异常消息：{ex.Message}，堆栈信息：{ex.StackTrace}");
            }
        }
    }
}
