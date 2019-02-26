using System;
namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 福利类型奖励DB对象
    /// </summary>
    public class TaskTypeRewardModel
    {
        /// <summary>
        /// 主键，自增
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 福利类型表外键ID
        /// </summary>
        public int TaskTypeID { get; set; }

        /// <summary>
        /// 福利类型值
        /// </summary>
        public string TaskTypeCode { get; set; }

        /// <summary>
        /// 福利类型名称
        /// </summary>
        public string TaskTypeName { get; set; }

        /// <summary>
        /// 奖励说明文案
        /// </summary>
        public string DescriptionTitle { get; set; }

        /// <summary>
        /// 奖励金额
        /// </summary>
        public int RewardValue { get; set; }

        /// <summary>
        /// 剩余天数 倒计时设置
        /// </summary>
        public int RemainingDay { get; set; }

        /// <summary>
        /// 状态：0 不开启，1 开启 是否开启奖励
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 状态字符类型
        /// </summary>
        public string StrStatus { get { return Status == 1 ? "启用" : "禁用"; } }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        /// 最后修改人
        /// </summary>
        public string LastUpdateBy { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        /// 最后修改时间字符类型
        /// </summary>
        public string StrLastUpdateDateTime
        {
            get { return LastUpdateDateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        /// <summary>
        /// 是否删除，0=未删除；1已删除
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
