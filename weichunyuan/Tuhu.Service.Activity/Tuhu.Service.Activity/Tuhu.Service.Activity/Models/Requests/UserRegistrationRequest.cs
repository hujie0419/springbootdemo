using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{
    public class UserRegistrationRequest
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long PKID { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 所在区域
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 活动Id
        /// </summary>
        public long ActivityId { get; set; }
    }
}
