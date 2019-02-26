using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.ActivityBoard
{
    public class ActivityBoardViewModel
    {
        //活动ID
        public int ActivityId { get; set; }

        //活动主题
        public string Title { get; set; }

        //活动看板中活动的类型  活动看板包括第三方活动与途虎内部的活动
        public string ActivityBoardType { get; set; }

        //活动类型 0:综合 1：车品 2：保养  3：轮胎  4：美容改装
        public string ActivityType { get; set; }

        public int DiffDays { get; set; }

        //开始时间
        public DateTime StartTime { get; set; }

        //结束时间
        public DateTime EndTime { get; set; }

        public bool IsNew { get; set; }

        public string HashKey { get; set; }
    }
}
