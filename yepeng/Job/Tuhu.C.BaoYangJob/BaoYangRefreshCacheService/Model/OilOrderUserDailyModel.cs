using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaoYangRefreshCacheService.Model
{
    /// <summary>
    /// 机油推荐--用户购买过的机油
    /// </summary>
    public class OilOrderUserDailyModel
    {
        /// <summary>
        /// 缓存前缀
        /// </summary>
        public string CachePrefix { get; set; }
        /// <summary>
        /// 机油Pid
        /// </summary>
        public string Pid { get; set; }
        /// <summary>
        /// 用户UserId
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 二级车型Id
        /// </summary>
        public string VehicleId { get; set; }
        /// <summary>
        /// 车型排量
        /// </summary>
        public string PaiLiang { get; set; }
        /// <summary>
        /// 车型年份
        /// </summary>
        public string Nian { get; set; }
    }
}
