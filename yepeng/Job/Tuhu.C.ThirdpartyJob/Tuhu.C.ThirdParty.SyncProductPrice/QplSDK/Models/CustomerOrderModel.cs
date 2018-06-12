using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qpl.Api.Models
{
	/// <summary>
	/// 订单模型
	/// <remarks>
	/// 创建：2015.05.19
	/// 修改：2015.05.19
	/// </remarks>
	/// </summary>
	public class CustomerOrderModel
	{
		[Key]
		public int IID { get; set; }

		/// <summary>
		/// 订单唯一id
		/// </summary>
		[Required(ErrorMessage = "必填")]
		[StringLength(50, MinimumLength = 4, ErrorMessage = "{1}到{0}个字符")]
		[Display(Name = "订单唯一id")]
		public string UID { get; set; }

		/// <summary>
		/// 用户编号
		/// </summary>
		[Required(ErrorMessage = "必填")]
		[StringLength(50, MinimumLength = 1, ErrorMessage = "{1}到{0}个字符")]
		[Display(Name = "用户编号")]
		public string UIID { get; set; }

		/// <summary>
		/// 订单编号
		/// </summary>
		[Required(ErrorMessage = "必填")]
		[StringLength(20, MinimumLength = 1, ErrorMessage = "{1}到{0}个字符")]
		[Display(Name = "订单编号")]
		public string PIID { get; set; }

		/// <summary>
		/// 订单编号
		/// </summary>
		[Required(ErrorMessage = "必填")]
		[StringLength(20, MinimumLength = 1, ErrorMessage = "{1}到{0}个字符")]
		[Display(Name = "订单编号")]
		public string OrderNo { get; set; }

		/// <summary>
		/// 订单编号
		/// </summary>
		[Required(ErrorMessage = "必填")]
		[StringLength(20, MinimumLength = 1, ErrorMessage = "{1}到{0}个字符")]
		[Display(Name = "订单编号")]
		public string ManagerId { get; set; }

		/// <summary>
		/// 商品列表价格合计
		/// </summary>
		//[StringLength(18, ErrorMessage = "最长{0}个数字")]//Length can only be configured for String and Byte array properties.
		[Display(Name = "商品列表价格合计")]
		public double TotalListPrice { get; set; }

		/// <summary>
		/// 优惠价
		/// </summary>
		//[StringLength(18, ErrorMessage = "最长{0}个数字")]
		[Display(Name = "优惠价")]
		public double PreferentialPrice { get; set; }

		/// <summary>
		/// 优惠券id
		/// </summary>
		[Required(ErrorMessage = "必填")]
		[StringLength(20, MinimumLength = 1, ErrorMessage = "{1}到{0}个字符")]
		[Display(Name = "优惠券id")]
		public string CouponId { get; set; }

		/// <summary>
		/// 优惠券金额
		/// </summary>
		//[StringLength(18, ErrorMessage = "最长{0}个数字")]
		[Display(Name = "优惠券金额")]
		public double CouponValue { get; set; }

		/// <summary>
		/// 最终原始金额（商品价格+运费-优惠券-现金券）
		/// </summary>
		//[StringLength(18, ErrorMessage = "最长{0}个数字")]
		[Display(Name = "最终原始金额")]
		public double TotalOriginalPrice { get; set; }

		/// <summary>
		/// 最终支付金额（销售修改后的最终原始金额）
		/// </summary>
		//[StringLength(18, ErrorMessage = "最长{0}个数字")]
		[Display(Name = "最终支付金额")]
		public double TotalFinalPrice { get; set; }

		/// <summary>
		/// 支付方式 1在线支付 2货到付款
		/// </summary>
		[Display(Name = "支付方式")]
		public int PaymentMethod { get; set; }

		/// <summary>
		/// 省份
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "省份")]
		public string Province { get; set; }

		/// <summary>
		/// 城市
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "城市")]
		public string City { get; set; }

		/// <summary>
		/// 乡镇
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "乡镇")]
		public string Town { get; set; }

		/// <summary>
		/// 街道
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "街道")]
		public string Street { get; set; }

		/// <summary>
		/// 地址
		/// </summary>
		[StringLength(128, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "地址")]
		public string Address { get; set; }

		/// <summary>
		/// 联系人
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "联系人")]
		public string Contacts { get; set; }

		/// <summary>
		/// 手机号
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "手机号")]
		public string ContactsPhone { get; set; }

		/// <summary>
		/// 订单状态=1：待付款；2：待发货；3：待收货；4：待评价；5：退款申请；6：退款审核；7：退款成功；8：关闭；
		/// </summary>
		[Display(Name = "订单状态")]
		public int OrderStatus { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "创建时间")]
		public string CreatedDate { get; set; }

		/// <summary>
		/// 更新时间
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "更新时间")]
		public string UpdateDate { get; set; }

		/// <summary>
		/// 支付方式 1支付宝
		/// </summary>
		[Display(Name = "支付方式")]
		public int PurchaseWay { get; set; }

		/// <summary>
		/// 支付时间
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "支付时间")]
		public string PurchaseDate { get; set; }

		/// <summary>
		/// 支付状态
		/// </summary>
		[Display(Name = "支付状态")]
		public int PurchaseState { get; set; }

		/// <summary>
		/// 收货时间
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "收货时间")]
		public string ReceivedDate { get; set; }

		/// <summary>
		/// 结算时间
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "结算时间")]
		public string BillingDate { get; set; }

		/// <summary>
		/// 父订单ID
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "父订单ID")]
		public string CustomerOrderId { get; set; }

		/// <summary>
		/// 购买者编号
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "购买者编号")]
		public string UserId { get; set; }

		/// <summary>
		/// 用户名
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "用户名")]
		public string UserName { get; set; }

		/// <summary>
		/// 用户手机号
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "用户手机号")]
		public string UserPhone { get; set; }

		/// <summary>
		/// 经销商ID
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "经销商ID")]
		public string SalesId { get; set; }

		/// <summary>
		/// 经销商名字
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "经")]
		public string SalesName { get; set; }

		/// <summary>
		/// 经销商电话
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "经销商电话")]
		public string SalesPhone { get; set; }

		#region 快递信息
		/// <summary>
		/// 配送类型 0快递配送 1商家配送
		/// </summary>
		[Display(Name = "配送类型")]
		public int DeliveryType { get; set; }

		/// <summary>
		/// 发货时间
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "发货时间")]
		public string DeliveryDate { get; set; }

		/// <summary>
		/// 配送人员
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "配送人员")]
		public string DeliveryUser { get; set; }

		/// <summary>
		/// 快递单号
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "快递单号")]
		public string DeliveryCode { get; set; }

		/// <summary>
		/// 快递留言
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "快递留言")]
		public string DeliveryMessage { get; set; }

		/// <summary>
		/// 快递金额
		/// </summary>
		//[StringLength(18, ErrorMessage = "最长{0}个数字")]
		[Display(Name = "快递金额")]
		public double DelivertyValue { get; set; }
		#endregion

		/// <summary>
		/// 商家备注
		/// </summary>
		[StringLength(500, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "商家备注")]
		public string SalesNotes { get; set; }

		/// <summary>
		/// 发票类型 0不开票 1开票
		/// </summary>
		[Display(Name = "发票类型")]
		public int InvoiceType { get; set; }

		/// <summary>
		/// 抬头
		/// </summary>
		[StringLength(512, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "发票抬头")]
		public string InvoiceTitle { get; set; }

		/// <summary>
		/// 是什么终端下的订单 1 手机端 2网站端
		/// </summary>
		[Display(Name = "平台类型")]
		public int PlantformType { get; set; }

		/// <summary>
		/// 现金券ID
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "现金券ID")]
		public string CrashCouponId { get; set; }

		/// <summary>
		/// 现金券金额
		/// </summary>
		//[StringLength(18, ErrorMessage = "最长{0}个数字")]
		[Display(Name = "现金券金额")]
		public double CrashCouponValue { get; set; }

		/// <summary>
		/// 评价时间
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "评价时间")]
		public string FeedbackDate { get; set; }

		/// <summary>
		/// 评价内容
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "评价内容")]
		public string FeedbackContent { get; set; }

		/// <summary>
		/// 描述相符得分
		/// </summary>
		//[StringLength(18, ErrorMessage = "最长{0}个数字")]
		[Display(Name = "描述相符得分")]
		public double DescriptionValue { get; set; }

		/// <summary>
		/// 服务态度得分
		/// </summary>
		//[StringLength(18, ErrorMessage = "最长{0}个数字")]
		[Display(Name = "服务态度得分")]
		public double AttitudeValue { get; set; }

		/// <summary>
		/// 配送速度
		/// </summary>
		//[StringLength(18, ErrorMessage = "最长{0}个数字")]
		[Display(Name = "配送速度")]
		public double DeliveryValue { get; set; }

		/// <summary>
		/// 物流服务
		/// </summary>
		//[StringLength(18, ErrorMessage = "最长{0}个数字")]
		[Display(Name = "物流服务")]
		public double LogisticsValue { get; set; }

		/// <summary>
		/// 结算编号
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "结算编号")]
		public string POID { get; set; }

		/// <summary>
		/// 是否结算
		/// </summary>
		[Display(Name = "是否结算")]
		public int IsBilling { get; set; }

		/// <summary>
		/// 订单价格修改人
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "订单价格修改人")]
		public string ModifyPriceUserId { get; set; }

		/// <summary>
		/// 订单价格修改人姓名
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "订单价格修改人姓名")]
		public string ModifyPriceUserName { get; set; }

		/// <summary>
		/// 订单价格修改时间
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "订单价格修改时间")]
		public string ModifyPriceDatetime { get; set; }

		/// <summary>
		/// 订单价格修改原因
		/// </summary>
		[StringLength(50, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "订单价格修改原因")]
		public string ModifyPriceReason { get; set; }

		/// <summary>
		/// 买家备注
		/// </summary>
		[StringLength(500, ErrorMessage = "最长{0}个字符")]
		[Display(Name = "买家备注")]
		public string Notes { get; set; }

		/// <summary>
		/// 订单明细信息
		/// </summary>
		public List<CustomerOrderItemModel> OrderItems { get; set; }

        //public TraderInfoModel SalesInfo { get; set; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        public int? RepertoryId { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string RepositoryName { get; set; }
        
        /// <summary>
        ///  1 是普通订单 2 大单抛 3 途虎门店订单 4 DKZ订单 
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// 占仓状态 0 未占仓 1成功 2 失败
        /// </summary>
        public string OccupyRepository { get; set; }
    }
}
