using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Tuhu.C.Job.UserShareJob.Dal;

namespace Tuhu.C.Job.UserShareJob.Job
{
    [DisallowConcurrentExecution]
    public class UserExpiringIntegralJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger<UserExpiringIntegralJob>();
        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("用户积分数据开始");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                DoJob();
            }
            catch (Exception ex)
            {
                Logger.Error($"用户积分数据异常：{ex}");
            }
            sw.Stop();
            Logger.Info($"用户积分数据完成，用时{sw.ElapsedMilliseconds}毫秒");
        }

        public static void DoJob()
        {
            var val = DalExpiringIntegral.CheckIntegralData();
            if (val)
            {
                Logger.Warn($"去年的统计数据已存在,请检查数据库数据后重试");
                return;
            }
            int index = 0;
            int step = 1000;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var data = DalExpiringIntegral.GetIntegralID(index, step);
            while (data.Any())
            {
                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[] {
                    new DataColumn("Id",typeof(Guid))
                });
                foreach (var item in data)
                {
                    DataRow r = dt.NewRow();
                    r[0] = item.IntegralID;
                    dt.Rows.Add(r);
                }
                try
                {
                    index += 1;
                    var dat = DalExpiringIntegral.InsertIntegralStatistics(dt);
                    sw.Stop();
                    Logger.Info($"插入{dat}条数据,用时{sw.ElapsedMilliseconds}毫秒");
                    sw.Reset();
                    sw.Start();
                    data = DalExpiringIntegral.GetIntegralID(index, step);
                }
                catch (Exception ex)
                {
                    Logger.Info($"统计第{index}批数据出现异常{ex.Message}");
                }
            }
        }
    }
}
