using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.ActivityBoard
{
    public enum OperationType
    {
        IsVisible,//是否可见
        Insert,//增
        Delete,//删
        Update,//改
        View,//查
        Effect//活动效果
    }
}
