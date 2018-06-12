using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Tuhu.Models;

namespace Tuhu.Service.Activity.Models.Requests
{
    public class CreateOrderRequest
    {
        public CreateOrderRequest()
        {
            //Status = "0New";
            OrderType = "1普通";
            OrderChannel = "1网站";
            //BookPeriod = "15:00";

            var now = DateTime.Now;
            //OrderDatetime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
            //BookDateTime = new DateTime(now.Year, now.Month, now.Day);
            //if (OrderDatetime.Hour > 11)
            //    BookDateTime = BookDateTime.Value.AddDays(1);
        }
        /// <summary>
        /// 订单来源
        /// </summary>
        /// <summary>订单安装预约发出日期</summary>

        /// <summary>订单安装预约发出时间</summary>

        /// <summary>CaseID</summary>

        /// <summary>订单渠道</summary>
        public string OrderChannel { get; set; }

        /// <summary>订单创建时间</summary>

        /// <summary>订单ID</summary>

        /// <summary>订单编号</summary>

        /// <summary>订单类型</summary>
        public string OrderType { get; set; }

        /// <summary>订单主人</summary>

        /// <summary>外联单号</summary>
        public string RefNo { get; set; }

        /// <summary>订单备注</summary>
        public string Remark { get; set; }
        /// <summary>
        /// 2017 03 21新增字段
        /// 系统备注信息：将原来的备注信息分为用户备注和系统备注
        /// </summary>

        /// <summary>订单状态</summary>

        /// <summary>订单提交人</summary>

        /// <summary>
        /// 订单仓库ID
        /// </summary>

        /// <summary>
        /// 订单仓库名字
        /// </summary>

        /// <summary>订单订购产品总数量</summary>
        [Display(Name = "商品数量")]
        [Range(1, int.MaxValue, ErrorMessage = "{0}最少为{1}")]
        public int SumNum { get; set; }

        /// <summary>订单商品</summary>
        [Display(Name = "商品数量")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ValidateCollection]
        public IEnumerable<OrderItem> Items { get; set; }
        /// <summary>设备号</summary>
        public string DeviceID { get; set; }

        /// <summary>物流</summary>
        [Display(Name = "物流方式")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ValidateObject]
        public OrderDelivery Delivery { get; set; }

        /// <summary>发票</summary>

        /// <summary>支付信息</summary>
        [Display(Name = "支付信息")]
        [Required(ErrorMessage = "{0}不能为空")]
        public OrderPayment Payment { get; set; }

        /// <summary>订单金额</summary>
        [Display(Name = "订单金额")]
        [Required(ErrorMessage = "{0}不能为空")]
        public OrderMoney Money { get; set; }

        /// <summary>订单车型</summary>
        public OrderCarModel Car { get; set; }

        /// <summary>客户信息</summary>
        [Display(Name = "客户信息")]
        [Required(ErrorMessage = "{0}不能为空")]
        public OrderCustomer Customer { get; set; }

        /// <summary>优惠券</summary>
        public long? PromotionCodeId { get; set; }
    }

    public class OrderCustomer
    {
        /// <summary>客户编号</summary>
        public Guid UserId { get; set; }

        /// <summary>客户名称</summary>
        public string UserName { get; set; }

        /// <summary>客户电话</summary>
        public string UserTel { get; set; }
    }

    public class OrderDelivery
    {
        /// <summary>收货地址</summary>
        [ValidateObject]
        public OrderAddressModel Address { get; set; }

        /// <summary>配送状态</summary>
        public string DeliveryStatus { get; set; }

        /// <summary>配送类型</summary>
        [Display(Name = "配送类型")]
        [Required]
        public string DeliveryType { get; set; }

        /// <summary>收客户安装费</summary>

        /// <summary>安装门店</summary>
        public string InstallShop { get; set; }

        /// <summary>安装门店ID</summary>
        public int? InstallShopId { get; set; }

