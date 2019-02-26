using System.Collections.Generic;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    ///     公众号领红包 - 红包领取动态
    /// </summary>
    public class OARedEnvelopeReceiveUpdatingsResponse
    {
        /// <summary>
        ///     领取的数组
        /// </summary>
        public List<OARedEnvelopeReceiveUpdatingsItem> Items { get; set; } =
            new List<OARedEnvelopeReceiveUpdatingsItem>();
    }

    /// <summary>
    ///     领取的明细对象
    /// </summary>
    public class OARedEnvelopeReceiveUpdatingsItem
    {
        /// <summary>
        ///     用户昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        ///     领取到的红包
        /// </summary>
        public decimal GetMoney { get; set; }

        /// <summary>
        ///     用户头像
        /// </summary>
        public string WXHeadImgUrl { get; set; }
    }
}
