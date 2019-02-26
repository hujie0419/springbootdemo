namespace Tuhu.Service.Activity.Models.Response
{
    /// <summary>
    /// 订单商品返佣接口返回实体
    /// </summary>
    public class CommodityRebateResponse
    {
        /// <summary>
        /// true:处理成功; false:处理失败
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 失败消息
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
