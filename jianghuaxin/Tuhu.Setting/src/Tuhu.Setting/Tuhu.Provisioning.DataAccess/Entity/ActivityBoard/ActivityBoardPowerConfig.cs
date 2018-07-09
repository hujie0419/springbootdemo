using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.ActivityBoard
{
    /// <summary>
    /// 用户权限
    /// </summary>
    public class ActivityBoardPowerConfig
    {
        public int PKID { get; set; }

        //用户账号
        public string UserEmail { get; set; }

        //是否可见
        public bool IsVisible { get; set; }

        //可见天数
        public int VisibleDays { get; set; }

        //是否允许查看
        public bool IsView { get; set; }

        //是否允许添加
        public bool IsInsert { get; set; }

        //是否允许修改
        public bool IsUpdate { get; set; }

        //是否允许查看
        public bool IsDelete { get; set; }

        //是否允许查看活动效果
        public bool IsViewActivityEffect { get; set; }

        //活动类型
        public ActivityBoardModuleType ModuleType { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime UpdatedTime { get; set; }
    }
}
