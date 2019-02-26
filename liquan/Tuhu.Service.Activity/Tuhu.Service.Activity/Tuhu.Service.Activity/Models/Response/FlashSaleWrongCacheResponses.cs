using System.Collections.Generic;

namespace Tuhu.Service.Activity.Models.Response
{
    public class FlashSaleWrongCacheResponse
    {
        public string ActiivtyId { get; set; }
        public List<FlashSaleCacheProduct> CacheProducts { get; set; }
    }

    public class FlashSaleCacheProduct
    {
        public string Pid { get; set; }

        /// <summary>
        /// 缓存数据落地在db
        /// </summary>
        public int SaleOutQuantity { get; set; }

        /// <summary>
        /// 缓存中数量
        /// </summary>
        public int CacheQuantity { get; set; }

        public int TotalQuantity { get; set; }

        public string StrType { get; set; }

        /// <summary>
        /// 日志表销量统计
        /// </summary>
        public int LogRecordQuantity { get; set; }
    }
}