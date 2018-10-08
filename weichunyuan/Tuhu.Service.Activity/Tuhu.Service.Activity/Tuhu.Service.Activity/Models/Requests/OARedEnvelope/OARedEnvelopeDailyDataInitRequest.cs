using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     公众号领红包 - 每日数据初始化    请求类
    /// </summary>
    public class OARedEnvelopeDailyDataInitRequest
    {
        /// <summary>
        ///     日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        ///     强制初始化
        /// </summary>
        public bool IsForce { get; set; }

        /// <summary>
        ///     公众号类型 1主号
        /// </summary>
        public int OfficialAccountType { get; set; } = 1;
    }
}
