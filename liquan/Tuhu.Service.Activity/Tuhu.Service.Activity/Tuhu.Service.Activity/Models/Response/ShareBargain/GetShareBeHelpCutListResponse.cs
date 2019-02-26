using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    /// GetShareBeHelpCutListAsync  接口返回实体 - 获取砍价分享的被帮砍记录
    /// </summary>
    public class GetShareBeHelpCutListResponse
    {
        public Guid UserId { get; set; }

        /// <summary>
        /// 砍价配置主键
        /// </summary>
        public int ActivityProductId { get; set; }

        /// <summary>
        /// 当前已砍去价格
        /// </summary>
        public decimal TotalReduce { get; set; }

        /// <summary>
        /// 当前已分享砍价次数
        /// </summary>
        public int CurrentlyTimes { get; set; }

        /// <summary>
        /// 受邀人 帮砍记录
        /// </summary>
        public List<ShareBargainBeHelped> BeHelpedList { get; set; } 

    }

    /// <summary>
    /// 受邀人 帮砍记录
    /// </summary>
    public class ShareBargainBeHelped
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
        /// 参与砍价的时间
        /// </summary>
        public DateTime BargainTime { get; set; }

        /// <summary>
        /// 当前商品总共被砍去的价格
        /// </summary>
        public decimal TotalReduce { get; set; }

    }

}
