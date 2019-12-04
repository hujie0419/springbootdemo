using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Nest;
using Newtonsoft.Json;
using Quartz;
using Tuhu.C.ActivityJob.Dal.GroupBuying;
using Tuhu.C.ActivityJob.Models.GroupBuying;

namespace Tuhu.C.ActivityJob.Job.GroupBuying
{
    public class GroupBuyingProductSyncJob : IJob
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GroupBuyingProductSyncJob));

        private int MaxSyncCount => 10;

        public void Execute(IJobExecutionContext context)
        {
            int totalCount = GroupBuyingDal.SelectProductGroupCount();

            int pageNum = (totalCount + MaxSyncCount - 1) / MaxSyncCount;
            Logger.Info($"同步拼团产品ES数据开始.一共{totalCount}个团.一共{pageNum}批次.");

            int maxPkid = 0;
            for (int index = 0; index <= pageNum; index++)
            {
                var groups = GroupBuyingDal.SelectProductGroups(MaxSyncCount, ref maxPkid);
                if (groups != null && groups.Any())
                {
                    foreach (var item in groups)
                    {
                        // 比较ES与DB的数据，然后删除ES中多余（DB不存在）的数据
                        var dbProductIds = GroupBuyingDal.SelectGroupBuyingProducts(item.ProductGroupId).Select(x => x.Pid);
                        var esProductIds = GetESGroupBuyingProducts(item.ProductGroupId).Select(x => x.PID);

                        var pids = esProductIds.Except(dbProductIds).ToList();
                        if (pids.Any())
                        {
                            var products = pids.Select(pid => new ESGroupBuyingProduct
                            {
                                PID = pid,
                                ProductGroupId = item.ProductGroupId,
                                ProductIndex = $"{item.ProductGroupId}/{pid}"
                            }).ToList();

                            DeleteESGroupBuyingProducts(products);
                        }
                    }
                }

                Logger.Info($"结束刷新第{index}批次,一共{pageNum}批次.MaxPkid:{maxPkid}");
            }
        }

        private List<ESGroupBuyingProduct> GetESGroupBuyingProducts(string productGroupId)
        {
            var array = new List<Func<QueryContainerDescriptor<ESGroupBuyingProduct>, QueryContainer>>
            {
                q => q.Term(f => f.Field(fd => fd.ProductGroupId).Value(productGroupId))
            };

            var client = ElasticsearchHelper.CreateClient();
            var response = client.Search<ESGroupBuyingProduct>(
                s => s.Index("pintuanproduct")
                    .Type("GroupBuyingProduct")
                    .Query(q => q.Bool(qb => qb.Must(array)))
                    .Size(100));

            var esProducts = new List<ESGroupBuyingProduct>();
            if (response.IsValid && response.Documents?.Any() == true)
            {
                esProducts = response.Documents.ToList();
            }
            return esProducts;
        }

        private void DeleteESGroupBuyingProducts(List<ESGroupBuyingProduct> products)
        {
            var client = ElasticsearchHelper.CreateClient();
            var response = client.Bulk(new BulkRequest("pintuanproduct")
            {
                Operations = products.Select<ESGroupBuyingProduct, IBulkOperation>(item =>
                    new BulkDeleteOperation<ESGroupBuyingProduct>(item)
                    {
                        Routing = item.ProductGroupId
                    }).ToList()
            });

            if (!response.IsValid)
            {
                var productGroupId = products.Select(p => p.ProductGroupId).FirstOrDefault();
                Logger.Warn($"{productGroupId} ES数据删除失败：{JsonConvert.SerializeObject(response.ItemsWithErrors)}");
            }
        }
    }
}
