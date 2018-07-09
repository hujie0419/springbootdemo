using System;

namespace Tuhu.Service.Activity.Models
{
    public class LuckyWheelUserlotteryCountModel
    {
        public Guid UserGroup { get; set; }

        public Guid UserId { get; set; }

        public int Record { get; set; }

        public int Count { get; set; }
    }
}
