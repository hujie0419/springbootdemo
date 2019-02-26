using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models
{
    /// <summary>
    /// 积分兑换配置请求参数
    /// </summary>
    public class ExchangeCenterConfigRequest
    {
        /// <summary>
        /// 起始页 默认1
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 分页大小 默认20
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// 配置位编号
        /// </summary>
        public string PostionCode { get; set; }

        /// <summary>
        /// 用户等级 参数：V0、V1、V2、V3、V4 与会员等级表GradeCode关联
        /// </summary>
        public string UserRank { get; set; }

        /// <summary>
        /// 优惠券名称
        /// </summary>
        public string CouponName { get; set; }

        /// <summary>
        /// 状态 true 启用和 false禁用
        /// </summary>
        public bool? Status { get; set; }
    }
}