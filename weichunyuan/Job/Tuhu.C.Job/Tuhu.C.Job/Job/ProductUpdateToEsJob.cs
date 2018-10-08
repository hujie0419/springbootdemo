using Common.Logging;
using Elasticsearch.Net;
using Nest;
using Quartz;
using System;
using System.Configuration;
using System.Data;
using System.Linq;

namespace Tuhu.C.Job.Job
{
    [DisallowConcurrentExecution]
    public class ProductUpdateToEsJob : IJob
    {
        private static readonly string IndexName = "product";

        protected static readonly ILog Logger = LogManager.GetLogger(typeof(ProductUpdateToEsJob));

        private static readonly string EsHost = ConfigurationManager.AppSettings["ESHost"];


        /// <summary>
        /// 查询产品信息
        /// </summary>
        /// <param name="context"></param>
        public virtual void Execute(IJobExecutionContext context)
        {
            Logger.Info("启动任务");

            //TuhuNotification.SendNotification("ProductModify", new { type = "RebuildIndex" });//老的es
            //TuhuNotification.SendNotification("ProductModify", new { type = "RebuildIndexNew" }, 1000 * 60 * 10);//新的es
            TuhuNotification.SendNotification("notification.productmatch.rebuild", new { type = "RebuildIndex" }, 1000 * 60 * 20);//标签es

            Logger.Info("结束任务");
        }

        /// <summary>
        /// 创建ElasticClient对象
        /// </summary>
        /// <returns></returns>
        protected static ElasticClient CreateElasticClient()
        {
            try
            {
                //创建连接池Elasticsearch 
                var connectString = EsHost;
                var nodesStr = connectString.Split('|');
                var nodes = nodesStr.Select(s => new Uri(s)).ToList();
                var connectionPool = new SniffingConnectionPool(nodes);
                var settings = new ConnectionSettings(connectionPool);
                var client = new ElasticClient(settings);
                return client;
            }
            catch (Exception ex)
            {
                Logger.Error("ProductUpdateToEsJob", ex);
                return null;
            }
        }
    }

    [DisallowConcurrentExecution]
    public class EsProductMonitor : ProductUpdateToEsJob
    {
        private static bool _error;
        public override void Execute(IJobExecutionContext context)
        {
            //2017.7.3 移除job
            //var db = GetDbTotal();
            //var es = GetEsTotal();

            //Task.WhenAll(db, es);

            //if (es.Result.IsValid)
            //{
            //    if (es.Result.Total < db.Result)
            //    {
            //        if (_error)
            //        {
            //            Logger.Info("Rebuild product index!");
            //            TuhuNotification.SendNotification("ProductModify", new { type = "RebuildIndex" });

            //            _error = false;
            //        }
            //        else
            //            _error = true;
            //    }
            //    else
            //        _error = false;
            //}
            //else
            //{
            //    Logger.Warn("查询es失败");
            //}
        }

        //private Task<ISearchResponse<Product>> GetEsTotal()
        //{
        //    var client = CreateElasticClient().SearchAsync<Product>(s => s.Index("productcatalog")
        //              .Query(q => q.MatchAll())
        //              .Fields(new string[0])
        //              .Take(0));
        //   // client.

        //    return client;
        //}

        //private async Task<int> GetDbTotal()
        //{
        //    using (var cmd = new SqlCommand("SELECT * FROM Tuhu_productcatalog..vw_ProductService_ProductSearch"))
        //    {
        //        return (await DbHelper.ExecuteDataTableAsync(true, cmd)).Rows.Count;
        //    }
        //}
    }
}
