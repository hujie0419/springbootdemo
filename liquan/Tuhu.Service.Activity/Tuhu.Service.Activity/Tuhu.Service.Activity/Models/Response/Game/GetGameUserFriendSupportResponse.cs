using System.Collections.Generic;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    ///     获取用户好友助力 - 返回类
    /// </summary>
    public class GetGameUserFriendSupportResponse
    {
        /// <summary>
        ///     助力明细列表
        /// </summary>
        public IList<GetGameUserFriendSupportResponseItem> SupportItems { get; set; } =
            new List<GetGameUserFriendSupportResponseItem>();

        public class GetGameUserFriendSupportResponseItem
        {
            /// <summary>
            ///     助力人的名称
            /// </summary>
            public string NickName { get; set; }

            /// <summary>
            ///     头像IMG URL
            /// </summary>
            public string HeadImgURL { get; set; }

            /// <summary>
            ///     助力次数
            /// </summary>
            public int SupportCount { get; set; }
        }
    }
}
