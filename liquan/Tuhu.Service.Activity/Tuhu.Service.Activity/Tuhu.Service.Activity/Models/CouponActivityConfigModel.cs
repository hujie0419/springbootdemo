using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    [Obsolete("请使用CouponActivityConfigNewModel")]
    public class CouponActivityConfigModel
    {
        public int Id { get; set; }
        public string ActivityNum { get; set; }
        public string ActivityName { get; set; }
        public int ActivityStatus { get; set; }
        public int CheckStatus { get; set; }
        public string LayerImage { get; set; }
        public int CouponId { get; set; }
        public string ButtonChar { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public string ActivityImage { get; set; }
        public string GetRuleGUID { get; set; }
        public int Type { get; set; }
    }

    /// <summary>
    /// 蓄电池/加油卡活动配置
    /// </summary>
    public class CouponActivityConfigNewModel
    {
        /// <summary>
        /// 活动Guid
        /// </summary>
        public string ActivityNum { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }

        /// <summary>
        /// 浮层状态
        /// </summary>
        public int CheckStatus { get; set; }

        /// <summary>
        /// 浮层图片
        /// </summary>
        public string LayerImage { get; set; }

        /// <summary>
        /// 活动图标
        /// </summary>
        public string ActivityImage { get; set; }

        /// <summary>
        /// 优惠券Guid
        /// </summary>
        public List<Guid> GetRuleGUIDList { get; set; }

        /// <summary>
        /// 跳转链接
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 类型
        /// 1--蓄电池  2--加油卡
        /// </summary>
        public int Type { get; set; }
    }
}
