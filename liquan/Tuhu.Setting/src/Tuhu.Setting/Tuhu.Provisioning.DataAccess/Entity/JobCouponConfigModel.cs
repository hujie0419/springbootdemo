using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary> 
    /// JobCouponConfigModel:实体类 
    /// </summary>  
    [Serializable]
    public class JobCouponConfigModel
    {
        /// <summary>
        /// 行数
        /// </summary>
        public int RowNumver { get; set; }
        /// <summary>
        /// 总行数
        /// </summary>
        public int JobCount { get; set; }

        #region JobCouponConfigModel

        public int ID { get; set; }

        public string ActivityName { get; set; }

        public string RuleID { get; set; }

        public string CouponNum { get; set; }

        public string CouponName { get; set; }
        
        public string CouponExplain { get; set; }
        /// <summary>
        /// 有效期
        /// </summary>
        public string ValidityTime { get; set; }
        /// <summary>
        /// 返券 产品类型
        /// </summary>
        public int ProductType { get; set; }
        /// <summary>
        /// 返券 订单状态
        /// </summary>
        public int OrderState { get; set; }
        /// <summary>
        /// 返券 订单金额
        /// </summary>
        public double OrderMoney { get; set; }
        /// <summary>
        /// 返券类型
        /// </summary>
        public int ReturnType { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// 优惠券配置规则项
        /// </summary>
        public string CouponRules { get; set; }

        public DateTime CreateTime { get; set; }

        #endregion
    }
}
