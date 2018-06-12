using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models
{
    public class MJsonResult
    {
        /// <summary>
        /// 标识执行过程的状态
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 执行结果 -用于返回
        /// </summary>
        public string Msg { get; set; }
    }
}