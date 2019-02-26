using System;
using System.Data;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 会员生活权益 
    /// </summary>
    public class MemberShipLivingRightsModel : BaseModel
    {
        /// <summary>
        /// 主键 自增
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 渠道Id
        /// </summary>
        public int ChannelId { get; set; }

        /// <summary>
        /// 渠道名称
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        /// 福利内容
        /// </summary>
        public string WelfareContent { get; set; }

        /// <summary>
        /// 权益值(价值金额)
        /// </summary>
        public decimal RightsValue { get; set; }

        /// <summary>
        /// 领取方式Id(用于后期扩展)
        /// </summary>
        public int ReceivingType { get; set; }

        /// <summary>
        /// 领取方式说明
        /// </summary>
        public string ReceivingDescription { get; set; }

        /// <summary>
        /// 券Id
        /// </summary>
        public int CouponId { get; set; }

        /// <summary>
        /// 券说明
        /// </summary>
        public string CouponDescription { get; set; }

        /// <summary>
        /// 跳转链接
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        /// 排序序列
        /// </summary>
        public int SortIndex { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public string StrLastUpdateDateTime { get { return LastUpdateDateTime.ToString("yyyy-MM-dd HH:mm:ss"); } }
        /// <summary>
        /// 最后修改人
        /// </summary>
        public string LastUpdateBy { get; set; }
    }
}
