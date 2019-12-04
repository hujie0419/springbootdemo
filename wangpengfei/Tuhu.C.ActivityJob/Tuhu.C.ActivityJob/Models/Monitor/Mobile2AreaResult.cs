using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.C.ActivityJob.Models.Monitor
{
    /// <summary>
    /// 途虎手机号码归属地查询响应结果
    /// </summary>
    public class Mobile2AreaResult
    {
        /// <summary>
        /// 返回码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 返回码的描述信息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 电话号码返回归属地以及运营商信息
        /// </summary>
        public DataInfo Data { get; set; }
    }

    public class DataInfo
    {
        /// <summary>
        /// 省、直辖市、自治区
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 运营商信息
        /// </summary>
        public string Operator { get; set; }
    }
}
