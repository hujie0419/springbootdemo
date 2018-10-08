using System;
using System.Collections.Generic;

namespace Tuhu.Service.Activity.Models
{
    public class VipCardBaseModel
    {
        /// <summary>
        /// 批次id
        /// </summary>
        public string BatchId { get; set; }
    }
   public  class VipCardSaleConfigDetailModel: VipCardBaseModel
    {
        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// 企业客户id
        /// </summary>
        public int ClientId { get; set; }
        /// <summary>
        /// 企业客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Vip卡名称
        /// </summary>
        public string CardName { get; set; }
        /// <summary>
        /// Vip卡面额
        /// </summary>
        public int CardValue { get; set; }
        /// <summary>
        /// 销售单价
        /// </summary>
        public int SalePrice { get; set; }
        /// <summary>
        /// 使用范围
        /// </summary>
        public string UseRange { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 剩余库存
        /// </summary>
        public int RemainingStock { get; set; }
        /// <summary>
        /// 库存
        /// </summary>
        public int Stock { get; set; }
    }

    public class VipCardBatchModel: VipCardBaseModel
    {
        /// <summary>
        /// 卡够买数量
        /// </summary>
        public int CardNum { get; set; }
    }
    public class VipCardRecordRequest 
    {
        /// <summary>
        /// 活动id
        /// </summary>
        public string ActivityId { get; set; }

        /// <summary>
        /// 订单id
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 用户手机号
        /// </summary>
        public string UserPhone { get; set; }

        public List<VipCardBatchModel> Batches { get; set; }
    }
}
