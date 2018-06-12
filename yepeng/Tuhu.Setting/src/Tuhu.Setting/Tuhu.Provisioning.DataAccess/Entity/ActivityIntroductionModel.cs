using System;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class ActivityIntroductionModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// 活动渠道
        /// </summary>
        public string ActivityChannel { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        ///结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        ///活动内容
        /// </summary>
        public string ActivityContent { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Shorder { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }
    }
}
