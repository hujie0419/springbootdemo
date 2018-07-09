using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    public class RegionActivityPageModel
    {
        /// <summary>
        /// 活动ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 活动的url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// code=0：没有该活动
        /// code=1：活动未开始
        /// code=2：活动正在进行中，拿到url活动链接
        /// code=3：活动已经结束
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 分享元素
        /// </summary>
        public string ShareParameters { get; set; }
        /// <summary>
        /// 页面title
        /// </summary>
        public string Name { get; set; }
    }
}
