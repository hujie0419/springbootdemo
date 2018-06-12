using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.WebSite.Web.Activity
{
    public enum CacheTimeEnum : byte
    {
        /// <summary>
        /// 1秒
        /// </summary>
        Second,
        /// <summary>
        /// 5秒
        /// </summary>
        Rapid,
        /// <summary>
        /// 10分钟
        /// </summary>
        Short,
        /// <summary>
        /// 15分钟
        /// </summary>
        Quarter,
        /// <summary>
        /// 30分钟
        /// </summary>
        HalfHour,
        /// <summary>
        /// 1小时
        /// </summary>
        Hour,
        /// <summary>
        /// 1天
        /// </summary>
        Day,
        /// <summary>
        /// 3天
        /// </summary>
        HalfWeek,
        /// <summary>
        /// 7天
        /// </summary>
        Week,
        /// <summary>
        /// 30天
        /// </summary>
        Month,
        /// <summary>
        /// 自定(不建议使用)
        /// 如有需求可以考虑增加枚举值
        /// </summary>
        Customize
    }
}
