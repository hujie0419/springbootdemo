using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.scm.external.wms.exchange.order.callback
    /// </summary>
    public class AlibabaScmExternalWmsExchangeOrderCallbackRequest : BaseTopRequest<Top.Api.Response.AlibabaScmExternalWmsExchangeOrderCallbackResponse>
    {
        /// <summary>
        /// 请求参数
        /// </summary>
        public string CallbackRequest { get; set; }

        public ErpRefundExchangeCallbackRequestDomain CallbackRequest_ { set { this.CallbackRequest = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.scm.external.wms.exchange.order.callback";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("callback_request", this.CallbackRequest);
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
/// ExchangeitemsDomain Data Structure.
/// </summary>
[Serializable]

public class ExchangeitemsDomain : TopObject
{
	        /// <summary>
	        /// 商品ID
	        /// </summary>
	        [XmlElement("item_id")]
	        public Nullable<long> ItemId { get; set; }
	
	        /// <summary>
	        /// 商品数量
	        /// </summary>
	        [XmlElement("item_quantity")]
	        public Nullable<long> ItemQuantity { get; set; }
}

	/// <summary>
/// OrderitemandexchangeitemsesDomain Data Structure.
/// </summary>
[Serializable]

public class OrderitemandexchangeitemsesDomain : TopObject
{
	        /// <summary>
	        /// 换出商品集合
	        /// </summary>
	        [XmlArray("exchange_items")]
	        [XmlArrayItem("exchangeitems")]
	        public List<ExchangeitemsDomain> ExchangeItems { get; set; }
	
	        /// <summary>
	        /// 子发货单id
	        /// </summary>
	        [XmlElement("order_item_id")]
	        public Nullable<long> OrderItemId { get; set; }
}

	/// <summary>
/// ErpRefundExchangeCallbackRequestDomain Data Structure.
/// </summary>
[Serializable]

public class ErpRefundExchangeCallbackRequestDomain : TopObject
{
	        /// <summary>
	        /// 业务类型
	        /// </summary>
	        [XmlElement("biz_type")]
	        public Nullable<long> BizType { get; set; }
	
	        /// <summary>
	        /// 创建人
	        /// </summary>
	        [XmlElement("creater")]
	        public string Creater { get; set; }
	
	        /// <summary>
	        /// 拓展属性
	        /// </summary>
	        [XmlElement("extend_fields")]
	        public string ExtendFields { get; set; }
	
	        /// <summary>
	        /// 发货单编码
	        /// </summary>
	        [XmlElement("order_code")]
	        public string OrderCode { get; set; }
	
	        /// <summary>
	        /// 换货商品集合
	        /// </summary>
	        [XmlArray("order_item_and_exchange_itemses")]
	        [XmlArrayItem("orderitemandexchangeitemses")]
	        public List<OrderitemandexchangeitemsesDomain> OrderItemAndExchangeItemses { get; set; }
	
	        /// <summary>
	        /// 交易编码
	        /// </summary>
	        [XmlElement("order_source_code")]
	        public string OrderSourceCode { get; set; }
	
	        /// <summary>
	        /// 订单类型：换货单（1101）
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
	        /// 仓库换货单号
	        /// </summary>
	        [XmlElement("store_order_code")]
	        public string StoreOrderCode { get; set; }
}

        #endregion
    }
}
