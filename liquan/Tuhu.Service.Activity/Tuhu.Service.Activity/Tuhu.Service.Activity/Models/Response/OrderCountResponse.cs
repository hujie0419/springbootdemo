using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Response
{
    public class OrderCountResponse
    {
        #region 计数器里的三个维度的数量
        public int PlaceDeviceCountCacheNum { get; set; }

        public int PlaceUserIdCountCacheNum { get; set; }

        public int PlaceUserTelCountCacheNum { get; set; }

        public int PersonDeviceCountCacheNum { get; set; }

        public int PersonUserIdCountCacheNum { get; set; }

        public int PersonUserTelCountCacheNum { get; set; }
        #endregion

        #region 数据库 对应的用户记录
        public int UserDbPlaceLimitQty { get; set; }

        public int UserDbPersonLimitQty { get; set; }
        #endregion
        public bool RedisField { get; set; }
        public string  RedisMsg { get; set; }
        #region 配置信息
        public int? PlaceConfigLimitQty { get; set; }

        public int? PersonConfigLimitQty { get; set; }
        public int? TotalConfigQty { get; set; }
        #endregion
    }
}
