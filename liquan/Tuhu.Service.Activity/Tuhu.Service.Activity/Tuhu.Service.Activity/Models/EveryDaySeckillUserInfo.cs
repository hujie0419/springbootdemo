using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    public class EveryDaySeckillUserInfo
    {
        public Guid? UserId { get; set; }

        public string MobilePhone { get; set; }
        public Guid FlashActivityGuid { get; set; }

        public string Pid { get; set; }

        public DateTime FlashActivityStartTime { get; set; }

        public string Type { get; set; } = "TTSeckill";
    }

    public class UserReminderInfo
    {
        public bool CanbeReminded { get; set; }

        public bool IsReminded { get; set; }
    }

    public class InsertEveryDaySeckillUserInfoResponse
    {
        public bool IsInsertSuccess { get; set; }

        public string Message { get; set; }

        public int Code { get; set; }
    }
}
