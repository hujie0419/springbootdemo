using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Requests
{
    public class RebateApplyRequest
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
        /// 微信ID
        /// </summary>
        public string WXId { get; set; }
        /// <summary>
        /// 微信昵称
        /// </summary>
        public string WXName { get; set; }
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
        /// <summary>
        /// 二维码图片
        /// </summary>
        public string QRCodeImg { get; set; }
        /// <summary>
        /// UnionId
        /// </summary>
        public string UnionId { get; set; }
        //
        // 摘要:
        //     账号授权OpenId
        public string OpenId { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public List<string> ImgList { get; set; }
    }
}
