using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.C.ActivityJob.Dal.MoveCar;

namespace Tuhu.C.ActivityJob.Job.MoveCar
{
    /// <summary>
    /// 描述：途虎挪车 - 挪车贴发放渠道追踪历史数据修复JOB
    /// 创建人：姜华鑫
    /// 创建时间：2019-05-09
    /// </summary>
    public class MoveCarRepairHistoricalDataJob: IJob
    {
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(MoveCarRepairHistoricalDataJob));

        public void Execute(IJobExecutionContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            Logger.Info("途虎挪车 - 挪车贴发放渠道追踪历史数据修复JOB 开始执行");
            Run();
            stopwatch.Stop();
            Logger.Info($"途虎挪车 - 挪车贴发放渠道追踪历史数据修复JOB 结束执行,用时{stopwatch.Elapsed}");
        }

        private void Run()
        {
            try
            {
                int qRCodeCount = MoveCarDal.GetMoveCarQRCodeCount();
                int updateNum = 500;
                var updateTimes = (int)Math.Ceiling(qRCodeCount / (updateNum * 1.0));
                int realUpdateTotalCount = 0;//更新业务主键的总条数

                Logger.Info($"途虎挪车 - 挪车贴发放渠道追踪历史数据修复JOB，需要更新业务主键的数据有{qRCodeCount}条");

                if (updateTimes > 0)
                {
                    Parallel.For(1, updateTimes + 1, new ParallelOptions() { MaxDegreeOfParallelism = 4 }, (i) =>
                    {
                        int updateCount = MoveCarDal.UpdateUniqueQRCode(updateNum);
                        realUpdateTotalCount += updateCount;
                    });
                }
                
                Logger.Info($"途虎挪车 - 挪车贴发放渠道追踪历史数据修复JOB，实际更新业务主键的数据有{realUpdateTotalCount}条数据");
            }
            catch (Exception ex)
            {
                Logger.Error($"MoveCarRepairHistoricalDataJob -> Run -> error ,异常消息：{ex.Message}，堆栈信息：{ex.StackTrace}");
            }
        }
    }
}
