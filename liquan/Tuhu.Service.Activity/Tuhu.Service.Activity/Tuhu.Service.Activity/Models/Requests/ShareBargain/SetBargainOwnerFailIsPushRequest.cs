using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    /// SetBargainOwnerFailIsPushAsync 接口请求实体   更新用户发起砍价记录的失败推送状态
    /// </summary>
    public class SetBargainOwnerFailIsPushRequest 
    {
        /// <summary>
        /// 砍价配置主键
        /// </summary>
        public int ActivityProductID { get; set; }


        /// <summary>
        /// 发起砍价配置唯一标识列表
        /// </summary>
        public List<Guid> IdKeyList { get; set; }

    }
}
