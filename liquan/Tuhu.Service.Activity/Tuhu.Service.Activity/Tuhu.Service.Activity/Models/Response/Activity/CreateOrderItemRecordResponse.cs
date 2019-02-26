using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    ///  佣金订单商品记录接口返回实体
    /// </summary>
    public class CreateOrderItemRecordResponse
    {
        /// <summary>
        /// true:操作成功; false:操作失败
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 失败消息
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
