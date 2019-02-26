using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    public class RebateApplyPageConfig
    {
        /// <summary>
        /// PKID
        /// </summary>
        public int PKID { get; set; }
        /// <summary>
        /// 背景图片
        /// </summary>
        public string BackgroundImg { get; set; }
        /// <summary>
        /// 规则
        /// </summary>
        public string ActivityRules { get; set; }
        /// <summary>
        /// 红包文案
        /// </summary>
        public string RedBagRemark { get; set; }
        /// <summary>
        /// 返现成功消息
        /// </summary>
        public string RebateSuccessMsg { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public List<RebateApplyImageConfig> ImgList { get; set; }
    }

    public class RebateApplyImageConfig
    {
        /// <summary>
        /// 图片链接
        /// </summary>
        public string ImgUrl { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 渠道
        /// </summary>
        public string Source { get; set; }
    }
}
