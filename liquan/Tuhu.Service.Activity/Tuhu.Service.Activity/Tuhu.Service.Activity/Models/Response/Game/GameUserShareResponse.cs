namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    ///     用户分享 - 返回值
    /// </summary>
    public class GameUserShareResponse
    {
        /// <summary>
        ///     分享成功后，增加的积分
        /// </summary>
        public int Point { get; set; }

        /// <summary>
        ///     分享成功后，返回的距离【马牌】
        /// </summary>
        public int Distance { get; set; }
    }
}
