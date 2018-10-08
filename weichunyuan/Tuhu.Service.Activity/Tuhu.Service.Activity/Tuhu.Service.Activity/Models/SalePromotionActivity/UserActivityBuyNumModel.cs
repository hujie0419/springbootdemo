
using System.Collections.Generic;

namespace Tuhu.Service.Activity.Models
{
    /// <summary>
    /// 用户的活动已购数量
    /// </summary>
    public class UserActivityBuyNumModel
    {
        public string ActivityId { get; set; }
        public string UserId { get; set; }
        public int BuyNum { get; set; }
    }

    /// <summary>
    /// 活动商品已售数量
    /// </summary>
    public class ActivityPidSoldNumModel
    {
        public string ActivityId { get; set; }
        public string Pid { get; set; }
        public int SoldNum { get; set; } 
    }


}
