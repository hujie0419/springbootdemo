using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.ActivityJob.Models.Monitor
{
    /// <summary>
    /// 高德GPS定位响应结果
    /// </summary>
    public class AMapGpsResult : AMapResult
    {
        /// <summary>
        /// 逆地理编码
        /// </summary>
        public RegeoCode RegeoCode { get; set; }
    }

    public class RegeoCode
    {
        /// <summary>
        /// 结构化地址信息（省份＋城市＋区县＋城镇＋乡村＋街道＋门牌号码）
        /// </summary>
        public string Formatted_Address { get; set; }
    }
}
