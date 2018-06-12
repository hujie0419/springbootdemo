using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// AlibabaScmExternalWmsItemGetResponse.
    /// </summary>
    public class AlibabaScmExternalWmsItemGetResponse : TopResponse
    {
        /// <summary>
        /// 保质期临期天数
        /// </summary>
        [XmlElement("advent_lifecycle")]
        public long AdventLifecycle { get; set; }

        /// <summary>
        /// 批准文号
        /// </summary>
        [XmlElement("approval_number")]
        public string ApprovalNumber { get; set; }

        /// <summary>
        /// 条形码
        /// </summary>
        [XmlElement("bar_code")]
        public string BarCode { get; set; }

        /// <summary>
        /// 品牌编码
        /// </summary>
        [XmlElement("brand")]
        public string Brand { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        [XmlElement("brand_name")]
        public string BrandName { get; set; }

        /// <summary>
        /// 商品类别编码
        /// </summary>
        [XmlElement("category")]
        public string Category { get; set; }

        /// <summary>
        /// 商品类别名称
        /// </summary>
        [XmlElement("category_name")]
        public string CategoryName { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        [XmlElement("color")]
        public string Color { get; set; }

        /// <summary>
        /// 成本价
        /// </summary>
        [XmlElement("cost_price")]
        public long CostPrice { get; set; }

        /// <summary>
        /// 错误码
        /// </summary>
        [XmlElement("error_code")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// 错误原因
        /// </summary>
        [XmlElement("error_msg")]
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 拓展字段
        /// </summary>
        [XmlElement("extend_fields")]
        public string ExtendFields { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        [XmlElement("gross_weight")]
        public long GrossWeight { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        [XmlElement("height")]
        public long Height { get; set; }

        /// <summary>
        /// 是否启用批次管理
        /// </summary>
        [XmlElement("is_batch_mgt")]
        public bool IsBatchMgt { get; set; }

        /// <summary>
        /// 是否危险品
        /// </summary>
        [XmlElement("is_danger")]
        public bool IsDanger { get; set; }

        /// <summary>
        /// 是否易碎品
        /// </summary>
        [XmlElement("is_hygroscopic")]
        public bool IsHygroscopic { get; set; }

        /// <summary>
        /// 是否启用保质期管理
        /// </summary>
        [XmlElement("is_shelflife")]
        public bool IsShelflife { get; set; }

        /// <summary>
        /// 是否启用序列号管理
        /// </summary>
        [XmlElement("is_snmgt")]
        public bool IsSnmgt { get; set; }

        /// <summary>
        /// 商家商品编码
        /// </summary>
        [XmlElement("item_code")]
        public string ItemCode { get; set; }

        /// <summary>
        /// 外部系统ID
        /// </summary>
        [XmlElement("item_id")]
        public string ItemId { get; set; }

        /// <summary>
        /// 零售价
        /// </summary>
        [XmlElement("item_price")]
        public long ItemPrice { get; set; }

        /// <summary>
        /// 长度
        /// </summary>
        [XmlElement("length")]
        public long Length { get; set; }

        /// <summary>
        /// 商品保质期天数
        /// </summary>
        [XmlElement("lifecycle")]
        public long Lifecycle { get; set; }

        /// <summary>
        /// 保质期禁售天数
        /// </summary>
        [XmlElement("lockup_lifecycle")]
        public long LockupLifecycle { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        [XmlElement("model")]
        public string Model { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 净含量
        /// </summary>
        [XmlElement("net_content")]
        public string NetContent { get; set; }

        /// <summary>
        /// 净重
        /// </summary>
        [XmlElement("net_weight")]
        public long NetWeight { get; set; }

        /// <summary>
        /// 机油等级
        /// </summary>
        [XmlElement("oil_level")]
        public string OilLevel { get; set; }

        /// <summary>
        /// 产地
        /// </summary>
        [XmlElement("origin_address")]
        public string OriginAddress { get; set; }

        /// <summary>
        /// 货主ID
        /// </summary>
        [XmlElement("owner_user_id")]
        public string OwnerUserId { get; set; }

        /// <summary>
        /// 箱规
        /// </summary>
        [XmlElement("pcs")]
        public string Pcs { get; set; }

        /// <summary>
        /// 保质期禁收天数
        /// </summary>
        [XmlElement("reject_lifecycle")]
        public long RejectLifecycle { get; set; }

        /// <summary>
        /// 尺码
        /// </summary>
        [XmlElement("size")]
        public string Size { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        [XmlElement("specification")]
        public string Specification { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        [XmlElement("success")]
        public bool Success { get; set; }

        /// <summary>
        /// 吊牌价
        /// </summary>
        [XmlElement("tag_price")]
        public long TagPrice { get; set; }

        /// <summary>
        /// 商品标题
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; }

        /// <summary>
        /// 商品类型
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }

        /// <summary>
        /// 计量单位 商品计量单位：件、箱、KG
        /// </summary>
        [XmlElement("unit")]
        public string Unit { get; set; }

        /// <summary>
        /// 启用标示
        /// </summary>
        [XmlElement("use_yn")]
        public bool UseYn { get; set; }

        /// <summary>
        /// 粘度级别
        /// </summary>
        [XmlElement("viscosity_level")]
        public string ViscosityLevel { get; set; }

        /// <summary>
        /// 体积
        /// </summary>
        [XmlElement("volume")]
        public long Volume { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        [XmlElement("width")]
        public long Width { get; set; }

    }
}
