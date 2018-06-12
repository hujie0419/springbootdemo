using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qpl.Api.Models
{
    public class QcjOrderModel
    {
        public QcjOrderInfo OrderInfo { get; set; }

        //public string OrderDeliverysInfo { get; set; }

        public QcjInvoicesInfo OrderInvoicesInfo { get; set; }

        /// <summary>
        /// 订单明细信息
        /// </summary>
        public List<QcjOrderItemInfo> OrderItemsInfo { get; set; }
    }

    public class QcjOrderItemInfo {
        public string UID { get; set; }


        public string OrderId { get; set; }

        public string OrderNo { get; set; }

        public string ProductName { get; set; }

        public int ProductSourceRequirement { get; set; }

        public string BrandName { get; set; }

        public int ItemType { get; set; }

        public string Model { get; set; }

        public double Price { get; set; }

        public decimal CostPrice { get; set; }

        public int Amount { get; set; }

        public string OENo { get; set; }

        public string UserSKU { get; set; }

        public int IsSpot { get; set; }

        public int IsWarranty { get; set; }

        public string SpotDesc { get; set; }

        public string Remark { get; set; }

        public string ThPid { get; set; }
    }

    public class QcjOrderInfo
    {
        [Key]
        public int IID { get; set; }
        /// <summary>
        /// 订单仓库ID
        /// </summary>
        public int? RepertoryId { get; set; }
        /// <summary>
        /// 订单仓库Name
        /// </summary>
        public string RepositoryName { get; set; }
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
        public string ProvinceName { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        [StringLength(50, ErrorMessage = "最长{0}个字符")]
        [Display(Name = "城市")]
        public string CityName { get; set; }

        /// <summary>
        /// 乡镇
        /// </summary>
        [StringLength(50, ErrorMessage = "最长{0}个字符")]
        [Display(Name = "乡镇")]
        public string TownName { get; set; }

        /// <summary>
        /// 街道
        /// </summary>
        [StringLength(50, ErrorMessage = "最长{0}个字符")]
        [Display(Name = "街道")]
        public string StreetName { get; set; }

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
        public string Phone { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

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
        public string Remark { get; set; }

        public DateTime OrderCreateTime { get; set; }

        public DateTime OrderUpdateTime { get; set; }

        public string ShopName { get; set; }

        public double ProductPrice { get; set; }

        public double DeliverPrice { get; set; }

        public int DeliverPayMethod { get; set; }

        public int MyProperty { get; set; }
    }

    public class QcjInvoicesInfo {
        public string UID { get; set; }

        public string OrderUID { get; set; }

        public int InvoiceType { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 识别码
        /// </summary>
        public string TaxCode { get; set; }
        /// <summary>
        /// 注册地址
        /// </summary>
        public string RegAddress { get; set; }
        /// <summary>
        /// 注册电话
        /// </summary>
        public string RegPhone { get; set; }
        /// <summary>
        /// 开户行
        /// </summary>
        public string Bank { get; set; }

        public string BankAccount { get; set; }

        public string InvoiceCode { get; set; }
    }
}
