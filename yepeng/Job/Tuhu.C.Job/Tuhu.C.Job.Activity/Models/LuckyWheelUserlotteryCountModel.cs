using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.Job.Activity.Models
{
    public class LuckyWheelUserlotteryCountModel
    {
        public int Pkid { get; set; }
        public Guid UserGroup { get; set; }

        public Guid UserId { get; set; }

        /// <summary>
        /// 总的抽奖次数
        /// </summary>
        public int Count { get; set; }
        public DateTime CreateDateTime { get; set; }

        public DateTime UpdateDateTime { get; set; }
        public string HashKey { get; set; }
        /// <summary>
        /// 已经使用的次数
        /// </summary>
        public int Record { get; set; }
    }
}
