using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BeautyService
{
    public class BankMrUsageRecord
    {
        public int PKID { get; set; }
        /// <summary>
        /// 用户手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 其他卡号
        /// </summary>
        public string OtherNum { get; set; }
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string BankNum { get; set; }
        /// <summary>
        /// 服务码
        /// </summary>
        public string ServiceCode { get; set; }
        /// <summary>
        /// 途虎订单
        /// </summary>
        public int TuhuOrderId { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 核销时间
        /// </summary>
        public DateTime? VerifyTime { get; set; }
        /// <summary>
        /// 核销门店
        /// </summary>
        public int? ShopId { get; set; }
        /// <summary>
        /// 活动ID
        /// </summary>
        public Guid ActivityId { get; set; }
        /// <summary>
        /// 有效期
        /// </summary>
        public int ValidityTime { get; set; }
        /// <summary>
        /// 手机号限购
        /// </summary>
        public int MobileDayLimit { get; set; }
        /// <summary>
        /// 卡号限购
        /// </summary>
        public int BankDayLimit { get; set; }
        /// <summary>
        /// 场次开始时间
        /// </summary>
        public DateTime RoundStartTime { get; set; }
        /// <summary>
        /// 场次结束时间
        /// </summary>
        public DateTime RoundEndTime { get; set; }
        /// <summary>
        /// 场次限购
        /// </summary>
        public int RoundDayLimit{get;set;}
        /// <summary>
        /// 每日限购
        /// </summary>
        public int DayLimit { get; set; }

        public int Total { get; set; }
    }
}
