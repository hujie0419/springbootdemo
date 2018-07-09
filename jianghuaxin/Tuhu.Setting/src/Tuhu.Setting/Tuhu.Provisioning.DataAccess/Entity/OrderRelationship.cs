using Tuhu.Component.ExportImport;

namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class OrderRelationship//订单关系类
    {
        /// <summary>
        /// 原订单
        /// </summary>
        public int ParentOrderId { get; set; }

        /// <summary>
        /// 子订单
        /// </summary>
        public int ChildrenOrderId { get; set; }

        /// <summary>
        /// 关系类型  1:部分取消关系，2：红冲关系，3：补安装单 4：拆单关系表,5:补礼品单
        /// </summary>
        public OrderRelationshipType.OrderRelationshipTypeEnum RelationshipType { get; set; }

        /// <summary>
        /// 关系记录
        /// </summary>
        public string Relationship{get;set;}

    }


	[TableExportImport(ExportImportName = "产品列表")]
	public class RelationshipOrder
	{
		[ColumnExportImport(ExportImportName = "原订单号")]
		/// <summary>
		/// 原订单号
		/// </summary>
		public int parentOrderId { get; set; }

		[ColumnExportImport(ExportImportName = "补发订单号")]
		/// <summary>
		/// 补发订单号
		/// </summary>
		public int PKID { get; set; }
		
		[ColumnExportImport(ExportImportName = "产品编号")]
		/// <summary>
		/// 产品编号
		/// </summary>
		public string pid { get; set; }

		[ColumnExportImport(ExportImportName = "数量")]
		/// <summary>
		/// 数量
		/// </summary>
		public int num { get; set; }

		[ColumnExportImport(ExportImportName = "总成本")]
		/// <summary>
		/// 总成本
		/// </summary>
		public decimal cost { get; set; }
		[ColumnExportImport(ExportImportName = "产品名称")]
		/// <summary>
		/// 产品名称
		/// </summary>
		public string Name { get; set; }

		[ColumnExportImport(ExportImportName = "门店名")]
		/// <summary>
		/// 门店名
		/// </summary>
		public string simplename { get; set; }
		public int installshopId { get; set; }
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

		[ColumnExportImport(ExportImportName = "拓展负责人")]
		/// <summary>
		/// 拓展负责人
		/// </summary>
		public string ResponsiblePerson { get; set; }

		[ColumnExportImport(ExportImportName = "管控负责人")]
		/// <summary>
		/// 管控负责人
		/// </summary>
		public string AccountingPerson { get; set; }
		[ColumnExportImport(ExportImportName = "制单人")]
		/// <summary>
		/// 制单人
		/// </summary>
		public string submitor { get; set; }

		[ColumnExportImport(ExportImportName = "订单备注")]
		/// <summary>
		/// 订单备注
		/// </summary>
		public string Remark { get; set; }
		
		[ColumnExportImport(ExportImportName = "订单列表备注")]
		/// <summary>
		/// 订单列表备注
		/// </summary>
		public string remark2 { get; set; }

		/// <summary>
		/// 记录总数
		/// </summary>
		public int TotalCount { get; set; }
	}
}
