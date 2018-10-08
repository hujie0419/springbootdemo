using System;

namespace Tuhu.Provisioning.DataAccess.Entity.CustomersActivity
{
    /// <summary>
    /// 大客户活动专享配置表
    /// </summary>
    public class CustomerExclusiveSettingModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public int PKID { get; set; }

        /// <summary>
        /// 活动专享ID,系统自动生成
        /// </summary>
        ///
        public string ActivityExclusiveId { get; set; }

        /// <summary>
        /// 订单渠道
        /// </summary>
        public string OrderChannel { get; set; }

        /// <summary>
        /// 大客户ID
        /// </summary>
        public int LargeCustomersID { get; set; }

        /// <summary>
        /// 大客户名称
        /// </summary>
        public string LargeCustomersName { get; set; }


        /// <summary>
        /// 活动链接
        /// </summary>
        public string EventLink { get; set; }

        /// <summary>
        /// 限时抢购ID
        /// </summary>
        public string ActivityId { get; set; }

        /// <summary>
        /// 首页广告图片地址
        /// </summary>
        public string ImageUrl { get; set; }


        /// <summary>
        /// 企业热线
        /// </summary>
        public string BusinessHotline { get; set; }


        /// <summary>
        /// 是否启用;  0 否; 1是;
        /// </summary>
        public string IsEnable { get; set; }


        /// <summary>
        /// 是否删除;  0否; 1是;
        /// </summary>
        public string IsDelete { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateDatetime { get; set; }
        /// <summary>
        /// 更新人
        /// </summary>
        public string UpdateBy { get; set; }

    }
}
