using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Response
{
    public class RebateApplyResponse
    {
        /// <summary>
        /// 返现编号
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 拒绝原因
        /// </summary>
        public string RefusalReason { get; set; }
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
        /// 图片
        /// </summary>
        public IEnumerable<string> ImgList { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? CheckTime { get; set; }
        /// <summary>
        /// 返现时间
        /// </summary>
        public DateTime? RebateTime { get; set; }
    }
}
