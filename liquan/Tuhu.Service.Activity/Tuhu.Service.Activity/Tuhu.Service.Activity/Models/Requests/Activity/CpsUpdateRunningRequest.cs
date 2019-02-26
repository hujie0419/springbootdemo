using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{
    public class CpsUpdateRunningRequest
    {
        /// <summary>
        /// 支付订单号（唯一）
        /// </summary>
        public string OutBizNo { get; set; }


        /// <summary>
        /// 金融交易流水号(支付成功写入)
        /// </summary>
        public string TransactionNo { get; set; }


        /// <summary>
        /// 状态，HASBEENSENT.已发送，SENDSUCCESS.发送成功，CALLBACKSUCCESSFUL.回调发送成功,CALLBACKFAILED.回调发送失败
        /// </summary>
        public string Status { get; set; }


        /// <summary>
        /// 失败原因
        /// </summary>
        public string Reason { get; set; }
    }
}
