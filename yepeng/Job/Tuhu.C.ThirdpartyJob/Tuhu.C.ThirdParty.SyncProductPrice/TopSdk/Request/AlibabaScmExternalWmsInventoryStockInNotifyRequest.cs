using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.scm.external.wms.inventory.stock.in.notify
    /// </summary>
    public class AlibabaScmExternalWmsInventoryStockInNotifyRequest : BaseTopRequest<Top.Api.Response.AlibabaScmExternalWmsInventoryStockInNotifyResponse>
    {
        /// <summary>
        /// 入库下发请求
        /// </summary>
        public string Request { get; set; }

        public StructDomain Request_ { set { this.Request = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.scm.external.wms.inventory.stock.in.notify";
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("request", this.Request);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("request", this.Request);
        }

	/// <summary>
/// BatchSendCtrlParamDomain Data Structure.
/// </summary>
[Serializable]

public class BatchSendCtrlParamDomain : TopObject
{
	        /// <summary>
	        /// 必选 是否有更多商品 0 一次发送 1 多次发送
	        /// </summary>
	        [XmlElement("distribute_type")]
	        public Nullable<long> DistributeType { get; set; }
	
	        /// <summary>
	        /// 可选 总共ITEM数量(当distribute_type为1时为必选
	        /// </summary>
	        [XmlElement("total_order_item_count")]
	        public Nullable<long> TotalOrderItemCount { get; set; }
}

	/// <summary>
/// ReceiverInfoDomain Data Structure.
/// </summary>
[Serializable]

public class ReceiverInfoDomain : TopObject
{
	        /// <summary>
	        /// 收件方地址
	        /// </summary>
	        [XmlElement("receiver_address")]
	        public string ReceiverAddress { get; set; }
	
	        /// <summary>
	        /// 收件方区县
	        /// </summary>
	        [XmlElement("receiver_area")]
	        public string ReceiverArea { get; set; }
	
	        /// <summary>
	        /// 收件方城市
	        /// </summary>
	        [XmlElement("receiver_city")]
	        public string ReceiverCity { get; set; }
	
	        /// <summary>
	        /// 收件方手机
	        /// </summary>
	        [XmlElement("receiver_mobile")]
	        public string ReceiverMobile { get; set; }
	
	        /// <summary>
	        /// 收件方名称
	        /// </summary>
	        [XmlElement("receiver_name")]
	        public string ReceiverName { get; set; }
	
	        /// <summary>
	        /// 收件方电话
	        /// </summary>
	        [XmlElement("receiver_phone")]
	        public string ReceiverPhone { get; set; }
	
	        /// <summary>
	        /// 收件方省份
	        /// </summary>
	        [XmlElement("receiver_province")]
	        public string ReceiverProvince { get; set; }
	
	        /// <summary>
	        /// 收件方镇
	        /// </summary>
	        [XmlElement("receiver_town")]
	        public string ReceiverTown { get; set; }
	
	        /// <summary>
	        /// 收件方邮编
	        /// </summary>
	        [XmlElement("receiver_zip_code")]
	        public string ReceiverZipCode { get; set; }
}

	/// <summary>
/// SenderInfoDomain Data Structure.
/// </summary>
[Serializable]

public class SenderInfoDomain : TopObject
{
	        /// <summary>
	        /// 发件方地址
	        /// </summary>
	        [XmlElement("sender_address")]
	        public string SenderAddress { get; set; }
	
	        /// <summary>
	        /// 发件方区县
	        /// </summary>
	        [XmlElement("sender_area")]
	        public string SenderArea { get; set; }
	
	        /// <summary>
	        /// 发件方城市
	        /// </summary>
	        [XmlElement("sender_city")]
	        public string SenderCity { get; set; }
	
	        /// <summary>
	        /// 发件方编码：销退入库单由ECP填充买家昵称、旺旺，采购入库单填充供应商编码，调  拨入库单填充调出仓库编码
	        /// </summary>
	        [XmlElement("sender_code")]
	        public string SenderCode { get; set; }
	
	        /// <summary>
	        /// 发件方手机
	        /// </summary>
	        [XmlElement("sender_mobile")]
	        public string SenderMobile { get; set; }
	
	        /// <summary>
	        /// 收发件方名称:（采购入库放供应商名称）， （销退填买家名称）， （调拨入库填写仓库名称）
	        /// </summary>
	        [XmlElement("sender_name")]
	        public string SenderName { get; set; }
	
	        /// <summary>
	        /// 发件方电话
	        /// </summary>
	        [XmlElement("sender_phone")]
	        public string SenderPhone { get; set; }
	
	        /// <summary>
	        /// 发件方省份
	        /// </summary>
	        [XmlElement("sender_province")]
	        public string SenderProvince { get; set; }
	
	        /// <summary>
	        /// 发件方镇
	        /// </summary>
	        [XmlElement("sender_town")]
	        public string SenderTown { get; set; }
	
	        /// <summary>
	        /// 发件方邮编
	        /// </summary>
	        [XmlElement("sender_zip_code")]
	        public string SenderZipCode { get; set; }
}

	/// <summary>
/// StockInOrderItemDomain Data Structure.
/// </summary>
[Serializable]

public class StockInOrderItemDomain : TopObject
{
	        /// <summary>
	        /// 形码
	        /// </summary>
	        [XmlElement("bar_code")]
	        public string BarCode { get; set; }
	
	        /// <summary>
	        /// 批次号
	        /// </summary>
	        [XmlElement("batch_code")]
	        public string BatchCode { get; set; }
	
	        /// <summary>
	        /// 备注
	        /// </summary>
	        [XmlElement("batch_remark")]
	        public string BatchRemark { get; set; }
	
	        /// <summary>
	        /// 到货日期格式为 yyyy-MM-dd HH:mm:ss
	        /// </summary>
	        [XmlElement("due_date")]
	        public string DueDate { get; set; }
	
	        /// <summary>
	        /// 订单商品拓展属性数据
	        /// </summary>
	        [XmlElement("extend_fields")]
	        public string ExtendFields { get; set; }
	
	        /// <summary>
	        /// 库存类型（1 可销售库存(正品) 101 残次 102 机损 103 箱损 201 冻结库存 301 在途库存 ）
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
	        /// 商品名称
	        /// </summary>
	        [XmlElement("item_name")]
	        public string ItemName { get; set; }
	
	        /// <summary>
	        /// 销售价格
	        /// </summary>
	        [XmlElement("item_price")]
	        public Nullable<long> ItemPrice { get; set; }
	
	        /// <summary>
	        /// 商品数量
	        /// </summary>
	        [XmlElement("item_quantity")]
	        public Nullable<long> ItemQuantity { get; set; }
	
	        /// <summary>
	        /// 商品版本号
	        /// </summary>
	        [XmlElement("item_version")]
	        public Nullable<long> ItemVersion { get; set; }
	
	        /// <summary>
	        /// ERP主键ID
	        /// </summary>
	        [XmlElement("order_item_id")]
	        public string OrderItemId { get; set; }
	
	        /// <summary>
	        /// 采购入库填写采购订单号，调拨  入库填写调拨单号
	        /// </summary>
	        [XmlElement("order_source_code")]
	        public string OrderSourceCode { get; set; }
	
	        /// <summary>
	        /// 货主ID 代销情况下货主ID和卖家ID不同(销售场景下：代销业务中货主ID和  卖家ID不同)
	        /// </summary>
	        [XmlElement("owner_user_id")]
	        public string OwnerUserId { get; set; }
	
	        /// <summary>
	        /// 货主名称
	        /// </summary>
	        [XmlElement("owner_user_name")]
	        public string OwnerUserName { get; set; }
	
	        /// <summary>
	        /// 生产编码，同一商品可能因商家不同有不同编码
	        /// </summary>
	        [XmlElement("produce_code")]
	        public string ProduceCode { get; set; }
	
	        /// <summary>
	        /// 生产日期，日期格式为 yyyy-MM-dd HH:mm:ss
	        /// </summary>
	        [XmlElement("produce_date")]
	        public string ProduceDate { get; set; }
	
	        /// <summary>
	        /// 采购价格
	        /// </summary>
	        [XmlElement("purchase_price")]
	        public Nullable<long> PurchasePrice { get; set; }
	
	        /// <summary>
	        /// 平台子交易编码
	        /// </summary>
	        [XmlElement("sub_source_code")]
	        public string SubSourceCode { get; set; }
	
	        /// <summary>
	        /// 吊牌价
	        /// </summary>
	        [XmlElement("tag_price")]
	        public Nullable<long> TagPrice { get; set; }
	
	        /// <summary>
	        /// 税率
	        /// </summary>
	        [XmlElement("tax_rate")]
	        public Nullable<long> TaxRate { get; set; }
	
	        /// <summary>
	        /// 卖家ID,一般情况下，货主ID和卖家ID相同
	        /// </summary>
	        [XmlElement("user_id")]
	        public string UserId { get; set; }
	
	        /// <summary>
	        /// 卖家名称(销售店铺名称)
	        /// </summary>
	        [XmlElement("user_name")]
	        public string UserName { get; set; }
}

	/// <summary>
/// StructDomain Data Structure.
/// </summary>
[Serializable]

public class StructDomain : TopObject
{
	        /// <summary>
	        /// 分批下发控制参数
	        /// </summary>
	        [XmlElement("batch_send_ctrl_param")]
	        public BatchSendCtrlParamDomain BatchSendCtrlParam { get; set; }
	
	        /// <summary>
	        /// 业务类型
	        /// </summary>
	        [XmlElement("biz_type")]
	        public Nullable<long> BizType { get; set; }
	
	        /// <summary>
	        /// 币种 默认CNY
	        /// </summary>
	        [XmlElement("currency")]
	        public string Currency { get; set; }
	
	        /// <summary>
	        /// 预期送达结束时间,格式为 yyyy-MM-dd HH:mm:ss
	        /// </summary>
	        [XmlElement("expect_end_time")]
	        public string ExpectEndTime { get; set; }
	
	        /// <summary>
	        /// 预期送达开始时间,格式为 yyyy-MM-dd HH:mm:ss
	        /// </summary>
	        [XmlElement("expect_start_time")]
	        public string ExpectStartTime { get; set; }
	
	        /// <summary>
	        /// 拓展属性数据
	        /// </summary>
	        [XmlElement("extend_fields")]
	        public string ExtendFields { get; set; }
	
	        /// <summary>
	        /// 自定义文本透传至WMS 加工归还 委外归还 借出归还 内部归还
	        /// </summary>
	        [XmlElement("inbound_type_desc")]
	        public string InboundTypeDesc { get; set; }
	
	        /// <summary>
	        /// 可空，默认true，用于分批控制；如果是分批下发，前几次都是false，最后一次传true
	        /// </summary>
	        [XmlElement("is_finished")]
	        public Nullable<bool> IsFinished { get; set; }
	
	        /// <summary>
	        /// erp入库单单号
	        /// </summary>
	        [XmlElement("order_code")]
	        public string OrderCode { get; set; }
	
	        /// <summary>
	        /// 订单创建时间，格式为 yyyy-MM-dd HH:mm:ss
	        /// </summary>
	        [XmlElement("order_create_time")]
	        public string OrderCreateTime { get; set; }
	
	        /// <summary>
	        /// 订单标记以逗号分隔： 9:上门退货入库 13: 退货时是否收取发票，默认不收取（即没13为多选项，如1,2,8,9）
	        /// </summary>
	        [XmlElement("order_flag")]
	        public string OrderFlag { get; set; }
	
	        /// <summary>
	        /// 订单商品信息列表
	        /// </summary>
	        [XmlArray("order_item_list")]
	        [XmlArrayItem("stock_in_order_item")]
	        public List<StockInOrderItemDomain> OrderItemList { get; set; }
	
	        /// <summary>
	        /// 单据来源 201 淘宝，214京东， 203 苏宁， 204 亚马逊中国， 205当当 ，206 ebay,207 VIP，208 一号店，209 国美 210 拍拍，211 聚美，212 乐蜂 202 1688，301 其他
	        /// </summary>
	        [XmlElement("order_source")]
	        public string OrderSource { get; set; }
	
	        /// <summary>
	        /// 302调拨入库单，501销退入库单 601采购入库单 904其他入库单
	        /// </summary>
	        [XmlElement("order_type")]
	        public Nullable<long> OrderType { get; set; }
	
	        /// <summary>
	        /// 货主id
	        /// </summary>
	        [XmlElement("owner_user_id")]
	        public Nullable<long> OwnerUserId { get; set; }
	
	        /// <summary>
	        /// 前物流订单号，如退货入库单可能会用到
	        /// </summary>
	        [XmlElement("prev_order_code")]
	        public string PrevOrderCode { get; set; }
	
	        /// <summary>
	        /// 前仓库编码
	        /// </summary>
	        [XmlElement("prev_store_code")]
	        public string PrevStoreCode { get; set; }
	
	        /// <summary>
	        /// 收货地址信息
	        /// </summary>
	        [XmlElement("receiver_info")]
	        public ReceiverInfoDomain ReceiverInfo { get; set; }
	
	        /// <summary>
	        /// 备注
	        /// </summary>
	        [XmlElement("remark")]
	        public string Remark { get; set; }
	
	        /// <summary>
	        /// 退货原因：销退场景下，如可能请提供退货的原因，多个退货原因用；号分开
	        /// </summary>
	        [XmlElement("return_reason")]
	        public string ReturnReason { get; set; }
	
	        /// <summary>
	        /// 店铺id
	        /// </summary>
	        [XmlElement("seller_id")]
	        public Nullable<long> SellerId { get; set; }
	
	        /// <summary>
	        /// 发件方信息
	        /// </summary>
	        [XmlElement("sender_info")]
	        public SenderInfoDomain SenderInfo { get; set; }
	
	        /// <summary>
	        /// 仓储编码
	        /// </summary>
	        [XmlElement("store_code")]
	        public string StoreCode { get; set; }
	
	        /// <summary>
	        /// 供应商编码，往来单位编码
	        /// </summary>
	        [XmlElement("supplier_code")]
	        public string SupplierCode { get; set; }
	
	        /// <summary>
	        /// 供应商名称，往来单位名称
	        /// </summary>
	        [XmlElement("supplier_name")]
	        public string SupplierName { get; set; }
	
	        /// <summary>
	        /// 运单号:1)采购入库单支持多运单号录入
	        /// </summary>
	        [XmlElement("tms_order_code")]
	        public string TmsOrderCode { get; set; }
	
	        /// <summary>
	        /// 配送公司编码：销退场景下如果是拒收（非妥投运单），由ECP填充原单配送公司编码,
	        /// </summary>
	        [XmlElement("tms_service_code")]
	        public string TmsServiceCode { get; set; }
	
	        /// <summary>
	        /// 运单号:1)采购入库单支持多运单号录入 2)销退场景下如果是拒收（非妥投运单），由ECP填充原运单号
	        /// </summary>
	        [XmlElement("tms_service_name")]
	        public string TmsServiceName { get; set; }
	
	        /// <summary>
	        /// 可空，用于分批控制；分批下发第一次留空，接下来几次都填第一次返回的wms订单
	        /// </summary>
	        [XmlElement("wlb_order_code")]
	        public string WlbOrderCode { get; set; }
}

        #endregion
    }
}
