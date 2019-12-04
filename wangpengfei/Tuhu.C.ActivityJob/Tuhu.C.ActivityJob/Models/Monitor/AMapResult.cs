using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.ActivityJob.Models.Monitor
{
    /// <summary>
    /// 高德定位响应结果
    /// </summary>
    public class AMapResult
    {
        /// <summary>
        /// 返回结果状态值（值为0或1，0表示失败；1表示成功）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 返回状态说明（status为0时，info返回错误原因，否则返回“OK”）
        /// </summary>
        public string Info { get; set; }
    }
}
