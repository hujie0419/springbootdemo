using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.einvoice.createreq
    /// </summary>
    public class AlibabaEinvoiceCreatereqRequest : BaseTopRequest<Top.Api.Response.AlibabaEinvoiceCreatereqResponse>
    {
        /// <summary>
        /// 开票申请ID，接收了开票申请消息后，需要把apply_id带上
        /// </summary>
        public string ApplyId { get; set; }

        /// <summary>
        /// 默认：0。对于商家对个人开具，为0;对于商家对企业开具，为1;
        /// </summary>
        public Nullable<long> BusinessType { get; set; }

        /// <summary>
        /// ERP系统中的单据号。如果没有erp的唯一单据号。建议使用platform_code+”_”+ platform_tid的组合方式
        /// </summary>
        public string ErpTid { get; set; }

        /// <summary>
        /// 开票金额； <span style="color:red;font-weight: bold;">当开红票时，该字段为负数</span>
        /// </summary>
        public string InvoiceAmount { get; set; }

        /// <summary>
        /// 电子发票明细
        /// </summary>
        public string InvoiceItems { get; set; }

        public List<InvoiceItemDomain> InvoiceItems_ { set { this.InvoiceItems = TopUtils.ObjectToJson(value); } } 

        /// <summary>
        /// 发票备注，有些省市会把此信息打印到PDF中
        /// </summary>
        public string InvoiceMemo { get; set; }

        /// <summary>
        /// 开票日期, 格式"YYYY-MM-DD HH:SS:MM"
        /// </summary>
        public Nullable<DateTime> InvoiceTime { get; set; }

        /// <summary>
        /// 发票(开票)类型，蓝票blue,红票red，默认blue
        /// </summary>
        public string InvoiceType { get; set; }

        /// <summary>
        /// 原发票代码(开红票时传入)
        /// </summary>
        public string NormalInvoiceCode { get; set; }

        /// <summary>
        /// 原发票号码(开红票时传入)
        /// </summary>
        public string NormalInvoiceNo { get; set; }

        /// <summary>
        /// 外部平台店铺名称，需要在阿里发票平台配置，只有当platform_code不为TB和TM时，这个字段才生效。注意：后台配置的店铺平台必须和入参platform_code一致
        /// </summary>
        public string OutShopName { get; set; }

        /// <summary>
        /// 开票方地址(新版中为必传)
        /// </summary>
        public string PayeeAddress { get; set; }

        /// <summary>
        /// 开票方银行及 帐号
        /// </summary>
        public string PayeeBankaccount { get; set; }

        /// <summary>
        /// 复核人
        /// </summary>
        public string PayeeChecker { get; set; }

        /// <summary>
        /// 开票方名称，公司名(如:XX商城)
        /// </summary>
        public string PayeeName { get; set; }

        /// <summary>
        /// 开票人
        /// </summary>
        public string PayeeOperator { get; set; }

        /// <summary>
        /// 收款方电话
        /// </summary>
        public string PayeePhone { get; set; }

        /// <summary>
        /// 收款人
        /// </summary>
        public string PayeeReceiver { get; set; }

        /// <summary>
        /// 收款方税务登记证号
        /// </summary>
        public string PayeeRegisterNo { get; set; }

        /// <summary>
        /// 消费者地址
        /// </summary>
        public string PayerAddress { get; set; }

        /// <summary>
        /// 付款方开票开户银行及账号
        /// </summary>
        public string PayerBankaccount { get; set; }

        /// <summary>
        /// 消费者电子邮箱
        /// </summary>
        public string PayerEmail { get; set; }

        /// <summary>
        /// 付款方名称, 对应发票台头
        /// </summary>
        public string PayerName { get; set; }

        /// <summary>
        /// 消费者联系电话
        /// </summary>
        public string PayerPhone { get; set; }

        /// <summary>
        /// 付款方税务登记证号。对企业开具电子发票时必填。目前北京地区暂未开放对企业开具电子发票，若北京地区放开后，对于向企业开具的情况，付款方税务登记证号和名称也不能为空
        /// </summary>
        public string PayerRegisterNo { get; set; }

        /// <summary>
        /// 电商平台代码。TB=淘宝 、TM=天猫 、JD=京东、DD=当当、PP=拍拍、YX=易讯、EBAY=ebay、QQ=QQ网购、AMAZON=亚马逊、SN=苏宁、GM=国美、WPH=唯品会、JM=聚美、LF=乐蜂、MGJ=蘑菇街、JS=聚尚、PX=拍鞋、YT=银泰、YHD=1号店、VANCL=凡客、YL=邮乐、YG=优购、1688=阿里巴巴、POS=POS门店、OTHER=其他,  (只传英文编码)
        /// </summary>
        public string PlatformCode { get; set; }

        /// <summary>
        /// 电商平台对应的主订单号
        /// </summary>
        public string PlatformTid { get; set; }

        /// <summary>
        /// 开票服务商的APPKEY
        /// </summary>
        public string ProviderAppkey { get; set; }

        /// <summary>
        /// 商家自己申请的放在开票代理客户端的appkey
        /// </summary>
        public string ProxyAppkey { get; set; }

        /// <summary>
        /// 开票流水号，唯一标志开票请求。如果两次请求流水号相同，则表示重复请求。请调用平台统一流水号获取接口，alibaba.einvoice.serialno.generate。
        /// </summary>
        public string SerialNo { get; set; }

        /// <summary>
        /// 合计金额(新版中为必传) <span style="color:red;font-weight: bold;">当开红票时，该字段为负数</span>
        /// </summary>
        public string SumPrice { get; set; }

        /// <summary>
        /// 合计税额 <span style="color:red;font-weight: bold;">当开红票时，该字段为负数</span>
        /// </summary>
        public string SumTax { get; set; }

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.einvoice.createreq";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("apply_id", this.ApplyId);
            parameters.Add("business_type", this.BusinessType);
            parameters.Add("erp_tid", this.ErpTid);
            parameters.Add("invoice_amount", this.InvoiceAmount);
            parameters.Add("invoice_items", this.InvoiceItems);
            parameters.Add("invoice_memo", this.InvoiceMemo);
            parameters.Add("invoice_time", this.InvoiceTime);
            parameters.Add("invoice_type", this.InvoiceType);
            parameters.Add("normal_invoice_code", this.NormalInvoiceCode);
            parameters.Add("normal_invoice_no", this.NormalInvoiceNo);
            parameters.Add("out_shop_name", this.OutShopName);
            parameters.Add("payee_address", this.PayeeAddress);
            parameters.Add("payee_bankaccount", this.PayeeBankaccount);
            parameters.Add("payee_checker", this.PayeeChecker);
            parameters.Add("payee_name", this.PayeeName);
            parameters.Add("payee_operator", this.PayeeOperator);
            parameters.Add("payee_phone", this.PayeePhone);
            parameters.Add("payee_receiver", this.PayeeReceiver);
            parameters.Add("payee_register_no", this.PayeeRegisterNo);
            parameters.Add("payer_address", this.PayerAddress);
            parameters.Add("payer_bankaccount", this.PayerBankaccount);
            parameters.Add("payer_email", this.PayerEmail);
            parameters.Add("payer_name", this.PayerName);
            parameters.Add("payer_phone", this.PayerPhone);
            parameters.Add("payer_register_no", this.PayerRegisterNo);
            parameters.Add("platform_code", this.PlatformCode);
            parameters.Add("platform_tid", this.PlatformTid);
            parameters.Add("provider_appkey", this.ProviderAppkey);
            parameters.Add("proxy_appkey", this.ProxyAppkey);
            parameters.Add("serial_no", this.SerialNo);
            parameters.Add("sum_price", this.SumPrice);
            parameters.Add("sum_tax", this.SumTax);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("business_type", this.BusinessType);
            RequestValidator.ValidateRequired("invoice_amount", this.InvoiceAmount);
            RequestValidator.ValidateRequired("invoice_items", this.InvoiceItems);
            RequestValidator.ValidateObjectMaxListSize("invoice_items", this.InvoiceItems, 100);
            RequestValidator.ValidateMaxLength("invoice_memo", this.InvoiceMemo, 200);
            RequestValidator.ValidateRequired("invoice_time", this.InvoiceTime);
            RequestValidator.ValidateRequired("invoice_type", this.InvoiceType);
            RequestValidator.ValidateMaxLength("normal_invoice_code", this.NormalInvoiceCode, 12);
            RequestValidator.ValidateMaxLength("normal_invoice_no", this.NormalInvoiceNo, 8);
            RequestValidator.ValidateRequired("payee_address", this.PayeeAddress);
            RequestValidator.ValidateMaxLength("payee_address", this.PayeeAddress, 100);
            RequestValidator.ValidateMaxLength("payee_bankaccount", this.PayeeBankaccount, 100);
            RequestValidator.ValidateMaxLength("payee_checker", this.PayeeChecker, 8);
            RequestValidator.ValidateRequired("payee_name", this.PayeeName);
            RequestValidator.ValidateMaxLength("payee_name", this.PayeeName, 100);
            RequestValidator.ValidateMaxLength("payee_operator", this.PayeeOperator, 8);
            RequestValidator.ValidateMaxLength("payee_phone", this.PayeePhone, 20);
            RequestValidator.ValidateMaxLength("payee_receiver", this.PayeeReceiver, 8);
            RequestValidator.ValidateRequired("payee_register_no", this.PayeeRegisterNo);
            RequestValidator.ValidateMaxLength("payee_register_no", this.PayeeRegisterNo, 20);
            RequestValidator.ValidateMaxLength("payer_address", this.PayerAddress, 100);
            RequestValidator.ValidateMaxLength("payer_bankaccount", this.PayerBankaccount, 100);
            RequestValidator.ValidateRequired("payer_name", this.PayerName);
            RequestValidator.ValidateMaxLength("payer_name", this.PayerName, 100);
            RequestValidator.ValidateMaxLength("payer_phone", this.PayerPhone, 20);
            RequestValidator.ValidateMaxLength("payer_register_no", this.PayerRegisterNo, 20);
            RequestValidator.ValidateRequired("platform_code", this.PlatformCode);
            RequestValidator.ValidateRequired("platform_tid", this.PlatformTid);
            RequestValidator.ValidateRequired("serial_no", this.SerialNo);
            RequestValidator.ValidateMaxLength("serial_no", this.SerialNo, 20);
            RequestValidator.ValidateRequired("sum_price", this.SumPrice);
            RequestValidator.ValidateRequired("sum_tax", this.SumTax);
        }

	/// <summary>
