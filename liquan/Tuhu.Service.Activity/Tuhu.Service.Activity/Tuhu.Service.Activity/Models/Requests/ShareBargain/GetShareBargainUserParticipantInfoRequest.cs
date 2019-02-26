using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests 
{
    /// <summary>
    /// GetShareBargainUserParticipantInfoAsync 接口请求实体  - 获取砍价配置当前用户参与信息
    /// </summary>
    public class GetShareBargainUserParticipantInfoRequest
    {
        /// <summary>
        /// 砍价配置主键
        /// </summary>
        public int ActivityProductID { get; set; }
        

    }
}
