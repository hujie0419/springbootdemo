using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    /// SetDiscountProductDetailImgAsync 接口请求参数
    /// </summary>
    public class SetDiscountProductDetailImgRequest
    {
        /// <summary>
        /// 打折活动id
        /// </summary>
        public string ActivityId { get; set; }

        /// <summary>
        /// 打折商品Pid
        /// </summary>
        public List<string> Pid { get; set; }

        /// <summary>
        /// 图片url
        /// </summary>
        public string DetailImgUrl { get; set; }

        /// <summary>
        /// 操作用户
        /// </summary>
        public string Operator { get; set; }

    }
}
