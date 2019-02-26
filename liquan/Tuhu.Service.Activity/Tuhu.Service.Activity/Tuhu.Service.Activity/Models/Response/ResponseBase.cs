using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    /// 服务接口返回实体的基类
    /// </summary>
    public class ResponseBase
    {

        /// <summary>
        /// 受影响的行数
        /// </summary>
        public int ResponseRow { get; set; }

        /// <summary>
        ///响应Code
        /// </summary>
        public string ResponseCode { get; set; }

        /// <summary>
        /// 响应消息
        /// </summary>
        public string ResponseMessage { get; set; }

    }
}
