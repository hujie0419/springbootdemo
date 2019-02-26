using System;
using System.Data;
using Tuhu.Component.Framework.Extension;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    /// <summary>
    /// 会员签到积分配置（规则）表
    /// </summary>
   public class UserDailyCheckInConfigModel
    {
        /// <summary>
        /// 主键，自增
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 连续签到天数
        /// </summary>
        public int ContinuousDays { get; set; }

        /// <summary>
        /// 奖励积分
        /// </summary>
        public int RewardIntegral { get; set; }

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
        /// 最后修改时间 字符类型
        /// </summary>
        public string StrLastUpdateDateTime
        {
            get
            { return LastUpdateDateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

    }
}
