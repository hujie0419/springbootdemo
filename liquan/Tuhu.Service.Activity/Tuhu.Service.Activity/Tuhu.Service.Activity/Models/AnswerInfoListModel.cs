using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models
{
   public class AnswerInfoListModel
    {
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
        /// 是否删除
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool Is_Deleted { get; set; }
    }
}
