using Common.Logging;
using Quartz;
using System;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class DistributedCacheController : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(DistributedCacheController));

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info("开始更新产品分类");

            try
            {
                DbHelper.ExecuteNonQuery(@"EXEC Tuhu_productcatalog..Product_UpdateProductHierarchyCache");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }

            Logger.Info("更新产品分类完成");
        }
    }
}
