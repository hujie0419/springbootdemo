using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.ActivityJob.Models.Monitor
{
    /// <summary>
    /// 高德IP定位响应结果
    /// </summary>
    public class AMapIpResult : AMapResult
    {
        /// <summary>
        /// 省份名称（若为直辖市则显示直辖市名称；如果在局域网 IP网段内，则返回“局域网”；非法IP以及国外IP则返回空）
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 城市名称（若为直辖市则显示直辖市名称；如果为局域网网段内IP或者非法IP或国外IP，则返回空）
        /// </summary>
        public string City { get; set; }
    }
}
