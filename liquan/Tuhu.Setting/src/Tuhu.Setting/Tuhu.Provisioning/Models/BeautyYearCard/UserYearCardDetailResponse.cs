using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models.BeautyYearCard
{
    public class UserYearCardDetailResponse
    {
        /// <summary>
        /// 年卡订单信息
        /// </summary>
        public YearCardOrderInfo OrderInfo { get; set; }
        /// <summary>
        /// 年卡产品信息
        /// </summary>
        public YearCardProduct Product { get; set; }
        /// <summary>
        /// 年卡服务码信息
        /// </summary>
        public IEnumerable<YearCard> YearCards { get; set; }
    }

    public class YearCardOrderInfo
    {
        public long OrderId { get; set; }

        public string OrderTime { get; set; }
    }

    public class YearCardProduct {
        public string Pid { get; set; }

        public int Num { get; set; }
        /// <summary>
        /// 使用周期
        /// </summary>
        public string UseCycle { get; set; }
    }

    public class YearCard
    {
        /// <summary>
        /// 是否不能选中
        /// </summary>
        public bool _disabled { get; set; }
        /// <summary>
        /// 服务码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 服务码状态
        /// </summary>
        public string Status { get; set; }
    }
}