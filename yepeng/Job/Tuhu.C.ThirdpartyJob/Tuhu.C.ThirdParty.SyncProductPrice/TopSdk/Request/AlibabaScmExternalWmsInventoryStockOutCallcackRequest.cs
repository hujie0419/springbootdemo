using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.scm.external.wms.inventory.stock.out.callcack
    /// </summary>
    public class AlibabaScmExternalWmsInventoryStockOutCallcackRequest : BaseTopRequest<Top.Api.Response.AlibabaScmExternalWmsInventoryStockOutCallcackResponse>
    {
        /// <summary>
        /// 出库回传报文体
        /// </summary>
        public string CallbackRequest { get; set; }

        public ErpStockOutOrderConfirmCallbackRequestDomain CallbackRequest_ { set { this.CallbackRequest = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.scm.external.wms.inventory.stock.out.callcack";
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
/// CheckitemsDomain Data Structure.
/// </summary>
[Serializable]

public class CheckitemsDomain : TopObject
{
	        /// <summary>
	        /// ERP出库单明细id
	        /// </summary>
	        [XmlElement("order_item_id")]
	        public Nullable<long> OrderItemId { get; set; }
	
	        /// <summary>
	        /// 货主id111
	        /// </summary>
	        [XmlElement("own_user_i_d")]
	        public Nullable<long> OwnUserID { get; set; }
	
	        /// <summary>
	        /// 数量
	        /// </summary>
	        [XmlElement("quantity")]
	        public Nullable<long> Quantity { get; set; }
}

	/// <summary>
/// PackageitemitemsDomain Data Structure.
/// </summary>
[Serializable]

public class PackageitemitemsDomain : TopObject
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
	        /// 库存类型1 可销售库存 101 类型用来定义残次品 201  冻结类型库存301 在途库存
	        /// </summary>
	        [XmlElement("inventory_type")]
	        public Nullable<long> InventoryType { get; set; }
	
	        /// <summary>
	        /// ERP商品id
	        /// </summary>
	        [XmlElement("item_id")]
	        public string ItemId { get; set; }
	
	        /// <summary>
	        /// 此包裹里该商品的数量
	        /// </summary>
	        [XmlElement("item_quantity")]
	        public Nullable<long> ItemQuantity { get; set; }
	
	        /// <summary>
	        /// erp出库单明细id
	        /// </summary>
	        [XmlElement("order_item_id")]
	        public string OrderItemId { get; set; }
	
	        /// <summary>
	        /// 生产批号
	        /// </summary>
	        [XmlElement("produce_code")]
	        public string ProduceCode { get; set; }
	
	        /// <summary>
	        /// 生产日期，格式为 yyyy-MM-dd HH:mm:ss
	        /// </summary>
	        [XmlElement("product_date")]
	        public Nullable<DateTime> ProductDate { get; set; }
}

	/// <summary>
/// PackageinfosDomain Data Structure.
/// </summary>
[Serializable]

public class PackageinfosDomain : TopObject
{
	        /// <summary>
	        /// 包裹号
	        /// </summary>
	        [XmlElement("package_code")]
	        public string PackageCode { get; set; }
	
	        /// <summary>
	        /// 包裹高度，单位：毫米
	        /// </summary>
	        [XmlElement("package_height")]
	        public Nullable<long> PackageHeight { get; set; }
	
	        /// <summary>
	        /// 包裹里面的商品信息列表
	        /// </summary>
	        [XmlArray("package_item_items")]
	        [XmlArrayItem("packageitemitems")]
	        public List<PackageitemitemsDomain> PackageItemItems { get; set; }
	
	        /// <summary>
	        /// 包裹长度，单位：毫米
	        /// </summary>
	        [XmlElement("package_length")]
	        public Nullable<long> PackageLength { get; set; }
	
	        /// <summary>
	        /// 包裹质量，单位：克
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
	        /// 运单编码
	        /// </summary>
	        [XmlElement("tms_order_code")]
	        public string TmsOrderCode { get; set; }
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
	        /// 保质期单位
	        /// </summary>
	        [XmlElement("guarantee_unit")]
	        public Nullable<long> GuaranteeUnit { get; set; }
	
	        /// <summary>
	        /// 库存类型1 可销售库存 101 类型用来定义残次品 201  冻结类型库存301 在途库存
	        /// </summary>
	        [XmlElement("inventory_type")]
	        public Nullable<long> InventoryType { get; set; }
	
	        /// <summary>
	        /// 生产地区
	        /// </summary>
	        [XmlElement("produce_area")]
	        public string ProduceArea { get; set; }
	
	        /// <summary>
	        /// 生产批号
	        /// </summary>
	        [XmlElement("produce_code")]
	        public string ProduceCode { get; set; }
	
	        /// <summary>
	        /// ，格式为 yyyy-MM-dd HH:mm:ss
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
	        /// 商品ID
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
	        /// erp出库单明细id
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
/// ErpStockOutOrderConfirmCallbackRequestDomain Data Structure.
/// </summary>
[Serializable]

public class ErpStockOutOrderConfirmCallbackRequestDomain : TopObject
{
	        /// <summary>
	        /// 业务类型
	        /// </summary>
	        [XmlElement("biz_type")]
	        public Nullable<long> BizType { get; set; }
	
	        /// <summary>
	        /// 承运商名称
	        /// </summary>
	        [XmlElement("carriers_name")]
	        public string CarriersName { get; set; }
	
	        /// <summary>
	        /// 订单商品校验信息列表，最后一次回传不能为空
	        /// </summary>
	        [XmlArray("check_items")]
	        [XmlArrayItem("checkitems")]
	        public List<CheckitemsDomain> CheckItems { get; set; }
	
	        /// <summary>
	        /// 支持出入库单多次确认 0 最后一次确认或是一次性确认；1 多次确认；当多次确认时，确认的ITEM种类全部被确认时，确认完成默认值为 0 例如输入2认为是0
	        /// </summary>
	        [XmlElement("confirm_type")]
	        public Nullable<long> ConfirmType { get; set; }
	
	        /// <summary>
	        /// 拓展属性
	        /// </summary>
	        [XmlElement("extend_attribute")]
	        public string ExtendAttribute { get; set; }
	
	        /// <summary>
	        /// ERP出库单号
	        /// </summary>
	        [XmlElement("order_code")]
	        public string OrderCode { get; set; }
	
	        /// <summary>
	        /// 仓库订单完成时间，格式为 yyyy-MM-dd HH:mm:ss
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
	        /// 退供出库单（901）、调拨出库单（301）
	        /// </summary>
	        [XmlElement("order_type")]
	        public Nullable<long> OrderType { get; set; }
	
	        /// <summary>
	        /// 外部回传幂等key，每单每次回传必须不同，同一次回传重试必须保证与第一次回传相同
	        /// </summary>
	        [XmlElement("out_biz_code")]
	        public string OutBizCode { get; set; }
	
	        /// <summary>
	        /// 快递单列表
	        /// </summary>
	        [XmlArray("package_infos")]
	        [XmlArrayItem("packageinfos")]
	        public List<PackageinfosDomain> PackageInfos { get; set; }
	
	        /// <summary>
	        /// 取件人电话
	        /// </summary>
	        [XmlElement("pick_call")]
	        public string PickCall { get; set; }
	
	        /// <summary>
	        /// 取件人证件号
	        /// </summary>
	        [XmlElement("pick_id")]
	        public string PickId { get; set; }
	
	        /// <summary>
	        /// 取件人姓名
	        /// </summary>
	        [XmlElement("pick_name")]
	        public string PickName { get; set; }
	
	        /// <summary>
	        /// 店铺id
	        /// </summary>
	        [XmlElement("seller_id")]
	        public Nullable<long> SellerId { get; set; }
	
	        /// <summary>
	        /// wms单据号
	        /// </summary>
	        [XmlElement("store_order_code")]
	        public string StoreOrderCode { get; set; }
}

        #endregion
    }
}
