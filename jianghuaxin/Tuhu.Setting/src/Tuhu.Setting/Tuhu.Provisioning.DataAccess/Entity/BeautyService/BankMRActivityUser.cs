using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.BeautyService
{
    public class BankMRActivityUser
    {
        /// <summary>
        /// 活动场次ID
        /// </summary>
        public int ActivityRoundId { get; set; }
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string BankCardNum { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public long? Mobile { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid? UserId { get; set; }
        /// <summary>
        /// 其他号码 比如ETC
        /// </summary>
        public string OtherNum { get; set; }
        /// <summary>
        /// 每个用户或卡号的限制购买数量
        /// </summary>
        public int LimitCount { get; set; }
        /// <summary>
        /// 每日限购
        /// </summary>
        public int DayLimit { get; set; }
        /// <summary>
        /// 文件批次
        /// </summary>
        public string BatchCode { get; set; }
    }

    public class ImportBankMRActivityRecordModel
    {
        public string BatchCode { get; set; }
        public int ActivityRoundId { get; set; }
        public string BankCardNum { get; set; }
        public string Mobile { get; set; }
        public DateTime CreateTime { get; set; }
        public int? LimitCount { get; set; }
        public int? DayLimit { get; set; }
        public Guid? ActivityId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string OperateUser { get; set; }

        public string VerifyType { get; set; }
        public string OtherNum { get; set; }
        public string Remarks { get; set; }
    }

    public class SignleBankMRActivityUserModel
    { 
        public string BankCardNum { get; set; }
        public string Mobile { get; set; }
        public int LimitCount { get; set; }
        public int DayLimit { get; set; }
        public string OtherNum { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateTimeString => CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
        public DateTime StartTime { get; set;}
        public string StartTimeString => StartTime.ToString("yyyy-MM-dd HH:mm:ss");
        public DateTime EndTime { get; set; }
        public string EndTimeString => EndTime.ToString("yyyy-MM-dd HH:mm:ss");
        public string OperateUser { get; set; }
    }
}