        /// <summary>安装状态</summary>
        public string InstallStatus { get; set; }

        /// <summary>安装类型</summary>
        public string InstallType { get; set; }
    }


    public class OrderMoney
    {
        /// <summary>收客户运费</summary>
        public decimal? ShippingMoney { get; set; }

        /// <summary>总扣价</summary>
        public decimal SumDisMoney { get; set; }

        /// <summary>市场</summary>
        public decimal SumMarkedMoney { get; set; }

        /// <summary>总价格</summary>
        public decimal SumMoney { get; set; }
    }

    public class OrderPayment
    {
        /// <summary>付款方式</summary>
        public string PayMothed { get; set; }

        /// <summary>付款状态</summary>
        public string PayStatus { get; set; }

        /// <summary>支付时间</summary>

        /// <summary>付款渠道</summary>
        public string PaymentType { get; set; }

        /// <summary>已付款金额</summary>
        public decimal? SumPaid { get; set; }
    }

    public class OrderItem
    {
        /// <summary>商品类别</summary>
        public string Category { get; set; }

        /// <summary>成本</summary>
        public decimal Cost { get; set; }

        /// <summary>优惠价格</summary>
        [JsonIgnore, IgnoreDataMember]
        public decimal Discount => MarkedPrice < Price ? 0 : MarkedPrice - Price;

        /// <summary>服务PID</summary>
        public string Fupid { get; set; }


        /// <summary>市场价</summary>
        public decimal MarkedPrice { get; set; }

        /// <summary>售价</summary>

        /// <summary>商品名称</summary>
        public string Name { get; set; }

        /// <summary>数量</summary>
        [Display(Name = "商品数量")]
        [Range(1, int.MaxValue, ErrorMessage = "{0}不能少于{1}")]
        public int Num { get; set; }

        /// <summary>PID</summary>
        public string Pid { get; set; }

        /// <summary>单价</summary>
        [Display(Name = "商品价格")]
        [Range(0.0, int.MaxValue, ErrorMessage = "{0}不能少于{1}")]
        public decimal Price { get; set; }

        /// <summary>产品类型</summary>
        public OrderProductTypes? ProductType { get; set; }

        /// <summary>采购状态</summary>

        /// <summary>外联商品ID</summary>

        /// <summary>备注</summary>
        public string Remark { get; set; }

        /// <summary>尺寸</summary>
        public string Size { get; set; }

        /// <summary>此产品是否使用优惠券</summary>
        public bool UsePromotionCode { get; set; }

        /// <summary>活动ID</summary>
        public Guid? ActivityId { get; set; }

        /// <summary>些赠品由哪些产品匹配到的</summary>

        /// <summary>扩展信息</summary>
        public OrderListExtModel ExtInfo { get; set; }

        /// <summary>套装产品</summary>
        public IEnumerable<OrderItem> PackageItems { get; set; }

        /// <summary>产品服务组ID。结合Fupid精确定位服务关系</summary>
        public Guid? ServiceGroupId { get; set; }

        public static OrderProductTypes? GetProductType(string nodeNo)
        {
            if (nodeNo == null)
                return null;

            if (nodeNo.StartsWith("1.") || nodeNo.StartsWith("15419."))
                return OrderProductTypes.Tire;

            if (nodeNo.StartsWith("28656."))
                return OrderProductTypes.BaoYang;

            if (nodeNo.StartsWith("28656."))
                return OrderProductTypes.BaoYang;

            if (nodeNo.StartsWith("28349."))
                return OrderProductTypes.AutoProduct;

            return OrderProductTypes.None;
        }
    }

