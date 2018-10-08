using System;

namespace Tuhu.C.Job.Activity.Models
{
    /// <summary>
    ///     问题对象
    /// </summary>
    public class QuestionModel
    {

        /// <summary>
        ///     主键
        /// </summary>
        public long PKID { get; set; }


        /// <summary>
        ///     问题的文字结果（可选）（只做展示用）
        /// </summary>
        public string QuestionTextResult { get; set; }


    }
}
