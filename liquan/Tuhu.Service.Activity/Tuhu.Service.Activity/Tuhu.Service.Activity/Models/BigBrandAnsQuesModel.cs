using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    /// <summary>
    /// 问答抽奖配置
    /// </summary>
  public  class BigBrandAnsQuesModel
    {
        /// <summary>
        /// 题目标签
        /// </summary>
        public string Tip { get; set; }

        /// <summary>
        /// 题目数量
        /// </summary>
        public int TipCount { get; set; }

        /// <summary>
        /// 默认标题
        /// </summary>
        public string DefTitle { get; set; }

        /// <summary>
        /// 默认小标题
        /// </summary>
        public string DefSmallTitle { get; set; }

        /// <summary>
        /// 分享标题
        /// </summary>
        public string ShareTitle { get; set; }

        /// <summary>
        /// 分享副标题
        /// </summary>
        public string ShareSmallTitle { get; set; }

        /// <summary>
        /// 无次数标题
        /// </summary>
        public string NoTimeTitle { get; set; }

        /// <summary>
        /// 无次数副标题
        /// </summary>
        public string NoTimeSmallTitle { get; set; }

        /// <summary>
        /// 默认结果标题
        /// </summary>
        public string DefResTitle { get; set; }

        /// <summary>
        /// 默认结果副标题
        /// </summary>
        public string DefResSmallTitle { get; set; }

        /// <summary>
        /// 分享结果标题
        /// </summary>
        public string ShareResTitle { get; set; }

        /// <summary>
        /// 分享结果副标题
        /// </summary>
        public string ShareResSmallTitle { get; set; }

        /// <summary>
        /// 背景图片
        /// </summary>
        public string BgImgUri { get; set; }

        /// <summary>
        /// 最高奖励图片
        /// </summary>
        public string LastImgUri { get; set; }

        public string HomeBgImgUri { get; set; }

        public string ResultImgUri { get; set; }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool Is_Deleted { get; set; }
    }
}
