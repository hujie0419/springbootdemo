using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// InvoiceResult Data Structure.
    /// </summary>
    [Serializable]
    public class InvoiceResult : TopObject
    {
        /// <summary>
        /// 防伪码
        /// </summary>
        [XmlElement("anti_fake_code")]
        public string AntiFakeCode { get; set; }

        /// <summary>
        /// 错误编码
        /// </summary>
        [XmlElement("biz_error_code")]
        public string BizErrorCode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [XmlElement("biz_error_msg")]
        public string BizErrorMsg { get; set; }

        /// <summary>
        /// 发票密文，密码区的字符串
        /// </summary>
        [XmlElement("ciphertext")]
        public string Ciphertext { get; set; }

        /// <summary>
        /// 税控设备编号(新版电子发票有)
        /// </summary>
        [XmlElement("device_no")]
        public string DeviceNo { get; set; }

        /// <summary>
        /// erp自定义单据号
        /// </summary>
        [XmlElement("erp_tid")]
        public string ErpTid { get; set; }

        /// <summary>
        /// 文件类型(pdf,jpg,png)
        /// </summary>
        [XmlElement("file_data_type")]
        public string FileDataType { get; set; }

        /// <summary>
        /// 发票PDF的下载地址(仅在单个查询接口上显示，批量查询不显示)
        /// </summary>
        [XmlElement("file_path")]
        public string FilePath { get; set; }

        /// <summary>
        /// 开票金额
        /// </summary>
        [XmlElement("invoice_amount")]
        public string InvoiceAmount { get; set; }

        /// <summary>
        /// 发票代码
        /// </summary>
        [XmlElement("invoice_code")]
        public string InvoiceCode { get; set; }

        /// <summary>
        /// 开票日期
        /// </summary>
        [XmlElement("invoice_date")]
        public string InvoiceDate { get; set; }

        /// <summary>
        /// 发票明细
        /// </summary>
        [XmlElement("invoice_items")]
        public string InvoiceItems { get; set; }

        /// <summary>
        /// 发票号码
        /// </summary>
        [XmlElement("invoice_no")]
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 发票(开票)类型，蓝票blue,红票red，默认blue
        /// </summary>
        [XmlElement("invoice_type")]
        public string InvoiceType { get; set; }

        /// <summary>
        /// 收款方名称
        /// </summary>
        [XmlElement("payee_name")]
        public string PayeeName { get; set; }

        /// <summary>
        /// 开票方电话
        /// </summary>
        [XmlElement("payee_phone")]
        public string PayeePhone { get; set; }

        /// <summary>
        /// 收款方税务登记证号
        /// </summary>
        [XmlElement("payee_register_no")]
        public string PayeeRegisterNo { get; set; }

        /// <summary>
        /// 付款方名称, 对应发票title
        /// </summary>
        [XmlElement("payer_name")]
        public string PayerName { get; set; }

        /// <summary>
        /// 电商平台代码。淘宝：taobao，天猫：tmall
        /// </summary>
        [XmlElement("platform_code")]
        public string PlatformCode { get; set; }

        /// <summary>
        /// 电商平台订单号
        /// </summary>
        [XmlElement("platform_tid")]
        public string PlatformTid { get; set; }

        /// <summary>
        /// 二维码
        /// </summary>
        [XmlElement("qr_code")]
        public string QrCode { get; set; }

        /// <summary>
        /// 开票流水号，唯一标志开票请求。如果两次请求流水号相同，则表示重复请求。
        /// </summary>
        [XmlElement("serial_no")]
        public string SerialNo { get; set; }

        /// <summary>
        /// 开票状态 (waiting = 开票中) 、(create_success = 开票成功)、(create_failed = 开票失败)
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}
