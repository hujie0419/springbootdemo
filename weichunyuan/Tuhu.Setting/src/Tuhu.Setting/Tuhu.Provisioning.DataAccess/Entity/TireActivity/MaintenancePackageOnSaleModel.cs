using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.TireActivity
{
    public class MaintenancePackageOnSaleModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 小保养套餐PID
        /// </summary>
        public string PID { get; set; }

        /// <summary>
        /// 小保养套餐原价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 一条轮胎时小保养套餐的优惠价
        /// </summary>
        public decimal OnetirePrice { get; set; }

        /// <summary>
        /// 二条轮胎优惠价
        /// </summary>
        public decimal TwotirePrice { get; set; }

        /// <summary>
        /// 三条轮胎优惠价
        /// </summary>
        public decimal ThreetirePrice { get; set; }

        /// <summary>
        /// 四条轮胎优惠价
        /// </summary>
        public decimal FourtirePrice { get; set; }

        /// <summary>
        /// 更新ID
        /// </summary>
        public int UpdateID { get; set; }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool Is_Deleted { get; set; }

        /// <summary>
        /// 状态。0=失效，1=有效
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string LastUpdateBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }
    }
}
