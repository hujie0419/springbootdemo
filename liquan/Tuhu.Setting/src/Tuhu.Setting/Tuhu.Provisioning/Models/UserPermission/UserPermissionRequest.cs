using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models
{
    public class UserPermissionRequest
    {
        /// <summary>
        /// 请求首页
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// 会员权益名称
        /// </summary>
        public string PermissionName { get; set; }

        /// <summary>
        /// 渠道名称
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        /// 优惠券名称
        /// </summary>
        public string CouponName { get; set; }

        /// <summary>
        /// 优惠券描述
        /// </summary>
        public string CouponDescription { get; set; }

        /// <summary>
        /// 会员等级外键(以此字段为主)
        /// </summary>
        public int MembershipsGradeId { get; set; }

        /// <summary>
        /// 奖励（礼品）名称
        /// </summary>
        public string Name { get; set; }
    }
}