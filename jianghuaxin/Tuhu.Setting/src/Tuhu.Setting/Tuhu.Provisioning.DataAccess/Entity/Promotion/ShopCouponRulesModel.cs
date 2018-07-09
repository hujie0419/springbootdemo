using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ShopCouponRulesModel
    {
        #region 基本信息
        /// <summary>
        /// 优惠券规则id
        /// </summary>
        public int RuleId { get; set; }
        /// <summary>
        /// 优惠券类型0.满减券
        /// </summary>
        public int PromotionType { get; set; }
        /// <summary>
        /// 优惠券名称
        /// </summary>
        public string PromotionName { get; set; }
        /// <summary>
        /// 使用说明
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 小程序显示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 面额
        /// </summary>
        public decimal Discount { get; set; }
        /// <summary>
        /// 满多少元可以用
        /// </summary>
        public decimal MinMoney { get; set; }
        #endregion
        /// <summary>
        /// 门店范围
        /// </summary>
        public IEnumerable<int> ShopIds { get; set; }
        /// <summary>
        /// NULL代表不修改
        /// 空集合代表修改为全部途虎标准服务
        /// 有值代表更改为指定途虎标准服务
        /// </summary>
        public IEnumerable<ShopCouponRuleProduct> Products { get; set; }
        /// <summary>
        /// 0.全部用户
        /// </summary>
        public int SupportUserRange { get; set; }
        /// <summary>
        /// 0.待发布，1.可领取，2.暂停领取，3.已作废
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public string Creater { get; set; }
        /// <summary>
        /// 最后修改者
        /// </summary>
        public string LastUpdater { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        public string CreateDateTimeString {
            get { return CreateDateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }
        public string LastUpdateDateTimeString
        {
            get { return LastUpdateDateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }
    }

    public class ShopCouponRuleProduct
    {
        public int RuleId { get; set; }
        public int Type { get; set; }
        public string ConfigValue { get; set; }
    }
}
