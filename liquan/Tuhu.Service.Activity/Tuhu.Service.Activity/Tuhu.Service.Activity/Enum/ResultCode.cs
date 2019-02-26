using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Enum
{
    public enum ResultCode
    {
        Success = 0,
        //用户不存在存在
        UserNotExist = 11,
        //用户已存在
        UserIsExist = 12,
        // 常规参数错误
        DefaultError = 100000,
        ArgumentShouldNotBeNull = 100001,
    }
}
