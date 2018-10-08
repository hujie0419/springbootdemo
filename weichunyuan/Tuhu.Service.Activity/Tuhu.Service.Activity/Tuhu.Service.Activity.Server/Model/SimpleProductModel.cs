
namespace Tuhu.Service.Activity.Server.Model
{
    public class SimpleProductModel
    {
        public string Pid { get; set; }
        public bool Onsale { get; set; }

        public string Brand { get; set; }

        public string Pattern { get; set; }

        public string SpeedRating { get; set; }

        public string RootCategoryName { get; set; }

        public decimal Price { get; set; }
        /// <summary>
        /// 市场价
        /// </summary>
        public decimal? MarketingPrice { get; set; }
    }
}
