using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: taobao.inventory.merchant.adjust
    /// </summary>
    public class InventoryMerchantAdjustRequest : BaseTopRequest<Top.Api.Response.InventoryMerchantAdjustResponse>
    {
        /// <summary>
        /// 调整库存对象
        /// </summary>
        public string InventoryCheck { get; set; }

        public InventoryCheckDtoDomain InventoryCheck_ { set { this.InventoryCheck = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "taobao.inventory.merchant.adjust";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("inventory_check", this.InventoryCheck);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("inventory_check", this.InventoryCheck);
        }

	/// <summary>
/// InventoryCheckDetailDtoDomain Data Structure.
/// </summary>
[Serializable]

public class InventoryCheckDetailDtoDomain : TopObject
{
	        /// <summary>
	        /// 如果是门店类型,则为必填。 ONLINE_INVENTORY  线上可售库存，  SHARE_INVENTORY 线下可售库存
	        /// </summary>
	        [XmlElement("inv_biz_code")]
	        public string InvBizCode { get; set; }
	
	        /// <summary>
	        /// 调整数量，正数盘盈，负数盘亏
	        /// </summary>
	        [XmlElement("quantity")]
	        public Nullable<long> Quantity { get; set; }
	
	        /// <summary>
	        /// 后端货品id
	        /// </summary>
	        [XmlElement("sc_item_id")]
	        public Nullable<long> ScItemId { get; set; }
	
	        /// <summary>
	        /// 每个货品的调整子单据号，作为业务调整依据，处理时会幂等
	        /// </summary>
	        [XmlElement("sub_order_id")]
	        public string SubOrderId { get; set; }
}

	/// <summary>
/// InventoryCheckDtoDomain Data Structure.
/// </summary>
[Serializable]

public class InventoryCheckDtoDomain : TopObject
{
	        /// <summary>
	        /// 2: 出入库盘盈盘亏
	        /// </summary>
	        [XmlElement("check_mode")]
	        public Nullable<long> CheckMode { get; set; }
	
	        /// <summary>
	        /// 调整明细
	        /// </summary>
	        [XmlArray("detail_list")]
	        [XmlArrayItem("inventory_check_detail_dto")]
	        public List<InventoryCheckDetailDtoDomain> DetailList { get; set; }
	
	        /// <summary>
	        /// 2： 仓库类型   6：门店类型
	        /// </summary>
	        [XmlElement("inv_store_type")]
	        public Nullable<long> InvStoreType { get; set; }
	
	        /// <summary>
	        /// 调整单据号
	        /// </summary>
	        [XmlElement("order_id")]
	        public string OrderId { get; set; }
	
	        /// <summary>
	        /// 仓库code或者门店id
	        /// </summary>
	        [XmlElement("store_code")]
	        public string StoreCode { get; set; }
}

        #endregion
    }
}