/// InvoiceItemDomain Data Structure.
/// </summary>
[Serializable]

public class InvoiceItemDomain : TopObject
{
	        /// <summary>
	        /// 价税合计。(等于sumPrice和tax之和) <span style="color:red;font-weight: bold;">当开红票时，该字段为负数</span>
	        /// </summary>
	        [XmlElement("amount")]
	        public string Amount { get; set; }
	
	        /// <summary>
	        /// 商品IMIE号(不用传，将废弃)
	        /// </summary>
	        [XmlElement("imei")]
	        public string Imei { get; set; }
	
	        /// <summary>
	        /// 发票项目名称（或商品名称）
	        /// </summary>
	        [XmlElement("item_name")]
	        public string ItemName { get; set; }
	
	        /// <summary>
	        /// <span style="color:red;font-weight: bold;">商品编码</span>，税号升级商品编码版本后必填
	        /// </summary>
	        [XmlElement("item_no")]
	        public string ItemNo { get; set; }
	
	        /// <summary>
	        /// 单价，格式：100.00。新版电子发票，折扣行此参数不能传，非折扣行必传；红票、蓝票都为正数
	        /// </summary>
	        [XmlElement("price")]
	        public string Price { get; set; }
	
	        /// <summary>
	        /// 数量。新版电子发票，折扣行此参数不能传，非折扣行必传； <span style="color:red;font-weight: bold;">当开红票时，该字段需为负数</span>
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
	        /// 总价，格式：100.00； <span style="color:red;font-weight: bold;">当开红票时，该字段为负数</span>
	        /// </summary>
	        [XmlElement("sum_price")]
	        public string SumPrice { get; set; }
	
	        /// <summary>
	        /// 税额； <span style="color:red;font-weight: bold;">当开红票时，该字段为负数</span>
	        /// </summary>
	        [XmlElement("tax")]
	        public string Tax { get; set; }
	
	        /// <summary>
	        /// 税率。税率只能为0或0.03或0.04或0.06或0.11或0.13或0.17
	        /// </summary>
	        [XmlElement("tax_rate")]
	        public string TaxRate { get; set; }
	
	        /// <summary>
	        /// 单位。新版电子发票，折扣行不能传，非折扣行必传
	        /// </summary>
	        [XmlElement("unit")]
	        public string Unit { get; set; }
	
	        /// <summary>
	        /// 0税率标识，只有税率为0的情况才有值，0=出口零税率，1=免税，2=不征收，3=普通零税率
	        /// </summary>
	        [XmlElement("zero_rate_flag")]
	        public string ZeroRateFlag { get; set; }
}

        #endregion
    }
}
