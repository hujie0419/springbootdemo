using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    /// GetBargainOwnerExpireListAsync  获取 砍价失败的发起记录  接口返回实体
    /// </summary>
    public class GetBargainOwnerExpireListResponse
    {
        /// <summary>
        /// 发起砍价唯一标识
        /// </summary>
        public Guid IdKey { get; set; }

        /// <summary>
        /// 发起砍价用户
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 砍价商品显示名称
        /// </summary>
        public string ProductName { get; set; }

    }
}
