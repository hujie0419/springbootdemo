using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests.Activity
{

    /// <summary>
    /// 佣金商品详情接口查询请求实体
    /// </summary>
    public class GetCommissionProductDetatilsRequest
    {
        /// <summary>
        /// 业务ID
        /// </summary>
        public Guid CpsId { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string PID { get; set; }
    }
}
