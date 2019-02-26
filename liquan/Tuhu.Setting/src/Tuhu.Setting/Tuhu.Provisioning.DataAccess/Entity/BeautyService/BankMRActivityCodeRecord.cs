using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BeautyService
{
    public class BankMRActivityCodeRecord
    {
        public int PKID { get; set; }
        /// <summary>
        /// 活动场次ID
        /// </summary>
        public int ActivityRoundId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 服务码
        /// </summary>
        public string ServiceCode { get; set; }
        /// <summary>
        ///  订单号
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string BankCardNum { get; set; }
        /// <summary>
        /// 其他号码，如ETC号
        /// </summary>
        public string OtherNum { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 规则ID，对应BankMRActivityUsers的PKID
        /// </summary>
        public int RuleId { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public int Total { get; set; }
    }
}
