using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.scm.external.wms.inventory.stock.check.callcack
    /// </summary>
    public class AlibabaScmExternalWmsInventoryStockCheckCallcackRequest : BaseTopRequest<Top.Api.Response.AlibabaScmExternalWmsInventoryStockCheckCallcackResponse>
    {
        /// <summary>
        /// 盘点回传报文体
        /// </summary>
        public string CallBackRequest { get; set; }

        public ErpInventoryCountCallBackRequestDomain CallBackRequest_ { set { this.CallBackRequest = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.scm.external.wms.inventory.stock.check.callcack";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("call_back_request", this.CallBackRequest);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("call_back_request", this.CallBackRequest);
        }

	/// <summary>
/// ItemlistDomain Data Structure.
/// </summary>
[Serializable]

public class ItemlistDomain : TopObject
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
	        /// 库存类型，1正品，101次品
	        /// </summary>
	        [XmlElement("inventory_type")]
	        public Nullable<long> InventoryType { get; set; }
	
	        /// <summary>
	        /// 商家对商品的编码
	        /// </summary>
	        [XmlElement("item_code")]
	        public string ItemCode { get; set; }
	
	        /// <summary>
	        /// 商品ID
	        /// </summary>
	        [XmlElement("item_id")]
	        public string ItemId { get; set; }
	
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
	
	        /// <summary>
	        /// 商品数量
	        /// </summary>
	        [XmlElement("quantity")]
	        public Nullable<long> Quantity { get; set; }
	
	        /// <summary>
	        /// 备注
	        /// </summary>
	        [XmlElement("remark")]
	        public string Remark { get; set; }
}

	/// <summary>
/// ErpInventoryCountCallBackRequestDomain Data Structure.
/// </summary>
[Serializable]

public class ErpInventoryCountCallBackRequestDomain : TopObject
{
	        /// <summary>
	        /// 损益调整主单号
	        /// </summary>
	        [XmlElement("adjust_biz_key")]
	        public string AdjustBizKey { get; set; }
	
	        /// <summary>
	        /// 差异原因
	        /// </summary>
	        [XmlElement("adjust_reason_type")]
	        public string AdjustReasonType { get; set; }
	
	        /// <summary>
	        /// 损益类型
	        /// </summary>
	        [XmlElement("adjust_type")]
	        public string AdjustType { get; set; }
	
	        /// <summary>
	        /// 业务类型值
	        /// </summary>
	        [XmlElement("biz_type")]
	        public Nullable<long> BizType { get; set; }
	
	        /// <summary>
	        /// 拓展属性
	        /// </summary>
	        [XmlElement("extend_attribute")]
	        public string ExtendAttribute { get; set; }
	
	        /// <summary>
	        /// 差异单订单编号
	        /// </summary>
	        [XmlElement("imbalance_order_code")]
	        public string ImbalanceOrderCode { get; set; }
	
	        /// <summary>
	        /// 商品信息列表
	        /// </summary>
	        [XmlArray("item_list")]
	        [XmlArrayItem("itemlist")]
	        public List<ItemlistDomain> ItemList { get; set; }
	
	        /// <summary>
	        /// wms盘点单号
	        /// </summary>
	        [XmlElement("order_code")]
	        public string OrderCode { get; set; }
	
	        /// <summary>
	        /// 订单类型：701 盘点出库（盘亏）702 盘点入库（盘盈）
	        /// </summary>
	        [XmlElement("order_type")]
	        public Nullable<long> OrderType { get; set; }
	
	        /// <summary>
	        /// 外部回传幂等key，每单每次回传必须不同，同一次回传重试必须保证与第一次回传相同
	        /// </summary>
	        [XmlElement("out_biz_code")]
	        public string OutBizCode { get; set; }
	
	        /// <summary>
	        /// 货主ID
	        /// </summary>
	        [XmlElement("owner_user_id")]
	        public string OwnerUserId { get; set; }
	
	        /// <summary>
	        /// 备注
	        /// </summary>
	        [XmlElement("remark")]
	        public string Remark { get; set; }
	
	        /// <summary>
	        /// 责任方
	        /// </summary>
	        [XmlElement("responsibility_code")]
	        public string ResponsibilityCode { get; set; }
	
	        /// <summary>
	        /// 店铺id
	        /// </summary>
	        [XmlElement("seller_id")]
	        public Nullable<long> SellerId { get; set; }
	
	        /// <summary>
	        /// 仓库编码
	        /// </summary>
	        [XmlElement("service_code")]
	        public string ServiceCode { get; set; }
	
	        /// <summary>
	        /// 仓库编码
	        /// </summary>
	        [XmlElement("store_code")]
	        public string StoreCode { get; set; }
	
	        /// <summary>
	        /// wms盘点单号
	        /// </summary>
	        [XmlElement("store_order_code")]
	        public string StoreOrderCode { get; set; }
}

        #endregion
    }
}
