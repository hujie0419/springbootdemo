using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 汽配龙修改价格记录日志表
    /// </summary>
    public class QPLPriceApplyModel
    {
        public int ApplyId { get; set; }
        public string Brand { get; set; }
        public string PID { get; set; }
        public string ProductName { get; set; }
        public decimal? StandPrice { get; set; }
        public decimal? TuHuPrice { get; set; }
        public decimal? NowPrice { get; set; }
        public decimal? OldPrice { get; set; }
        public string ApplyReason { get; set; }
        public DateTime? ApplyDateTime { get; set; }
        public string ApplyUser { get; set; }
        public string Auditor { get; set; }
        public string ApplyStatus { get; set; }

        public string ApplyStatusName { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public DateTime? UpdateDateTime { get; set; }
        public string Remark { get; set; }
    }
}
