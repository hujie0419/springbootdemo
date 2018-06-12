
namespace Tuhu.Service.Activity.Models.Requests
{
   public class OrderCountCacheRequest:OrderCountCacheBaseRequest
    {
        /// <summary>
        /// 个人限购数量
        /// </summary>
        public int PerSonalNum { get; set; }

        /// <summary>
        /// 会场限购数量
        /// </summary>
        public int PlaceNum { get; set; }
    }

    public class OrderCountCacheBaseRequest
    {
        public string UserId { get; set; }
        public string DeviceId { get; set; }
        public string UserTel { get; set; }
        public string ActivityId { get; set; }
        public string Pid { get; set; }
    }
}
