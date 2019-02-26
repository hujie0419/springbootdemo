using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Server.Model
{
    public class CheckFlashSaleResponseModel : SimpleResponseModel
    {
        public bool HasQuantityLimit { get; set; } = false;

        public bool HasPlaceLimit { get; set; } = false;
        /// <summary>
        /// 多个场次的限购id，有值就是加入了，没有就是没有加入
        /// </summary>
        public string AllPlaceLimitId { get; set; }
        public Guid ActivityId { get; set; }
        public string PID { get; set; }
        public int Num { get; set; }

        /// <summary>
        /// 限时抢购 是否有配置活动商品总数量限制
        /// </summary>
        public bool HasTotalQuantityLimit { get; set; } 
    }
}
