using System;

namespace Tuhu.Service.Activity.DataAccess.Models.Activity
{
    /// <summary>
    ///     奖品/兑换物表
    /// </summary>
    public class ActivityPrizeModel
    {
        /// <summary>
        ///     主键
        /// </summary>
        public long PKID { get; set; }

        /// <summary>
        ///     外键：关联表 tbl_Activity
        /// </summary>
        public long ActivityId { get; set; }

        /// <summary>
        ///     缩略图地址
        /// </summary>
        public string PicUrl { get; set; }

        /// <summary>
        ///     库存
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        ///     总库存
        /// </summary>
        public int SumStock { get; set; }

        /// <summary>
        ///     优惠券规则ID
        /// </summary>
        public Guid GetRuleId { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateDatetime { get; set; }

        /// <summary>
        ///     最后修改时间
        /// </summary>
        public DateTime LastUpdateDateTime { get; set; }

        /// <summary>
        ///     当前状态是否删除 删除为1  不删除为0
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        ///     乐观锁 更新的时候需要增加此字段的判断条件
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        ///     该兑换物是否是只读，不能线上销售，只做展示
        /// </summary>
        public bool IsDisableSale { get; set; }

        /// <summary>
        ///     商品ID
        /// </summary>
        public string PID { get; set; }

        /// <summary>
        ///     需要的兑换券数量
        /// </summary>
        public int CouponCount { get; set; }

        /// <summary>
        ///     0 未上架  1 已上架
        /// </summary>
        public int OnSale { get; set; }

        /// <summary>
        ///     奖品名称
        /// </summary>
        public string ActivityPrizeName { get; set; }
    }
}
