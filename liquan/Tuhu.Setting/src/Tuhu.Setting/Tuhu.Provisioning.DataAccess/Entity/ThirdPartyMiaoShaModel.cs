using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
   public class ThirdPartyMiaoShaModel
    {
        /// <summary>
        /// PKID
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 途虎PID
        /// </summary>
        public string Pid { get; set; }

        /// <summary>
        /// 途虎价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 途虎产品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 1 抢购中  0 即将开抢
        /// </summary>
        public int Type { get; set; }

        public long SkuID { get; set; }

        /// <summary>
        /// 产品名
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 秒杀开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 秒杀结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 三方价格
        /// </summary>
        public decimal ThirdPartyPrice { get; set; }
        /// <summary>
        /// 秒杀价格
        /// </summary>
        public decimal MiaoShaPrice { get; set; }
        /// <summary>
        /// 门店编号
        /// </summary>
        public string ShopCode { get; set; }
        /// <summary>
        /// 第三方ItemId
        /// </summary>
        public string ItemId { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public string ItemCode { get; set; }
    }
}
