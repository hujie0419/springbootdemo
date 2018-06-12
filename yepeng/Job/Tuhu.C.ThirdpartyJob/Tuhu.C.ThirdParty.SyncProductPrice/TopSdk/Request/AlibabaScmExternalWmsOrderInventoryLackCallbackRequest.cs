using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.scm.external.wms.order.inventory.lack.callback
    /// </summary>
    public class AlibabaScmExternalWmsOrderInventoryLackCallbackRequest : BaseTopRequest<Top.Api.Response.AlibabaScmExternalWmsOrderInventoryLackCallbackResponse>
    {
        /// <summary>
        /// 回告请求对象
        /// </summary>
        public string CallbackRequest { get; set; }

        public ErpInventoryLackUploadCallBackRequestDomain CallbackRequest_ { set { this.CallbackRequest = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.scm.external.wms.order.inventory.lack.callback";
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
/// ItemlistDomain Data Structure.
/// </summary>
[Serializable]

public class ItemlistDomain : TopObject
{
	        /// <summary>
	        /// 商家对商品的编码
	        /// </summary>
	        [XmlElement("item_code")]
	        public string ItemCode { get; set; }
	
	        /// <summary>
	        /// 后端商品编号，Scm 通过此字段关联商品数据
	        /// </summary>
	        [XmlElement("item_id")]
	        public string ItemId { get; set; }
	
	        /// <summary>
	        /// 应发商品数量
	        /// </summary>
	        [XmlElement("item_qty")]
	        public Nullable<long> ItemQty { get; set; }
	
	        /// <summary>
	        /// 报缺标志（0 系统缺货 或者 1 实物缺货）
	        /// </summary>
	        [XmlElement("lack_flag")]
	        public Nullable<long> LackFlag { get; set; }
	
	        /// <summary>
	        /// 商品缺货数量
	        /// </summary>
	        [XmlElement("lack_qty")]
	        public Nullable<long> LackQty { get; set; }
	
	        /// <summary>
	        /// 订单明细行编码 scm通过此字段关联子发货单记录
	        /// </summary>
	        [XmlElement("order_item_id")]
	        public string OrderItemId { get; set; }
	
	        /// <summary>
	        /// 货主ID
	        /// </summary>
	        [XmlElement("own_user_id")]
	        public string OwnUserId { get; set; }
	
	        /// <summary>
	        /// 报缺原因（系统报缺，实物报缺)
	        /// </summary>
	        [XmlElement("reason")]
	        public string Reason { get; set; }
}

	/// <summary>
/// ErpInventoryLackUploadCallBackRequestDomain Data Structure.
/// </summary>
[Serializable]

public class ErpInventoryLackUploadCallBackRequestDomain : TopObject
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
	        /// 创建时间  格式: yyyy-MM-dd HH:mm:ss
	        /// </summary>
	        [XmlElement("create_time")]
	        public string CreateTime { get; set; }
	
	        /// <summary>
	        /// 商品信息列表 服务商品不要回告
	        /// </summary>
	        [XmlArray("item_list")]
	        [XmlArrayItem("itemlist")]
	        public List<ItemlistDomain> ItemList { get; set; }
	
	        /// <summary>
	        /// 阿里Scm 订单编码，Scm通过此字段关联发货单数据
	        /// </summary>
	        [XmlElement("order_code")]
	        public string OrderCode { get; set; }
	
	        /// <summary>
	        /// 阿里交易订单编号，通知业务系统时，需要使用。
	        /// </summary>
	        [XmlElement("out_order_code")]
	        public string OutOrderCode { get; set; }
	
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
}

        #endregion
    }
}
