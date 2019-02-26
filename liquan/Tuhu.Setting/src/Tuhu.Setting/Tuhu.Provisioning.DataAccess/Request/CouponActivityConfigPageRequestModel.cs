using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Provisioning.DataAccess.Request
{
    /// <summary>
    ///  列表请求实体
    /// </summary>
    public class CouponActivityConfigPageRequestModel
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 页面大小
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public int Type { get; set; }
    }
}
