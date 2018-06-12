using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.CommonEnum
{
    /// <summary>
    /// FiveSeat 五座轿车
    /// SuvOrMpv SUV或者MPV的车
    /// None 不限制车型
    /// </summary>
    public enum MrVehicleType
    {
        FiveSeat = 1,
        SuvOrMpv = 2,
        Suv = 3,
        Mpv = 4,
        None = 5
    }
}
