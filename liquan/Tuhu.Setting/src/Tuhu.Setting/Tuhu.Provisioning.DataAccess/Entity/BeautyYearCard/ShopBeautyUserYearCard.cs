using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BeautyYearCard
{
    public class ShopBeautyUserYearCard
    {
        public int PKID { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }

        public Guid UserId { get; set; }
        /// <summary>
        /// 用户年卡的订单Id
        /// </summary>
        public long UserCardId { get; set; }
        /// <summary>
        /// 年卡Id
        /// </summary>
        public int CardId { get; set; }
        /// <summary>
        /// 年卡价格
        /// </summary>
        public decimal CardPrice { get; set; }

        public int ShopId { get; set; }
        /// <summary>
        /// 适配车型
        /// </summary>
        public int AdaptVehicle { get; set; }
        /// <summary>
        /// 年卡开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }
        /// <summary>
        /// 年卡结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 0=正常，1=过期，2=作废
        /// </summary>
        public int Status { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 0 线上  1线下
        /// </summary>
        public int CardType { get; set; }
    }
}
