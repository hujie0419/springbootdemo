using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api.Util;

namespace Top.Api.Request
{
    /// <summary>
    /// TOP API: alibaba.scm.external.wms.order.notify
    /// </summary>
    public class AlibabaScmExternalWmsOrderNotifyRequest : BaseTopRequest<Top.Api.Response.AlibabaScmExternalWmsOrderNotifyResponse>
    {
        /// <summary>
        /// 请求对象
        /// </summary>
        public string Request { get; set; }

        public StructDomain Request_ { set { this.Request = TopUtils.ObjectToJson(value); } } 

        #region ITopRequest Members

        public override string GetApiName()
        {
            return "alibaba.scm.external.wms.order.notify";
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
/// DeliverRequirementsDomain Data Structure.
/// </summary>
[Serializable]

public class DeliverRequirementsDomain : TopObject
{
	        /// <summary>
	        /// 配送类型： PTPS-常温配送 LLPS-冷链配送
	        /// </summary>
	        [XmlElement("delivery_type")]
	        public string DeliveryType { get; set; }
	
	        /// <summary>
	        /// 送达日期（格式为 yyyy-MM-dd HH:mm:ss)
	        /// </summary>
	        [XmlElement("schedule_day")]
	        public string ScheduleDay { get; set; }
	
	        /// <summary>
	        /// 送达结束时间（格式为 yyyy-MM-dd HH:mm:ss）
	        /// </summary>
	        [XmlElement("schedule_end")]
	        public string ScheduleEnd { get; set; }
	
	        /// <summary>
	        /// 送达开始时间（格式为 yyyy-MM-dd HH:mm:ss）
	        /// </summary>
	        [XmlElement("schedule_start")]
	        public string ScheduleStart { get; set; }
	
	        /// <summary>
	        /// 投递时延要求(1-工作日 2-节假日 101,当日达102次晨达103次日达 111 活动标  104 预约达 )
	        /// </summary>
	        [XmlElement("schedule_type")]
	        public Nullable<long> ScheduleType { get; set; }
}

	/// <summary>
/// BatchSendCtrlParamDomain Data Structure.
/// </summary>
[Serializable]

public class BatchSendCtrlParamDomain : TopObject
{
	        /// <summary>
	        /// 是否有更多商品
	        /// </summary>
	        [XmlElement("distribute_type")]
	        public Nullable<long> DistributeType { get; set; }
	
	        /// <summary>
	        /// 总共ITEM数量
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
	        /// 收件方镇
	        /// </summary>
	        [XmlElement("receive_town")]
	        public string ReceiveTown { get; set; }
	
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
	        /// 收件人手机
	        /// </summary>
	        [XmlElement("receiver_mobile")]
	        public string ReceiverMobile { get; set; }
	
	        /// <summary>
	        /// 收件人名称
	        /// </summary>
	        [XmlElement("receiver_name")]
	        public string ReceiverName { get; set; }
	
	        /// <summary>
	        /// 收件人昵称/门店联系人
	        /// </summary>
	        [XmlElement("receiver_nick")]
	        public string ReceiverNick { get; set; }
	
	        /// <summary>
	        /// 收件人电话
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
	        /// 发件方名称
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
	        /// 发件方邮编
	        /// </summary>
	        [XmlElement("sender_zip_code")]
	        public string SenderZipCode { get; set; }
}

	/// <summary>
/// ItemDetailDomain Data Structure.
/// </summary>
[Serializable]

public class ItemDetailDomain : TopObject
{
	        /// <summary>
	        /// 总金额
	        /// </summary>
	        [XmlElement("amount")]
	        public Nullable<long> Amount { get; set; }
	
	        /// <summary>
	        /// 商品名称
	        /// </summary>
	        [XmlElement("item_name")]
	        public string ItemName { get; set; }
	
	        /// <summary>
	        /// 单价
	        /// </summary>
	        [XmlElement("price")]
	        public Nullable<long> Price { get; set; }
	
	        /// <summary>
	        /// 数量
	        /// </summary>
	        [XmlElement("quantity")]
	        public Nullable<long> Quantity { get; set; }
	
	        /// <summary>
	        /// 单位
	        /// </summary>
	        [XmlElement("unit")]
	        public string Unit { get; set; }
}

	/// <summary>
/// InvoinceItemDomain Data Structure.
/// </summary>
[Serializable]

public class InvoinceItemDomain : TopObject
{
	        /// <summary>
	        /// 发票金额
	        /// </summary>
	        [XmlElement("bill_account")]
	        public string BillAccount { get; set; }
	
	        /// <summary>
	        /// 发票内容
	        /// </summary>
	        [XmlElement("bill_content")]
	        public string BillContent { get; set; }
	
	        /// <summary>
	        /// ERP发票ID
	        /// </summary>
	        [XmlElement("bill_id")]
	        public Nullable<long> BillId { get; set; }
	
	        /// <summary>
	        /// 发票抬头
	        /// </summary>
	        [XmlElement("bill_title")]
	        public string BillTitle { get; set; }
	
	        /// <summary>
	        /// 发票类型(增值税普通发票)
	        /// </summary>
	        [XmlElement("bill_type")]
	        public string BillType { get; set; }
	
	        /// <summary>
	        /// 票明细列表
	        /// </summary>
	        [XmlArray("detail_list")]
	        [XmlArrayItem("item_detail")]
	        public List<ItemDetailDomain> DetailList { get; set; }
}

	/// <summary>
/// ConsignOrderItemDomain Data Structure.
/// </summary>
[Serializable]

public class ConsignOrderItemDomain : TopObject
{
	        /// <summary>
	        /// 商品实际价格（以分为单位）
	        /// </summary>
	        [XmlElement("actual_price")]
	        public Nullable<long> ActualPrice { get; set; }
	
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
	        /// 商品优惠金额（以分为单位）
	        /// </summary>
	        [XmlElement("discount_amount")]
	        public Nullable<long> DiscountAmount { get; set; }
	
	        /// <summary>
	        /// 库存类型(1 正品 101 残次 102 机损 103 箱损 201 冻结库存 301 在途库存 )
	        /// </summary>
	        [XmlElement("inventory_type")]
	        public Nullable<long> InventoryType { get; set; }
	
	        /// <summary>
	        /// 商家对商品的编码
	        /// </summary>
	        [XmlElement("item_code")]
	        public string ItemCode { get; set; }
	
	        /// <summary>
	        /// 交易平台商品编码
	        /// </summary>
	        [XmlElement("item_ext_code")]
	        public string ItemExtCode { get; set; }
	
	        /// <summary>
	        /// 后端商品编号，商品关联以此为准
	        /// </summary>
	        [XmlElement("item_id")]
	        public string ItemId { get; set; }
	
	        /// <summary>
	        /// 商品名称
	        /// </summary>
	        [XmlElement("item_name")]
	        public string ItemName { get; set; }
	
	        /// <summary>
	        /// 商品销售价格 （以分为单位）
	        /// </summary>
	        [XmlElement("item_price")]
	        public Nullable<long> ItemPrice { get; set; }
	
	        /// <summary>
	        /// 商品数量
	        /// </summary>
	        [XmlElement("item_quantity")]
	        public Nullable<long> ItemQuantity { get; set; }
	
	        /// <summary>
	        /// 商品类型 0 实物商品 1 服务商品 （服务商品在仓库回告中需要排除掉）
	        /// </summary>
	        [XmlElement("item_type")]
	        public Nullable<long> ItemType { get; set; }
	
	        /// <summary>
	        /// 商品版本
	        /// </summary>
	        [XmlElement("item_version")]
	        public Nullable<long> ItemVersion { get; set; }
	
	        /// <summary>
	        /// 订单商品ID （回告时，需要带回， scm通过此字段关联子发货单记录）
	        /// </summary>
	        [XmlElement("order_item_id")]
	        public string OrderItemId { get; set; }
	
	        /// <summary>
	        /// 交易编码
	        /// </summary>
	        [XmlElement("order_source_code")]
	        public string OrderSourceCode { get; set; }
	
	        /// <summary>
	        /// 是否交易订单 是交易的订单：0  非交易的：1
	        /// </summary>
	        [XmlElement("order_type_flag")]
	        public string OrderTypeFlag { get; set; }
	
	        /// <summary>
	        /// 货主ID  代销情况下货主ID和卖家ID不同
	        /// </summary>
	        [XmlElement("owner_user_id")]
	        public string OwnerUserId { get; set; }
	
	        /// <summary>
	        /// 货主名称
	        /// </summary>
	        [XmlElement("owner_user_name")]
	        public string OwnerUserName { get; set; }
	
	        /// <summary>
	        /// 子交易编码
	        /// </summary>
	        [XmlElement("sub_source_code")]
	        public string SubSourceCode { get; set; }
	
	        /// <summary>
	        /// 卖家ID  一般情况下，货主ID和卖家ID相同
	        /// </summary>
	        [XmlElement("user_id")]
	        public string UserId { get; set; }
	
	        /// <summary>
	        /// 卖家名称
	        /// </summary>
	        [XmlElement("user_name")]
	        public string UserName { get; set; }
}

	/// <summary>
/// PrintListDomain Data Structure.
/// </summary>
[Serializable]

public class PrintListDomain : TopObject
{
	        /// <summary>
	        /// 配送中心
	        /// </summary>
	        [XmlElement("consign_center")]
	        public string ConsignCenter { get; set; }
	
	        /// <summary>
	        /// 配送快递
	        /// </summary>
	        [XmlElement("consign_deliver")]
	        public string ConsignDeliver { get; set; }
	
	        /// <summary>
	        /// 派送要求
	        /// </summary>
	        [XmlElement("consign_need")]
	        public string ConsignNeed { get; set; }
	
	        /// <summary>
	        /// 是否送货前通知
	        /// </summary>
	        [XmlElement("consign_notify")]
	        public string ConsignNotify { get; set; }
	
	        /// <summary>
	        /// 分拣代码
	        /// </summary>
	        [XmlElement("consign_pick")]
	        public string ConsignPick { get; set; }
	
	        /// <summary>
	        /// 配送站点
	        /// </summary>
	        [XmlElement("consign_sit")]
	        public string ConsignSit { get; set; }
	
	        /// <summary>
	        /// 送货时间
	        /// </summary>
	        [XmlElement("consign_time")]
	        public string ConsignTime { get; set; }
	
	        /// <summary>
	        /// 出库交接时间
	        /// </summary>
	        [XmlElement("deliver_time")]
	        public string DeliverTime { get; set; }
	
	        /// <summary>
	        /// 作业号
	        /// </summary>
	        [XmlElement("job_no")]
	        public string JobNo { get; set; }
	
	        /// <summary>
	        /// 支付方式
	        /// </summary>
	        [XmlElement("payment_mode")]
	        public string PaymentMode { get; set; }
	
	        /// <summary>
	        /// 退换货地址
	        /// </summary>
	        [XmlElement("return_address")]
	        public string ReturnAddress { get; set; }
	
	        /// <summary>
	        /// 退换货电话
	        /// </summary>
	        [XmlElement("return_phone")]
	        public string ReturnPhone { get; set; }
	
	        /// <summary>
	        /// 退换货具体政策提醒
	        /// </summary>
	        [XmlElement("return_policy")]
	        public string ReturnPolicy { get; set; }
	
	        /// <summary>
	        /// 退换货收件人
	        /// </summary>
	        [XmlElement("return_receiver")]
	        public string ReturnReceiver { get; set; }
	
	        /// <summary>
	        /// 退换货提醒
	        /// </summary>
	        [XmlElement("return_remind")]
	        public string ReturnRemind { get; set; }
	
	        /// <summary>
	        /// 退换货邮编
	        /// </summary>
	        [XmlElement("return_zip")]
	        public string ReturnZip { get; set; }
	
	        /// <summary>
	        /// 发货清单标题
	        /// </summary>
	        [XmlElement("title")]
	        public string Title { get; set; }
}

	/// <summary>
/// StructDomain Data Structure.
/// </summary>
[Serializable]

public class StructDomain : TopObject
{
	        /// <summary>
	        /// AE自营前端时效标
	        /// </summary>
	        [XmlElement("ae_self_time_type")]
	        public string AeSelfTimeType { get; set; }
	
	        /// <summary>
	        /// 支付宝交易号
	        /// </summary>
	        [XmlElement("alipay_no")]
	        public string AlipayNo { get; set; }
	
	        /// <summary>
	        /// 订单应收金额，消费者还需要付多少钱 以分为单位
	        /// </summary>
	        [XmlElement("ar_amount")]
	        public Nullable<long> ArAmount { get; set; }
	
	        /// <summary>
	        /// 分批下发控制参数
	        /// </summary>
	        [XmlElement("batch_send_ctrl_param")]
	        public BatchSendCtrlParamDomain BatchSendCtrlParam { get; set; }
	
	        /// <summary>
	        /// 帐套类型：系统交互使用 下发之后，需要在回告中带回来
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
	        /// 配送要求
	        /// </summary>
	        [XmlElement("deliver_requirements")]
	        public DeliverRequirementsDomain DeliverRequirements { get; set; }
	
	        /// <summary>
	        /// 订单优惠金额 以分为单位
	        /// </summary>
	        [XmlElement("discount_amount")]
	        public Nullable<long> DiscountAmount { get; set; }
	
	        /// <summary>
	        /// 分销附带信息
	        /// </summary>
	        [XmlElement("distributor_user_nick")]
	        public string DistributorUserNick { get; set; }
	
	        /// <summary>
	        /// 扩展属性 可能存在的扩展属性：车型: extendFields.etCarType    车牌号: extendFields.etPlateNumber    车主电话: extendFields.mobile    车主姓名: extendFields.etConsignee    换货单编号：extendFields.exchangeCode  默认为：none
	        /// </summary>
	        [XmlElement("extend_fields")]
	        public string ExtendFields { get; set; }
	
	        /// <summary>
	        /// 订单已收金额，消费者已经支付多少钱 以分为单位
	        /// </summary>
	        [XmlElement("got_amount")]
	        public Nullable<long> GotAmount { get; set; }
	
	        /// <summary>
	        /// 发票信息列表
	        /// </summary>
	        [XmlArray("invoice_info_list")]
	        [XmlArrayItem("invoince_item")]
	        public List<InvoinceItemDomain> InvoiceInfoList { get; set; }
	
	        /// <summary>
	        /// 订单金额（=总商品金额-订单优惠金额+快递费用）以分为单位
	        /// </summary>
	        [XmlElement("order_amount")]
	        public Nullable<long> OrderAmount { get; set; }
	
	        /// <summary>
	        /// 阿里Scm 发货单编码 Scm通过此字段关联数据，需要在回告的内容中带回来
	        /// </summary>
	        [XmlElement("order_code")]
	        public string OrderCode { get; set; }
	
	        /// <summary>
	        /// 订单创建时间 格式为 yyyy-MM-dd HH:mm:ss
	        /// </summary>
	        [XmlElement("order_create_time")]
	        public string OrderCreateTime { get; set; }
	
	        /// <summary>
	        /// 订单审核时间 格式为 yyyy-MM-dd HH:mm:ss
	        /// </summary>
	        [XmlElement("order_examination_time")]
	        public string OrderExaminationTime { get; set; }
	
	        /// <summary>
	        /// 订单商品列表
	        /// </summary>
	        [XmlArray("order_item_list")]
	        [XmlArrayItem("consign_order_item")]
	        public List<ConsignOrderItemDomain> OrderItemList { get; set; }
	
	        /// <summary>
	        /// 订单支付时间 格式为 yyyy-MM-dd HH:mm:ss
	        /// </summary>
	        [XmlElement("order_pay_time")]
	        public string OrderPayTime { get; set; }
	
	        /// <summary>
	        /// 订单优先级(0 -10，根据订单作业优先级设置，数字越大，优先级越高)
	        /// </summary>
	        [XmlElement("order_priority")]
	        public Nullable<long> OrderPriority { get; set; }
	
	        /// <summary>
	        /// 前台交易创建时间 格式为 yyyy-MM-dd HH:mm:ss
	        /// </summary>
	        [XmlElement("order_shop_create_time")]
	        public string OrderShopCreateTime { get; set; }
	
	        /// <summary>
	        /// 订单来源 201 淘宝，214京东 228 其他 230 天猫 （一般交易出库 230 换货出库228）
	        /// </summary>
	        [XmlElement("order_source")]
	        public Nullable<long> OrderSource { get; set; }
	
	        /// <summary>
	        /// 交易内部来源(文本透传 WAP(手机);HITAO(嗨淘);TOP(TOP平台);TAOBAO(普通淘宝);JHS(聚划算))
	        /// </summary>
	        [XmlElement("order_sub_source")]
	        public string OrderSubSource { get; set; }
	
	        /// <summary>
	        /// 订单类型 201交易出库单 固定值201
	        /// </summary>
	        [XmlElement("order_type")]
	        public Nullable<long> OrderType { get; set; }
	
	        /// <summary>
	        /// 阿里交易订单编号
	        /// </summary>
	        [XmlElement("outer_biz_order_id")]
	        public string OuterBizOrderId { get; set; }
	
	        /// <summary>
	        /// 货主编号 代销情况下货主ID和卖家ID不同
	        /// </summary>
	        [XmlElement("owner_user_id")]
	        public string OwnerUserId { get; set; }
	
	        /// <summary>
	        /// 取件人电话
	        /// </summary>
	        [XmlElement("pick_call")]
	        public string PickCall { get; set; }
	
	        /// <summary>
	        /// 取件人身份证
	        /// </summary>
	        [XmlElement("pick_id")]
	        public string PickId { get; set; }
	
	        /// <summary>
	        /// 取件人姓名
	        /// </summary>
	        [XmlElement("pick_name")]
	        public string PickName { get; set; }
	
	        /// <summary>
	        /// 快递费用 以分为单位
	        /// </summary>
	        [XmlElement("postfee")]
	        public Nullable<long> Postfee { get; set; }
	
	        /// <summary>
	        /// 前物流订单号
	        /// </summary>
	        [XmlElement("prev_order_code")]
	        public string PrevOrderCode { get; set; }
	
	        /// <summary>
	        /// 发货清单列表
	        /// </summary>
	        [XmlArray("print_list_info")]
	        [XmlArrayItem("print_list")]
	        public List<PrintListDomain> PrintListInfo { get; set; }
	
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
	        /// 卖家编号：系统交互使用 下发之后，需要在回告中带回来
	        /// </summary>
	        [XmlElement("seller_id")]
	        public Nullable<long> SellerId { get; set; }
	
	        /// <summary>
	        /// 发货方信息
	        /// </summary>
	        [XmlElement("sender_info")]
	        public SenderInfoDomain SenderInfo { get; set; }
	
	        /// <summary>
	        /// 服务费
	        /// </summary>
	        [XmlElement("service_fee")]
	        public Nullable<long> ServiceFee { get; set; }
	
	        /// <summary>
	        /// 仓库编码
	        /// </summary>
	        [XmlElement("store_code")]
	        public string StoreCode { get; set; }
	
	        /// <summary>
	        /// 配送编码
	        /// </summary>
	        [XmlElement("tms_service_code")]
	        public string TmsServiceCode { get; set; }
	
	        /// <summary>
	        /// 配送公司名称
	        /// </summary>
	        [XmlElement("tms_service_name")]
	        public string TmsServiceName { get; set; }
	
	        /// <summary>
	        /// 出库方式(自提，非自提，销毁)
	        /// </summary>
	        [XmlElement("transport_mode")]
	        public string TransportMode { get; set; }
	
	        /// <summary>
	        /// 卖家编号
	        /// </summary>
	        [XmlElement("user_id")]
	        public string UserId { get; set; }
}

        #endregion
    }
}
