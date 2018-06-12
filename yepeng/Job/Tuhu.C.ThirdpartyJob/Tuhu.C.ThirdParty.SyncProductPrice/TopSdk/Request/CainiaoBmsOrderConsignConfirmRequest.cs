using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: cainiao.bms.order.consign.confirm
    /// </summary>
    public class CainiaoBmsOrderConsignConfirmRequest : BaseTopRequest<Top.Api.Response.CainiaoBmsOrderConsignConfirmResponse>
    {
        /// <summary>
        /// 通知消息主体
        /// </summary>
        public string Content { get; set; }

        public BmsConsignOrderConfirmDomain Content_ { set { this.Content = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "cainiao.bms.order.consign.confirm";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("content", this.Content);
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
/// TmsItemDomain Data Structure.
/// </summary>
[Serializable]

public class TmsItemDomain : TopObject
{
	        /// <summary>
	        /// 前端商家编码
	        /// </summary>
	        [XmlElement("item_code")]
	        public string ItemCode { get; set; }
	
	        /// <summary>
	        /// 数量
	        /// </summary>
	        [XmlElement("item_quantity")]
	        public Nullable<long> ItemQuantity { get; set; }
	
	        /// <summary>
	        /// 货品ID
	        /// </summary>
	        [XmlElement("sc_item_id")]
	        public string ScItemId { get; set; }
}

	/// <summary>
/// PackageMaterialListDomain Data Structure.
/// </summary>
[Serializable]

public class PackageMaterialListDomain : TopObject
{
	        /// <summary>
	        /// 数量
	        /// </summary>
	        [XmlElement("material_quantity")]
	        public Nullable<long> MaterialQuantity { get; set; }
	
	        /// <summary>
	        /// 包材
	        /// </summary>
	        [XmlElement("material_type")]
	        public string MaterialType { get; set; }
}

	/// <summary>
/// TmsOrdersDomain Data Structure.
/// </summary>
[Serializable]

public class TmsOrdersDomain : TopObject
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
	        /// 包裹长度，单位：毫米
	        /// </summary>
	        [XmlElement("package_length")]
	        public Nullable<long> PackageLength { get; set; }
	
	        /// <summary>
	        /// 包裹的包材信息列表
	        /// </summary>
	        [XmlArray("package_material_list")]
	        [XmlArrayItem("package_material_list")]
	        public List<PackageMaterialListDomain> PackageMaterialList { get; set; }
	
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
	        /// 运单信息
	        /// </summary>
	        [XmlArray("tms_items")]
	        [XmlArrayItem("tms_item")]
	        public List<TmsItemDomain> TmsItems { get; set; }
	
	        /// <summary>
	        /// 运单编码，运单号
	        /// </summary>
	        [XmlElement("tms_order_code")]
	        public string TmsOrderCode { get; set; }
}

	/// <summary>
/// OrderItemsDomain Data Structure.
/// </summary>
[Serializable]

public class OrderItemsDomain : TopObject
{
	        /// <summary>
	        /// 商品金额 123.33元，单位：分
	        /// </summary>
	        [XmlElement("item_amount")]
	        public Nullable<long> ItemAmount { get; set; }
	
	        /// <summary>
	        /// 前端商家编码
	        /// </summary>
	        [XmlElement("item_code")]
	        public string ItemCode { get; set; }
	
	        /// <summary>
	        /// 前端宝贝itemId
	        /// </summary>
	        [XmlElement("item_id")]
	        public string ItemId { get; set; }
	
	        /// <summary>
	        /// 数量
	        /// </summary>
	        [XmlElement("item_quantity")]
	        public Nullable<long> ItemQuantity { get; set; }
	
	        /// <summary>
	        /// 默认：0；促销赠品1001
	        /// </summary>
	        [XmlElement("item_tag")]
	        public string ItemTag { get; set; }
	
	        /// <summary>
	        /// 明细ID
	        /// </summary>
	        [XmlElement("order_item_id")]
	        public Nullable<long> OrderItemId { get; set; }
	
	        /// <summary>
	        /// 后端商家编码
	        /// </summary>
	        [XmlElement("sc_item_code")]
	        public string ScItemCode { get; set; }
	
	        /// <summary>
	        /// 货品id
	        /// </summary>
	        [XmlElement("sc_item_id")]
	        public string ScItemId { get; set; }
	
	        /// <summary>
	        /// 前端skuId
	        /// </summary>
	        [XmlElement("sku_id")]
	        public string SkuId { get; set; }
	
	        /// <summary>
	        /// 明细对应主单的交易单号
	        /// </summary>
	        [XmlElement("trade_id")]
	        public string TradeId { get; set; }
	
	        /// <summary>
	        /// 明细对应的子交易单号
	        /// </summary>
	        [XmlElement("trade_item_id")]
	        public string TradeItemId { get; set; }
}

	/// <summary>
/// BmsConsignOrderConfirmDomain Data Structure.
/// </summary>
[Serializable]

public class BmsConsignOrderConfirmDomain : TopObject
{
	        /// <summary>
	        /// 每次发货均重新生成
	        /// </summary>
	        [XmlElement("consign_code")]
	        public string ConsignCode { get; set; }
	
	        /// <summary>
	        /// 仓库出库时间
	        /// </summary>
	        [XmlElement("consign_time")]
	        public string ConsignTime { get; set; }
	
	        /// <summary>
	        /// out_biz_id，（非导入时为订单创建时的交易号）
	        /// </summary>
	        [XmlElement("erp_order_code")]
	        public string ErpOrderCode { get; set; }
	
	        /// <summary>
	        /// 交易订单金额，以分为单位
	        /// </summary>
	        [XmlElement("order_amount")]
	        public Nullable<long> OrderAmount { get; set; }
	
	        /// <summary>
	        /// BMS订单编码
	        /// </summary>
	        [XmlElement("order_code")]
	        public string OrderCode { get; set; }
	
	        /// <summary>
	        /// 订单商品信息列表
	        /// </summary>
	        [XmlArray("order_items")]
	        [XmlArrayItem("order_items")]
	        public List<OrderItemsDomain> OrderItems { get; set; }
	
	        /// <summary>
	        /// 邮费，以分为单位
	        /// </summary>
	        [XmlElement("order_post_fee")]
	        public Nullable<long> OrderPostFee { get; set; }
	
	        /// <summary>
	        /// 0销售平台、1手工录入、2导入发货、3ERP推送
	        /// </summary>
	        [XmlElement("order_soruce")]
	        public Nullable<long> OrderSoruce { get; set; }
	
	        /// <summary>
	        /// 操作子类型(201 一般交易出库单,502 换货出库单,503 补发出库单)
	        /// </summary>
	        [XmlElement("order_type")]
	        public Nullable<long> OrderType { get; set; }
	
	        /// <summary>
	        /// 货主id
	        /// </summary>
	        [XmlElement("owner_user_id")]
	        public string OwnerUserId { get; set; }
	
	        /// <summary>
	        /// 店铺id，主店铺时跟货主id相同
	        /// </summary>
	        [XmlElement("shop_id")]
	        public string ShopId { get; set; }
	
	        /// <summary>
	        /// 发货仓的仓库编码
	        /// </summary>
	        [XmlElement("store_code")]
	        public string StoreCode { get; set; }
	
	        /// <summary>
	        /// 仓库作业单号，LBX号
	        /// </summary>
	        [XmlElement("store_order_code")]
	        public string StoreOrderCode { get; set; }
	
	        /// <summary>
	        /// 运单信息列表
	        /// </summary>
	        [XmlArray("tms_orders")]
	        [XmlArrayItem("tms_orders")]
	        public List<TmsOrdersDomain> TmsOrders { get; set; }
}

        #endregion
    }
}
