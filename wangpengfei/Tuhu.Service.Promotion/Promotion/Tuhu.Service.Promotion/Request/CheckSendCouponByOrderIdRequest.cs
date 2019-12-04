using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Request
{
    public class CheckSendCouponByOrderIdRequest
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public int orderID { get; set; }


        #region 计算预算相关

        /// <summary>
        /// true：开启 预算逻辑
        /// </summary>
        public bool Budget { get; set; }

        /// <summary>
        /// 优惠券使用规则id
        /// </summary>
        public List<int> CouponRuleIDList { get; set; }

        /// <summary>
        /// 业务线名称
        /// </summary>
        public string BusinessLineName { get; set; }

        #endregion
    }
}
