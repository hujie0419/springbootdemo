using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Response
{
   public class QuestionAnsResponse
    {
        /// <summary>
        /// 是否正确
        /// </summary>
        public bool IsReal { get; set; }

        /// <summary>
        /// 正确结果
        /// </summary>
        public string OptionsReal { get; set; }

        /// <summary>
        /// 答对题目数
        /// </summary>
        public int? RealValue { get; set; }

        /// <summary>
        /// 打错题目数
        /// </summary>
        public int? Error { get; set; }
    }
}
