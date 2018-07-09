namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    ///     奖品/兑换物 客户兑换明细表
    /// </summary>
    public class ActivityPrizeOrderDetailResponse
    {

        /// <summary>
        ///     冗余-tbl_Activity 的 ActivityName
        /// </summary>
        public string ActivityName { get; set; }
            
        /// <summary>
        ///     冗余-兑换的商品名称
        /// </summary>
        public string ActivityPrizeName { get; set; }

        /// <summary>
        ///     冗余-兑换商品的照片url
        /// </summary>
        public string ActivityPrizePicUrl { get; set; }

        /// <summary>
        ///     实际支付的兑换券数量
        /// </summary>
        public int CouponCount { get; set; }

    }
}
