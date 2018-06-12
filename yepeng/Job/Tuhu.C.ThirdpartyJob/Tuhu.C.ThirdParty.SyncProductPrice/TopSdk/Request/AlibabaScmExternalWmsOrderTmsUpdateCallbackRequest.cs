using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.scm.external.wms.order.tms.update.callback
    /// </summary>
    public class AlibabaScmExternalWmsOrderTmsUpdateCallbackRequest : BaseTopRequest<Top.Api.Response.AlibabaScmExternalWmsOrderTmsUpdateCallbackResponse>
    {
        /// <summary>
        /// 回告请求对象
        /// </summary>
        public string CallbackRequest { get; set; }

        public ErpTmsUpdateCallbackRequestDomain CallbackRequest_ { set { this.CallbackRequest = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.scm.external.wms.order.tms.update.callback";
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
	        /// 包裹状态 TMS_ACCEPT:揽收;TMS_SIGN:签收;TMS_FAILED:拒签
	        /// </summary>
	        [XmlElement("package_status")]
	        public string PackageStatus { get; set; }
	
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
	        /// 备注；拒签原因
	        /// </summary>
	        [XmlElement("remark")]
	        public string Remark { get; set; }
	
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
	
	        /// <summary>
	        /// 更新时间
	        /// </summary>
	        [XmlElement("update_time")]
	        public Nullable<DateTime> UpdateTime { get; set; }
}

	/// <summary>
/// ErpTmsUpdateCallbackRequestDomain Data Structure.
/// </summary>
[Serializable]

public class ErpTmsUpdateCallbackRequestDomain : TopObject
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
	        /// 配送员，只有揽收状态，需要进行回传
	        /// </summary>
	        [XmlElement("courier")]
	        public string Courier { get; set; }
	
	        /// <summary>
	        /// 配送员电话，只有揽收状态，需要进行回传
	        /// </summary>
	        [XmlElement("courier_phone")]
	        public string CourierPhone { get; set; }
	
	        /// <summary>
	        /// 阿里Scm 发货单信息，Scm通过此字段关联发货单数据
	        /// </summary>
	        [XmlElement("order_code")]
	        public string OrderCode { get; set; }
	
	        /// <summary>
	        /// 外部业务编码
	        /// </summary>
	        [XmlElement("out_biz_code")]
	        public string OutBizCode { get; set; }
	
	        /// <summary>
	        /// 阿里交易订单编号，通知业务系统时，需要使用。
	        /// </summary>
	        [XmlElement("outer_biz_order_id")]
	        public string OuterBizOrderId { get; set; }
	
	        /// <summary>
	        /// 卖家编号：系统交互使用，Scm 内部区分来源
	        /// </summary>
	        [XmlElement("seller_id")]
	        public Nullable<long> SellerId { get; set; }
	
	        /// <summary>
	        /// 状态.TMS_ACCEPT:揽收;TMS_SIGN:签收;TMS_PART_SIGN:部分签收;TMS_FAILED:拒签
	        /// </summary>
	        [XmlElement("status")]
	        public string Status { get; set; }
	
	        /// <summary>
	        /// 仓库子订单编码 如果没有父子区分 storOrderCode和cnOrderCode 保持一致即可
	        /// </summary>
	        [XmlElement("store_order_code")]
	        public string StoreOrderCode { get; set; }
	
	        /// <summary>
	        /// 包裹列表
	        /// </summary>
	        [XmlArray("tms_orders")]
	        [XmlArrayItem("tmsorders")]
	        public List<TmsordersDomain> TmsOrders { get; set; }
}

        #endregion
    }
}
