using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tuhu.C.Job.ProductCache.DAL;
using Tuhu.C.Job.ProductCache.Model;

namespace Tuhu.C.Job.ProductCache.Job
{
    [DisallowConcurrentExecution]
    public class ProductNodeCntRefreshJob : IJob
    {
        const int brash = 100;
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(ProductRefreshCacheJob));

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info($"开始刷新数据");
            var nodeNoList = ProductCacheDal.SelectAllProductNode();
            var categoryList = ProductCacheDal.SelectAllCategoryNodeNo().ToList();

            var max = nodeNoList.Sum(p => p.NodeNo.Split('.').Count());
            var allnodeNoList = new List<ProductNodeModel>(max);
            nodeNoList.ForEach(p =>
            {
                var nodeNos = p.NodeNo.Split(new char[] { '.'},StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < nodeNos.Count(); i++)
                {
                    var node = string.Join(".", nodeNos.Take(i + 1));
                    allnodeNoList.Add(new ProductNodeModel() { NodeNo = node, ProductCount = p.ProductCount });
                }
            });
            var result = allnodeNoList.GroupBy(p => p.NodeNo)
                .ToDictionary(p => p.Key, s => s.Sum(ss => ss.ProductCount));
            categoryList.ForEach(p =>
            {
                if (result.ContainsKey(p.NodeNo))
                    p.ProductCount = result[p.NodeNo];
            });
            Logger.Info($"开始更新数据；总量:{categoryList.Count}");
            var branchN = 0;
            foreach (var one in categoryList)
            {
                branchN++;
                var upResult = ProductCacheDal.UpdateProductNodeCNT(one.NodeNo, one.ProductCount);
                if (!upResult)
                    Logger.Warn($"刷新失败;{one.NodeNo}:{one.ProductCount}");
                if (branchN % brash == 0)
                    Thread.Sleep(1000);
            }
            Logger.Info($"刷新完成");
        }
    }
}
