namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     获取 用户助力信息 - 请求类
    /// </summary>
    public class GetGameUserSupportInfoRequest : GameObjectRequest
    {
        /// <summary>
        ///     OpenId
        /// </summary>
        public string OpenId { get; set; }
    }
}
