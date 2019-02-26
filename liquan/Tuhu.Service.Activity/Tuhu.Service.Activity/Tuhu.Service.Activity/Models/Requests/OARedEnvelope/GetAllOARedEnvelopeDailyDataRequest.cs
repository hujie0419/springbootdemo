using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     获取生成的全部红包对象 - 请求类
    /// </summary>
    public class GetAllOARedEnvelopeDailyDataRequest
    {
        /// <summary>
        ///     日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        ///     公众号类型 1主号
        /// </summary>
        public int OfficialAccountType { get; set; } = 1;
    }
}
