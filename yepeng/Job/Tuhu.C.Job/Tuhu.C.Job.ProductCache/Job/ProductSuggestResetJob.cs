using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuhu.Service.Product;
using Tuhu.C.Job.ProductCache.DAL;

namespace Tuhu.C.Job.ProductCache.Job
{
    [DisallowConcurrentExecution]
    public class ProductSuggestResetJob : IJob
    {
        int brash = 100;
        protected static readonly ILog Logger = LogManager.GetLogger(typeof(ProductSuggestResetJob));

        public void Execute(IJobExecutionContext context)
        {
            Logger.Info($"开始刷新Suggest");
            var client = new ProductSearchClient();
            var suggestList = ProductCacheDal.SelectSuggestWord(brash);
            foreach (var oneList in suggestList)
            {
                var response = client.SearchKeywordsResultNumber(oneList.Select(_ => _.Keyword).Distinct().ToList());
                if (!response.Success)
                {
                    client = new ProductSearchClient();
                    response = client.SearchKeywordsResultNumber(oneList.Select(_ => _.Keyword).ToList()); if (response.Success)
                    {
                        if (!response.Success)
                        {
                            Logger.Error($"服务调用失败");
                            return;
                        }
                    }
                }

                if (response.Result == null)
                {
                    Logger.Warn($"服务返回null;{response.ErrorMessage}");
                    continue;
                }

                var upUnActive = ProductCacheDal.UpdateSuggestActive(
                    response.Result.Where(_ => _.Value == 0).Select(_ => _.Key).ToList()
                    , false);

                var upActive =  ProductCacheDal.UpdateSuggestActive(
                    response.Result.Where(_ => _.Value > 0).Select(_ => _.Key).ToList()
                    , true);

                if (!upUnActive)
                {
                    Logger.Warn($"数据库更新失败upUnActive;");
                    continue;
                }
                if (!upActive)
                {
                    Logger.Warn($"数据库更新失败upActive;");
                    continue;
                }

            }
            Logger.Info($"刷新Suggest完成;通知刷新 RebuildSuggest");
            TuhuNotification.SendNotification("ProductModify", new { type = "RebuildSuggest" });
        }
    }
}
