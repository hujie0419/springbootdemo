using Tuhu.Models;

namespace Tuhu.C.SyncProductPriceJob.Models
{
    public class ProductPriceMappingModel : BaseModel
    {
        public string ShopCode { get; set; }
        public string Pid { get; set; }
        public long ItemId { get; set; }
        public long SkuId { get; set; }
        public string ItemCode { get; set; }
        public string Properties { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }

        /// <summary>
        /// 京东plus价格
        /// </summary>
        public decimal Price2 { get; set; }
    }
}
