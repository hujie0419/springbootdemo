using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.ActivityJob.Models.Monitor
{
    /// <summary>
    /// 聚合手机号码归属地响应结果
    /// </summary>
    public class JuHeMobileResult
    {
        /// <summary>
        /// 结果状态值（200表示成功）
        /// </summary>
        public int ResultCode { get; set; }

        /// <summary>
        /// 结果说明
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 响应结果
        /// </summary>
        public Result Result { get; set; }
    }

    public class Result
    {
        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 运营商
        /// </summary>
        public string Company { get; set; }
    }
}
