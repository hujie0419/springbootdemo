using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.scm.external.wms.inventory.stock.out.notify
    /// </summary>
    public class AlibabaScmExternalWmsInventoryStockOutNotifyRequest : BaseTopRequest<Top.Api.Response.AlibabaScmExternalWmsInventoryStockOutNotifyResponse>
    {
        /// <summary>
        /// 调拨出库，退供出库下发消息体
        /// </summary>
        public string Request { get; set; }

        public StructDomain Request_ { set { this.Request = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.scm.external.wms.inventory.stock.out.notify";
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
	        public string DistributeType { get; set; }
	
	        /// <summary>
	        /// 可选 总共ITEM数量(当distribute_type为1时为必选)
	        /// </summary>
	        [XmlElement("total_order_item_count")]
	        public string TotalOrderItemCount { get; set; }
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
	        /// 收件方编码
	        /// </summary>
	        [XmlElement("receiver_code")]
	        public string ReceiverCode { get; set; }
	
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
	        /// 发件方手机
	        /// </summary>
	        [XmlElement("sender_mobile")]
	        public string SenderMobile { get; set; }
	
	        /// <summary>
	        /// 收发件方名称
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
/// StockOutOrderItemDomain Data Structure.
/// </summary>
[Serializable]

public class StockOutOrderItemDomain : TopObject
{
	        /// <summary>
	        /// 条形码
	        /// </summary>
	        [XmlElement("bar_code")]
	        public string BarCode { get; set; }
	
	        /// <summary>
	        /// 批次号
	        /// </summary>
	        [XmlElement("batch_code")]
	        public string BatchCode { get; set; }
	
	        /// <summary>
	        /// 批次备注
	        /// </summary>
	        [XmlElement("batch_remark")]
	        public string BatchRemark { get; set; }
	
	        /// <summary>
	        /// 到货日期，格式为 yyyy-MM-dd HH:mm:ss
	        /// </summary>
	        [XmlElement("due_date")]
	        public string DueDate { get; set; }
	
	        /// <summary>
	        /// 订单商品拓展属性数据
	        /// </summary>
	        [XmlElement("extend_fields")]
	        public string ExtendFields { get; set; }
	
	        /// <summary>
	        /// 库存类型：1 可销售库存 正品 101 残次 102 机损 103 箱损 201 冻结库存 301 在途库存
	        /// </summary>
	        [XmlElement("inventory_type")]
	        public Nullable<long> InventoryType { get; set; }
	
	        /// <summary>
	        /// ERP商品编码
	        /// </summary>
	        [XmlElement("item_code")]
	        public string ItemCode { get; set; }
	
	        /// <summary>
	        /// ERP商品ID
	        /// </summary>
	        [XmlElement("item_id")]
	        public string ItemId { get; set; }
	
	        /// <summary>
	        /// ERP商品名称
	        /// </summary>
	        [XmlElement("item_name")]
	        public string ItemName { get; set; }
	
	        /// <summary>
	        /// 商品销售价格
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
	        /// 平台交易编码
	        /// </summary>
	        [XmlElement("order_source_code")]
	        public string OrderSourceCode { get; set; }
	
	        /// <summary>
	        /// 货主ID 代销情况下货主ID和卖家ID不同
	        /// </summary>
	        [XmlElement("owner_user_id")]
	        public string OwnerUserId { get; set; }
	
	        /// <summary>
	        /// 货主名称
	        /// </summary>
	        [XmlElement("owner_user_name")]
	        public string OwnerUserName { get; set; }
	
	        /// <summary>
	        /// 生产批号
	        /// </summary>
	        [XmlElement("produce_code")]
	        public string ProduceCode { get; set; }
	
	        /// <summary>
	        /// 生产日期，格式为 yyyy-MM-dd HH:mm:ss
	        /// </summary>
	        [XmlElement("produce_date")]
	        public string ProduceDate { get; set; }
	
	        /// <summary>
	        /// 平台子交易编码
	        /// </summary>
	        [XmlElement("sub_source_code")]
	        public string SubSourceCode { get; set; }
	
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
	        /// 业务类型id，路由使用
	        /// </summary>
	        [XmlElement("biz_type")]
	        public Nullable<long> BizType { get; set; }
	
	        /// <summary>
	        /// 车票号
	        /// </summary>
	        [XmlElement("car_no")]
	        public string CarNo { get; set; }
	
	        /// <summary>
	        /// 承运商名称
	        /// </summary>
	        [XmlElement("carriers_name")]
	        public string CarriersName { get; set; }
	
	        /// <summary>
	        /// 拓展属性
	        /// </summary>
	        [XmlElement("extend_fields")]
	        public string ExtendFields { get; set; }
	
	        /// <summary>
	        /// 可空，默认true，用于分批控制；如果是分批下发，前几次都是false，最后一次传true
	        /// </summary>
	        [XmlElement("is_finished")]
	        public Nullable<bool> IsFinished { get; set; }
	
	        /// <summary>
	        /// ERP订单号ID
	        /// </summary>
	        [XmlElement("order_code")]
	        public string OrderCode { get; set; }
	
	        /// <summary>
	        /// 订单创建时间:格式为 yyyy-MM-dd HH:mm:ss
	        /// </summary>
	        [XmlElement("order_create_time")]
	        public string OrderCreateTime { get; set; }
	
	        /// <summary>
	        /// 订单标记以逗号分隔： 9:上门退货入库 13: 退货时是否收取发票
	        /// </summary>
	        [XmlElement("order_flag")]
	        public string OrderFlag { get; set; }
	
	        /// <summary>
	        /// 订单商品信息列表
	        /// </summary>
	        [XmlArray("order_item_list")]
	        [XmlArrayItem("stock_out_order_item")]
	        public List<StockOutOrderItemDomain> OrderItemList { get; set; }
	
	        /// <summary>
	        /// 订单来源,参考上述字段枚举值
	        /// </summary>
	        [XmlElement("order_source")]
	        public Nullable<long> OrderSource { get; set; }
	
	        /// <summary>
	        /// 操作类型：301 调拨出库单 901 普通出库单 (如货主拉走一部分货) 903 其他出库单
	        /// </summary>
	        [XmlElement("order_type")]
	        public Nullable<long> OrderType { get; set; }
	
	        /// <summary>
	        /// 自定义文本： 加工领料 委外领料 借出领用 内部领用
	        /// </summary>
	        [XmlElement("outbound_type_desc")]
	        public string OutboundTypeDesc { get; set; }
	
	        /// <summary>
	        /// 货主ID
	        /// </summary>
	        [XmlElement("owner_user_id")]
	        public string OwnerUserId { get; set; }
	
	        /// <summary>
	        /// 取件人电话
	        /// </summary>
	        [XmlElement("pick_call")]
	        public string PickCall { get; set; }
	
	        /// <summary>
	        /// 取件人证件号
	        /// </summary>
	        [XmlElement("pick_id")]
	        public string PickId { get; set; }
	
	        /// <summary>
	        /// 取件人姓名
	        /// </summary>
	        [XmlElement("pick_name")]
	        public string PickName { get; set; }
	
	        /// <summary>
	        /// 前物流订单号
	        /// </summary>
	        [XmlElement("prev_order_code")]
	        public string PrevOrderCode { get; set; }
	
	        /// <summary>
	        /// 收货方信息
	        /// </summary>
	        [XmlElement("receiver_info")]
	        public ReceiverInfoDomain ReceiverInfo { get; set; }
	
	        /// <summary>
	        /// 备注
	        /// </summary>
	        [XmlElement("remark")]
	        public string Remark { get; set; }
	
	        /// <summary>
	        /// 店铺id
	        /// </summary>
	        [XmlElement("seller_id")]
	        public Nullable<long> SellerId { get; set; }
	
	        /// <summary>
	        /// 要求出库时间 格式为 yyyy-MM-dd HH:mm:ss
	        /// </summary>
	        [XmlElement("send_time")]
	        public string SendTime { get; set; }
	
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
	        /// 出库方式(自提，非自提，销毁)
	        /// </summary>
	        [XmlElement("transport_mode")]
	        public string TransportMode { get; set; }
	
	        /// <summary>
	        /// 可空，用于分批控制；分批下发第一次留空，接下来几次都填第一次返回的wms订单
	        /// </summary>
	        [XmlElement("wlb_order_code")]
	        public string WlbOrderCode { get; set; }
}

        #endregion
    }
}
