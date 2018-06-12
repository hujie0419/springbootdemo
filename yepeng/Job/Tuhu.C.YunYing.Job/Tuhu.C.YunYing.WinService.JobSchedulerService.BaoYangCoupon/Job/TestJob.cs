using System;
using System.Data;
using System.Data.SqlClient;
using Common.Logging;
using Quartz;

namespace Tuhu.C.YunYing.WinService.JobSchedulerService.BaoYangCoupon.Job
{
    [DisallowConcurrentExecution]
    public class TestJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (TestJob));

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Guid id = Guid.NewGuid();
                Logger.Info("开始任务" + id);

                var remark = $"TestJob:{id}-Time{DateTime.Now}";

                using (var cmd = new SqlCommand("INSERT INTO SystemLog..tbl_TestLog VALUES(@Remark)"))
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.AddWithValue("@Remark", remark);

                    DbHelper.ExecuteNonQuery(cmd);
                }

                Logger.Info("结束任务");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }
    }

}
