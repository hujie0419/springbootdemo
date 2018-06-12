using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using BaoYangRefreshCacheService.Model;
using Elasticsearch.Net;
using System.Configuration;
using System.Threading;

namespace BaoYangRefreshCacheService.BLL
{
    public class VehicleParts_LiYangEsBll
    {
        private static string _Index = "VehiclePartsLiYang_20171116".ToLower();
        private IElasticClient _Client = null;
        private ILog logger = null;
        public VehicleParts_LiYangEsBll(ILog log)
        {
            _Client = ElasticsearchHelper.CreateClient();// new ElasticClient(new Uri("http://es.qipeilong.net:9200"));
#if DEBUG
            // ElasticsearchHelper.EnableDebug();
#endif
            logger = log;
        }
        private bool CreateIndex()
        {
            return _Client.CreateIndexIfNotExists(
                _Index
                , c => c.Settings(cs => cs.NumberOfShards(5).NumberOfReplicas(1)).
                Mappings(cm => cm.MapDefault().Map<VehiclePartsLiYangEsModel>(mp => mp.AutoMap()))
                .Aliases(a => a.Alias("vehiclepartsliyang")));

        }
        public bool BatchInsert_VehicleParts_LiYangEs(IEnumerable<VehiclePartsLiYangEsModel> list)
        {

            if (!CreateIndex())
            {
                throw new Exception($"不存在索引{_Index}");
            }
            var request = new BulkRequest(_Index)
            {
                Operations = list.Select<VehiclePartsLiYangEsModel, IBulkOperation>(l => new BulkIndexOperation<VehiclePartsLiYangEsModel>(l)).ToArray()
            };
            var esResult = _Client.Bulk(request);
            if (!esResult.IsValid)
            {
                logger.Error($"BatchInsert_VehicleParts_LiYangEs:异常=>{esResult.OriginalException}");
                Thread.Sleep(1000);
            }
            logger.Info($"BatchInsert_VehicleParts_LiYangEs:导入成功{esResult.Items?.Count()}，导入失败{esResult.ItemsWithErrors?.Count()}");
            return esResult.IsValid;
        }
    }
}
