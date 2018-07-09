using System;
using System.Collections.Generic;

namespace Tuhu.Service.Activity.Models.Requests
{
    public class FlashSaleOrderRequest
    {
        public Guid UserId { get; set; }
        public string DeviceId { get; set; }
        public string UseTel { get; set; }
        public int OrderId { get; set; }
        public int OrderListId { get; set; }
        public IEnumerable<OrderItems> Products { get; set; }
    }
    public class OrderItems
    {
        public int Num { get; set; }
        public string PID { get; set; }
        public Guid? ActivityId { get; set; }
        public string AllPlaceLimitId { get; set; }
        public int Type { get; set; } = 1;
    }
    public class FlashSaleOrderResponse
    {
        public bool IsCanBuy { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public enum CheckFlashSaleStatus
    {
        /// <summary>
        /// 验证通过
        /// </summary>
        Succeed = 1,
        /// <summary>
        /// 活动不存在或时间不对或活动不包含此PID
        /// </summary>
        NoExist = -3,

        /// <summary>
        /// 总剩余数量不足
        /// </summary>
        NoEnough = -4,

        /// <summary>
        /// 剩余购买数量不足
        /// </summary>
        MaxQuantityLimit = -5,

        /// <summary>
        /// 会场限购
        /// </summary>
        PlaceQuantityLimit = -6,
        /// <summary>
        /// sql执行失败
        /// </summary>
        SqlExecuteFiled = -7,

        /// <summary>
        /// redis执行失败
        /// </summary>
        NoSqlSqlExecuteFiled = -8


    }
}
