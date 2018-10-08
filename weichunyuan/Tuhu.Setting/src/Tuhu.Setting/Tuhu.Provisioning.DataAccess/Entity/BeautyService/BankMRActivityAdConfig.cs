using System;

namespace Tuhu.Provisioning.DataAccess.Entity.BeautyService
{
    public class BankMRActivityAdConfig
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PKID { get; set; }

        /// <summary>
        /// 活动ID
        /// </summary>
        public Guid ActivityId { get; set; }

        /// <summary>
        /// 广告类型
        /// </summary>
        public string AdType { get; set; }

        /// <summary>
        /// 图片链接
        /// </summary>
        public string ImgUrl { get; set; }

        /// <summary>
        /// 跳转链接
        /// </summary>
        public string JumpUrl { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}