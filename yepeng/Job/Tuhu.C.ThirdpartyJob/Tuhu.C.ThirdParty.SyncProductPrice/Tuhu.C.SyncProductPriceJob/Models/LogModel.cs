using Newtonsoft.Json;

namespace Tuhu.C.SyncProductPriceJob.Models
{
    public class LogModel
    {
        public string ShopCode { get; set; }
        public string Api { get; set; }
        public string RefNo { get; set; }
        public int? OrderId { get; set; }
        public string Message { get; set; }
        public string ResponseContent { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }
    }
}
