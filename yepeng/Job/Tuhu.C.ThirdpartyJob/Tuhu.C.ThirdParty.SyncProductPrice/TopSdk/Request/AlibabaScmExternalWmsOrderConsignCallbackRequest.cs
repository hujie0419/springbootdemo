using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.scm.external.wms.order.consign.callback
    /// </summary>
    public class AlibabaScmExternalWmsOrderConsignCallbackRequest : BaseTopRequest<Top.Api.Response.AlibabaScmExternalWmsOrderConsignCallbackResponse>
    {
        /// <summary>
        /// 请求对象
        /// </summary>
        public string CallbackRequest { get; set; }

        public ErpConsignOrderConfirmCallbackRequestDomain CallbackRequest_ { set { this.CallbackRequest = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.scm.external.wms.order.consign.callback";
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
	        /// 到效日期
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
	        /// 库存类型：(1 正品,101 类型用来定义残次品,201 冻结类型库存,301 在途库存
	        /// </summary>
	        [XmlElement("inventory_type")]
	        public Nullable<long> InventoryType { get; set; }
	
	        /// <summary>
	        /// 包裹号
	        /// </summary>
	        [XmlElement("package_code")]
	        public string PackageCode { get; set; }
	
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
	        /// 生产日期
	        /// </summary>
	        [XmlElement("produce_date")]
	        public Nullable<DateTime> ProduceDate { get; set; }
	
	        /// <summary>
	        /// 数量
	        /// </summary>
	        [XmlElement("quantity")]
	        public Nullable<long> Quantity { get; set; }
	
	        /// <summary>
	        /// sn编码
	        /// </summary>
	        [XmlElement("sn_code")]
	        public string SnCode { get; set; }
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
	        /// 此明细是否发货完成，此字段暂未使用
	        /// </summary>
	        [XmlElement("is_completed")]
	        public Nullable<bool> IsCompleted { get; set; }
	
	        /// <summary>
	        /// 商家商品编码
	        /// </summary>
	        [XmlElement("item_code")]
	        public string ItemCode { get; set; }
	
	        /// <summary>
	        /// 后端商品编号，Scm 通过此字段关联商品数据
	        /// </summary>
	        [XmlElement("item_id")]
	        public string ItemId { get; set; }
	
	        /// <summary>
	        /// 商品属性列表
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
	        /// 订单明细编码行号 scm通过此字段关联子发货单记录
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
/// TmsitemsDomain Data Structure.
/// </summary>
[Serializable]

public class TmsitemsDomain : TopObject
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
	        /// 商家编码
	        /// </summary>
	        [XmlElement("item_code")]
	        public string ItemCode { get; set; }
	
	        /// <summary>
	        /// 后端商品编号，Scm 通过此字段关联商品数据
	        /// </summary>
	        [XmlElement("item_id")]
	        public string ItemId { get; set; }
	
	        /// <summary>
	        /// 此运单里该商品的数量
	        /// </summary>
	        [XmlElement("item_quantity")]
	        public Nullable<long> ItemQuantity { get; set; }
	
	        /// <summary>
	        /// 订单明细编码行号 scm通过此字段关联子发货单记录
	        /// </summary>
	        [XmlElement("order_item_id")]
	        public string OrderItemId { get; set; }
	
	        /// <summary>
	        /// 生产批号
	        /// </summary>
	        [XmlElement("produce_code")]
	        public string ProduceCode { get; set; }
	
	        /// <summary>
	        /// 生产日期
	        /// </summary>
	        [XmlElement("product_date")]
	        public Nullable<DateTime> ProductDate { get; set; }
}

	/// <summary>
/// PackagemateriallistDomain Data Structure.
/// </summary>
[Serializable]

public class PackagemateriallistDomain : TopObject
{
	        /// <summary>
	        /// 包材的数量
	        /// </summary>
	        [XmlElement("material_quantity")]
	        public Nullable<long> MaterialQuantity { get; set; }
	
	        /// <summary>
	        /// 淘宝指定的包材型号
	        /// </summary>
	        [XmlElement("material_type")]
	        public string MaterialType { get; set; }
}

	/// <summary>
/// TmsordersDomain Data Structure.
/// </summary>
[Serializable]

public class TmsordersDomain : TopObject
{
	        /// <summary>
	        /// 包裹号 如果没有包裹号，此字段和tmsOrderCode 保持一致即可
	        /// </summary>
	        [XmlElement("package_code")]
	        public string PackageCode { get; set; }
	
	        /// <summary>
	        /// 包裹高度，单位：毫米
	        /// </summary>
	        [XmlElement("package_height")]
	        public Nullable<long> PackageHeight { get; set; }
	
	        /// <summary>
	        /// 包裹长度，单位：毫米
	        /// </summary>
	        [XmlElement("package_length")]
	        public Nullable<long> PackageLength { get; set; }
	
	        /// <summary>
	        /// 包裹的包材信息列表
	        /// </summary>
	        [XmlArray("package_material_list")]
	        [XmlArrayItem("packagemateriallist")]
	        public List<PackagemateriallistDomain> PackageMaterialList { get; set; }
	
	        /// <summary>
	        /// 包裹重量，单位：克
	        /// </summary>
	        [XmlElement("package_weight")]
	        public Nullable<long> PackageWeight { get; set; }
	
	        /// <summary>
	        /// 包裹宽度，单位：毫米
	        /// </summary>
	        [XmlElement("package_width")]
	        public Nullable<long> PackageWidth { get; set; }
	
	        /// <summary>
	        /// 快递公司服务编码
	        /// </summary>
	        [XmlElement("tms_code")]
	        public string TmsCode { get; set; }
	
	        /// <summary>
	        /// 包裹里面的商品信息列表
	        /// </summary>
	        [XmlArray("tms_items")]
	        [XmlArrayItem("tmsitems")]
	        public List<TmsitemsDomain> TmsItems { get; set; }
	
	        /// <summary>
	        /// 运单编码
	        /// </summary>
	        [XmlElement("tms_order_code")]
	        public string TmsOrderCode { get; set; }
}

	/// <summary>
/// InvoinceconfirmsDomain Data Structure.
/// </summary>
[Serializable]

public class InvoinceconfirmsDomain : TopObject
{
	        /// <summary>
	        /// Erp发票ID
	        /// </summary>
	        [XmlElement("bill_id")]
	        public Nullable<long> BillId { get; set; }
	
	        /// <summary>
	        /// 发票代码
	        /// </summary>
	        [XmlElement("invoice_code")]
	        public string InvoiceCode { get; set; }
	
	        /// <summary>
	        /// 发票号
	        /// </summary>
	        [XmlElement("invoice_number")]
	        public string InvoiceNumber { get; set; }
	
	        /// <summary>
	        /// 无效字段
	        /// </summary>
	        [XmlElement("trade_number")]
	        public string TradeNumber { get; set; }
}

	/// <summary>
/// ErpConsignOrderConfirmCallbackRequestDomain Data Structure.
/// </summary>
[Serializable]

public class ErpConsignOrderConfirmCallbackRequestDomain : TopObject
{
	        /// <summary>
	        /// 帐套类型：系统交互使用 Scm 内部区分来源
	        /// </summary>
	        [XmlElement("biz_type")]
	        public Nullable<long> BizType { get; set; }
	
	        /// <summary>
	        /// 仓库父订单编码
	        /// </summary>
	        [XmlElement("cn_order_code")]
	        public string CnOrderCode { get; set; }
	
	        /// <summary>
	        /// 确认类型 0 最后一次确认或是一次性确认,1 多次确认； 目前销售出库只支持一次性确认
	        /// </summary>
	        [XmlElement("confirm_type")]
	        public Nullable<long> ConfirmType { get; set; }
	
	        /// <summary>
	        /// 发票确认信息列表
	        /// </summary>
	        [XmlArray("invoince_confirms")]
	        [XmlArrayItem("invoinceconfirms")]
	        public List<InvoinceconfirmsDomain> InvoinceConfirms { get; set; }
	
	        /// <summary>
	        /// 阿里Scm 发货单信息，Scm通过此字段关联发货单数据
	        /// </summary>
	        [XmlElement("order_code")]
	        public string OrderCode { get; set; }
	
	        /// <summary>
	        /// 仓库订单完成时间
	        /// </summary>
	        [XmlElement("order_confirm_time")]
	        public Nullable<DateTime> OrderConfirmTime { get; set; }
	
	        /// <summary>
	        /// 订单商品信息列表,服务商品不要回告
	        /// </summary>
	        [XmlArray("order_items")]
	        [XmlArrayItem("orderitems")]
	        public List<OrderitemsDomain> OrderItems { get; set; }
	
	        /// <summary>
	        /// 操作子类型(201 一般交易出库单,502 换货出库单,503 补发出库单)
	        /// </summary>
	        [XmlElement("order_type")]
	        public Nullable<long> OrderType { get; set; }
	
	        /// <summary>
	        /// 阿里交易订单编号，通知业务系统时，需要使用
	        /// </summary>
	        [XmlElement("outer_biz_order_id")]
	        public string OuterBizOrderId { get; set; }
	
	        /// <summary>
	        /// 货主编码
	        /// </summary>
	        [XmlElement("owner_user_id")]
	        public string OwnerUserId { get; set; }
	
	        /// <summary>
	        /// 卖家编号：系统交互使用，Scm 内部区分来源
	        /// </summary>
	        [XmlElement("seller_id")]
	        public Nullable<long> SellerId { get; set; }
	
	        /// <summary>
	        /// 仓库编码
	        /// </summary>
	        [XmlElement("store_code")]
	        public string StoreCode { get; set; }
	
	        /// <summary>
	        /// 仓库子订单编码 如果没有父子区分 storOrderCode和cnOrderCode 保持一致即可
	        /// </summary>
	        [XmlElement("store_order_code")]
	        public string StoreOrderCode { get; set; }
	
	        /// <summary>
	        /// 运单信息列表 服务商品不要回告
	        /// </summary>
	        [XmlArray("tms_orders")]
	        [XmlArrayItem("tmsorders")]
	        public List<TmsordersDomain> TmsOrders { get; set; }
}

        #endregion
    }
}
