using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.Service.Product;
using Tuhu.C.Job.ProductCache.DAL;

namespace Tuhu.C.Job.ProductCache.Job
{
    [DisallowConcurrentExecution]
    public class ProductRebuildToEsJob : IJob
    {
        private int mgIndex=0;
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(ProductRebuildToEsJob));

        public void Execute(IJobExecutionContext context)
        {
            mgIndex++;
            Logger.Info($"启动任务=>{mgIndex}");

            var dt = DateTime.Now;
            var isDeleteOld = true;

            var cnt = ProductCacheDal.GetProductEsCount();

            Logger?.Info($"RGZ{mgIndex}=更新产品总数为：" + cnt);
            if (cnt == 0)
                return;

            var batchCnt = 500;

            var pageCnt = Math.Ceiling((double)cnt / batchCnt);

            for (var i = 1; i <= pageCnt; i++)
            {
                var pids = ProductCacheDal.GetProductEsPids(batchCnt, i);
                using(var client= new CacheClient())
                {
                    var result = client.RebuildProductEs(pids);
                    if (!result.Success)
                    {
                        using (var client2 = new CacheClient())
                        {
                            Thread.Sleep(200);
                            result = client2.RebuildProductEs(pids);
                            if (!result.Success)
                            {
                                Logger?.Warn($"RGZ{mgIndex}={i}=失败,不删除老数据：{result.ErrorCode};{result.ErrorMessage}");
                                isDeleteOld = false;
                                continue;
                            }
                        }
                    }
                }
               
                Logger?.Info($"RGZ{mgIndex}={i}=成功");
            }


            if (!isDeleteOld)
            {
                Logger?.Info($"RGZ{mgIndex}=有更新错误的数据，不删除老数据");
                return;
            }
			//防止更新延迟误删
            Thread.Sleep(TimeSpan.FromMinutes(1));
            using (var client3 = new ProductSearchClient())
            {
                var deleteResult = client3.DeleteOldProductEs(dt);
                if (!deleteResult.Success)
                    Logger?.Info($"RGZ{mgIndex}=删除老数据出错");
            }

            Logger?.Info($"RGZ{mgIndex}全部成功");
            return;
        }
    }
}
