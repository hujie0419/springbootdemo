using System;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    ///    删除用户ID
    /// </summary>
    public class DeleteGameUserDataRequest : GameObjectRequest
    {
        /// <summary>
        ///     用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string OpenId { get; set; }
    }
}
