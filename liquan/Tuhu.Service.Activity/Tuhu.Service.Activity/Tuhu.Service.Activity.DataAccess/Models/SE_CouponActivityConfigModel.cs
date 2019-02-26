using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.DataAccess.Models
{
    /// <summary>
    /// 蓄电池/加油卡 活动配置
    /// </summary>
    public class SE_CouponActivityConfigModel
    {
        /// <summary>
        /// 表自增主键
        /// </summary>
        public int Id { get; set; }

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
        /// 类型
        /// 1--蓄电池  2--加油卡
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 适用平台
        /// </summary>
        public string Channel { get; set; }
    }

    /// <summary>
    /// 蓄电池/加油卡  活动优惠券/跳转链接配置
    /// </summary>
    public class SE_CouponActivityChannelConfigModel
    {
        /// <summary>
        /// SE_CouponActivityConfig表Id
        /// </summary>
        public int ConfigId { get; set; }

        /// <summary>
        /// 平台
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// 优惠券Guid
        /// </summary>
        public Guid GetRuleGUID { get; set; }

        /// <summary>
        /// 跳转链接
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 类型--跳转/优惠券
        /// </summary>
        public string Type { get; set; }
    }
}
