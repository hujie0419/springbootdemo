using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.scm.external.wms.inventory.stock.in.calback
    /// </summary>
    public class AlibabaScmExternalWmsInventoryStockInCalbackRequest : BaseTopRequest<Top.Api.Response.AlibabaScmExternalWmsInventoryStockInCalbackResponse>
    {
        /// <summary>
        /// 入库回传报文体
        /// </summary>
        public string CallbackRequest { get; set; }

        public ErpStockInOrderConfirmCallbackRequestDomain CallbackRequest_ { set { this.CallbackRequest = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.scm.external.wms.inventory.stock.in.calback";
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
            RequestValidator.ValidateRequired("callback_request", this.CallbackRequest);
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
	        /// 失效日期，格式为 yyyy-MM-dd HH:mm:ss
	        /// </summary>
	        [XmlElement("due_date")]
	        public Nullable<DateTime> DueDate { get; set; }
	
	        /// <summary>
	        /// 保质期，默认以天为单位
	        /// </summary>
	        [XmlElement("guarantee_period")]
	        public Nullable<long> GuaranteePeriod { get; set; }
	
	        /// <summary>
	        /// 保质期单元
	        /// </summary>
	        [XmlElement("guarantee_unit")]
	        public Nullable<long> GuaranteeUnit { get; set; }
	
	        /// <summary>
	        /// 库存类型1 可销售库存  101 类型用来定义残次品  201  冻结类型库存 301 在途库存
	        /// </summary>
	        [XmlElement("inventory_type")]
	        public Nullable<long> InventoryType { get; set; }
	
	        /// <summary>
	        /// 生产地区
	        /// </summary>
	        [XmlElement("produce_area")]
	        public string ProduceArea { get; set; }
	
	        /// <summary>
	        /// 生产编码，同一商品可能因商家不同有不同编码
	        /// </summary>
	        [XmlElement("produce_code")]
	        public string ProduceCode { get; set; }
	
	        /// <summary>
	        /// 生产日期格式为 yyyy-MM-dd HH:mm:ss
	        /// </summary>
	        [XmlElement("produce_date")]
	        public Nullable<DateTime> ProduceDate { get; set; }
	
	        /// <summary>
	        /// 商品数量
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
	        /// 高 单位 mm
	        /// </summary>
	        [XmlElement("height")]
	        public Nullable<long> Height { get; set; }
	
	        /// <summary>
	        /// 是否完成
	        /// </summary>
	        [XmlElement("is_completed")]
	        public Nullable<bool> IsCompleted { get; set; }
	
	        /// <summary>
	        /// 商家编码
	        /// </summary>
	        [XmlElement("item_code")]
	        public string ItemCode { get; set; }
	
	        /// <summary>
	        /// 商品id
	        /// </summary>
	        [XmlElement("item_id")]
	        public string ItemId { get; set; }
	
	        /// <summary>
	        /// 商品信息列表
	        /// </summary>
	        [XmlArray("items")]
	        [XmlArrayItem("items")]
	        public List<ItemsDomain> Items { get; set; }
	
	        /// <summary>
	        /// 长 单位 mm
	        /// </summary>
	        [XmlElement("length")]
	        public Nullable<long> Length { get; set; }
	
	        /// <summary>
	        /// erp入库单明细id
	        /// </summary>
	        [XmlElement("order_item_id")]
	        public string OrderItemId { get; set; }
	
	        /// <summary>
	        /// 货主ID
	        /// </summary>
	        [XmlElement("own_user_id")]
	        public string OwnUserId { get; set; }
	
	        /// <summary>
	        /// 体积
	        /// </summary>
	        [XmlElement("volume")]
	        public Nullable<long> Volume { get; set; }
	
	        /// <summary>
	        /// sku重量 单位克
	        /// </summary>
	        [XmlElement("weight")]
	        public Nullable<long> Weight { get; set; }
	
	        /// <summary>
	        /// 宽 单位 mm
	        /// </summary>
	        [XmlElement("width")]
	        public Nullable<long> Width { get; set; }
}

	/// <summary>
/// CheckitemsDomain Data Structure.
/// </summary>
[Serializable]

public class CheckitemsDomain : TopObject
{
	        /// <summary>
	        /// ERP入库单明细id
	        /// </summary>
	        [XmlElement("order_item_id")]
	        public Nullable<long> OrderItemId { get; set; }
	
	        /// <summary>
	        /// 货主id
	        /// </summary>
	        [XmlElement("own_user_i_d")]
	        public Nullable<long> OwnUserID { get; set; }
	
	        /// <summary>
	        /// quantity
	        /// </summary>
	        [XmlElement("quantity")]
	        public Nullable<long> Quantity { get; set; }
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
	        /// 订单商品校验信息列表，标记入库回传已经完成confirmType为0时，必须回传
	        /// </summary>
	        [XmlArray("check_items")]
	        [XmlArrayItem("checkitems")]
	        public List<CheckitemsDomain> CheckItems { get; set; }
	
	        /// <summary>
	        /// 菜鸟订单编号
	        /// </summary>
	        [XmlElement("cn_order_code")]
	        public string CnOrderCode { get; set; }
	
	        /// <summary>
	        /// 支持出入库单多次确认 0 最后一次确认或是一次性确认；1 多次确认
	        /// </summary>
	        [XmlElement("confirm_type")]
	        public Nullable<long> ConfirmType { get; set; }
	
	        /// <summary>
	        /// 是否一次性回传，true会一次性回传，false为分配回传
	        /// </summary>
	        [XmlElement("is_disposable")]
	        public Nullable<bool> IsDisposable { get; set; }
	
	        /// <summary>
	        /// ERP订单编号
	        /// </summary>
	        [XmlElement("order_code")]
	        public string OrderCode { get; set; }
	
	        /// <summary>
	        /// 仓库订单完成时间,格式为 yyyy-MM-dd HH:mm:ss
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
	        /// 采购入库单（601）、调拨入库单（302）
	        /// </summary>
	        [XmlElement("order_type")]
	        public Nullable<long> OrderType { get; set; }
	
	        /// <summary>
	        /// 外部回传幂等key，每单每次回传必须不同，同一次回传重试必须保证与第一次回传相同
	        /// </summary>
	        [XmlElement("out_biz_code")]
	        public string OutBizCode { get; set; }
	
	        /// <summary>
	        /// 货主id
	        /// </summary>
	        [XmlElement("owner_user_id")]
	        public string OwnerUserId { get; set; }
	
	        /// <summary>
	        /// 店铺id
	        /// </summary>
	        [XmlElement("seller_id")]
	        public Nullable<long> SellerId { get; set; }
	
	        /// <summary>
	        /// 仓库编码
	        /// </summary>
	        [XmlElement("store_code")]
	        public string StoreCode { get; set; }
	
	        /// <summary>
	        /// wms入库单号
	        /// </summary>
	        [XmlElement("store_order_code")]
	        public string StoreOrderCode { get; set; }
}

        #endregion
    }
}
