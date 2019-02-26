using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tuhu.Service.Member.Models;

namespace Tuhu.Provisioning.Models.Promotion
{
    /// <summary>
    /// 保养发券
    /// </summary>
    public class SendCouponForBaoYangModel: CreatePromotionModel
    {
        /// <summary>
        /// 用户手机号
        /// </summary>
        public string Phone { get; set; }
    }
}