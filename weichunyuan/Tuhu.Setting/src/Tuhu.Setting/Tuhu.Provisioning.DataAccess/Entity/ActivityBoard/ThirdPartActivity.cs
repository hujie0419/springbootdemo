using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.ActivityBoard
{
    public class ThirdPartActivity
    {
        public int PKID { get; set; }

        public ActivityType ActivityType { get; set; }

        //活动名称
        public string ActivityName { get; set; }

        //手机链接
        public string H5Url { get; set; }

        //网站链接
        public string WebUrl { get; set; }

        //活动规则
        public string ActivityRules { get; set; }

        //开始时间
        public DateTime StartTime { get; set; }

        //结束时间
        public DateTime EndTime { get; set; }

        //负责人
        public string Owner { get; set; }

        //第三方渠道
        public string Channel { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime? UpdatedTime { get; set; }

        public int Total { get; set; }
    }
}
