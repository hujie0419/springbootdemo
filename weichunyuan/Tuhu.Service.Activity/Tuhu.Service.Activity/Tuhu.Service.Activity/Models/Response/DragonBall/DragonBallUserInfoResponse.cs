namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    ///    七龙珠 -   用户当前龙珠总数/召唤次数 返回类
    /// </summary>
    public class DragonBallUserInfoResponse
    {
        /// <summary>
        ///     用户当前龙珠总数
        /// </summary>
        public int DragonBallCount { get; set; }

        /// <summary>
        ///     用户当前召唤神龙次数
        /// </summary>
        public int DragonBallSummonCount { get; set; }
    }
}
