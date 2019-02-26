using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     里程碑信息 请求类
    /// </summary>
    public class GetGameMilepostInfoRequest : GameObjectRequest
    {
        /// <summary>
        ///     用户ID
        /// </summary>
        public Guid UserId { get; set; }
    }
}
