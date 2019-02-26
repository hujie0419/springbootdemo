namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     公众号领红包 - 红包领取动态 请求类
    /// </summary>
    public class OARedEnvelopeReceiveUpdatingsRequest
    {
        /// <summary>
        ///     需求的数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        ///     公众号类型 1主号
        /// </summary>
        public int OfficialAccountType { get; set; }
    }
}
