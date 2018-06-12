using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.scm.external.wms.return.order.notify
    /// </summary>
    public class AlibabaScmExternalWmsReturnOrderNotifyRequest : BaseTopRequest<Top.Api.Response.AlibabaScmExternalWmsReturnOrderNotifyResponse>
    {
        /// <summary>
        /// 请求对象
        /// </summary>
        public string Request { get; set; }

        public StructDomain Request_ { set { this.Request = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.scm.external.wms.return.order.notify";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("request", this.Request);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("request", this.Request);
        }

	/// <summary>
/// SenderInfoDomain Data Structure.
/// </summary>
[Serializable]

public class SenderInfoDomain : TopObject
{
	        /// <summary>
	        /// 发件方地址
	        /// </summary>
	        [XmlElement("sender_address")]
	        public string SenderAddress { get; set; }
	
	        /// <summary>
	        /// 发件方区县
	        /// </summary>
	        [XmlElement("sender_area")]
	        public string SenderArea { get; set; }
	
	        /// <summary>
	        /// 发件方城市
	        /// </summary>
	        [XmlElement("sender_city")]
	        public string SenderCity { get; set; }
	
	        /// <summary>
	        /// 发件方手机
	        /// </summary>
	        [XmlElement("sender_mobile")]
	        public string SenderMobile { get; set; }
	
	        /// <summary>
	        /// 发件方名称
	        /// </summary>
	        [XmlElement("sender_name")]
	        public string SenderName { get; set; }
	
	        /// <summary>
	        /// 发件方电话
	        /// </summary>
	        [XmlElement("sender_phone")]
	        public string SenderPhone { get; set; }
	
	        /// <summary>
	        /// 发件方省份
	        /// </summary>
	        [XmlElement("sender_province")]
	        public string SenderProvince { get; set; }
	
	        /// <summary>
	        /// 发件方镇村
	        /// </summary>
	        [XmlElement("sender_town")]
	        public string SenderTown { get; set; }
	
	        /// <summary>
	        /// 发件方邮编
	        /// </summary>
	        [XmlElement("sender_zip_code")]
	        public string SenderZipCode { get; set; }
}

	/// <summary>
/// ReceiverInfoDomain Data Structure.
/// </summary>
[Serializable]

public class ReceiverInfoDomain : TopObject
{
	        /// <summary>
	        /// 收件方镇
	        /// </summary>
	        [XmlElement("receive_town")]
	        public string ReceiveTown { get; set; }
	
	        /// <summary>
	        /// 收件方地址
	        /// </summary>
	        [XmlElement("receiver_address")]
	        public string ReceiverAddress { get; set; }
	
	        /// <summary>
	        /// 收件方区县
	        /// </summary>
	        [XmlElement("receiver_area")]
	        public string ReceiverArea { get; set; }
	
	        /// <summary>
	        /// 收件方城市
	        /// </summary>
	        [XmlElement("receiver_city")]
	        public string ReceiverCity { get; set; }
	
	        /// <summary>
	        /// 收件人手机
	        /// </summary>
	        [XmlElement("receiver_mobile")]
	        public string ReceiverMobile { get; set; }
	
	        /// <summary>
	        /// 收件人名称
	        /// </summary>
	        [XmlElement("receiver_name")]
	        public string ReceiverName { get; set; }
	
	        /// <summary>
	        /// 收件人昵称
	        /// </summary>
	        [XmlElement("receiver_nick")]
	        public string ReceiverNick { get; set; }
	
	        /// <summary>
	        /// 收件人电话
	        /// </summary>
	        [XmlElement("receiver_phone")]
	        public string ReceiverPhone { get; set; }
	
	        /// <summary>
	        /// 收件方省份
	        /// </summary>
	        [XmlElement("receiver_province")]
	        public string ReceiverProvince { get; set; }
	
	        /// <summary>
	        /// 收件方邮编
	        /// </summary>
	        [XmlElement("receiver_zip_code")]
	        public string ReceiverZipCode { get; set; }
}

	/// <summary>
/// OrderItemDomain Data Structure.
/// </summary>
[Serializable]

public class OrderItemDomain : TopObject
{
	        /// <summary>
	        /// 扩展属性
	        /// </summary>
	        [XmlElement("extend_fields")]
	        public string ExtendFields { get; set; }
	
	        /// <summary>
	        /// 商品ID
	        /// </summary>
	        [XmlElement("item_id")]
	        public string ItemId { get; set; }
	
	        /// <summary>
	        /// 商品名称
	        /// </summary>
	        [XmlElement("item_name")]
	        public string ItemName { get; set; }
	
	        /// <summary>
	        /// 商品数量
	        /// </summary>
	        [XmlElement("item_quantity")]
	        public Nullable<long> ItemQuantity { get; set; }
	
	        /// <summary>
	        /// 退货单明细ID
	        /// </summary>
	        [XmlElement("order_item_id")]
	        public string OrderItemId { get; set; }
	
	        /// <summary>
	        /// 交易编码
	        /// </summary>
	        [XmlElement("order_source_code")]
	        public string OrderSourceCode { get; set; }
	
	        /// <summary>
	        /// 子交易编码
	        /// </summary>
	        [XmlElement("sub_source_code")]
	        public string SubSourceCode { get; set; }
}

	/// <summary>
/// StructDomain Data Structure.
/// </summary>
[Serializable]

public class StructDomain : TopObject
{
	        /// <summary>
	        /// 业务类型
	        /// </summary>
	        [XmlElement("biz_type")]
	        public Nullable<long> BizType { get; set; }
	
	        /// <summary>
	        /// 买家昵称
	        /// </summary>
	        [XmlElement("buyer_nick")]
	        public string BuyerNick { get; set; }
	
	        /// <summary>
	        /// 拓展属性：换货单编号exchangeCode
	        /// </summary>
	        [XmlElement("extend_fields")]
	        public string ExtendFields { get; set; }
	
	        /// <summary>
	        /// ERP单据编码
	        /// </summary>
	        [XmlElement("order_code")]
	        public string OrderCode { get; set; }
	
	        /// <summary>
	        /// 订单创建时间
	        /// </summary>
	        [XmlElement("order_create_time")]
	        public string OrderCreateTime { get; set; }
	
	        /// <summary>
	        /// 订单商品列表
	        /// </summary>
	        [XmlArray("order_item_list")]
	        [XmlArrayItem("order_item")]
	        public List<OrderItemDomain> OrderItemList { get; set; }
	
	        /// <summary>
	        /// 订单来源
	        /// </summary>
	        [XmlElement("order_source")]
	        public Nullable<long> OrderSource { get; set; }
	
	        /// <summary>
	        /// 订单类型：退货单（501）
	        /// </summary>
	        [XmlElement("order_type")]
	        public Nullable<long> OrderType { get; set; }
	
	        /// <summary>
	        /// 货主id
	        /// </summary>
	        [XmlElement("owner_user_id")]
	        public string OwnerUserId { get; set; }
	
	        /// <summary>
	        /// 来源单据号
	        /// </summary>
	        [XmlElement("prev_order_code")]
	        public string PrevOrderCode { get; set; }
	
	        /// <summary>
	        /// 收货方信息
	        /// </summary>
	        [XmlElement("receiver_info")]
	        public ReceiverInfoDomain ReceiverInfo { get; set; }
	
	        /// <summary>
	        /// 退货类型
	        /// </summary>
	        [XmlElement("refund_type")]
	        public string RefundType { get; set; }
	
	        /// <summary>
	        /// 备注
	        /// </summary>
	        [XmlElement("remark")]
	        public string Remark { get; set; }
	
	        /// <summary>
	        /// 退货原因
	        /// </summary>
	        [XmlElement("return_reason")]
	        public string ReturnReason { get; set; }
	
	        /// <summary>
	        /// 发货方信息
	        /// </summary>
	        [XmlElement("sender_info")]
	        public SenderInfoDomain SenderInfo { get; set; }
	
	        /// <summary>
	        /// 仓库code
	        /// </summary>
	        [XmlElement("store_code")]
	        public string StoreCode { get; set; }
	
	        /// <summary>
	        /// 运单号
	        /// </summary>
	        [XmlElement("tms_order_code")]
	        public string TmsOrderCode { get; set; }
	
	        /// <summary>
	        /// 配送编码
	        /// </summary>
	        [XmlElement("tms_service_code")]
	        public string TmsServiceCode { get; set; }
	
	        /// <summary>
	        /// 配送公司名称
	        /// </summary>
	        [XmlElement("tms_service_name")]
	        public string TmsServiceName { get; set; }
	
	        /// <summary>
	        /// 店铺id
	        /// </summary>
	        [XmlElement("user_id")]
	        public string UserId { get; set; }
}

        #endregion
    }
}
