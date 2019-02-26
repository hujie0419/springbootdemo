using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///     获取 用户里程收支明细 - 请求类
    /// </summary>
    public class GetGameUserDistanceInfoRequest : GameObjectRequest
    {
        /// <summary>
        ///     用户ID
        /// </summary>
        public Guid UserId { get; set; }
    }
}
