using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Questionnaire
{
    public class QuestionOptionModel
    {
        public long OptionID { get; set; }
        public long QuestionID { get; set; }
        public string OptionContent { get; set; }
        public int ForkQuestionID { get; set; }
        public int Sort { get; set; }
        /// <summary>
        /// 是否是附加信息
        /// </summary>
        public bool IsAdditionalInfo { get; set; }
        /// <summary>
        /// 是否显示附加信息
        /// </summary>
        public bool IsShowAdditionalInfo { get; set; }

        /// <summary>
        ///     指向自己的PKID  作为父ID使用 
        /// </summary>
        public long QuestionParentID { get; set; }

        /// <summary>
        ///     选择此项 要使用的积分 （可选字段）
        /// </summary>
        public int? UseIntegral { get; set; }

        /// <summary>
        ///     胜利后 获得的  兑换券
        /// </summary>
        public int? WinCouponCount { get; set; }
    }
}
