using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tuhu.Provisioning.Models
{
    public class SendPromotionTaskMQModel
    {
        /// <summary>
        /// taskid
        /// </summary>
        public int PromotionTaskId { get; set; }
        /// <summary>
        /// 开始发的起始位置
        /// </summary>
        public int MinPkid { get; set; }
        /// <summary>
        /// 触发任务 的订单号
        /// </summary>
        public string OrderNo { get; set; }
        /// <summary>
        /// 是发短信 还是执行发券 0.先发券 ，后发短信 .1.发券，2.发短信
        /// </summary>
        public int ExcuteType { get; set; }

        public string RouteKey { get; set; }
    }
}