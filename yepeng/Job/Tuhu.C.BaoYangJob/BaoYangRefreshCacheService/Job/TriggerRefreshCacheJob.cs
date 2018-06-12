using System;
using BaoYangRefreshCacheService.Common;
using BaoYangRefreshCacheService.DAL;
using Common.Logging;
using Quartz;

namespace BaoYangRefreshCacheService.Job
{
    /// <summary>
    /// 通过在数据库中添加任务，来触发刷新缓存
    /// </summary>
    [DisallowConcurrentExecution]
    public class TriggerRefreshCacheJob : BaseJob
    {
        public TriggerRefreshCacheJob()
        {
            logger = LogManager.GetLogger(typeof (TriggerRefreshCacheJob));
        }

        protected override string JobName
        {
            get { return typeof (TriggerRefreshCacheJob).ToString(); }
        }

        public override void Exec()
        {
            var config = BaoYangDal.SelectRunJobConfig();

            if (config != null && !string.IsNullOrWhiteSpace(config.JobName))
            {
                BaoYangDal.UpdateRunJobConfig(config.Id, "Runing");

                var t = Type.GetType(config.JobName);
                if (t != null)
                {
                    object o = Activator.CreateInstance(t);
                    var method = o.GetType().GetMethod("Execute");
                    object[] obj = new object[1];
                    obj[0] = null;
                    method.Invoke(o, obj);
                }

                BaoYangDal.UpdateRunJobConfig(config.Id, "Success");
            }
        }
    }
}
