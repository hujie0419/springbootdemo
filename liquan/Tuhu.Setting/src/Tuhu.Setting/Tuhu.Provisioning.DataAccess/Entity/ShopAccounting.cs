using System;
using Tuhu.Component.ExportImport;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    [TableExportImport(ExportImportName = "门店列表")]
    public class ShopAccounting
    {
        /// <summary>
        /// 序号 递增
        /// </summary>
        public int PKID { get; set; }
         [ColumnExportImport(ExportImportName = "核对门店编号")]
        /// <summary>
        /// 核对门店编号
        /// </summary>
        public int ShopID { get; set; }
         [ColumnExportImport(ExportImportName = "门店简单名")]
         /// <summary>
         /// 门店简单名
         /// </summary>
         public string SimpleName { get; set; }
         [ColumnExportImport(ExportImportName = "省")]
         /// <summary>
         /// 省
         /// </summary>
         public string Province { get; set; }
         [ColumnExportImport(ExportImportName = "市")]
         /// <summary>
         /// 市
         /// </summary>
         public string City { get; set; }
         [ColumnExportImport(ExportImportName = "区")]
         /// <summary>
         /// 区
         /// </summary>
         public string District { get; set; }
         [ColumnExportImport(ExportImportName = "周次")]
         public string WeeklyStr { get; set; }

        [ColumnExportImport(ExportImportName = "核定对账日")]
        /// <summary>
        /// 核定对账周期 星期几
        /// </summary>
        public string CheckWeekDay { get; set; }
        [ColumnExportImport(ExportImportName = "当周核定对账日期")]
        /// <summary>
        /// 当周核定对账日期
        /// </summary>
        public DateTime CheckDate { get; set; }
        
        /// <summary>
        /// 周次 第几周
        /// </summary>
        public int Weekly { get; set; }
        [ColumnExportImport(ExportImportName = "实际对账时间")]
        /// <summary>
        /// 实际对账时间
        /// </summary>
        public DateTime? AccountingCheckDate { get; set; }
         [ColumnExportImport(ExportImportName = "结算总金额")]
        /// <summary>
        /// 已核对对账单结算总金额
        /// </summary>
        public decimal BalanceSumMoney { get; set; }
        /// <summary>
        /// 结算备注
        /// </summary>
        public string BalanceRemark { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string Operator { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime? OperateDateTime { get; set; }

        /// <summary>
        /// 其他备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 是否删除 假删除（当月门店重新设置对账周期并和之前设置的不一样时会假删除）
        /// </summary>
        public bool IsDeleted { get; set; }
        
        
        [ColumnExportImport(ExportImportName = "未对账订单统计")]
        /// <summary>
        /// 未对账订单统计
        /// </summary>
        public int UncheckedCount { get; set; }
        [ColumnExportImport(ExportImportName = "门店核对正确订单统计")]
        /// <summary>
        /// 门店核对正确订单统计
        /// </summary>
        public int CheckedCount { get; set; }
        [ColumnExportImport(ExportImportName = "途虎核对正确订单统计")]
        /// <summary>
        /// 途虎核对正确订单统计
        /// </summary>
        public int TuhuCheckedCount { get; set; }
        [ColumnExportImport(ExportImportName = "待结算批次统计")]
        /// <summary>
        /// 待结算批次统计
        /// </summary>
        public int UnPayOffCount { get; set; }
        [ColumnExportImport(ExportImportName = "正在配送的订单统计")]
        /// <summary>
        /// 正在配送的订单统计
        /// </summary>
        public int NotSignedCount { get; set; }
        [ColumnExportImport(ExportImportName = "需退回订单统计")]

        /// <summary>
        /// 需退回订单统计
        /// </summary>
        public int CallBackCount { get; set; }
        [ColumnExportImport(ExportImportName = "区域对账人")]

        /// <summary>
        /// 对账人
        /// </summary>
        public string AccountingPerson { get; set; }
        /// <summary>
        /// 上下架状态
        /// </summary>
        public bool IsActive { get; set; }
        [ColumnExportImport(ExportImportName = "上下架状态")]

        /// <summary>
        /// 上下架状态
        /// </summary>
        public string IsActiveStr { get; set; }
		[ColumnExportImport(ExportImportName = "营业状态")]
		/// <summary>
		/// 营业状态
		/// </summary>
		public string shopIsActiveType { get; set; }
        /// <summary>
        /// 对账人设置时间
        /// </summary>
        public DateTime UpdateAccountingPeriodTime { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public string ResponsiblePerson { get; set; }
        /// <summary>
        /// 负责人手机
        /// </summary>
        public string ResponsiblePersonTel { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }
        [ColumnExportImport(ExportImportName = "门店确认付款日期")]
        public string SList { get; set; }
    }
	 [TableExportImport(ExportImportName = "门店列表")]
	public class ShopAccountingHistory
	{
		/// <summary>
		/// 序号 递增
		/// </summary>
		public int PKID { get; set; }
		[ColumnExportImport(ExportImportName = "核对门店编号")]
		/// <summary>
		/// 核对门店编号
		/// </summary>
		public int ShopID { get; set; }
		[ColumnExportImport(ExportImportName = "门店简单名")]
		/// <summary>
		/// 门店简单名
		/// </summary>
		public string SimpleName { get; set; }
		[ColumnExportImport(ExportImportName = "省")]
		/// <summary>
		/// 省
		/// </summary>
		public string Province { get; set; }
		[ColumnExportImport(ExportImportName = "市")]
		/// <summary>
		/// 市
		/// </summary>
		public string City { get; set; }
		[ColumnExportImport(ExportImportName = "区")]
		/// <summary>
		/// 区
		/// </summary>
		public string District { get; set; }
		[ColumnExportImport(ExportImportName = "周次")]
		public string WeeklyStr { get; set; }

		[ColumnExportImport(ExportImportName = "核定对账日")]
		/// <summary>
		/// 核定对账周期 星期几
		/// </summary>
		public string CheckWeekDay { get; set; }
		[ColumnExportImport(ExportImportName = "当周核定对账日期")]
		/// <summary>
		/// 当周核定对账日期
		/// </summary>
		public DateTime CheckDate { get; set; }

		/// <summary>
		/// 周次 第几周
		/// </summary>
		public int Weekly { get; set; }
		[ColumnExportImport(ExportImportName = "实际对账时间")]
		/// <summary>
		/// 实际对账时间
		/// </summary>
		public DateTime? AccountingCheckDate { get; set; }
		[ColumnExportImport(ExportImportName = "结算总金额")]
		/// <summary>
		/// 已核对对账单结算总金额
		/// </summary>
		public decimal BalanceSumMoney { get; set; }
		/// <summary>
		/// 结算备注
		/// </summary>
		public string BalanceRemark { get; set; }

		/// <summary>
		/// 操作人
		/// </summary>
		public string Operator { get; set; }
		/// <summary>
		/// 操作时间
		/// </summary>
		public DateTime? OperateDateTime { get; set; }

		/// <summary>
		/// 其他备注
		/// </summary>
		public string Remark { get; set; }

		/// <summary>
		/// 是否删除 假删除（当月门店重新设置对账周期并和之前设置的不一样时会假删除）
		/// </summary>
		public bool IsDeleted { get; set; }

		[ColumnExportImport(ExportImportName = "区域对账人")]

		/// <summary>
		/// 对账人
		/// </summary>
		public string AccountingPerson { get; set; }
		/// <summary>
		/// 上下架状态
		/// </summary>
		public bool IsActive { get; set; }
		[ColumnExportImport(ExportImportName = "上下架状态")]

		/// <summary>
		/// 上下架状态
		/// </summary>
		public string IsActiveStr { get; set; }
		/// <summary>
		/// 对账人设置时间
		/// </summary>
		public DateTime UpdateAccountingPeriodTime { get; set; }
		/// <summary>
		/// 负责人
		/// </summary>
		public string ResponsiblePerson { get; set; }
		/// <summary>
		/// 负责人手机
		/// </summary>
		public string ResponsiblePersonTel { get; set; }

		/// <summary>
		/// 总记录数
		/// </summary>
		public int TotalCount { get; set; }
		
		public string SList { get; set; }
		[ColumnExportImport(ExportImportName = "结算批次")]

		public string PayOffID { get; set; }
		[ColumnExportImport(ExportImportName = "门店确认付款日期")]

		public DateTime? ConfirmPayDate { get; set; }

	}

	 [TableExportImport(ExportImportName = "门店列表")]
	 public class ExportShopAccounting
	 {
		 /// <summary>
		 /// 序号 递增
		 /// </summary>
		 public int PKID { get; set; }
		 [ColumnExportImport(ExportImportName = "核对门店编号")]
		 /// <summary>
		 /// 核对门店编号
		 /// </summary>
		 public int ShopID { get; set; }
		 [ColumnExportImport(ExportImportName = "门店简单名")]
		 /// <summary>
		 /// 门店简单名
		 /// </summary>
		 public string SimpleName { get; set; }
		 [ColumnExportImport(ExportImportName = "省")]
		 /// <summary>
		 /// 省
		 /// </summary>
		 public string Province { get; set; }
		 [ColumnExportImport(ExportImportName = "市")]
		 /// <summary>
		 /// 市
		 /// </summary>
		 public string City { get; set; }
		 [ColumnExportImport(ExportImportName = "区")]
		 /// <summary>
		 /// 区
		 /// </summary>
		 public string District { get; set; }
		 [ColumnExportImport(ExportImportName = "周次")]
		 public string WeeklyStr { get; set; }

		 [ColumnExportImport(ExportImportName = "核定对账日")]
		 /// <summary>
		 /// 核定对账周期 星期几
		 /// </summary>
		 public string CheckWeekDay { get; set; }
		 [ColumnExportImport(ExportImportName = "当周核定对账日期")]
		 /// <summary>
		 /// 当周核定对账日期
		 /// </summary>
		 public DateTime CheckDate { get; set; }

		 /// <summary>
		 /// 周次 第几周
		 /// </summary>
		 public int Weekly { get; set; }
		 [ColumnExportImport(ExportImportName = "实际对账时间")]
		 /// <summary>
		 /// 实际对账时间
		 /// </summary>
		 public DateTime? AccountingCheckDate { get; set; }
		 [ColumnExportImport(ExportImportName = "结算总金额")]
		 /// <summary>
		 /// 已核对对账单结算总金额
		 /// </summary>
		 public decimal BalanceSumMoney { get; set; }
		 /// <summary>
		 /// 结算备注
		 /// </summary>
		 public string BalanceRemark { get; set; }

		 /// <summary>
		 /// 操作人
		 /// </summary>
		 public string Operator { get; set; }
		 /// <summary>
		 /// 操作时间
		 /// </summary>
		 public DateTime? OperateDateTime { get; set; }

		 /// <summary>
		 /// 其他备注
		 /// </summary>
		 public string Remark { get; set; }

		 /// <summary>
		 /// 是否删除 假删除（当月门店重新设置对账周期并和之前设置的不一样时会假删除）
		 /// </summary>
		 public bool IsDeleted { get; set; }


		 /// <summary>
		 /// 未对账订单统计
		 /// </summary>
		 public int UncheckedCount { get; set; }
		 /// <summary>
		 /// 门店核对正确订单统计
		 /// </summary>
		 public int CheckedCount { get; set; }
		 /// <summary>
		 /// 途虎核对正确订单统计
		 /// </summary>
		 public int TuhuCheckedCount { get; set; }
		 /// <summary>
		 /// 待结算批次统计
		 /// </summary>
		 public int UnPayOffCount { get; set; }
		 /// <summary>
		 /// 正在配送的订单统计
		 /// </summary>
		 public int NotSignedCount { get; set; }

		 /// <summary>
		 /// 需退回订单统计
		 /// </summary>
		 public int CallBackCount { get; set; }
		 [ColumnExportImport(ExportImportName = "区域对账人")]

		 /// <summary>
		 /// 对账人
		 /// </summary>
		 public string AccountingPerson { get; set; }
		 /// <summary>
		 /// 上下架状态
		 /// </summary>
		 public bool IsActive { get; set; }
		 [ColumnExportImport(ExportImportName = "上下架状态")]
		 /// <summary>
		 /// 上下架状态
		 /// </summary>
		 public string IsActiveStr { get; set; }
		 [ColumnExportImport(ExportImportName = "营业状态")]
		 /// <summary>
		 /// 营业状态
		 /// </summary>
		 public string shopIsActiveType { get; set; }
		 /// <summary>
		 /// 对账人设置时间
		 /// </summary>
		 public DateTime UpdateAccountingPeriodTime { get; set; }
		 /// <summary>
		 /// 负责人
		 /// </summary>
		 public string ResponsiblePerson { get; set; }
		 /// <summary>
		 /// 负责人手机
		 /// </summary>
		 public string ResponsiblePersonTel { get; set; }

		 /// <summary>
		 /// 总记录数
		 /// </summary>
		 public int TotalCount { get; set; }
		 public string SList { get; set; }
		 [ColumnExportImport(ExportImportName = "结算批次")]

		 public string PayOffID { get; set; }
		 [ColumnExportImport(ExportImportName = "门店确认付款日期")]

		 public DateTime? ConfirmPayDate { get; set; }
	 }
}
