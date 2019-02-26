using System;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    ///     用户领红包 - 领取返回 返回值
    /// </summary>
    public class OARedEnvelopeUserInfoResponse
    {
        /// <summary>
        ///     获得的金额
        /// </summary>
        public decimal GetMoney { get; set; }

        /// <summary>
        ///     获取的时间
        /// </summary>
        public DateTime GetDate { get; set; }
    }
}
