using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    /// GetShareBargainProductInfo 获取砍价活动商品配置信息 接口请求实体
    /// </summary>
    public class GetShareBargainProductInfoRequest
    {
        /// <summary>
        /// 砍价活动唯一标识
        /// </summary>
        public int ActivitiProductID { get; set; }

    }
}
