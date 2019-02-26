using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests 
{
    /// <summary>
    /// GetBargainProductAndUserInfo    获取砍价商品配置和用户发起砍价信息  接口请求参数实体
    /// </summary>
    public class GetBargainProductAndUserInfoRequest
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 发起砍价唯一标识
        /// </summary>
        public Guid IdKey { get; set; }

        /// <summary>
        /// 砍价配置主键
        /// </summary>
        public int ActivityProductID { get; set; }

        /// <summary>
        /// 砍价商品pid
        /// </summary>
        public string Pid { get; set; }

    }
}
