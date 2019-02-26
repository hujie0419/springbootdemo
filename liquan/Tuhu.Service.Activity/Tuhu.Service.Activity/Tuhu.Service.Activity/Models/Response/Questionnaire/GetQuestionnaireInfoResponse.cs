using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.Activity.Models.Response
{
    public class GetQuestionnaireInfoResponse
    {
        /// <summary>
        /// 问卷标识
        /// </summary>
        public long QuestionnaireID { get; set; }
        /// <summary>
        /// 问卷名称
        /// </summary>
        public string QuestionnaireName { get; set; }
        /// <summary>
        /// 问卷类型，1=问答；2=投票；3=测试；4=报名
        /// </summary>
        public int QuestionnaireType { get; set; }
        /// <summary>
        /// 是否启用活动规则，0=不启用；1=启用
        /// </summary>
        public int IsShowRegulation { get; set; }
        /// <summary>
        /// 规则内容
        /// </summary>
        public string Regulation { get; set; }
        /// <summary>
        /// 提交结束信息
        /// </summary>
        public string CompletionInfo { get; set; }
        /// <summary>
        /// 参与人数
        /// </summary>
        public int ParticipantsCount { get; set; }
        /// <summary>
        /// 问题列表
        /// </summary>
        public List<Question> QuestionList { get; set; }
    }
    /// <summary>
    /// 问题类
    /// </summary>
    public class Question
    {
        /// <summary>
        /// 问题标识
        /// </summary>
        public long QuestionID { get; set; }
        /// <summary>
        /// 问题标题
        /// </summary>
        public string QuestionTitle { get; set; }
        /// <summary>
        /// 问题类型，1=单行输入；2=多行输入；3=单选；4=多选；5=下拉框；6=手机号；7=地址；8=日期；9=图片单选；10=图片多选
        /// </summary>
        public int QuestionType { get; set; }
        /// <summary>
        /// 是否分叉，0=不分叉；1=分叉
        /// </summary>
        public int IsFork { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }
        /// <summary>
        /// 是否必填,1=必填；0=非必填
        /// </summary>
        public int IsRequired { get; set; }
        /// <summary>
        /// 是否验证最少字符，1=验证；0=不验证
        /// </summary>
        public int IsValidateMinChar { get; set; }
        /// <summary>
        /// 最少多少字
        /// </summary>
        public int MinChar { get; set; }
        /// <summary>
        /// 是否验证最大字数，1=验证；0=不验证
        /// </summary>
        public int IsValidateMaxChar { get; set; }
        /// <summary>
        /// 最大字数
        /// </summary>
        public int MaxChar { get; set; }
        /// <summary>
        /// 问题排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 问题选项列表
        /// </summary>
        public List<QuestionOption> QuestionOptionList { get; set; }

        /// <summary>
        ///     问题的文字结果（可选）（只做展示用）
        /// </summary>
        public string QuestionTextResult { get; set; }

        /// <summary>
        ///     下线日期
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        ///     问题状态  0 正常 1 竞猜截止 2 已经参与 
        /// </summary>
        public int QuestionStatus { get; set; }

        /// <summary>
        ///     用户的答案
        /// </summary>
        public string UserAnswer { get; set; }

        /// <summary>
        ///     判断问题是否截止
        /// </summary>
        public DateTime? DeadLineTime { get; set; }
    }
    /// <summary>
    /// 问题选项类
    /// </summary>
    public class QuestionOption
    {
        /// <summary>
        /// 问题选项标识
        /// </summary>
        public long OptionID { get; set; }

        /// <summary>
        /// 问题id
        /// </summary>
        public long QuestionID { get; set; }

        /// <summary>
        /// 选项内容
        /// </summary>
        public string OptionContent { get; set; }
        /// <summary>
        /// 分叉的问题标识
        /// </summary>
        public long ForkQuestionID { get; set; }
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
        public long? UseIntegral { get; set; }

        /// <summary>
        ///     胜利后 获得的  兑换券
        /// </summary>
        public long? WinCouponCount { get; set; }

        /// <summary>
        ///     是否用户选择的选项
        /// </summary>
        public bool? IsUserAnswer { get; set; }

        /// <summary>
        ///     子选项
        /// </summary>
        public List<QuestionOption> SubOptions { get; set; }
    }
}
