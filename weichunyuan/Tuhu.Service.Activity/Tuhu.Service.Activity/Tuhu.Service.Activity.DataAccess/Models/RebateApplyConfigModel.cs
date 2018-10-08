using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.DataAccess.Models
{
    public class RebateApplyConfigModel
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string UserPhone { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status{ get; set; }
        /// <summary>
        /// 微信ID
        /// </summary>
        public string WXId { get; set; }
        /// <summary>
        /// 微信昵称
        /// </summary>
        public string WXName { get; set; }
        /// <summary>
        /// 微信授权唯一ID
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 百度ID
        /// </summary>
        public string BaiDuId { get; set; }
        /// <summary>
        /// 百度Name
        /// </summary>
        public string BaiDuName { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public string PrincipalPerson { get; set; }
        /// <summary>
        /// 返现金额
        /// </summary>
        public decimal RebateMoney { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
    }
}
