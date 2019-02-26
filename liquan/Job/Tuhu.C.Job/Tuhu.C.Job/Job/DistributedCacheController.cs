using Common.Logging;
using Quartz;
using System;
using System.Data.SqlClient;

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
                //DbHelper.ExecuteNonQuery(@"EXEC Tuhu_productcatalog..Product_UpdateProductHierarchyCache");
                using (var dbhelper = DbHelper.CreateDbHelper(false))
                {
                    using (var cmd = new SqlCommand("Tuhu_productcatalog..Product_UpdateProductHierarchyCache"))
                    {
                        cmd.CommandTimeout = 1200;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        var result = dbhelper.ExecuteNonQuery(cmd);
                        Logger.Info($"结束更新产品分类:{result}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }

            Logger.Info("更新产品分类完成");
        }
    }
}
