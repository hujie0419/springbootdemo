using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tuhu.Service.Promotion.Server.Model
{
    public enum CouponOperateEnum
    {
        释放 = 0,
        核销 = 1,
        作废 = 3,
        延期 = 11,
        修改金额 = 12,
    }
}
