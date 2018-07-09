namespace Tuhu.C.Job.Activity.Models
{
    /// <summary>
    ///     问题选项类
    /// </summary>
    public class QuestionOptionModel
    {
        /// <summary>
        ///     主键
        /// </summary>
        public long PKID { get; set; }

        /// <summary>
        ///     选项ID
        /// </summary>
        public long QuestionID { get; set; }


        /// <summary>
        ///     指向自己的PKID  作为父ID使用 
        /// </summary>
        public long QuestionParentID { get; set; }

        /// <summary>
        ///     是否是正确选项 1是
        /// </summary>
        public int IsRightValue { get; set; }

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
