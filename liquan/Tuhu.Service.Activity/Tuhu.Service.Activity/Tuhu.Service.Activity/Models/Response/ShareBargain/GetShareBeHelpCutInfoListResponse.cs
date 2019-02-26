using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    /// GetShareBeHelpCutInfoListAsync 接口返回实体 - 获取砍价分享的被帮砍记录
    /// </summary>
    public class GetShareBeHelpCutInfoListResponse
    {
        /// <summary>
        /// 受邀人UserId
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 砍去的价格
        /// </summary>
        public decimal Reduce { get; set; }

        /// <summary>
        /// 砍去金额和平均数的比例 : 0.5-0.8 摸了一下，0.8-1.2 华丽一刀，1.2-1.5 暴击了
        /// </summary>
        public double Rate { get; set; }

        /// <summary>
        /// 提示文案
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// 帮砍时间
        /// </summary>
        public DateTime HelpCutTime { get; set; }

        /// <summary>
        /// 当前商品总共被砍去的价格
        /// </summary>
        public decimal TotalReduce { get; set; }

    }
}
