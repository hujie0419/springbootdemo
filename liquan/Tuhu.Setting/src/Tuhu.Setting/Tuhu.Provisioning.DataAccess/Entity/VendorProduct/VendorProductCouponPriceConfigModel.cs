using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class VendorProductCouponPriceConfigModel
    {
        /// <summary>
        /// 自增主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 产品类型
        /// </summary>
        public string ProductType { get; set; }

        /// <summary>
        /// 蓄电池Pid
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 是否展示
        /// </summary>
        public bool IsShow { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDateTime { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? LastUpdateDateTime { get; set; }
    }

    public class VendorProductCouponPriceConfigViewModel : VendorProductCouponPriceConfigModel
    {
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 展示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 可用优惠券Guid
        /// </summary>
        public List<Guid> Coupons { get; set; }

        /// <summary>
        /// 蓄电池产品Oid
        /// </summary>
        public int Oid { get; set; }

        /// <summary>
        /// 券后最低价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 蓄电池产品库价格
        /// </summary>
        public decimal OriginalPrice { get; set; }
    }
}
