using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// AlibabaEinvoiceApplyGetResponse.
    /// </summary>
    public class AlibabaEinvoiceApplyGetResponse : TopResponse
    {
        /// <summary>
        /// 开票明细
        /// </summary>
        [XmlArray("apply_list")]
        [XmlArrayItem("apply")]
        public List<ApplyDomain> ApplyList { get; set; }

        /// <summary>
        /// success
        /// </summary>
        [XmlElement("is_success")]
        public bool IsSuccess { get; set; }

	/// <summary>
/// InvoiceItemDomain Data Structure.
/// </summary>
[Serializable]

public class InvoiceItemDomain : TopObject
{
	        /// <summary>
	        /// 价税合计。(等于sumPrice和tax之和)
	        /// </summary>
	        [XmlElement("amount")]
	        public string Amount { get; set; }
	
	        /// <summary>
	        /// 发票项目名称（或商品名称）
	        /// </summary>
	        [XmlElement("item_name")]
	        public string ItemName { get; set; }
	
	        /// <summary>
	        /// 单价，格式：100.00(不含税)
	        /// </summary>
	        [XmlElement("price")]
	        public string Price { get; set; }
	
	        /// <summary>
	        /// 数量
	        /// </summary>
	        [XmlElement("quantity")]
	        public string Quantity { get; set; }
	
	        /// <summary>
	        /// 发票行性质。0表示正常行，1表示折扣行，2表示被折扣行。比如充电器单价100元，折扣10元，则明细为2行，充电器行性质为2，折扣行性质为1。如果充电器没有折扣，则值应为0
	        /// </summary>
	        [XmlElement("row_type")]
	        public string RowType { get; set; }
	
	        /// <summary>
	        /// 规格型号,可选
	        /// </summary>
	        [XmlElement("specification")]
	        public string Specification { get; set; }
	
	        /// <summary>
	        /// 总价，格式：100.00(不含税)
	        /// </summary>
	        [XmlElement("sum_price")]
	        public string SumPrice { get; set; }
	
	        /// <summary>
	        /// 税额
	        /// </summary>
	        [XmlElement("tax")]
	        public string Tax { get; set; }
	
	        /// <summary>
	        /// 税率。税率只能为0或0.03或0.04或0.06或0.11或0.13或0.17
	        /// </summary>
	        [XmlElement("tax_rate")]
	        public string TaxRate { get; set; }
	
	        /// <summary>
	        /// 单位
	        /// </summary>
	        [XmlElement("unit")]
	        public string Unit { get; set; }
}

	/// <summary>
/// ApplyDomain Data Structure.
/// </summary>
[Serializable]

public class ApplyDomain : TopObject
{
	        /// <summary>
	        /// 抬头类型，0=个人，1=企业
	        /// </summary>
	        [XmlElement("business_type")]
	        public long BusinessType { get; set; }
	
	        /// <summary>
	        /// 开票金额
	        /// </summary>
	        [XmlElement("invoice_amount")]
	        public string InvoiceAmount { get; set; }
	
	        /// <summary>
	        /// 发票明细
	        /// </summary>
	        [XmlArray("invoice_items")]
	        [XmlArrayItem("invoice_item")]
	        public List<InvoiceItemDomain> InvoiceItems { get; set; }
	
	        /// <summary>
	        /// 发票种类，0=电子发票，1=纸质发票，2=专票，现在默认是0
	        /// </summary>
	        [XmlElement("invoice_kind")]
	        public long InvoiceKind { get; set; }
	
	        /// <summary>
	        /// 发票(开票)类型，蓝票blue,红票red，默认blue
	        /// </summary>
	        [XmlElement("invoice_type")]
	        public string InvoiceType { get; set; }
	
	        /// <summary>
	        /// 买家备注
	        /// </summary>
	        [XmlElement("memo")]
	        public string Memo { get; set; }
	
	        /// <summary>
	        /// 买家抬头
	        /// </summary>
	        [XmlElement("payer_name")]
	        public string PayerName { get; set; }
	
	        /// <summary>
	        /// 买家税号
	        /// </summary>
	        [XmlElement("payer_register_no")]
	        public string PayerRegisterNo { get; set; }
	
	        /// <summary>
	        /// 电商平台代码,TB,TM,ALIPAY,JD
	        /// </summary>
	        [XmlElement("platform_code")]
	        public string PlatformCode { get; set; }
	
	        /// <summary>
	        /// 电商平台对应的订单号
	        /// </summary>
	        [XmlElement("platform_tid")]
	        public string PlatformTid { get; set; }
	
	        /// <summary>
	        /// 开票申请状态，0=已拒绝，1=申请中，2=已同意
	        /// </summary>
	        [XmlElement("status")]
	        public long Status { get; set; }
	
	        /// <summary>
	        /// 不含税总金额
	        /// </summary>
	        [XmlElement("sum_price")]
	        public string SumPrice { get; set; }
	
	        /// <summary>
	        /// 总税额
	        /// </summary>
	        [XmlElement("sum_tax")]
	        public string SumTax { get; set; }
	
	        /// <summary>
	        /// 开票申请的触发类型，buyer_payed=卖家已付款，sent_goods=卖家已发货，buyer_confirm=买家确认收货，refund_seller_confirm=卖家同意退款，invoice_supply=买家申请补开发票，invoice_change=买家申请改抬头
	        /// </summary>
	        [XmlElement("trigger_status")]
	        public string TriggerStatus { get; set; }
}

    }
}
