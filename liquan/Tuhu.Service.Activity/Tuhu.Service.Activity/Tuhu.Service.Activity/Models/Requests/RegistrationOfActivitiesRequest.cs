using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    /// 活动报名页的请求类
    /// </summary>
    public class RegistrationOfActivitiesRequest
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 客户端ip
        /// </summary>
        public string ClientIp { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 申请理由
        /// </summary>
        public string ApplicationReasons { get; set; }

        /// <summary>
        /// 上传图片
        /// </summary>
        public List<string> Pictures { get; set; }
    }
}
