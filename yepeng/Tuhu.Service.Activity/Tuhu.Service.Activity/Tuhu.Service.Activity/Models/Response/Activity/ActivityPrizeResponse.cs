namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    ///     奖品/兑换物表
    /// </summary>
    public class ActivityPrizeResponse
    {
        /// <summary>
        ///     主键
        /// </summary>
        public long PKID { get; set; }

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
        ///     该兑换物是否是只读，不能线上销售，只做展示
        /// </summary>
        public bool IsDisableSale { get; set; }

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

        /// <summary>
        ///     0 正常 1 已经兑换  2 已经兑光  3 只读
        /// </summary>
        public int ActivityPrizeStatus { get; set; }
    }
}
