using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.CouponActivityConfigV2
{
    public class CouponActivityConfigV2Model : CouponActivityConfig
    {
        /// <summary>
        /// 平台详细配置
        /// </summary>
        public List<CouponActivityChannelConfigModel> ChannelConfigs { get; set; }
        /// <summary>
        /// 平台配置集合  未过滤重复
        /// </summary>
        public List<string> Channels { get; set; }
    }
 
  
    public enum ChannelConfigType
    {
        /// <summary>
        /// 跳转类型
        /// </summary>
        URL,
        /// <summary>
        /// 优惠券类型
        /// </summary>
        Coupon
    
    } 
    /// <summary>
    /// SE_CouponActivityChannelConfig 映射实体
    /// </summary>
    public class CouponActivityChannelConfigModel
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        ///活动ID
        /// </summary>
        public int ConfigId { get; set; }
        /// <summary>
        /// 平台
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// 优惠券码
        /// </summary>
        public Guid GetRuleGUID { get; set; }
        /// <summary>
        /// 跳转链接
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 配置类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }
    }
}
