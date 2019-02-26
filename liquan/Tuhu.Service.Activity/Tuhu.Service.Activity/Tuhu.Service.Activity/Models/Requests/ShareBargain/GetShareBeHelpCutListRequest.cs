using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    /// GetShareBeHelpCutListAsync  接口请求参数 获取砍价分享的被帮砍记录
    /// </summary>
    public class GetShareBeHelpCutListRequest
    {
        /// <summary>
        /// 发起砍价分享唯一标识
        /// </summary>
        public Guid IdKey { get; set; }

        /// <summary>
        /// 分页页号
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 分页页码
        /// </summary>
        public int PageSize { get; set; }

    }
}
