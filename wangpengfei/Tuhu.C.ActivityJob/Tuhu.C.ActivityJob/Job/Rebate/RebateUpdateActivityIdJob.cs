using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.ActivityJob.Dal.Rebate;

namespace Tuhu.C.ActivityJob.Job.Rebate
{
    /// <summary>
    /// 描述：朋友圈返现 - 返现申请数据关联活动ID JOB
    /// 创建人：姜华鑫
    /// 创建时间：2019-06-25
    /// </summary>
    public class RebateUpdateActivityIdJob : IJob
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(RebateUpdateActivityIdJob));

        public void Execute(IJobExecutionContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            Logger.Info("朋友圈返现 - 返现申请数据关联活动ID JOB 开始执行");
            Run();
            stopwatch.Stop();
            Logger.Info($"朋友圈返现 - 返现申请数据关联活动ID JOB 结束执行,用时{stopwatch.Elapsed}");
        }

        private void Run()
        {
            try
            {
                
                Guid rebate16ActivityId = new Guid("060ce1df-ca85-4789-88e2-a27440475f14");
                Guid rebate25ActivityId = new Guid("e6cfad24-4c2d-4d69-ae0a-f54907f447c9");
                Guid aikaActivityId = new Guid("31491a47-75a8-4ddd-98d1-b62397698e43");
                Guid carHouseActivityId = new Guid("4a10b0d6-18de-4df3-a9fb-5371022beb51");

                List<string> sourceList = new List<string> { "Rebate16", "Rebate25", "爱卡", "汽车之家" };
                int updateCount = 0;//实际关联活动id的条数
                var rebateApplyData = RebateDal.GetRebateApplyConfigListBySource(sourceList);//需要关联活动id的返现申请数据

                Logger.Info($"朋友圈返现 - 返现申请数据关联活动ID JOB，需要关联活动ID的返现数据有{rebateApplyData.Count}条");

                //关联活动id
                Parallel.ForEach(rebateApplyData, new ParallelOptions() { MaxDegreeOfParallelism = 4 }, (item) =>
                {
                    if (item.Source.Trim() == "Rebate16")
                    {
                        item.ActivityId = rebate16ActivityId;
                    }
                    if (item.Source.Trim() == "Rebate25")
                    {
                        item.ActivityId = rebate25ActivityId;
                    }
                    if(item.Source.Trim() == "爱卡")
                    {
                        item.ActivityId = aikaActivityId;
                    }
                    if(item.Source.Trim()== "汽车之家")
                    {
                        item.ActivityId = carHouseActivityId;
                    }
                });

                updateCount = RebateDal.BatchUpdateActivityId(rebateApplyData.Split(200));

                Logger.Info($"朋友圈返现 - 返现申请数据关联活动ID JOB，实际关联活动ID的返现数据有{updateCount}条");
            }
            catch (Exception ex)
            {
                Logger.Error($"RebateUpdateActivityIdJob -> Run -> error ,异常消息：{ex.Message}，堆栈信息：{ex.StackTrace}");
            }
        }
    }
}
