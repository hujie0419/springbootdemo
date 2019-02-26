using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
    /// <summary>
    /// 问答抽奖题目
    /// </summary>
    public class BigBrandQuesList
    {
        public int PKID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        /// 用户UserId
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 是否回答
        /// </summary>
        public bool IsFinish { get; set; }

        /// <summary>
        /// 回答结果
        /// </summary>
        public string ResValue { get; set; }

        /// <summary>
        /// 题目标签
        /// </summary>
        public string Tip { get; set; }

        /// <summary>
        /// 题目
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// 选项A
        /// </summary>
        public string OptionsA { get; set; }

        /// <summary>
        /// 选项B
        /// </summary>
        public string OptionsB { get; set; }

        /// <summary>
        /// 选项C
        /// </summary>
        public string OptionsC { get; set; }

        /// <summary>
        /// 选项C
        /// </summary>
        public string OptionsD { get; set; }

        /// <summary>
        /// 正确结果
        /// </summary>
        public string OptionsReal { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderBy { get; set; }

        /// <summary>
        /// 试卷编号
        /// </summary>
        public string CurAnsNo { get; set; }

        /// <summary>
        /// 抽奖hashkey
        /// </summary>
        public string HashKey { get; set; }

    }
}
