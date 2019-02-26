using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models.BeautyYearCard
{
    public class GetUserYearCardResponse
    {
        public int PKID { get; set; }
        public int OrderId { get; set; }
        /// <summary>
        /// 0 线上  1线下
        /// </summary>
        public int CardType { get; set; }
        /// <summary>
        /// 年卡价格
        /// </summary>
        public decimal CardPrice { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        public string CreateTime { get; set; }
        /// <summary>
        /// 有效期
        /// </summary>
        public string EffictiveTime { get; set; }
        /// <summary>
        /// 剩余次数（格式：剩余/总数）
        /// </summary>
        public string RemainTime { get; set; }
    }
}