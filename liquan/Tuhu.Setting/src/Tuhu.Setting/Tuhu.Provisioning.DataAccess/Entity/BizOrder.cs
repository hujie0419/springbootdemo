using System;
using System.Collections.Generic;
using Tuhu.Component.ExportImport;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    [TableExportImport(Name = "延期订单异常订单")]
    public class BizOrder
    {
        /// <summary>
        /// PKID
        /// </summary>
        [ColumnExportImport(ExportImportName = "订单号")]
        public int PKID { get; set; }
        [ColumnExportImport(ExportImportName = "异常/延期备注")]
        public string RemarkSp { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 采购状态
        /// </summary>
        public int PurchaseStatus { get; set; }

        /// <summary>
        /// 付款金额
        /// </summary>
        [ColumnExportImport(ExportImportName = "总价")]
        public decimal SumMoney { get; set; }

        /// <summary>
        /// 付款状态
        /// </summary>
        public string PayStatus { get; set; }
        [ColumnExportImport(ExportImportName = "付款状态")]

        public string PayStatusValue { get; set; }

        /// <summary>
        /// 已付金额(订单表里面的sumPaid值)
        /// </summary>
        public decimal SumPaid { get; set; }

        /// <summary>
        /// 付款类型
        /// </summary>
        public string PaymentType { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public string PayMothed { get; set; }

        /// <summary>
        /// 付款时间
        /// </summary>
        public DateTime PayToCPDate { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        public int InstallShopID { get; set; }

        /// <summary>
        /// 门店名字
        /// </summary>
        [ColumnExportImport(ExportImportName = "安装门店")]
        public string InstallShopName { get; set; }
        /// <summary>
        /// 订单所在的省份
        /// </summary>
        [ColumnExportImport(ExportImportName = "省")]
        public string ProviceRegionName { get; set; }
        /// <summary>
        /// 订单所在的市
        /// </summary>
        [ColumnExportImport(ExportImportName = "市")]
        public string CityRegionName { get; set; }
        /// <summary>
        /// 订单所在的区
        /// </summary>
        [ColumnExportImport(ExportImportName = "区")]
        public string DistrictRegionName { get; set; }
        /// <summary>
        /// 门店负责人(中文)
        /// </summary>
        [ColumnExportImport(ExportImportName = "门店负责人")]
        public string ShopResponsiblePerson { get; set; }
        /// <summary>
        /// 门店负责人(邮箱)
        /// </summary>
        public string ResponsiblePerson { get; set; }

        /// <summary>
        /// 订单所在的市的PKID
        /// </summary>
        public int? RegionId { get; set; }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public int WareHouseId { get; set; }

        /// <summary>
        /// 仓库名字
        /// </summary>
        public string WareHouseName { get; set; }

        /// <summary>
        /// 快递公司
        /// </summary>
        public string DeliveryCompany { get; set; }

        /// <summary>
        /// 配送方式
        /// </summary>
        public string DeliveryType { get; set; }

        /// <summary>
        /// 配送状态
        /// </summary>
        public string DeliveryStatus { get; set; }

        public string IDeliveryStatus { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime OrderDatetime { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// 付款方式（中文）
        /// </summary>
        public string iPayMothed { get; set; }

        /// <summary>
        /// 门店核对备注
        /// </summary>
        public string CheckComment { get; set; }

        /// <summary>
        /// 店面核对时间
        /// </summary>
        public DateTime? DianMianCheckDateTime { get; set; }

        /// <summary>
        /// pos单号
        /// </summary>
        public string TranRefNum { get; set; }

        /// <summary>
        /// 途虎处理备注
        /// </summary>
        public string HandleComment { get; set; }

        /// <summary>
        /// 红冲订单的源订单
        /// </summary>
        public int? ParentOrderId { get; set; }

        /// <summary>
        /// 红冲订单的源订单订单号
        /// </summary>
        public string ParentOrderNo { get; set; }

        /// <summary>
        /// 轮胎订单的源订单号
        /// </summary>
        public string TireOrderNo { get; set; }

        /// <summary>
        /// 门店核对状态
        /// </summary>
        public bool? CheckMark { get; set; }

        /// <summary>
        /// 安装时间
        /// </summary>
        public DateTime? InstallDatetime { get; set; }

        /// <summary>
        /// 订单备注
        /// </summary>
        [ColumnExportImport(ExportImportName = "备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 途虎处理结果
        /// </summary>
        public bool? IsHandle { get; set; }

        /// <summary>
        /// 安装费(所有orderList installFee相加)
        /// </summary>
        public decimal InstallFee { get; set; }

        /// <summary>
        /// 服务费(所有服务orderList cost相加)
        /// </summary>
        public decimal ServiceFee { get; set; }

        /// <summary>
        /// 产品详情(产品名字name + 产品数量num)
        /// </summary>
        public string ProductDetails { get; set; }

        /// <summary>
        /// 订单核对情况（中文）
        /// </summary>
        public string OrderMark { get; set; }

        /// <summary>
        /// 收现金(付款方式是现金，订单总金额SumMoney减去订单已经付款金额SumPaid)
        /// </summary>
        public decimal CashMoney { get; set; }

        /// <summary>
        /// 扣其他费用(订单表里面OtherPayShop字段值)
        /// </summary>
        public decimal OtherPayShop { get; set; }

        /// <summary>
        /// 店面核对状态
        /// </summary>
        public bool? DianMianCheckStatus { get; set; }

        /// <summary>
        /// 店面核对备注
        /// </summary>
        public string DianMianCheckRemark { get; set; }

        /// <summary>
        /// 途虎核对备注
        /// </summary>
        public string TuHuCheckRemark { get; set; }

        /// <summary>
        /// 途虎核对状态
        /// </summary>
        public int TuHuCheckStatus { get; set; }

        /// <summary>
        /// 途虎核对时间
        /// </summary>
        public DateTime? TuHuCheckDate { get; set; }

        /// <summary>
        /// 途虎核对时间
        /// </summary>
        public string TuHuCheckBy { get; set; }

        /// <summary>
        /// 结算状态
        /// </summary>
        public bool? Reconciliation { get; set; }

        /// <summary>
        /// 结算时间
        /// </summary>
        public DateTime? ReconciliationTime { get; set; }

        /// <summary>
        /// 车主姓名
        /// </summary>
        [ColumnExportImport(ExportImportName = "用户名")]
        public string UserName { get; set; }

        /// <summary>
        /// 车牌
        /// </summary>
        public string CarPlate { get; set; }
        /// <summary>
        /// 车牌ID
        /// </summary>
        public string CarID { get; set; }

        /// <summary>
        /// 坐席编号
        /// </summary>
        [ColumnExportImport(ExportImportName = "主人")]
        public string Owner { get; set; }

        /// <summary>
        /// 预约时间
        /// </summary>
        [ColumnExportImport(ExportImportName = "预约安装时间")]
        public DateTime BookDatetime { get; set; }

        /// <summary>
        /// 预约时间
        /// </summary>
        public string BookPeriod { get; set; }

        /// <summary>
        /// 安装门店
        /// </summary>
        public string CarparName { get; set; }

        /// <summary>
        /// 安装门店
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 门店
        /// </summary>
        public string SimpleName { get; set; }

        /// <summary>
        /// 车型
        /// </summary>
        public string CarType { get; set; }

        /// <summary>
        /// 税费
        /// </summary>
        public decimal InvoiceAddTax { get; set; }

        /// <summary>
        /// 运费
        /// </summary>
        public decimal ShippingMoney { get; set; }

        /// <summary>
        /// 安装费
        /// </summary>
        public decimal InstallMoney { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int SumNum { get; set; }

        /// <summary>
        /// 订单渠道
        /// </summary>
        public string OrderChannel { get; set; }

        /// <summary>
        /// 安装类型
        /// </summary>
        public string InstallType { get; set; }
        [ColumnExportImport(ExportImportName = "安装类型")]

        public string InstallTypeValue { get; set; }

        public decimal? FeePerTire { get; set; }
        public decimal? FeePerFrontBrake { get; set; }
        public decimal? FeePerRearBrake { get; set; }
        public decimal? FeePerFrontDisc { get; set; }
        public decimal? FeePerRearDisc { get; set; }
        public decimal? FeePerMaintain { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string UserTel { get; set; }
        public string u_region_name { get; set; }
        public string u_address_line1 { get; set; }
        public string DeliveryCode { get; set; }
        public string InvoiceDeliveryCode { get; set; }
        public decimal? DeliveryFee { get; set; }
        [ColumnExportImport(ExportImportName = "到店日期")]
        public DateTime? DeliveryDate { get; set; }

        public List<OrderList> OrderLists { get; set; }

      
        /// <summary>
        /// 订单状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 订单状态中文
        /// </summary>
        [ColumnExportImport(ExportImportName = "订单状态")]
        public string StatusValue { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime? SubmitDate { get; set; }

        /// <summary>
        /// 配送类型中文
        /// </summary>
        [ColumnExportImport(ExportImportName = "配送类型")]
        public string DeliveryTypeValue { get; set; }

        /// <summary>
        /// 配送状态
        /// </summary>
        [ColumnExportImport(ExportImportName = "配送状态")]
        public string DeliveryStatusValue { get; set; }

        /// <summary>
        /// 取消日期
        /// </summary>
        public DateTime? CanceledDateTime { get; set; }

        /// <summary>
        /// 提交人
        /// </summary>
        public string Submitor { get; set; }

        /// <summary>
        /// 提交人名字
        /// </summary>
        public string SubmitorName { get; set; }

        /// <summary>
        /// 主人名字
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// 安装状态（中文）
        /// </summary>
        public string InstallStatusValue { get; set; }

        /// <summary>
        /// 外联单号
        /// </summary>
        public string Refno { get; set; }

        /// <summary>
        /// 是否催单
        /// </summary>
        public bool? IsReminder { get; set; }

        /// <summary>
        /// 轮胎数量
        /// </summary>
        public int TireNum { get; set; }

        /// <summary>
        /// 配送状态修改人
        /// </summary>
        public string DeliveryConfirmor { get; set; }

        public int CommentStatus { get; set; }

        public Guid? UserId { get; set; }
        public DateTime? OutStockDateTime { get; set; }

        /// <summary>
        /// 物流任务PKID
        /// </summary>
        public int LogisticTaskId { get; set; }

        /// <summary>
        /// 物流任务状态
        /// </summary>
        public string LogisticTaskStatus { get; set; }

        /// <summary>
        /// 产品列表
        /// </summary>
        [ColumnExportImport(ExportImportName = "产品详情")]
        public string OrderProducts { get; set; }

        public int? LogisticStockId { get; set; }

        public string LogisticStockName { get; set; }

        public int? TuhuStockId { get; set; }
        public string TuhuStockName { get; set; }

        /// <summary>
        /// 门店的市的名称
        /// </summary>
        public string ShopRegionName { get; set; }

        public string InvoiceTitle { get; set; }
        public string InvoiceType { get; set; }
        public string InvoiceStatus { get; set; }
        public string InvAddress { get; set; }
        public decimal InvTaxNum { get; set; }
        public string  InvTaxNumNew { get; set; }
        public string InvBank { get; set; }
        public string InvBankAccount { get; set; }
        public decimal InvAmont { get; set; }
        public string DeliveryAddressID { get; set; }

        /// <summary>
        /// 打印状态
        /// </summary>
        public bool? OrderProcessed { get; set; }

        /// <summary>
        /// 安装状态
        /// </summary>
        public string InstallStatus { get; set; }

        /// <summary>
        /// tbl_EndUserCase
        /// </summary>
        public string CaseId { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? LastUpdateTime { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        public decimal SumMarkedMoney { get; set; }

        /// <summary>
        /// 总折扣
        /// </summary>
        public decimal SumDisMoney { get; set; }

        /// <summary>
        /// 优惠价格
        /// </summary>
        public decimal PromotionMoney { get; set; }

        /// <summary>
        /// 收现金
        /// </summary>
        public decimal PayCashMoney { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public string Auditor { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditDatetime { get; set; }

        /// <summary>
        /// 安装单关联订单
        /// </summary>
        public string InstallOrderNo { get; set; }

        public int InstallOrderId { get; set; }

        /// <summary>
        /// 付款信息
        /// </summary>
        public string PayInfo { get; set; }

        /// <summary>
        /// 付款类型中文
        /// </summary>
        public string PaymentTypeValue { get; set; }

        /// <summary>
        /// 付款方式中文
        /// </summary>
        [ColumnExportImport(ExportImportName = "付款方式")]
        public string PayMothedValue { get; set; } 

        public AddressObjects addressObj { get; set; }

        [ColumnExportImport(ExportImportName = "异常原因")]
        public string AbnormalRemark { get; set; }

        /// <summary>
        /// 取消原因一级目录
        /// </summary>
        public string FirstMenus { get; set; }

        /// <summary>
        /// 取消原因二级目录
        /// </summary>
        public string SecondMenus { get; set; }

        /// <summary>
        /// 取消原因创建人
        /// </summary>
        public string CanceledCreateByName { get; set; }

        /// <summary>
        /// 取消原因备注
        /// </summary>
        public string OtherReason { get; set; }

        /// <summary>
        /// 到帐日期
        /// </summary>
        public DateTime? GotPayDate { get; set; }

        public List<string> CancelOrders { get; set; }
		public List<OrderRelationship> OrderRelationships { get; set; }
        [ColumnExportImport(ExportImportName = "异常/延期时间")]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 日志记录
        /// </summary>
        public string Operation { get; set; }
        /// <summary>
        /// 预约发货日期
        /// </summary>
        public DateTime? BookSentDateTime { get; set; }
        /// <summary>
        /// 渠道值
        /// </summary>
        public string iOrderChannel { get; set; }
        //
        public string Abst { get; set; }
        public DateTime? DeliveryDatetime { get; set; } 


        /// <summary>
        /// 门店电话
        /// </summary>
        public string Telephone { get; set; }
        /// <summary>
        /// 老板电话
        /// </summary>
        public string Mobile { get; set; }

        //电子签购单拼html拼
        public string TranPicUrlHtml { get; set; }
		public string TranPicUrlBig { get; set; }
		public string TranPicUrlSmall { get; set; }
        public string InterceptType { get; set; }
        public string Province { get; set; }
        public string City { get; set; }

		//pos机类别
		public string DataType { get; set; }

        public int TotalCount { get; set; }

		public string DeliveryPoint { get; set; }

		#region 车型信息
		public string Brand { get; set; }
        public string Vehicle { get; set; }
        public string PaiLiang { get; set; }
        public string Nian { get; set; }
        public string SalesName { get; set; }

        #endregion

        //产品成本，不包含服务和保险
        //public decimal? SumProCost { get; set; }
    }
}