    [Flags]
    public enum OrderProductTypes
    {
        /// <summary>普通</summary>
        None = 0,
        /// <summary>轮胎轮毂</summary>
        Tire = 1,
        /// <summary>保养产品</summary>
        BaoYang = 2,
        /// <summary>车品</summary>
        AutoProduct = 4,
        /// <summary>美容</summary>
        Beauty = 8,
        /// <summary>赠品</summary>
        Gifts = 16,
        /// <summary>套装产品</summary>
        Package = 32,
        /// <summary>促销产品</summary>
        Promotion = 64
    }
    public class OrderCarModel : BaseModel
    {
        /// <summary>
        /// PKID
        /// </summary>
        /// <summary>
        /// 订单号
        /// </summary>
        /// <summary>
        /// 车型编号
        /// </summary>
		public string VehicleId { get; set; }
        /// <summary>
        /// 车型名称
        /// </summary>
		public string Vehicle { get; set; }
        /// <summary>
        /// 品牌名称
        /// </summary>
		public string Brand { get; set; }
        /// <summary>
        /// 排量
        /// </summary>
		public string PaiLiang { get; set; }
        /// <summary>
        /// 生产年份
        /// </summary>
		public string Nian { get; set; }
        /// <summary>
        /// 生产年份子款型名称（例如：“2010款 2.0T 双离合 Quattro 四驱 越野版”）
        /// </summary>
		public string SalesName { get; set; }
        /// <summary>
        /// 力洋编号
        /// </summary>
		public string LiYangId { get; set; }
        /// <summary>
        /// 嘉之道编号
        /// </summary>
		public string Tid { get; set; }
        /// <summary>
        /// VIN码
        /// </summary>
		public string VinCode { get; set; }
        /// <summary>
        /// 车牌
        /// </summary>
		public string PlateNumber { get; set; }
        /// <summary>
        /// 扩展列
        /// </summary>
		[Column("ExtCol")]
        public IDictionary<string, object> ExtCol { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        /// <summary>
        /// 最后一次更新时间
        /// </summary>
        /// <summary>
        /// 行驶里程
        /// </summary>
		public int? Distance { get; set; }
        /// <summary>
        /// 上路月份
        /// </summary>
		public int? OnRoadMonth { get; set; }
        /// <summary>
        /// 上路年份
        /// </summary>
		public int? OnRoadYear { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 车型描述
        /// </summary>
        public string CarTypeDescription { get; set; }


    }
    public class OrderListExtModel : BaseModel
    {
        public int InstallShopId { get; set; }
        public string InstallShop { get; set; }
        public OrderCarModel Car { get; set; }
    }

    public class OrderAddressModel : BaseModel
    {
        [Display(Name = "收货人")]
        [Required(ErrorMessage = "{0}不能为空。")]
        [StringLength(20, ErrorMessage = "{0}最大长度为{1}。")]
        public string ConsigneeName { get; set; }

        [Display(Name = "手机号")]
        [RegularExpression(@"1\d{10}", ErrorMessage = "{0}格式错误。")]
        public string Cellphone { get; set; }

        [Display(Name = "电话号码")]
        [StringLength(20, ErrorMessage = "{0}最大长度为{1}。")]
        public string Telphone { get; set; }

        public int ProvinceId { get; set; }
        [Display(Name = "省/直辖市")]
        [Required(ErrorMessage = "{0}不能为空。")]
        [StringLength(10, ErrorMessage = "{0}最大长度为{1}。")]
        public string Province { get; set; }

        public int CityId { get; set; }
        [Display(Name = "市/区")]
        [Required(ErrorMessage = "{0}不能为空。")]
        [StringLength(20, ErrorMessage = "{0}最大长度为{1}。")]
        public string City { get; set; }

        public int? CountyId { get; set; }
        [StringLength(20, ErrorMessage = "{0}最大长度为{1}。")]
        public string County { get; set; }

        [Display(Name = "地址明细")]
        [Required(ErrorMessage = "{0}不能为空。")]
        [StringLength(100, ErrorMessage = "{0}最大长度为{1}。")]
        public string Address { get; set; }

        [Display(Name = "邮编")]
        [StringLength(10, ErrorMessage = "{0}最大长度为{1}。")]
        public string Zip { get; set; }


    }
}
