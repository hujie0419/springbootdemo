using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.wlb.waybill.i.fullupdate
    /// </summary>
    public class WlbWaybillIFullupdateRequest : BaseTopRequest<Top.Api.Response.WlbWaybillIFullupdateResponse>
    {
        /// <summary>
        /// 更新面单信息请求
        /// </summary>
        public string WaybillApplyFullUpdateRequest { get; set; }

        public WaybillApplyFullUpdateRequestDomain WaybillApplyFullUpdateRequest_ { set { this.WaybillApplyFullUpdateRequest = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.wlb.waybill.i.fullupdate";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("waybill_apply_full_update_request", this.WaybillApplyFullUpdateRequest);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("waybill_apply_full_update_request", this.WaybillApplyFullUpdateRequest);
        }

	/// <summary>
/// PackageItemDomain Data Structure.
/// </summary>
[Serializable]

public class PackageItemDomain : TopObject
{
	        /// <summary>
	        /// 商品数量
	        /// </summary>
	        [XmlElement("count")]
	        public Nullable<long> Count { get; set; }
	
	        /// <summary>
	        /// 商品名称
	        /// </summary>
	        [XmlElement("item_name")]
	        public string ItemName { get; set; }
}

	/// <summary>
/// LogisticsServiceDomain Data Structure.
/// </summary>
[Serializable]

public class LogisticsServiceDomain : TopObject
{
	        /// <summary>
	        /// 服务编码
	        /// </summary>
	        [XmlElement("service_code")]
	        public string ServiceCode { get; set; }
	
	        /// <summary>
	        /// 服务类型值，json格式表示
	        /// </summary>
	        [XmlElement("service_value4_json")]
	        public string ServiceValue4Json { get; set; }
}

	/// <summary>
/// WaybillAddressDomain Data Structure.
/// </summary>
[Serializable]

public class WaybillAddressDomain : TopObject
{
	        /// <summary>
	        /// 详细地址
	        /// </summary>
	        [XmlElement("address_detail")]
	        public string AddressDetail { get; set; }
	
	        /// <summary>
	        /// 区名称（三级地址）
	        /// </summary>
	        [XmlElement("area")]
	        public string Area { get; set; }
	
	        /// <summary>
	        /// 市名称（二级地址）
	        /// </summary>
	        [XmlElement("city")]
	        public string City { get; set; }
	
	        /// <summary>
	        /// 一级地址（省、直辖市
	        /// </summary>
	        [XmlElement("province")]
	        public string Province { get; set; }
	
	        /// <summary>
	        /// 街道\镇名称（四级地址）
	        /// </summary>
	        [XmlElement("town")]
	        public string Town { get; set; }
}

	/// <summary>
/// WaybillApplyFullUpdateRequestDomain Data Structure.
/// </summary>
[Serializable]

public class WaybillApplyFullUpdateRequestDomain : TopObject
{
	        /// <summary>
	        /// 收\发货地址
	        /// </summary>
	        [XmlElement("consignee_address")]
	        public WaybillAddressDomain ConsigneeAddress { get; set; }
	
	        /// <summary>
	        /// 收件人姓名
	        /// </summary>
	        [XmlElement("consignee_name")]
	        public string ConsigneeName { get; set; }
	
	        /// <summary>
	        /// 收件人电话
	        /// </summary>
	        [XmlElement("consignee_phone")]
	        public string ConsigneePhone { get; set; }
	
	        /// <summary>
	        /// 快递服务商CODE
	        /// </summary>
	        [XmlElement("cp_code")]
	        public string CpCode { get; set; }
	
	        /// <summary>
	        /// 物流服务能力集合
	        /// </summary>
	        [XmlArray("logistics_service_list")]
	        [XmlArrayItem("logistics_service")]
	        public List<LogisticsServiceDomain> LogisticsServiceList { get; set; }
	
	        /// <summary>
	        /// 订单渠道类型
	        /// </summary>
	        [XmlElement("order_channels_type")]
	        public string OrderChannelsType { get; set; }
	
	        /// <summary>
	        /// ERP 订单号或包裹号
	        /// </summary>
	        [XmlElement("package_id")]
	        public string PackageId { get; set; }
	
	        /// <summary>
	        /// 包裹里面的商品名称
	        /// </summary>
	        [XmlArray("package_items")]
	        [XmlArrayItem("package_item")]
	        public List<PackageItemDomain> PackageItems { get; set; }
	
	        /// <summary>
	        /// 快递服务产品类型编码
	        /// </summary>
	        [XmlElement("product_type")]
	        public string ProductType { get; set; }
	
	        /// <summary>
	        /// 使用者ID
	        /// </summary>
	        [XmlElement("real_user_id")]
	        public Nullable<long> RealUserId { get; set; }
	
	        /// <summary>
	        /// 发件人姓名
	        /// </summary>
	        [XmlElement("send_name")]
	        public string SendName { get; set; }
	
	        /// <summary>
	        /// 发件人联系方式
	        /// </summary>
	        [XmlElement("send_phone")]
	        public string SendPhone { get; set; }
	
	        /// <summary>
	        /// 交易订单号（组合表示合并订单）
	        /// </summary>
	        [XmlArray("trade_order_list")]
	        [XmlArrayItem("string")]
	        public List<string> TradeOrderList { get; set; }
	
	        /// <summary>
	        /// 包裹体积 单位为ML(毫升)或立方厘米
	        /// </summary>
	        [XmlElement("volume")]
	        public Nullable<long> Volume { get; set; }
	
	        /// <summary>
	        /// 电子面单单号
	        /// </summary>
	        [XmlElement("waybill_code")]
	        public string WaybillCode { get; set; }
	
	        /// <summary>
	        /// 包裹重量 单位为G(克)
	        /// </summary>
	        [XmlElement("weight")]
	        public Nullable<long> Weight { get; set; }
}

        #endregion
    }
}
