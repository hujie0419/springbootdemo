using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BeautyYearCard
{
    public class ShopBeautyUserYearCardDetail
    {
        public int PKID { get; set; }
        /// <summary>
        /// 用户年卡ShopBeautyUserYearCard的PKID,
        /// </summary>
        public int UserYearCardId { get; set; }
        /// <summary>
        /// 年卡Id,即订单Id
        /// </summary>
        public int UserCardId { get; set; }
        /// <summary>
        /// 产品Id
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal ProductPrice { get; set; }
        /// <summary>
        /// 产品num
        /// </summary>
        public int ProductNumber { get; set; }
        /// <summary>
        /// 佣金比
        /// </summary>
        public float Commission { get; set; }
        /// <summary>
        /// 使用次数
        /// </summary>
        public int UseCycle { get; set; }
        /// <summary>
        /// 使用周期（0=年，1=月，2=周，3=天）
        /// </summary>
        public int CycleType { get; set; }
        /// <summary>
        /// 服务码
        /// </summary>
        public string FwCode { get; set; }
        /// <summary>
        /// 门店Id
        /// </summary>
        public int UsedShopId { get; set; }
        /// <summary>
        /// 使用时间
        /// </summary>
        public DateTime UsedTime { get; set; }

        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 服务描述
        /// </summary>
        public string ProductDescription { get; set; }
    }
}
