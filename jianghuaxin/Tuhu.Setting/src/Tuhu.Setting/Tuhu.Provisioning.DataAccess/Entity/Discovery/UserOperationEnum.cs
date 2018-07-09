using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Entity.Discovery
{
    public enum UserOperationEnum
    {
        /// <summary>
        /// 阅读
        /// </summary>
        Read,
        /// <summary>
        /// 分享
        /// </summary>
        Share,
        /// <summary>
        /// 评论
        /// </summary>
        Comment,
        /// <summary>
        /// 收藏
        /// </summary>
        Favorite,
        /// <summary>
        /// 取消收藏
        /// </summary>
        CancelFavorite,
    }
}
