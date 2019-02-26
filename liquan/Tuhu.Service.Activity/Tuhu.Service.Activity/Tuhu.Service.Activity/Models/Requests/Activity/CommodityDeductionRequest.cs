namespace Tuhu.Service.Activity.Models.Requests
{
    /// <summary>
    /// 订单商品扣佣接口请求实体
    /// </summary>
    public class CommodityDeductionRequest
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId { get; set; }
    }
}
