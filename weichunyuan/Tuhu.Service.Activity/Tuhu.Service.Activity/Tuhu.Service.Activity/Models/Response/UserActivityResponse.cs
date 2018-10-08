using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Response
{
    public class UserActivityResponse
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long PKID { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        public string Content { get; set; }
    }
}
