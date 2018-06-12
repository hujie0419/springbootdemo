using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.scm.external.wms.return.order.callback
    /// </summary>
    public class AlibabaScmExternalWmsReturnOrderCallbackRequest : BaseTopRequest<Top.Api.Response.AlibabaScmExternalWmsReturnOrderCallbackResponse>
    {
        /// <summary>
        /// 请求参数
        /// </summary>
        public string CallbackrRequest { get; set; }

        public ErpStockInOrderConfirmCallbackRequestDomain CallbackrRequest_ { set { this.CallbackrRequest = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.scm.external.wms.return.order.callback";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("callbackr_request", this.CallbackrRequest);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
        }

	/// <summary>
/// ItemsDomain Data Structure.
/// </summary>
[Serializable]

public class ItemsDomain : TopObject
{
	        /// <summary>
	        /// 批次号
	        /// </summary>
	        [XmlElement("batch_code")]
	        public string BatchCode { get; set; }
	
	        /// <summary>
	        /// 失效日期
	        /// </summary>
	        [XmlElement("due_date")]
	        public Nullable<DateTime> DueDate { get; set; }
	
	        /// <summary>
	        /// 保质期
	        /// </summary>
	        [XmlElement("guarantee_period")]
	        public Nullable<long> GuaranteePeriod { get; set; }
	
	        /// <summary>
	        /// 保质期单元
	        /// </summary>
	        [XmlElement("guarantee_unit")]
	        public Nullable<long> GuaranteeUnit { get; set; }
	
	        /// <summary>
	        /// 库存类型：正品1；残次品101
	        /// </summary>
	        [XmlElement("inventory_type")]
	        public Nullable<long> InventoryType { get; set; }
	
	        /// <summary>
	        /// 生产地区
	        /// </summary>
	        [XmlElement("produce_area")]
	        public string ProduceArea { get; set; }
	
	        /// <summary>
	        /// 生产编码
	        /// </summary>
	        [XmlElement("produce_code")]
	        public string ProduceCode { get; set; }
	
	        /// <summary>
	        /// 生产日期
	        /// </summary>
	        [XmlElement("produce_date")]
	        public Nullable<DateTime> ProduceDate { get; set; }
	
	        /// <summary>
	        /// 数量
	        /// </summary>
	        [XmlElement("quantity")]
	        public Nullable<long> Quantity { get; set; }
}

	/// <summary>
/// OrderitemsDomain Data Structure.
/// </summary>
[Serializable]

public class OrderitemsDomain : TopObject
{
	        /// <summary>
	        /// 商家编码
	        /// </summary>
	        [XmlElement("item_code")]
	        public string ItemCode { get; set; }
	
	        /// <summary>
	        /// 商品ID
	        /// </summary>
	        [XmlElement("item_id")]
	        public string ItemId { get; set; }
	
	        /// <summary>
	        /// 库存信息列表
	        /// </summary>
	        [XmlArray("items")]
	        [XmlArrayItem("items")]
	        public List<ItemsDomain> Items { get; set; }
	
	        /// <summary>
	        /// 订单明细行号
	        /// </summary>
	        [XmlElement("order_item_id")]
	        public string OrderItemId { get; set; }
	
	        /// <summary>
	        /// 货主ID
	        /// </summary>
	        [XmlElement("own_user_id")]
	        public string OwnUserId { get; set; }
}

	/// <summary>
/// ErpStockInOrderConfirmCallbackRequestDomain Data Structure.
/// </summary>
[Serializable]

public class ErpStockInOrderConfirmCallbackRequestDomain : TopObject
{
	        /// <summary>
	        /// 业务类型
	        /// </summary>
	        [XmlElement("biz_type")]
	        public Nullable<long> BizType { get; set; }
	
	        /// <summary>
	        /// ERP订单编号
	        /// </summary>
	        [XmlElement("order_code")]
	        public string OrderCode { get; set; }
	
	        /// <summary>
	        /// 仓库订单完成时间
	        /// </summary>
	        [XmlElement("order_confirm_time")]
	        public Nullable<DateTime> OrderConfirmTime { get; set; }
	
	        /// <summary>
	        /// 订单商品信息列表
	        /// </summary>
	        [XmlArray("order_items")]
	        [XmlArrayItem("orderitems")]
	        public List<OrderitemsDomain> OrderItems { get; set; }
	
	        /// <summary>
	        /// 单据类型：退货单（501）
	        /// </summary>
	        [XmlElement("order_type")]
	        public Nullable<long> OrderType { get; set; }
	
	        /// <summary>
	        /// 货主ID
	        /// </summary>
	        [XmlElement("owner_user_id")]
	        public string OwnerUserId { get; set; }
	
	        /// <summary>
	        /// 卖家id
	        /// </summary>
	        [XmlElement("seller_id")]
	        public Nullable<long> SellerId { get; set; }
	
	        /// <summary>
	        /// 仓库编码
	        /// </summary>
	        [XmlElement("store_code")]
	        public string StoreCode { get; set; }
	
	        /// <summary>
	        /// 仓库订单编号
	        /// </summary>
	        [XmlElement("store_order_code")]
	        public string StoreOrderCode { get; set; }
}

        #endregion
    }
}
